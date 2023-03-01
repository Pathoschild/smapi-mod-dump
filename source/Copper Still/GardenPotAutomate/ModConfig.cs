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

namespace GardenPotAutomate {
    internal class ModConfig {
        public bool Enabled { get; set; } = true;
        public bool HarvestCrops { get; set; } = true;
        public bool HarvestFlowers { get; set; } = true;
        public bool PlantSeeds { get; set; } = true;
        public bool ApplyFertilizers { get; set; } = true;
        public bool UseWateringCan { get; set; } = true;
        public bool ApplyProfessions { get; set; } = true;
        public bool GainExperience { get; set; } = true;

        public delegate void ModifyDelegate();
        internal ModifyDelegate OnModify = null!;

        public static ModConfig Register(IModHelper helper) {
            var config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += (s, e) => {
                var configMenu = helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
                if (configMenu is null)
                    return;

                var manifest = helper.ModRegistry.Get(helper.ModContent.ModID)!.Manifest;
                configMenu.Register(
                    mod: manifest,
                    reset: () => config = new ModConfig(),
                    save: () => helper.WriteConfig(config)
                );

                configMenu.AddBoolOption(
                    mod: manifest,
                    name: () => "Enabled",
                    tooltip: () => "",
                    getValue: () => config.Enabled,
                    setValue: value => config.Enabled = value
                );
                configMenu.AddBoolOption(
                    mod: manifest,
                    name: () => "Harvest Crops",
                    tooltip: () => "Attempt to harvest mature crops from garden pots.",
                    getValue: () => config.HarvestCrops,
                    setValue: value => config.HarvestCrops = value
                );
                configMenu.AddBoolOption(
                    mod: manifest,
                    name: () => "Harvest Flowers",
                    tooltip: () => "Attempt to harvest mature flowers from garden pots.",
                    getValue: () => config.HarvestFlowers,
                    setValue: value => config.HarvestFlowers = value
                );
                configMenu.AddBoolOption(
                    mod: manifest,
                    name: () => "Plant Seeds",
                    tooltip: () => "Attempt to plant seeds into garden pots.",
                    getValue: () => config.PlantSeeds,
                    setValue: value => config.PlantSeeds = value
                );
                configMenu.AddBoolOption(
                    mod: manifest,
                    name: () => "Apply Fertilizers",
                    tooltip: () => "Attempt to apply fertilizers to garden pots.",
                    getValue: () => config.ApplyFertilizers,
                    setValue: value => config.ApplyFertilizers = value
                );
                configMenu.AddBoolOption(
                    mod: manifest,
                    name: () => "Use Watering Can",
                    tooltip: () => "Attempt to use watering cans to water garden pots.",
                    getValue: () => config.UseWateringCan,
                    setValue: value => config.UseWateringCan = value
                );
                configMenu.AddBoolOption(
                    mod: manifest,
                    name: () => "Apply Professions",
                    tooltip: () => "Apply profession bonuses of the garden pot owner when automatically harvesting.",
                    getValue: () => config.ApplyProfessions,
                    setValue: value => config.ApplyProfessions = value
                );
                configMenu.AddBoolOption(
                    mod: manifest,
                    name: () => "Gain Experience",
                    tooltip: () => "Grant the garden pot owner experience when automatically harvesting.",
                    getValue: () => config.GainExperience,
                    setValue: value => config.GainExperience = value
                );
            };

            return config;
        }
    }

    public interface IGenericModConfigMenuApi {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);

        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string>? tooltip = null, string? fieldId = null);
    }
}
