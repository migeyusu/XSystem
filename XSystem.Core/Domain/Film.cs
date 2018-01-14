using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Castle.Components.DictionaryAdapter;
using Castle.Core.Internal;
using FORCEBuild.UI.WPF;

namespace XSystem.Core.Domain
{
    public class Film:Model
    {
        //object seriesLocker=new object();

        public virtual List<Actor> Actors { get; set; }

        public virtual Series Series { get; set; }
        /// <summary>
        /// 绝对地址
        /// </summary>
        public string FileLocation { get; set; }

        public string Characteristic { get; set; }

        public Region Region { get; set; }

        /// <summary>
        /// 来源页面
        /// </summary>
        public string SourceUrl { get; set; }

        public DateTime LastUpdateTime { get; set; }

        /// <summary>
        /// 本地图片标识，loading在UI层实现
        /// </summary>
        public string ShotTag { get; set; }
        /// <summary>
        /// 在线url
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// 推荐级别
        /// </summary>
        public RecommendLevel RecommendLevel { get; set; }
        /// <summary>
        /// 代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 详情页Url
        /// </summary>
        public string DetailUrl { get; set; }

        public bool IsOneActor => Actors.Count == 1;

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