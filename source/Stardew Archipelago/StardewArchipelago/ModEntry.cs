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
using Microsoft.Xna.Framework;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Archipelago.Gifting;
using StardewArchipelago.Bundles;
using StardewArchipelago.GameModifications;
using StardewArchipelago.GameModifications.CodeInjections;
using StardewArchipelago.GameModifications.EntranceRandomizer;
using StardewArchipelago.GameModifications.Seasons;
using StardewArchipelago.Goals;
using StardewArchipelago.Items;
using StardewArchipelago.Items.Mail;
using StardewArchipelago.Items.Traps;
using StardewArchipelago.Locations;
using StardewArchipelago.Locations.CodeInjections.Modded.SVE;
using StardewArchipelago.Locations.CodeInjections.Vanilla;
using StardewArchipelago.Locations.CodeInjections.Vanilla.Relationship;
using StardewArchipelago.Locations.Patcher;
using StardewArchipelago.Serialization;
using StardewArchipelago.Stardew;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewArchipelago.GameModifications.Modded;
using StardewArchipelago.Locations.CodeInjections.Vanilla.MonsterSlayer;
using StardewArchipelago.Locations.CodeInjections.Modded;
using StardewArchipelago.Constants;
using StardewArchipelago.Stardew.NameMapping;

namespace StardewArchipelago
{
    public class ModEntry : Mod
    {
        public static ModEntry Instance;

        private const string CONNECT_SYNTAX = "Syntax: connect_override ip:port slot password";
        private const string AP_DATA_KEY = "ArchipelagoData";
        private const string AP_EXPERIENCE_KEY = "ArchipelagoSkillsExperience";
        private const string AP_FRIENDSHIP_KEY = "ArchipelagoFriendshipPoints";

        private IModHelper _helper;
        private Harmony _harmony;
        private ArchipelagoClient _archipelago;
        private AdvancedOptionsManager _advancedOptionsManager;
        private Mailman _mail;
        private ChatForwarder _chatForwarder;
        private IGiftHandler _giftHandler;
        private ItemManager _itemManager;
        private RandomizedLogicPatcher _logicPatcher;
        private MailPatcher _mailPatcher;
        private LocationChecker _locationChecker;
        private LocationPatcher _locationsPatcher;
        private ItemPatcher _itemPatcher;
        private GoalManager _goalManager;
        private StardewItemManager _stardewItemManager;
        private MultiSleep _multiSleep;
        private JojaDisabler _jojaDisabler;
        private SeasonsRandomizer _seasonsRandomizer;
        private AppearanceRandomizer _appearanceRandomizer;
        private QuestCleaner _questCleaner;
        private EntranceManager _entranceManager;
        private NightShippingBehaviors _shippingBehaviors;

        private ModRandomizedLogicPatcher _modLogicPatcher;
        private CallableModData _callableModData;
        private ModifiedVillagerEventChecker _villagerEvents;

        public ArchipelagoStateDto State { get; set; }
        private ArchipelagoConnectionInfo _apConnectionOverride;

        public ModEntry() : base()
        {
            Instance = this;
        }

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            _apConnectionOverride = null;

            _helper = helper;
            _harmony = new Harmony(this.ModManifest.UniqueID);

            _archipelago = new ArchipelagoClient(Monitor, _helper, _harmony, OnItemReceived, ModManifest);

            _helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            _helper.Events.GameLoop.SaveCreating += this.OnSaveCreating;
            _helper.Events.GameLoop.SaveCreated += this.OnSaveCreated;
            _helper.Events.GameLoop.Saving += this.OnSaving;
            _helper.Events.GameLoop.Saved += this.OnSaved;
            _helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            _helper.Events.GameLoop.TimeChanged += this.OnTimeChanged;
            _helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            _helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            _helper.Events.GameLoop.DayEnding += this.OnDayEnding;
            _helper.Events.GameLoop.ReturnedToTitle += this.OnReturnedToTitle;


