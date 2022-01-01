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
using StardewModdingAPI;
using TehPers.Core.Api.DI;
using TehPers.Core.Api.Setup;
using TehPers.FishingOverhaul.Config;
using TehPers.FishingOverhaul.Integrations.GenericModConfigMenu;

namespace TehPers.FishingOverhaul.Services.Setup
{
    internal class GenericModConfigMenuSetup : ISetup
    {
        private readonly IModHelper helper;
        private readonly IManifest manifest;
        private readonly IOptional<IGenericModConfigMenuApi> configApi;
        private readonly HudConfig hudConfig;
        private readonly ConfigManager<HudConfig> hudConfigManager;
        private readonly FishConfig fishConfig;
        private readonly ConfigManager<FishConfig> fishConfigManager;
        private readonly TreasureConfig treasureConfig;
        private readonly ConfigManager<TreasureConfig> treasureConfigManager;

        public GenericModConfigMenuSetup(
            IModHelper helper,
            IManifest manifest,
            IOptional<IGenericModConfigMenuApi> configApiFactory,
            HudConfig hudConfig,
            ConfigManager<HudConfig> hudConfigManager,
            FishConfig fishConfig,
            ConfigManager<FishConfig> fishConfigManager,
            TreasureConfig treasureConfig,
            ConfigManager<TreasureConfig> treasureConfigManager
        )
        {
            this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
            this.manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
            this.configApi = configApiFactory
                ?? throw new ArgumentNullException(nameof(configApiFactory));
            this.hudConfig = hudConfig ?? throw new ArgumentNullException(nameof(hudConfig));
            this.hudConfigManager = hudConfigManager
                ?? throw new ArgumentNullException(nameof(hudConfigManager));
            this.fishConfig = fishConfig ?? throw new ArgumentNullException(nameof(fishConfig));
            this.fishConfigManager = fishConfigManager
                ?? throw new ArgumentNullException(nameof(fishConfigManager));
            this.treasureConfig =
                treasureConfig ?? throw new ArgumentNullException(nameof(treasureConfig));
            this.treasureConfigManager = treasureConfigManager
                ?? throw new ArgumentNullException(nameof(treasureConfigManager));
        }

        public void Setup()
        {
            if (!this.configApi.TryGetValue(out var configApi))
            {
                return;
            }

            string Id(string key) => $"{this.manifest.UniqueID}/{key}";
            Translation Name(string key) => this.helper.Translation.Get($"text.config.{key}.name");
            Translation Desc(string key) => this.helper.Translation.Get($"text.config.{key}.desc");

            // Register the mod with GMCM
            configApi.Register(
                this.manifest,
                () =>
                {
                    ((IModConfig)this.hudConfig).Reset();
                    ((IModConfig)this.fishConfig).Reset();
                    ((IModConfig)this.treasureConfig).Reset();
                },
                () =>
                {
                    this.hudConfigManager.Save(this.hudConfig);
                    this.fishConfigManager.Save(this.fishConfig);
                    this.treasureConfigManager.Save(this.treasureConfig);
                }
            );

            // Create page links for the different groups of settings
            configApi.AddPageLink(this.manifest, Id("hud"), () => Name("hud"), () => Desc("hud"));
            configApi.AddPageLink(
                this.manifest,
                Id("fish"),
                () => Name("fish"),
                () => Desc("fish")
            );
            configApi.AddPageLink(
                this.manifest,
                Id("treasure"),
                () => Name("treasure"),
                () => Desc("treasure")
            );

            // HUD config settings
            configApi.AddPage(this.manifest, Id("hud"), () => Name("hud"));
            ((IModConfig)this.hudConfig).RegisterOptions(
                configApi,
                this.manifest,
                this.helper.Translation
            );

            // Fishing config settings
            configApi.AddPage(this.manifest, Id("fish"), () => Name("fish"));
            ((IModConfig)this.fishConfig).RegisterOptions(
                configApi,
                this.manifest,
                this.helper.Translation
            );

            // Treasure config settings
            configApi.AddPage(this.manifest, Id("treasure"), () => Name("treasure"));
            ((IModConfig)this.treasureConfig).RegisterOptions(
                configApi,
                this.manifest,
                this.helper.Translation
            );
        }
    }
}