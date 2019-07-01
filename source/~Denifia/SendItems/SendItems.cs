using Autofac;
using Denifia.Stardew.SendItems.Domain;
using Denifia.Stardew.SendItems.Services;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace Denifia.Stardew.SendItems
{
    public class SendItems : Mod
    {
        private IContainer _container;

        public override void Entry(IModHelper helper)
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(this).As<IMod>();
            builder.RegisterInstance(helper).As<IModHelper>();
            builder.RegisterInstance(helper.Events).As<IModEvents>();
            builder.RegisterAssemblyTypes(typeof(SendItems).Assembly)
                .Where(t => t.Name.EndsWith("Service"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(typeof(SendItems).Assembly)
                .Where(t => t.Name.EndsWith("Detector"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            _container = builder.Build();

            // Init repo first!
            Repository.Instance.Init(_container.Resolve<IConfigurationService>());

            // Instance classes that do their own thing
            _container.Resolve<IFarmerService>();
            _container.Resolve<ICommandService>();
            _container.Resolve<IPostboxService>();
            _container.Resolve<ILetterboxService>();
            _container.Resolve<IPostboxInteractionDetector>();
            _container.Resolve<ILetterboxInteractionDetector>();
            _container.Resolve<IMailDeliveryService>();
            _container.Resolve<IMailCleanupService>();
            _container.Resolve<IMailScheduleService>();
        }
    }
}
