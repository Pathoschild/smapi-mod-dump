/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FishingTrawler
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FishingTrawler.API;
using FishingTrawler.API.Interfaces;
using FishingTrawler.GameLocations;
using FishingTrawler.Messages;
using FishingTrawler.Objects;
using FishingTrawler.Objects.Rewards;
using FishingTrawler.Objects.Tools;
using FishingTrawler.Patches.Locations;
using FishingTrawler.UI;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;
using xTile.Dimensions;

namespace FishingTrawler
{
    public class ModEntry : Mod
    {
        internal static IMonitor monitor;
        internal static IModHelper modHelper;
        internal static IManifest manifest;
        internal static ITranslationHelper i18n;
        internal static Multiplayer multiplayer;
        internal static ModConfig config;
        internal static string trawlerThemeSong;
        internal static bool themeSongUpdated;
        internal static Farmer mainDeckhand;
        internal static int numberOfDeckhands;
        internal static string todayDayOfWeek;

        // Trawler beach map related
        internal static Murphy murphyNPC;
        internal static Trawler trawlerObject;
        internal static Chest rewardChest;

        // Trawler map / texture related
        private readonly PerScreen<int> fishingTripTimer = new PerScreen<int>();
        private readonly PerScreen<TrawlerHull> _trawlerHull = new PerScreen<TrawlerHull>();
        private readonly PerScreen<TrawlerSurface> _trawlerSurface = new PerScreen<TrawlerSurface>();
        private readonly PerScreen<TrawlerCabin> _trawlerCabin = new PerScreen<TrawlerCabin>();
        private readonly PerScreen<TrawlerRewards> _trawlerRewards = new PerScreen<TrawlerRewards>();
        private PerScreen<bool> _isTripEnding = new PerScreen<bool>();
        private string _trawlerItemsPath = Path.Combine("assets", "TrawlerItems");

        // Location names
        private const string TRAWLER_SURFACE_LOCATION_NAME = "Custom_FishingTrawler";
        private const string TRAWLER_HULL_LOCATION_NAME = "Custom_TrawlerHull";
        private const string TRAWLER_CABIN_LOCATION_NAME = "Custom_TrawlerCabin";

        // Day to appear settings
        internal const int BOAT_DEPART_EVENT_ID = 840603900;

        // Mod data related
        internal const string REWARD_CHEST_DATA_KEY = "PeacefulEnd.FishingTrawler_RewardChest";
        internal const string MURPHY_WAS_GREETED_TODAY_KEY = "PeacefulEnd.FishingTrawler_MurphyGreeted";
        internal const string MURPHY_SAILED_TODAY_KEY = "PeacefulEnd.FishingTrawler_MurphySailedToday";
        internal const string MURPHY_WAS_TRIP_SUCCESSFUL_KEY = "PeacefulEnd.FishingTrawler_MurphyTripSuccessful";
        internal const string MURPHY_FINISHED_TALKING_KEY = "PeacefulEnd.FishingTrawler_MurphyFinishedTalking";
        internal const string MURPHY_HAS_SEEN_FLAG_KEY = "PeacefulEnd.FishingTrawler_MurphyHasSeenFlag";
        internal const string MURPHY_ON_TRIP = "PeacefulEnd.FishingTrawler_MurphyOnTrip";
        internal const string MURPHY_DAY_TO_APPEAR = "PeacefulEnd.FishingTrawler_MurphyDayToAppear";
        internal const string MURPHY_DAY_TO_APPEAR_ISLAND = "PeacefulEnd.FishingTrawler_MurphyDayToAppearIsland";
        internal const string MURPHY_TRIPS_COMPLETED = "PeacefulEnd.FishingTrawler_MurphyTripsCompleted";

        internal const string BAILING_BUCKET_KEY = "PeacefulEnd.FishingTrawler_BailingBucket";
        internal const string ANCIENT_FLAG_KEY = "PeacefulEnd.FishingTrawler_AncientFlag";

        internal const string HOISTED_FLAG_KEY = "PeacefulEnd.FishingTrawler_HoistedFlag";

        // Notificiation messages
        private KeyValuePair<string, int> MESSAGE_EVERYTHING_FAILING;
        private KeyValuePair<string, int> MESSAGE_LOSING_FISH;
        private KeyValuePair<string, int> MESSAGE_MAX_LEAKS;
        private KeyValuePair<string, int> MESSAGE_MULTI_PROBLEMS;
        private KeyValuePair<string, int> MESSAGE_ENGINE_PROBLEM;
        private KeyValuePair<string, int> MESSAGE_NET_PROBLEM;
        private KeyValuePair<string, int> MESSAGE_LEAK_PROBLEM;

        // Notification related
        private uint _eventSecondInterval;
        private bool _isNotificationFading;
        private float _notificationAlpha;
        private string _activeNotification;


        public override void Entry(IModHelper helper)
        {
            // Set up the monitor and helper
            monitor = Monitor;
            modHelper = helper;
            manifest = ModManifest;
            multiplayer = helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
            i18n = helper.Translation;

            // Load in our assets
            ModResources.SetUpAssets(helper);

            // Initialize the timer for fishing trip
            fishingTripTimer.Value = 0;

            // Set up our notification system on the trawler
            _eventSecondInterval = 600;
            _isTripEnding.Value = false;
            _activeNotification = String.Empty;
            _notificationAlpha = 1f;
            _isNotificationFading = false;

            // Load our Harmony patches
            try
            {
                var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

                // Apply our patches
                new BeachPatch(monitor).Apply(harmony);
                new IslandSouthEastPatch(monitor).Apply(harmony);
                new GameLocationPatch(monitor).Apply(harmony);
            }
            catch (Exception e)
            {
                Monitor.Log($"Issue with Harmony patching: {e}", LogLevel.Error);
                return;
            }

            // Add in our debug commands
            helper.ConsoleCommands.Add("ft_getflags", "Gives all the variations of the ancient flag.\n\nUsage: ft_getflags", this.DebugGetAllFlags);
            helper.ConsoleCommands.Add("ft_getspecials", "Gives all the special rewards.\n\nUsage: ft_getspecials", this.DebugGetSpecialRewards);

            // Hook into GameLoops related events
            helper.Events.GameLoop.UpdateTicking += this.OnUpdateTicking;
            helper.Events.GameLoop.OneSecondUpdateTicking += this.OnOneSecondUpdateTicking;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.GameLoop.DayEnding += this.OnDayEnding;

            // Hook into Display related events
            helper.Events.Display.RenderingHud += this.OnRenderingHud;
            helper.Events.Display.RenderedHud += this.OnRenderedHud;

            // Hook into Player related events
            helper.Events.Player.Warped += this.OnWarped;

            // Hook into MouseClicked
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;

            // Hook into Multiplayer related
            helper.Events.Multiplayer.PeerConnected += this.OnPeerConnected;
            helper.Events.Multiplayer.ModMessageReceived += this.OnModMessageReceived;
        }

