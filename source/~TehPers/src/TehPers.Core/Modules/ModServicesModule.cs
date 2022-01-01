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
using StardewModdingAPI;
using TehPers.Core.Api.Content;
using TehPers.Core.Api.DI;
using TehPers.Core.Content;
using TehPers.Core.DI;

namespace TehPers.Core.Modules
{
    internal class ModServicesModule : ModModule
    {
        private readonly IModKernel modKernel;

        public ModServicesModule(IModKernel modKernel)
        {
            this.modKernel = modKernel;
        }

        public override void Load()
        {
            // SMAPI types
            this.Bind(this.modKernel.ParentMod.GetType(), typeof(IMod))
                .ToConstant(this.modKernel.ParentMod)
                .InSingletonScope();
            this.Bind<IMonitor>()
                .ToMethod(context => context.Kernel.Get<IMod>().Monitor)
                .InSingletonScope();
            this.Bind<IModHelper>()
                .ToMethod(context => context.Kernel.Get<IMod>().Helper)
                .InSingletonScope();
            this.Bind<IManifest>()
                .ToMethod(context => context.Kernel.Get<IMod>().ModManifest)
                .InSingletonScope();

            // The mod's kernel
            this.Bind<IModKernel>().ToConstant(this.modKernel).InSingletonScope();

            // Content
            this.Bind<IAssetProvider>()
                .To<ModAssetProvider>()
                .InSingletonScope()
                .WithMetadata(nameof(ContentSource), ContentSource.ModFolder);
            this.Bind<IAssetProvider>()
                .To<GameAssetProvider>()
                .InSingletonScope()
                .WithMetadata(nameof(ContentSource), ContentSource.GameContent);

            // DI-related types
            this.Bind(typeof(IOptional<>)).To(typeof(InjectedOptional<>)).InTransientScope();
            this.Bind(typeof(ISimpleFactory<>)).To(typeof(SimpleFactory<>)).InSingletonScope();

            // Startup types
            this.GlobalProxyRoot.Bind<Startup>().ToSelf().InSingletonScope();
        }
    }
}