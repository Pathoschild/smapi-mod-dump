using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System;
using System.Linq;

namespace SunscreenMod
{
    /// <summary>The mod configuration model.</summary>
    public class ModConfig
    {
        static IModHelper Helper => ModEntry.Instance.Helper;
        static IMonitor Monitor => ModEntry.Instance.Monitor;

        internal static ModConfig Instance { get; private set; }

        static readonly ITranslationHelper i18n = Helper.Translation;

        static readonly string NL = Environment.NewLine;


        #region Properties and Fields for config values


        //==BASIC FEATURES==

        /// <summary>Enables sunburn effects from this mod for the current player.</summary>
        public bool EnableSunburn { get; set; } = true;

        /// <summary>Enables or disables sunburn chance during certain times of year.</summary>
        public string SunburnSeasons
        {
            get { return _sunburnSeasons; }
            set
            {
                if (SunburnSeasonsChoices.Contains(value)) _sunburnSeasons = value;
                else
                {
                    string fallback = SunburnSeasonsDefault;
                    _sunburnSeasons = fallback;
                    Monitor.Log(i18n.Get(
                        "SunburnSeasons.error",
                        new
                        {
                            value,
                            listSunburnSeasonsChoices = string.Join(", ", SunburnSeasonsChoices),
                            fallback
                        }), LogLevel.Warn);
                }
            }
        }
        private static readonly string[] SunburnSeasonsChoices = new string[] { "SummerOnly", "SpringSummerFall", "AllSeasons" };
        private static readonly string SunburnSeasonsDefault = SunburnSeasonsChoices[2]; // Default to "AllSeasons"
        private string _sunburnSeasons = SunburnSeasonsDefault;

        /// <summary>Reports tomorrow's UV Index on the TV weather channel.</summary>
        public bool WeatherReport { get; set; } = true;

        /// <summary>Changes skin display color to red when sunburnt.</summary>
        public bool SkinColorChange { get; set; } = true;

        /// <summary>Villagers react in shock when they see a sunburnt player.</summary>
        public bool VillagerReactions { get; set; } = true;


        //==ADVANCED SETTINGS==

        /// <summary>How many in-game hours sunscreen lasts before wearing off.</summary>
        public int SunscreenDuration
        {
            get { return _sunscreenDuration; }
            set
            {
                if (value > 0) _sunscreenDuration = value;
                else
                {
                    _sunscreenDuration = 3; // Default to 3 hours
                    Monitor.Log(i18n.Get(
                        "SunscreenDuration.error",
                        new
                        {
                            value,
                            _sunscreenDuration
                        }), LogLevel.Warn);
                }
            }
        }
        private int _sunscreenDuration = 3;

        /// <summary>Loss in new day starting health per level of sunburn damage.</summary>
        public int HealthLossPerLevel
        {
            get { return _healthLossPerLevel; }
            set
            {
                if (value >= 0) _healthLossPerLevel = value;
                else
                {
                    _healthLossPerLevel = 20; // Default to 20 health points
                    Monitor.Log(i18n.Get(
                        "HealthLossPerLevel.error",
                        new
                        {
                            value,
                            _healthLossPerLevel
                        }), LogLevel.Warn);
                }
            }
        }
        private int _healthLossPerLevel = 20;

        /// <summary>Loss in new day starting energy per level of sunscreen damage.</summary>
        public int EnergyLossPerLevel
        {
            get { return _energyLossPerLevel; }
            set
            {
                if (value >= 0) _energyLossPerLevel = value;
                else
                {
                    _energyLossPerLevel = 50; // Default to 50 stamina points
                    Monitor.Log(i18n.Get(
                        "EnergyLossPerLevel.error",
                        new
                        {
                            value,
                            _energyLossPerLevel
                        }), LogLevel.Warn);
                }
            }
        }
        private int _energyLossPerLevel = 50;

        /// <summary>Active sunburn (no matter what severity) gives a -1 speed debuff.</summary>
        public bool SunburnSpeedDebuff { get; set; } = true;

