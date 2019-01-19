using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Layers;
using xTile.ObjectModel;
using xTile.Tiles;

namespace MTN2.Patches.GameLocationPatches
{
    /// <summary>
    /// REASON FOR PATCHING: To enable "No Debris" option, and for more than
    /// three cabins at the start of a game.
    /// 
    /// Patches the method GameLocation.loadObjects to enable to use of the
    /// no debris option when creating a new game. In addition to readjusting
    /// the maximum amount of starting cabins for multipler on a custom farm map.
    /// </summary>
    public class loadObjectsPatch
    {
        private static ICustomManager customManager;

        /// <summary>
        /// Constructor. Awkward method of setting references needed. However, Harmony patches
        /// are required to be static. Thus we must break good Object Orientated practices.
        /// </summary>
        /// <param name="CustomManager">The class controlling information pertaining to the customs (and the loaded customs).</param>
        public loadObjectsPatch(ICustomManager customManager) {
            loadObjectsPatch.customManager = customManager;
        }

        /// <summary>
        /// Prefix Method. Occurs before the original method is executed.
        /// 
        /// Checks to see if the <see cref="GameLocation"/> is a <see cref="Farm"/>. Skips the original
        /// method if so.
        /// </summary>
        /// <param name="__instance">The instance of <see cref="GameLocation"/> that called loadObjects.</param>
        /// <returns></returns>
        public static bool Prefix(GameLocation __instance) {
            if (__instance is Farm) return false;
            return true;
        }

