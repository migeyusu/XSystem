using System.Data.Entity;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using XSystem.Core.Domain;

namespace XSystem.Core.Infrastructure
{
    public class DomainInstaller:IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
//            container.Register(Component.For<IRepository<Actor>>()
//                .ImplementedBy<MemoryRepository<Actor>>()
//                .LifestyleSingleton());
//            container.Register(Component.For<IRepository<Film>>()
//                .ImplementedBy<MemoryRepository<Film>>()
//                .LifestyleSingleton());
//            container.Register(Component.For<IRepository<Series>>()
//                .ImplementedBy<MemoryRepository<Series>>()
//                .LifestyleSingleton());
//            container.Register(Component.For<IRepository<Publisher>>()
//                .ImplementedBy<MemoryRepository<Publisher>>()
//                .LifestyleSingleton());
//            container.Register(Component.For<IWorkUnit>()
//                .ImplementedBy<MemoryWorkUnit>()
//                .LifestyleSingleton());
            container.Register(Component.For<IRepository<Actor>>()
                .ImplementedBy<BaseEfRepository<Actor>>()
                .LifestyleSingleton());
            container.Register(Component.For<IRepository<Film>>()
                .ImplementedBy<BaseEfRepository<Film>>()
                .LifestyleSingleton());
            container.Register(Component.For<IRepository<Series>>()
                .ImplementedBy<BaseEfRepository<Series>>()
                .LifestyleSingleton());
            container.Register(Component.For<IRepository<Publisher>>()
                .ImplementedBy<BaseEfRepository<Publisher>>()
                .LifestyleSingleton());
            container.Register(Component.For<IWorkUnit>()
                .ImplementedBy<EFWorkUnit>()
                .LifestyleSingleton());
            container.Register(Component.For<DbContext>()
                .ImplementedBy<FilmContext>()
                .LifestyleSingleton());
            container.Register(Component.For<FilmService>()
                .LifestyleSingleton());
            container.Register(Component.For<Deploy>()
                .LifestyleSingleton());
        }
    }
}