        /*/// <summary>Debuff to defense stat per level of sunscreen damage.</summary>
        public int DefenseLossPerLevel
        {
            get { return _defenseLossPerLevel; }
            set
            {
                if (value >= 0) _defenseLossPerLevel = value;
                else
                {
                    _defenseLossPerLevel = 1; // Default to 1 defense stat
                    Monitor.Log(i18n.Get(
                        "DefenseLossPerLevel.error",
                        new
                        {
                            value,
                            _defenseLossPerLevel
                        }), LogLevel.Warn);
                }
            }
        }
        private int _defenseLossPerLevel = 1;

        /// <summary>How many days to heal one level of sunburn damage naturally, without treatment.</summary>
        public int RecoveryDaysPerLevel
        {
            get { return _recoveryDaysPerLevel; }
            set
            {
                if (value > 0) _recoveryDaysPerLevel = value;
                else
                {
                    _recoveryDaysPerLevel = 1; // Default to 1 day
                    Monitor.Log(i18n.Get(
                        "RecoveryDaysPerLevel.error",
                        new
                        {
                            value,
                            _recoveryDaysPerLevel
                        }), LogLevel.Warn);
                }
            }
        }
        private int _recoveryDaysPerLevel = 1;

        /// <summary>How many levels of sunburn damage can stack with additive effects.</summary>
        public int MaximumSeverity //DON'T USE???
        {
            get { return _maximumSeverity; }
            set
            {
                if (value > 0) _maximumSeverity = value;
                else
                {
                    _maximumSeverity = 3; // Default to 3 levels
                    Monitor.Log(i18n.Get(
                        "MaximumSeverity.error",
                        new
                        {
                            value,
                            _maximumSeverity
                        }), LogLevel.Warn);
                }
            }
        }
        private int _maximumSeverity = 3;*/


        //==MULTIPLAYER==

        /// <summary>Choose which skin color IDs are overwritten with sunburn colors. Important in multiplayer!</summary>
        public int[] BurnSkinColorIndex
        {
            get { return _burnSkinColorIndex; }
            set
            {
                if (value.Length != 3) // Wrong length
                {
                    Monitor.Log(i18n.Get(
                        "BurnSkinColorIndex.error.wrongLength",
                        new { valueList = string.Join(", ", value) }
                        ), LogLevel.Warn);
                    return;
                }
                else if (value[0] == value[1] || value[0] == value[2] || value[1] == value[2]) // Not unique
                {
                    Monitor.Log(i18n.Get(
                        "BurnSkinColorIndex.error.notUnique",
                        new { valueList = string.Join(", ", value) }
                        ), LogLevel.Warn);
                    return;
                }
                else if (!value.All(num => num >= 1 && num <= 24)) // Out of range for skin index
                {
                    Monitor.Log(i18n.Get(
                        "BurnSkinColorIndex.error.outOfRange",
                        new { valueList = string.Join(", ", value) }
                        ), LogLevel.Warn);
                    return;
                }
                _burnSkinColorIndex = value;
            }
        }
        private int[] _burnSkinColorIndex = new int[3] { 19, 20, 21 };

        //These getters and setters used in GMCM settings menu

        /// <summary>Skin color ID to use for level 1 sunburn in multiplayer games.</summary>
        private int SkinColorIndex1
        {
            get { return _burnSkinColorIndex[0]; }
            set
            {
                if (!(value >= 1 && value <= 24)) // Out of range for skin index
                {
                    Monitor.Log(i18n.Get(
                        "SkinColorIndex.error.outOfRange",
                        new { value }
                        ), LogLevel.Warn);
                    return;
                }
                _burnSkinColorIndex[0] = value;
            }
        }

        /// <summary>Skin color ID to use for level 2 sunburn in multiplayer games.</summary>
        private int SkinColorIndex2
        {
            get { return _burnSkinColorIndex[1]; }
            set
            {
                if (!(value >= 1 && value <= 24)) // Out of range for skin index
                {
                    Monitor.Log(i18n.Get(
                        "SkinColorIndex.error.outOfRange",
                        new { value }
                        ), LogLevel.Warn);
                    return;
                }
                _burnSkinColorIndex[1] = value;
            }
        }

