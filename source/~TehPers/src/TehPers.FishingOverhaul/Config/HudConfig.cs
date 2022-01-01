/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System.ComponentModel;
using StardewModdingAPI;
using TehPers.Core.Api.Json;
using TehPers.FishingOverhaul.Integrations.GenericModConfigMenu;

namespace TehPers.FishingOverhaul.Config
{
    /// <summary>
    /// Configuration for the fishing HUD.
    /// </summary>
    /// <inheritdoc cref="IModConfig"/>
    [JsonDescribe]
    public sealed class HudConfig : IModConfig
    {
        /// <summary>
        /// Whether or not to show current streak, chance for treasure, chance for each fish, etc.
        /// while fishing.
        /// </summary>
        [DefaultValue(true)]
        public bool ShowFishingHud { get; set; } = true;

        /// <summary>
        /// The X coordinate of the top left corner of the fishing HUD.
        /// </summary>
        [DefaultValue(0)]
        public int TopLeftX { get; set; }

        /// <summary>
        /// The Y coordinate of the top left corner of the fishing HUD.
        /// </summary>
        [DefaultValue(0)]
        public int TopLeftY { get; set; }

        /// <summary>
        /// The number of fish to show on the fishing HUD.
        /// </summary>
        [DefaultValue(5)]
        public int MaxFishTypes { get; set; } = 5;

        void IModConfig.Reset()
        {
            this.ShowFishingHud = true;
            this.TopLeftX = 0;
            this.TopLeftY = 0;
            this.MaxFishTypes = 5;
        }

        void IModConfig.RegisterOptions(
            IGenericModConfigMenuApi configApi,
            IManifest manifest,
            ITranslationHelper translations
        )
        {
            Translation Name(string key) => translations.Get($"text.config.hud.{key}.name");
            Translation Desc(string key) => translations.Get($"text.config.hud.{key}.desc");

            configApi.AddBoolOption(
                manifest,
                () => this.ShowFishingHud,
                val => this.ShowFishingHud = val,
                () => Name("showFishingHud"),
                () => Desc("showFishingHud")
            );
            configApi.AddNumberOption(
                manifest,
                () => this.TopLeftX,
                val => this.TopLeftX = val,
                () => Name("topLeftX"),
                () => Desc("topLeftX")
            );
            configApi.AddNumberOption(
                manifest,
                () => this.TopLeftY,
                val => this.TopLeftY = val,
                () => Name("topLeftY"),
                () => Desc("topLeftY")
            );
            configApi.AddNumberOption(
                manifest,
                () => this.MaxFishTypes,
                val => this.MaxFishTypes = val,
                () => Name("maxFishTypes"),
                () => Desc("maxFishTypes"),
                0,
                20
            );
        }
    }
}