            _helper.ConsoleCommands.Add("connect_override", $"Overrides your next connection to Archipelago. {CONNECT_SYNTAX}", this.OnCommandConnectToArchipelago);
            _helper.ConsoleCommands.Add("export_all_gifts", "Export all currently loaded giftable items and their traits", this.ExportGifts);
            _helper.ConsoleCommands.Add("deathlink", "Override the deathlink setting", this.OverrideDeathlink);
            _helper.ConsoleCommands.Add("trap_difficulty", "Override the trap difficulty setting", this.OverrideTrapDifficulty);

#if DEBUG
            _helper.ConsoleCommands.Add("connect", $"Connect to Archipelago. {CONNECT_SYNTAX}", this.OnCommandConnectToArchipelago);
            _helper.ConsoleCommands.Add("disconnect", $"Disconnects from Archipelago. {CONNECT_SYNTAX}", this.OnCommandDisconnectFromArchipelago);
            _helper.ConsoleCommands.Add("set_next_season", "Sets the next season to a chosen value", this.SetNextSeason);
            //_helper.ConsoleCommands.Add("test_sendalllocations", "Tests if every AP item in the stardew_valley_location_table json file are supported by the mod", _tester.TestSendAllLocations);
            // _helper.ConsoleCommands.Add("load_entrances", "Loads the entrances file", (_, _) => _entranceRandomizer.LoadTransports());
            // _helper.ConsoleCommands.Add("save_entrances", "Saves the entrances file", (_, _) => EntranceInjections.SaveNewEntrancesToFile());
            _helper.ConsoleCommands.Add("export_shippables", "Export all currently loaded shippable items", this.ExportShippables);
            _helper.ConsoleCommands.Add("release_slot", "Release the current slot completely", this.ReleaseSlot);
            _helper.ConsoleCommands.Add("debug_method", "Runs whatever is currently in the debug method", this.DebugMethod);
#endif
        }

        private void ResetArchipelago()
        {
            _archipelago.DisconnectPermanently();
            if (State != null)
            {
                State.APConnectionInfo = null;
            }
            State = new ArchipelagoStateDto();

            _harmony.UnpatchAll(ModManifest.UniqueID);
            SeasonsRandomizer.ResetMailKeys();
            _multiSleep = new MultiSleep(Monitor, _helper, _harmony);
            _advancedOptionsManager = new AdvancedOptionsManager(this, Monitor, _helper, _harmony, _archipelago);
            _advancedOptionsManager.InjectArchipelagoAdvancedOptions();
            _giftHandler = new CrossGiftHandler();
            _villagerEvents = new ModifiedVillagerEventChecker();
            SkillInjections.ResetSkillExperience();
            FriendshipInjections.ResetArchipelagoFriendshipPoints();

            IslandWestMapInjections.PatchMapInjections(Monitor, _helper, _harmony);
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            ResetArchipelago();
        }

        private void OnSaveCreating(object sender, SaveCreatingEventArgs e)
        {
            State.ItemsReceived = new List<ReceivedItem>();
            State.LocationsChecked = new List<string>();
            State.LocationsScouted = new Dictionary<string, ScoutedLocation>();
            State.LettersGenerated = new Dictionary<string, string>();
            SkillInjections.ResetSkillExperience();
            FriendshipInjections.ResetArchipelagoFriendshipPoints();

            if (!_archipelago.IsConnected)
            {
                Monitor.Log("You are not allowed to create a new game without connecting to Archipelago", LogLevel.Error);
                Game1.ExitToTitle();
                return;
            }

            _seasonsRandomizer = new SeasonsRandomizer(Monitor, _helper, _archipelago, State);
            State.AppearanceRandomizerOverride = null;
            State.TrapDifficultyOverride = null;
            State.SeasonsOrder = new List<string>();
            State.SeasonsOrder.Add(_seasonsRandomizer.GetFirstSeason());
            SeasonsRandomizer.SetSeason(State.SeasonsOrder.Last());

            DebugAssertStateValues(State);
            _helper.Data.WriteSaveData(AP_DATA_KEY, State);
            _helper.Data.WriteSaveData(AP_EXPERIENCE_KEY, SkillInjections.GetArchipelagoExperience());
            _helper.Data.WriteSaveData(AP_FRIENDSHIP_KEY, FriendshipInjections.GetArchipelagoFriendshipPoints());
        }

        private void OnSaveCreated(object sender, SaveCreatedEventArgs e)
        {
        }

