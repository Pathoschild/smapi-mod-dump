/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Newtonsoft.Json;
using StardewArchipelago.GameModifications.EntranceRandomizer;
using StardewArchipelago.Locations.CodeInjections.Vanilla;
using StardewModdingAPI;
using StardewValley;

namespace StardewArchipelago.Archipelago
{
    public class SlotData
    {
        private const string GOAL_KEY = "goal";
        private const string STARTING_MONEY_KEY = "starting_money";
        private const string PROFIT_MARGIN_KEY = "profit_margin";
        private const string ENTRANCE_RANDOMIZATION_KEY = "entrance_randomization";
        private const string SEASON_RANDOMIZATION_KEY = "season_randomization";
        private const string CROPSANITY_KEY = "cropsanity";
        private const string BACKPACK_PROGRESSION_KEY = "backpack_progression";
        private const string TOOL_PROGRESSION_KEY = "tool_progression";
        private const string ELEVATOR_PROGRESSION_KEY = "elevator_progression";
        private const string SKILLS_PROGRESSION_KEY = "skill_progression";
        private const string BUILDING_PROGRESSION_KEY = "building_progression";
        private const string FESTIVAL_OBJECTIVES_KEY = "festival_locations";
        private const string ARCADE_MACHINES_KEY = "arcade_machine_locations";
        private const string SPECIAL_ORDERS_KEY = "special_order_locations";
        private const string HELP_WANTED_LOCATIONS_KEY = "help_wanted_locations";
        private const string FISHSANITY_KEY = "fishsanity";
        private const string MUSEUMSANITY_KEY = "museumsanity";
        private const string FRIENDSANITY_KEY = "friendsanity";
        private const string FRIENDSANITY_HEART_SIZE_KEY = "friendsanity_heart_size";
        private const string EXCLUDE_GINGER_ISLAND_KEY = "exclude_ginger_island";
        private const string TRAP_ITEMS_KEY = "trap_items";
        private const string MULTI_SLEEP_ENABLED_KEY = "multiple_day_sleep_enabled";
        private const string MULTI_SLEEP_COST_KEY = "multiple_day_sleep_cost";
        private const string EXPERIENCE_MULTIPLIER_KEY = "experience_multiplier";
        private const string FRIENDSHIP_MULTIPLIER_KEY = "friendship_multiplier";
        private const string DEBRIS_MULTIPLIER_KEY = "debris_multiplier";
        private const string QUICK_START_KEY = "quick_start";
        private const string GIFTING_KEY = "gifting";
        private const string BANKING_KEY = "banking";
        private const string BANK_TAX_KEY = "bank_tax";
        private const string DEATH_LINK_KEY = "death_link";
        private const string SEED_KEY = "seed";
        private const string MODIFIED_BUNDLES_KEY = "modified_bundles";
        private const string MODIFIED_ENTRANCES_KEY = "randomized_entrances";
        // private const string RANDOMIZE_NPC_APPEARANCES_KEY = "randomize_appearances";
        // private const string RANDOMIZE_NPC_APPEARANCES_DAILY_KEY = "randomize_appearances_daily";
        private const string MULTIWORLD_VERSION_KEY = "client_version";
        private const string MOD_LIST_KEY = "mods";
        
        private Dictionary<string, object> _slotDataFields;
        private IMonitor _console;

        public string SlotName { get; private set; }
        public Goal Goal { get; private set; }
        public int StartingMoney { get; private set; }
        public double ProfitMargin { get; private set; }
        public EntranceRandomization EntranceRandomization { get; private set; }
        public SeasonRandomization SeasonRandomization { get; private set; }
        public Cropsanity Cropsanity { get; private set; }
        public BackpackProgression BackpackProgression { get; private set; }
        public ToolProgression ToolProgression { get; private set; }
        public ElevatorProgression ElevatorProgression { get; private set; }
        public SkillsProgression SkillProgression { get; private set; }
        public BuildingProgression BuildingProgression { get; private set; }
        public FestivalLocations FestivalLocations { get; private set; }
        public ArcadeLocations ArcadeMachineLocations { get; private set; }
        public SpecialOrderLocations SpecialOrderLocations { get; private set; }
        public int HelpWantedLocationNumber { get; private set; }
        public Fishsanity Fishsanity { get; private set; }
        public Museumsanity Museumsanity { get; private set; }
        public Friendsanity Friendsanity { get; private set; }
        public int FriendsanityHeartSize { get; private set; }
        public bool ExcludeGingerIsland { get; private set; }
        public TrapItemsDifficulty TrapItemsDifficulty { get; private set; }
        public bool EnableMultiSleep { get; private set; }
        public int MultiSleepCostPerDay { get; private set; }
        public double ExperienceMultiplier { get; private set; }
        public double FriendshipMultiplier { get; private set; }
        public DebrisMultiplier DebrisMultiplier { get; private set; }
        public bool QuickStart { get; private set; }
        public bool Gifting { get; private set; }
        public bool Banking { get; private set; }
        public double BankTax { get; private set; }
        public bool DeathLink { get; private set; }
        public string Seed { get; private set; }
        public string MultiworldVersion { get; private set; }
        public BundlesManager Bundles { get; set; }
        public Dictionary<string, string> ModifiedEntrances { get; set; }
        public AppearanceRandomization AppearanceRandomization { get; set; }
        public bool AppearanceRandomizationDaily { get; set; }
        public ModsManager Mods { get; set; }

