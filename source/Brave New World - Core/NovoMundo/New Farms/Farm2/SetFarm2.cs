/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiogoAlbano/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
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
using Object = StardewValley.Object;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using System.Reflection;
using Netcode;
using StardewValley.Network;
using HarmonyLib;

namespace NovoMundo.Farm2
{
    public class Set_Farm2
    {
        public event EventHandler BeforeRemoveEvent;
        public event EventHandler AfterAppendEvent;
        internal IModHelper helper;
        internal readonly ICollection<BluePrint> FarmBlueprints = new List<BluePrint>();
        internal readonly ICollection<BluePrint> ExpansionBlueprints = new List<BluePrint>();
        private NMFarm2 farmExpansion;
        private Map map;
        private XmlSerializer locationSerializer = new XmlSerializer(typeof(NMFarm2));
        private NPC robin;
        private IClickableMenu menuOverride;
        private bool overridableMenuActive;
        public void Apply_Harmony(Harmony harmony)
        {
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
        public Set_Farm2(IModHelper helper)
        {
            this.helper = helper;
        }
        internal void AddFarmBluePrint(BluePrint blueprint)
        {
            this.FarmBlueprints.Add(blueprint);
        }
        internal void AddExpansionBluePrint(BluePrint blueprint)
        {
            this.ExpansionBlueprints.Add(blueprint);
        }
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
        internal void RenderingActiveMenu(object sender, EventArgs e)
        {
            menuOverride.draw(Game1.spriteBatch);
        }
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
                            this.helper.Reflection.GetField<string>(mp, "playerLocationName").SetValue("Farm Expansion");
                        }
                        return;
                    }
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
            }
        }
        internal void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            map = helper.ModContent.Load<Map>("assets/NMFarm2.tbin");
            map.LoadTileSheets(Game1.mapDisplayDevice);

            if (!File.Exists(Path.Combine(helper.DirectoryPath, "pslocationdata2", $"{Constants.SaveFolderName}.xml")))
            {
                farmExpansion = new NMFarm2(map, "NMFarm2", this)
                {
                    IsFarm = true,
                    IsOutdoors = true

                };

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
        private void PatchMap()
        {
            //farmExpansion.warps.Add(new Warp(48, 4, "Backwoods", 0, 27, false));
            //farmExpansion.warps.Add(new Warp(48, 5, "Backwoods", 0, 28, false));
            //farmExpansion.warps.Add(new Warp(48, 6, "Backwoods", 0, 29, false));
            farmExpansion.map.Properties.Add("EnableGrassSpread", true);
        }
        internal void OnSaving(object sender, SavingEventArgs e)
        {
            Save();
            BeforeRemoveEvent?.Invoke(farmExpansion, EventArgs.Empty);
            Game1.locations.Remove(farmExpansion);
        }
        internal void OnSaved(object sender, SavedEventArgs e)
        {
            Game1.locations.Add(farmExpansion);
            AfterAppendEvent?.Invoke(farmExpansion, EventArgs.Empty);
        }
        internal void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            farmExpansion = null;
            map = null;
        }
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
                                "NMFarm2",
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
            string path = Path.Combine(helper.DirectoryPath, "pslocationdata2", $"{Constants.SaveFolderName}.xml");

            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            using (var writer = XmlWriter.Create(path))
            {
                locationSerializer.Serialize(writer, farmExpansion);
            }
        }
        private void Load()
        {
            farmExpansion = new NMFarm2(map, "NMFarm2", this)
            {
                IsFarm = true,
                IsOutdoors = true,
            };
            string path = Path.Combine(helper.DirectoryPath, "pslocationdata2", $"{Constants.SaveFolderName}.xml");
            NMFarm2 loaded;
            using (var reader = XmlReader.Create(path))
            {
                loaded = (NMFarm2)locationSerializer.Deserialize(reader);
            }
            farmExpansion.animals.CopyFrom(loaded.animals.Pairs);
            farmExpansion.buildings.ReplaceWith(loaded.buildings);
            farmExpansion.characters.ReplaceWith(loaded.characters);
            farmExpansion.terrainFeatures.ReplaceWith(loaded.terrainFeatures);
            farmExpansion.largeTerrainFeatures.ReplaceWith(loaded.largeTerrainFeatures);
            farmExpansion.resourceClumps.ReplaceWith(loaded.resourceClumps);
            farmExpansion.objects.ReplaceWith(loaded.objects);
            farmExpansion.numberOfSpawnedObjectsOnMap = loaded.numberOfSpawnedObjectsOnMap;
            farmExpansion.piecesOfHay.Value = loaded.piecesOfHay.Value;
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
                        warps.Add(new Warp(warp.X, warp.Y, "NMFarm2", building.humanDoor.X + building.tileX.Value, building.humanDoor.Y + building.tileY.Value + 1, false));
                    }
                    building.indoors.Value.warps.Clear();
                    building.indoors.Value.warps.AddRange(warps);
                }
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
    }
    [HarmonyPatch(typeof(NetBuildingRef))]
    [HarmonyPatch("Value", MethodType.Getter)]
    public class NetBuildingRefPatch
    {
        public static void Postfix(NetBuildingRef __instance, NetString ___nameOfIndoors, ref Building __result)
        {
            if (__result != null)
                return;
            var locationFromName = (NMFarm2)Game1.getLocationFromName("NMFarm2");
            if (locationFromName == null)
                return;
            __result = locationFromName.getBuildingByName((string)((NetFieldBase<string, NetString>)___nameOfIndoors).Value);
        }
    }
    static class Helpers
    {
        public static void ReplaceWith<T, TField>(this NetVector2Dictionary<T, TField> collection, NetVector2Dictionary<T, TField> source)
            where TField : NetField<T, TField>, new()
        {
            collection.Clear();
            foreach (var kvp in source.Pairs)
            {
                collection.Add(kvp.Key, kvp.Value);
            }
        }
        public static void ReplaceWith(this OverlaidDictionary collection, OverlaidDictionary source)
        {
            collection.Clear();
            foreach (var kvp in source.Pairs)
            {
                collection.Add(kvp.Key, kvp.Value);
            }
        }
        public static void ReplaceWith<T>(this Netcode.NetCollection<T> collection, Netcode.NetCollection<T> source)
            where T : class, Netcode.INetObject<Netcode.INetSerializable>
        {
            collection.Clear();
            foreach (var item in source)
            {
                collection.Add(item);
            }
        }
    }
}