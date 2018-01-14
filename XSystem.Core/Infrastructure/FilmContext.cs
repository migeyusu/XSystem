using System.Data.Entity;
using XSystem.Core.Domain;

namespace XSystem.Core.Infrastructure
{
    public class FilmContext:DbContext
    {
        public DbSet<Actor> Actors { get; set; }

        public DbSet<Film> Films { get; set; }

        public DbSet<Publisher> Publishers { get; set; }

        public DbSet<ReptileHistory> ReptileHistories { get; set; }
    }
}
