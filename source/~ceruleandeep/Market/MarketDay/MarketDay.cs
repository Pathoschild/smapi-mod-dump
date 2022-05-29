/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HarmonyLib;
using MailFrameworkMod;
using MarketDay.API;
using MarketDay.Data;
using MarketDay.ItemPriceAndStock;
using MarketDay.Shop;
using MarketDay.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using xTile.ObjectModel;
using static MarketDay.MarketDay;
using SObject = StardewValley.Object;

namespace MarketDay
{
    public class SalesRecord
    {
        public Item item;
        public int price;
        public NPC npc;
        public double mult;
        public int timeOfDay;
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public class MarketDay : Mod
    {
        // set true when NPC schedules have been re-planned 
        // following CP map patching
        private static bool MapChangesSynced;
        private static List<Vector2> previousShopLayout = new();
        private static List<GrangeShop> previousOpenedShops = new();
        
        // set true when hot reload is done 
        // following a GMCM change to market day
        private static bool ConfigChangesSynced = true;
        
        internal static ModConfig Config;
        internal static ProgressionModel Progression;
        internal static IModHelper helper;
        private static IMonitor monitor;
        internal static Mod SMod;

        internal static SpriteFont Font;
        internal static Texture2D BlankSign;
        
        public const string TotalGoldKey = "TotalGold";
        
        // ChroniclerCherry
        //The following variables are to help revert hardcoded warps done by the carpenter and
        //animal shop menus
        internal static GameLocation SourceLocation;
        private static Vector2 _playerPos = Vector2.Zero;
        internal static bool VerboseLogging = false;
        
        private IGenericModConfigMenuApi configMenu;
        private IContentPatcherAPI ContentPatcherApi;
        private static bool GMMJosephPresent;
        private static bool GMMPaisleyPresent;

        private static bool ForceLevelUpMail;
        
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="h">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper h)
        {
            helper = h;
            monitor = Monitor;
            SMod = this;

            Helper.Events.GameLoop.GameLaunched += OnLaunched;
            Helper.Events.GameLoop.GameLaunched += OnLaunched_STFRegistrations;
            Helper.Events.GameLoop.GameLaunched += OnLaunched_ReadProgressionData;
            Helper.Events.GameLoop.GameLaunched += OnLaunched_ReadFontData;
            Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded_STFInit;
            Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded_RehydrateMail;
            Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded_DestroyFurniture;
            // Helper.Events.GameLoop.DayStarted += OnDayStarted_UpdateStock;
            Helper.Events.GameLoop.DayStarted += OnDayStarted_MakePlayerShops;
            Helper.Events.GameLoop.DayStarted += OnDayStarted_FlagSyncNeeded;
            Helper.Events.GameLoop.DayStarted += OnDayStarted_SendPrompt;
            Helper.Events.Content.AssetReady += OnAssetReady_FlagSyncNeeded;
            Helper.Events.GameLoop.OneSecondUpdateTicking += OnOneSecondUpdateTicking_SyncMapUpdateStockOpenShop;
            Helper.Events.GameLoop.OneSecondUpdateTicking += OnOneSecondUpdateTicking_InteractWithNPCs;
            helper.Events.GameLoop.OneSecondUpdateTicked += Wizard.ConfigureFromWizard;

            Helper.Events.GameLoop.TimeChanged += OnTimeChanged_RestockThroughDay;
            Helper.Events.GameLoop.DayEnding += OnDayEnding_CloseShopsAndDestroyFurniture;
            Helper.Events.GameLoop.Saving += OnSaving_WriteConfig;
            Helper.Events.GameLoop.Saved += OnSaved_DoNothing;
            Helper.Events.Input.ButtonPressed += OnButtonPressed_ShowShopOrGrangeOrStats;

            var harmony = new Harmony("ceruleandeep.MarketDay");
            harmony.PatchAll();

            helper.ConsoleCommands.Add("md_furniture", "Remove stray furniture", RemoveStrayFurniture);
            helper.ConsoleCommands.Add("md_reload", "Reload shop data", HotReload);
            helper.ConsoleCommands.Add("md_tiles", "List shop tiles", ListShopTiles);
            helper.ConsoleCommands.Add("md_set_gold", "Set gold", SetGold);
            helper.ConsoleCommands.Add("md_levelup", "Trigger the level-up email", LevelUp);
        }

        // private static void Entry_InitSTF()
        // {
        //     ShopManager.LoadContentPacks();
        //     MakePlayerShop();
        // }

        private static void RemovePlayerShops()
        {
            foreach (var (shopKey, grangeShop) in ShopManager.GrangeShops)
            {
                if (grangeShop.IsPlayerShop()) ShopManager.GrangeShops.Remove(shopKey);
            }
        }
        
        private static void MakePlayerShops()
        {
            var farmers = Game1.getAllFarmers().Where(f => f.isActive()).ToList();
            var multiplayer = farmers.Count > 1;
            foreach (var farmer in farmers)
            {
                var signText = multiplayer 
                    ? Get("shop-sign", new {Owner=farmer.Name}) 
                    : Get("farm-sign", new {FarmName=Game1.player.farmName.Value});
                
                var PlayerShop = new GrangeShop()
                {
                    ShopName = $"Farmer:{farmer.Name}",
                    Quote = "Farmer store",
                    ItemStocks = Array.Empty<ItemStock>(),
                    PlayerID = farmer.UniqueMultiplayerID,
                    OpenSignText = signText
                };
                ShopManager.GrangeShops.Add(PlayerShop.ShopKey, PlayerShop);
                Log($"Added shop {PlayerShop.ShopKey} ({signText}) for {PlayerShop.ShopName} ({PlayerShop.PlayerID})", LogLevel.Trace);
            }
        }

        // void OnFieldChanged(IManifest mod, Action<string, object> onChange);

        private static void GMCMFieldChanged(string fieldID, object val)
        {
            Log($"GMCMFieldChanged: {fieldID} = {val}", LogLevel.Trace);
            if (fieldID.StartsWith("fm_")) ConfigChangesSynced = false;
        }

        private static void SyncAfterConfigChanges()
        {
            if (!Context.IsMainPlayer) return;
            if (!Context.IsWorldReady) return;
            Log($"SyncAfterConfigChanges: does nothing at the moment", LogLevel.Info, true);
            LogShopPositions("SyncAfterConfigChanges");
        }
        
