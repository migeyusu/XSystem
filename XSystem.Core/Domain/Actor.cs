using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Xunit;

namespace XSystem.Core.Domain
{
    public class Actor : Model
    {
        public string ShotTag { get; set; }

        public string ShotUrl { get; set; }

        /// <summary>
        /// 可抓取该actor对应的film的search url
        /// 由于searchurl往往以一定规则生成，所以不持久化
        /// </summary>
        [NotMapped]
        public string FilmSearchUrl {
            get {
                switch (Region) {
                    case Region.Dmm:
                        return $"http://www.dmm.co.jp/mono/dvd/-/actress/=/keyword={Code}/";
                    case Region.Mgs:
                        return null;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// 指示源url，可能来源于不同类型的页面（同类型页面或来自其他类型）
        /// </summary>
        public string SourceUrl { get; set; }
        
        /// <summary>
        /// dmm的actor search id
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 抓取域,指示在哪个网站抓取
        /// </summary>
        public Region Region { get; set; }

        /// <summary>
        /// 表示已抓取过，如果在抓取过程中网络异常不会完成
        /// </summary>
        public bool IsInitialAccomplish { get; set; }
        /// <summary>
        /// 最近更新时间
        /// </summary>
        public DateTime LastUpdateTime { get; set; }

        public virtual List<Film> Films { get; set; }
        
        private readonly object _filmsLocker = new object();

        //public ConcurrentDictionary<string,bool> NeedSearchUrls { get; set; }

        public Actor()
        {
            Films = new List<Film>();
        }

        //public void SetNeedFetchUrls(IEnumerable<string> urls)
        //{
        //    _needSearchUrls =
        //        new ConcurrentDictionary<string, bool>(urls.Select(s => new KeyValuePair<string, bool>(s, false)));
        //}

        //public void AccomplishedUrl(string url)
        //{
        //    _needSearchUrls.TryRemove(url, out bool value);
        //}

        public void AddFilm(Film film)
        {
            lock (_filmsLocker) {
                Films.Add(film);
            }
        }

        public void AddFilmRange(IEnumerable<Film> films)
        {
            lock (_filmsLocker) {
                Films.AddRange(films);
            }
        }

        protected bool Equals(Actor other)
        {
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Actor) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}