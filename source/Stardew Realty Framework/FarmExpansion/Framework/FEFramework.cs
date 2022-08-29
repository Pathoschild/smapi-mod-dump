/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/FarmExpansion
**
*************************************************/

using FarmExpansion.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.TerrainFeatures;
using xTile;
using xTile.Layers;
using xTile.Tiles;
using Object = StardewValley.Object;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using System.Reflection;
using Harmony;

namespace FarmExpansion.Framework
{
    public class FEFramework
    {
        public event EventHandler BeforeRemoveEvent;
        public event EventHandler AfterAppendEvent;

        internal IModHelper helper;
        internal IMonitor monitor;

        /// <summary>The farm blueprints to add to every menu.</summary>
        internal readonly ICollection<BluePrint> FarmBlueprints = new List<BluePrint>();

        /// <summary>The expansion area blueprints to add to every menu.</summary>
        internal readonly ICollection<BluePrint> ExpansionBlueprints = new List<BluePrint>();

        private FarmExpansion farmExpansion;
        private Map map;
        private XmlSerializer locationSerializer = new XmlSerializer(typeof(FarmExpansion));
        private NPC robin;
        internal FEConfig config;

        internal bool IsTreeTransplantLoaded;
        internal Texture2D TreeTransplantFarmIcon;
        private IClickableMenu menuOverride;
        private bool overridableMenuActive;

        static FEFramework()
        {
            HarmonyInstance.Create("FarmExpansion.Framework.FEFramework").PatchAll(Assembly.GetExecutingAssembly());
        }

        public FEFramework(IModHelper helper, IMonitor monitor)
        {
            this.helper = helper;
            this.monitor = monitor;
            config = helper.ReadConfig<FEConfig>();
        }

        internal void AddFarmBluePrint(BluePrint blueprint)
        {
            this.FarmBlueprints.Add(blueprint);
        }

        internal void AddExpansionBluePrint(BluePrint blueprint)
        {
            this.ExpansionBlueprints.Add(blueprint);
        }

        /*internal void ControlEvents_KeyPress(object sender, EventArgsKeyPressed e)
        {
            if (!Context.IsWorldReady || Game1.activeClickableMenu != null)
                return;
            if (e.KeyPressed.ToString() == "V")
            {
                if (Game1.currentLocation.Name != "FarmExpansion")
                {
                    Game1.warpFarmer("FarmExpansion", 46, 4, false);
                }
                else
                {
                    Game1.warpFarmer("FarmHouse", 5, 8, false);
                }
            }
            if (e.KeyPressed.ToString().Equals("K"))
            {
                Game1.activeClickableMenu = new FECarpenterMenu(this);
            }
            if (e.KeyPressed.ToString().Equals("N"))
            {
                Game1.activeClickableMenu = new FEPurchaseAnimalsMenu(this);
            }
            if (e.KeyPressed.ToString().Equals("G"))
            {

            }
            if (e.KeyPressed.ToString().Equals("O"))
            {

            }
        }*/

        internal void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!overridableMenuActive)
                return;

