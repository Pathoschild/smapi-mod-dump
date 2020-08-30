using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicConversationTopics
{
    /// <summary>The mod configuration model.</summary>
    public class ModConfig
    {
        protected static IModHelper Helper => ModEntry.Instance.Helper;
        protected static IMonitor Monitor => ModEntry.Instance.Monitor;

        internal static ModConfig Instance { get; private set; }

        protected static ITranslationHelper i18n = Helper.Translation;

        static readonly string NL = Environment.NewLine;


        #region Properties and Fields for config values
        /// <summary>Enables or disable conversation topics from certain event types.</summary>
        public bool EnableAchievements { get; set; } = true;
        public bool EnableFestivals { get; set; } = true;
        public bool EnableHeartEvents { get; set; } = true;
        public bool EnableQuests { get; set; } = true;
        public bool EnableSecretNotes { get; set; } = true;
        public bool EnableStoryTriggers { get; set; } = true;


        /// <summary>Toggles debug mode on and off.</summary>
        public bool DebugMode { get; set; } = true;
        #endregion


        #region Utility functions and fields to access config data
        internal StardewModdingAPI.LogLevel Log()
        {
            if (DebugMode)
            {
                return LogLevel.Debug;
            }
            else return LogLevel.Trace;
        }

        internal static readonly List<string> NPCs = new List<string>
        {
            "Abigail",
            "Alex",
            "Caroline",
            "Clint",
            "Demetrius",
            "Dwarf",
            "Elliott",
            "Emily",
            "Evelyn",
            "George",
            "Gil",
            "Gus",
            "Haley",
            "Harvey",
            "Jas",
            "Jodi",
            "Kent",
            "Krobus",
            "Leah",
            "Lewis",
            "Linus",
            "Marnie",
            "Maru",
            "Mister Qi",
            "Pam",
            "Penny",
            "Pierre",
            "Robin",
            "Sam",
            "Sandy",
            "Sebastian",
            "Shane",
            "Vincent",
            "Willy",
            "Wizard"
        };

        internal static readonly List<string> EventLocations = new List<string>
        {
            "AbandonedJojaMart",
            "AnimalShop",
            "ArchaeologyHouse",
            "Backwoods",
            "BathHouse_Pool",
            "Beach",
            "BusStop",
            "CommunityCenter",
            "ElliottHouse",
            "Farm",
            "FarmHouse",
            "Forest",
            "HaleyHouse",
            "HarveyRoom",
            "Hospital",
            "JoshHouse",
            "LeahHouse",
            "ManorHouse",
            "Mine",
            "Mountain",
            "Railroad",
            "Saloon",
            "SamHouse",
            "SandyHouse",
            "ScienceHouse",
            "SebastianRoom",
            "SeedShop",
            "Sewer",
            "Sunroom",
            "Temp",
            "Tent",
            "Town",
            "Trailer",
            "Trailer_Big",
            "WizardHouse",
            "Woods"
        };

        internal static bool AssetMatch(IAssetInfo asset, string path, List<string> targets)
        {
            foreach(string target in targets)
            {
                if (asset.AssetNameEquals(path + "\\" + target))
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
        internal static void Save()
        {
            Helper.WriteConfig(Instance);
            //ModConfig.Print();
            Helper.Content.InvalidateCache(asset // Trigger changed assets to reload on next use.
                => asset.DataType == typeof(Dictionary<string, string>)
                && (AssetMatch(asset, "Characters\\Dialogue", NPCs) || AssetMatch(asset, "Data\\Events", EventLocations))
                );
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

            ISemanticVersion gmcm = Helper.ModRegistry.Get("spacechase0.GenericModConfigMenu").Manifest.Version;
            ISemanticVersion req = new SemanticVersion("1.1.0");
            if (gmcm.IsOlderThan(req))
            {
                Monitor.Log($"Installed version {gmcm} of GMCM is not supported. Please update Generic Mod Config Menu to version {req} or newer to enable its features for Dynamic Conversation Topics mod.", LogLevel.Warn);
                return;
            }

            var manifest = ModEntry.Instance.ModManifest;
            api.RegisterModConfig(manifest, Reset, Save);

            api.RegisterLabel(manifest, 
                //i18n.Get("DialogueOptions.title"),
                "Debugging Tools",
                "");

            api.RegisterSimpleOption(manifest,
                "Debug mode", //i18n.Get("GenderNeutrality.name"),
                "Visible console logging, conversation topic alerts, and dialogue box labels", //i18n.Get("GenderNeutrality.description"),
                () => Instance.DebugMode,
                (bool val) => Instance.DebugMode = val);

            Monitor.Log("Added DCT config to GMCM", LogLevel.Info);
        }
        #endregion
    }
}
