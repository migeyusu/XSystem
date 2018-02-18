using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using XSystem.Core.Domain;
using XSystem.Core.Infrastructure;

namespace DeployTest.Service
{
    public class EntityFrameworkPersistence : IPersistence
    {
        private readonly FilmContext _context;

        public EntityFrameworkPersistence(FilmContext context)
        {
            this._context = context;
            context.Configuration.AutoDetectChangesEnabled = false;
        }

        public IQueryable<T> Get<T>(Expression<Func<T, bool>> filterException = null) where T : class
        {
            return filterException != null ? _context.Set<T>().Where(filterException) : _context.Set<T>();
        }

        public void Save(VisionReptile reptile)
        {
            Console.WriteLine($"ready to save,{reptile.Processor.ErrorUrl.Count} error urls");
            try {
                using (var dbContextTransaction = _context.Database.BeginTransaction()) {
                    _context.Films.AddRange(reptile.FetchedNewFilms);
                    _context.Actors.AddRange(reptile.FetchedNewActors);
                    _context.Publishers.AddRange(reptile.FetchedNewPublishers);
                    _context.ReptileHistories.Add(new ReptileHistory() {
                        DateTime = DateTime.Now,
                        ErrorPages = reptile.Processor.ErrorUrl
                            .Select(page => FetchErrorPage.Create(page.Url, page.Exception, page.Error))
                            .ToList()
                    });
                    _context.Configuration.AutoDetectChangesEnabled = true;
                    _context.SaveChanges();
                    dbContextTransaction.Commit();
                }
                Console.WriteLine($"saved success.{reptile.FetchedNewFilms.Count}films {reptile.FetchedNewActors.Count}actors {reptile.FetchedNewPublishers.Count}publishers");
            }
            catch (Exception exception) {
                var stringBuilder = new StringBuilder();
                while (exception != null) {
                    stringBuilder.AppendLine(exception.Message);
                    exception = exception.InnerException;
                }
                Console.WriteLine($"save failed all ops will rollback:\r\n{stringBuilder.ToString()}");
            }
            finally {
                _context.Configuration.AutoDetectChangesEnabled = false;
            }
        }
    }
}