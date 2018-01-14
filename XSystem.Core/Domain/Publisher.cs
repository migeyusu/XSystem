using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Castle.Core.Internal;
using FORCEBuild.Helper;

namespace XSystem.Core.Domain
{
    /* 由于一个出版商不会有太多系列，系列的查找用遍历即可
     */

    public class Publisher : Model
    {
        public virtual List<Series> Series { get; set; }

        private readonly object _serieslocker = new object();

        public Publisher()
        {
            Series = new List<Series>();
        }

        public Series GetOrAddSeries(string seriesname)
        {
            if (seriesname.IsNullOrEmpty()) {
                throw new NullReferenceException("can't add origin model");
            }
            Series series;
            lock (_serieslocker) {
                if ((series = Series.FirstOrDefault(
                        series1 => series1.Name == seriesname)) != null) return series;
                series = new Series() {
                    Name = seriesname
                };
                Series.Add(series);
                return series;
            }
        }

        public bool TryGetSeriesByName(string name, out Series series)
        {
            lock (_serieslocker) {
                return (series = Series.FirstOrDefault((series1 => series1.Name == name))) != null;
            }
        }

        protected bool Equals(Publisher other)
        {
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Publisher) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}