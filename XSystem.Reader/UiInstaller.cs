using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace XSystem.Reader
{
    public class UiInstaller:IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<MainWindow>()
                .LifestyleSingleton());
            container.Register(Component.For<MainWindowViewModel>()
                .LifestyleSingleton());
        }
    }
}