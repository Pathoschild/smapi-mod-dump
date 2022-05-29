/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using Ninject;
using Ninject.Extensions.Factory;
using StardewModdingAPI;
using TehPers.Core.Api.DI;
using TehPers.Core.Api.Items;
using TehPers.Core.DI;
using TehPers.Core.Modules;

namespace TehPers.Core
{
    public class ModCore : Mod
    {
        public ModCore()
        {
            // Create kernel factory and add core processors
            ModServices.Factory = new ModKernelFactory();
            ModServices.Factory.AddKernelProcessor(
                kernel => kernel.Load(new FuncModule(), new ModServicesModule(kernel))
            );
        }

        public override void Entry(IModHelper helper)
        {
            // Add processors
            ModServices.Factory!.AddKernelProcessor(kernel => kernel.Load<ModJsonModule>());

            // Add 
            var kernel = ModServices.Factory.GetKernel(this);
            kernel.Load(new GlobalJsonModule(helper, this.Monitor));
            kernel.Load<CoreServicesModule>();

            // Setup services on game launched
            helper.Events.GameLoop.GameLaunched += (_, _) =>
            {
                var startups = kernel.GetAll<Startup>();
                foreach (var startup in startups)
                {
                    startup.Initialize();
                }
            };

            // Reload namespace registry on save loaded
            helper.Events.GameLoop.SaveLoaded +=
                (_, _) => kernel.Get<INamespaceRegistry>().RequestReload();

            // Add custom commands
            helper.ConsoleCommands.Add(
                "tehcore_listkeys",
                "Lists the known registered namespaced keys.",
                ListKeys
            );

            void ListKeys(string s, string[] strings)
            {
                var registry = kernel.Get<INamespaceRegistry>();
                var monitor = kernel.Get<IMonitor>();

                // List namespaces
                monitor.Log("Registered namespaces:", LogLevel.Info);
                foreach (var ns in registry.GetRegisteredNamespaces())
                {
                    monitor.Log($" - {ns}", LogLevel.Info);
                }

                // List keys
                monitor.Log("Known namespaced keys:", LogLevel.Info);
                monitor.Log(string.Join(", ", registry.GetKnownItemKeys()), LogLevel.Info);
            }
        }
    }
}
