using System;
using System.Data.Entity;
using System.Linq;
using XSystem.Core.Domain;

namespace XSystem.Core.Infrastructure
{
    public class Deploy
    {
        private readonly DbSet<Actor> actors;

        private readonly DbSet<Publisher> publishers;

        private DbSet<Series> series;

        private readonly DbContext _dbContext;

        public Deploy(DbContext dbContext)
        {
            actors = dbContext.Set<Actor>();
            publishers = dbContext.Set<Publisher>();
            series = dbContext.Set<Series>();
            this._dbContext = dbContext;
        }

        public void GenerateNew()
        {
            var series1 = new Series() {
                Name = "dsf"
            };
            var publisher = new Publisher() {
                Name = "dsdf"
            };
            publisher.Series.Add(series1);
            //series.Add(series1);
            publishers.Add(publisher);
           
            _dbContext.SaveChanges();
        }

        public void RelationChange()
        {
            var publisher = publishers.First();
            var first = series.First();
            publisher.Series.Add(first);
            _dbContext.SaveChanges();
        }

        public void DeleteRandom()
        {
            publishers.Remove(publishers.First());
            _dbContext.SaveChanges();
        }

        public bool Load()
        {
            var publisher = publishers.First();
            Console.WriteLine(publisher.Series.Count);
            return true;
        }
    }
}