        private static void HotReload(string command=null, string[] args=null)
        {
            if (!Context.IsMainPlayer) return;

            Log($"HotReload", LogLevel.Info);

            Log($"    Closing {MapUtility.ShopAtTile().Values.Count} stores", LogLevel.Debug);
            OnDayEnding_CloseShopsAndDestroyFurniture(null, null);

            // helper.ConsoleCommands.Trigger("patch", new[]{"reload", SMod.ModManifest.UniqueID+".CP"});
            
            // this bit is just for hot reload of data, not normal life cycle
            helper.WriteConfig(Config);
            OnLaunched_ReadProgressionData(null, null);
            
            Log($"    Loading content packs", LogLevel.Debug);
            OnSaveLoaded_STFInit(null, null);
            OnSaveLoaded_DestroyFurniture(null, null);
            // end: this bit is just for hot reload of data, not normal life cycle

            Log($"    Rebuilding player shops", LogLevel.Debug);
            OnDayStarted_MakePlayerShops(null, null);

            Log($"    Updating stock", LogLevel.Debug);
            ShopManager.UpdateStock();
            OnDayStarted_SendPrompt(null, null);

            Log($"    Opening stores", LogLevel.Debug);
            OnAssetReady_FlagSyncNeeded(null, null);
            
            // trick OnOneSecondUpdateTicking_SyncMapUpdateStockOpenShop into thinking the map has changed
            previousShopLayout = new List<Vector2>();
            
            LogShopPositions("HotReload: done");
            LogModState();
        }

        private static void LogModState()
        {
            try
            {
                var state = new List<string>
                {
                    $"{SMod.ModManifest.Name} {SMod.ModManifest.Version} state:",
                    $"Time  {Game1.currentSeason} {Game1.dayOfMonth} {Game1.timeOfDay} {Game1.ticks}"
                };
                var festival = StardewValley.Utility.isFestivalDay(Game1.dayOfMonth, Game1.currentSeason);
                state.Add($"Weather  Rain: {Game1.isRaining}  Snow: {Game1.isSnowing}  Festival: {festival}");
                state.Add($"Market Day: {IsMarketDay}");
                state.Add($"GMM Compat  Enabled: {Config.GMMCompat}  GMM Day: {isGMMDay()}  Joseph: {GMMJosephPresent}  Paisley: {GMMPaisleyPresent}");
                if (Config.Progression)
                {
                    state.Add($"Progression:  Enabled  Shops: {Progression.NumberOfShops}  Weekly Target: {Progression.WeeklyGoldTarget}");   
                    state.Add($"    AutoRestock: {Progression.AutoRestock}  ShopSize: {Progression.ShopSize}  PriceMultiplierLimit: {Progression.SellPriceMultiplierLimit}");
                    state.Add($"    Current level: [{Progression.CurrentLevel.Number}] {Progression.CurrentLevel.Name}");
                    if (Progression.NextLevel is not null)
                    {
                        state.Add($"    Next level: [{Progression.NextLevel.Number}] {Progression.NextLevel.Name} unlocks at {Progression.NextLevel.UnlockAtEarningsForDifficulty}");
                    }
                }
                else
                {
                    state.Add($"Progression:  Disabled");
                }

                var positions = string.Join(", ", ShopPositions());
                state.Add($"Shop positions: {Config.NumberOfShops} shops requested in config, {Progression.NumberOfShops} shops actual");
                state.Add($"    Positions sent to CP: {positions}");

                var mapPositions = string.Join(", ", MapUtility.ShopTiles);
                state.Add($"    Positions on map: {mapPositions}");

                state.AddRange(MapUtility.ShopAtTile().Select(kvp => $"    {kvp.Key}: {kvp.Value.ShopName}"));

                state.Add($"Shop config:");
                var enabled = Config.ShopsEnabled.Select(kvp => $"    {kvp.Key}: {kvp.Value}").ToList();
                if (enabled.Count == 0)
                    state.Add("    No shops enabled/disabled in config.json");
                state.AddRange(enabled);

                var apsk = string.Join(", ", AvailablePlayerShopKeys);
                state.Add($"Available player shops:  {AvailablePlayerShopKeys.Count} shops: {apsk}");

                var ansk = string.Join(", ", AvailableNPCShopKeys);
                state.Add($"Available NPC shops:  {AvailableNPCShopKeys.Count} shops: {ansk}");

                state.Add($"NPC interactions:");
                var times = Schedule.NPCInteractions.Keys.ToList();
                times.Sort();
                foreach (var time in times)
                {
                    var interactions = Schedule.NPCInteractions[time];
                    interactions.Sort();
                    state.Add($"    {time}:");
                    state.AddRange(interactions.Select(interaction => $"        {interaction}").Distinct());
                }
                
                foreach (var line in state) Log(line, Config.VerboseLogging ? LogLevel.Debug : LogLevel.Trace);
            }
            catch (Exception ex)
            {
                Log($"Unable to log mod state ({ex})", LogLevel.Trace);
            }
        }
        
        private static void LogShopPositions(string caller="unspecified")
        {
            try
            {
                var state = new List<string>
                {
                    $"{SMod.ModManifest.Name} {SMod.ModManifest.Version} shop positions  [caller: {caller}]"
                };
                var md = IsMarketDay ? "market day" : "not market day";
                state.Add($"Time  {Game1.currentSeason} {Game1.dayOfMonth} {Game1.timeOfDay} {Game1.ticks}  ({md})");

                state.Add($"Shops: {Progression.NumberOfShops}");
                if (ShopPositions() is not null)
                {
                    var positions = string.Join(", ", ShopPositions());
                    state.Add($"    Positions sent to CP: {positions}");
                }
                else
                {
                    state.Add($"    Positions sent to CP: null");
                }

                if (MapUtility.ShopTiles.Count > 0)
                {
                    var mapPositions = string.Join(", ", MapUtility.ShopTiles);
                    state.Add($"    Positions on map: {mapPositions}");
                }
                else
                {
                    state.Add($"    No shop positions on the map");
                }

                if (MapUtility.EmptyShopLocations().Count > 0)
                {
                    var mapPositions = string.Join(", ", MapUtility.EmptyShopLocations());
                    state.Add($"    Empty shop locations: {mapPositions}");
                }
                else
                {
                    state.Add($"    No empty shop locations");
                }

                if (previousShopLayout is not null && previousShopLayout.Count > 0)
                {
                    var lastMapPositions = string.Join(", ", previousShopLayout);
                    state.Add($"    Previous positions on map: {lastMapPositions}");
                }

                if (MapUtility.OpenShops().Count > 0)
                {
                    state.Add("Open shops:");
                    state.AddRange(MapUtility.OpenShops().Select(shop => $"    {shop.Origin}: {shop.ShopName}"));
                }
                else
                {
                    state.Add("No shops open");
                }

                foreach (var line in state) Log(line, Config.VerboseLogging ? LogLevel.Debug : LogLevel.Trace);
            }
            catch (Exception ex)
            {
                Log($"Could not log shop state ({ex})", LogLevel.Trace);
            }
        }

