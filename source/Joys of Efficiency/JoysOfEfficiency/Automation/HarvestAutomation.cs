using System;
using System.Collections.Generic;
using System.Linq;

using JoysOfEfficiency.Core;
using JoysOfEfficiency.Utils;

using Microsoft.Xna.Framework;

using StardewModdingAPI;

using StardewValley;
using StardewValley.Characters;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using SVObject = StardewValley.Object;

namespace JoysOfEfficiency.Automation
{
    internal class HarvestAutomation
    {
        private static IReflectionHelper Reflection => InstanceHolder.Reflection;
        private static Config Config => InstanceHolder.Config;

        private static readonly Logger Logger = new Logger("HarvestAutomation");

        private static readonly List<Vector2> FlowerLocationProducingNectar = new List<Vector2>();
        public static void UpdateNectarInfo()
        {
            FlowerLocationProducingNectar.Clear();
            foreach (KeyValuePair<Vector2, SVObject> kv in
                Game1.currentLocation.Objects.Pairs.Where(
                    pair => pair.Value.Name == "Bee House")
                )
            {
                Vector2 houseLoc = kv.Key;
                foreach (Vector2 flowerLoc in GetAreaOfCollectingNectar(houseLoc))
                {
                    if ((int)flowerLoc.X >= 0 && (int)flowerLoc.Y >= 0 && !FlowerLocationProducingNectar.Contains(flowerLoc))
                    {
                        FlowerLocationProducingNectar.Add(flowerLoc);
                    }
                }
            }
        }

        public static void HarvestNearbyCrops(Farmer player)
        {
            GameLocation location = player.currentLocation;
            int radius = Config.AutoHarvestRadius;

            if (Config.ProtectNectarProducingFlower)
            {
                UpdateNectarInfo();
            }

            foreach (KeyValuePair<Vector2, HoeDirt> kv in Util.GetFeaturesWithin<HoeDirt>(radius))
            {
                Vector2 loc = kv.Key;
                HoeDirt dirt = kv.Value;
                if (dirt.crop == null || !dirt.readyForHarvest())
                {
                    continue;
                }

                if (IsBlackListed(dirt.crop) || Config.ProtectNectarProducingFlower && IsProducingNectar(loc))
                {
                    continue;
                }

                if (!dirt.crop.harvest((int) loc.X, (int) loc.Y, dirt))
                {
                    continue;
                }

                if (dirt.crop.regrowAfterHarvest.Value == -1 || dirt.crop.forageCrop.Value)
                {
                    dirt.destroyCrop(loc, true, location);
                }
            }
            foreach (IndoorPot pot in Util.GetObjectsWithin<IndoorPot>(radius))
            {
                HoeDirt dirt = pot.hoeDirt.Value;
                if (dirt?.crop == null || !dirt.readyForHarvest())
                {
                    continue;
                }

                if (!dirt.crop.harvest((int)pot.TileLocation.X, (int)pot.TileLocation.Y, dirt))
                {
                    continue;
                }

                if (dirt.crop.regrowAfterHarvest.Value == -1 || dirt.crop.forageCrop.Value)
                {
                    dirt.destroyCrop(pot.TileLocation, true, location);
                }
            }
        }

