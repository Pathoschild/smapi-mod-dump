/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ncarigon/StardewValleyMods
**
*************************************************/

using System;
using StardewModdingAPI;

namespace PassableCrops {
    public class Config {
        public bool PassableCrops { get; set; } = true;
        public bool PassableScarecrows { get; set; } = true;
        public bool PassableSprinklers { get; set; } = true;
        public bool PassableForage { get; set; } = true;
        public bool PassableTeaBushes { get; set; } = true;
        public int PassableTreeGrowth { get; set; } = 4;
        public int PassableFruitTreeGrowth { get; set; } = 1;
        public bool PassableWeeds { get; set; } = true;
        public bool SlowDownWhenPassing { get; set; } = true;
        public bool UseCustomDrawing { get; set; } = true;

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
                configMenu.AddBoolOption(
                    mod: manifest,
                    name: () => "Crops",
                    tooltip: () => "Allow farmers to walk through all crops.",
                    getValue: () => config.PassableCrops,
                    setValue: value => config.PassableCrops = value
                );
                configMenu.AddBoolOption(
                    mod: manifest,
                    name: () => "Scarecrows",
                    tooltip: () => "Allow farmers to walk through scarecrows.",
                    getValue: () => config.PassableScarecrows,
                    setValue: value => config.PassableScarecrows = value
                );
                configMenu.AddBoolOption(
                    mod: manifest,
                    name: () => "Sprinklers",
                    tooltip: () => "Allow farmers to walk through sprinklers.",
                    getValue: () => config.PassableSprinklers,
                    setValue: value => config.PassableSprinklers = value
                );
                configMenu.AddBoolOption(
                    mod: manifest,
                    name: () => "Forage",
                    tooltip: () => "Allow farmers to walk through forage items.",
                    getValue: () => config.PassableForage,
                    setValue: value => config.PassableForage = value
                );
                configMenu.AddBoolOption(
                    mod: manifest,
                    name: () => "Tea Bushes",
                    tooltip: () => "Allow farmers to walk through tea bushes.",
                    getValue: () => config.PassableTeaBushes,
                    setValue: value => config.PassableTeaBushes = value
                );
                configMenu.AddNumberOption(
                    mod: manifest,
                    name: () => "Trees - Growth Stage",
                    tooltip: () => "Allow farmers to walk through tree saplings, up to the given growth stage.",
                    getValue: () => config.PassableTreeGrowth,
                    setValue: value => config.PassableTreeGrowth = value,
                    min: 0,
                    max: 4
                );
                configMenu.AddNumberOption(
                    mod: manifest,
                    name: () => "Fruit Trees - Growth Stage",
                    tooltip: () => "Allow farmers to walk through fruit tree saplings, up to the given growth stage.",
                    getValue: () => config.PassableFruitTreeGrowth,
                    setValue: value => config.PassableFruitTreeGrowth = value,
                    min: -1,
                    max: 3
                );
                configMenu.AddBoolOption(
                    mod: manifest,
                    name: () => "Weeds",
                    tooltip: () => "Allow farmers to walk through weeds.",
                    getValue: () => config.PassableWeeds,
                    setValue: value => config.PassableWeeds = value
                );
                configMenu.AddBoolOption(
                    mod: manifest,
                    name: () => "Slow down when passing",
                    tooltip: () => "Farmers will walk slightly slower through objects, just like in tall grass.",
                    getValue: () => config.SlowDownWhenPassing,
                    setValue: value => config.SlowDownWhenPassing = value
                );
                configMenu.AddBoolOption(
                    mod: manifest,
                    name: () => "Use custom object drawing",
                    tooltip: () => "Some objects require custom drawing logic in order to shake and calculate the correct layer depth. This logic may be incompatible with other mods. This option disables the custom drawing and may prevent errors that could arise. It is not recommended to disable custom drawing if no issues are present.",
                    getValue: () => config.UseCustomDrawing,
                    setValue: value => config.UseCustomDrawing = value
                );
            };
            return config;
        }
    }

    public interface IGenericModConfigMenuApi {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);

        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null!, string fieldId = null!);

        void AddNumberOption(IManifest mod, Func<int> getValue, Action<int> setValue, Func<string> name, Func<string> tooltip = null!, int? min = null, int? max = null, int? interval = null, Func<int, string> formatValue = null!, string fieldId = null!);
    }
}
