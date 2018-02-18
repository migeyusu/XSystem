using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Castle.Components.DictionaryAdapter;
using Castle.Core.Internal;
using FORCEBuild.UI.WPF;

namespace XSystem.Core.Domain
{
    public class Film:Model
    {
        public const string ShotDirPath = "FilmShotDir";

        /// <summary>
        /// 用于UI绑定，仅显示前几个
        /// </summary>
        [NotMapped]
        public IEnumerable<Actor> MainActors => Actors?.Take(3);

        public virtual List<Actor> Actors { get; set; }

        public virtual Series Series { get; set; }
        /// <summary>
        /// 绝对地址
        /// </summary>
        public string FileLocation { get; set; }

        [NotMapped]
        public string[] CharacteristicItems {
            get { return Characteristic.Split(new[] {"\r\n"}, StringSplitOptions.None); }
        }
        /// <summary>
        /// 用于持久化
        /// </summary>
        public string Characteristic { get; set; }

        public Region Region { get; set; }
        
        /// <summary>
        /// 来源页面根据规则
        /// </summary>
        [NotMapped]
        public string SourceUrl {
            get {
                switch (Region) {
                    case Region.Dmm:
                        return $"http://www.dmm.co.jp/mono/dvd/-/detail/=/cid={Code}/";
                    case Region.Mgs:
                        return null;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }}

        public DateTime LastUpdateTime { get; set; }

        /// <summary>
        /// 本地图片标识，loading在UI层实现
        /// </summary>
        public string ShotTag { get; set; }

        /// <summary>
        /// 图片url根据规则生成
        /// </summary>
        [NotMapped]
        public string ShotUrl {
            get {
                switch (Region) {
                    case Region.Dmm:
                        if (Code==null) {
                            throw new NullReferenceException("haven't fetch!");
                        }
                        return $"https://pics.dmm.co.jp/mono/movie/adult/{Code}/{Code}ps.jpg";
                    case Region.Mgs:
                        return null;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// 代码
        /// </summary>
        public string Code { get; set; }

        public bool IsOneActor { get; set; }

        public ICommand PlayCommand => new AnotherCommandImplementation((o => {
            Process.Start(FileLocation);
        }),o => !FileLocation.IsNullOrEmpty());
        
        public Film()
        {
            Actors = new List<Actor>();
        }

        //public void SetSeries(Series series)
        //{
        //    lock (seriesLocker) {
        //        if (!Equals(Series, series)) {
        //            ser
        //        }
        //    }
        //}
        protected bool Equals(Film other)
        {
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Film)obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}