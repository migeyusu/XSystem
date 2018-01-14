using System.Collections.Generic;
using System.Data.Entity;

namespace XSystem.Core.Domain
{
    public class FilmService
    {
        private readonly IWorkUnit _workUnit;

        private readonly IRepository<Actor> _actorRepository;

        private readonly IRepository<Film> _filmRepository;

        private readonly IRepository<Series> _seriesRepository;

        private readonly IRepository<Publisher> _publisherRepository;

        public FilmService(IRepository<Actor> actorRepository, IRepository<Film> filmRepository, 
            IRepository<Series> seriesRepository, IRepository<Publisher> publisherRepository, IWorkUnit workUnit)
        {
            this._actorRepository = actorRepository;
            this._filmRepository = filmRepository;
            this._seriesRepository = seriesRepository;
            this._publisherRepository = publisherRepository;
            this._workUnit = workUnit;
        }

        
        

        public void Update()
        {
            _workUnit.Commit();
        }

        public IEnumerable<Film> AllFilms => _filmRepository.All();

        public IEnumerable<Actor> AllActors => _actorRepository.All();

        public IEnumerable<Series> AllSeries => _seriesRepository.All();

        public IEnumerable<Publisher> AllPublishers => _publisherRepository.All();
    }
}

//public void AddActors(IEnumerable<Actor> actors)
//{
//    foreach (var actor in actors) {
//        _actorRepository.Insert(actor);
//    }
//    _workUnit.Commit();
//}

//public void AddFilms(IEnumerable<Film> films)
//{
//    foreach (var film in films) {
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
//    foreach (var actor in actors) {
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
