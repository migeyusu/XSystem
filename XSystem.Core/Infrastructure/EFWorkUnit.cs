using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using XSystem.Core.Domain;

namespace XSystem.Core.Infrastructure
{
    public class EFWorkUnit:IWorkUnit
    {
        private readonly DbContext _context;

        public DbContext Context => _context;

        public EFWorkUnit(DbContext context)
        {
            this._context = context;
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}