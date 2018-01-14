using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace XSystem.Core.Domain
{
    public class Series:Model
    {
        private string _characteristic;

        public string Characteristic {
            get { return _characteristic; }
            set {
                if (value == _characteristic) return;
                _characteristic = value;
                OnPropertyChanged();
            }
        }

        object filmsLocker=new object();

        public virtual List<Film> Films { get; set; }
        
        public Series()
        {
            Films = new List<Film>();
        }

        public void AddFilm(Film film)
        {
            lock (filmsLocker) {
                Films.Add(film);
            }
        }

        protected bool Equals(Series other)
        {
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Series)obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}