        private static void ListShopTiles(string command, string[] args)
        {
            Log($"ListShopTiles: {Game1.currentSeason} {Game1.dayOfMonth} {Game1.timeOfDay} {Game1.ticks}", LogLevel.Trace);
            var town = Game1.getLocationFromName("Town");
            if (town is null)
            {
                Log($"    Town location not available", LogLevel.Error);
                return;
            }
            
            var layerWidth = town.map.Layers[0].LayerWidth;
            var layerHeight = town.map.Layers[0].LayerHeight;

            Log($"    Map dimensions {layerWidth} {layerHeight}", LogLevel.Trace);

            // top left corner is z_MarketDay 253
            for (var x = 0; x < layerWidth; x++)
            {
                for (var y = 0; y < layerHeight; y++)
                {
                    var tileSheetIdAt = town.getTileSheetIDAt(x, y, "Buildings");
                    if (! tileSheetIdAt.StartsWith("z_MarketDay")) continue;
                    var tileIndexAt = town.getTileIndexAt(x, y, "Buildings");
                    if (tileIndexAt != 253) continue;

                    Log($"    {x} {y}: {tileSheetIdAt} {tileIndexAt}", LogLevel.Trace);
                }
            }
        }

        private static void SetGold(string command, string[] args)
        {
            if (args.Length == 2)
            {
                if (!MapUtility.ShopOwners.TryGetValue(args[0], out var grangeShop))
                {
                    var playerShops = string.Join(" ", MapUtility.ShopOwners.Values.Where(s => s.IsPlayerShop()).Select(s => s.Owner()));
                    Log($"Could not find shop for {args[0]}, should be one of [{playerShops}]", LogLevel.Error);
                    return;
                }
                grangeShop.SetSharedValue(GrangeShop.GoldTodayKey, int.Parse(args[1]));
                Log($"Shop {grangeShop.ShopName} gold today {grangeShop.GetSharedValue(GrangeShop.GoldTodayKey)}", LogLevel.Debug);
                return;
            }
            SetSharedValue(TotalGoldKey, int.Parse(args[0]));
            Log($"Total gold {GetSharedValue(TotalGoldKey)}", LogLevel.Debug);
        }
        
        private static void LevelUp(string command, string[] args)
        {
            if (args.Length == 1)
            {
                var idx = int.Parse(args[0]);
                if (Progression.Levels.Count <= idx)
                {
                    Log($"Could not find level {args[0]}, should be [0..{Progression.Levels.Count-1}]", LogLevel.Error);
                    return;
                }
                var level = Progression.Levels[int.Parse(args[0])];
                Log($"Setting level to {level.Name}", LogLevel.Debug);
                SetGold("", new[]{$"{level.UnlockAtEarningsForDifficulty}"});
            }
            Log($"Will send level-up mail tonight", LogLevel.Debug);
            ForceLevelUpMail = true;
            // ConfigChangesSynced = false; 
            // SyncAfterConfigChanges();
        }

        /// <summary>Raised after the game is saved</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaving_WriteConfig(object sender, SavingEventArgs e)
        {
            Log($"OnSaving: {Game1.currentSeason} {Game1.dayOfMonth} {Game1.timeOfDay} {Game1.ticks}", LogLevel.Trace);
            ListShopTiles("", null);
            Helper.WriteConfig(Config);
            Log($"OnSaving: complete at {Game1.currentSeason} {Game1.dayOfMonth} {Game1.timeOfDay} {Game1.ticks}", LogLevel.Trace);

        }

        private static void OnSaved_DoNothing(object sender, SavedEventArgs e)
        {
            Log($"OnSaved: {Game1.currentSeason} {Game1.dayOfMonth} {Game1.timeOfDay} {Game1.ticks}", LogLevel.Trace);
            ListShopTiles("", null);
            Log($"OnSaved: complete at {Game1.currentSeason} {Game1.dayOfMonth} {Game1.timeOfDay} {Game1.ticks}", LogLevel.Trace);
        }

        /// <summary>Raised after the player loads a save slot and the world is initialised.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private static void OnLaunched_ReadProgressionData(object sender, EventArgs e)
        {
            Progression = helper.Data.ReadJsonFile<ProgressionModel>("assets/progression.json");
            if (Progression is null)
            {
                Log("Could not read progression data", LogLevel.Error);
                Progression = new ProgressionModel();
            }
        }
        
        private static void OnLaunched_ReadFontData(object sender, EventArgs e)
        {
            try
            {
                Font = helper.ModContent.Load<SpriteFont>("assets\\IckleFont.xnb");
            }
            catch (Exception ex)
            {
                Log($"Could not load sign font: {ex}", LogLevel.Error);
            }
            
            try
            {
                BlankSign = helper.ModContent.Load<Texture2D>("Assets\\open-brown.png");
            }
            catch (Exception ex)
            {
                Log($"Could not load sign texture: {ex}", LogLevel.Error);
            }
        }

        private static void OnSaveLoaded_RehydrateMail(object sender, EventArgs e)
        {
            Mail.LoadMails();
        }

        private static void OnSaveLoaded_DestroyFurniture(object sender, EventArgs e)
        {
            Log($"OnSaveLoaded_DestroyFurniture: {Game1.currentSeason} {Game1.dayOfMonth} {Game1.timeOfDay} {Game1.ticks}", LogLevel.Trace);

            // get a clean slate in case previous debugging runs have left furniture lying around
            RemoveStrayFurniture();
            
            Log($"OnSaveLoaded_DestroyFurniture: complete at {Game1.currentSeason} {Game1.dayOfMonth} {Game1.timeOfDay} {Game1.ticks}", LogLevel.Trace);

        }

