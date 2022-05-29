/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using ContentPatcher;
using HarmonyLib;
using Newtonsoft.Json;
using Ninject;
using StardewModdingAPI;
using TehPers.Core.Api.DI;
using TehPers.Core.Api.Extensions;
using TehPers.Core.Api.Setup;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Api.Content;
using TehPers.FishingOverhaul.Api.Effects;
using TehPers.FishingOverhaul.Config;
using TehPers.FishingOverhaul.Effects;
using TehPers.FishingOverhaul.Integrations.Emp;
using TehPers.FishingOverhaul.Integrations.GenericModConfigMenu;
using TehPers.FishingOverhaul.Services;
using TehPers.FishingOverhaul.Services.Setup;
using TehPers.FishingOverhaul.Services.Tokens;

namespace TehPers.FishingOverhaul
{
    internal class FishingModule : ModModule
    {
        public override void Load()
        {
            // Initialization
            this.Bind<ISetup>().To<FishingHudRenderer>().InSingletonScope();
            this.Bind<ISetup>().To<GenericModConfigMenuSetup>().InSingletonScope();
            this.Bind<ISetup>().ToMethod(FishingRodPatcher.Create).InSingletonScope();
            this.Bind<ISetup>().To<DefaultContentReloader>().InSingletonScope();
            this.Bind<ISetup>().To<ContentPatcherSetup>().InSingletonScope();
            this.Bind<ISetup>().To<DefaultCustomEvents>().InSingletonScope();
            this.Bind<ISetup>().To<ConsoleCommandsSetup>().InSingletonScope();
            this.Bind<ISetup>().To<MissingSecretNotesToken>().InSingletonScope();
            this.Bind<ISetup>().To<MissingJournalScrapsToken>().InSingletonScope();
            this.Bind<ISetup>().To<FishingEffectApplier>().InSingletonScope();
            this.Bind<ISetup>()
                .ToMethod(context => context.Kernel.Get<ModifyChanceEffectManager>())
                .InSingletonScope();

            // Resources/services
            this.GlobalProxyRoot.Bind<IFishingApi, ISimplifiedFishingApi, FishingApi>()
                .ToMethod(
                    context => new FishingApi(
                        context.Kernel.Get<IModHelper>(),
                        context.Kernel.Get<IMonitor>(),
                        context.Kernel.Get<IManifest>(),
                        context.Kernel.Get<FishConfig>(),
                        context.Kernel.Get<TreasureConfig>(),
                        context.Kernel.Get<Func<IEnumerable<IFishingContentSource>>>(),
                        context.Kernel.Get<EntryManagerFactory<FishEntry, FishAvailabilityInfo>>(),
                        context.Kernel.Get<EntryManagerFactory<TrashEntry, AvailabilityInfo>>(),
                        context.Kernel.Get<EntryManagerFactory<TreasureEntry, AvailabilityInfo>>(),
                        context.Kernel.Get<FishingEffectManagerFactory>(),
                        context.Kernel.Get<Lazy<IOptional<IEmpApi>>>()
                    )
                )
                .InSingletonScope();
            this.Bind<ICustomBobberBarFactory>().To<CustomBobberBarFactory>().InSingletonScope();
            this.Bind<FishingTracker>().ToSelf().InSingletonScope();
            this.Bind<Harmony>()
                .ToMethod(
                    context =>
                    {
                        var manifest = context.Kernel.Get<IManifest>();
                        return new(manifest.UniqueID);
                    }
                )
                .InSingletonScope();
            this.Bind<CalculatorFactory>().ToSelf().InSingletonScope();
            this.Bind(typeof(EntryManagerFactory<,>)).ToSelf().InSingletonScope();
            this.Bind<FishingEffectManagerFactory>().ToSelf().InSingletonScope();
            this.GlobalProxyRoot.Bind<FishingEffectRegistration>()
                .ToConstant(
                    FishingEffectRegistration.Of<ModifyChanceEffectEntry>("ModifyFishChance")
                )
                .InSingletonScope();
            this.GlobalProxyRoot.Bind<ModifyChanceEffectManager>().ToSelf().InSingletonScope();

            // Configs
            this.BindConfiguration<HudConfig>("config/hud.json");
            this.BindConfiguration<FishConfig>("config/fish.json");
            this.BindConfiguration<TreasureConfig>("config/treasure.json");

            // Content
            this.GlobalProxyRoot.Bind<IFishingContentSource>()
                .To<ContentPackSource>()
                .InSingletonScope();
            this.GlobalProxyRoot.Bind<IFishingContentSource>()
                .To<DefaultFishingSource>()
                .InSingletonScope();

            // JSON converters
            this.GlobalProxyRoot.Bind<JsonConverter>()
                .To<EffectEntryJsonConverter>()
                .InSingletonScope();

            // Foreign APIs
            this.BindForeignModApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu")
                .InSingletonScope();
            this.BindForeignModApi<IContentPatcherAPI>("Pathoschild.ContentPatcher")
                .InSingletonScope();
            this.BindForeignModApi<IEmpApi>("Esca.EMP").InSingletonScope();
        }

        private void BindConfiguration<T>(string path)
            where T : class, IModConfig, new()
        {
            this.Bind<ConfigManager<T>>()
                .ToSelf()
                .InSingletonScope()
                .WithConstructorArgument("path", path);
            this.Bind<IModConfig, T>()
                .ToMethod(context => context.Kernel.Get<ConfigManager<T>>().Load())
                .InSingletonScope();
        }
    }
}