        private void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID != this.ModManifest.UniqueID)
            {
                return;
            }

            switch (e.Type)
            {
                case nameof(DepartureMessage):
                    DepartureMessage departureMessage = e.ReadAs<DepartureMessage>();
                    mainDeckhand = Game1.getAllFarmers().First(f => f.UniqueMultiplayerID == departureMessage.MainDeckhand);
                    trawlerObject.TriggerDepartureEvent();
                    break;
                case nameof(TrawlerEventMessage):
                    TrawlerEventMessage eventMessage = e.ReadAs<TrawlerEventMessage>();
                    UpdateLocalTrawlerMap(eventMessage.EventType, eventMessage.Tile, eventMessage.IsRepairing);
                    break;
                case nameof(TrawlerSyncMessage):
                    TrawlerSyncMessage syncMessage = e.ReadAs<TrawlerSyncMessage>();
                    SyncLocalTrawlerMap(syncMessage.SyncType, syncMessage.Quantity);
                    break;
                case nameof(TrawlerNotificationMessage):
                    TrawlerNotificationMessage notificationMessage = e.ReadAs<TrawlerNotificationMessage>();
                    _activeNotification = notificationMessage.Notification;
                    break;
            }
        }

        private void OnPeerConnected(object sender, PeerConnectedEventArgs e)
        {
            if (!Context.IsMainPlayer)
            {
                // Set Farmer moddata used for this mod
                EstablishPlayerData();
            }
        }

        private void OnRenderingHud(object sender, RenderingHudEventArgs e)
        {
            if (!IsPlayerOnTrawler())
            {
                return;
            }

            if (!String.IsNullOrEmpty(_activeNotification))
            {
                TrawlerUI.DrawNotification(e.SpriteBatch, Game1.player.currentLocation, _activeNotification, _notificationAlpha);
            }
        }

        private void OnRenderedHud(object sender, RenderedHudEventArgs e)
        {
            if (!IsPlayerOnTrawler())
            {
                return;
            }

            TrawlerUI.DrawUI(e.SpriteBatch, fishingTripTimer.Value, _trawlerSurface.Value.fishCaughtQuantity, _trawlerHull.Value.GetWaterLevel(), _trawlerHull.Value.HasLeak(), _trawlerSurface.Value.GetRippedNetsCount(), _trawlerCabin.Value.GetLeakingPipesCount());
        }

        private void OnWarped(object sender, WarpedEventArgs e)
        {
            // Check if player just left the trawler
            if (!IsPlayerOnTrawler() && IsValidTrawlerLocation(e.OldLocation))
            {
                if (murphyNPC is null && (e.NewLocation is Beach || e.NewLocation is IslandSouthEast))
                {
                    // Spawn Murphy, if he isn't already there
                    e.NewLocation.modData[ModEntry.MURPHY_ON_TRIP] = "false";
                    SpawnMurphy(e.NewLocation);
                }

                // Set the theme to null
                SetTrawlerTheme(null);

                numberOfDeckhands = 0;
                mainDeckhand = null;

                // Take away any bailing buckets
                foreach (BailingBucket bucket in Game1.player.Items.Where(i => i != null && i is BailingBucket))
                {
                    Game1.player.removeItemFromInventory(bucket);
                }

                // Reset the trawler
                _trawlerHull.Value.Reset();
                _trawlerSurface.Value.Reset();
                _trawlerCabin.Value.Reset();

                // Finish trip ending logic
                _isTripEnding.Value = false;

                return;
            }

            // Check if player just entered the trawler
            if (IsPlayerOnTrawler() && !IsValidTrawlerLocation(e.OldLocation))
            {
                // Set the default track
                Game1.changeMusicTrack("fieldofficeTentMusic");

                // Give them a bailing bucket
                if (!Game1.player.items.Any(i => i is BailingBucket))
                {
                    Game1.player.addItemToInventory(new BailingBucket());
                    Game1.addHUDMessage(new HUDMessage(i18n.Get("game_message.given_bailing_bucket"), null));
                }

                // Clear any previous reward data, set the head deckhand (which determines fishing level for reward calc)
                _trawlerRewards.Value.Reset(Game1.player);

                // Set flag data
                _trawlerSurface.Value.SetFlagTexture(GetHoistedFlag());

                // Start the timer (2.5 minute default)
                fishingTripTimer.Value = 150000; //150000

                // Apply flag benefits
                switch (GetHoistedFlag())
                {
                    case FlagType.Parley:
                        // Disable all leaks, but reduce fish catch chance by 25% during reward calculations (e.g. more chance of junk / lower quality fish)
                        _trawlerHull.Value.areLeaksEnabled = false;
                        _trawlerRewards.Value.fishCatchChanceOffset = 0.25f;
                        break;
                    case FlagType.JollyRoger:
                        // Quadruples net output 
                        _trawlerSurface.Value.fishCaughtMultiplier = 4;
                        _trawlerHull.Value.hasWeakHull = true;
                        break;
                    case FlagType.GamblersCrest:
                        // 50% of doubling chest, 25% of getting nothing
                        _trawlerRewards.Value.isGambling = true;
                        break;
                    case FlagType.MermaidsBlessing:
                        // 10% of fish getting consumed, but gives random fishing chest reward
                        _trawlerRewards.Value.hasMermaidsBlessing = true;
                        break;
                    case FlagType.PatronSaint:
                        // 25% of fish getting consumed, but gives full XP
                        _trawlerRewards.Value.hasPatronSaint = true;
                        break;
                    case FlagType.SharksFin:
                        // Adds one extra minute to timer, allowing for more fish haul
                        fishingTripTimer.Value += 60000;
                        break;
                    case FlagType.Worldly:
                        // Allows catching of non-ocean fish
                        _trawlerRewards.Value.hasWorldly = true;
                        break;
                    case FlagType.SlimeKing:
                        // Consumes all fish but gives a 75% chance of converting each fish into some slime, 50% chance to convert to a Slimejack and a 1% chance to convert into a random slime egg
                        _trawlerRewards.Value.hasSlimeKing = true;
                        break;
                    case FlagType.KingCrab:
                        // Causes the trawler to only catch crab pot based creatures, higher chance of crab
                        _trawlerRewards.Value.hasKingCrab = true;
                        break;
                    default:
                        // Do nothing
                        break;
                }

                return;
            }
        }

        private void OnUpdateTicking(object sender, UpdateTickingEventArgs e)
        {
            if (!Context.IsWorldReady || !IsPlayerOnTrawler() || _isTripEnding.Value)
            {
                return;
            }

            if (Game1.activeClickableMenu != null && !Context.IsMultiplayer)
            {
                // Allow pausing in singleplayer via menu
                return;
            }

            if (_isNotificationFading)
            {
                _notificationAlpha -= 0.1f;
            }

            if (_notificationAlpha < 0f)
            {
                _activeNotification = String.Empty;
                _isNotificationFading = false;
                _notificationAlpha = 1f;
            }

            if (IsMainDeckhand())
            {
                if (e.IsMultipleOf(150))
                {
                    // Update water level (from leaks) every second
                    _trawlerHull.Value.RecaculateWaterLevel();
                    SyncTrawler(SyncType.WaterLevel, _trawlerHull.Value.GetWaterLevel(), GetFarmersOnTrawler());
                }
            }

            // Every quarter of a second play leaking sound, if there is a leak
            if (e.IsMultipleOf(15))
            {
                if (Game1.player.currentLocation is TrawlerHull && _trawlerHull.Value.HasLeak())
                {
                    Game1.playSoundPitched("wateringCan", Game1.random.Next(1, 5) * 100);
                }
            }

            if (e.IsMultipleOf(150))
            {
                if (!String.IsNullOrEmpty(_activeNotification))
                {
                    _isNotificationFading = true;
                }
            }

            if (_trawlerHull.Value.GetWaterLevel() == 100)
            {
                Monitor.Log($"Ending trip due to flooding for: {Game1.player.Name}", LogLevel.Trace);
                _trawlerSurface.Value.fishCaughtQuantity /= 4;

                // Set the status as failed
                Game1.player.modData[MURPHY_WAS_TRIP_SUCCESSFUL_KEY] = "false";
                Game1.player.modData[MURPHY_SAILED_TODAY_KEY] = "true";

                // End trip due to flooding
                Game1.player.currentLocation.playSound("fishEscape");
                Game1.player.CanMove = false;
                Game1.addHUDMessage(new HUDMessage(i18n.Get("game_message.trip_failed"), null));

                EndTrip();
            }
        }

        private void OnOneSecondUpdateTicking(object sender, OneSecondUpdateTickingEventArgs e)
        {
            if (!Context.IsWorldReady || !IsPlayerOnTrawler() || _isTripEnding.Value)
            {
                return;
            }

            if (Game1.activeClickableMenu != null && !Context.IsMultiplayer)
            {
                // Allow pausing in singleplayer via menu
                return;
            }

            // Iterate the fishing trip timer
            if (fishingTripTimer.Value > 0f)
            {
                fishingTripTimer.Value -= 1000;
            }

            // Update the track if needed
            if (themeSongUpdated)
            {
                themeSongUpdated = false;

                _trawlerCabin.Value.miniJukeboxTrack.Value = String.IsNullOrEmpty(trawlerThemeSong) ? null : trawlerThemeSong;
                _trawlerHull.Value.miniJukeboxTrack.Value = String.IsNullOrEmpty(trawlerThemeSong) ? null : trawlerThemeSong;
                _trawlerSurface.Value.miniJukeboxTrack.Value = String.IsNullOrEmpty(trawlerThemeSong) ? null : trawlerThemeSong;
            }

            if (IsMainDeckhand())
            {
                // Every 5 seconds recalculate the amount of fish caught / lost
                if (e.IsMultipleOf(300))
                {
                    _trawlerSurface.Value.UpdateFishCaught(_trawlerCabin.Value.AreAllPipesLeaking());
                    SyncTrawler(SyncType.FishCaught, _trawlerSurface.Value.fishCaughtQuantity, GetFarmersOnTrawler());
                }

                // Every random interval check for new event (leak, net tearing, etc.) on Trawler
                if (e.IsMultipleOf(_eventSecondInterval))
                {
                    string message = String.Empty;

                    // Check if the player gets lucky and skips getting an event, otherwise create the event(s)
                    if (Game1.random.NextDouble() < 0.05)
                    {
                        message = i18n.Get("status_message.sea_favors_us");
                    }
                    else
                    {
                        message = CreateTrawlerEventsAndGetMessage();
                    }

                    // Check for empty string 
                    if (String.IsNullOrEmpty(message))
                    {
                        message = i18n.Get("status_message.default");
                    }

                    if (_activeNotification != message)
                    {
                        _activeNotification = message;
                        BroadcastNotification(message, GetFarmersOnTrawler());
                    }

                    _eventSecondInterval = (uint)Game1.random.Next(config.eventFrequencyLower, config.eventFrequencyUpper + 1) * 100;
                }
            }

            if (fishingTripTimer.Value <= 0f)
            {
                // Set the status as successful
                Game1.player.modData[MURPHY_WAS_TRIP_SUCCESSFUL_KEY] = "true";
                Game1.player.modData[MURPHY_SAILED_TODAY_KEY] = "true";

                // End trip due to timer finishing
                Game1.player.currentLocation.playSound("trainWhistle");
                Game1.player.CanMove = false;

                Game1.addHUDMessage(new HUDMessage(i18n.Get("game_message.trip_succeeded"), null));

                EndTrip();
            }
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if ((!e.IsDown(SButton.MouseRight) && !e.IsDown(Buttons.A.ToSButton())) || !Context.IsWorldReady || Game1.activeClickableMenu != null)
            {
                return;
            }

            if (e.IsDown(Buttons.A.ToSButton()))
            {
                if (Game1.player.currentLocation.NameOrUniqueName == TRAWLER_HULL_LOCATION_NAME)
                {
                    for (int y = 0; y < 4; y++)
                    {
                        _trawlerHull.Value.AttemptPlugLeak((int)Game1.player.getTileX(), (int)Game1.player.getTileY() - y, Game1.player);
                        BroadcastTrawlerEvent(EventType.HullHole, new Vector2((int)Game1.player.getTileX(), (int)Game1.player.getTileY() - y), true, GetFarmersOnTrawler());
                    }
                }
                else if (Game1.player.currentLocation.NameOrUniqueName == TRAWLER_SURFACE_LOCATION_NAME)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        _trawlerSurface.Value.AttemptFixNet((int)Game1.player.getTileX(), (int)Game1.player.getTileY() - y, Game1.player);
                        BroadcastTrawlerEvent(EventType.NetTear, new Vector2((int)Game1.player.getTileX(), (int)Game1.player.getTileY() - y), true, GetFarmersOnTrawler());
                    }
                }
                else if (Game1.player.currentLocation.NameOrUniqueName == TRAWLER_CABIN_LOCATION_NAME)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        _trawlerCabin.Value.AttemptPlugLeak((int)Game1.player.getTileX(), (int)Game1.player.getTileY() - y, Game1.player);
                        BroadcastTrawlerEvent(EventType.EngineFailure, new Vector2((int)Game1.player.getTileX(), (int)Game1.player.getTileY() - y), true, GetFarmersOnTrawler());
                    }
                }
            }
            else
            {
                if (Game1.player.currentLocation.NameOrUniqueName == TRAWLER_HULL_LOCATION_NAME)
                {
                    _trawlerHull.Value.AttemptPlugLeak((int)e.Cursor.Tile.X, (int)e.Cursor.Tile.Y, Game1.player);
                    BroadcastTrawlerEvent(EventType.HullHole, new Vector2((int)e.Cursor.Tile.X, (int)e.Cursor.Tile.Y), true, GetFarmersOnTrawler());
                }
                else if (Game1.player.currentLocation.NameOrUniqueName == TRAWLER_SURFACE_LOCATION_NAME)
                {
                    // Attempt two checks, in case the user clicks above the rope
                    _trawlerSurface.Value.AttemptFixNet((int)e.Cursor.Tile.X, (int)e.Cursor.Tile.Y, Game1.player);
                    _trawlerSurface.Value.AttemptFixNet((int)e.Cursor.Tile.X, (int)e.Cursor.Tile.Y + 1, Game1.player);

                    BroadcastTrawlerEvent(EventType.NetTear, new Vector2((int)e.Cursor.Tile.X, (int)e.Cursor.Tile.Y), true, GetFarmersOnTrawler());
                    BroadcastTrawlerEvent(EventType.NetTear, new Vector2((int)e.Cursor.Tile.X, (int)e.Cursor.Tile.Y + 1), true, GetFarmersOnTrawler());
                }
                else if (Game1.player.currentLocation.NameOrUniqueName == TRAWLER_CABIN_LOCATION_NAME)
                {
                    _trawlerCabin.Value.AttemptPlugLeak((int)e.Cursor.Tile.X, (int)e.Cursor.Tile.Y, Game1.player);

                    BroadcastTrawlerEvent(EventType.EngineFailure, new Vector2((int)e.Cursor.Tile.X, (int)e.Cursor.Tile.Y), true, GetFarmersOnTrawler());
                }
            }
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Set our default config
            config = Helper.ReadConfig<ModConfig>();

            // Hook into the APIs we utilize
            if (Helper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu") && ApiManager.HookIntoGMCM(Helper))
            {
                // Register our config options
                var configAPI = ApiManager.GetGMCMInterface();
                configAPI.RegisterModConfig(ModManifest, () => config = new ModConfig(), () => Helper.WriteConfig(config));
                configAPI.RegisterClampedOption(ModManifest, i18n.Get("config.option.required_fishing_level.name"), i18n.Get("config.option.required_fishing_level.description"), () => config.minimumFishingLevel, (int val) => config.minimumFishingLevel = val, 0, 10);
                configAPI.RegisterClampedOption(ModManifest, i18n.Get("config.option.net_output.name"), i18n.Get("config.option.net_output.description"), () => config.fishPerNet, (float val) => config.fishPerNet = val, 0f, 1f, 0.5f);
                configAPI.RegisterClampedOption(ModManifest, i18n.Get("config.option.engine_boost.name"), i18n.Get("config.option.engine_boost.description"), () => config.engineFishBonus, (int val) => config.engineFishBonus = val, 0, 2);
                configAPI.RegisterClampedOption(ModManifest, i18n.Get("config.option.event_frequency_lower.name"), i18n.Get("config.option.event_frequency_lower.description"), () => config.eventFrequencyLower, (int val) => config.eventFrequencyLower = val, 1, 15);
                configAPI.RegisterClampedOption(ModManifest, i18n.Get("config.option.event_frequency_upper.name"), i18n.Get("config.option.event_frequency_upper.description"), () => config.eventFrequencyUpper, (int val) => config.eventFrequencyUpper = val, 1, 15);
                configAPI.RegisterChoiceOption(ModManifest, i18n.Get("config.option.murphy_appearance_day.name"), i18n.Get("config.option.murphy_appearance_day.description"), () => config.dayOfWeekChoice, (string val) => config.dayOfWeekChoice = val, ModConfig.murphyDayToAppear);
                configAPI.RegisterChoiceOption(ModManifest, i18n.Get("config.option.murphy_appearance_day_island.name"), i18n.Get("config.option.murphy_appearance_day_island.description"), () => config.dayOfWeekChoiceIsland, (string val) => config.dayOfWeekChoiceIsland = val, ModConfig.murphyDayToAppear);

                Monitor.Log($"{Game1.player.Name} has following config options -> [Min Fish Level]: {config.minimumFishingLevel} | [Fishing Net Output]: {config.fishPerNet} | [Engine Boost]: {config.engineFishBonus} | [Event Freq Lower]: {config.eventFrequencyLower} | [Event Freq Upper]: {config.eventFrequencyUpper} | [Day for Murphy]: {config.dayOfWeekChoice}", LogLevel.Trace);
            }

            if (Helper.ModRegistry.IsLoaded("Pathoschild.ContentPatcher") && ApiManager.HookIntoContentPatcher(Helper))
            {
                var patcherAPI = ApiManager.GetContentPatcherInterface();
                patcherAPI.RegisterToken(ModManifest, "MurphyAppearanceDay", () =>
                {
                    return new[] { Game1.MasterPlayer.modData.ContainsKey(MURPHY_DAY_TO_APPEAR) ? Game1.MasterPlayer.modData[MURPHY_DAY_TO_APPEAR] : config.dayOfWeekChoice };
                });
                patcherAPI.RegisterToken(ModManifest, "MurphyAppearanceDayIsland", () =>
                {
                    return new[] { Game1.MasterPlayer.modData.ContainsKey(MURPHY_DAY_TO_APPEAR_ISLAND) ? Game1.MasterPlayer.modData[MURPHY_DAY_TO_APPEAR_ISLAND] : config.dayOfWeekChoiceIsland };
                });
            }
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            // Set up notification messages
            MESSAGE_EVERYTHING_FAILING = new KeyValuePair<string, int>(i18n.Get("status_message.ship_falling_apart"), 10);
            MESSAGE_LOSING_FISH = new KeyValuePair<string, int>(i18n.Get("status_message.losing_fish"), 9);
            MESSAGE_MAX_LEAKS = new KeyValuePair<string, int>(i18n.Get("status_message.taking_on_water"), 8);
            MESSAGE_MULTI_PROBLEMS = new KeyValuePair<string, int>(i18n.Get("status_message.lots_of_problems"), 7);
            MESSAGE_ENGINE_PROBLEM = new KeyValuePair<string, int>(i18n.Get("status_message.engine_failing"), 7);
            MESSAGE_NET_PROBLEM = new KeyValuePair<string, int>(i18n.Get("status_message.nets_torn"), 6);
            MESSAGE_LEAK_PROBLEM = new KeyValuePair<string, int>(i18n.Get("status_message.leak"), 5);

            todayDayOfWeek = SDate.Now().DayOfWeek.ToString();

            Beach beach = Game1.getLocationFromName("Beach") as Beach;
            beach.modData[MURPHY_ON_TRIP] = "false";

            IslandSouthEast island = Game1.getLocationFromName("IslandSouthEast") as IslandSouthEast;
            island.modData[MURPHY_ON_TRIP] = "false";

            // Set Farmer moddata used for this mod
            EstablishPlayerData();

            if (Context.IsMainPlayer)
            {
                // Must be a user set date (default Wednesday), the player's fishing level >= 3 and the bridge must be fixed on the beach
                if (!Game1.MasterPlayer.mailReceived.Contains("PeacefulEnd.FishingTrawler_WillyIntroducesMurphy") && Game1.MasterPlayer.FishingLevel >= config.minimumFishingLevel && beach.bridgeFixed && todayDayOfWeek == Game1.MasterPlayer.modData[MURPHY_DAY_TO_APPEAR])
                {
                    Monitor.Log($"Sending {Game1.MasterPlayer.Name} intro letter about Murphy!", LogLevel.Trace);
                    Helper.Content.AssetEditors.Add(new CustomMail());
                    Game1.MasterPlayer.mailbox.Add("PeacefulEnd.FishingTrawler_WillyIntroducesMurphy");
                }

                // Must be a user set island date (default Satuday), met Murphy and Ginger Island's resort must be built
                IslandSouth resort = Game1.getLocationFromName("IslandSouth") as IslandSouth;
                if (!Game1.MasterPlayer.mailReceived.Contains("PeacefulEnd.FishingTrawler_MurphyGingerIsland") && Game1.MasterPlayer.mailReceived.Contains("PeacefulEnd.FishingTrawler_WillyIntroducesMurphy") && resort.resortRestored && todayDayOfWeek == Game1.MasterPlayer.modData[MURPHY_DAY_TO_APPEAR_ISLAND])
                {
                    Monitor.Log($"Sending {Game1.MasterPlayer.Name} Ginger Island letter about Murphy!", LogLevel.Trace);
                    Helper.Content.AssetEditors.Add(new CustomMail());
                    Game1.MasterPlayer.mailbox.Add("PeacefulEnd.FishingTrawler_MurphyGingerIsland");
                }
            }

            // Reset ownership of boat, deckhands
            mainDeckhand = null;
            numberOfDeckhands = 0;

            // Set the reward chest
            Vector2 rewardChestPosition = new Vector2(-100, -100);
            Farm farm = Game1.getLocationFromName("Farm") as Farm;
            rewardChest = farm.objects.Values.FirstOrDefault(o => o.modData.ContainsKey(REWARD_CHEST_DATA_KEY)) as Chest;
            if (rewardChest is null)
            {
                Monitor.Log($"Creating reward chest {rewardChestPosition}", LogLevel.Trace);
                rewardChest = new Chest(true, rewardChestPosition) { Name = "Trawler Rewards" };
                rewardChest.modData.Add(REWARD_CHEST_DATA_KEY, "true");

                farm.setObject(rewardChestPosition, rewardChest);
            }

            // Create the trawler object for the beach
            var locationContext = (todayDayOfWeek == Game1.MasterPlayer.modData[MURPHY_DAY_TO_APPEAR_ISLAND] ? GameLocation.LocationContext.Island : GameLocation.LocationContext.Default);
            if (todayDayOfWeek == Game1.MasterPlayer.modData[MURPHY_DAY_TO_APPEAR_ISLAND])
            {
                trawlerObject = new Trawler(island);
            }
            else
            {
                trawlerObject = new Trawler(beach);
            }

            // Create the TrawlerReward class
            _trawlerRewards.Value = new TrawlerRewards(rewardChest);

            // Add the surface location
            TrawlerSurface surfaceLocation = new TrawlerSurface(Path.Combine(ModResources.assetFolderPath, "Maps", "FishingTrawler.tmx"), TRAWLER_SURFACE_LOCATION_NAME) { IsOutdoors = true, IsFarm = false, locationContext = locationContext };
            Game1.locations.Add(surfaceLocation);

            // Add the hull location
            TrawlerHull hullLocation = new TrawlerHull(Path.Combine(ModResources.assetFolderPath, "Maps", "TrawlerHull.tmx"), TRAWLER_HULL_LOCATION_NAME) { IsOutdoors = false, IsFarm = false, locationContext = locationContext };
            Game1.locations.Add(hullLocation);

            // Add the cabin location
            TrawlerCabin cabinLocation = new TrawlerCabin(Path.Combine(ModResources.assetFolderPath, "Maps", "TrawlerCabin.tmx"), TRAWLER_CABIN_LOCATION_NAME) { IsOutdoors = false, IsFarm = false, locationContext = locationContext };
            Game1.locations.Add(cabinLocation);

            // Verify our locations were added and establish our location variables
            _trawlerHull.Value = Game1.getLocationFromName(TRAWLER_HULL_LOCATION_NAME) as TrawlerHull;
            _trawlerSurface.Value = Game1.getLocationFromName(TRAWLER_SURFACE_LOCATION_NAME) as TrawlerSurface;
            _trawlerCabin.Value = Game1.getLocationFromName(TRAWLER_CABIN_LOCATION_NAME) as TrawlerCabin;
        }

        private void OnDayEnding(object sender, DayEndingEventArgs e)
        {
            // Offload the custom locations
            Game1.locations.Remove(_trawlerHull.Value);
            Game1.locations.Remove(_trawlerSurface.Value);
            Game1.locations.Remove(_trawlerCabin.Value);
        }

        private string CreateTrawlerEventsAndGetMessage()
        {
            int amountOfEvents = 0;
            for (int x = 0; x < 4; x++)
            {
                // Chance of skipping an event increases with each pass of this loop
                if (Game1.random.NextDouble() < 0.1 + (x * 0.1f))
                {
                    // Skip event
                    continue;
                }

                amountOfEvents++;
            }

            int executedEvents = 0;
            List<KeyValuePair<string, int>> possibleMessages = new List<KeyValuePair<string, int>>();
            for (int x = 0; x < amountOfEvents; x++)
            {
                if (!_trawlerSurface.Value.AreAllNetsRipped() && Game1.random.NextDouble() < 0.35)
                {
                    Location tile = _trawlerSurface.Value.GetRandomWorkingNet();

                    _trawlerSurface.Value.AttemptCreateNetRip(tile.X, tile.Y);
                    BroadcastTrawlerEvent(EventType.NetTear, new Vector2(tile.X, tile.Y), false, GetFarmersOnTrawler());

                    possibleMessages.Add(_trawlerSurface.Value.AreAllNetsRipped() && _trawlerCabin.Value.AreAllPipesLeaking() ? MESSAGE_LOSING_FISH : MESSAGE_NET_PROBLEM);

                    executedEvents++;
                    continue;
                }

                if (!_trawlerCabin.Value.AreAllPipesLeaking() && Game1.random.NextDouble() < 0.25)
                {
                    Location tile = _trawlerCabin.Value.GetRandomWorkingPipe();

                    _trawlerCabin.Value.AttemptCreatePipeLeak(tile.X, tile.Y);
                    BroadcastTrawlerEvent(EventType.EngineFailure, new Vector2(tile.X, tile.Y), false, GetFarmersOnTrawler());

                    possibleMessages.Add(_trawlerSurface.Value.AreAllNetsRipped() && _trawlerCabin.Value.AreAllPipesLeaking() ? MESSAGE_LOSING_FISH : MESSAGE_ENGINE_PROBLEM);

                    executedEvents++;
                    continue;
                }

                // Default hull breaking event
                if (!_trawlerHull.Value.AreAllHolesLeaking() && _trawlerHull.Value.areLeaksEnabled)
                {
                    if (_trawlerHull.Value.hasWeakHull)
                    {
                        foreach (Location tile in _trawlerHull.Value.GetAllLeakableLocations())
                        {
                            _trawlerHull.Value.AttemptCreateHullLeak(tile.X, tile.Y);
                            BroadcastTrawlerEvent(EventType.HullHole, new Vector2(tile.X, tile.Y), false, GetFarmersOnTrawler());
                        }
                    }
                    else
                    {
                        Location tile = _trawlerHull.Value.GetRandomPatchedHullHole();

                        _trawlerHull.Value.AttemptCreateHullLeak(tile.X, tile.Y);
                        BroadcastTrawlerEvent(EventType.HullHole, new Vector2(tile.X, tile.Y), false, GetFarmersOnTrawler());
                    }

                    possibleMessages.Add(_trawlerHull.Value.AreAllHolesLeaking() ? MESSAGE_MAX_LEAKS : MESSAGE_LEAK_PROBLEM);

                    executedEvents++;
                    continue;
                }
            }

            // Check if all possible events are activated
            if (_trawlerSurface.Value.AreAllNetsRipped() && _trawlerCabin.Value.AreAllPipesLeaking() && _trawlerHull.Value.AreAllHolesLeaking())
            {
                possibleMessages.Add(MESSAGE_EVERYTHING_FAILING);
            }

            // Add a generic message if there are lots of issues
            if (executedEvents > 1)
            {
                possibleMessages.Add(MESSAGE_MULTI_PROBLEMS);
            }

            // Select highest priority item (priority == default_priority_level * frequency)
            return amountOfEvents == 0 ? i18n.Get("status_message.yoba_be_praised") : possibleMessages.OrderByDescending(m => m.Value * possibleMessages.Count(p => p.Key == m.Key)).FirstOrDefault().Key;
        }

        internal static void AlertPlayersOfDeparture(long mainDeckhandID, List<Farmer> farmersToAlert)
        {
            if (Context.IsMultiplayer)
            {
                modHelper.Multiplayer.SendMessage(new DepartureMessage(mainDeckhandID), nameof(DepartureMessage), new[] { manifest.UniqueID }, farmersToAlert.Select(f => f.UniqueMultiplayerID).ToArray());
            }
        }

        internal static void BroadcastTrawlerEvent(EventType eventType, Vector2 locationOfEvent, bool isRepairing, List<Farmer> farmersToAlert)
        {
            if (Context.IsMultiplayer)
            {
                modHelper.Multiplayer.SendMessage(new TrawlerEventMessage(eventType, locationOfEvent, isRepairing), nameof(TrawlerEventMessage), new[] { manifest.UniqueID }, farmersToAlert.Select(f => f.UniqueMultiplayerID).ToArray());
            }
        }

        internal static void SyncTrawler(SyncType syncType, int quantity, List<Farmer> farmersToAlert)
        {
            if (Context.IsMultiplayer)
            {
                modHelper.Multiplayer.SendMessage(new TrawlerSyncMessage(syncType, quantity), nameof(TrawlerSyncMessage), new[] { manifest.UniqueID }, farmersToAlert.Select(f => f.UniqueMultiplayerID).ToArray());
            }
        }

        internal static void BroadcastNotification(string notification, List<Farmer> farmersToAlert)
        {
            if (Context.IsMultiplayer)
            {
                modHelper.Multiplayer.SendMessage(new TrawlerNotificationMessage(notification), nameof(TrawlerNotificationMessage), new[] { manifest.UniqueID }, farmersToAlert.Select(f => f.UniqueMultiplayerID).ToArray());
            }
        }

        internal static void SetTrawlerTheme(string songName)
        {
            trawlerThemeSong = songName;
            themeSongUpdated = true;
        }

        internal static bool IsPlayerOnTrawler()
        {
            return IsValidTrawlerLocation(Game1.player.currentLocation);
        }

        private static bool IsValidTrawlerLocation(GameLocation location)
        {
            switch (location)
            {
                case TrawlerSurface surface:
                case TrawlerHull hull:
                case TrawlerCabin cabin:
                    return true;
                default:
                    return false;
            }
        }

        internal static List<Farmer> GetFarmersOnTrawler()
        {
            return Game1.getAllFarmers().Where(f => IsValidTrawlerLocation(f.currentLocation)).ToList();
        }

        internal static bool ShouldMurphyAppear(GameLocation location)
        {
            if (Game1.MasterPlayer.mailReceived.Contains("PeacefulEnd.FishingTrawler_WillyIntroducesMurphy"))
            {
                if (location is Beach && !Game1.isStartingToGetDarkOut() && todayDayOfWeek == Game1.MasterPlayer.modData[MURPHY_DAY_TO_APPEAR] && (!location.modData.ContainsKey(MURPHY_ON_TRIP) || location.modData[MURPHY_ON_TRIP] == "false"))
                {
                    return true;
                }
            }

            if (Game1.MasterPlayer.mailReceived.Contains("PeacefulEnd.FishingTrawler_MurphyGingerIsland"))
            {
                if (location is IslandSouthEast && !Game1.isStartingToGetDarkOut() && todayDayOfWeek == Game1.MasterPlayer.modData[MURPHY_DAY_TO_APPEAR_ISLAND] && (!location.modData.ContainsKey(MURPHY_ON_TRIP) || location.modData[MURPHY_ON_TRIP] == "false"))
                {
                    return true;
                }
            }

            return false;
        }

        internal static void SpawnMurphy(GameLocation location)
        {
            if (location is IslandSouthEast)
            {
                murphyNPC = new Murphy(new AnimatedSprite(ModResources.murphyTexturePath, 0, 16, 32), new Vector2(12.05f, 39.5f) * 64f, 2, i18n.Get("etc.murphy_name"), ModResources.murphyPortraitTexture);
            }
            else
            {
                murphyNPC = new Murphy(new AnimatedSprite(ModResources.murphyTexturePath, 0, 16, 32), new Vector2(89f, 38.5f) * 64f, 2, i18n.Get("etc.murphy_name"), ModResources.murphyPortraitTexture);
            }
        }

        internal static bool IsMainDeckhand()
        {
            return mainDeckhand != null && mainDeckhand == Game1.player ? true : false;
        }

        internal static FlagType GetHoistedFlag()
        {
            Farmer flagOwner = mainDeckhand != null ? mainDeckhand : Game1.player;
            return Enum.TryParse(flagOwner.modData[HOISTED_FLAG_KEY], out FlagType flagType) ? flagType : FlagType.Unknown;
        }

        internal static void SetHoistedFlag(FlagType flagType)
        {
            Game1.player.modData[HOISTED_FLAG_KEY] = flagType.ToString();
        }

        internal static bool HasFarmerGoneSailing(Farmer who)
        {
            return who.modData.ContainsKey(MURPHY_SAILED_TODAY_KEY) && who.modData[MURPHY_SAILED_TODAY_KEY] == "true";
        }

        private void EstablishPlayerData()
        {
            if (!Game1.player.modData.ContainsKey(HOISTED_FLAG_KEY))
            {
                Game1.player.modData.Add(HOISTED_FLAG_KEY, FlagType.Unknown.ToString());
            }
            else
            {
                SetHoistedFlag(Enum.TryParse(Game1.player.modData[HOISTED_FLAG_KEY], out FlagType flagType) ? flagType : FlagType.Unknown);
            }

            Game1.player.modData[MURPHY_WAS_GREETED_TODAY_KEY] = "false";
            Game1.player.modData[MURPHY_DAY_TO_APPEAR] = config.dayOfWeekChoice;

            Game1.player.modData[MURPHY_DAY_TO_APPEAR_ISLAND] = config.dayOfWeekChoiceIsland;
            if (config.dayOfWeekChoice == config.dayOfWeekChoiceIsland)
            {
                // Prevent Murphy from showing up on the beach and island on same day by offsetting it by 1 (or setting it the first index)
                int currentIndex = Array.IndexOf(ModConfig.murphyDayToAppear, config.dayOfWeekChoiceIsland) + 1;
                if (currentIndex > ModConfig.murphyDayToAppear.Length - 1)
                {
                    currentIndex = 0;
                }

                Game1.player.modData[MURPHY_DAY_TO_APPEAR_ISLAND] = ModConfig.murphyDayToAppear[currentIndex];
            }

            if (!Game1.player.modData.ContainsKey(MURPHY_SAILED_TODAY_KEY))
            {
                Game1.player.modData.Add(MURPHY_SAILED_TODAY_KEY, "false");
                Game1.player.modData.Add(MURPHY_WAS_TRIP_SUCCESSFUL_KEY, "false");
                Game1.player.modData.Add(MURPHY_FINISHED_TALKING_KEY, "false");
            }
            else if (todayDayOfWeek == Game1.MasterPlayer.modData[MURPHY_DAY_TO_APPEAR] || todayDayOfWeek == Game1.MasterPlayer.modData[MURPHY_DAY_TO_APPEAR_ISLAND]) // This is intended, as we want MasterPlayer to determine Murphy appearance
            {
                Game1.player.modData[MURPHY_SAILED_TODAY_KEY] = "false";
                Game1.player.modData[MURPHY_WAS_TRIP_SUCCESSFUL_KEY] = "false";
                Game1.player.modData[MURPHY_FINISHED_TALKING_KEY] = "false";
            }

            // One time events, do not renew
            if (!Game1.player.modData.ContainsKey(MURPHY_HAS_SEEN_FLAG_KEY))
            {
                Game1.player.modData.Add(MURPHY_HAS_SEEN_FLAG_KEY, "false");
            }

            if (!Game1.player.modData.ContainsKey(MURPHY_TRIPS_COMPLETED))
            {
                Game1.player.modData[MURPHY_TRIPS_COMPLETED] = "0";
            }
        }

        internal void UpdateLocalTrawlerMap(EventType eventType, Vector2 tile, bool isRepairing)
        {
            bool result = false;
            switch (eventType)
            {
                case EventType.HullHole:
                    result = isRepairing ? _trawlerHull.Value.AttemptPlugLeak((int)tile.X, (int)tile.Y, Game1.player, true) : _trawlerHull.Value.AttemptCreateHullLeak((int)tile.X, (int)tile.Y);
                    break;
                case EventType.EngineFailure:
                    result = isRepairing ? _trawlerCabin.Value.AttemptPlugLeak((int)tile.X, (int)tile.Y, Game1.player, true) : _trawlerCabin.Value.AttemptCreatePipeLeak((int)tile.X, (int)tile.Y);
                    break;
                case EventType.NetTear:
                    result = isRepairing ? _trawlerSurface.Value.AttemptFixNet((int)tile.X, (int)tile.Y, Game1.player, true) : _trawlerSurface.Value.AttemptCreateNetRip((int)tile.X, (int)tile.Y);
                    break;
                default:
                    monitor.Log($"A trawler event was received, but its EventType was not handled: {eventType}", LogLevel.Debug);
                    break;
            }
        }

        internal void SyncLocalTrawlerMap(SyncType syncType, int quantity)
        {
            bool result = false;
            switch (syncType)
            {
                case SyncType.WaterLevel:
                    result = true;
                    _trawlerHull.Value.RecaculateWaterLevel(quantity);
                    break;
                case SyncType.FishCaught:
                    result = true;
                    _trawlerSurface.Value.UpdateFishCaught(false, quantity);
                    break;
                default:
                    monitor.Log($"A trawler tried tried to sync, but its SyncType was not handled: {syncType}", LogLevel.Debug);
                    break;
            }
        }

        internal void EndTrip()
        {
            // Give the player(s) their rewards, if they left the trawler as expected (warping out early does not give any rewards)
            Monitor.Log($"Trip is ending for {GetFarmersOnTrawler().Count()} deckhands...", LogLevel.Trace);
            _trawlerRewards.Value.CalculateAndPopulateReward(_trawlerSurface.Value.fishCaughtQuantity);

            if (trawlerObject.location is IslandSouthEast)
            {
                DelayedAction.warpAfterDelay("IslandSouthEast", new Point(9, 39), 2500);
            }
            else
            {
                DelayedAction.warpAfterDelay("Beach", new Point(86, 38), 2500);
            }


            _isTripEnding.Value = true;

            // Increment trip counter for this player by one
            Game1.player.modData[MURPHY_TRIPS_COMPLETED] = (int.Parse(Game1.player.modData[MURPHY_TRIPS_COMPLETED]) + 1).ToString();
        }

        // Debug commands
        private void DebugGetAllFlags(string command, string[] args)
        {
            foreach (FlagType flagType in Enum.GetValues(typeof(FlagType)))
            {
                Game1.player.addItemByMenuIfNecessary(new AncientFlag(flagType));
            }
            Monitor.Log($"Giving all ancient flags to {Game1.player}.", LogLevel.Trace);
        }

        private void DebugGetSpecialRewards(string command, string[] args)
        {
            Game1.player.addItemByMenuIfNecessary(new AnglerRing());
            Monitor.Log($"Giving all special rewards to {Game1.player}.", LogLevel.Trace);
        }
    }
}