        private void OnSaving(object sender, SavingEventArgs e)
        {
            SeasonsRandomizer.PrepareDateForSaveGame();
            State.ItemsReceived = _itemManager.GetAllItemsAlreadyProcessed();
            State.LocationsChecked = _locationChecker.GetAllLocationsAlreadyChecked();
            State.LocationsScouted = _archipelago.ScoutedLocations;
            // _state.SeasonOrder should be fine?

            DebugAssertStateValues(State);
            _helper.Data.WriteSaveData(AP_DATA_KEY, State);
            _helper.Data.WriteSaveData(AP_EXPERIENCE_KEY, SkillInjections.GetArchipelagoExperience());
            _helper.Data.WriteSaveData(AP_FRIENDSHIP_KEY, FriendshipInjections.GetArchipelagoFriendshipPoints());
        }

        private void DebugAssertStateValues(ArchipelagoStateDto state)
        {
            if (state.APConnectionInfo == null)
            {
                Monitor.Log(
                    $"About to write Archipelago State data, but the connectionInfo is null! This should never happen. Please contact KaitoKid and describe what you did last so it can be investigated.",
                    LogLevel.Error);
            }

            if (state.LettersGenerated == null)
            {
                Monitor.Log(
                    $"About to write Archipelago State data, but the there are no custom letters! This should never happen. Please contact KaitoKid and describe what you did last so it can be investigated.",
                    LogLevel.Error);
            }
        }

        private void OnSaved(object sender, SavedEventArgs e)
        {
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            try
            {
                ReadPersistentArchipelagoData();
                
                _stardewItemManager = new StardewItemManager();
                _mail = new Mailman(State);
                _locationChecker = new LocationChecker(Monitor, _archipelago, State.LocationsChecked);
                _itemPatcher = new ItemPatcher(Monitor, _helper, _harmony, _archipelago);
                _goalManager = new GoalManager(Monitor, _helper, _harmony, _archipelago, _locationChecker);
                _entranceManager = new EntranceManager(Monitor, _archipelago, State);
                var shopStockGenerator = new ShopStockGenerator(Monitor, _helper, _archipelago, _locationChecker);
                var junimoShopGenerator = new JunimoShopGenerator(_archipelago, shopStockGenerator, _stardewItemManager);
                var nameSimplifier = new NameSimplifier();
                var friends = new Friends();
                _logicPatcher = new RandomizedLogicPatcher(Monitor, _helper, _harmony, _archipelago, _locationChecker, _stardewItemManager, _entranceManager, shopStockGenerator, nameSimplifier, friends, State);
                _modLogicPatcher = new ModRandomizedLogicPatcher(Monitor, _helper, _harmony, _archipelago, shopStockGenerator, _stardewItemManager, junimoShopGenerator);
                _jojaDisabler = new JojaDisabler(Monitor, _helper, _harmony);
                _seasonsRandomizer = new SeasonsRandomizer(Monitor, _helper, _archipelago, State);
                _appearanceRandomizer = new AppearanceRandomizer(Monitor, _archipelago);
                var tileChooser = new TileChooser();
                _chatForwarder = new ChatForwarder(Monitor, _helper, _harmony, _archipelago, _giftHandler, _goalManager, tileChooser);
                _questCleaner = new QuestCleaner();
                
                if (!_archipelago.IsConnected)
                {
                    if (_apConnectionOverride != null)
                    {
                        State.APConnectionInfo = _apConnectionOverride;
                        _apConnectionOverride = null;
                    }

                    var errorMessage = "";
                    if (State.APConnectionInfo == null)
                    {
                        errorMessage =
                            $"The game being loaded has no connection information.{Environment.NewLine}Please use the connect_override command to input connection fields before loading it";
                    }
                    else
                    {
                        _archipelago.Connect(State.APConnectionInfo, out errorMessage);
                    }

                    if (!_archipelago.IsConnected)
                    {
                        State.APConnectionInfo = null;
                        Game1.activeClickableMenu = new InformationDialog(errorMessage, onCloseBehavior: (_) => OnCloseBehavior());
                        return;
                    }
                }

                var babyBirther = new BabyBirther();
                _giftHandler.Initialize(Monitor, _archipelago, _stardewItemManager, _mail);
                _itemManager = new ItemManager(Monitor, _helper, _harmony, _archipelago, _stardewItemManager, _mail, tileChooser, babyBirther, _giftHandler.Sender, State.ItemsReceived);
                var weaponsManager = new WeaponsManager(_stardewItemManager, _archipelago.SlotData.Mods);
                _mailPatcher = new MailPatcher(Monitor, _harmony, _archipelago, _locationChecker, State,
                    new LetterActions(_helper, _mail, _archipelago, weaponsManager, _itemManager.TrapManager, babyBirther, _stardewItemManager));
                var bundlesManager = new BundlesManager(_helper, _stardewItemManager, _archipelago.SlotData.BundlesData);
                bundlesManager.ReplaceAllBundles();
                _locationsPatcher = new LocationPatcher(Monitor, _helper, _harmony, _archipelago, State, _locationChecker, _stardewItemManager, weaponsManager, shopStockGenerator, junimoShopGenerator, friends);
                _shippingBehaviors = new NightShippingBehaviors(Monitor, _archipelago, _locationChecker, nameSimplifier);
                _chatForwarder.ListenToChatMessages();
                _logicPatcher.PatchAllGameLogic();
                _modLogicPatcher.PatchAllModGameLogic();
                _mailPatcher.PatchMailBoxForApItems();
                _entranceManager.SetEntranceRandomizerSettings(_archipelago.SlotData);
                _locationsPatcher.ReplaceAllLocationsRewardsWithChecks();
                _itemPatcher.PatchApItems();
                _goalManager.InjectGoalMethods();
                _jojaDisabler.DisableJojaMembership();
                _multiSleep.InjectMultiSleepOption(_archipelago.SlotData);
                TravelingMerchantInjections.UpdateTravelingMerchantForToday(Game1.getLocationFromName("Forest") as Forest, Game1.dayOfMonth);
                SeasonsRandomizer.ChangeMailKeysBasedOnSeasonsToDaysElapsed();
                _callableModData = new CallableModData(Monitor, _archipelago);
                Game1.chatBox?.addMessage($"Connected to Archipelago as {_archipelago.SlotData.SlotName}. Type !!help for client commands", Color.Green);

            }
            catch (Exception)
            {
                Game1.chatBox?.addMessage($"A Fatal error has occurred while initializing Archipelago. Check SMAPI for details to report the problem", Color.Red);
                throw;
            }
        }