        private static void OnDayStarted_SendPrompt(object sender, EventArgs e)
        {
            Log($"OnDayStarted: {Game1.currentSeason} {Game1.dayOfMonth} {Game1.timeOfDay} {Game1.ticks}", LogLevel.Trace);
            if (!IsMarketDay) return;

            // send market day prompt
            var openingTime = (Config.OpeningTime*100).ToString();
            openingTime = openingTime[..^2] + ":" + openingTime[^2..];

            var ProfitTarget = StardewValley.Utility.getNumberWithCommas(Progression.WeeklyGoldTarget);
            var prompt = Config.Progression 
                ? Get("market-day-progression", new {ProfitTarget}) 
                : Get("market-day", new {openingTime});
            MessageUtility.SendMessage(prompt);
            
            Log($"OnDayStarted: complete at {Game1.currentSeason} {Game1.dayOfMonth} {Game1.timeOfDay} {Game1.ticks}", LogLevel.Trace);
        }

        private static void OnDayStarted_MakePlayerShops(object sender, EventArgs e)
        {
            RemovePlayerShops();
            MakePlayerShops();
        }
        
        private static void OnAssetReady_FlagSyncNeeded(object sender, AssetReadyEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            if (!Context.IsMainPlayer) return;
            
            if (e is not null && ! e.Name.Name.StartsWith("Maps/Town")) return;

            // layout will change next tick
            previousShopLayout = MapUtility.ShopTiles.Keys.ToList();
            previousOpenedShops = MapUtility.ShopAtTile().Values.ToList();

            LogShopPositions("OnAssetReady_FlagSyncNeeded");
            
            Log($"OnAssetReady: requesting sync", LogLevel.Info, true);
            MapChangesSynced = false;
        }
        
        private static void OnDayStarted_FlagSyncNeeded(object sender, EventArgs e)
        {
            LogShopPositions("OnDayStarted_FlagSyncNeeded");
            Log($"OnDayStarted_FlagSyncNeeded: requesting sync", LogLevel.Info, true);
            MapChangesSynced = false;
        }
        
        private static void OnOneSecondUpdateTicking_SyncMapUpdateStockOpenShop(object sender, OneSecondUpdateTickingEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            if (!Context.IsMainPlayer) return;
            if (MapChangesSynced) return;

            // the map is updated at this point so whatever's there is what there is
            MapChangesSynced = true;

            LogShopPositions("OnOneSecondUpdateTicking_SyncMap checking for invalid shops");
            var shopTiles = MapUtility.ShopTiles;
            foreach (var shop in MapUtility.OpenShops().Where(shop => !shopTiles.Keys.Contains(shop.Origin)))
            {
                Log($"Shop at {shop.Origin} is not on a shop tile", LogLevel.Trace);
                shop.CloseShop();
            }
            LogShopPositions("OnOneSecondUpdateTicking_SyncMap checked for invalid shops");

            var availableShopCount = AvailableNPCShopKeys.Count + AvailablePlayerShopKeys.Count;            
            var expectedShopCount = IsMarketDay ? ShopPositions().Length : 0;
            var expectedPlayerShopCount = IsMarketDay ? Math.Min(AvailablePlayerShopKeys.Count, expectedShopCount) : 0;
            var actualShopCount = MapUtility.OpenShops().Count;
            var actualPlayerShopCount = MapUtility.OpenPlayerShops().Count;
            
            if (actualShopCount == expectedShopCount && actualPlayerShopCount == expectedPlayerShopCount)
            {
                Log($"Correct number of shops already open ({expectedPlayerShopCount} player, {actualShopCount} total)", LogLevel.Trace);
                return;
            }

            Log($"Incorrect number of shops open. Player: {expectedPlayerShopCount} expected, {actualPlayerShopCount} actual, {AvailablePlayerShopKeys.Count} available. "
                +$"NPCs: {expectedShopCount} expected, {actualShopCount} actual, {availableShopCount} available", LogLevel.Trace);

            if (actualShopCount > 0)
            {
                Log($"SyncMapUpdateStockOpenShop: closing shops", LogLevel.Info, true);
                CloseShopsRemoveFurnitureSendMail(MapUtility.OpenShops());
                LogShopPositions("OnOneSecondUpdateTicking_SyncMap (closed open shops)");
            }

            if (!IsMarketDay) return;
            Log($"SyncMapUpdateStockOpenShop: syncing and opening shops", LogLevel.Info, true);

            if (!AnyShopTilesOnMap())
            {
                Log($"SyncMapUpdateStockOpenShop: it's market day but there are no shop locations", LogLevel.Debug);
                LogShopPositions("OnOneSecondUpdateTicking_SyncMap (no shop locs)");
                return;
            }

            // these are only assigned in OnAssetReady_FlagSyncNeeded
            // and the values are no longer needed
            previousShopLayout = null;
            previousOpenedShops = null;
            
            ShopManager.UpdateStock();
            OpenShops();
            RecalculateSchedules();

            OnDayStarted_SendPrompt(null, null);

            LogModState();
        }
        
        private static void OnOneSecondUpdateTicking_InteractWithNPCs(object sender, OneSecondUpdateTickingEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            if (!Context.IsMainPlayer) return;
            if (!IsMarketDay) return;

            foreach (var shop in MapUtility.ShopAtTile().Values) shop.InteractWithNearbyNPCs();
        }

        private static void OnTimeChanged_RestockThroughDay(object sender, EventArgs e)
        {
            if (Game1.timeOfDay % 100 > 0) return;
            foreach (var store in MapUtility.ShopAtTile().Values) store.RestockThroughDay(IsMarketDay);
        }

        private static bool AnyShopTilesOnMap()
        {
            Log($"MapReady: {Game1.currentSeason} {Game1.dayOfMonth} {Game1.timeOfDay} {Game1.ticks}", LogLevel.Trace);

            if (MapUtility.ShopTiles is null)
            {
                Log($"MapReady: MarketDay.ShopTiles is null, called too early", LogLevel.Debug);
                return false;
            }
            
            if (MapUtility.ShopTiles.Count == 0)
            {
                Log($"MapReady: MarketDay.ShopTiles.Count {MapUtility.ShopTiles.Count}, called too early", LogLevel.Debug);
                return false;
            }
            Log($"    MapReady: {Game1.ticks}", LogLevel.Trace);

            Log($"MapReady: true at {Game1.currentSeason} {Game1.dayOfMonth} {Game1.timeOfDay} {Game1.ticks}", LogLevel.Trace);
            return true;
        }
        