        public static void WaterNearbyCrops()
        {
            WateringCan can = Util.FindToolFromInventory<WateringCan>(Game1.player, InstanceHolder.Config.FindCanFromInventory);
            if (can == null)
                return;

            Util.GetMaxCan(can);
            bool watered = false;
            foreach (KeyValuePair<Vector2, HoeDirt> kv in Util.GetFeaturesWithin<HoeDirt>(InstanceHolder.Config.AutoWaterRadius))
            {
                HoeDirt dirt = kv.Value;
                float consume = 2 * (1.0f / (can.UpgradeLevel / 2.0f + 1));
                if (dirt.crop == null || dirt.crop.dead.Value || dirt.state.Value != 0 ||
                    !(Game1.player.Stamina >= consume)
                    || can.WaterLeft <= 0)
                {
                    continue;
                }

                dirt.state.Value = 1;
                Game1.player.Stamina -= consume;
                can.WaterLeft--;
                watered = true;
            }
            foreach (IndoorPot pot in Util.GetObjectsWithin<IndoorPot>(InstanceHolder.Config.AutoWaterRadius))
            {
                if (pot.hoeDirt.Value == null)
                    continue;

                HoeDirt dirt = pot.hoeDirt.Value;
                float consume = 2 * (1.0f / (can.UpgradeLevel / 2.0f + 1));
                if (dirt.crop != null && !dirt.crop.dead.Value && dirt.state.Value != 1 && Game1.player.Stamina >= consume && can.WaterLeft > 0)
                {
                    dirt.state.Value = 1;
                    pot.showNextIndex.Value = true;
                    Game1.player.Stamina -= consume;
                    can.WaterLeft--;
                    watered = true;
                }
            }
            if (watered)
            {
                Game1.playSound("slosh");
            }
        }

        public static void ToggleBlacklistUnderCursor()
        {
            GameLocation location = Game1.currentLocation;
            Vector2 tile = Game1.currentCursorTile;
            if (!location.terrainFeatures.TryGetValue(tile, out TerrainFeature terrain))
                return;
            if (!(terrain is HoeDirt dirt))
                return;

            if (dirt.crop == null)
            {
                Util.ShowHudMessage("There is no crop under the cursor");
            }
            else
            {
                string name = dirt.crop.forageCrop.Value ? Util.GetItemName(dirt.crop.whichForageCrop.Value) : Util.GetItemName(dirt.crop.indexOfHarvest.Value);
                if (name == "")
                {
                    return;
                }

                string text = ToggleBlackList(dirt.crop)
                    ? $"{name} has been added to AutoHarvest exception"
                    : $"{name} has been removed from AutoHarvest exception";
                Util.ShowHudMessage(text, 1000);
                Logger.Log(text);
            }
        }

        public static void DestroyNearDeadCrops(Farmer player)
        {
            GameLocation location = player.currentLocation;
            foreach (KeyValuePair<Vector2, HoeDirt> kv in Util.GetFeaturesWithin<HoeDirt>(1))
            {
                Vector2 loc = kv.Key;
                HoeDirt dirt = kv.Value;
                if (dirt.crop != null && dirt.crop.dead.Value)
                {
                    dirt.destroyCrop(loc, true, location);
                }

            }
            foreach (IndoorPot pot in Util.GetObjectsWithin<IndoorPot>(1))
            {
                Vector2 loc = Util.GetLocationOf(location, pot);
                HoeDirt dirt = pot.hoeDirt.Value;
                if (dirt?.crop != null && dirt.crop.dead.Value)
                {
                    dirt.destroyCrop(loc, true, location);
                }
            }
        }

        public static void ShakeNearbyFruitedBush()
        {
            int radius = InstanceHolder.Config.AutoShakeRadius;
            foreach (Bush bush in Game1.currentLocation.largeTerrainFeatures.OfType<Bush>())
            {
                Vector2 loc = bush.tilePosition.Value;
                Vector2 diff = loc - Game1.player.getTileLocation();
                if (Math.Abs(diff.X) > radius || Math.Abs(diff.Y) > radius)
                    continue;

                if (!bush.townBush.Value && bush.tileSheetOffset.Value == 1 && bush.inBloom(Game1.currentSeason, Game1.dayOfMonth))
                    bush.performUseAction(loc, Game1.currentLocation);
            }
        }