        /// <summary>
        /// Postfix Method. Occurs after the original method has executed.
        /// 
        /// Checks to see if the <see cref="GameLocation"/> is a <see cref="Farm"/>. Populates the map
        /// with objects. Excludes certain objects if no debris was selected by user. Populates the map
        /// with an adjusted amount of cabins if allowed and requested.
        /// </summary>
        /// <param name="__instance">The instance of <see cref="GameLocation"/> that called loadObjects.</param>
        public static void Postfix(GameLocation __instance) {
            int cabinCount = 0;

            if (!(__instance is Farm)) return;

            if (__instance.map != null) {
                updateWarps(__instance);
                PropertyValue springObjects;
                __instance.map.Properties.TryGetValue(Game1.currentSeason.Substring(0, 1).ToUpper() + Game1.currentSeason.Substring(1) + "_Objects", out springObjects);
                if (springObjects != null && !Game1.eventUp) {
                    __instance.spawnObjects();
                }
                bool hasPathsLayer = false;
                using (IEnumerator<Layer> enumerator = __instance.map.Layers.GetEnumerator()) {
                    while (enumerator.MoveNext()) {
                        if (enumerator.Current.Id.Equals("Paths")) {
                            hasPathsLayer = true;
                            break;
                        }
                    }
                }
                PropertyValue trees;
                __instance.map.Properties.TryGetValue("Trees", out trees);
                if (trees != null) {
                    string[] rawTreeString = trees.ToString().Split(new char[]
                    {
                ' '
                    });
                    for (int i = 0; i < rawTreeString.Length; i += 3) {
                        int x = Convert.ToInt32(rawTreeString[i]);
                        int y = Convert.ToInt32(rawTreeString[i + 1]);
                        int treeType = Convert.ToInt32(rawTreeString[i + 2]) + 1;
                        __instance.terrainFeatures.Add(new Vector2((float)x, (float)y), new Tree(treeType, 5));
                    }
                }
                if ((__instance.isOutdoors || __instance.name.Equals("BathHouse_Entry") || __instance.treatAsOutdoors) && hasPathsLayer) {
                    List<Vector2> startingCabins = new List<Vector2>();
                    for (int x2 = 0; x2 < __instance.map.Layers[0].LayerWidth; x2++) {
                        for (int y2 = 0; y2 < __instance.map.Layers[0].LayerHeight; y2++) {
                            Tile t = __instance.map.GetLayer("Paths").Tiles[x2, y2];
                            if (t != null) {
                                Vector2 tile = new Vector2((float)x2, (float)y2);
                                int treeType2 = -1;
                                switch (t.TileIndex) {
                                    case 9:
                                        //Tree 1
                                        if (__instance is Farm && customManager.NoDebris) break;
                                        treeType2 = 1;
                                        if (Game1.currentSeason.Equals("winter")) {
                                            treeType2 += 3;
                                        }
                                        break;
                                    case 10:
                                        //Tree 2
                                        if (__instance is Farm && customManager.NoDebris) break;
                                        treeType2 = 2;
                                        if (Game1.currentSeason.Equals("winter")) {
                                            treeType2 += 3;
                                        }
                                        break;
                                    case 11:
                                        //Tree 3
                                        if (__instance is Farm && customManager.NoDebris) break;
                                        treeType2 = 3;
                                        break;
                                    case 12:
                                        //Palm Tree
                                        if (__instance is Farm && customManager.NoDebris) break;
                                        treeType2 = 6;
                                        break;
                                }
                                if (treeType2 != -1) {
                                    if (!__instance.terrainFeatures.ContainsKey(tile) && !__instance.objects.ContainsKey(tile)) {
                                        __instance.terrainFeatures.Add(tile, new Tree(treeType2, 5));
                                    }
                                } else {
                                    switch (t.TileIndex) {
                                        case 13:
                                        case 14:
                                        case 15:
                                            //Grass
                                            if (__instance is Farm && customManager.NoDebris) break;
                                            if (!__instance.objects.ContainsKey(tile)) {
                                                __instance.objects.Add(tile, new StardewValley.Object(tile, GameLocation.getWeedForSeason(Game1.random, Game1.currentSeason), 1));
                                            }
                                            break;
                                        case 16:
                                            //Rock
                                            if (__instance is Farm && customManager.NoDebris) break;
                                            if (!__instance.objects.ContainsKey(tile)) {
                                                __instance.objects.Add(tile, new StardewValley.Object(tile, (Game1.random.NextDouble() < 0.5) ? 343 : 450, 1));
                                            }
                                            break;
                                        case 17:
                                            //Rock
                                            if (__instance is Farm && customManager.NoDebris) break;
                                            if (!__instance.objects.ContainsKey(tile)) {
                                                __instance.objects.Add(tile, new StardewValley.Object(tile, (Game1.random.NextDouble() < 0.5) ? 343 : 450, 1));
                                            }
                                            break;
                                        case 18:
                                            //Twig
                                            if (__instance is Farm && customManager.NoDebris) break;
                                            if (!__instance.objects.ContainsKey(tile)) {
                                                __instance.objects.Add(tile, new StardewValley.Object(tile, (Game1.random.NextDouble() < 0.5) ? 294 : 295, 1));
                                            }
                                            break;
                                        case 19:
                                            //Big Log
                                            if (__instance is Farm && customManager.NoDebris) break;
                                            if (__instance is Farm) {
                                                (__instance as Farm).addResourceClumpAndRemoveUnderlyingTerrain(602, 2, 2, tile);
                                            }
                                            break;
                                        case 20:
                                            //Rock
                                            if (__instance is Farm && customManager.NoDebris) break;
                                            if (__instance is Farm) {
                                                (__instance as Farm).addResourceClumpAndRemoveUnderlyingTerrain(672, 2, 2, tile);
                                            }
                                            break;
                                        case 21:
                                            //Stump
                                            if (__instance is Farm && customManager.NoDebris) break;
                                            if (__instance is Farm) {
                                                (__instance as Farm).addResourceClumpAndRemoveUnderlyingTerrain(600, 2, 2, tile);
                                            }
                                            break;
                                        case 22:
                                            //Grass
                                            if (__instance is Farm && customManager.NoDebris) break;
                                            if (!__instance.terrainFeatures.ContainsKey(tile)) {
                                                __instance.terrainFeatures.Add(tile, new Grass(1, 3));
                                            }
                                            break;
                                        case 23:
                                            //Small Tree
                                            if (__instance is Farm && customManager.NoDebris) break;
                                            if (!__instance.terrainFeatures.ContainsKey(tile)) {
                                                __instance.terrainFeatures.Add(tile, new Tree(Game1.random.Next(1, 4), Game1.random.Next(2, 4)));
                                            }
                                            break;
                                        case 24:
                                            if (!__instance.terrainFeatures.ContainsKey(tile)) {
                                                __instance.largeTerrainFeatures.Add(new Bush(tile, 2, __instance));
                                            }
                                            break;
                                        case 25:
                                            if (!__instance.terrainFeatures.ContainsKey(tile)) {
                                                __instance.largeTerrainFeatures.Add(new Bush(tile, 1, __instance));
                                            }
                                            break;
                                        case 26:
                                            if (!__instance.terrainFeatures.ContainsKey(tile)) {
                                                __instance.largeTerrainFeatures.Add(new Bush(tile, 0, __instance));
                                            }
                                            break;
                                        case 27:
                                            changeMapProperties(__instance, "BrookSounds", string.Concat(new object[]
                                            {tile.X, " ", tile.Y," 0"}));
                                            break;
                                        case 28: {
                                                string a = __instance.name;
                                                if (a == "BugLand") {
                                                    __instance.characters.Add(new Grub(new Vector2(tile.X * 64f, tile.Y * 64f), true));
                                                }
                                                break;
                                            }
                                        case 29:
                                        case 30:
                                            if (Game1.startingCabins > 0) {

                                                PropertyValue pv = null;
                                                t.Properties.TryGetValue("Order", out pv);
                                                if (pv != null && int.Parse(pv.ToString()) <= Game1.startingCabins && ((t.TileIndex == 29 && !Game1.cabinsSeparate) || (t.TileIndex == 30 && Game1.cabinsSeparate))) {
                                                    startingCabins.Add(tile);
                                                    cabinCount++;
                                                }
                                            }
                                            break;
                                    }
                                }
                            }
                            if (__instance.map.GetLayer("Buildings").Tiles[x2, y2] != null) {
                                PropertyValue door = null;
                                __instance.map.GetLayer("Buildings").Tiles[x2, y2].Properties.TryGetValue("Action", out door);
                                if (door != null && door.ToString().Contains("Warp")) {
                                    string[] split = door.ToString().Split(new char[]
                                    {
                                ' '
                                    });
                                    if (split[0].Equals("WarpCommunityCenter")) {
                                        __instance.doors.Add(new Point(x2, y2), new NetString("CommunityCenter"));
                                    } else if ((!__instance.name.Equals("Mountain") || x2 != 8 || y2 != 20) && split.Length > 2) {
                                        __instance.doors.Add(new Point(x2, y2), new NetString(split[3]));
                                    }
                                }
                            }
                        }
                    }
                    if (startingCabins.Count > 0) {
                        if (cabinCount < Game1.startingCabins) {
                            //Gotta finish the job
                            //if (Memory.listOfCustomFarms[Game1.whichFarm].additionalCabins != null)
                            //{
                            //    foreach (extraCabin ec in Memory.listOfCustomFarms[Game1.whichFarm].additionalCabins)
                            //    {
                            //        Vector2 tile = new Vector2(ec.x, ec.y);
                            //        startingCabins.Add(tile);
                            //    }
                            //}
                        }
                        performCabins(__instance, startingCabins);
                    }
                }
            }
        }