        private static void RecalculateSchedules()
        {
            Log($"RecalculateSchedules: begins at {Game1.currentSeason} {Game1.dayOfMonth} {Game1.timeOfDay} {Game1.ticks}", LogLevel.Trace);

            Schedule.NPCInteractions = new();

            foreach (var npc in StardewValley.Utility.getAllCharacters())
            {
                if (npc is null) continue;
                if (!npc.isVillager()) continue;
                npc.Schedule = npc.getSchedule(Game1.dayOfMonth);
            }

            Log($"RecalculateSchedules: completed at {Game1.currentSeason} {Game1.dayOfMonth} {Game1.timeOfDay} {Game1.ticks}", LogLevel.Trace);
        }

        private static void OpenShops()
        {
            if (!MapChangesSynced) throw new Exception("Map changes not synced");

            StardewValley.Utility.Shuffle(Game1.random, MapUtility.ShopTiles.Keys.ToList());
            
            var availableShopKeys = AvailableNPCShopKeys;
            StardewValley.Utility.Shuffle(Game1.random, availableShopKeys);
            availableShopKeys.InsertRange(0, AvailablePlayerShopKeys);
            
            var strNames = string.Join(", ", availableShopKeys);
            Log($"OpenShops: Adding shops ({MapUtility.ShopTiles.Count} of {strNames})", LogLevel.Trace);

            LogShopPositions("OpenShops");
            // pass 1 
            
            foreach (var (ShopLocation, RequestedShopKey) in MapUtility.EmptyShopLocations())
            {
                if (RequestedShopKey == "Random") continue;
                if (availableShopKeys.Count == 0) break;

                if (!availableShopKeys.Contains(RequestedShopKey))
                {
                    Log($"OpenShops: {RequestedShopKey} not in availableShopKeys", LogLevel.Trace);
                    continue;
                }

                if (!ShopManager.GrangeShops.ContainsKey(RequestedShopKey))
                {
                    Log($"OpenShops: {RequestedShopKey} not in GrangeShops", LogLevel.Trace);
                    continue;
                }

                availableShopKeys.Remove(RequestedShopKey);
                Log($"    {RequestedShopKey} at {ShopLocation}", LogLevel.Trace);
                ShopManager.GrangeShops[RequestedShopKey].OpenAt(ShopLocation);
                PruneBushes(ShopLocation);
            }
            LogShopPositions("OpenShops pass 1");

            // pass 2
            
            foreach (var (ShopLocation, RequestedShopKey) in MapUtility.EmptyShopLocations())
            {
                if (availableShopKeys.Count == 0) break;
                var ShopKey = availableShopKeys[0];
                availableShopKeys.RemoveAt(0);
                if (ShopManager.GrangeShops.ContainsKey(ShopKey))
                {
                    Log($"    {ShopKey} at {ShopLocation}", LogLevel.Trace);
                    ShopManager.GrangeShops[ShopKey].OpenAt(ShopLocation);
                    PruneBushes(ShopLocation);
                }
                else
                {
                    Log($"    {ShopKey} is not in shopManager.GrangeShops", LogLevel.Trace);
                }
            }
            
            LogShopPositions("OpenShops pass 2");

        }

        private static void PruneBushes(Vector2 ShopTile)
        {
            var town = Game1.getLocationFromName("Town");
            for (var dx = -2; dx <= 6; dx++)
            {
                for (var dy = -2; dy <= 6; dy++)
                {
                    var (x, y) = ShopTile + new Vector2(dx, dy);
                    if (town.getLargeTerrainFeatureAt((int)x, (int)y) is Bush bush)
                    {
                        town.largeTerrainFeatures.Remove(bush);
                    }
                }
            }
        }

        private static List<string> AvailableNPCShopKeys
        {
            get
            {
                var availableShopKeys = new List<string>();
                foreach (var (ShopKey, shop) in ShopManager.GrangeShops)
                {
                    if (shop.IsPlayerShop()) continue;
                    if (Config.ShopsEnabled.TryGetValue(ShopKey, out var enabled) && !enabled) continue;
                    if (shop.When is not null)
                    {
                        if (!APIs.Conditions.CheckConditions(shop.When)) continue;
                    }
                    availableShopKeys.Add(ShopKey);
                }
                return availableShopKeys;
            }
        }

        private static List<string> AvailablePlayerShopKeys
        {
            get
            {
                var availableShopKeys = new List<string>();
                foreach (var (ShopKey, shop) in ShopManager.GrangeShops)
                {
                    if (!shop.IsPlayerShop()) continue;
                    if (Config.ShopsEnabled.TryGetValue(ShopKey, out var enabled) && !enabled) continue;
                    if (shop.When is not null)
                    {
                        if (!APIs.Conditions.CheckConditions(shop.When)) continue;
                    }
                    availableShopKeys.Add(ShopKey);
                }
                return availableShopKeys;
            }
        }

        private static void OnDayEnding_CloseShopsAndDestroyFurniture(object sender, EventArgs e)
        {
            CloseShopsRemoveFurnitureSendMail(MapUtility.ShopAtTile().Values);
        }

        private static void CloseShopsRemoveFurnitureSendMail(IEnumerable<GrangeShop> shops)
        {
            Log($"OnDayEnding: {Game1.currentSeason} {Game1.dayOfMonth} {Game1.timeOfDay} {Game1.ticks}", LogLevel.Trace);
            if (!Context.IsMainPlayer) return;

            LogShopPositions("CloseShopsRemoveFurnitureSendMail");
            
            var levelBeforeClose = Progression.CurrentLevel;

            foreach (var store in shops) store.CloseShop();
            RemoveStrayFurniture();

            if (Progression.CurrentLevel != levelBeforeClose || ForceLevelUpMail)
            {
                var LevelStrapline = Progression.CurrentLevel.Name;
                var Name = Game1.player.farmName.Value;
                var mailKey = $"md_levelup_{Name}_{Game1.currentSeason}_{Game1.dayOfMonth}_Y{Game1.year}";
                var text = Get("level-up", new {Name, LevelStrapline});
                Log($"Sending level-up mail {mailKey}", LogLevel.Debug);
                Mail.Send(mailKey, text, null, 0, 2);
            }

            Log($"OnDayEnding: complete at {Game1.currentSeason} {Game1.dayOfMonth} {Game1.timeOfDay} {Game1.ticks}",
                LogLevel.Trace);
        }

