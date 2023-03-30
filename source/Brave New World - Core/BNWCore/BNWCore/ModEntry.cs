/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiogoAlbano/StardewValleyMods
**
*************************************************/

using BNWCore.Patches;
using BNWCore.Automate;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System.Collections.Generic;
using System.Linq;
using Object = StardewValley.Object;
using Microsoft.Xna.Framework;
using BNWCore.Grabbers;
using System.Text;
using System;
using StardewValley.Menus;
using xTile.ObjectModel;
using StardewValley.TerrainFeatures;
using GenericModConfigMenu;

namespace BNWCore
{
    public class ModEntry : Mod
    {
        internal static IModHelper ModHelper;
        internal static IMonitor IMonitor;
        internal static IApi IApi;
        public static ModConfig Config;
        public static Texture2D ObjectsTexture;
        public static Texture2D ConstructionTexture;
        private AssetEditor AssetEditor;
        public static string ModDataKey => $"{ModHelper.ModRegistry.ModID}.MagicNets";
        public static int BNWCoreMagicNetId { get; private set; } = 735;
        public override void Entry(IModHelper helper)
        {
            if (Helper.ModRegistry.IsLoaded("DiogoAlbano.BNWCore") && Helper.ModRegistry.IsLoaded("Pathoschild.Automate"))
            {
                Helper.Events.GameLoop.GameLaunched += (s, e) =>
                {
                    var api = Helper.ModRegistry.GetApi<IAutomateApi>("Pathoschild.Automate");
                    if (api is not null)
                    {
                        api.AddFactory(new BNWCoreMagicNetFactory());
                        Monitor.Log($"Successfully registered fishnets to automate");
                    }
                    else Monitor.Log($"Failed to register fishnets to automate, api couldn't be accessed", LogLevel.Error);
                };
            }
            else Monitor.Log($"A requirement was not loaded, mod will not be loaded", LogLevel.Error);
            ModHelper = helper;
            IMonitor = Monitor;
            this.AssetEditor = new AssetEditor();
            Config = this.Helper.ReadConfig<ModConfig>();
            ObjectsTexture = ModHelper.ModContent.Load<Texture2D>("assets/objects.png");
            ConstructionTexture = ModHelper.ModContent.Load<Texture2D>("assets/construction.png");
            helper.ConsoleCommands.Add("getbnwmail", "Upgrade all tools to junimo", this.SendEmails);
            helper.Events.GameLoop.GameLaunched += (s, e) => Patcher.Patch(helper);
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.Content.AssetRequested += this.onAssetRequested;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.GameLoop.DayStarted += onDayStarted;
            helper.Events.GameLoop.DayEnding += onDayEnding;
            helper.Events.GameLoop.TimeChanged += OnTenMinuteUpdate;
            helper.Events.Display.RenderedWorld += OnRenderedWorld;
            helper.Events.World.ObjectListChanged += OnObjectListChanged;
            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            Game1.content.Load<Dictionary<int, string>>("Data\\ObjectInformation");
        }
        private void SendEmails(string command, string[] args)
        {
            Game1.player.mailbox.Add("earth_farming_blessing");
            Game1.player.mailbox.Add("nature_foraging_blessing");
            Game1.player.mailbox.Add("water_fishing_blessing");
            Game1.player.mailbox.Add("fire_mining_blessing");
            Game1.player.mailbox.Add("wind_combat_blessing");
        }
        private void GameLoop_DayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            var robin = Game1.getCharacterFromName("Robin");
            var demetrius = Game1.getCharacterFromName("Demetrius");
            var maru = Game1.getCharacterFromName("Maru");
            var sebastian = Game1.getCharacterFromName("Sebastian");
            if (robin is null)
            {
                return;
            }
            robin.shouldPlayRobinHammerAnimation.Value = false;
            robin.ignoreScheduleToday = false;
            robin.resetCurrentDialogue();
            robin.reloadDefaultLocation();
            demetrius.reloadDefaultLocation();
            maru.reloadDefaultLocation();
            sebastian.reloadDefaultLocation();
            Game1.warpCharacter(robin, robin.DefaultMap, robin.DefaultPosition / 64f);
            Game1.warpCharacter(demetrius, demetrius.DefaultMap, demetrius.DefaultPosition / 64f);
            Game1.warpCharacter(maru, maru.DefaultMap, maru.DefaultPosition / 64f);
            Game1.warpCharacter(sebastian, sebastian.DefaultMap, sebastian.DefaultPosition / 64f);
        }
        private string GetTodayScheduleString(NPC robin)
        {
            if (robin.isMarried())
            {
                if (robin.hasMasterScheduleEntry("marriage_" + Game1.currentSeason + "_" + Game1.dayOfMonth))
                {
                    return robin.getMasterScheduleEntry("marriage_" + Game1.currentSeason + "_" + Game1.dayOfMonth);
                }
                string day = Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);
                if (!Game1.isRaining && robin.hasMasterScheduleEntry("marriage_" + Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)))
                {
                    return robin.getMasterScheduleEntry("marriage_" + Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth));
                }
            }
            else
            {
                if (robin.hasMasterScheduleEntry(Game1.currentSeason + "_" + Game1.dayOfMonth))
                {
                    return robin.getMasterScheduleEntry(Game1.currentSeason + "_" + Game1.dayOfMonth);
                }
                int friendship = Utility.GetAllPlayerFriendshipLevel(robin);
                if (friendship >= 0)
                {
                    friendship /= 250;
                }
                while (friendship > 0)
                {
                    if (robin.hasMasterScheduleEntry(Game1.dayOfMonth.ToString() + "_" + friendship))
                    {
                        return robin.getMasterScheduleEntry(Game1.dayOfMonth.ToString() + "_" + friendship);
                    }
                    friendship--;
                }
                if (robin.hasMasterScheduleEntry(Game1.dayOfMonth.ToString()))
                {
                    return robin.getMasterScheduleEntry(Game1.dayOfMonth.ToString());
                }
                if (Game1.IsRainingHere(Game1.getLocationFromName(robin.DefaultMap)))
                {
                    if (Game1.random.NextDouble() < 0.5 && robin.hasMasterScheduleEntry("rain2"))
                    {
                        return robin.getMasterScheduleEntry("rain2");
                    }
                    if (robin.hasMasterScheduleEntry("rain"))
                    {
                        return robin.getMasterScheduleEntry("rain");
                    }
                }
                List<string> key = new List<string>
                {
                    Game1.currentSeason,
                    Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)
                };
                friendship = Utility.GetAllPlayerFriendshipLevel(robin);
                if (friendship >= 0)
                {
                    friendship /= 250;
                }
                while (friendship > 0)
                {
                    key.Add(friendship.ToString());
                    if (robin.hasMasterScheduleEntry(string.Join("_", key)))
                    {
                        return robin.getMasterScheduleEntry(string.Join("_", key));
                    }
                    friendship--;
                    key.RemoveAt(key.Count - 1);
                }
                if (robin.hasMasterScheduleEntry(string.Join("_", key)))
                {
                    return robin.getMasterScheduleEntry(string.Join("_", key));
                }
                if (robin.hasMasterScheduleEntry(Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)))
                {
                    return robin.getMasterScheduleEntry(Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth));
                }
                if (robin.hasMasterScheduleEntry(Game1.currentSeason))
                {
                    return robin.getMasterScheduleEntry(Game1.currentSeason);
                }
                if (robin.hasMasterScheduleEntry("spring_" + Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)))
                {
                    return robin.getMasterScheduleEntry("spring_" + Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth));
                }
                key.RemoveAt(key.Count - 1);
                key.Add("spring");
                friendship = Utility.GetAllPlayerFriendshipLevel(robin);
                if (friendship >= 0)
                {
                    friendship /= 250;
                }
                while (friendship > 0)
                {
                    key.Add(string.Empty + friendship.ToString());
                    if (robin.hasMasterScheduleEntry(string.Join("_", key)))
                    {
                        return robin.getMasterScheduleEntry(string.Join("_", key));
                    }
                    friendship--;
                    key.RemoveAt(key.Count - 1);
                }
                if (robin.hasMasterScheduleEntry("spring"))
                {
                    return robin.getMasterScheduleEntry("spring");
                }
            }
            return null;
        }
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(Config)
            );
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => ModHelper.Translation.Get("BNWCoreConfigMenuSectionTitleGrabber"),
                tooltip: () => ModHelper.Translation.Get("BNWCoreConfigMenuSectionTitleGrabbertooltip")
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => ModHelper.Translation.Get("BNWCoreslimeHutchbooloption"),
                tooltip: () => ModHelper.Translation.Get("BNWCoreslimeHutchbooloptiontooltip"),
                getValue: () => Config.BNWCoreslimeHutch,
                setValue: value => Config.BNWCoreslimeHutch = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => ModHelper.Translation.Get("BNWCoreharvestCropsbooloption"),
                tooltip: () => ModHelper.Translation.Get("BNWCoreharvestCropsbooloptiontooltip"),
                getValue: () => Config.BNWCoreharvestCrops,
                setValue: value => Config.BNWCoreharvestCrops = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => ModHelper.Translation.Get("BNWCoreharvestCropsIndoorPotsbooloption"),
                tooltip: () => ModHelper.Translation.Get("BNWCoreharvestCropsIndoorPotsbooloptiontooltip"),
                getValue: () => Config.BNWCoreharvestCropsIndoorPots,
                setValue: value => Config.BNWCoreharvestCropsIndoorPots = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => ModHelper.Translation.Get("BNWCoreartifactSpotsbooloption"),
                tooltip: () => ModHelper.Translation.Get("BNWCoreartifactSpotsbooloptiontooltip"),
                getValue: () => Config.BNWCoreartifactSpots,
                setValue: value => Config.BNWCoreartifactSpots = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => ModHelper.Translation.Get("BNWCoreorePanbooloption"),
                tooltip: () => ModHelper.Translation.Get("BNWCoreorePanbooloptiontooltip"),
                getValue: () => Config.BNWCoreorePan,
                setValue: value => Config.BNWCoreorePan = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => ModHelper.Translation.Get("BNWCorefruitTreesbooloption"),
                tooltip: () => ModHelper.Translation.Get("BNWCorefruitTreesbooloptiontooltip"),
                getValue: () => Config.BNWCorefruitTrees,
                setValue: value => Config.BNWCorefruitTrees = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => ModHelper.Translation.Get("BNWCoreseedTreesbooloption"),
                tooltip: () => ModHelper.Translation.Get("BNWCoreseedTreesbooloptiontooltip"),
                getValue: () => Config.BNWCoreseedTrees,
                setValue: value => Config.BNWCoreseedTrees = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => ModHelper.Translation.Get("BNWCoreflowersbooloption"),
                tooltip: () => ModHelper.Translation.Get("BNWCoreflowersbooloptiontooltip"),
                getValue: () => Config.BNWCoreflowers,
                setValue: value => Config.BNWCoreflowers = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => ModHelper.Translation.Get("BNWCoregarbageCansbooloption"),
                tooltip: () => ModHelper.Translation.Get("BNWCoregarbageCansbooloptiontooltip"),
                getValue: () => Config.BNWCoregarbageCans,
                setValue: value => Config.BNWCoregarbageCans = value
            );
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => ModHelper.Translation.Get("BNWCoreConfigMenuSectionTitleTools"),
                 tooltip: () => ModHelper.Translation.Get("BNWCoreConfigMenuSectionTitleTools")
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => ModHelper.Translation.Get("BNWCoreToolLengthintoption"),
                tooltip: () => ModHelper.Translation.Get("BNWCoreToolLengthintoptiontooltip"),
                getValue: () => Config.BNWCoreToolLength,
                setValue: value => Config.BNWCoreToolLength = value
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => ModHelper.Translation.Get("BNWCoreToolWidthintoption"),
                tooltip: () => ModHelper.Translation.Get("BNWCoreToolWidthintoptiontooltip"),
                getValue: () => Config.BNWCoreToolWidth,
                setValue: value => Config.BNWCoreToolWidth = value
            );
        }
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if(Game1.MasterPlayer.mailReceived.Contains("water_fishing_blessing"))
            {
                if ((e.Button == SButton.MouseLeft || e.Button == SButton.ControllerA) && Game1.player.CurrentItem.ParentSheetIndex == 738)
                {
                    const int radius = 2;
                    GameLocation location = Game1.currentLocation;
                    Point tile = Game1.player.getTileLocationPoint();
                    for (int y = tile.Y - radius; y < tile.Y + radius + 1; y++)
                    {
                        for (int x = tile.X - radius; x < tile.X + radius + 1; x++)
                        {
                            if (location.terrainFeatures.TryGetValue(new Vector2(x, y), out TerrainFeature feature) && feature is HoeDirt dirt)
                                dirt.state.Value = HoeDirt.watered;
                        }
                    }
                    Game1.player.reduceActiveItemByOne();
                }
            }
            
        }
        private void Input_ButtonPressed(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            if (!Context.CanPlayerMove)
                return;
            if (Constants.TargetPlatform == GamePlatform.Android)
            {
                if (e.Button != SButton.MouseLeft)
                    return;
                if (e.Cursor.GrabTile != e.Cursor.Tile)
                    return;
            }
            else if (!e.Button.IsActionButton())
                return;
            Vector2 clickedTile = Helper.Input.GetCursorPosition().GrabTile;
            IPropertyCollection tileProperty = TileUtility.GetTileProperty(Game1.currentLocation, "Buildings", clickedTile);
            if (tileProperty == null)
                return;
            CheckForShopToOpen(tileProperty, e);
        }
        private void CheckForShopToOpen(IPropertyCollection tileProperty, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            tileProperty.TryGetValue("Shop", out PropertyValue shopProperty);
            if (shopProperty != null)
            {
                IClickableMenu menu = TileUtility.CheckVanillaShop(shopProperty, out bool warpingShop);
                if (menu != null)
                {
                    ModEntry.ModHelper.Input.Suppress(e.Button);
                    Game1.activeClickableMenu = menu;

                }
            }
        }
        public override object GetApi() => IApi ??= new Api();
        public void LogDebug(string message)
        {
            Monitor.Log(message, LogLevel.Trace);
        }
        private void OnObjectListChanged(object sender, StardewModdingAPI.Events.ObjectListChangedEventArgs e)
        {
            LogDebug($"Object list changed at {e.Location.Name}");
            GrabAtLocation(e.Location);
        }
        private void OnTenMinuteUpdate(object sender, StardewModdingAPI.Events.TimeChangedEventArgs e)
        {
            if (e.NewTime % 100 == 0)
            {
                LogDebug($"Autograbbing on time change");
                foreach (var location in Game1.locations)
                {
                    var orePanGrabber = new OrePanGrabber(this, location);
                    if (orePanGrabber.CanGrab())
                    {
                        orePanGrabber.GrabItems();
                    }
                }
            }
        }
        private void onDayEnding(object sender, DayEndingEventArgs e)
        {
            if (!Context.IsMainPlayer) return;
            foreach (var l in Game1.locations)
            {
                if (l.Objects is null || l.Objects.Count() <= 0)
                {
                    if (l.modData.ContainsKey(ModDataKey))
                        l.modData.Remove(ModDataKey);
                    continue;
                }
                var magicNets = l.Objects.Values.Where(x => x is BNWCoreMagicNet);
                var serializable = new List<BNWCoreMagicNet.BNWCoreMagicNetSerializable>();
                foreach (var f in magicNets) 
                {
                    f.DayUpdate(l);
                    serializable.Add(new((BNWCoreMagicNet)f));
                }
                if (serializable is not null && serializable.Count > 0)
                {
                    string json = JsonConvert.SerializeObject(serializable);
                    l.modData[ModDataKey] = json;
                }
                else l.modData.Remove(ModDataKey);

                foreach (var f in magicNets)
                    l.Objects.Remove(f.TileLocation);
            }
        }
        private void onDayStarted(object sender, DayStartedEventArgs e)
        {
            if (!Context.IsMainPlayer) return;
            foreach (var l in Game1.locations)
            {
                if (!l.modData.ContainsKey(ModDataKey)) continue;
                string json = l.modData[ModDataKey];
                var deserialized = JsonConvert.DeserializeObject<List<BNWCoreMagicNet.BNWCoreMagicNetSerializable>>(json);           
                foreach (var f in deserialized)
                {
                    var magicNet = new BNWCoreMagicNet(f.Tile);
                    magicNet.owner.Value = f.Owner;
                    if (f.Bait >= 0)
                        magicNet.bait.Value = new Object(f.Bait, 1);
                    if (f.ObjectId >= 0)
                        magicNet.heldObject.Value = new Object(f.ObjectId, 1);
                    if (!l.Objects.ContainsKey(f.Tile))
                        l.Objects.Add(f.Tile, magicNet);
                    magicNet.DayUpdate(l);
                }
            }
            LogDebug($"Autograbbing on day start");
            var locations = Game1.locations.Concat(Game1.getFarm().buildings.Select(building => building.indoors.Value)).Where(location => location != null);
            foreach (var location in locations) GrabAtLocation(location);
        }
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!e.IsMultipleOf(8))
                return;
            Farmer farmer = Game1.player;
            Item item;
            try
            {
                item = farmer.Items[farmer.CurrentToolIndex];
            }
            catch (System.ArgumentOutOfRangeException)
            {
                return;
            }
            if (Game1.player.daysLeftForToolUpgrade.Value > 0)
            {
                if (Game1.player.toolBeingUpgraded.Value is StardewValley.Tools.GenericTool genericTool)
                {
                    genericTool.actionWhenClaimed();
                }
                else
                {
                    Game1.player.addItemToInventory(Game1.player.toolBeingUpgraded.Value);
                }
                Game1.player.toolBeingUpgraded.Value = null;
                Game1.player.daysLeftForToolUpgrade.Value = 0;
            }
        }
        private void onAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            this.AssetEditor.OnAssetRequested(e);
        }
        private void OnRenderedWorld(object sender, StardewModdingAPI.Events.RenderedWorldEventArgs e)
        {
            if (!(Context.IsPlayerFree && !Game1.eventUp && Game1.farmEvent == null && InternalConfig.harvestCropsRange > 0 && ModEntry.Config.BNWCoreharvestCrops)) return;

            if (ModEntry.Config.BNWCoreharvestCrops
                && InternalConfig.harvestCropsRange > 0
                && Game1.player.ActiveObject != null
                && Game1.player.ActiveObject.bigCraftable.Value
                && Game1.player.ActiveObject.ParentSheetIndex == ItemIds.Autograbber)
            {
                var placementTile = Game1.GetPlacementGrabTile();
                var X = (int)placementTile.X;
                var Y = (int)placementTile.Y;
                if (Game1.IsPerformingMousePlacement())
                {
                    var range = InternalConfig.harvestCropsRange;
                    for (int x = X - range; x <= X + range; x++)
                    {
                        for (int y = Y - range; y <= Y + range; y++)
                        {
                            if (InternalConfig.harvestCropsRangeMode == "Walk" && Math.Abs(X - x) + Math.Abs(Y - y) > range) continue;
                            Game1.spriteBatch.Draw(
                                        Game1.mouseCursors,
                                        Game1.GlobalToLocal(new Vector2(x, y) * 64),
                                        new Rectangle(194, 388, 16, 16),
                                        Color.White,
                                        0,
                                        Vector2.Zero,
                                        4,
                                        SpriteEffects.None,
                                        0.01f
                            );
                        }
                    }
                }
            }
        }
        private bool GrabAtLocation(GameLocation location)
        {
            var grabber = new AggregateDailyGrabber(this, location);
            var previousInventory = InternalConfig.reportYield ? grabber.GetInventory() : null;
            var grabbed = grabber.GrabItems();
            if (previousInventory != null)
            {
                var nextInventory = grabber.GetInventory();
                var sb = new StringBuilder($"Yield of autograbber(s) at {location.Name}:\n");
                var shouldPrint = false;
                foreach (var pair in nextInventory)
                {
                    var item = pair.Key;
                    var stack = pair.Value;
                    var diff = stack;
                    if (previousInventory.ContainsKey(pair.Key))
                    {
                        diff -= previousInventory[pair.Key];
                    }
                    if (diff <= 0) continue;
                    sb.AppendLine($"    {item.Name} ({item.QualityName}) x{diff}");
                    shouldPrint = true;
                }
                if (shouldPrint) Monitor.Log(sb.ToString(), LogLevel.Info);
            }
            return grabbed;
        }
    }
}
