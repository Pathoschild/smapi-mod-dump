/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CaptainSully/StardewMods
**
*************************************************/

namespace BetterTappers
{
    // <summary>The raw mod configuration.</summary>
    public class ModConfig
    {
        /// <summary>Logging tool</summary>
        private static readonly Log log = ModEntry.Instance.log;

        /*********
        ** Accessors
        *********/
        /// <summary>Whether the mod is disabled.</summary>
        public bool DisableAllModEffects { get; set; } = false;
        /// <summary>Whether the mod is disabled.</summary>
        public bool ChangeTapperTimes { get; set; } = true;
        /// <summary>Whether the mod is disabled.</summary>
        public bool TappersUseQuality { get; set; } = true;
        /// <summary>Whether the mod is disabled.</summary>
        public int TapperXP { get; set; } = 10;
        /// <summary>Whether the mod is disabled.</summary>
        public bool AllowAutomatedXP { get; set; } = true;
        /// <summary>Whether the mod is disabled.</summary>
        public bool GathererAffectsTappers { get; set; } = true;
        /// <summary>Whether the mod is disabled.</summary>
        public bool BotanistAffectsTappers { get; set; } = true;

        // Options for regular tappers
        /// <summary>Whether the mod is disabled.</summary>
        public float DaysForSyrups { get; set; } = 7f;
        /// <summary>Whether the mod is disabled.</summary>
        public float DaysForSap { get; set; } = 1f;
        /// <summary>Whether the mod is disabled.</summary>
        public float DaysForMushroom { get; set; } = 7f;

        // Options for hardwood tappers
        public float HeavyTapperMultiplier { get; set; } = 0.5f;
        /// <summary>Whether the mod is disabled.</summary>

        // Quality options
        /// <summary>Whether the mod is disabled.</summary>
        public bool ForageLevelAffectsQuality { get; set; } = true;
        /// <summary>Whether the mod is disabled.</summary>
        public bool TimesHarvestedAffectsQuality { get; set; } = true;
        /// <summary>Whether the mod is disabled.</summary>
        public bool TreeAgeAffectsQuality { get; set; } = true;
        /// <summary>Whether the mod is disabled.</summary>
        internal int Formula { get; set; } = 0;
        /// <summary>Whether the mod is disabled.</summary>
        internal int LvlCap { get; set; } = 0;

        // Debug mode
        /// <summary>General on the fly debug mode.</summary>
        public bool DebugMode { get; set; } = false;
        /// <summary>Log method calls.</summary>
        internal bool DebugMethods { get; set; } = false;
        /// <summary>Enable more specific logging.</summary>
        internal bool DebugLogic { get; set; } = false;
        /// <summary>Log patcher logic.</summary>
        internal bool DebugPatcher { get; set; } = false;
        

        //different outputs?
        //more outputs? (like 3-8 sap)


        /*********
        ** Public methods
        *********/
        /// <summary>Check for and reset any invalid configuration settings.</summary>
        public static void VerifyConfigValues(ModConfig config, Mod mod)
        {
            bool invalidConfig = false;

            if (config.TapperXP < 0)
            {
                invalidConfig = true;
                config.TapperXP = 0;
            }

            if (config.DaysForSyrups < 0)
            {
                invalidConfig = true;
                config.DaysForSyrups = 7f;
            }

            if (config.DaysForSap < 0)
            {
                invalidConfig = true;
                config.DaysForSap = 1f;
            }

            if (config.DaysForMushroom < 0)
            {
                invalidConfig = true;
                config.DaysForMushroom = 7f;
            }

            if (config.HeavyTapperMultiplier < 0)
            {
                invalidConfig = true;
                config.HeavyTapperMultiplier = 0.5f;
            }

            if (invalidConfig)
            {
                log.I("At least one config value was out of range and was reset.");
                mod.Helper.WriteConfig(config);
            }
        }

        /// <summary>Set up Generic Mod Config Menu integration.</summary>
        public static void SetUpModConfigMenu(ModConfig config, Mod mod)
        {
            // Get the Generic Mod Config Menu API
            IGenericModConfigMenuApi api = mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api is null) { return; }
            var manifest = mod.ModManifest;

            // Register the Generic Mod Config Menu API
            api.Register(manifest, () => config = new ModConfig(), delegate { mod.Helper.WriteConfig(config); VerifyConfigValues(config, mod); });

            // General mod settings. Some of these affect the other categories
            api.AddSectionTitle(manifest, text: () => "General");