        private static void RemoveStrayFurniture(string command=null, string[] args=null)
        {
            if (!Context.IsWorldReady) return;
            if (!Context.IsMainPlayer) return;

            Log($"DestroyAllFurniture", LogLevel.Trace);

            var toRemove = new Dictionary<Vector2, SObject>();
            var location = Game1.getLocationFromName("Town");

            foreach (var (tile, item) in location.Objects.Pairs)
            {
                if (item.modData.TryGetValue($"{SMod.ModManifest.UniqueID}/{GrangeShop.GrangeChestKey}", out var owner))
                {
                    Log($"    {owner} {GrangeShop.GrangeChestKey} at {item.TileLocation}", LogLevel.Trace);
                    toRemove[tile] = item;
                }

                if (item.modData.TryGetValue($"{SMod.ModManifest.UniqueID}/{GrangeShop.StockChestKey}", out owner))
                {
                    Log($"    {owner} {GrangeShop.StockChestKey} at {item.TileLocation}", LogLevel.Trace);
                    toRemove[tile] = item;
                }

                if (item.modData.TryGetValue($"{SMod.ModManifest.UniqueID}/{GrangeShop.ShopSignKey}", out owner))
                {
                    Log($"    {owner} {GrangeShop.ShopSignKey} at {item.TileLocation}", LogLevel.Trace);
                    toRemove[tile] = item;
                }
            }

            foreach (var (tile, item) in toRemove)
            {
                if (item is not Chest && item is not Sign) continue;
                Log($"    Removing {item} from {tile}", LogLevel.Trace);
                location.Objects.Remove(tile);
            }
        }


        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonPressed_ShowShopOrGrangeOrStats(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            if (Game1.activeClickableMenu is not null) return;
            
            if (Config.DebugKeybinds) CheckDebugKeybinds(e);
            
            string signOwner = null;
            
            if (Game1.currentLocation is null) return;
            if (Game1.currentLocation.objects.TryGetValue(e.Cursor.GrabTile, out var objectAt))
            {
                objectAt.modData.TryGetValue($"{ModManifest.UniqueID}/{GrangeShop.ShopSignKey}", out signOwner);
            }
            
            if (e.Button.IsUseToolButton() && objectAt is not null && signOwner is not null)
            {
                Log($"Tool use on {objectAt.Name} owned by {signOwner}", LogLevel.Trace);
                if (signOwner.StartsWith("Farmer:"))
                {
                    // clicked on player shop sign, show the summary
                    Helper.Input.Suppress(e.Button);
                    ShopManager.GrangeShops[signOwner].ShowSummary();
                    return;
                }
                    
                // clicked on NPC shop sign, open the store
                Helper.Input.Suppress(e.Button);
                ShopManager.GrangeShops[signOwner].DisplayShop();
                return;
            }

            if (!e.Button.IsActionButton()) return;
            
            var gtShop = MapUtility.ShopNearTile(e.Cursor.GrabTile);
            gtShop?.OnActionButton(e);
        }

        private void CheckDebugKeybinds(ButtonPressedEventArgs e)
        {
            if (e.Button == Config.ReloadKeybind)
            {
                HotReload("");
                Helper.Input.Suppress(e.Button);
            }

            string oldOutput = Game1.debugOutput;
            if (e.Button == Config.WarpKeybind)
            {
                Log("Warping", LogLevel.Debug);

                var debugCommand = Game1.player.currentLocation.Name == "Town"
                    ? "warp FarmHouse"
                    : "warp Town";
                Game1.game1.parseDebugInput(debugCommand);
                
                // show result
                Log(Game1.debugOutput != oldOutput
                    ? $"> {Game1.debugOutput}"
                    : $"Sent debug command '{debugCommand}' to the game, but there was no output.", LogLevel.Info);

                Helper.Input.Suppress(e.Button);
            }

            if (e.Button == Config.OpenConfigKeybind)
            {
                configMenu.OpenModMenu(ModManifest);
                Helper.Input.Suppress(e.Button);
            }

            if (e.Button == Config.StatusKeybind)
            {
                LogModState();
                Helper.Input.Suppress(e.Button);
            }
        }

        private void OnLaunched(object sender, GameLaunchedEventArgs e)
        {
            Log($"OnLaunched: {Game1.ticks}", LogLevel.Trace);

            Config = Helper.ReadConfig<ModConfig>();
            
            setupGMCM();
            ContentPatcherApi = Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");
            if (ContentPatcherApi is null)
            {
                Log("Content Patcher API not available. Market Day requires Content Patcher", LogLevel.Error);
                return;
            }
            ContentPatcherApi.RegisterToken(ModManifest, "IsMarketDay",
                () => { return Context.IsWorldReady ? new[] {IsMarketDay ? "true" : "false"} : null; });
            ContentPatcherApi.RegisterToken(ModManifest, "ShopPositions", ShopPositions);
            
            // APIs.RegisterAlmanac();
            // var weather = APIs.IsWeatherEnabled();
            // Log($"weather: {weather}", LogLevel.Info);
            
            GMMJosephPresent = this.Helper.ModRegistry.IsLoaded("Elaho.JosephsSeedShopDisplayCP");
            GMMPaisleyPresent = this.Helper.ModRegistry.IsLoaded("Elaho.PaisleysBridalBoutiqueCP");
            
            Log($"OnLaunched: complete at {Game1.ticks}", LogLevel.Trace);

        }

        /// <summary>
        /// On game launched initialize all the shops and register all external APIs
        ///
        /// From STF/ChroniclerCherry
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLaunched_STFRegistrations(object sender, GameLaunchedEventArgs e)
        {
            APIs.RegisterJsonAssets();
            if (APIs.JsonAssets != null)
                APIs.JsonAssets.AddedItemsToShop += JsonAssets_AddedItemsToShop;

            APIs.RegisterExpandedPreconditionsUtility();
            APIs.RegisterBFAV();
            APIs.RegisterFAVR();
        }

        private static void OnSaveLoaded_STFInit(object sender, SaveLoadedEventArgs e)
        {
            
            // some hooks for STF
            ShopManager.LoadContentPacks();
            
            Translations.UpdateSelectedLanguage();
            ShopManager.UpdateTranslations();

            ItemsUtil.UpdateObjectInfoSource();

            ShopManager.InitializeShops();
            ShopManager.InitializeItemStocks();

            ItemsUtil.RegisterItemsToRemove();
        }
        
