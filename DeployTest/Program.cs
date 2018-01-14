#define filmfetch
//#define actorfetch
//#define dmmfetchtest
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;
using Castle.Windsor;
using FORCEBuild.Message.Base;
using XSystem.Core.Infrastructure;
using Reptile;
using XSystem.Core.Domain;
using Xunit;

namespace DeployTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //using (var windsorContainer = new WindsorContainer())
            //{
            //    windsorContainer.Install(new DomainInstaller());
            //    var resolve = windsorContainer.Resolve<Deploy>();
            //    // resolve.RelationChange();
            //    var series = resolve.Load();
            //    Console.WriteLine(series);
            //}
#if filmfetch
            int count = 1;

            var context = new FilmContext();
            context.Configuration.AutoDetectChangesEnabled = false;
            var allactors = context.Actors.ToList();
            var fetchactors = allactors
                .Where(actor => actor.IsInitialAccomplish == false)
                .Take(count);
            var starturls = fetchactors.Select(actor => actor.FilmSearchUrl);
            var pageDecode = new PageDecode {
                SeedFetchUrls = new ConcurrentBag<string>(starturls),
                AllPublishersDictionary = new ConcurrentDictionary<string, Publisher>(
                    context.Publishers
                    .ToList()
                    .Select(
                        publisher => new KeyValuePair<string, Publisher>(publisher.Name, publisher))),
                AllActorsDictionary = new ConcurrentDictionary<int, Actor>(
                    allactors
                    .Select(actor => new KeyValuePair<int, Actor>(actor.Code, actor))),
                AllFilms = new ConcurrentDictionary<string, Film>(
                    context.Films
                    .ToList()
                    .Select(film => new KeyValuePair<string, Film>(film.Code, film)))
            };
            var reptileProcessor = new ReptileProcessor(pageDecode.DmmFilmsParse, 4, 2,
                () => starturls);

            reptileProcessor.InnerEnd += () => {
                reptileProcessor.Stop();
                Thread.Sleep(1000);
                Console.WriteLine(
                    $"{pageDecode.AllFilms.Count}film result collected,{reptileProcessor.ErrorUrl.Count} error urls" +
                    $"fetch spent {reptileProcessor.FetchSpentTime} ms,decode spent {reptileProcessor.DecodeSpentTime} ms");
                foreach (var actor in fetchactors) {
                    actor.IsInitialAccomplish = true;
                }
                context.Films.AddRange(pageDecode.FetchedNewFilms);
                context.Actors.AddRange(pageDecode.FetchedNewActors);
                context.Publishers.AddRange(pageDecode.FetchedNewPublishers);
                context.Configuration.AutoDetectChangesEnabled = true;
                context.SaveChanges();
                context.ReptileHistories.Add(new ReptileHistory {
                    DateTime = DateTime.Now,
                    ErrorPages = reptileProcessor.ErrorUrl
                    .Select((page => FetchErrorPage.Create(page.Url,page.Exception)))
                    .ToList()
                });
                context.SaveChanges();
                Console.WriteLine("result save");
            };
            reptileProcessor.Start();
            var readLine = Console.ReadLine();
            Console.WriteLine(readLine);
            reptileProcessor.Dispose();
            context.Dispose();
#endif
//            using (var filmContext = new FilmContext()) {
//                filmContext.Database.ExecuteSqlCommand("delete from Actors");
////                filmContext.Configuration.AutoDetectChangesEnabled = false;
////                filmContext.Configuration.AutoDetectChangesEnabled = true;
//                filmContext.SaveChanges();
//                Console.WriteLine("done!");
//            }
#if actorfetch

            ActorsGrab();
            
#endif


#if dmmfetchtest

            var webRequest = WebRequest.Create("http://www.dmm.co.jp/mono/dvd/-/actress/=/keyword=wa/");
            using (var webResponse = webRequest.GetResponse())
            {
                using (var responseStream = webResponse.GetResponseStream())
                {
                    using (var streamReader = new StreamReader(responseStream))
                    {
                        var readToEnd = streamReader.ReadToEnd();
                        var regex = new Regex(@"<li><a href=""(/mono/dvd/-/list/=/article=actress/id=[0-9]+?/)""><img src=""(http://pics.dmm.co.jp/mono/actjpgs/medium/\w+?.jpg)"" alt="""" width=""100"" height=""100""><br>(\S+?)</a></li>");
                        var value = regex.Matches(readToEnd);
                        //.Cast<Match>()
                        //.Max(match => int.Parse(match.Groups[1].Value));

                        Console.WriteLine(value.Count);
                    }
                }
            }
#endif

        }

        public static void ActorsGrab()
        {
            var pageDecode = new PageDecode();
            var reptileProcessor = new ReptileProcessor(pageDecode.DmmActorParse, 4,2,
                () => new[] { "http://www.dmm.co.jp/mono/dvd/-/actress/=/keyword=a/" });
            reptileProcessor.InnerEnd += () =>
            {
                Console.WriteLine($"actors collected,{reptileProcessor.ErrorUrl.Count} error urls");
                using (var filmContext = new FilmContext())
                {
                    filmContext.Configuration.AutoDetectChangesEnabled = false;
                    filmContext.Actors.AddRange(pageDecode.FetchedNewActors);
                    filmContext.ReptileHistories.Add(new ReptileHistory()
                    {
                        DateTime = DateTime.Now,
                        ErrorPages = reptileProcessor.ErrorUrl
                            .Select(page => FetchErrorPage.Create(page.Url, page.Exception))
                            .ToList()
                    });
                    filmContext.Configuration.AutoDetectChangesEnabled = true;
                    filmContext.SaveChanges();
                }
                Console.WriteLine("actors saved");
            };
            reptileProcessor.Start();
            var readLine = Console.ReadLine();
            Console.WriteLine(readLine);
            reptileProcessor.Dispose();
        }

    }

    /* 抓取流：抓取所有actors
    * 依据actor获得film列表，并打上标记和更新日期
    * 依据film的detail更新series和Publisher
    * 更新流：首先检查是否出现了新的Actor
    * 然后对检查每个actor的新的film
    */
}