        public SlotData(string slotName, Dictionary<string, object> slotDataFields, IMonitor console)
        {
            SlotName = slotName;
            _slotDataFields = slotDataFields;
            _console = console;

            Goal = GetSlotSetting(GOAL_KEY, Goal.CommunityCenter);
            StartingMoney = GetSlotSetting(STARTING_MONEY_KEY, 500);
            ProfitMargin = GetSlotSetting(PROFIT_MARGIN_KEY, 100) / 100.0;
            EntranceRandomization = GetSlotSetting(ENTRANCE_RANDOMIZATION_KEY, EntranceRandomization.Disabled);
            SeasonRandomization = GetSlotSetting(SEASON_RANDOMIZATION_KEY, SeasonRandomization.Disabled);
            Cropsanity = GetSlotSetting(CROPSANITY_KEY, Cropsanity.Disabled);
            BackpackProgression = GetSlotSetting(BACKPACK_PROGRESSION_KEY, BackpackProgression.Progressive);
            ToolProgression = GetSlotSetting(TOOL_PROGRESSION_KEY, ToolProgression.Progressive);
            ElevatorProgression = GetSlotSetting(ELEVATOR_PROGRESSION_KEY, ElevatorProgression.ProgressiveFromPreviousFloor);
            SkillProgression = GetSlotSetting(SKILLS_PROGRESSION_KEY, SkillsProgression.Progressive);
            BuildingProgression = GetSlotSetting(BUILDING_PROGRESSION_KEY, BuildingProgression.Shuffled);
            FestivalLocations = GetSlotSetting(FESTIVAL_OBJECTIVES_KEY, FestivalLocations.Easy);
            ArcadeMachineLocations = GetSlotSetting(ARCADE_MACHINES_KEY, ArcadeLocations.FullShuffling);
            SpecialOrderLocations = GetSlotSetting(SPECIAL_ORDERS_KEY, SpecialOrderLocations.BoardOnly);
            HelpWantedLocationNumber = GetSlotSetting(HELP_WANTED_LOCATIONS_KEY, 0);
            Fishsanity = GetSlotSetting(FISHSANITY_KEY, Fishsanity.None);
            Museumsanity = GetSlotSetting(MUSEUMSANITY_KEY, Museumsanity.None);
            Friendsanity = GetSlotSetting(FRIENDSANITY_KEY, Friendsanity.None);
            FriendsanityHeartSize = GetSlotSetting(FRIENDSANITY_HEART_SIZE_KEY, 4);
            ExcludeGingerIsland = GetSlotSetting(EXCLUDE_GINGER_ISLAND_KEY, true);
            TrapItemsDifficulty = GetSlotSetting(TRAP_ITEMS_KEY, TrapItemsDifficulty.Medium);
            EnableMultiSleep = GetSlotSetting(MULTI_SLEEP_ENABLED_KEY, true);
            MultiSleepCostPerDay = GetSlotSetting(MULTI_SLEEP_COST_KEY, 0);
            ExperienceMultiplier = GetSlotSetting(EXPERIENCE_MULTIPLIER_KEY, 100) / 100.0;
            FriendshipMultiplier = GetSlotSetting(FRIENDSHIP_MULTIPLIER_KEY, 100) / 100.0;
            DebrisMultiplier = GetSlotSetting(DEBRIS_MULTIPLIER_KEY, DebrisMultiplier.HalfDebris);
            QuickStart = GetSlotSetting(QUICK_START_KEY, false);
            Gifting = GetSlotSetting(GIFTING_KEY, true);
            Banking = true;
            BankTax = 25 / 100.0;
            DeathLink = GetSlotSetting(DEATH_LINK_KEY, false);
            Seed = GetSlotSetting(SEED_KEY, "");
            MultiworldVersion = GetSlotSetting(MULTIWORLD_VERSION_KEY, "");
            var newBundleStringData = GetSlotSetting(MODIFIED_BUNDLES_KEY, "");
            var bundlesData = JsonConvert.DeserializeObject<Dictionary<string, string>>(newBundleStringData);
            Bundles = new BundlesManager(bundlesData);
            var newEntrancesStringData = GetSlotSetting(MODIFIED_ENTRANCES_KEY, "");
            ModifiedEntrances = JsonConvert.DeserializeObject<Dictionary<string, string>>(newEntrancesStringData);
            AppearanceRandomization = AppearanceRandomization.Disabled; // GetSlotSetting(RANDOMIZE_NPC_APPEARANCES_KEY, AppearanceRandomization.Disabled);
            AppearanceRandomizationDaily = false; // GetSlotSetting(RANDOMIZE_NPC_APPEARANCES_DAILY_KEY, false);
            var modsString = GetSlotSetting(MOD_LIST_KEY, "");
            var mods = JsonConvert.DeserializeObject<List<string>>(modsString);
            Mods = new ModsManager(mods);
        }

