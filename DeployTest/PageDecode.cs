using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Reptile;
using XSystem.Core.Domain;

namespace DeployTest {
    public class PageDecode
    {
        public const string UndefinedSeries = "UndefinedSeries";

        public const string UndefinedActor = "UndefinedActor";

        public const string UndefinedPublisher = "UndefinedPublisher";

        private readonly Regex _dmmmakerRegex =
            new Regex(@"<a href=""/mono/dvd/-/list/=/article=maker/id=[0-9]+?/"">(.+?)</a>");

        private readonly Regex _dmmnameRegex = new Regex(@"<h1 id=""title"" class=""item fn"">(.*?)</h1>");

        private readonly Regex _dmmGroupRegex =
            new Regex(@"<td align=""right"" valign=""top"" class=""nw"">�����룺</td>([\s\S]+?)</tr>");

        private readonly Regex _dmmCharaRegex =
            new Regex(@"<a href=""/mono/dvd/-/list/=/article=keyword/id=[0-9]+?/"">(.+?)</a>");

        private readonly Regex _dmmActorNameRegex =
            new Regex(@"<a href=""/mono/dvd/-/list/=/article=actress/id=([0-9]+?)/"">(.+?)</a>");

        private readonly Regex _dmmSeriesRegex =
            new Regex(@"<a href=""/mono/dvd/-/list/=/article=series/id=[0-9]+/"">(.+?)</a>");

        private readonly Regex _dmmItemRegex =
            new Regex(@"<a href=""http://www.dmm.co.jp/mono/dvd/-/detail/=/cid=([\w]+?)/"">");

        private readonly Regex _dmmActorListRegex = new Regex(@"<a href=""(/mono/dvd/-/actress/=/keyword=[a-z]+?/)"">");

        private readonly Regex _dmmActorItemPageRegex = new Regex(@"/page=([0-9]+?)/"">");