        private static void JsonAssets_AddedItemsToShop(object sender, EventArgs e)
        {
            if (Game1.activeClickableMenu is ShopMenu shop)
            {
                shop.setItemPriceAndStock(ItemsUtil.RemoveSpecifiedJAPacks(shop.itemPriceAndStock));
            }
        }

        private void setupGMCM() {
            configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) return;

            configMenu.Unregister(ModManifest);
            configMenu.Register(ModManifest, () => Config = new ModConfig(), SaveConfig);
            configMenu.OnFieldChanged(ModManifest, GMCMFieldChanged);

            configMenu.SetTitleScreenOnlyForNextOptions(ModManifest, false);

            configMenu.AddSectionTitle(ModManifest,
                () => Helper.Translation.Get("cfg.game-mode"));
            configMenu.AddParagraph(ModManifest,
                () => Helper.Translation.Get("cfg.challenge-mode.msg"));
            
            configMenu.AddBoolOption(ModManifest,
                () => Config.Progression,
                val => Config.Progression = val,
                () => Helper.Translation.Get("cfg.challenge-mode"),
                () => Helper.Translation.Get("cfg.challenge-mode.msg"),
                fieldId: "fm_ChallengeMode"
            );
            
            configMenu.AddSectionTitle(ModManifest,
                () => Helper.Translation.Get("cfg.open-close-options"));
            configMenu.AddParagraph(ModManifest,
                () => Helper.Translation.Get("cfg.open-close-options.msg"));
            
            configMenu.AddNumberOption(ModManifest,
                () => Config.DayOfWeek,
                val => Config.DayOfWeek = val,
                () => Helper.Translation.Get("cfg.day-of-week"),
                () => Helper.Translation.Get("cfg.day-of-week.msg"),
                min: 0,
                max: 6,
                fieldId: "fm_DayOfWeek"
            );

            configMenu.AddBoolOption(ModManifest,
                () => Config.OpenInRain,
                val => Config.OpenInRain = val,
                () => Helper.Translation.Get("cfg.open-in-rain"),
                () => Helper.Translation.Get("cfg.open-in-rain.msg"),
                "fm_OpenInRain"
            );
            
            configMenu.AddBoolOption(ModManifest,
                () => Config.OpenInSnow,
                val => Config.OpenInSnow = val,
                () => Helper.Translation.Get("cfg.open-in-snow"),
                () => Helper.Translation.Get("cfg.open-in-snow.msg"),
                "fm_OpenInSnow"
            );
            
            configMenu.AddNumberOption(ModManifest,
                () => Config.OpeningTime,
                val => Config.OpeningTime = val,
                () => Helper.Translation.Get("cfg.opening-time"),
                () => Helper.Translation.Get("cfg.opening-time.msg"),
                min: 6,
                max: 26
            );
            
            configMenu.AddNumberOption(ModManifest,
                () => Config.ClosingTime,
                val => Config.ClosingTime = val,
                () => Helper.Translation.Get("cfg.closing-time"),
                () => Helper.Translation.Get("cfg.closing-time.msg"),
                min: 6,
                max: 26
            );
            
            configMenu.AddBoolOption(ModManifest,
                () => Config.AlwaysMarketDay,
                val => Config.AlwaysMarketDay = val,
                () => Helper.Translation.Get("cfg.always-market-day"),
                () => Helper.Translation.Get("cfg.always-market-day.msg"),
                fieldId: "fm_AlwaysMarketDay"
            );
            
            configMenu.AddBoolOption(ModManifest,
                () => Config.GMMCompat,
                val => Config.GMMCompat = val,
                () => Helper.Translation.Get("cfg.gmm-compat"),
                () => Helper.Translation.Get("cfg.gmm-compat.msg"),
                fieldId: "fm_GMMCompat"
            );

            configMenu.AddSectionTitle(ModManifest,
                () => Helper.Translation.Get("cfg.free-play-options"));
            configMenu.AddParagraph(ModManifest,
                () => Helper.Translation.Get("cfg.free-play-options.msg"));
            
            configMenu.AddNumberOption(ModManifest,
                () => Config.NumberOfShops,
                val => Config.NumberOfShops = val,
                () => Helper.Translation.Get("cfg.shop-layout"),
                () => Helper.Translation.Get("cfg.shop-layout.msg"),
                0,
                15,
                fieldId: "fm_ShopLayout"
            );

            configMenu.AddNumberOption(ModManifest,
                () => Config.RestockItemsPerHour,
                val => Config.RestockItemsPerHour = val,
                () => Helper.Translation.Get("cfg.restock-per-hour"),
                () => Helper.Translation.Get("cfg.restock-per-hour.msg"),
                0,
                9
            );
            
            
            configMenu.AddSectionTitle(ModManifest,
                () => Helper.Translation.Get("cfg.visitor-options"));
            configMenu.AddParagraph(ModManifest,
                () => Helper.Translation.Get("cfg.visitor-options.msg"));
            
            configMenu.AddBoolOption(ModManifest,
                () => Config.NPCVisitors,
                val => Config.NPCVisitors = val,
                () => Helper.Translation.Get("cfg.npc-visitors"),
                () => Helper.Translation.Get("cfg.npc-visitors.msg")
            );
            
            configMenu.AddNumberOption(ModManifest,
                () => Config.StallVisitChance,
                val => Config.StallVisitChance = val,
                () => Helper.Translation.Get("cfg.npc-stall-visit-chance"),
                () => Helper.Translation.Get("cfg.npc-stall-visit-chance"),
                min: 0f,
                max: 1f
            );
            
            configMenu.AddBoolOption(ModManifest,
                () => Config.NPCRescheduling,
                val => Config.NPCRescheduling = val,
                () => Helper.Translation.Get("cfg.npc-rescheduling"),
                () => Helper.Translation.Get("cfg.npc-rescheduling.msg")
            );



            configMenu.AddSectionTitle(ModManifest,
                () => Helper.Translation.Get("cfg.debug-settings"));

            configMenu.AddBoolOption(ModManifest,
                () => Config.PeekIntoChests,
                val => Config.PeekIntoChests = val,
                () => Helper.Translation.Get("cfg.peek-into-chests"),
                () => Helper.Translation.Get("cfg.peek-into-chests.msg")
            );

            configMenu.AddBoolOption(ModManifest,
                () => Config.RuinTheFurniture,
                val => Config.RuinTheFurniture = val,
                () => Helper.Translation.Get("cfg.ruin-furniture"),
                () => Helper.Translation.Get("cfg.ruin-furniture.msg")
            );