        private T GetSlotSetting<T>(string key, T defaultValue) where T : struct, Enum, IConvertible
        {
            return _slotDataFields.ContainsKey(key) ? Enum.Parse<T>(_slotDataFields[key].ToString(), true) : GetSlotDefaultValue(key, defaultValue);
        }

        private string GetSlotSetting(string key, string defaultValue)
        {
            return _slotDataFields.ContainsKey(key) ? _slotDataFields[key].ToString() : GetSlotDefaultValue(key, defaultValue);
        }

        private int GetSlotSetting(string key, int defaultValue)
        {
            return _slotDataFields.ContainsKey(key) ? (int)(long)_slotDataFields[key] : GetSlotDefaultValue(key, defaultValue);
        }

        private bool GetSlotSetting(string key, bool defaultValue)
        {
            return _slotDataFields.ContainsKey(key) && _slotDataFields[key] != null ? (bool)_slotDataFields[key] : GetSlotDefaultValue(key, defaultValue);
        }

        private T GetSlotDefaultValue<T>(string key, T defaultValue)
        {
            _console.Log($"SlotData did not contain expected key: \"{key}\"", LogLevel.Warn);
            return defaultValue;
        }
    }

    public enum EntranceRandomization
    {
        Disabled = 0,
        PelicanTown = 1,
        NonProgression = 2,
        Buildings = 3,
        Everything = 4,
        Chaos = 5,
    }

    public enum SeasonRandomization
    {
        Disabled = 0,
        Randomized = 1,
        RandomizedNotWinter = 2,
        Progressive = 3,
    }

    public enum Cropsanity
    {
        Disabled = 0,
        Shuffled = 1,
    }

    public enum BackpackProgression
    {
        Vanilla = 0,
        Progressive = 1,
        ProgressiveEarlyBackpack = 2
    }

    public enum ToolProgression
    {
        Vanilla = 0,
        Progressive = 1,
    }

    public enum ElevatorProgression
    {
        Vanilla = 0,
        Progressive = 1,
        ProgressiveFromPreviousFloor = 2,
    }

    public enum SkillsProgression
    {
        Vanilla = 0,
        Progressive = 1,
    }

    public enum BuildingProgression
    {
        Vanilla = 0,
        Shuffled = 1,
        ShuffledEarlyShippingBin = 2
    }

    public enum FestivalLocations
    {
        Vanilla = 0,
        Easy = 1,
        Hard = 2,
    }

    public enum ArcadeLocations
    {
        Disabled = 0,
        Victories = 1,
        VictoriesEasy = 2,
        FullShuffling = 3,
    }

    public enum SpecialOrderLocations
    {
        Disabled = 0,
        BoardOnly = 1,
        BoardAndQi = 2,
    }

    public enum Fishsanity
    {
        None = 0,
        Legendaries = 1,
        Special = 2,
        RandomSelection = 3,
        All = 4,
        ExcludeLegendaries = 5,
        ExcludeHardFish = 6,
        OnlyEasyFish = 7,
    }

    public enum Museumsanity
    {
        None = 0,
        Milestones = 1,
        RandomSelection = 2,
        All = 3,
    }

    public enum Friendsanity
    {
        None = 0,
        // MarryOnePerson = 1,
        Bachelors = 2,
        StartingNpcs = 3,
        All = 4,
        AllWithMarriage = 5,
    }

    public enum TrapItemsDifficulty
    {
        NoTraps = 0,
        Easy = 1,
        Medium = 2,
        Hard = 3,
        Hell = 4,
        Nightmare = 5,
    }

    public enum Goal
    {
        CommunityCenter = 0,
        GrandpaEvaluation = 1,
        BottomOfMines = 2,
        CrypticNote = 3,
        MasterAngler = 4,
        CompleteCollection = 5,
        FullHouse = 6,
        GreatestWalnutHunter = 7,
        Perfection = 25,
    }

    public enum DebrisMultiplier
    {
        Vanilla = 0,
        HalfDebris = 1,
        QuarterDebris = 2,
        NoDebris = 3,
        StartClear = 4,
    }

    public enum AppearanceRandomization
    {
        Disabled = 0,
        Villagers = 1,
        All = 2,
        Chaos = 3,
    }
}