            // Options to display
            // General
            api.AddBoolOption(manifest, () => config.DisableAllModEffects, (bool val) => config.DisableAllModEffects = val,
                    name: () => "Disable this mods effects", tooltip: () => "Game will follow vanilla behaviour if true.\n'True' overrides ALL other settings.");
            api.AddBoolOption(manifest, () => config.ChangeTapperTimes, (bool val) => config.ChangeTapperTimes = val,
                    name: () => "Enable modified production times", tooltip: () => "Let tappers use modified product times.\n'False' overrides the production times settings.");
            api.AddBoolOption(manifest, () => config.TappersUseQuality, (bool val) => config.TappersUseQuality = val,
                    name: () => "Enable quality", tooltip: () => "Lets tappers produce items with higher qualities.\n'False' overrides the quality section below.");
            api.AddNumberOption(manifest, () => config.TapperXP, (int val) => config.TapperXP = val,
                    name: () => "Tapper experience gain", tooltip: () => "Amount of experience gained for harvesting from tappers.\nMod default is 10, vanilla is 0.");
            api.AddBoolOption(manifest, () => config.AllowAutomatedXP, (bool val) => config.AllowAutomatedXP = val,
                    name: () => "XP is gained from automation", tooltip: () => "Whether or not autoharvesting tappers ('automate' mod for example) will give exp.");
            api.AddBoolOption(manifest, () => config.GathererAffectsTappers, (bool val) => config.GathererAffectsTappers = val,
                    name: () => "Enable Gatherer perk on tappers", tooltip: () => "The gatherer foraging perk (vanilla) gives a chance for double foraged items.");
            api.AddBoolOption(manifest, () => config.BotanistAffectsTappers, (bool val) => config.BotanistAffectsTappers = val,
                    name: () => "Enable Botanist perk on tappers", tooltip: () => "The botanist foraging perk (vanilla) makes forage items always irridium quality.");

            // Production times for normal tappers
            api.AddSectionTitle(manifest, text: () => "Tappers",
                tooltip: () => "These options affect production time of tappers.\nThis section requires 'Enable modified production times' to be true.");

            api.AddNumberOption(manifest, () => config.DaysForSyrups, (float val) => config.DaysForSyrups = val,
                    name: () => "Days for maple/oak/pine trees", tooltip: () => "Number of days for regular tappers to produce on listed trees.\nVanilla is 5-9 depending on tree type.");
            api.AddNumberOption(manifest, () => config.DaysForSap, (float val) => config.DaysForSap = val,
                    name: () => "Days for mahogany trees", tooltip: () => "Number of days for regular tappers to produce on mahogany trees.\nVanilla is 1.");
            api.AddNumberOption(manifest, () => config.DaysForMushroom, (float val) => config.DaysForMushroom = val,
                    name: () => "Days for mushroom trees", tooltip: () => "Number of days for regular tappers to produce on mushroom trees.\nVanilla is 1 or 2 or not at all based on season.\nNote that rules for *which* mushroom is produced are not changed in any way.");

            // Production time for heavy tappers
            api.AddSectionTitle(manifest, text: () => "Heavy Tappers",
                tooltip: () => "These options affect production time of heavy tappers.\nThis section requires 'Enable modified production times' to be true.");

            api.AddNumberOption(manifest, () => config.HeavyTapperMultiplier, (float val) => config.HeavyTapperMultiplier = val,
                    name: () => "Heavy tapper time multiplier", tooltip: () => "Defaults to half normal tappers, which is the same as vanilla.");

            // How to determine tapper product quality
            api.AddSectionTitle(manifest, text: () => "Tapper Product Quality");
            api.AddParagraph(manifest, text: () => "These options affect how output quality is determined. This section requires " +
                    "'Enable quality for tapper products' to be true. If all of these are false products will never have quality." +
                    "\nWith default settings, each of 'Forage level', 'Times harvested', and 'Tree age' are used together to determine the output.");

            api.AddBoolOption(manifest, () => config.ForageLevelAffectsQuality, (bool val) => config.ForageLevelAffectsQuality = val,
                    name: () => "Forage level affects quality", tooltip: () => "Your level of foraging will affect the quality of tapper products.");
            api.AddBoolOption(manifest, () => config.TimesHarvestedAffectsQuality, (bool val) => config.TimesHarvestedAffectsQuality = val,
                    name: () => "Times harvested affects quality", tooltip: () => "Number of times a tapper has been harvested will affect the quality of its products.");
            api.AddBoolOption(manifest, () => config.TreeAgeAffectsQuality, (bool val) => config.TreeAgeAffectsQuality = val,
                    name: () => "Tree age affects quality", tooltip: () => "Tree age will affect the quality of tapper products.");

            // Debuging
            api.AddParagraph(manifest, text: () => " ");
            api.AddSectionTitle(manifest, text: () => "Debuging");
            api.AddBoolOption(manifest, () => config.DebugMode, (bool val) => config.DebugMode = val,
                    name: () => "Debug mode", tooltip: () => "This is for helping me debug/test things.\nEnable only if you're trying to do the same.");
        }
    }
}