            configMenu.AddBoolOption(ModManifest,
                () => Config.VerboseLogging,
                val => Config.VerboseLogging = val,
                () => Helper.Translation.Get("cfg.verbose-logging"),
                () => Helper.Translation.Get("cfg.verbose-logging.msg")
            );
            
            configMenu.AddBoolOption(ModManifest,
                () => Config.ShowShopPositions,
                val => Config.ShowShopPositions = val,
                () => Helper.Translation.Get("cfg.shop-positions"),
                () => Helper.Translation.Get("cfg.shop-position.msg")

            );
            
            configMenu.AddBoolOption(ModManifest,
                () => Config.DebugKeybinds,
                val => Config.DebugKeybinds = val,
                () => Helper.Translation.Get("cfg.debug-keybinds"),
                () => Helper.Translation.Get("cfg.debug-keybinds.msg")
            );
            
            configMenu.AddKeybind(ModManifest,
                () => Config.OpenConfigKeybind,
                val => Config.OpenConfigKeybind = val,
                () => Helper.Translation.Get("cfg.open-config"),
                () => ""
            );
            
            configMenu.AddKeybind(ModManifest,
                () => Config.ReloadKeybind,
                val => Config.ReloadKeybind = val,
                () => Helper.Translation.Get("cfg.reload"),
                () => ""
            );
            
            configMenu.AddKeybind(ModManifest,
                () => Config.WarpKeybind,
                val => Config.WarpKeybind = val,
                () => Helper.Translation.Get("cfg.warp"),
                () => ""
            );
            
            // configMenu.AddKeybind(ModManifest,
            //     () => Config.StatusKeybind,
            //     val => Config.StatusKeybind = val,
            //     () => Helper.Translation.Get("cfg.status"),
            //     () => ""
            // );
        }

        public static void SaveConfig() {
            helper.WriteConfig(Config);
            if (! ConfigChangesSynced) SyncAfterConfigChanges();
        }

        internal static bool IsMarketDay
        {
            get
            {
                if (Config.AlwaysMarketDay) return true;
                var md = Game1.dayOfMonth % 7 == Config.DayOfWeek
                         && !StardewValley.Utility.isFestivalDay(Game1.dayOfMonth, Game1.currentSeason);
                md = md && (Config.OpenInRain || !Game1.isRaining);
                md = md && (Config.OpenInSnow || !Game1.isSnowing);
                return md;
            }
        }

        private static readonly Dictionary<string, List<int>> ShopLayouts = new()
        {
            ["0 Shops"] = new List<int>(),
            ["1 Shop"] = new List<int>  {1},
            ["2 Shops"] = new List<int> {1, 6},
            ["3 Shops"] = new List<int> {1, 7, 8},
            ["4 Shops"] = new List<int> {1, 5, 6, 8},
            ["5 Shops"] = new List<int> {1, 5, 6, 7, 8},
            ["6 Shops"] = new List<int> {1, 2, 6, 7, 8, 9},
            ["7 Shops"] = new List<int> {1, 2, 5, 6, 7, 8, 9},
            ["8 Shops"] = new List<int> {0, 1, 2, 5, 6, 7, 8, 9}, 
            ["9 Shops"] = new List<int> {0, 1, 2, 3, 5, 6, 7, 8, 9},
            ["10 Shops"] = new List<int> {0, 1, 2, 3, 4, 5, 6, 7, 8, 9},
            ["11 Shops"] = new List<int> {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10},
            ["12 Shops"] = new List<int> {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11},
            ["13 Shops"] = new List<int> {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12},
            ["14 Shops"] = new List<int> {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13},
            ["15 Shops"] = new List<int> {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14},
        };

        private static bool isGMMDay()
        {
            if (!StardewValley.Utility.doesAnyFarmerHaveMail("makersmarketletter")) return false;
            return Game1.dayOfMonth is 6 or 7 or 20 or 21;
        }

        private static List<int> ShopPositionsWithoutGMM()
        {
            if (!Context.IsWorldReady) return null;

            var shopCount = Progression.NumberOfShops;
            var key = $"{shopCount} Shops";
            if (!ShopLayouts.ContainsKey(key)) key = "6 Shops";
            if (!ShopLayouts.TryGetValue(key, out var layout)) return null;
            return layout;
        }
        
        private static string[] ShopPositions()
        {
            if (!Context.IsWorldReady) return null;
            var layout = ShopPositionsWithoutGMM();
            if (!Config.GMMCompat || !isGMMDay()) return layout.Select(i => $"Shop{i}").ToArray();
            
            if (GMMPaisleyPresent) layout.Remove(0);
            if (GMMPaisleyPresent) layout.Remove(5);
            if (GMMPaisleyPresent) layout.Remove(12);
            if (GMMPaisleyPresent) layout.Remove(13);
            if (GMMJosephPresent) layout.Remove(2);
            if (GMMJosephPresent) layout.Remove(7);
            return layout.Select(i => $"Shop{i}").ToArray();
        }

        internal static string Get(string key)
        {
            return helper.Translation.Get(key);
        }

        private static string Get(string key, object tokens)
        {
            return helper.Translation.Get(key, tokens);
        }

        internal static void Log(string message, LogLevel level, bool VerboseOnly = false)
        {
            if (VerboseOnly && Config is not null && !Config.VerboseLogging) return;
            monitor.Log(!Context.IsWorldReady ? $"{message}" : $"[{Game1.player.Name}] {message}", level);
        }

        internal static void IncrementSharedValue(string key, int amount = 1)
        {
            var val = GetSharedValue(key);
            val += amount;
            SetSharedValue(key, val);
        }

        internal static int GetSharedValue(string key)
        {
            var val = 0;
            var strVal = GetSharedString(key);
            if (strVal is not null) val = int.Parse(strVal);
            return val;
        }

        internal static string GetSharedString(string key)
        {
            Game1.getFarm().modData.TryGetValue($"{SMod.ModManifest.UniqueID}/{key}", out var strVal);
            return strVal;
        }

        internal static void SetSharedValue(string key, int val)
        {
            Game1.getFarm().modData[$"{SMod.ModManifest.UniqueID}/{key}"] = $"{val}";
        }

        internal static void SetSharedValue(string key, string val)
        {
            Game1.getFarm().modData[$"{SMod.ModManifest.UniqueID}/{key}"] = val;
        }

    }
}