        //public static void Postfix(GameLocation __instance)
        public static void performCabins(GameLocation g, List<Vector2> cabins) {
            if (cabins.Count > 0) {
                List<string> cabinStyleOrder = new List<string>();
                Random rng = new Random();

                for (int k = 0; k < cabins.Count; k++) {
                    int num = rng.Next(0, 2);
                    cabinStyleOrder.Add((num == 0) ? "Log Cabin" : (num == 1 ? "Plank Cabin" : "Stone Cabin"));
                }

                for (int j = 0; j < cabins.Count; j++) {
                    if (g is BuildableGameLocation) {
                        clearArea(g, (int)cabins[j].X, (int)cabins[j].Y, 5, 3);
                        clearArea(g, (int)cabins[j].X + 2, (int)cabins[j].Y + 3, 1, 1);
                        g.setTileProperty((int)cabins[j].X + 2, (int)cabins[j].Y + 3, "Back", "NoSpawn", "All");
                        Building b = new Building(new BluePrint(cabinStyleOrder[j]) {
                            magical = true
                        }, cabins[j]);
                        b.daysOfConstructionLeft.Value = 0;
                        b.load();
                        (g as BuildableGameLocation).buildStructure(b, cabins[j], Game1.player, true);
                    }
                }
            }
        }

        private static void clearArea(GameLocation g, int startingX, int startingY, int width, int height) {
            for (int x = startingX; x < startingX + width; x++) {
                for (int y = startingY; y < startingY + height; y++) {
                    g.removeEverythingExceptCharactersFromThisTile(x, y);
                }
            }
        }

        private static void changeMapProperties(GameLocation g, string propertyName, string toAdd) {
            try {
                bool addSpaceToFront = true;
                if (!g.map.Properties.ContainsKey(propertyName)) {
                    g.map.Properties.Add(propertyName, new PropertyValue(string.Empty));
                    addSpaceToFront = false;
                }
                if (!g.map.Properties[propertyName].ToString().Contains(toAdd)) {
                    StringBuilder b = new StringBuilder(g.map.Properties[propertyName].ToString());
                    if (addSpaceToFront) {
                        b.Append(" ");
                    }
                    b.Append(toAdd);
                    g.map.Properties[propertyName] = new PropertyValue(b.ToString());
                }
            } catch (Exception) {
            }
        }

        protected static void updateWarps(GameLocation g) {
            g.warps.Clear();
            PropertyValue warpsUnparsed;
            g.map.Properties.TryGetValue("Warp", out warpsUnparsed);
            if (warpsUnparsed != null) {
                string[] warpInfo = warpsUnparsed.ToString().Split(new char[]
                {
                    ' '
                });
                for (int i = 0; i < warpInfo.Length; i += 5) {
                    g.warps.Add(new Warp(Convert.ToInt32(warpInfo[i]), Convert.ToInt32(warpInfo[i + 1]), warpInfo[i + 2], Convert.ToInt32(warpInfo[i + 3]), Convert.ToInt32(warpInfo[i + 4]), false));
                }
            }
        }
    }
}