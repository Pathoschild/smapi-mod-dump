using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AngryGrandpa
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
        /// <summary>Changes the dialogue used during evaluation and re-evaluation events.</summary>
        public string GrandpaDialogue 
        { 
            get { return _grandpaDialogue; } 
            set 
            {
                if (GrandpaDialogueChoices.Contains(value)) _grandpaDialogue = value;
                else
                {
                    string fallback = GrandpaDialogueDefault;
                    _grandpaDialogue = fallback;
                    Monitor.Log(i18n.Get(
                        "GrandpaDialogue.error",
                        new
                        {
                            value,
                            listGrandpaDialogueChoices = string.Join(", ", GrandpaDialogueChoices),
                            fallback
                        }), LogLevel.Warn);
                }
            } 
        }
        private static readonly string[] GrandpaDialogueChoices = new string[] { "Original", "Vanilla", "Nuclear" };
        private static readonly string GrandpaDialogueDefault = GrandpaDialogueChoices[0]; // Default to "Original"
        private string _grandpaDialogue = GrandpaDialogueDefault;

        /// <summary>Removes references to player gender from dialogue strings.</summary>
        public bool GenderNeutrality
        {
            get 
            {
                if (_genderNeutrality == null) // Determine the appropriate default setting, set it, and then return it
                {
                    Monitor.Log("Looking for Hana.GenderNeutralityMod", LogLevel.Debug);
                    bool hasMod = Helper.ModRegistry.IsLoaded("Hana.GenderNeutralityMod");
                    if (hasMod)
                    {
                        Monitor.Log($"GenderNeutralityMod detected. Setting up AngryGrandpa config with GenderNeutrality: {hasMod.ToString().ToLower()}", LogLevel.Info);
                    }
                    else
                    {
                        Monitor.Log($"GenderNeutralityMod not detected. Setting up AngryGrandpa config with GenderNeutrality: {hasMod.ToString().ToLower()}", LogLevel.Info);
                    }
                    _genderNeutrality = hasMod; // set default
                }
                return _genderNeutrality.GetValueOrDefault(); 
            }
            set { _genderNeutrality = value; }
        }
        private bool? _genderNeutrality = null; // Initialized with null before determining which default setting to use

        /// <summary>Gives grandpa a variety of new facial expressions.</summary>
        public bool ExpressivePortraits
        {
            get { return _expressivePortraits; }
            set
            {
                _expressivePortraits = value;
                setPortraitTokens();
            }
        }
        private bool _expressivePortraits; // Initialize this one in the constructor

        /// <summary>Changes how points are scored and how many are required to earn 4 candles.</summary>
        public string ScoringSystem
        {
            get { return _scoringSystem; }
            set
            {
                if (ScoringSystemChoices.Contains(value)) _scoringSystem = value;
                else
                {
                    string fallback = ScoringSystemDefault;
                    _scoringSystem = fallback;
                    Monitor.Log(i18n.Get(
                        "ScoringSystem.error",
                        new
                        {
                            value,
                            listScoringSystemChoices = string.Join(", ", ScoringSystemChoices),
                            fallback
                        }), LogLevel.Warn);
                }
            }
        }
        private static readonly string[] ScoringSystemChoices = new string[] { "Original", "Vanilla", "Hard", "Expert" };
        private static readonly string ScoringSystemDefault = ScoringSystemChoices[1]; // Default to "Vanilla"
        private string _scoringSystem = ScoringSystemDefault;

        /// <summary>How many in-game years to wait before grandpa's first visit.</summary>
        public int YearsBeforeEvaluation 
        {
            get { return _yearsBeforeEvaluation; }
            set {
                if (value >= 0) _yearsBeforeEvaluation = value;
                else
                {
                    _yearsBeforeEvaluation = 2; // Default to 2 years
                    Monitor.Log(i18n.Get(
                        "YearsBeforeEvaluation.error",
                        new
                        {
                            value,
                            _yearsBeforeEvaluation
                        }), LogLevel.Warn); 
                }
            }
        }
        private int _yearsBeforeEvaluation = 2;

        /// <summary>Displays your raw score during the evaluation.</summary>
        public bool ShowPointsTotal { get; set; } = true;

        /// <summary>Gives new bonus rewards for earning 1-3 candles.</summary>
        public bool BonusRewards { get; set; } = true;

        /// <summary>In a multiplayer game, allows each farmhand to receive their own Statue of Perfection.</summary>
        public bool StatuesForFarmhands { get; set; } = true;

        /// <summary>(Unimplemented) Allow custom score thresholds for earning candles.</summary>
        private int[] CustomCandleScores // Change this to public when I update to allow custom configs
        { 
            get { return _customCandleScores; }
            set 
            { 
                if ( !(value.Length == 4 && value[0] == 0) ) // Wrong length or first number not zero
                {
                    Monitor.Log(i18n.Get(
                        "CustomCandleScores.error.wrongLengthOrNotZero",
                        new { valueList = string.Join(", ", value) }
                        ), LogLevel.Warn);
                    return;
                }
                else if ( !(value[0] <= value[1] && value[1] <= value[2] && value[2] <= value[3]) ) // Not in ascending order
                {
                    Monitor.Log(i18n.Get(
                        "CustomCandleScores.error.notAscendingOrder",
                        new { valueList = string.Join(", ", value) }
                        ), LogLevel.Warn);
                    return;
                }
                _customCandleScores = value;
            }
        }
        private int[] _customCandleScores = new int[4] { 0, 4, 8, 12 };
        #endregion

        #region ModConfig constructor
        /// <summary>Constructor will let ExpressivePortraits default to true.</summary>
        public ModConfig()
        {
            ExpressivePortraits = true; // This makes sure setPortraitTokens runs on setup
        }
        #endregion

        #region Utility functions and fields to access config data
        internal int GetScoreForCandles(int candles)
        {
            if (candles < 1 || candles > 4)
            {
                throw new System.ArgumentOutOfRangeException("candles", candles, "candles must be an integer between 1 and 4 inclusive.");
            }
            int[] candleScores;
            if (ScoringSystem == "Original" || ScoringSystem == "Vanilla") 
            {
                candleScores = new int[4] { 0, 4, 8, 12 }; 
            }
            else if (ScoringSystem == "Hard") 
            {
                candleScores = new int[4] { 0, 10, 14, 18 }; 
            }
            else if (ScoringSystem == "Expert")
            {
                candleScores = new int[4] { 0, 15, 18, 21 };
            }
            else if (ScoringSystem == "Custom")
            {
                candleScores = CustomCandleScores;
            }
            else { throw new System.InvalidOperationException("ModConfig.ScoringSystem has an unaccounted-for value."); }
            return candleScores[candles - 1];
        }

        internal int GetMaxScore()
        {
            if (ScoringSystem == "Original")
            {
                return 13;
            }
            else return 21;
        }

        private static readonly List<string> PortraitNames = 
            new List<string>
        {
            "gpaNeutral",
            "gpaHappy",
            "gpaTears",
            "gpaShock",
            "gpaLove",
            "gpaAngry",
            "gpaSigh",
            "gpaRage",
            "gpaFrown",
            "gpaStern",
            "gpaSurprise",
            "gpaJoy"
        };

        // EventEditor and EvaluationEditor each grab this dictionary for translation tokens
        internal Dictionary<string, string> PortraitTokens = new Dictionary<string, string>();

        private void setPortraitTokens()
        {
            PortraitTokens.Clear();
            int count = 0;
            foreach (string emotion in PortraitNames)
            {
                if (ExpressivePortraits)
                {
                    PortraitTokens[emotion] = "$" + count.ToString();
                }
                else PortraitTokens[emotion] = "";

                count++;
            }
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
            ModConfig.Print();
            Helper.Content.InvalidateCache(asset // Trigger changed assets to reload on next use.
                => asset.AssetNameEquals("Strings\\Locations") 
                || asset.AssetNameEquals("Data\\mail")
                || asset.AssetNameEquals("Data\\Events\\Farmhouse")
                || asset.AssetNameEquals("Data\\Events\\Farm")
                || asset.AssetNameEquals("Portraits\\Grandpa"));
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

            api.RegisterLabel(manifest, i18n.Get("DialogueOptions.title"), "");

            api.RegisterChoiceOption(manifest, 
                i18n.Get("GrandpaDialogue.name"),
                i18n.Get("GrandpaDialogue.description", new { NL } ),
                () => Instance.GrandpaDialogue,
                (string val) => Instance.GrandpaDialogue = val,
                ModConfig.GrandpaDialogueChoices);

            api.RegisterSimpleOption(manifest,
                i18n.Get("GenderNeutrality.name"),
                i18n.Get("GenderNeutrality.description"),
                () => Instance.GenderNeutrality,
                (bool val) => Instance.GenderNeutrality = val);

            api.RegisterSimpleOption(manifest,
                i18n.Get("ExpressivePortraits.name"),
                i18n.Get("ExpressivePortraits.description"),
                () => Instance.ExpressivePortraits,
                (bool val) => Instance.ExpressivePortraits = val);

            api.RegisterLabel(manifest, "", "");
            api.RegisterLabel(manifest, i18n.Get("ScoringOptions.title"), "");

            api.RegisterChoiceOption(manifest,
                i18n.Get("ScoringSystem.name"),
                i18n.Get("ScoringSystem.description", new { NL }),
                () => Instance.ScoringSystem,
                (string val) => Instance.ScoringSystem = val,
                ModConfig.ScoringSystemChoices);

            api.RegisterSimpleOption(manifest,
                i18n.Get("YearsBeforeEvaluation.name"),
                i18n.Get("YearsBeforeEvaluation.description", new { NL }),
                () => Instance.YearsBeforeEvaluation,
                (int val) => Instance.YearsBeforeEvaluation = val);

            api.RegisterSimpleOption(manifest,
                i18n.Get("ShowPointsTotal.name"),
                i18n.Get("ShowPointsTotal.description"),
                () => Instance.ShowPointsTotal,
                (bool val) => Instance.ShowPointsTotal = val);

            api.RegisterLabel(manifest, "", "");
            api.RegisterLabel(manifest, i18n.Get("Rewards.title"), "");

            api.RegisterSimpleOption(manifest,
                i18n.Get("BonusRewards.name"),
                i18n.Get("BonusRewards.description"),
                () => Instance.BonusRewards,
                (bool val) => Instance.BonusRewards = val);

            api.RegisterSimpleOption(manifest,
                i18n.Get("StatuesForFarmhands.name"),
                i18n.Get("StatuesForFarmhands.description"),
                () => Instance.StatuesForFarmhands,
                (bool val) => Instance.StatuesForFarmhands = val);

            Monitor.Log("Added Angry Grandpa Config to GMCM", LogLevel.Info);
        }
        #endregion

        /// <summary>Prints current config values to the console.</summary>
        internal static void Print()
        {
            Monitor.Log(
                $"CONFIG\n" +
                $"    ====================\n" +
                $"    GrandpaDialogue: \"{Instance.GrandpaDialogue}\"\n" +
                $"    GenderNeutrality: {Instance.GenderNeutrality.ToString().ToLower()}\n" +
                $"    ExpressivePortraits: {Instance.ExpressivePortraits.ToString().ToLower()}\n" +
                $"    ScoringSystem: \"{Instance.ScoringSystem}\"\n" +
                $"    YearsBeforeEvaluation: {Instance.YearsBeforeEvaluation}\n" +
                $"    ShowPointsTotal: {Instance.ShowPointsTotal.ToString().ToLower()}\n" +
                $"    BonusRewards: {Instance.BonusRewards.ToString().ToLower()}\n" +
                $"    StatuesForFarmhands: {Instance.StatuesForFarmhands.ToString().ToLower()}\n" +
                $"    ====================", LogLevel.Debug); // Use .ToLower to make bool capitalization match config.json format
        }
    }
}