        private void OnCloseBehavior()
        {
            Monitor.Log("You are not allowed to load a save without connecting to Archipelago", LogLevel.Error);
            // TitleMenu.subMenu = previousMenu;
            Game1.ExitToTitle();
        }

        private void ReadPersistentArchipelagoData()
        {
            var state = _helper.Data.ReadSaveData<ArchipelagoStateDto>(AP_DATA_KEY);
            if (state != null)
            {
                State = state;
                _archipelago.ScoutedLocations = State.LocationsScouted;
            }

            var apExperience = _helper.Data.ReadSaveData<Dictionary<int, int>>(AP_EXPERIENCE_KEY);
            SkillInjections.SetArchipelagoExperience(apExperience);

            var apFriendship = _helper.Data.ReadSaveData<Dictionary<string, int>>(AP_FRIENDSHIP_KEY);
            FriendshipInjections.SetArchipelagoFriendshipPoints(apFriendship);
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            if (!_archipelago.MakeSureConnected(5))
            {
                return;
            }

            SeasonsRandomizer.ChangeMailKeysBasedOnSeasonsToDaysElapsed();
            SeasonsRandomizer.SendMailHardcodedForToday();

            if (MultiSleep.DaysToSkip > 0)
            {
                MultiSleep.DaysToSkip--;
                Game1.NewDay(0);
                return;
            }

            _questCleaner.CleanQuests(Game1.player);

            FarmInjections.DeleteStartingDebris();
            FarmInjections.PlaceEarlyShippingBin();
            _mail.SendToday();
            FarmInjections.ForcePetIfNeeded(_mail);
            _locationChecker.VerifyNewLocationChecksWithArchipelago();
            _locationChecker.SendAllLocationChecks();
            _itemManager.ReceiveAllNewItems(false);
            _goalManager.CheckGoalCompletion();
            _mail.SendTomorrow();
            PlayerBuffInjections.CheckForApBuffs();
            if (State.AppearanceRandomizerOverride != null)
            {
                _archipelago.SlotData.AppearanceRandomization = State.AppearanceRandomizerOverride.Value;
            }
            if (State.TrapDifficultyOverride != null)
            {
                _archipelago.SlotData.TrapItemsDifficulty = State.TrapDifficultyOverride.Value;
            }
            _appearanceRandomizer.ShuffleCharacterAppearances();
            _entranceManager.ResetCheckedEntrancesToday(_archipelago.SlotData);
            TheaterInjections.UpdateScheduleForEveryone();
            DoBugsCleanup();
        }