        private readonly Regex _dmmActorItemRegex =
                new Regex(
                    @"<li><a href=""(/mono/dvd/-/list/=/article=actress/id=[0-9]+?/)""><img src=""(http://pics.dmm.co.jp/mono/actjpgs/medium/\w+?.jpg)"" alt="""" width=""100"" height=""100""><br>(.+?)</a></li>")
            ;

        #region actorsinitial

        private volatile bool _actorlistpagesdecoded = false;

        //��Ҫ����url���ϵ�������ҳ��
        private readonly ConcurrentBag<string> _accomplishedactordic = new ConcurrentBag<string>();

        public ConcurrentBag<Actor> FetchedNewActors { get; set; } = new ConcurrentBag<Actor>();

        public void DmmActorParse(Page page, ReptileContext context)
        {
            if (page.Url.StartsWith("http://www.dmm.co.jp/mono/dvd/-/actress/=/keyword")) {
                if (!_actorlistpagesdecoded) {
                    //����ҳ��ֻ��һ��
                    _actorlistpagesdecoded = true;
                    var enumerable = _dmmActorListRegex
                        .Matches(page.Content)
                        .Cast<Match>()
                        .Select(match1 => $"http://www.dmm.co.jp{match1.Groups[1].Value}")
                        .Distinct();
                    foreach (var s in enumerable) {
                        _accomplishedactordic.Add(s);
                    }
                    foreach (var s in enumerable) {
                        context.AddUrl(s);
                    }
                    return;
                }
                //ÿ������ֻץȡһ��
                if (_accomplishedactordic.Contains(page.Url)) {
                    ParseAllUrls(page, context);
                }
                var actors = _dmmActorItemRegex.Matches(page.Content)
                    .Cast<Match>()
                    .Select(match => {
                        var url = $"http://www.dmm.co.jp{match.Groups[1].Value}";
                        var index = url.IndexOf("id") + 3;
                        return new Actor {
                            FilmSearchUrl = url,
                            ShotUrl = match.Groups[2].Value,
                            Name = match.Groups[3].Value,
                            Code = int.Parse(url.Substring(index, url.Length - index - 1)),
                            LastUpdateTime = DateTime.Now,
                            SourceUrl = page.Url
                        };
                    });
                foreach (var actor in actors) {
                    /* ��ʼ���׶Σ�dmmҳ������ʾ�������⣬���ܻ��ظ�
                     * 
                     */

                    if (AllActorsDictionary.TryAdd(actor.Code, actor)) {
                        FetchedNewActors.Add(actor);
                    }
                }
                return;
            }
        }

        #endregion

        private void ParseAllUrls(Page page, ReptileContext context)
        {
            var matchCollection = _dmmActorItemPageRegex.Matches(page.Content);
            if (matchCollection.Count > 0) {
                var max = matchCollection.Cast<Match>()
                    .Max(match => int.Parse(match.Groups[1].Value));
                for (var i = 2; i <= max; i++) {
                    context.AddUrl($"{page.Url}page={i}/");
                }
            }
        }

        public ConcurrentBag<Publisher> FetchedNewPublishers { get; set; } = new ConcurrentBag<Publisher>();

        public ConcurrentBag<Film> FetchedNewFilms { get; set; } = new ConcurrentBag<Film>();

        /// <summary>
        /// ��Ŵ�������actors������URL
        /// </summary>
        public ConcurrentBag<string> SeedFetchUrls { get; set; }

        public ConcurrentDictionary<int, Actor> AllActorsDictionary { get; set; } =
            new ConcurrentDictionary<int, Actor>();

        public ConcurrentDictionary<string, Publisher> AllPublishersDictionary { get; set; } =
            new ConcurrentDictionary<string, Publisher>();

        /// <summary>
        /// key:code because code is smaller
        /// </summary>
        public ConcurrentDictionary<string, Film> AllFilms { get; set; } = new ConcurrentDictionary<string, Film>();

        /// <summary>
        /// �б�ģʽ��ȡfilm��film�����ظ���Ҳ�������µ�actor���������������߸��µ�����
        /// </summary>
        /// <param name="page"></param>
        /// <param name="context"></param>
        public void DmmFilmsParse(Page page, ReptileContext context)
        {
            if (page.Url.StartsWith("http://www.dmm.co.jp/mono/dvd/-/list/")) {
                if (SeedFetchUrls.Contains(page.Url)) {
                    ParseAllUrls(page, context);
                }
                //����ȫ��ȥ�ص�url
                var filmItemsUrl = _dmmItemRegex.Matches(page.Content)
                    .AsParallel()
                    .Cast<Match>()
                    .Select(match => match.Groups[1].Value)
                    //���ò������ϲ���film�ظ��Բ�Ԥ��ռλ��������ҳ����update
                    .Where(s => AllFilms.TryAdd(s,null))
                    .Select(s => $"http://www.dmm.co.jp/mono/dvd/-/detail/=/cid={s}/");
                foreach (var s in filmItemsUrl) {
                    context.AddUrl(s);
                }
                return;
            }
            if (page.Url.StartsWith("http://www.dmm.co.jp/mono/dvd/-/detail/")) {
                var nameValue = _dmmnameRegex.Match(page.Content).Groups[1].Value;
                var makerMatchGroup = _dmmmakerRegex.Match(page.Content).Groups;
                var publisherNameValue = makerMatchGroup.Count == 1 ? UndefinedPublisher : makerMatchGroup[1].Value;
                var publisher = AllPublishersDictionary.GetOrAdd(publisherNameValue,
                    (s => {
                        var publisher1 = new Publisher {Name = publisherNameValue};
                        FetchedNewPublishers.Add(publisher1);
                        return publisher1;
                    }));
                var match1 = _dmmSeriesRegex.Match(page.Content);
                var seriesname = match1.Groups.Count == 1 ? UndefinedSeries : match1.Groups[1].Value;
                var series = publisher.GetOrAddSeries(seriesname);

                //�����µ�actor
                var currentactors = _dmmActorNameRegex.Matches(page.Content)
                    .Cast<Match>()
                    .Select(match => {
                        var actorId = int.Parse(match.Groups[1].Value);
                        return AllActorsDictionary.GetOrAdd(actorId, i => {
                            var actor = new Actor {
                                Code = actorId,
                                Name = match.Groups[2].Value,
                                SourceUrl = page.Url,
                                LastUpdateTime = DateTime.Now,
                                FilmSearchUrl = $"http://www.dmm.co.jp/mono/dvd/-/list/=/article=actress/id={actorId}/"
                            };
                            FetchedNewActors.Add(actor);
                            return actor;
                        });
                    });

                var substring = page.Url.Substring(45, page.Url.Length - 45 - 1);
                var groupValue = _dmmGroupRegex.Match(page.Content).Groups[1].Value;
                var matchCollection = _dmmCharaRegex.Matches(groupValue).Cast<Match>()
                    .Select((match => match.Groups[1].Value));
                var stringBuilder = new StringBuilder();
                foreach (var s in matchCollection) {
                    stringBuilder.Append(s);
                    stringBuilder.Append("\r\n");
                }
                var film = new Film() {
                    Name = nameValue,
                    Code = substring,
                    ImageUrl = $"https://pics.dmm.co.jp/mono/movie/adult/{substring}/{substring}ps.jpg",
                    ShotTag = substring,
                    Characteristic = stringBuilder.ToString(),
                    Actors = currentactors.ToList(),
                    Series = series,
                    SourceUrl = page.Url
                };
                series.AddFilm(film);
                foreach (var actor in currentactors) {
                    actor.AddFilm(film);
                }
                if (AllFilms.TryUpdate(film.Code, film, null)) {
                    FetchedNewFilms.Add(film);
                }
            }
        }
    }
}