        public static void ShakeNearbyFruitedTree()
        {
            foreach (KeyValuePair<Vector2, TerrainFeature> kv in Util.GetFeaturesWithin<TerrainFeature>(InstanceHolder.Config.AutoShakeRadius))
            {
                Vector2 loc = kv.Key;
                TerrainFeature feature = kv.Value;
                switch (feature)
                {
                    case Tree tree:
                        if (tree.hasSeed.Value && !tree.stump.Value)
                        {
                            if (!Game1.IsMultiplayer && Game1.player.ForagingLevel < 1)
                            {
                                break;
                            }

                            int num2;
                            switch (tree.treeType.Value)
                            {
                                case 3:
                                    num2 = 311;
                                    break;
                                case 1:
                                    num2 = 309;
                                    break;
                                case 2:
                                    num2 = 310;
                                    break;
                                case 6:
                                    num2 = 88;
                                    break;
                                default:
                                    num2 = -1;
                                    break;
                            }

                            if (Game1.currentSeason.Equals("fall") && tree.treeType.Value == 2 &&
                                Game1.dayOfMonth >= 14)
                            {
                                num2 = 408;
                            }

                            if (num2 != -1)
                            { 
                                Reflection.GetMethod(tree, "shake").Invoke(loc, false, Game1.currentLocation);
                                Logger.Log($@"Shook fruited tree @{loc}");
                            }
                        }

                        break;
                    case FruitTree ftree:
                        if (ftree.growthStage.Value >= 4 && ftree.fruitsOnTree.Value > 0 && !ftree.stump.Value)
                        {
                            ftree.shake(loc, false, Game1.currentLocation);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Is the dirt's crop is a flower and producing nectar
        /// </summary>
        /// <param name="location">HoeDirt location to evaluate</param>
        /// <returns>Result</returns>
        private static bool IsProducingNectar(Vector2 location) => FlowerLocationProducingNectar.Contains(location);

        private static bool IsBlackListed(Crop crop)
        {
            int index = crop.forageCrop.Value ? crop.whichForageCrop.Value : crop.indexOfHarvest.Value;
            return InstanceHolder.Config.HarvestException.Contains(index);
        }

        private static bool ToggleBlackList(Crop crop)
        {
            int index = crop.forageCrop.Value ? crop.whichForageCrop.Value : crop.indexOfHarvest.Value;
            if (IsBlackListed(crop))
            {
                InstanceHolder.Config.HarvestException.Remove(index);
            }
            else
            {
                InstanceHolder.Config.HarvestException.Add(index);
            }

            InstanceHolder.WriteConfig();
            return IsBlackListed(crop);
        }

        private static IEnumerable<Vector2> GetAreaOfCollectingNectar(Vector2 homePoint)
        {
            List<Vector2> cropLocations = new List<Vector2>();
            Queue<Vector2> vector2Queue = new Queue<Vector2>();
            HashSet<Vector2> vector2Set = new HashSet<Vector2>();
            vector2Queue.Enqueue(homePoint);
            for (int index1 = 0; index1 <= 150 && vector2Queue.Count > 0; ++index1)
            {
                Vector2 index2 = vector2Queue.Dequeue();
                if (Game1.currentLocation.terrainFeatures.ContainsKey(index2) &&
                    Game1.currentLocation.terrainFeatures[index2] is HoeDirt dirt && dirt.crop != null &&
                    dirt.crop.programColored.Value && !dirt.crop.dead.Value && dirt.crop.currentPhase.Value >= dirt.crop.phaseDays.Count - 1)
                {
                    cropLocations.Add(index2);
                }
                foreach (Vector2 adjacentTileLocation in Utility.getAdjacentTileLocations(index2))
                {
                    if (!vector2Set.Contains(adjacentTileLocation))
                        vector2Queue.Enqueue(adjacentTileLocation);
                }
                vector2Set.Add(index2);
            }

            return cropLocations;
        }
        private static bool Harvest(int xTile, int yTile, HoeDirt soil, JunimoHarvester junimoHarvester = null)
        {
            return soil.crop.harvest(xTile, yTile, soil, junimoHarvester);
        }
    }
}
