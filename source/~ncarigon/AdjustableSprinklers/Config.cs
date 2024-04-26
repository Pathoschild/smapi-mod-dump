/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ncarigon/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;

namespace AdjustableSprinklers {
    public class Config {
        public int BaseRadius { get; set; } = 1;
        public int TierIncrease { get; set; } = 1;
        public bool CircularArea { get; set; } = true;
        public bool ShowSprinklerArea { get; set; } = true;
        public bool ShowScarecrowArea { get; set; } = true;
        public bool ActivateWhenClicked { get; set; } = true;
        public bool WaterGardenPots { get; set; } = true;

        internal static Config Register(IModHelper helper) {
            var config = helper.ReadConfig<Config>();

            helper.Events.GameLoop.GameLaunched += (s, e) => {
                var configMenu = helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
                if (configMenu is null)
                    return;

                var manifest = helper.ModRegistry.Get(helper.ModContent.ModID)!.Manifest;
                configMenu.Register(
                    mod: manifest,
                    reset: () => config = new Config(),
                    save: () => helper.WriteConfig(config)
                );
                configMenu.AddNumberOption(
                    mod: manifest,
                    name: () => "Base Radius",
                    tooltip: () => "Set the starting radius for sprinkler watering. 0 = Default Game Value",
                    getValue: () => config.BaseRadius,
                    setValue: value => config.BaseRadius = value,
                    min: 0,
                    max: 10
                );
                configMenu.AddNumberOption(
                    mod: manifest,
                    name: () => "Tier Increase",
                    tooltip: () => "Set the radius increase each sprinkler upgrade applies. 1 = Default Game Value",
                    getValue: () => config.BaseRadius,
                    setValue: value => config.BaseRadius = value,
                    min: 1,
                    max: 10
                );
                configMenu.AddBoolOption(
                    mod: manifest,
                    name: () => "Circular Sprinkler Area",
                    tooltip: () => "Watered tiles by a sprinkler use a circular area instead of square.",
                    getValue: () => config.CircularArea,
                    setValue: value => config.CircularArea = value
                );
                configMenu.AddBoolOption(
                    mod: manifest,
                    name: () => "Show Sprinkler Area",
                    tooltip: () => "Shows tiles to be watered by a sprinkler on placement.",
                    getValue: () => config.ShowSprinklerArea,
                    setValue: value => config.ShowSprinklerArea = value
                );
                configMenu.AddBoolOption(
                    mod: manifest,
                    name: () => "Show Scarecrow Area",
                    tooltip: () => "Shows tiles to be protected by a scarecrow on placement.",
                    getValue: () => config.ShowScarecrowArea,
                    setValue: value => config.ShowScarecrowArea = value
                );
                configMenu.AddBoolOption(
                    mod: manifest,
                    name: () => "Activate When Clicked",
                    tooltip: () => "Activates a sprinkler when you click on it.",
                    getValue: () => config.ActivateWhenClicked,
                    setValue: value => config.ActivateWhenClicked = value
                );
                configMenu.AddBoolOption(
                    mod: manifest,
                    name: () => "Water Garden Pots",
                    tooltip: () => "Allow sprinklers to water garden pots.",
                    getValue: () => config.WaterGardenPots,
                    setValue: value => config.WaterGardenPots = value
                );
            };

            return config;
        }
    }

    public interface IGenericModConfigMenuApi {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);

        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string>? tooltip = null, string? fieldId = null);

        void AddNumberOption(IManifest mod, Func<int> getValue, Action<int> setValue, Func<string> name, Func<string> tooltip = null!, int? min = null, int? max = null, int? interval = null, Func<int, string>? formatValue = null, string? fieldId = null);
    }
}
