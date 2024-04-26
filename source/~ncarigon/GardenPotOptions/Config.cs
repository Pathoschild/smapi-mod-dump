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
using StardewValley;

namespace GardenPotOptions {
    public class Config {
        public bool KeepContents { get; set; } = true;
        public bool EnableSprinklers { get; set; } = true;
        public bool AllowAncientSeeds { get; set; } = false;
        public string SafeTool { get; set; } = "Pickaxe";
        public bool AllowTransplant { get; set; } = true;
        public int HeartsForGardenPot { get; set; } = 4;
        public int HeartsForRecipe { get; set; } = 8;

        internal static void Register() {
            var config = ModEntry.Instance?.Helper?.ReadConfig<Config>();
            if (config is not null) {
                ModEntry.Instance!.ModConfig = config;
                ModEntry.Instance!.Helper.Events.GameLoop.GameLaunched += (s, e) => {
                    var configMenu = ModEntry.Instance!.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
                    if (configMenu is null)
                        return;
                    configMenu.Register(
                        mod: ModEntry.Instance!.ModManifest,
                        reset: () => ModEntry.Instance!.ModConfig = new Config(),
                        save: () => ModEntry.Instance!.Helper.WriteConfig(ModEntry.Instance!.ModConfig ?? new Config())
                    );
                    configMenu.AddBoolOption(
                        mod: ModEntry.Instance!.ModManifest,
                        name: () => ModEntry.Instance?.Helper.Translation.Get("NCarigon.GardenPotOptions/config_name_KeepContents") ?? "null",
                        tooltip: () => ModEntry.Instance?.Helper.Translation.Get("NCarigon.GardenPotOptions/config_desc_KeepContents") ?? "null",
                        getValue: () => ModEntry.Instance!.ModConfig.KeepContents,
                        setValue: value => ModEntry.Instance!.ModConfig.KeepContents = value
                    );
                    configMenu.AddTextOption(
                        mod: ModEntry.Instance!.ModManifest,
                        name: () => ModEntry.Instance?.Helper.Translation.Get("NCarigon.GardenPotOptions/config_name_SafeTool") ?? "null",
                        tooltip: () => ModEntry.Instance?.Helper.Translation.Get("NCarigon.GardenPotOptions/config_desc_SafeTool") ?? "null",
                        getValue: () => ModEntry.Instance!.ModConfig.SafeTool,
                        setValue: value => ModEntry.Instance!.ModConfig.SafeTool = value,
                        allowedValues: new string[] { "Pickaxe", "Axe", "Hoe" },
                        formatAllowedValue: (v) => ItemRegistry.GetMetadata($"(T){v}").CreateItem().DisplayName
                    );
                    configMenu.AddBoolOption(
                        mod: ModEntry.Instance!.ModManifest,
                        name: () => ModEntry.Instance?.Helper.Translation.Get("NCarigon.GardenPotOptions/config_name_EnableSprinklers") ?? "null",
                        tooltip: () => ModEntry.Instance?.Helper.Translation.Get("NCarigon.GardenPotOptions/config_desc_EnableSprinklers") ?? "null",
                        getValue: () => ModEntry.Instance!.ModConfig.EnableSprinklers,
                        setValue: value => ModEntry.Instance!.ModConfig.EnableSprinklers = value
                    );
                    configMenu.AddBoolOption(
                        mod: ModEntry.Instance!.ModManifest,
                        name: () => ModEntry.Instance?.Helper.Translation.Get("NCarigon.GardenPotOptions/config_name_AncientSeeds") ?? "null",
                        tooltip: () => ModEntry.Instance?.Helper.Translation.Get("NCarigon.GardenPotOptions/config_desc_AncientSeeds") ?? "null",
                        getValue: () => ModEntry.Instance!.ModConfig.AllowAncientSeeds,
                        setValue: value => ModEntry.Instance!.ModConfig.AllowAncientSeeds = value
                    );
                    configMenu.AddBoolOption(
                        mod: ModEntry.Instance!.ModManifest,
                        name: () => ModEntry.Instance?.Helper.Translation.Get("NCarigon.GardenPotOptions/config_name_AllowTransplant") ?? "null",
                        tooltip: () => ModEntry.Instance?.Helper.Translation.Get("NCarigon.GardenPotOptions/config_desc_AllowTransplant") ?? "null",
                        getValue: () => ModEntry.Instance!.ModConfig.AllowTransplant,
                        setValue: value => ModEntry.Instance!.ModConfig.AllowTransplant = value
                    );
                    configMenu.AddNumberOption(
                        mod: ModEntry.Instance!.ModManifest,
                        name: () => ModEntry.Instance?.Helper.Translation.Get("NCarigon.GardenPotOptions/config_name_HeartsGardenPot") ?? "null",
                        tooltip: () => ModEntry.Instance?.Helper.Translation.Get("NCarigon.GardenPotOptions/config_desc_HeartsGardenPot") ?? "null",
                        getValue: () => config.HeartsForGardenPot,
                        setValue: value => config.HeartsForGardenPot = value,
                        min: -1,
                        max: 10
                    );
                    configMenu.AddNumberOption(
                        mod: ModEntry.Instance!.ModManifest,
                        name: () => ModEntry.Instance?.Helper.Translation.Get("NCarigon.GardenPotOptions/config_name_HeartsRecipe") ?? "null",
                        tooltip: () => ModEntry.Instance?.Helper.Translation.Get("NCarigon.GardenPotOptions/config_desc_HeartsRecipe") ?? "null",
                        getValue: () => config.HeartsForRecipe,
                        setValue: value => config.HeartsForRecipe = value,
                        min: -1,
                        max: 10
                    );
                };
            }
        }
    }

    public interface IGenericModConfigMenuApi {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);

        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null!, string fieldId = null!);

        void AddTextOption(IManifest mod, Func<string> getValue, Action<string> setValue, Func<string> name, Func<string> tooltip = null!, string[] allowedValues = null!, Func<string, string> formatAllowedValue = null!, string fieldId = null!);

        void AddNumberOption(IManifest mod, Func<int> getValue, Action<int> setValue, Func<string> name, Func<string> tooltip = null!, int? min = null, int? max = null, int? interval = null, Func<int, string> formatValue = null!, string fieldId = null!);
    }
}
