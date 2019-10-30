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
        private static IMonitor Monitor => InstanceHolder.Monitor;
        private static Config Config => InstanceHolder.Config;

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

                if (!Harvest((int) loc.X, (int) loc.Y, dirt))
                {
                    continue;
                }

                if (dirt.crop.regrowAfterHarvest.Value == -1 || dirt.crop.forageCrop.Value)
                {
                    //destroy crop if it does not regrow.
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

                Vector2 tileLoc = Util.GetLocationOf(location, pot);
                if (!dirt.crop.harvest((int) tileLoc.X, (int) tileLoc.Y, dirt))
                {
                    continue;
                }
                if (dirt.crop.regrowAfterHarvest.Value == -1 || dirt.crop.forageCrop.Value)
                {
                    //destroy crop if it does not regrow.
                    dirt.destroyCrop(tileLoc, true, location);
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
                Monitor.Log(text);
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
                            if (Game1.currentSeason.Equals("fall") && tree.treeType.Value == 2 && Game1.dayOfMonth >= 14)
                            {
                                num2 = 408;
                            }
                            if (num2 != -1)
                                Reflection.GetMethod(tree, "shake").Invoke(loc, false);
                        }
                        break;
                    case FruitTree ftree:
                        if (ftree.growthStage.Value >= 4 && ftree.fruitsOnTree.Value > 0 && !ftree.stump.Value)
                        {
                            ftree.shake(loc, false);
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
            Multiplayer multiplayer = Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
            Crop crop = soil.crop;

            Farmer player = Game1.player;
            Random random = Game1.random;
            Stats stats = Game1.stats;
            GameLocation currentLocation = Game1.currentLocation;
            ulong uniqueIdForThisGame = Game1.uniqueIDForThisGame;
            double dailyLuck = Game1.dailyLuck;

            if (crop.dead.Value)
            {
                return false;
            }
            if (crop.forageCrop.Value)
            {
                SVObject o = null;
                const int experience2 = 3;
                int num = crop.whichForageCrop.Value;
                if (num == 1)
                {
                    o = new SVObject(399, 1);
                }
                if (player.professions.Contains(16))
                {
                    if (o != null) o.Quality = 4;
                }
                else if (random.NextDouble() < player.ForagingLevel / 30f)
                {
                    if (o != null) o.Quality = 2;
                }
                else if (random.NextDouble() < player.ForagingLevel / 15f)
                {
                    if (o != null) o.Quality = 1;
                }

                if (o == null)
                    return false;

                stats.ItemsForaged += (uint)o.Stack;
                if (junimoHarvester != null)
                {
                    junimoHarvester.tryToAddItemToHut(o);
                    return true;
                }

                if (player.addItemToInventoryBool(o))
                {
                    Vector2 initialTile2 = new Vector2(xTile, yTile);
                    player.animateOnce(279 + player.FacingDirection);
                    player.canMove = false;
                    player.currentLocation.playSound("harvest");
                    DelayedAction.playSoundAfterDelay("coin", 260);
                    if (crop.regrowAfterHarvest.Value == -1)
                    {
                        multiplayer.broadcastSprites(currentLocation,
                            new TemporaryAnimatedSprite(17, new Vector2(initialTile2.X * 64f, initialTile2.Y * 64f),
                                Color.White, 7, random.NextDouble() < 0.5, 125f));
                        multiplayer.broadcastSprites(currentLocation,
                            new TemporaryAnimatedSprite(14, new Vector2(initialTile2.X * 64f, initialTile2.Y * 64f),
                                Color.White, 7, random.NextDouble() < 0.5, 50f));
                    }

                    player.gainExperience(2, experience2);
                    return true;
                }
            }
            else if (crop.currentPhase.Value >= crop.phaseDays.Count - 1 && (!crop.fullyGrown.Value || crop.dayOfCurrentPhase.Value <= 0))
            {
                int numToHarvest = 1;
                int cropQuality = 0;
                int fertilizerQualityLevel = 0;
                if (crop.indexOfHarvest.Value == 0)
                {
                    return true;
                }
                Random r = new Random(xTile * 7 + yTile * 11 + (int)stats.DaysPlayed + (int)uniqueIdForThisGame);

                switch (soil.fertilizer.Value)
                {
                    case 368:
                        fertilizerQualityLevel = 1;
                        break;
                    case 369:
                        fertilizerQualityLevel = 2;
                        break;
                }

                double chanceForGoldQuality = 0.2 * (player.FarmingLevel / 10.0) + 0.2 * fertilizerQualityLevel * ((player.FarmingLevel + 2.0) / 12.0) + 0.01;
                double chanceForSilverQuality = Math.Min(0.75, chanceForGoldQuality * 2.0);
                if (r.NextDouble() < chanceForGoldQuality)
                {
                    cropQuality = 2;
                }
                else if (r.NextDouble() < chanceForSilverQuality)
                {
                    cropQuality = 1;
                }
                if (crop.minHarvest.Value > 1 || crop.maxHarvest.Value > 1)
                {
                    numToHarvest = r.Next(crop.minHarvest.Value, Math.Min(crop.minHarvest.Value + 1, crop.maxHarvest.Value + 1 + player.FarmingLevel / crop.maxHarvestIncreasePerFarmingLevel.Value));
                }
                if (crop.chanceForExtraCrops.Value > 0.0)
                {
                    while (r.NextDouble() < Math.Min(0.9, crop.chanceForExtraCrops.Value))
                    {
                        numToHarvest++;
                    }
                }
                if (crop.harvestMethod.Value == 1)
                {
                    for (int j = 0; j < numToHarvest; j++)
                    {
                        Game1.createObjectDebris(crop.indexOfHarvest.Value, xTile, yTile, -1, cropQuality);
                    }
                    if (crop.regrowAfterHarvest.Value == -1)
                    {
                        return true;
                    }
                    crop.dayOfCurrentPhase.Value = crop.regrowAfterHarvest.Value;
                    crop.fullyGrown.Value = true;
                }
                else if (player.addItemToInventoryBool(crop.programColored.Value ? new ColoredObject(crop.indexOfHarvest.Value, 1, crop.tintColor.Value)
                {
                    Quality = cropQuality
                }
                : new SVObject(crop.indexOfHarvest.Value, 1, false, -1, cropQuality)))
                {
                    Vector2 initialTile = new Vector2(xTile, yTile);
                    if (junimoHarvester == null)
                    {
                        player.animateOnce(279 + player.FacingDirection);
                        player.canMove = false;
                    }
                    else
                    {
                        junimoHarvester.tryToAddItemToHut(crop.programColored.Value ? new ColoredObject(crop.indexOfHarvest.Value, 1, crop.tintColor.Value)
                        {
                            Quality = cropQuality
                        } : new SVObject(crop.indexOfHarvest.Value, 1, false, -1, cropQuality));
                    }
                    if (r.NextDouble() < player.LuckLevel / 1500f + dailyLuck / 1200.0 + 9.9999997473787516E-05)
                    {
                        numToHarvest *= 2;
                        if (junimoHarvester == null)
                        {
                            player.currentLocation.playSound("dwoop");
                        }
                        else if (Utility.isOnScreen(junimoHarvester.getTileLocationPoint(), 64, junimoHarvester.currentLocation))
                        {
                            junimoHarvester.currentLocation.playSound("dwoop");
                        }
                    }
                    else if (crop.harvestMethod.Value == 0)
                    {
                        if (junimoHarvester == null)
                        {
                            player.currentLocation.playSound("harvest");
                        }
                        if (junimoHarvester == null)
                        {
                            DelayedAction.playSoundAfterDelay("coin", 260, player.currentLocation);
                        }
                        else if (Utility.isOnScreen(junimoHarvester.getTileLocationPoint(), 64, junimoHarvester.currentLocation))
                        {
                            DelayedAction.playSoundAfterDelay("coin", 260, junimoHarvester.currentLocation);
                        }
                        if (crop.regrowAfterHarvest.Value == -1)
                        {
                            multiplayer.broadcastSprites(currentLocation, new TemporaryAnimatedSprite(17, new Vector2(initialTile.X * 64f, initialTile.Y * 64f), Color.White, 7, random.NextDouble() < 0.5, 125f));
                            multiplayer.broadcastSprites(currentLocation, new TemporaryAnimatedSprite(14, new Vector2(initialTile.X * 64f, initialTile.Y * 64f), Color.White, 7, random.NextDouble() < 0.5, 50f));
                        }
                    }
                    if (crop.indexOfHarvest.Value == 421)
                    {
                        crop.indexOfHarvest.Value = 431;
                        numToHarvest = r.Next(1, 4);
                    }
                    for (int i = 0; i < numToHarvest - 1; i++)
                    {
                        Game1.createObjectDebris(crop.indexOfHarvest.Value, xTile, yTile);
                    }
                    int price = Convert.ToInt32(Game1.objectInformation[crop.indexOfHarvest.Value].Split('/')[1]);
                    float experience = (float)(16.0 * Math.Log(0.018 * price + 1.0, 2.7182818284590451));
                    if (junimoHarvester == null)
                    {
                        player.gainExperience(0, (int)Math.Round(experience));
                    }
                    if (crop.regrowAfterHarvest.Value == -1)
                    {
                        return true;
                    }
                    crop.dayOfCurrentPhase.Value = crop.regrowAfterHarvest.Value;
                    crop.fullyGrown.Value = true;
                }
            }
            return false;
        }
    }
}