        /// <summary>Skin color ID to use for level 3 sunburn in multiplayer games.</summary>
        private int SkinColorIndex3
        {
            get { return _burnSkinColorIndex[2]; }
            set
            {
                if (!(value >= 1 && value <= 24)) // Out of range for skin index
                {
                    Monitor.Log(i18n.Get(
                        "SkinColorIndex.error.outOfRange",
                        new { value }
                        ), LogLevel.Warn);
                    return;
                }
                _burnSkinColorIndex[2] = value;
            }
        }

        //==DEVELOPER OPTIONS==

        /// <summary>Enable noisy console debug messages.</summary>
        public bool DebugMode { get; set; } = false;

        #endregion

        #region ModConfig constructor
        /// <summary>Currently empty constructor method.</summary>
        public ModConfig()
        {

        }
        #endregion

        #region Utility functions and fields to access config data
        internal bool SunburnPossible(SDate date)
        {
            if (EnableSunburn)
            {
                if ((SunburnSeasons == "AllSeasons") // Season of input date matches config settings
                    || (SunburnSeasons == "SummerOnly" && date.Season == "summer")
                    || (SunburnSeasons == "SpringSummerFall" && date.Season != "winter"))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Generic Mod Config Menu helper functions
        /// <summary>Load user config options from file using smapi's Config API.</summary>
        internal static void Load()
        {
            Instance = Helper.ReadConfig<ModConfig>();
        }
        /// <summary>Save user config options and invalidate cache for affected assets.</summary>
        internal static void Save()
        {
            Helper.WriteConfig(Instance);
            ModConfig.Print();
            Helper.Content.InvalidateCache(asset // Trigger changed assets to reload on next use.
                => asset.AssetNameEquals("Characters\\Farmer\\skinColors")
                || asset.AssetNameEquals("Strings\\StringsFromCSFiles"));
        }

        /// <summary>Reset all config options to their default values.</summary>
        internal static void Reset()
        {
            Instance = new ModConfig();
        }

        /// <summary>Register API stuff for Generic Mod Config Menu.</summary>
        internal static void SetUpMenu()
        {
            var api = Helper.ModRegistry.GetApi<GenericModConfigMenu.IApi>
                ("spacechase0.GenericModConfigMenu");

            if (api == null)
                return;

            var manifest = ModEntry.Instance.ModManifest;
            api.RegisterModConfig(manifest, Reset, Save);

            api.RegisterLabel(manifest, i18n.Get("BasicFeatures.title"), "");

            api.RegisterSimpleOption(manifest,
                i18n.Get("EnableSunburn.name"),
                i18n.Get("EnableSunburn.description"),
                () => Instance.EnableSunburn,
                (bool val) => Instance.EnableSunburn = val);

            api.RegisterChoiceOption(manifest,
                i18n.Get("SunburnSeasons.name"),
                i18n.Get("SunburnSeasons.description", new { NL }),
                () => Instance.SunburnSeasons,
                (string val) => Instance.SunburnSeasons = val,
                ModConfig.SunburnSeasonsChoices);

            api.RegisterSimpleOption(manifest,
                i18n.Get("WeatherReport.name"),
                i18n.Get("WeatherReport.description"),
                () => Instance.WeatherReport,
                (bool val) => Instance.WeatherReport = val);

            api.RegisterSimpleOption(manifest,
                i18n.Get("SkinColorChange.name"),
                i18n.Get("SkinColorChange.description"),
                () => Instance.SkinColorChange,
                (bool val) => Instance.SkinColorChange = val);

            api.RegisterSimpleOption(manifest,
                i18n.Get("VillagerReactions.name"),
                i18n.Get("VillagerReactions.description"),
                () => Instance.VillagerReactions,
                (bool val) => Instance.VillagerReactions = val);

            api.RegisterLabel(manifest, i18n.Get("AdvancedSettings.title"), "");

            api.RegisterSimpleOption(manifest,
                i18n.Get("SunscreenDuration.name"),
                i18n.Get("SunscreenDuration.description", new { NL }),
                () => Instance.SunscreenDuration,
                (int val) => Instance.SunscreenDuration = val);

            api.RegisterSimpleOption(manifest,
                i18n.Get("HealthLossPerLevel.name"),
                i18n.Get("HealthLossPerLevel.description", new { NL }),
                () => Instance.HealthLossPerLevel,
                (int val) => Instance.HealthLossPerLevel = val);

            api.RegisterSimpleOption(manifest,
                i18n.Get("EnergyLossPerLevel.name"),
                i18n.Get("EnergyLossPerLevel.description", new { NL }),
                () => Instance.EnergyLossPerLevel,
                (int val) => Instance.EnergyLossPerLevel = val);

            api.RegisterSimpleOption(manifest,
                i18n.Get("SunburnSpeedDebuff.name"),
                i18n.Get("SunburnSpeedDebuff.description"),
                () => Instance.SunburnSpeedDebuff,
                (bool val) => Instance.SunburnSpeedDebuff = val);

            /*api.RegisterSimpleOption(manifest,
                i18n.Get("DefenseLossPerLevel.name"),
                i18n.Get("DefenseLossPerLevel.description", new { NL }),
                () => Instance.DefenseLossPerLevel,
                (int val) => Instance.DefenseLossPerLevel = val);

            api.RegisterSimpleOption(manifest,
                i18n.Get("RecoveryDaysPerLevel.name"),
                i18n.Get("RecoveryDaysPerLevel.description", new { NL }),
                () => Instance.RecoveryDaysPerLevel,
                (int val) => Instance.RecoveryDaysPerLevel = val);

            api.RegisterSimpleOption(manifest,
                i18n.Get("MaximumSeverity.name"),
                i18n.Get("MaximumSeverity.description", new { NL }),
                () => Instance.MaximumSeverity,
                (int val) => Instance.MaximumSeverity = val);*/

            api.RegisterLabel(manifest, i18n.Get("Multiplayer.title"), "");

            api.RegisterSimpleOption(manifest,
                i18n.Get("SkinColorIndex1.name"),
                i18n.Get("SkinColorIndex1.description", new { NL }),
                () => Instance.SkinColorIndex1,
                (int val) => Instance.SkinColorIndex1 = val);

            api.RegisterSimpleOption(manifest,
                i18n.Get("SkinColorIndex2.name"),
                i18n.Get("SkinColorIndex2.description", new { NL }),
                () => Instance.SkinColorIndex2,
                (int val) => Instance.SkinColorIndex2 = val);

            api.RegisterSimpleOption(manifest,
                i18n.Get("SkinColorIndex3.name"),
                i18n.Get("SkinColorIndex3.description", new { NL }),
                () => Instance.SkinColorIndex3,
                (int val) => Instance.SkinColorIndex3 = val);

            api.RegisterLabel(manifest, i18n.Get("DeveloperOptions.title"), "");

            api.RegisterSimpleOption(manifest,
                i18n.Get("DebugMode.name"),
                i18n.Get("DebugMode.description", new { NL }),
                () => Instance.DebugMode,
                (bool val) => Instance.DebugMode = val);

            Monitor.Log("Added UV Index (Sunscreen Mod) config to GMCM", LogLevel.Info);
        }
        #endregion

        /// <summary>Prints current config values to the console.</summary>
        internal static void Print()
        {
            Monitor.Log(
                $"CONFIG\n" +
                $"    ====================\n" +
                $"    EnableSunburn: {Instance.EnableSunburn.ToString().ToLower()}\n" +
                $"    SunburnSeasons: \"{Instance.SunburnSeasons}\"\n" +
                $"    WeatherReport: {Instance.WeatherReport.ToString().ToLower()}\n" +
                $"    SkinColorChange: {Instance.SkinColorChange.ToString().ToLower()}\n" +
                $"    VillagerReactions: {Instance.VillagerReactions.ToString().ToLower()}\n" +
                $"    SunscreenDuration: {Instance.SunscreenDuration}\n" +
                $"    HealthLossPerLevel: {Instance.HealthLossPerLevel}\n" +
                $"    EnergyLossPerLevel: {Instance.EnergyLossPerLevel}\n" +
                $"    SunburnSpeedDebuff: {Instance.SunburnSpeedDebuff.ToString().ToLower()}\n" +
                $"    BurnSkinColorIndex: [ {string.Join(", ", Instance.BurnSkinColorIndex)} ]\n" +
                $"    DebugMode: {Instance.DebugMode.ToString().ToLower()}\n" +
                $"    ====================", LogLevel.Debug); // Use .ToLower to make bool capitalization match config.json format
        }
    }
}