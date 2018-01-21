using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Reptile;
using XSystem.Core.Domain;
using XSystem.Core.Infrastructure;

namespace DeployTest.Service
{
    public class VisionReptile:IDisposable
    {
        public const string UndefinedSeries = "UndefinedSeries";

        public const string UndefinedActor = "UndefinedActor";

        public const string UndefinedPublisher = "UndefinedPublisher";

        public Region Region { get; set; }

        public ReptileProcessor Processor { get; set; }

        public ConcurrentBag<Actor> FetchedNewActors { get; set; } = new ConcurrentBag<Actor>();

        public ConcurrentBag<Publisher> FetchedNewPublishers { get; set; } = new ConcurrentBag<Publisher>();

        public ConcurrentBag<Film> FetchedNewFilms { get; set; } = new ConcurrentBag<Film>();

        //全局的model来源，使用IQueryable接口提高性能
        protected IQueryable<Actor> AvailableActors => Persistence.Get<Actor>(actor => actor.Region == Region);

        protected IQueryable<Film> AvailableFilms => Persistence.Get<Film>(film => film.Region == Region);

        protected IQueryable<Publisher> AvailablePublishers => Persistence.Get<Publisher>(
            publisher => publisher.Region == Region);
       
        protected readonly IPersistence Persistence;

        public VisionReptile(IPersistence persistence)
        {
            Persistence = persistence;
        }

        public void Dispose()
        {
            Processor?.Dispose();
        }
    }

}