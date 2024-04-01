/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FishingTrawler
**
*************************************************/

using FishingTrawler.Framework.External.GenericModConfigMenu;
using FishingTrawler.Framework.Managers;
using FishingTrawler.Framework.Objects.Items.Resources;
using FishingTrawler.Framework.Objects.Items.Rewards;
using FishingTrawler.Framework.Objects.Items.Tools;
using FishingTrawler.Framework.Patches.Characters;
using FishingTrawler.Framework.Patches.Objects;
using FishingTrawler.Framework.Patches.SMAPI;
using FishingTrawler.Framework.Patches.xTiles;
using FishingTrawler.Framework.Utilities;
using FishingTrawler.GameLocations;
using FishingTrawler.Messages;
using FishingTrawler.Objects;
using FishingTrawler.Patches.Locations;
using FishingTrawler.UI;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FishingTrawler
{
    public class FishingTrawler : Mod
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

        // Managers
        internal static ApiManager apiManager;
        internal static AssetManager assetManager;
        internal static EventManager eventManager;
        internal static NotificationManager notificationManager;

        // Trawler beach map related
        internal static Murphy murphyNPC;
        internal static Trawler trawlerObject;
        internal static Chest rewardChest;

        // Trawler map / texture related
        private readonly PerScreen<TrawlerHull> _trawlerHull = new PerScreen<TrawlerHull>();
        private readonly PerScreen<TrawlerSurface> _trawlerSurface = new PerScreen<TrawlerSurface>();
        private readonly PerScreen<TrawlerCabin> _trawlerCabin = new PerScreen<TrawlerCabin>();
        private readonly PerScreen<TrawlerRewards> _trawlerRewards = new PerScreen<TrawlerRewards>();
        private PerScreen<bool> _isTripEnding = new PerScreen<bool>();
        private string _trawlerItemsPath = Path.Combine("assets", "TrawlerItems");

        // Day to appear settings
        internal const string BOAT_DEPART_EVENT_ID = "840603900";

        public override void Entry(IModHelper helper)
        {
            // Set up the monitor and helper
            monitor = Monitor;
            modHelper = helper;
            manifest = ModManifest;
            multiplayer = helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
            i18n = helper.Translation;

            // Load managers
            apiManager = new ApiManager(monitor);
            assetManager = new AssetManager(monitor, modHelper);
            eventManager = new EventManager(monitor);
            notificationManager = new NotificationManager(monitor);

            // Initialize the timer for fishing trip
            eventManager.SetTripTimer(0);

            // Set up our notification system on the trawler
            _isTripEnding.Value = false;

            // Load our Harmony patches
            try
            {
                var harmony = new Harmony(ModManifest.UniqueID);

                // Apply Location patches
                new BeachPatch(monitor, modHelper).Apply(harmony);
                new IslandSouthEastPatch(monitor, modHelper).Apply(harmony);
                new GameLocationPatch(monitor, modHelper).Apply(harmony);

                // Apply SMAPI patches
                new DisplayDevicePatch(monitor, modHelper).Apply(harmony);

                // Apply xTile patches
                new LayerPatch(monitor, modHelper).Apply(harmony);

                // Apply Object patches
                new ObjectPatch(monitor, modHelper).Apply(harmony);
                new RingPatch(monitor, modHelper).Apply(harmony);
                new FurniturePatch(monitor, modHelper).Apply(harmony);
                new ToolPatch(monitor, modHelper).Apply(harmony);
                new FishingRodPatch(monitor, modHelper).Apply(harmony);

                // Apply Character patches
                new FarmerPatch(monitor, modHelper).Apply(harmony);

                // Apply Bells and Whistles patch
                new ScreenFadePatch(monitor, modHelper).Apply(harmony);

                // Apply Core patches
                new GamePatch(monitor, modHelper).Apply(harmony);
            }
            catch (Exception e)
            {
                Monitor.Log($"Issue with Harmony patching: {e}", LogLevel.Error);
                return;
            }

            // Add in our debug commands
            helper.ConsoleCommands.Add("ft_get_flags", "Gives all the variations of the ancient flag.\n\nUsage: ft_get_flags", DebugGetAllFlags);
            helper.ConsoleCommands.Add("ft_get_specials", "Gives all the special rewards.\n\nUsage: ft_get_specials", DebugGetSpecialRewards);
            helper.ConsoleCommands.Add("ft_skip_requirements", "Skips all requirements to meet Murphy and enables the minigame.\n\nUsage: ft_skip_requirements", DebugSkipRequirements);
            helper.ConsoleCommands.Add("ft_warp", "Warps to the entrance of the minigame.\n\nUsage: ft_warp", delegate { Monitor.Log($"Warping {Game1.player.Name} to Fishing Trawler minigame entrance!", LogLevel.Debug); if (ShouldMurphyAppear(Game1.getLocationFromName("IslandSouthEast"))) Game1.warpFarmer("IslandSouthEast", 10, 27, 2); else Game1.warpFarmer("Beach", 86, 37, 2); });

            // Hook into Content related events
            helper.Events.Content.AssetRequested += OnAssetRequested;
            helper.Events.Content.AssetsInvalidated += OnAssetsInvalidated;

            // Hook into GameLoops related events
            helper.Events.GameLoop.UpdateTicking += OnUpdateTicking;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.DayEnding += OnDayEnding;

            // Hook into Display related events
            helper.Events.Display.RenderedWorld += OnRenderedWorld;

            // Hook into Player related events
            helper.Events.Player.Warped += OnWarped;

            // Hook into MouseClicked
            helper.Events.Input.ButtonPressed += OnButtonPressed;

            // Hook into Multiplayer related
            helper.Events.Multiplayer.PeerConnected += OnPeerConnected;
            helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
        }

        private void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID != ModManifest.UniqueID)
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
                    notificationManager.SetNotification(notificationMessage.Notification);
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

        private void OnRenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            if (!IsPlayerOnTrawler())
            {
                return;
            }

            if (!String.IsNullOrEmpty(notificationManager.GetActiveNotification()))
            {
                notificationManager.DrawNotification(e.SpriteBatch, Game1.player.currentLocation);
            }

            TrawlerUI.DrawUI(e.SpriteBatch, eventManager.GetTripTimer(), _trawlerSurface.Value.fishCaughtQuantity, _trawlerHull.Value.GetWaterLevel(), _trawlerHull.Value.HasLeak(), _trawlerSurface.Value.GetRippedNetsCount(), _trawlerHull.Value.GetFuelLevel(), _trawlerCabin.Value.IsComputerReady(), _trawlerCabin.Value.HasLeftCabin());
        }

        private void OnWarped(object sender, WarpedEventArgs e)
        {
            // Check if player just left the trawler
            if (!IsPlayerOnTrawler() && IsValidTrawlerLocation(e.OldLocation))
            {
                if (murphyNPC is null && (e.NewLocation is Beach || e.NewLocation is IslandSouthEast))
                {
                    // Spawn Murphy, if he isn't already there
                    e.NewLocation.modData[ModDataKeys.MURPHY_ON_TRIP] = "false";
                    SpawnMurphy(e.NewLocation);
                }

                // Set the theme to null
                SetTrawlerTheme(null);
                Game1.changeMusicTrack("none");

                numberOfDeckhands = 0;
                mainDeckhand = null;

                // Take away any bailing buckets
                foreach (var bucket in Game1.player.Items.Where(i => i is Tool tool && new BailingBucket(tool).IsValid))
                {
                    Game1.player.removeItemFromInventory(bucket);
                }

                // Take away any fuel clumps
                foreach (var fuelClump in Game1.player.Items.Where(i => CoalClump.IsValid(i)))
                {
                    Game1.player.removeItemFromInventory(fuelClump);
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
                if (Game1.player.Items.Any(i => i is Tool tool && new BailingBucket(tool).IsValid) is false)
                {
                    Game1.player.addItemToInventory(BailingBucket.CreateInstance());
                    Game1.addHUDMessage(new HUDMessage(i18n.Get("game_message.given_bailing_bucket")) { timeLeft = 1250f, noIcon = true });
                }

                // Clear any previous reward data, set the head deckhand (which determines fishing level for reward calc)
                _trawlerRewards.Value.Reset(Game1.player);

                // Start the timer (2.5 minute default)
                eventManager.SetTripTimer(150000); //150000

                // Setup the cabin
                _trawlerCabin.Value.Reset();

                // Setup the hull
                _trawlerHull.Value.Reset();

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
                        eventManager.IncrementTripTimer(60000);
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
                    case FlagType.EternalFlame:
                        // Doubles net output, but doubles fuel consumption
                        _trawlerSurface.Value.fishCaughtMultiplier = 2;
                        _trawlerHull.Value.fuelConsumptionIncrement = -Math.Abs(_trawlerHull.Value.fuelConsumptionIncrement * 2);
                        break;
                    case FlagType.SwiftWinds:
                        // Increase Murphy's coffee buff to 3 minutes and increase the power by 1
                        _trawlerCabin.Value.hasSwiftWinds = true;
                        break;
                    default:
                        // Do nothing
                        break;
                }

                return;
            }

            // Check if player is warping between traweler locations and needs to adjust for useOldTrawlerSprite
            if (IsPlayerOnTrawler() && IsValidTrawlerLocation(e.NewLocation) && config.useOldTrawlerSprite is true)
            {
                if (e.OldLocation is TrawlerCabin)
                {
                    e.Player.setTileLocation(new Vector2(42, 26));
                }
                else if (e.OldLocation is TrawlerHull)
                {
                    e.Player.setTileLocation(new Vector2(38, 24));
                }
            }
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Mail"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, string>().Data;
                    data[ModDataKeys.MAIL_FLAG_MURPHY_WAS_INTRODUCED] = string.Format(FishingTrawler.i18n.Get("letter.meet_murphy"), Game1.MasterPlayer.modData.ContainsKey(ModDataKeys.MURPHY_DAY_TO_APPEAR) ? Game1.MasterPlayer.modData[ModDataKeys.MURPHY_DAY_TO_APPEAR] : config.dayOfWeekChoice);
                    data[ModDataKeys.MAIL_FLAG_MURPHY_FOUND_GINGER_ISLAND] = string.Format(FishingTrawler.i18n.Get("letter.island_murphy"), Game1.MasterPlayer.modData.ContainsKey(ModDataKeys.MURPHY_DAY_TO_APPEAR_ISLAND) ? Game1.MasterPlayer.modData[ModDataKeys.MURPHY_DAY_TO_APPEAR_ISLAND] : config.dayOfWeekChoiceIsland);
                });
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/ChairTiles"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, string>().Data;
                    data["TrawlerCabin/12/5"] = "1/1/down/default/-1/-1/false";
                });
            }
            else if (e.DataType == typeof(Texture2D))
            {
                var asset = e.Name;
                if (assetManager.Textures.ContainsKey(asset.Name))
                {
                    e.LoadFrom(() => assetManager.Textures[asset.Name], AssetLoadPriority.Low);
                }
            }
        }

        private void OnAssetsInvalidated(object sender, AssetsInvalidatedEventArgs e)
        {
            foreach (var asset in e.Names)
            {
                if (assetManager.Textures.ContainsKey(asset.Name))
                {
                    assetManager.Textures[asset.Name] = Helper.GameContent.Load<Texture2D>(asset);
                }
            }
        }

        private void OnUpdateTicking(object sender, UpdateTickingEventArgs e)
        {
            if (Context.IsWorldReady is false || IsPlayerOnTrawler() is false || _isTripEnding.Value)
            {
                return;
            }

            if ((Game1.activeClickableMenu is not null || Game1.game1.IsActive is false) && Context.IsMultiplayer is false)
            {
                // Allow pausing in singleplayer via menu
                return;
            }

            // Fade the notification, if applicable
            notificationManager.FadeNotification(0.1f);

            if (IsMainDeckhand() && _trawlerCabin.Value.HasLeftCabin())
            {
                _trawlerSurface.Value.SetFlagTexture(GetHoistedFlag());
                eventManager.UpdateEvents(e, _trawlerCabin.Value, _trawlerSurface.Value, _trawlerHull.Value);
            }

            // Every quarter of a second play leaking sound, if there is a leak
            if (e.IsMultipleOf(15))
            {
                if (Game1.player.currentLocation is TrawlerHull && _trawlerHull.Value.HasLeak())
                {
                    Game1.playSound("wateringCan", Game1.random.Next(1, 5) * 100);
                }
            }

            // Update TrawlerSurface with TrawlerHull's fuel level
            TrawlerSurface.hullFuelLevel = _trawlerHull.Value.GetFuelLevel();

            // Start fading the message after 3 seconds
            if (notificationManager.HasExpired(Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds) is true)
            {
                notificationManager.StartFading();
            }

            // End the trip if the ship has flooded or the timer has run out
            if (_trawlerHull.Value.HasFlooded())
            {
                Monitor.Log($"Ending trip due to flooding for: {Game1.player.Name}", LogLevel.Trace);
                _trawlerSurface.Value.fishCaughtQuantity /= 4;

                // Set the status as failed
                Game1.player.modData[ModDataKeys.MURPHY_WAS_TRIP_SUCCESSFUL_KEY] = "false";
                Game1.player.modData[ModDataKeys.MURPHY_SAILED_TODAY_KEY] = "true";

                // End trip due to flooding
                Game1.player.currentLocation.playSound("fishEscape");
                Game1.player.CanMove = false;
                Game1.addHUDMessage(new HUDMessage(i18n.Get("game_message.trip_failed")));

                EndTrip();
            }
            else if (eventManager.GetTripTimer() <= 0f)
            {
                // Set the status as successful
                Game1.player.modData[ModDataKeys.MURPHY_WAS_TRIP_SUCCESSFUL_KEY] = "true";
                Game1.player.modData[ModDataKeys.MURPHY_SAILED_TODAY_KEY] = "true";

                // End trip due to timer finishing
                Game1.player.currentLocation.playSound("trainWhistle");
                Game1.player.CanMove = false;

                Game1.addHUDMessage(new HUDMessage(i18n.Get("game_message.trip_succeeded")));

                EndTrip();
            }
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!e.IsDown(SButton.MouseRight) && !e.IsDown(Buttons.A.ToSButton()) || !Context.IsWorldReady || Game1.activeClickableMenu != null)
            {
                return;
            }

            if (e.IsDown(Buttons.A.ToSButton()))
            {
                if (Game1.player.currentLocation.NameOrUniqueName == ModDataKeys.TRAWLER_HULL_LOCATION_NAME)
                {
                    for (int y = 0; y < 4; y++)
                    {
                        _trawlerHull.Value.AttemptPlugLeak((int)Game1.player.Tile.X, (int)(Game1.player.Tile.Y - y), Game1.player);
                        BroadcastTrawlerEvent(EventType.HullHole, new Vector2(Game1.player.Tile.X, Game1.player.Tile.Y - y), true, GetFarmersOnTrawler());
                    }
                }
                else if (Game1.player.currentLocation.NameOrUniqueName == ModDataKeys.TRAWLER_SURFACE_LOCATION_NAME)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        _trawlerSurface.Value.AttemptFixNet((int)Game1.player.Tile.X, (int)(Game1.player.Tile.Y - y), Game1.player);
                        BroadcastTrawlerEvent(EventType.NetTear, new Vector2(Game1.player.Tile.X, Game1.player.Tile.Y - y), true, GetFarmersOnTrawler());
                    }
                }
            }
            else
            {
                if (Game1.player.currentLocation.NameOrUniqueName == ModDataKeys.TRAWLER_HULL_LOCATION_NAME)
                {
                    _trawlerHull.Value.AttemptPlugLeak((int)e.Cursor.Tile.X, (int)e.Cursor.Tile.Y, Game1.player);
                    BroadcastTrawlerEvent(EventType.HullHole, new Vector2((int)e.Cursor.Tile.X, (int)e.Cursor.Tile.Y), true, GetFarmersOnTrawler());
                }
                else if (Game1.player.currentLocation.NameOrUniqueName == ModDataKeys.TRAWLER_SURFACE_LOCATION_NAME)
                {
                    // Attempt two checks, in case the user clicks above the rope
                    _trawlerSurface.Value.AttemptFixNet((int)e.Cursor.Tile.X, (int)e.Cursor.Tile.Y, Game1.player);
                    _trawlerSurface.Value.AttemptFixNet((int)e.Cursor.Tile.X, (int)e.Cursor.Tile.Y + 1, Game1.player);

                    BroadcastTrawlerEvent(EventType.NetTear, new Vector2((int)e.Cursor.Tile.X, (int)e.Cursor.Tile.Y), true, GetFarmersOnTrawler());
                    BroadcastTrawlerEvent(EventType.NetTear, new Vector2((int)e.Cursor.Tile.X, (int)e.Cursor.Tile.Y + 1), true, GetFarmersOnTrawler());
                }
            }
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Set our default config
            config = Helper.ReadConfig<ModConfig>();

            // Hook into the APIs we utilize
            if (Helper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu") && apiManager.HookIntoGMCM(Helper))
            {
                // Register our config options
                var configAPI = apiManager.GetGMCMInterface();
                configAPI.Register(ModManifest, () => config = new ModConfig(), () => Helper.WriteConfig(config), titleScreenOnly: true);
                configAPI.AddNumberOption(ModManifest, () => config.minimumFishingLevel, value => config.minimumFishingLevel = value, () => i18n.Get("config.option.required_fishing_level.name"), () => i18n.Get("config.option.required_fishing_level.description"), 0, 10);
                configAPI.AddBoolOption(ModManifest, () => config.disableScreenFade, (val) => config.disableScreenFade = val, () => i18n.Get("config.option.disable_screen_fade.name"), () => i18n.Get("config.option.disable_screen_fade.description"));
                configAPI.AddBoolOption(ModManifest, () => config.useOldTrawlerSprite, (val) => config.useOldTrawlerSprite = val, () => i18n.Get("config.option.use_old_trawler_sprite.name"), () => i18n.Get("config.option.use_old_trawler_sprite.description")); ;
                configAPI.AddNumberOption(ModManifest, () => config.fishPerNet, (val) => config.fishPerNet = val, () => i18n.Get("config.option.net_output.name"), () => i18n.Get("config.option.net_output.description"), 0f, 1f, 0.5f);
                configAPI.AddNumberOption(ModManifest, () => config.engineFishBonus, value => config.engineFishBonus = value, () => i18n.Get("config.option.engine_boost.name"), () => i18n.Get("config.option.engine_boost.description"), 0, 2);
                configAPI.AddNumberOption(ModManifest, () => config.hullEventFrequencyUpper, (val) => config.hullEventFrequencyUpper = val, () => i18n.Get("config.option.hull.event_frequency_upper.name"), () => i18n.Get("config.option.hull.event_frequency_upper.description"), 1, 15);
                configAPI.AddNumberOption(ModManifest, () => config.hullEventFrequencyLower, (val) => config.hullEventFrequencyLower = val, () => i18n.Get("config.option.hull.event_frequency_lower.name"), () => i18n.Get("config.option.hull.event_frequency_lower.description"), 1, 15);
                configAPI.AddNumberOption(ModManifest, () => config.netEventFrequencyUpper, (val) => config.netEventFrequencyUpper = val, () => i18n.Get("config.option.net.event_frequency_upper.name"), () => i18n.Get("config.option.net.event_frequency_upper.description"), 1, 15);
                configAPI.AddNumberOption(ModManifest, () => config.netEventFrequencyLower, (val) => config.netEventFrequencyLower = val, () => i18n.Get("config.option.net.event_frequency_lower.name"), () => i18n.Get("config.option.net.event_frequency_lower.description"), 1, 15);
                configAPI.AddTextOption(ModManifest, () => config.dayOfWeekChoice, (val) => config.dayOfWeekChoice = val, () => i18n.Get("config.option.murphy_appearance_day.name"), () => i18n.Get("config.option.murphy_appearance_day.description"), ModConfig.murphyDayToAppear);
                configAPI.AddTextOption(ModManifest, () => config.dayOfWeekChoiceIsland, (val) => config.dayOfWeekChoiceIsland = val, () => i18n.Get("config.option.murphy_appearance_day_island.name"), () => i18n.Get("config.option.murphy_appearance_day_island.description"), ModConfig.murphyDayToAppear);

                Monitor.Log($"{Game1.player.Name} has following config options -> [Min Fish Level]: {config.minimumFishingLevel} | [Fishing Net Output]: {config.fishPerNet} | [Engine Boost]: {config.engineFishBonus} | [Hull Event Freq Lower]: {config.hullEventFrequencyLower} | [Hull Event Freq Upper]: {config.hullEventFrequencyUpper} | [Net Event Freq Lower]: {config.netEventFrequencyLower} | [Net Event Freq Upper]: {config.netEventFrequencyUpper} | [Day for Murphy]: {config.dayOfWeekChoice}", LogLevel.Trace);
            }

            if (Helper.ModRegistry.IsLoaded("Pathoschild.ContentPatcher") && apiManager.HookIntoContentPatcher(Helper))
            {
                var patcherAPI = apiManager.GetContentPatcherInterface();
                patcherAPI.RegisterToken(ModManifest, "MurphyAppearanceDay", () =>
                {
                    return new[] { Game1.MasterPlayer.modData.ContainsKey(ModDataKeys.MURPHY_DAY_TO_APPEAR) ? Game1.MasterPlayer.modData[ModDataKeys.MURPHY_DAY_TO_APPEAR] : config.dayOfWeekChoice };
                });
                patcherAPI.RegisterToken(ModManifest, "MurphyAppearanceDayIsland", () =>
                {
                    return new[] { Game1.MasterPlayer.modData.ContainsKey(ModDataKeys.MURPHY_DAY_TO_APPEAR_ISLAND) ? Game1.MasterPlayer.modData[ModDataKeys.MURPHY_DAY_TO_APPEAR_ISLAND] : config.dayOfWeekChoiceIsland };
                });
            }

            if (Helper.ModRegistry.IsLoaded("PeacefulEnd.DynamicReflections") && apiManager.HookIntoDynamicReflections(Helper))
            {
                // Do nothing here
            }

            // Make our internal textures available to the game
            foreach (var textureName in assetManager.Textures.Keys)
            {
                var loadedTexture = Helper.GameContent.Load<Texture2D>(textureName);
                assetManager.Textures[textureName] = loadedTexture;
            }
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // TODO: Convert modded items to vanilla
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            todayDayOfWeek = SDate.Now().DayOfWeek.ToString();

            Beach beach = Game1.getLocationFromName("Beach") as Beach;
            beach.modData[ModDataKeys.MURPHY_ON_TRIP] = "false";

            IslandSouthEast island = Game1.getLocationFromName("IslandSouthEast") as IslandSouthEast;
            island.modData[ModDataKeys.MURPHY_ON_TRIP] = "false";

            // Set Farmer moddata used for this mod
            EstablishPlayerData();

            if (Context.IsMainPlayer)
            {
                // Must be a user set date (default Wednesday), the player's fishing level >= 3 and the bridge must be fixed on the beach
                if (!Game1.MasterPlayer.mailReceived.Contains(ModDataKeys.MAIL_FLAG_MURPHY_WAS_INTRODUCED) && Game1.MasterPlayer.FishingLevel >= config.minimumFishingLevel && beach.bridgeFixed && todayDayOfWeek == Game1.MasterPlayer.modData[ModDataKeys.MURPHY_DAY_TO_APPEAR])
                {
                    Monitor.Log($"Sending {Game1.MasterPlayer.Name} intro letter about Murphy!", LogLevel.Trace);
                    Game1.MasterPlayer.mailbox.Add(ModDataKeys.MAIL_FLAG_MURPHY_WAS_INTRODUCED);
                }

                // Must be a user set island date (default Satuday), met Murphy and Ginger Island's resort must be built
                IslandSouth resort = Game1.getLocationFromName("IslandSouth") as IslandSouth;
                if (!Game1.MasterPlayer.mailReceived.Contains(ModDataKeys.MAIL_FLAG_MURPHY_FOUND_GINGER_ISLAND) && Game1.MasterPlayer.mailReceived.Contains(ModDataKeys.MAIL_FLAG_MURPHY_WAS_INTRODUCED) && resort.resortRestored && todayDayOfWeek == Game1.MasterPlayer.modData[ModDataKeys.MURPHY_DAY_TO_APPEAR_ISLAND])
                {
                    Monitor.Log($"Sending {Game1.MasterPlayer.Name} Ginger Island letter about Murphy!", LogLevel.Trace);
                    Game1.MasterPlayer.mailbox.Add(ModDataKeys.MAIL_FLAG_MURPHY_FOUND_GINGER_ISLAND);
                }
            }

            // Reset ownership of boat, deckhands
            mainDeckhand = null;
            numberOfDeckhands = 0;

            // Set the reward chest
            Vector2 rewardChestPosition = new Vector2(-100, -100);
            Farm farm = Game1.getLocationFromName("Farm") as Farm;
            rewardChest = farm.objects.Values.FirstOrDefault(o => o.modData.ContainsKey(ModDataKeys.REWARD_CHEST_DATA_KEY)) as Chest;
            if (rewardChest is null)
            {
                Monitor.Log($"Creating reward chest {rewardChestPosition}", LogLevel.Trace);
                rewardChest = new Chest(true, rewardChestPosition) { Name = "Trawler Rewards" };
                rewardChest.modData.Add(ModDataKeys.REWARD_CHEST_DATA_KEY, "true");

                farm.setObject(rewardChestPosition, rewardChest);
            }

            // Create the trawler object for the beach
            if (todayDayOfWeek == Game1.MasterPlayer.modData[ModDataKeys.MURPHY_DAY_TO_APPEAR_ISLAND])
            {
                trawlerObject = new Trawler(island);
            }
            else
            {
                trawlerObject = new Trawler(beach);
            }

            // Create the TrawlerReward class
            _trawlerRewards.Value = new TrawlerRewards(rewardChest);

            // Add the hull location
            TrawlerHull hullLocation = new TrawlerHull(Path.Combine(assetManager.assetFolderPath, "Maps", "TrawlerHull.tmx"), ModDataKeys.TRAWLER_HULL_LOCATION_NAME) { IsOutdoors = false, IsFarm = false };
            Game1.locations.Add(hullLocation);

            // Add the surface location
            TrawlerSurface surfaceLocation = new TrawlerSurface(config.useOldTrawlerSprite ? Path.Combine(assetManager.assetFolderPath, "Maps", "Old", "FishingTrawler.tmx") : Path.Combine(assetManager.assetFolderPath, "Maps", "FishingTrawler.tmx"), ModDataKeys.TRAWLER_SURFACE_LOCATION_NAME) { IsOutdoors = true, IsFarm = false };
            Game1.locations.Add(surfaceLocation);

            // Add the cabin location
            TrawlerCabin cabinLocation = new TrawlerCabin(Path.Combine(assetManager.assetFolderPath, "Maps", "TrawlerCabin.tmx"), ModDataKeys.TRAWLER_CABIN_LOCATION_NAME) { IsOutdoors = false, IsFarm = false };
            Game1.locations.Add(cabinLocation);

            // Verify our locations were added and establish our location variables
            _trawlerHull.Value = Game1.getLocationFromName(ModDataKeys.TRAWLER_HULL_LOCATION_NAME) as TrawlerHull;
            _trawlerSurface.Value = Game1.getLocationFromName(ModDataKeys.TRAWLER_SURFACE_LOCATION_NAME) as TrawlerSurface;
            _trawlerCabin.Value = Game1.getLocationFromName(ModDataKeys.TRAWLER_CABIN_LOCATION_NAME) as TrawlerCabin;
        }

        private void OnDayEnding(object sender, DayEndingEventArgs e)
        {
            // Offload the custom locations
            Game1.locations.Remove(_trawlerHull.Value);
            Game1.locations.Remove(_trawlerSurface.Value);
            Game1.locations.Remove(_trawlerCabin.Value);
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
            if (Game1.MasterPlayer.mailReceived.Contains(ModDataKeys.MAIL_FLAG_MURPHY_WAS_INTRODUCED))
            {
                if (location is Beach && !Game1.isStartingToGetDarkOut(location) && todayDayOfWeek == Game1.MasterPlayer.modData[ModDataKeys.MURPHY_DAY_TO_APPEAR] && (!location.modData.ContainsKey(ModDataKeys.MURPHY_ON_TRIP) || location.modData[ModDataKeys.MURPHY_ON_TRIP] == "false"))
                {
                    return true;
                }
            }

            if (Game1.MasterPlayer.mailReceived.Contains(ModDataKeys.MAIL_FLAG_MURPHY_FOUND_GINGER_ISLAND))
            {
                if (location is IslandSouthEast && !Game1.isStartingToGetDarkOut(location) && todayDayOfWeek == Game1.MasterPlayer.modData[ModDataKeys.MURPHY_DAY_TO_APPEAR_ISLAND] && (!location.modData.ContainsKey(ModDataKeys.MURPHY_ON_TRIP) || location.modData[ModDataKeys.MURPHY_ON_TRIP] == "false"))
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
                murphyNPC = new Murphy(new AnimatedSprite(assetManager.murphyTexturePath, 0, 16, 32), new Vector2(12.05f, 39.5f) * 64f, 2, i18n.Get("etc.murphy_name"), assetManager.MurphyPortraitTexture);
                murphyNPC.Sprite.spriteTexture = assetManager.MurphyTexture;
            }
            else
            {
                murphyNPC = new Murphy(new AnimatedSprite(assetManager.murphyTexturePath, 0, 16, 32), new Vector2(89f, 38.5f) * 64f, 2, i18n.Get("etc.murphy_name"), assetManager.MurphyPortraitTexture);
                murphyNPC.Sprite.spriteTexture = assetManager.MurphyTexture;
            }
        }

        internal static bool IsMainDeckhand()
        {
            return mainDeckhand != null && mainDeckhand == Game1.MasterPlayer ? true : false;
        }

        internal static FlagType GetHoistedFlag()
        {
            Farmer flagOwner = mainDeckhand != null ? mainDeckhand : Game1.player;
            return Enum.TryParse(flagOwner.modData[ModDataKeys.HOISTED_FLAG_KEY], out FlagType flagType) ? flagType : FlagType.Unknown;
        }

        internal static void SetHoistedFlag(FlagType flagType)
        {
            Game1.player.modData[ModDataKeys.HOISTED_FLAG_KEY] = flagType.ToString();
        }

        internal static bool HasFarmerGoneSailing(Farmer who)
        {
            return who.modData.ContainsKey(ModDataKeys.MURPHY_SAILED_TODAY_KEY) && who.modData[ModDataKeys.MURPHY_SAILED_TODAY_KEY] == "true";
        }

        private void EstablishPlayerData()
        {
            if (!Game1.player.modData.ContainsKey(ModDataKeys.HOISTED_FLAG_KEY))
            {
                Game1.player.modData.Add(ModDataKeys.HOISTED_FLAG_KEY, FlagType.Unknown.ToString());
            }
            else
            {
                SetHoistedFlag(Enum.TryParse(Game1.player.modData[ModDataKeys.HOISTED_FLAG_KEY], out FlagType flagType) ? flagType : FlagType.Unknown);
            }

            Game1.player.modData[ModDataKeys.MURPHY_WAS_GREETED_TODAY_KEY] = "false";
            Game1.player.modData[ModDataKeys.MURPHY_DAY_TO_APPEAR] = config.dayOfWeekChoice;

            Game1.player.modData[ModDataKeys.MURPHY_DAY_TO_APPEAR_ISLAND] = config.dayOfWeekChoiceIsland;
            if (config.dayOfWeekChoice == config.dayOfWeekChoiceIsland)
            {
                // Prevent Murphy from showing up on the beach and island on same day by offsetting it by 1 (or setting it the first index)
                int currentIndex = Array.IndexOf(ModConfig.murphyDayToAppear, config.dayOfWeekChoiceIsland) + 1;
                if (currentIndex > ModConfig.murphyDayToAppear.Length - 1)
                {
                    currentIndex = 0;
                }

                Game1.player.modData[ModDataKeys.MURPHY_DAY_TO_APPEAR_ISLAND] = ModConfig.murphyDayToAppear[currentIndex];
            }

            if (!Game1.player.modData.ContainsKey(ModDataKeys.MURPHY_SAILED_TODAY_KEY))
            {
                Game1.player.modData.Add(ModDataKeys.MURPHY_SAILED_TODAY_KEY, "false");
                Game1.player.modData.Add(ModDataKeys.MURPHY_WAS_TRIP_SUCCESSFUL_KEY, "false");
                Game1.player.modData.Add(ModDataKeys.MURPHY_FINISHED_TALKING_KEY, "false");
            }
            else if (todayDayOfWeek == Game1.MasterPlayer.modData[ModDataKeys.MURPHY_DAY_TO_APPEAR] || todayDayOfWeek == Game1.MasterPlayer.modData[ModDataKeys.MURPHY_DAY_TO_APPEAR_ISLAND]) // This is intended, as we want MasterPlayer to determine Murphy appearance
            {
                Game1.player.modData[ModDataKeys.MURPHY_SAILED_TODAY_KEY] = "false";
                Game1.player.modData[ModDataKeys.MURPHY_WAS_TRIP_SUCCESSFUL_KEY] = "false";
                Game1.player.modData[ModDataKeys.MURPHY_FINISHED_TALKING_KEY] = "false";
            }

            // One time events, do not renew
            if (!Game1.player.modData.ContainsKey(ModDataKeys.MURPHY_HAS_SEEN_FLAG_KEY))
            {
                Game1.player.modData.Add(ModDataKeys.MURPHY_HAS_SEEN_FLAG_KEY, "false");
            }

            if (!Game1.player.modData.ContainsKey(ModDataKeys.MURPHY_TRIPS_COMPLETED))
            {
                Game1.player.modData[ModDataKeys.MURPHY_TRIPS_COMPLETED] = "0";
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
                    _trawlerHull.Value.RecalculateWaterLevel(quantity);
                    break;
                case SyncType.FishCaught:
                    result = true;
                    _trawlerSurface.Value.UpdateFishCaught(_trawlerHull.Value.GetFuelLevel(), quantity);
                    break;
                case SyncType.Fuel:
                    result = true;
                    _trawlerHull.Value.AdjustFuelLevel(quantity);
                    break;
                case SyncType.RestartGPS:
                    result = true;
                    _trawlerCabin.Value.RestartComputer();
                    break;
                case SyncType.TripTimer:
                    result = true;
                    eventManager.SetTripTimer(quantity);
                    break;
                case SyncType.GPSCooldown:
                    result = true;
                    _trawlerCabin.Value.SetCooldown(quantity);
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
            Game1.player.modData[ModDataKeys.MURPHY_TRIPS_COMPLETED] = (int.Parse(Game1.player.modData[ModDataKeys.MURPHY_TRIPS_COMPLETED]) + 1).ToString();
        }

        // Debug commands
        private void DebugGetAllFlags(string command, string[] args)
        {
            foreach (FlagType flagType in Enum.GetValues(typeof(FlagType)))
            {
                Game1.player.addItemByMenuIfNecessary(AncientFlag.CreateInstance(flagType));
            }
            Monitor.Log($"Giving all ancient flags to {Game1.player.Name}.", LogLevel.Debug);
        }

        private void DebugGetSpecialRewards(string command, string[] args)
        {
            Game1.player.addItemToInventory(AnglerRing.CreateInstance());
            Game1.player.addItemToInventory(LostFishingCharm.CreateInstance());
            foreach (TackleType tackleType in Enum.GetValues(typeof(TackleType)))
            {
                if (tackleType is TackleType.Unknown)
                {
                    continue;
                }
                Game1.player.addItemByMenuIfNecessary(SeaborneTackle.CreateInstance(tackleType));
            }
            Game1.player.addItemToInventory(Trident.CreateInstance());
            Monitor.Log($"Giving all special rewards to {Game1.player.Name}.", LogLevel.Debug);
        }

        private void DebugSkipRequirements(string command, string[] args)
        {
            Game1.player.mailReceived.Add(ModDataKeys.MAIL_FLAG_MURPHY_WAS_INTRODUCED);
            Monitor.Log($"Skipping requirements to meet Murphy for {Game1.player.Name}.", LogLevel.Debug);
        }
    }
}