        private void DoBugsCleanup()
        {
            if (_archipelago.SlotData.Mods.HasMod(ModNames.LUCK) && Game1.player.LuckLevel >= 8 && _locationChecker.IsLocationMissing("Lucky Lunch Recipe"))
            {
                _locationChecker.AddCheckedLocation("Lucky Lunch Recipe");
                if (_archipelago.SlotData.Chefsanity.HasFlag(Chefsanity.Skills) && !_archipelago.HasReceivedItem("Lucky Lunch Recipe") && Game1.player.cookingRecipes.ContainsKey("Lucky Lunch"))
                {
                    Game1.player.cookingRecipes.Remove("Lucky Lunch");
                }
            }
            if (_archipelago.SlotData.Mods.HasMod(ModNames.MAGIC))
            {
                if (_archipelago.HasReceivedItem("Magic Elixir Recipe") & !Game1.player.cookingRecipes.ContainsKey("Magic Elixir"))
                {
                    Game1.player.cookingRecipes.Add("Magic Elixir", 0); // Its a cooking recipe.
                }
                var magicElixir = _stardewItemManager.GetItemByName("Magic Elixir").Id;
                if (Game1.player.recipesCooked.ContainsKey(magicElixir))
                {
                    _locationChecker.AddCheckedLocation("Craft Magic Elixir"); // If you cooked it just relog.
                }
            }
            // Fix to remove dupes in Railroad Boulder
            if (_archipelago.SlotData.Mods.HasMod(ModNames.SVE))
            {
                var railroadBoulderOrder = SpecialOrder.GetSpecialOrder("Clint2", null);
                var railroadDupeCount = Game1.player.team.specialOrders.Count(x => x.questKey.Value.Equals("Clint2Again"));
                if (railroadDupeCount > 1)
                {
                    railroadBoulderOrder.questKey.Value = "Clint2Again";
                    while (railroadDupeCount > 1)
                    {
                        Game1.player.team.specialOrders.Remove(railroadBoulderOrder);
                        railroadDupeCount -= 1;
                    }
                }
                // Async Fix for the change from eventsSeen to mailReceived checks.
                var deprecatedEvents = new Dictionary<int, string>() { { 658059254, "apAuroraVineyard" }, { 658078924, "apMorganSchooling" } };
                foreach (var (id, mail) in deprecatedEvents)
                {
                    if (Game1.player.eventsSeen.Contains(id))
                    {
                        Game1.player.eventsSeen.Remove(id);
                        Game1.player.mailReceived.Add(mail);
                    }
                }
                // Async fix for the change in call to fix Morris/Claire/Martin
                if (!Game1.player.mailReceived.Contains("apAbandonedJojaMart") && _archipelago.HasReceivedItem("Progressive Movie Theater"))
                {
                    Game1.player.mailReceived.Add("apAbandonedJojaMart");
                }
                if ((Game1.player.eventsSeen.Contains(181091237) || Game1.player.eventsSeen.Contains(1810912313)) && !_archipelago.HasReceivedItem("Ginger Tincture Recipe"))
                {
                    Game1.player.craftingRecipes.Remove("Ginger Tincture");
                    _locationChecker.AddCheckedLocation("Ginger Tincture Recipe");
                }
                if (_archipelago.HasReceivedItem("Krobus' Protection") && !Game1.player.mailReceived.Contains("GaveVoidSouls"))
                {
                    Game1.player.mailReceived.Add("GaveVoidSouls");
                }
            }
        }

        private void OnDayEnding(object sender, DayEndingEventArgs e)
        {
            _giftHandler.ReceiveAllGiftsTomorrow();
            _villagerEvents.CheckJunaHearts(_archipelago);
            AdventurerGuildInjections.RemoveExtraItemsFromItemsLostLastDeath();
            _shippingBehaviors?.CheckShipsanityLocationsBeforeSleep();
        }

