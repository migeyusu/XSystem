using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using FORCEBuild.Concurrency;

namespace XSystem.Core.Domain
{
    public class FilmService
    {
        private readonly IWorkUnit _workUnit;

        private readonly IRepository<Actor> _actorRepository;

        private readonly IRepository<Film> _filmRepository;

        private readonly IRepository<Publisher> _publisherRepository;

        private readonly IRepository<Series> _seriesRepository;

        public FilmService(IRepository<Actor> actorRepository, IRepository<Film> filmRepository,
            IRepository<Publisher> publisherRepository, IWorkUnit workUnit, IRepository<Series> seriesRepository)
        {
            this._actorRepository = actorRepository;
            this._filmRepository = filmRepository;
            this._publisherRepository = publisherRepository;
            this._workUnit = workUnit;
            this._seriesRepository = seriesRepository;
        }

        //public void AddActors(IEnumerable<Actor> actors)
        //{
        //    foreach (var actor in actors)
        //    {
        //        _actorRepository.Insert(actor);
        //    }
        //    _workUnit.Commit();
        //}

        //public void AddFilms(IEnumerable<Film> films)
        //{
        //    foreach (var film in films)
        //    {
        //        _filmRepository.Insert(film);
        //    }
        //    _workUnit.Commit();
        //}

        //public void AddPublishers(IEnumerable<Publisher> publishers)
        //{
        //    foreach (var film in publishers)
        //    {
        //        _publisherRepository.Insert(film);
        //    }
        //    _workUnit.Commit();
        //}

        //public void AddSeries(IEnumerable<Series> series)
        //{
        //    foreach (var film in series)
        //    {
        //        _seriesRepository.Insert(film);
        //    }
        //    _workUnit.Commit();
        //}

        //public void RemoveActors(IEnumerable<Actor> actors)
        //{
        //    foreach (var actor in actors)
        //    {
        //        _actorRepository.Delete(actor);
        //    }
        //    _workUnit.Commit();
        //}

        //public void RemovePublishers(IEnumerable<Publisher> publishers)
        //{
        //    foreach (var publisher in publishers)
        //    {
        //        _publisherRepository.Delete(publisher);
        //    }
        //    _workUnit.Commit();
        //}

        //public void RemoveSeries(IEnumerable<Series> series)
        //{
        //    foreach (var ser in series)
        //    {
        //        _seriesRepository.Delete(ser);
        //    }
        //    _workUnit.Commit();
        //}
        //public void RemoveFilms(IEnumerable<Film> films)
        //{
        //    foreach (var film in films)
        //    {
        //        _filmRepository.Delete(film);
        //    }
        //    _workUnit.Commit();
        //}

        public void Update()
        {
            _workUnit.Commit();
        }

        public IQueryable<Actor> ActorSearch(Expression<Func<Actor, bool>> filterExpression = null,
            Func<IQueryable<Actor>, IOrderedQueryable<Actor>>orderByFunc = null,
            string includeProperties = "") => _actorRepository.Get(filterExpression, orderByFunc, includeProperties);

        public IQueryable<Publisher> PublishersSearch(Expression<Func<Publisher, bool>> filterExpression = null,
            Func<IQueryable<Publisher>, IOrderedQueryable<Publisher>>orderByFunc = null,
            string includeProperties = "") =>
            _publisherRepository.Get(filterExpression, orderByFunc, includeProperties);


        public IQueryable<Film> FilmsSearch(Expression<Func<Film, bool>> filterExpression = null,
            Func<IQueryable<Film>, IOrderedQueryable<Film>>orderByFunc = null,
            string includeProperties = "")
            => _filmRepository.Get(filterExpression, orderByFunc, includeProperties);


        public IQueryable<Series> SeriesSearch(Expression<Func<Series, bool>> filterExpression = null,
            Func<IQueryable<Series>, IOrderedQueryable<Series>>orderByFunc = null,
            string includeProperties = "")
            => _seriesRepository.Get(filterExpression, orderByFunc, includeProperties);

        public IQueryable<Film> AllFilms => _filmRepository.All();

        public IQueryable<Actor> AllActors => _actorRepository.All();

        public IQueryable<Publisher> AllPublishers => _publisherRepository.All();

        public IQueryable<Series> AllSeries => _seriesRepository.All();
    }
}