            if (e.Button == SButton.MouseLeft)
                menuOverride.receiveLeftClick(Game1.getMouseX(), Game1.getMouseY(), false);
        }

        internal void OnCursorMoved(object sender, CursorMovedEventArgs e)
        {
            if (!overridableMenuActive)
                return;

            menuOverride.performHoverAction(Game1.getMouseX(), Game1.getMouseY());
        }

        /// <summary>When a menu is open (<see cref="Game1.activeClickableMenu"/> isn't null), raised before that menu is drawn to the screen.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        internal void RenderingActiveMenu(object sender, EventArgs e)
        {
            menuOverride.draw(Game1.spriteBatch);
        }

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        internal void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            switch (e.NewMenu)
            {
                // menu closed
                case null:
                    if (e.OldMenu is NamingMenu)
                    {
                        foreach (Building building in farmExpansion.buildings)
                            Game1.getFarm().buildings.Remove(building);
                    }

                    if (IsTreeTransplantLoaded)
                    {
                        if (e.OldMenu.GetType().FullName.Equals("TreeTransplant.TreeTransplantMenu"))
                        {
                            helper.Events.Input.ButtonPressed -= this.OnButtonPressed;
                            helper.Events.Input.CursorMoved -= this.OnCursorMoved;
                            helper.Events.Display.RenderingActiveMenu -= this.RenderingActiveMenu;
                            overridableMenuActive = false;
                            menuOverride = null;
                        }
                    }
                    break;

                // Add farm expansion point to world map
                case GameMenu _:
                    {
                        MapPage mp = null;

                        foreach (IClickableMenu page in this.helper.Reflection.GetField<List<IClickableMenu>>(Game1.activeClickableMenu, "pages").GetValue())
                        {
                            if (!(page is MapPage))
                                continue;
                            mp = page as MapPage;
                            break;
                        }
                        if (mp == null)
                            return;

                        int mapX = this.helper.Reflection.GetField<int>(mp, "mapX").GetValue();
                        int mapY = this.helper.Reflection.GetField<int>(mp, "mapY").GetValue();
                        Rectangle locationOnMap = new Rectangle(mapX + 156, mapY + 272, 100, 80);

                        mp.points.Add(new ClickableComponent(locationOnMap, "Farm Expansion"));

                        foreach (ClickableComponent cc in mp.points)
                        {
                            if (cc.myID != 1030)
                                continue;

                            cc.bounds.Width -= 64;
                            break;
                        }

                        if (Game1.currentLocation == farmExpansion)
                        {
                            // may result in wrong map position, getPlayerMapPosition is complex 600-line method in SDV1.3 instead of just a
                            // simple Vector2 field to be assigned. But at glance everything seems to work allright even without this hack.
                            //this.helper.Reflection.GetField<Vector2>(mp, "playerMapPosition").SetValue(new Vector2(mapX + 50 * Game1.pixelZoom, mapY + 75 * Game1.pixelZoom));
                            this.helper.Reflection.GetField<string>(mp, "playerLocationName").SetValue("Farm Expansion");
                        }
                        return;
                    }

                // Intercept carpenter menu
                case CarpenterMenu _:
                    if (!this.helper.Reflection.GetField<bool>(e.NewMenu, "magicalConstruction").GetValue())
                        Game1.activeClickableMenu = new FECarpenterMenu(this, this.FarmBlueprints.ToArray(), this.ExpansionBlueprints.ToArray()); // copy blueprint lists to avoid saving temporary blueprints
                    return;

                // Intercept purchase animals menu
                case PurchaseAnimalsMenu _:
                    Game1.activeClickableMenu = new FEPurchaseAnimalsMenu(this);
                    return;

                // Fixes infinite loop when animals hatch on farm expansion
                case NamingMenu _:
                    foreach (Building building in farmExpansion.buildings)
                    {
                        if (building.indoors.Value != null && building.indoors.Value == Game1.currentLocation)
                        {
                            foreach (var b in farmExpansion.buildings)
                                Game1.getFarm().buildings.Add(b);
                        }
                    }
                    return;

                default:
                    if (IsTreeTransplantLoaded && e.NewMenu.GetType().FullName.Equals("TreeTransplant.TreeTransplantMenu"))
                    {
                        if (TreeTransplantFarmIcon == null)
                        {
                            try
                            {
                                this.TreeTransplantFarmIcon = this.helper.Content.Load<Texture2D>(@"assets\TreeTransplantFarmIcon.png");
                            }
                            catch (Exception ex)
                            {
                                this.monitor.Log($"Could not load the menu icon for TreeTransplant compatibility.\n{ex}");
                                return;
                            }
                        }
                        menuOverride = new FETreeTransplantMenu(this, e.NewMenu);
                        overridableMenuActive = true;
                        helper.Events.Display.RenderingActiveMenu += this.RenderingActiveMenu;
                        helper.Events.Input.ButtonPressed += this.OnButtonPressed;
                        helper.Events.Input.CursorMoved += this.OnCursorMoved;
                    }

                    break;
            }
        }

        /// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        internal void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            try
            {
                map = helper.Content.Load<Map>(@"assets\FarmExpansion.tbin", ContentSource.ModFolder);
                map.LoadTileSheets(Game1.mapDisplayDevice);
            }
            catch (Exception ex)
            {
                //ControlEvents.KeyPressed -= this.ControlEvents_KeyPress;
                helper.Events.Display.MenuChanged -= this.OnMenuChanged;
                helper.Events.GameLoop.SaveLoaded -= this.OnSaveLoaded;
                helper.Events.GameLoop.Saving -= this.OnSaving;
                helper.Events.GameLoop.Saved -= this.OnSaved;
                helper.Events.GameLoop.ReturnedToTitle -= this.OnReturnedToTitle;
                helper.Events.GameLoop.DayStarted -= this.OnDayStarted;

                monitor.Log(ex.Message, LogLevel.Error);
                monitor.Log($"Unable to load map file 'FarmExpansion.tbin', unloading mod. Please try re-installing the mod.", LogLevel.Alert);
                return;
            }

            if (!File.Exists(Path.Combine(helper.DirectoryPath, "pslocationdata", $"{Constants.SaveFolderName}.xml")))
            {
                farmExpansion = new FarmExpansion(map, "FarmExpansion", this)
                {
                    IsFarm = true,
                    IsOutdoors = true
                };
                /*if (Game1.currentSeason.Equals("winter"))
                {
                    // Get rid of grass maybe... at some point (only a few lines of code but may have unintended consequences)
                }*/
            }
            else
            {
                Load();
            }

            for (int i = 0; i < farmExpansion.Map.TileSheets.Count; i++)
            {
                if (!farmExpansion.Map.TileSheets[i].ImageSource.Contains("path") && !farmExpansion.Map.TileSheets[i].ImageSource.Contains("object"))
                {
                    farmExpansion.Map.TileSheets[i].ImageSource = "Maps\\" + Game1.currentSeason + "_" + farmExpansion.Map.TileSheets[i].ImageSource.Split(new char[]
                    {
                                    '_'
                    })[1];
                    farmExpansion.Map.DisposeTileSheets(Game1.mapDisplayDevice);
                    farmExpansion.Map.LoadTileSheets(Game1.mapDisplayDevice);
                }
            }

            Game1.locations.Add(farmExpansion);
            AfterAppendEvent?.Invoke(farmExpansion, EventArgs.Empty);
            PatchMap();
            RepairBuildingWarps();
        }

        /// <summary>Raised before the game begins writes data to the save file (except the initial save creation).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        internal void OnSaving(object sender, SavingEventArgs e)
        {
            Save();
            BeforeRemoveEvent?.Invoke(farmExpansion, EventArgs.Empty);
            Game1.locations.Remove(farmExpansion);
        }

        /// <summary>Raised after the game finishes writing data to the save file (except the initial save creation).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        internal void OnSaved(object sender, SavedEventArgs e)
        {
            Game1.locations.Add(farmExpansion);
            AfterAppendEvent?.Invoke(farmExpansion, EventArgs.Empty);
        }

        /// <summary>Raised after the game returns to the title screen.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        internal void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            farmExpansion = null;
            map = null;
        }

        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        internal void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            if (Game1.isRaining)
            {
                foreach (var feature in farmExpansion.terrainFeatures.Values)
                {
                    if (feature is HoeDirt dirt)
                        dirt.state.Value = 1;
                }
            }

            foreach (Building current in farmExpansion.buildings)
                if (current.indoors.Value != null)
                    for (int k = current.indoors.Value.objects.Count() - 1; k >= 0; k--)
                        if (current.indoors.Value.objects[current.indoors.Value.objects.Keys.ElementAt(k)].minutesElapsed(3000 - Game1.timeOfDay, current.indoors.Value))
                            current.indoors.Value.objects.Remove(current.indoors.Value.objects.Keys.ElementAt(k));

            if (Game1.player.currentUpgrade != null)
                if (farmExpansion.objects.ContainsKey(new Vector2(Game1.player.currentUpgrade.positionOfCarpenter.X / Game1.tileSize, Game1.player.currentUpgrade.positionOfCarpenter.Y / Game1.tileSize)))
                    farmExpansion.objects.Remove(new Vector2(Game1.player.currentUpgrade.positionOfCarpenter.X / Game1.tileSize, Game1.player.currentUpgrade.positionOfCarpenter.Y / Game1.tileSize));

            RepairBuildingWarps();

            if (farmExpansion.isThereABuildingUnderConstruction() && !Utility.isFestivalDay(Game1.dayOfMonth, Game1.currentSeason))
            {
                bool flag2 = false;
                foreach (GameLocation location in Game1.locations)
                {
                    if (flag2)
                        break;

                    foreach (NPC npc in location.characters)
                    {
                        if (!npc.Name.Equals("Robin"))
                            continue;

                        robin = npc;
                        npc.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>
                        {
                            new FarmerSprite.AnimationFrame(24, 75),
                            new FarmerSprite.AnimationFrame(25, 75),
                            new FarmerSprite.AnimationFrame(26, 300, false, false, new AnimatedSprite.endOfAnimationBehavior(robinHammerSound), false),
                            new FarmerSprite.AnimationFrame(27, 1000, false, false, new AnimatedSprite.endOfAnimationBehavior(robinVariablePause), false)
                        });
                        npc.ignoreScheduleToday = true;
                        Building buildingUnderConstruction = farmExpansion.getBuildingUnderConstruction();
                        if (buildingUnderConstruction.daysUntilUpgrade.Value > 0)
                        {
                            if (!buildingUnderConstruction.indoors.Value.characters.Contains(npc))
                                buildingUnderConstruction.indoors.Value.addCharacter(npc);

                            if (npc.currentLocation != null)
                                npc.currentLocation.characters.Remove(npc);

                            npc.currentLocation = buildingUnderConstruction.indoors.Value;
                            npc.setTilePosition(1, 5);
                        }
                        else
                        {
                            Game1.warpCharacter(
                                npc,
                                "FarmExpansion",
                                new Vector2(
                                    buildingUnderConstruction.tileX.Value + buildingUnderConstruction.tilesWide.Value / 2,
                                    buildingUnderConstruction.tileY.Value + buildingUnderConstruction.tilesHigh.Value / 2
                                ));
                            npc.position.X = npc.position.X + Game1.tileSize / 4;
                            npc.position.Y = npc.position.Y - Game1.tileSize / 2;
                        }
                        npc.CurrentDialogue.Clear();
                        npc.CurrentDialogue.Push(new Dialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3926", new object[0]), npc));
                        flag2 = true;
                        break;
                    }
                }
            }
            else
            {
                farmExpansion.removeCarpenter();
            }
        }

        private void Save()
        {
            string path = Path.Combine(helper.DirectoryPath, "pslocationdata", $"{Constants.SaveFolderName}.xml");

            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            using (var writer = XmlWriter.Create(path))
            {
                locationSerializer.Serialize(writer, farmExpansion);
            }
            //monitor.Log($"Object serialized to {path}");
        }

        private void Load()
        {
            farmExpansion = new FarmExpansion(map, "FarmExpansion", this)
            {
                IsFarm = true,
                IsOutdoors = true
            };

            string path = Path.Combine(helper.DirectoryPath, "pslocationdata", $"{Constants.SaveFolderName}.xml");

            FarmExpansion loaded;
            using (var reader = XmlReader.Create(path))
            {
                loaded = (FarmExpansion)locationSerializer.Deserialize(reader);
            }
            //monitor.Log($"Object deserialized from {path}");

            farmExpansion.animals.CopyFrom(loaded.animals.Pairs);
            farmExpansion.buildings.ReplaceWith(loaded.buildings);
            farmExpansion.characters.ReplaceWith(loaded.characters);
            farmExpansion.terrainFeatures.ReplaceWith(loaded.terrainFeatures);
            farmExpansion.largeTerrainFeatures.ReplaceWith(loaded.largeTerrainFeatures);
            farmExpansion.resourceClumps.ReplaceWith(loaded.resourceClumps);
            farmExpansion.objects.ReplaceWith(loaded.objects);
            farmExpansion.numberOfSpawnedObjectsOnMap = loaded.numberOfSpawnedObjectsOnMap;
            farmExpansion.piecesOfHay.Value = loaded.piecesOfHay.Value;
            //farmExpansion.hasSeenGrandpaNote = loaded.hasSeenGrandpaNote;
            //farmExpansion.grandpaScore = loaded.grandpaScore;

            foreach (KeyValuePair<long, FarmAnimal> animal in farmExpansion.animals.Pairs)
                animal.Value.reload(null);

            foreach (Building building in farmExpansion.buildings)
            {
                building.load();
                if (building.indoors.Value != null && building.indoors.Value is AnimalHouse)
                {
                    foreach (KeyValuePair<long, FarmAnimal> animalsInBuilding in ((AnimalHouse)building.indoors.Value).animals.Pairs)
                    {
                        FarmAnimal animal = animalsInBuilding.Value;

                        foreach (Building current in farmExpansion.buildings)
                        {
                            if (current.tileX.Value == (int)animal.homeLocation.X && current.tileY.Value == (int)animal.homeLocation.Y)
                            {
                                animal.home = current;
                                break;
                            }
                        }
                    }
                }
            }
            for (int i = farmExpansion.characters.Count - 1; i >= 0; i--)
            {
                if (!farmExpansion.characters[i].DefaultPosition.Equals(Vector2.Zero))
                    farmExpansion.characters[i].position.Value = farmExpansion.characters[i].DefaultPosition;

                farmExpansion.characters[i].currentLocation = farmExpansion;

                if (i < farmExpansion.characters.Count)
                    farmExpansion.characters[i].reloadSprite();
            }

            foreach (KeyValuePair<Vector2, TerrainFeature> terrainFeature in farmExpansion.terrainFeatures.Pairs)
                terrainFeature.Value.loadSprite();

            foreach (KeyValuePair<Vector2, Object> current in farmExpansion.objects.Pairs)
            {
                current.Value.initializeLightSource(current.Key);
                current.Value.reloadSprite();
            }
            foreach (Building building in farmExpansion.buildings)
            {
                Vector2 tile = new Vector2((float)building.tileX.Value, (float)building.tileY.Value);

                if (building.indoors.Value is Shed)
                {
                    (building.indoors.Value as Shed).furniture.ReplaceWith((loaded.getBuildingAt(tile).indoors.Value as Shed).furniture);
                    (building.indoors.Value as Shed).wallPaper.Set((loaded.getBuildingAt(tile).indoors.Value as Shed).wallPaper);
                    (building.indoors.Value as Shed).floor.Set((loaded.getBuildingAt(tile).indoors.Value as Shed).floor);
                }
            }
        }

        private void RepairBuildingWarps()
        {
            foreach (Building building in farmExpansion.buildings)
            {
                if (building.indoors.Value != null)
                {
                    List<Warp> warps = new List<Warp>();
                    foreach (Warp warp in building.indoors.Value.warps)
                    {
                        warps.Add(new Warp(warp.X, warp.Y, "FarmExpansion", building.humanDoor.X + building.tileX.Value, building.humanDoor.Y + building.tileY.Value + 1, false));
                    }
                    building.indoors.Value.warps.Clear();
                    building.indoors.Value.warps.AddRange(warps);
                }
            }
        }

        private void PatchMap()
        {
            GameLocation gl = config.useBackwoodsEntrance ? Game1.getLocationFromName("Backwoods") : Game1.getFarm();
            string tsID = "";
            foreach (TileSheet ts in gl.map.TileSheets)
            {
                if (ts.ImageSource.Contains($"{Game1.currentSeason}_outdoorsTileSheet"))
                {
                    tsID = ts.Id;
                    break;
                }
            }
            if (string.IsNullOrEmpty(tsID))
            {
                this.monitor.Log($"Could not find appropriate tilesheet, farm map will not be patched", LogLevel.Error);
                return;
            }
            int warpYLocA, warpYLocB, warpYLocC;
            List<Tile> tiles = new List<Tile>();

            if (config.useBackwoodsEntrance)
            {
                tiles.Add(new Tile(TileLayer.Back, 10, 25, 200, tsID)); tiles.Add(new Tile(TileLayer.Back, 11, 25, 202, tsID)); tiles.Add(new Tile(TileLayer.Back, 0, 26, 537, tsID));
                tiles.Add(new Tile(TileLayer.Back, 1, 26, 537, tsID)); tiles.Add(new Tile(TileLayer.Back, 2, 26, 537, tsID)); tiles.Add(new Tile(TileLayer.Back, 3, 26, 537, tsID));
                tiles.Add(new Tile(TileLayer.Back, 4, 26, 537, tsID)); tiles.Add(new Tile(TileLayer.Back, 5, 26, 537, tsID)); tiles.Add(new Tile(TileLayer.Back, 6, 26, 537, tsID));
                tiles.Add(new Tile(TileLayer.Back, 7, 26, 537, tsID)); tiles.Add(new Tile(TileLayer.Back, 8, 26, 537, tsID)); tiles.Add(new Tile(TileLayer.Back, 9, 26, 537, tsID));
                tiles.Add(new Tile(TileLayer.Back, 10, 26, 561, tsID)); tiles.Add(new Tile(TileLayer.Back, 11, 26, 176, tsID)); tiles.Add(new Tile(TileLayer.Back, 12, 26, 176, tsID));
                tiles.Add(new Tile(TileLayer.Back, 0, 27, 562, tsID)); tiles.Add(new Tile(TileLayer.Back, 1, 27, 562, tsID)); tiles.Add(new Tile(TileLayer.Back, 2, 27, 562, tsID));
                tiles.Add(new Tile(TileLayer.Back, 3, 27, 562, tsID)); tiles.Add(new Tile(TileLayer.Back, 4, 27, 562, tsID)); tiles.Add(new Tile(TileLayer.Back, 5, 27, 562, tsID));
                tiles.Add(new Tile(TileLayer.Back, 6, 27, 562, tsID)); tiles.Add(new Tile(TileLayer.Back, 7, 27, 562, tsID)); tiles.Add(new Tile(TileLayer.Back, 8, 27, 562, tsID));
                tiles.Add(new Tile(TileLayer.Back, 9, 27, 562, tsID)); tiles.Add(new Tile(TileLayer.Back, 10, 27, 565, tsID)); tiles.Add(new Tile(TileLayer.Back, 11, 27, 176, tsID));
                tiles.Add(new Tile(TileLayer.Back, 12, 27, 176, tsID)); tiles.Add(new Tile(TileLayer.Back, 0, 28, 176, tsID)); tiles.Add(new Tile(TileLayer.Back, 1, 28, 176, tsID));
                tiles.Add(new Tile(TileLayer.Back, 2, 28, 176, tsID)); tiles.Add(new Tile(TileLayer.Back, 3, 28, 176, tsID)); tiles.Add(new Tile(TileLayer.Back, 4, 28, 176, tsID));
                tiles.Add(new Tile(TileLayer.Back, 5, 28, 176, tsID)); tiles.Add(new Tile(TileLayer.Back, 6, 28, 176, tsID)); tiles.Add(new Tile(TileLayer.Back, 7, 28, 176, tsID));
                tiles.Add(new Tile(TileLayer.Back, 8, 28, 176, tsID)); tiles.Add(new Tile(TileLayer.Back, 9, 28, 176, tsID)); tiles.Add(new Tile(TileLayer.Back, 10, 28, 176, tsID));
                tiles.Add(new Tile(TileLayer.Back, 11, 28, 176, tsID)); tiles.Add(new Tile(TileLayer.Back, 12, 28, 176, tsID)); tiles.Add(new Tile(TileLayer.Back, 0, 29, 176, tsID));
                tiles.Add(new Tile(TileLayer.Back, 1, 29, 176, tsID)); tiles.Add(new Tile(TileLayer.Back, 2, 29, 176, tsID)); tiles.Add(new Tile(TileLayer.Back, 3, 29, 176, tsID));
                tiles.Add(new Tile(TileLayer.Back, 4, 29, 176, tsID)); tiles.Add(new Tile(TileLayer.Back, 5, 29, 176, tsID)); tiles.Add(new Tile(TileLayer.Back, 6, 29, 176, tsID));
                tiles.Add(new Tile(TileLayer.Back, 7, 29, 176, tsID)); tiles.Add(new Tile(TileLayer.Back, 8, 29, 176, tsID)); tiles.Add(new Tile(TileLayer.Back, 9, 29, 176, tsID));
                tiles.Add(new Tile(TileLayer.Back, 10, 29, 176, tsID)); tiles.Add(new Tile(TileLayer.Back, 11, 29, 176, tsID)); tiles.Add(new Tile(TileLayer.Back, 12, 29, 176, tsID));
                tiles.Add(new Tile(TileLayer.Back, 0, 31, 351, tsID)); tiles.Add(new Tile(TileLayer.Back, 1, 31, 351, tsID)); tiles.Add(new Tile(TileLayer.Back, 2, 31, 351, tsID));
                tiles.Add(new Tile(TileLayer.Back, 3, 31, 351, tsID)); tiles.Add(new Tile(TileLayer.Back, 4, 31, 351, tsID)); tiles.Add(new Tile(TileLayer.Back, 5, 31, 351, tsID));
                tiles.Add(new Tile(TileLayer.Back, 6, 31, 351, tsID)); tiles.Add(new Tile(TileLayer.Back, 0, 32, 351, tsID)); tiles.Add(new Tile(TileLayer.Back, 1, 32, 351, tsID));
                tiles.Add(new Tile(TileLayer.Back, 2, 32, 351, tsID)); tiles.Add(new Tile(TileLayer.Back, 3, 32, 351, tsID)); tiles.Add(new Tile(TileLayer.Back, 4, 32, 351, tsID));

                tiles.Add(new Tile(TileLayer.Buildings, 10, 22, 444, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 0, 23, 467, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 1, 23, 468, tsID));
                tiles.Add(new Tile(TileLayer.Buildings, 2, 23, 467, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 3, 23, 468, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 4, 23, 467, tsID));
                tiles.Add(new Tile(TileLayer.Buildings, 5, 23, 468, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 6, 23, 467, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 7, 23, 468, tsID));
                tiles.Add(new Tile(TileLayer.Buildings, 8, 23, 467, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 9, 23, 468, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 10, 23, 469, tsID));
                tiles.Add(new Tile(TileLayer.Buildings, 0, 24, 492, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 1, 24, 493, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 2, 24, 492, tsID));
                tiles.Add(new Tile(TileLayer.Buildings, 3, 24, 493, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 4, 24, 492, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 5, 24, 493, tsID));
                tiles.Add(new Tile(TileLayer.Buildings, 6, 24, 492, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 7, 24, 493, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 8, 24, 492, tsID));
                tiles.Add(new Tile(TileLayer.Buildings, 9, 24, 493, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 10, 24, 494, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 0, 25, 517, tsID));
                tiles.Add(new Tile(TileLayer.Buildings, 1, 25, 518, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 2, 25, 517, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 3, 25, 518, tsID));
                tiles.Add(new Tile(TileLayer.Buildings, 4, 25, 517, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 5, 25, 518, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 6, 25, 517, tsID));
                tiles.Add(new Tile(TileLayer.Buildings, 7, 25, 518, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 8, 25, 517, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 9, 25, 518, tsID));
                tiles.Add(new Tile(TileLayer.Buildings, 10, 25, 519, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 0, 26, 542, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 1, 26, 543, tsID));
                tiles.Add(new Tile(TileLayer.Buildings, 2, 26, 542, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 3, 26, 543, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 4, 26, 542, tsID));
                tiles.Add(new Tile(TileLayer.Buildings, 5, 26, 543, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 6, 26, 542, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 7, 26, 543, tsID));
                tiles.Add(new Tile(TileLayer.Buildings, 8, 26, 542, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 9, 26, 543, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 10, 26, 544, tsID));
                tiles.Add(new Tile(TileLayer.Buildings, 0, 27, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 1, 27, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 2, 27, -1, tsID));
                tiles.Add(new Tile(TileLayer.Buildings, 3, 27, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 4, 27, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 5, 27, -1, tsID));
                tiles.Add(new Tile(TileLayer.Buildings, 6, 27, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 7, 27, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 8, 27, -1, tsID));
                tiles.Add(new Tile(TileLayer.Buildings, 9, 27, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 10, 27, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 0, 28, -1, tsID));
                tiles.Add(new Tile(TileLayer.Buildings, 1, 28, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 2, 28, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 3, 28, -1, tsID));
                tiles.Add(new Tile(TileLayer.Buildings, 4, 28, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 5, 28, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 6, 28, -1, tsID));
                tiles.Add(new Tile(TileLayer.Buildings, 7, 28, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 8, 28, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 9, 28, -1, tsID));
                tiles.Add(new Tile(TileLayer.Buildings, 10, 28, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 0, 29, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 1, 29, -1, tsID));
                tiles.Add(new Tile(TileLayer.Buildings, 2, 29, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 3, 29, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 4, 29, -1, tsID));
                tiles.Add(new Tile(TileLayer.Buildings, 5, 29, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 6, 29, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 7, 29, -1, tsID));
                tiles.Add(new Tile(TileLayer.Buildings, 8, 29, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 9, 29, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 10, 29, -1, tsID));
                tiles.Add(new Tile(TileLayer.Buildings, 12, 29, -1, tsID));

                tiles.Add(new Tile(TileLayer.Buildings, 0, 30, 326, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 1, 30, 326, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 2, 30, 326, tsID));
                tiles.Add(new Tile(TileLayer.Buildings, 3, 30, 326, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 4, 30, 326, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 5, 30, 326, tsID));
                tiles.Add(new Tile(TileLayer.Buildings, 6, 30, 326, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 7, 30, 326, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 8, 30, 326, tsID));
                tiles.Add(new Tile(TileLayer.Buildings, 9, 30, 327, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 0, 31, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 1, 31, -1, tsID));
                tiles.Add(new Tile(TileLayer.Buildings, 2, 31, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 3, 31, -1, tsID));

                tiles.Add(new Tile(TileLayer.Front, 1, 22, -1, tsID)); tiles.Add(new Tile(TileLayer.Front, 2, 22, -1, tsID)); tiles.Add(new Tile(TileLayer.Front, 3, 22, -1, tsID));
                tiles.Add(new Tile(TileLayer.Front, 1, 23, -1, tsID)); tiles.Add(new Tile(TileLayer.Front, 2, 23, -1, tsID)); tiles.Add(new Tile(TileLayer.Front, 3, 23, -1, tsID));
                tiles.Add(new Tile(TileLayer.Front, 0, 29, 414, tsID)); tiles.Add(new Tile(TileLayer.Front, 1, 29, 413, tsID)); tiles.Add(new Tile(TileLayer.Front, 2, 29, 414, tsID));
                tiles.Add(new Tile(TileLayer.Front, 3, 29, 413, tsID)); tiles.Add(new Tile(TileLayer.Front, 4, 29, 414, tsID)); tiles.Add(new Tile(TileLayer.Front, 5, 29, 413, tsID));
                tiles.Add(new Tile(TileLayer.Front, 6, 29, 414, tsID)); tiles.Add(new Tile(TileLayer.Front, 7, 29, 413, tsID)); tiles.Add(new Tile(TileLayer.Front, 8, 29, 414, tsID));
                tiles.Add(new Tile(TileLayer.Front, 9, 29, 413, tsID)); tiles.Add(new Tile(TileLayer.Front, 10, 29, 414, tsID)); tiles.Add(new Tile(TileLayer.Front, 11, 29, 413, tsID));
                tiles.Add(new Tile(TileLayer.Front, 12, 29, 438, tsID)); tiles.Add(new Tile(TileLayer.Front, 6, 30, 113, tsID)); tiles.Add(new Tile(TileLayer.Front, 7, 30, 114, tsID));

                tiles.Add(new Tile(TileLayer.AlwaysFront, 5, 19, -1, tsID)); tiles.Add(new Tile(TileLayer.AlwaysFront, 6, 19, -1, tsID)); tiles.Add(new Tile(TileLayer.AlwaysFront, 7, 19, -1, tsID));
                tiles.Add(new Tile(TileLayer.AlwaysFront, 5, 20, -1, tsID)); tiles.Add(new Tile(TileLayer.AlwaysFront, 6, 20, -1, tsID)); tiles.Add(new Tile(TileLayer.AlwaysFront, 7, 20, -1, tsID));
                tiles.Add(new Tile(TileLayer.AlwaysFront, 5, 21, -1, tsID)); tiles.Add(new Tile(TileLayer.AlwaysFront, 6, 21, -1, tsID)); tiles.Add(new Tile(TileLayer.AlwaysFront, 7, 21, -1, tsID));
                tiles.Add(new Tile(TileLayer.AlwaysFront, 5, 22, -1, tsID)); tiles.Add(new Tile(TileLayer.AlwaysFront, 6, 22, -1, tsID)); tiles.Add(new Tile(TileLayer.AlwaysFront, 7, 22, -1, tsID));
                tiles.Add(new Tile(TileLayer.AlwaysFront, 6, 29, 88, tsID)); tiles.Add(new Tile(TileLayer.AlwaysFront, 7, 29, 89, tsID)); tiles.Add(new Tile(TileLayer.AlwaysFront, 9, 29, 60, tsID));
                tiles.Add(new Tile(TileLayer.AlwaysFront, 10, 29, 61, tsID)); tiles.Add(new Tile(TileLayer.AlwaysFront, 11, 29, 62, tsID));

                performTileEdits(gl, tiles);

                gl.warps.Add(new Warp(-1, 27, "FarmExpansion", 46, 4, false));
                gl.warps.Add(new Warp(-1, 28, "FarmExpansion", 46, 5, false));
                gl.warps.Add(new Warp(-1, 29, "FarmExpansion", 46, 6, false));

                farmExpansion.warps.Add(new Warp(48, 4, "Backwoods", 0, 27, false));
                farmExpansion.warps.Add(new Warp(48, 5, "Backwoods", 0, 28, false));
                farmExpansion.warps.Add(new Warp(48, 6, "Backwoods", 0, 29, false));

                return;
            }

            switch (Game1.whichFarm)
            {
                case 1: // Fishing Farm
                    tiles.Add(new Tile(TileLayer.Back, 0, 55, 251, tsID)); tiles.Add(new Tile(TileLayer.Back, 1, 55, 251, tsID)); tiles.Add(new Tile(TileLayer.Back, 0, 56, 326, tsID));
                    tiles.Add(new Tile(TileLayer.Back, 1, 56, 326, tsID)); tiles.Add(new Tile(TileLayer.Back, 0, 57, 351, tsID)); tiles.Add(new Tile(TileLayer.Back, 1, 57, 351, tsID));

                    tiles.Add(new Tile(TileLayer.Buildings, 0, 55, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 1, 55, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 2, 55, -1, tsID));
                    tiles.Add(new Tile(TileLayer.Buildings, 0, 56, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 1, 56, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 2, 56, -1, tsID));
                    tiles.Add(new Tile(TileLayer.Buildings, 0, 57, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 1, 57, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 2, 57, -1, tsID));
                    tiles.Add(new Tile(TileLayer.Buildings, 0, 58, 175, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 1, 58, 175, tsID));

                    tiles.Add(new Tile(TileLayer.Front, 0, 54, -1, tsID)); tiles.Add(new Tile(TileLayer.Front, 1, 54, -1, tsID)); tiles.Add(new Tile(TileLayer.Front, 2, 54, -1, tsID));
                    tiles.Add(new Tile(TileLayer.Front, 0, 55, -1, tsID)); tiles.Add(new Tile(TileLayer.Front, 1, 55, -1, tsID)); tiles.Add(new Tile(TileLayer.Front, 0, 57, 413, tsID));
                    tiles.Add(new Tile(TileLayer.Front, 1, 57, 414, tsID)); tiles.Add(new Tile(TileLayer.Front, 2, 57, 438, tsID)); tiles.Add(new Tile(TileLayer.Front, 0, 58, 175, tsID));
                    tiles.Add(new Tile(TileLayer.Front, 1, 58, 175, tsID));

                    warpYLocA = 55; warpYLocB = 56; warpYLocC = 57;
                    break;
                case 2: // Foraging Farm
                    tiles.Add(new Tile(TileLayer.Back, 2, 28, 375, tsID)); tiles.Add(new Tile(TileLayer.Back, 3, 28, 376, tsID)); tiles.Add(new Tile(TileLayer.Back, 4, 28, 376, tsID));
                    tiles.Add(new Tile(TileLayer.Back, 5, 28, 377, tsID)); tiles.Add(new Tile(TileLayer.Back, 2, 29, 175, tsID)); tiles.Add(new Tile(TileLayer.Back, 3, 29, 175, tsID));
                    tiles.Add(new Tile(TileLayer.Back, 4, 29, 175, tsID)); tiles.Add(new Tile(TileLayer.Back, 5, 29, 175, tsID)); tiles.Add(new Tile(TileLayer.Back, 2, 30, 175, tsID));
                    tiles.Add(new Tile(TileLayer.Back, 3, 30, 175, tsID)); tiles.Add(new Tile(TileLayer.Back, 4, 30, 175, tsID)); tiles.Add(new Tile(TileLayer.Back, 5, 30, 175, tsID));

                    tiles.Add(new Tile(TileLayer.Buildings, 0, 28, 967, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 1, 28, 968, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 2, 28, 967, tsID));
                    tiles.Add(new Tile(TileLayer.Buildings, 3, 28, 968, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 0, 29, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 1, 29, -1, tsID));
                    tiles.Add(new Tile(TileLayer.Buildings, 2, 29, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 3, 29, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 0, 30, -1, tsID));
                    tiles.Add(new Tile(TileLayer.Buildings, 1, 30, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 2, 30, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 3, 30, -1, tsID));
                    tiles.Add(new Tile(TileLayer.Buildings, 4, 30, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 0, 31, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 1, 31, -1, tsID));
                    tiles.Add(new Tile(TileLayer.Buildings, 2, 31, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 3, 31, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 4, 31, -1, tsID));

                    tiles.Add(new Tile(TileLayer.Front, 0, 31, 1042, tsID)); tiles.Add(new Tile(TileLayer.Front, 1, 31, 1043, tsID)); tiles.Add(new Tile(TileLayer.Front, 2, 31, 1042, tsID));
                    tiles.Add(new Tile(TileLayer.Front, 3, 31, 1042, tsID)); tiles.Add(new Tile(TileLayer.Front, 4, 31, 1043, tsID)); tiles.Add(new Tile(TileLayer.Front, 5, 31, 1017, tsID));

                    tiles.Add(new Tile(TileLayer.AlwaysFront, 0, 26, 940, tsID)); tiles.Add(new Tile(TileLayer.AlwaysFront, 0, 27, 941, tsID)); tiles.Add(new Tile(TileLayer.AlwaysFront, 1, 27, 942, tsID));
                    tiles.Add(new Tile(TileLayer.AlwaysFront, 0, 28, 967, tsID)); tiles.Add(new Tile(TileLayer.AlwaysFront, 1, 28, 968, tsID)); tiles.Add(new Tile(TileLayer.AlwaysFront, 2, 28, 967, tsID));
                    tiles.Add(new Tile(TileLayer.AlwaysFront, 3, 28, 968, tsID)); tiles.Add(new Tile(TileLayer.AlwaysFront, 4, 28, 992, tsID)); tiles.Add(new Tile(TileLayer.AlwaysFront, 0, 29, -1, tsID));
                    tiles.Add(new Tile(TileLayer.AlwaysFront, 1, 29, -1, tsID)); tiles.Add(new Tile(TileLayer.AlwaysFront, 2, 29, -1, tsID)); tiles.Add(new Tile(TileLayer.AlwaysFront, 3, 29, -1, tsID));
                    tiles.Add(new Tile(TileLayer.AlwaysFront, 4, 29, -1, tsID)); tiles.Add(new Tile(TileLayer.AlwaysFront, 5, 29, -1, tsID)); tiles.Add(new Tile(TileLayer.AlwaysFront, 0, 30, -1, tsID));
                    tiles.Add(new Tile(TileLayer.AlwaysFront, 1, 30, -1, tsID)); tiles.Add(new Tile(TileLayer.AlwaysFront, 2, 30, -1, tsID)); tiles.Add(new Tile(TileLayer.AlwaysFront, 3, 30, -1, tsID));
                    tiles.Add(new Tile(TileLayer.AlwaysFront, 4, 30, -1, tsID)); tiles.Add(new Tile(TileLayer.AlwaysFront, 5, 30, -1, tsID)); tiles.Add(new Tile(TileLayer.AlwaysFront, 0, 31, -1, tsID));
                    tiles.Add(new Tile(TileLayer.AlwaysFront, 1, 31, -1, tsID)); tiles.Add(new Tile(TileLayer.AlwaysFront, 2, 31, -1, tsID)); tiles.Add(new Tile(TileLayer.AlwaysFront, 3, 31, -1, tsID));
                    tiles.Add(new Tile(TileLayer.AlwaysFront, 4, 31, -1, tsID)); tiles.Add(new Tile(TileLayer.AlwaysFront, 5, 31, -1, tsID)); tiles.Add(new Tile(TileLayer.AlwaysFront, 0, 32, 1068, tsID));
                    tiles.Add(new Tile(TileLayer.AlwaysFront, 1, 32, 1067, tsID)); tiles.Add(new Tile(TileLayer.AlwaysFront, 2, 32, 1068, tsID)); tiles.Add(new Tile(TileLayer.AlwaysFront, 3, 32, 1067, tsID));
                    tiles.Add(new Tile(TileLayer.AlwaysFront, 0, 33, 1070, tsID)); tiles.Add(new Tile(TileLayer.AlwaysFront, 1, 33, 1070, tsID)); tiles.Add(new Tile(TileLayer.AlwaysFront, 2, 33, 1065, tsID));
                    tiles.Add(new Tile(TileLayer.AlwaysFront, 3, 33, 1065, tsID)); tiles.Add(new Tile(TileLayer.AlwaysFront, 0, 34, 971, tsID)); tiles.Add(new Tile(TileLayer.AlwaysFront, 1, 34, 996, tsID));

                    warpYLocA = 29; warpYLocB = 30; warpYLocC = 31;
                    break;
                case 3: // Mining Farm
                    tiles.Add(new Tile(TileLayer.Back, 0, 50, 537, tsID)); tiles.Add(new Tile(TileLayer.Back, 1, 50, 537, tsID)); tiles.Add(new Tile(TileLayer.Back, 0, 51, 562, tsID));
                    tiles.Add(new Tile(TileLayer.Back, 1, 51, 562, tsID)); tiles.Add(new Tile(TileLayer.Back, 2, 51, 562, tsID)); tiles.Add(new Tile(TileLayer.Back, 0, 52, 587, tsID));
                    tiles.Add(new Tile(TileLayer.Back, 1, 52, 587, tsID)); tiles.Add(new Tile(TileLayer.Back, 0, 53, 587, tsID)); tiles.Add(new Tile(TileLayer.Back, 1, 53, 587, tsID));
                    tiles.Add(new Tile(TileLayer.Back, 0, 55, 326, tsID));

                    tiles.Add(new Tile(TileLayer.Buildings, 0, 47, 467, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 1, 47, 468, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 2, 47, 467, tsID));
                    tiles.Add(new Tile(TileLayer.Buildings, 0, 48, 493, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 1, 48, 492, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 2, 48, 493, tsID));
                    tiles.Add(new Tile(TileLayer.Buildings, 0, 49, 518, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 1, 49, 517, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 2, 49, 518, tsID));
                    tiles.Add(new Tile(TileLayer.Buildings, 0, 50, 543, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 1, 50, 542, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 2, 50, 543, tsID));
                    tiles.Add(new Tile(TileLayer.Buildings, 0, 51, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 1, 51, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 2, 51, -1, tsID));
                    tiles.Add(new Tile(TileLayer.Buildings, 0, 52, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 1, 52, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 2, 52, -1, tsID));
                    tiles.Add(new Tile(TileLayer.Buildings, 0, 53, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 1, 53, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 2, 53, -1, tsID));
                    tiles.Add(new Tile(TileLayer.Buildings, 0, 54, 175, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 1, 54, 175, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 1, 55, 352, tsID));

                    tiles.Add(new Tile(TileLayer.Front, 0, 48, -1, tsID)); tiles.Add(new Tile(TileLayer.Front, 1, 48, -1, tsID)); tiles.Add(new Tile(TileLayer.Front, 0, 49, -1, tsID));
                    tiles.Add(new Tile(TileLayer.Front, 1, 49, -1, tsID)); tiles.Add(new Tile(TileLayer.Front, 2, 49, -1, tsID)); tiles.Add(new Tile(TileLayer.Front, 0, 50, -1, tsID));
                    tiles.Add(new Tile(TileLayer.Front, 1, 50, -1, tsID)); tiles.Add(new Tile(TileLayer.Front, 2, 50, -1, tsID)); tiles.Add(new Tile(TileLayer.Front, 0, 51, -1, tsID));
                    tiles.Add(new Tile(TileLayer.Front, 1, 51, -1, tsID)); tiles.Add(new Tile(TileLayer.Front, 2, 51, -1, tsID)); tiles.Add(new Tile(TileLayer.Front, 0, 52, -1, tsID));
                    tiles.Add(new Tile(TileLayer.Front, 0, 53, 414, tsID)); tiles.Add(new Tile(TileLayer.Front, 1, 53, 413, tsID)); tiles.Add(new Tile(TileLayer.Front, 2, 53, 438, tsID));
                    tiles.Add(new Tile(TileLayer.Front, 0, 54, 175, tsID)); tiles.Add(new Tile(TileLayer.Front, 1, 54, 175, tsID)); tiles.Add(new Tile(TileLayer.Front, 2, 54, 419, tsID));

                    warpYLocA = 51; warpYLocB = 52; warpYLocC = 53;
                    break;
                case 4: // Combat Farm
                    tiles.Add(new Tile(TileLayer.Back, 2, 33, 346, tsID)); tiles.Add(new Tile(TileLayer.Back, 2, 34, 346, tsID)); tiles.Add(new Tile(TileLayer.Back, 2, 35, 346, tsID));
                    tiles.Add(new Tile(TileLayer.Back, 0, 38, 537, tsID)); tiles.Add(new Tile(TileLayer.Back, 1, 38, 537, tsID)); tiles.Add(new Tile(TileLayer.Back, 2, 38, 618, tsID));
                    tiles.Add(new Tile(TileLayer.Back, 0, 39, 587, tsID)); tiles.Add(new Tile(TileLayer.Back, 1, 39, 587, tsID)); tiles.Add(new Tile(TileLayer.Back, 0, 41, 587, tsID));
                    tiles.Add(new Tile(TileLayer.Back, 1, 41, 587, tsID)); tiles.Add(new Tile(TileLayer.Back, 2, 41, 587, tsID));

                    tiles.Add(new Tile(TileLayer.Buildings, 1, 33, 377, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 0, 34, 175, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 1, 34, 175, tsID));
                    tiles.Add(new Tile(TileLayer.Buildings, 2, 34, 444, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 0, 35, 467, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 1, 35, 468, tsID));
                    tiles.Add(new Tile(TileLayer.Buildings, 2, 35, 469, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 0, 36, 492, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 1, 36, 493, tsID));
                    tiles.Add(new Tile(TileLayer.Buildings, 2, 36, 371, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 0, 37, 517, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 1, 37, 518, tsID));
                    tiles.Add(new Tile(TileLayer.Buildings, 2, 37, 519, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 0, 38, 542, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 1, 38, 543, tsID));
                    tiles.Add(new Tile(TileLayer.Buildings, 2, 38, 544, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 0, 39, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 1, 39, -1, tsID));
                    tiles.Add(new Tile(TileLayer.Buildings, 2, 39, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 0, 40, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 1, 40, -1, tsID));
                    tiles.Add(new Tile(TileLayer.Buildings, 2, 40, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 0, 41, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 1, 41, -1, tsID));
                    tiles.Add(new Tile(TileLayer.Buildings, 2, 41, -1, tsID));

                    tiles.Add(new Tile(TileLayer.Front, 0, 35, -1, tsID)); tiles.Add(new Tile(TileLayer.Front, 2, 36, 494, tsID));

                    warpYLocA = 39; warpYLocB = 40; warpYLocC = 41;
                    break;
                default: // Default Farm
                    tiles.Add(new Tile(TileLayer.Back, 0, 38, 175, tsID)); tiles.Add(new Tile(TileLayer.Back, 1, 38, 175, tsID)); tiles.Add(new Tile(TileLayer.Back, 0, 43, 537, tsID));
                    tiles.Add(new Tile(TileLayer.Back, 1, 43, 537, tsID)); tiles.Add(new Tile(TileLayer.Back, 2, 43, 586, tsID)); tiles.Add(new Tile(TileLayer.Back, 0, 44, 566, tsID));
                    tiles.Add(new Tile(TileLayer.Back, 1, 44, 537, tsID)); tiles.Add(new Tile(TileLayer.Back, 2, 44, 618, tsID)); tiles.Add(new Tile(TileLayer.Back, 0, 45, 587, tsID));
                    tiles.Add(new Tile(TileLayer.Back, 1, 45, 473, tsID)); tiles.Add(new Tile(TileLayer.Back, 0, 46, 587, tsID)); tiles.Add(new Tile(TileLayer.Back, 1, 46, 587, tsID));
                    tiles.Add(new Tile(TileLayer.Back, 0, 48, 175, tsID)); tiles.Add(new Tile(TileLayer.Back, 1, 48, 175, tsID));

                    tiles.Add(new Tile(TileLayer.Buildings, 0, 39, 175, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 1, 39, 175, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 2, 39, 444, tsID));
                    tiles.Add(new Tile(TileLayer.Buildings, 0, 40, 446, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 1, 40, 468, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 2, 40, 469, tsID));
                    tiles.Add(new Tile(TileLayer.Buildings, 0, 41, 492, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 1, 41, 493, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 2, 41, 494, tsID));
                    tiles.Add(new Tile(TileLayer.Buildings, 0, 42, 517, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 1, 42, 518, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 2, 42, 519, tsID));
                    tiles.Add(new Tile(TileLayer.Buildings, 0, 43, 542, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 1, 43, 543, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 2, 43, 544, tsID));
                    tiles.Add(new Tile(TileLayer.Buildings, 0, 44, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 1, 44, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 2, 44, -1, tsID));
                    tiles.Add(new Tile(TileLayer.Buildings, 0, 45, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 1, 45, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 2, 45, -1, tsID));
                    tiles.Add(new Tile(TileLayer.Buildings, 0, 46, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 1, 46, -1, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 2, 46, -1, tsID));
                    tiles.Add(new Tile(TileLayer.Buildings, 0, 47, 175, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 1, 47, 175, tsID)); tiles.Add(new Tile(TileLayer.Buildings, 0, 48, -1, tsID));

                    tiles.Add(new Tile(TileLayer.Front, 0, 36, -1, tsID)); tiles.Add(new Tile(TileLayer.Front, 1, 36, -1, tsID)); tiles.Add(new Tile(TileLayer.Front, 0, 37, -1, tsID));
                    tiles.Add(new Tile(TileLayer.Front, 1, 37, -1, tsID)); tiles.Add(new Tile(TileLayer.Front, 0, 38, -1, tsID)); tiles.Add(new Tile(TileLayer.Front, 1, 38, -1, tsID));
                    tiles.Add(new Tile(TileLayer.Front, 0, 39, -1, tsID)); tiles.Add(new Tile(TileLayer.Front, 1, 39, -1, tsID)); tiles.Add(new Tile(TileLayer.Front, 0, 40, -1, tsID));
                    tiles.Add(new Tile(TileLayer.Front, 0, 41, -1, tsID)); tiles.Add(new Tile(TileLayer.Front, 0, 46, 414, tsID)); tiles.Add(new Tile(TileLayer.Front, 1, 46, 413, tsID));
                    tiles.Add(new Tile(TileLayer.Front, 2, 46, 438, tsID)); tiles.Add(new Tile(TileLayer.Front, 0, 47, 175, tsID)); tiles.Add(new Tile(TileLayer.Front, 1, 47, 175, tsID));
                    tiles.Add(new Tile(TileLayer.Front, 2, 47, 394, tsID));

                    warpYLocA = 44; warpYLocB = 45; warpYLocC = 46;
                    break;
            }

            performTileEdits(gl, tiles);

            gl.warps.Add(new Warp(-1, warpYLocA, "FarmExpansion", 46, 4, false));
            gl.warps.Add(new Warp(-1, warpYLocB, "FarmExpansion", 46, 5, false));
            gl.warps.Add(new Warp(-1, warpYLocC, "FarmExpansion", 46, 6, false));

            farmExpansion.warps.Add(new Warp(48, 4, "Farm", 0, warpYLocA, false));
            farmExpansion.warps.Add(new Warp(48, 5, "Farm", 0, warpYLocB, false));
            farmExpansion.warps.Add(new Warp(48, 6, "Farm", 0, warpYLocC, false));
        }

        private void performTileEdits(GameLocation gl, List<Tile> tiles)
        {
            foreach (Tile tile in tiles)
            {
                Layer layer = gl.map.GetLayer(tile.LayerName);
                TileSheet tilesheet = gl.map.GetTileSheet(tile.Tilesheet);

                if (tile.TileID < 0)
                {
                    gl.removeTile(tile.X, tile.Y, tile.LayerName);
                    continue;
                }

                if (layer.Tiles[tile.X, tile.Y] == null || layer.Tiles[tile.X, tile.Y].TileSheet.Id != tile.Tilesheet)
                    layer.Tiles[tile.X, tile.Y] = new StaticTile(layer, tilesheet, BlendMode.Alpha, tile.TileID);
                else
                    gl.setMapTileIndex(tile.X, tile.Y, tile.TileID, layer.Id);
            }
        }

        private void robinHammerSound(StardewValley.Farmer who)
        {
            if (Game1.currentLocation.Equals(robin.currentLocation) && Utility.isOnScreen(robin.position, Game1.tileSize * 4))
            {
                Game1.playSound((Game1.random.NextDouble() < 0.1) ? "clank" : "axchop");
                helper.Reflection.GetField<int>(robin, "shakeTimer").SetValue(250);
            }
        }

        private void robinVariablePause(StardewValley.Farmer who)
        {
            if (Game1.random.NextDouble() < 0.4)
            {
                robin.Sprite.currentAnimation[robin.Sprite.currentAnimationIndex] = new FarmerSprite.AnimationFrame(27, 300, false, false, new AnimatedSprite.endOfAnimationBehavior(robinVariablePause), false);
                return;
            }
            if (Game1.random.NextDouble() < 0.25)
            {
                robin.Sprite.currentAnimation[robin.Sprite.currentAnimationIndex] = new FarmerSprite.AnimationFrame(23, Game1.random.Next(500, 4000), false, false, new AnimatedSprite.endOfAnimationBehavior(robinVariablePause), false);
                return;
            }
            robin.Sprite.currentAnimation[robin.Sprite.currentAnimationIndex] = new FarmerSprite.AnimationFrame(27, Game1.random.Next(1000, 4000), false, false, new AnimatedSprite.endOfAnimationBehavior(robinVariablePause), false);
        }

        internal Farm swapFarm(Farm currentFarm)
        {
            return expansionSelected(currentFarm.Name) ? Game1.getFarm() : farmExpansion;
        }

        internal bool expansionSelected(string currentFarmName)
        {
            return currentFarmName.Equals("FarmExpansion");
        }
    }
}