        private void OnTimeChanged(object sender, TimeChangedEventArgs e)
        {
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            _archipelago.APUpdate();
        }

        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            ResetArchipelago();
        }

        private void OnCommandConnectToArchipelago(string arg1, string[] arg2)
        {
            if (arg2.Length < 2)
            {
                Monitor.Log($"You must provide an IP with a port, and a slot name, to connect to archipelago. {CONNECT_SYNTAX}", LogLevel.Info);
                return;
            }

            var ipAndPort = arg2[0].Split(":");
            if (ipAndPort.Length < 2)
            {
                Monitor.Log($"You must provide an IP with a port, and a slot name, to connect to archipelago. {CONNECT_SYNTAX}", LogLevel.Info);
                return;
            }

            var ip = ipAndPort[0];
            var port = int.Parse(ipAndPort[1]);
            var slot = arg2[1];
            var password = arg2.Length >= 3 ? arg2[2] : "";
            _apConnectionOverride = new ArchipelagoConnectionInfo(ip, port, slot, null, password);
        }

        private void OnCommandDisconnectFromArchipelago(string arg1, string[] arg2)
        {
            ArchipelagoDisconnect();
        }

        private void OnItemReceived()
        {
            _itemManager?.ReceiveAllNewItems(true);
        }

        public bool ArchipelagoConnect(string ip, int port, string slot, string password, out string errorMessage)
        {
            var apConnection = new ArchipelagoConnectionInfo(ip, port, slot, null, password);
            _archipelago.Connect(apConnection, out errorMessage);
            if (!_archipelago.IsConnected)
            {
                return false;
            }

            State.APConnectionInfo = apConnection;
            return true;
        }

        public void ArchipelagoDisconnect()
        {
            Game1.ExitToTitle();
            _archipelago.DisconnectPermanently();
            State.APConnectionInfo = null;
        }

        private void SetNextSeason(string arg1, string[] arg2)
        {
            if (arg2.Length < 1)
            {
                Monitor.Log($"You must specify a season", LogLevel.Info);
                return;
            }

            var season = arg2[0];
            var currentSeasonNumber = (int) Game1.stats.DaysPlayed / 28;
            if (State.SeasonsOrder.Count <= currentSeasonNumber)
            {
                State.SeasonsOrder.Add(season);
            }
            else
            {
                State.SeasonsOrder[currentSeasonNumber] = season;
            }
        }

        private void ExportShippables(string arg1, string[] arg2)
        {
            _stardewItemManager.ExportAllItemsMatching(x => x.canBeShipped(), "shippables.json");
        }

#if DEBUG

        private void ReleaseSlot(string arg1, string[] arg2)
        {
            if (!_archipelago.IsConnected || !Game1.hasLoadedGame || arg2.Length < 1)
            {
                return;
            }

            var slotName = arg2[0];

            if (slotName != _archipelago.GetPlayerName() || slotName != Game1.player.Name)
            {
                return;
            }

            foreach (var missingLocation in _locationChecker.GetAllMissingLocationNames())
            {
                _locationChecker.AddCheckedLocation(missingLocation);
            }
        }

#endif

        private void ExportGifts(string arg1, string[] arg2)
        {
            _giftHandler.ExportAllGifts("gifts.json");
        }

        private void OverrideDeathlink(string arg1, string[] arg2)
        {
            _archipelago?.ToggleDeathlink();
        }

        private void OverrideTrapDifficulty(string arg1, string[] arg2)
        {
            if (_archipelago == null || State == null || !_archipelago.MakeSureConnected(0))
            {
                Monitor.Log($"This command can only be used from in-game, when connected to Archipelago", LogLevel.Info);
                return;
            }

            if (arg2.Length < 1)
            {
                Monitor.Log($"Choose one of the following difficulties: [NoTraps, Easy, Medium, Hard, Hell, Nightmare].", LogLevel.Info);
                return;
            }

            var difficulty = arg2[0];
            if (!Enum.TryParse<TrapItemsDifficulty>(difficulty, true, out var difficultyOverride))
            {
                Monitor.Log($"Choose one of the following difficulties: [NoTraps, Easy, Medium, Hard, Hell, Nightmare].", LogLevel.Info);
                return;
            }

            State.TrapDifficultyOverride = difficultyOverride;
            Monitor.Log($"Trap Difficulty set to [{difficultyOverride}]. Change will be saved next time you sleep", LogLevel.Info);
        }

        private void DebugMethod(string arg1, string[] arg2)
        {
            _itemManager.ItemParser.TrapManager.AddJinxedDebuff();
        }
    }
}
