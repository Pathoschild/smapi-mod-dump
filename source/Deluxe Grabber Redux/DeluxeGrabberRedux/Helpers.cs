/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ferdaber/sdv-mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace DeluxeGrabberRedux
{
    static class Helpers
    {
        public static IEnumerable<KeyValuePair<Vector2, SObject>> GetNearbyObjectsToTile(Vector2 tile, IEnumerable<KeyValuePair<Vector2, SObject>> objects, int range, string rangeMode)
        {
            if (range > -1)
            {
                if (rangeMode == "Walk")
                {
                    return objects.Where(pair =>
                    {
                        var other = pair.Key;
                        var manhattanDistance = Math.Abs(tile.X - other.X) + Math.Abs(tile.Y - other.Y);
                        return manhattanDistance <= range;
                    });
                }
                else if (rangeMode == "Square")
                {
                    return objects.Where(pair =>
                    {
                        var other = pair.Key;
                        return tile.X >= other.X - range && tile.X <= other.X + range && tile.Y >= other.Y - range && tile.Y <= other.Y + range;
                    });
                }
                else
                {
                    throw new Exception($"Unexpected range mode {rangeMode}.");
                }
            }
            else
            {
                return objects;
            }
        }

        public static SObject SetForageStatsBasedOnProfession(Farmer player, SObject forageable, Vector2 tileSpawned, bool ignoreGatherer = false)
        {
            // impl @ StardewValley::GameLocation::checkAction::objects[vector].isSpawnedObject
            var r = new Random((int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed + (int)tileSpawned.X + (int)tileSpawned.Y * 777);
            if (player.professions.Contains(Farmer.botanist))
            {
                forageable.Quality = SObject.bestQuality;
            }
            else if (r.NextDouble() < player.ForagingLevel / 30f)
            {
                forageable.Quality = SObject.highQuality;
            }
            else if (r.NextDouble() < player.ForagingLevel / 15f)
            {
                forageable.Quality = SObject.medQuality;
            }
            if (!ignoreGatherer && player.professions.Contains(Farmer.gatherer) && r.NextDouble() < 0.2) forageable.Stack += 1;
            return forageable;
        }

        public static List<SObject> HarvestCropFromHoeDirt(
            Farmer player,
            HoeDirt dirt,
            Vector2 tile,
            bool excludeFlowers,
            out int exp
        )
        {
            // impl @ StardewValley::Crop::harvest::NOT (bool)forageCrop
            exp = 0;
            Crop crop = dirt.crop;
            var harvests = new List<SObject>();
            var harvestable = crop.currentPhase.Value >= crop.phaseDays.Count - 1 && (!crop.fullyGrown.Value || crop.dayOfCurrentPhase.Value <= 0);
            if (!harvestable) return harvests;

            var harvestId = crop.indexOfHarvest.Value;
            if (harvestId == ItemIds.GoldenWalnut) return harvests;
            if (excludeFlowers && new SObject(harvestId, 1).Category == ItemIds.FlowersCategory) return harvests;

            Random r = new Random((int)tile.X * 7 + (int)tile.Y * 11 + (int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame);
            var fertilizerQualityLevel = ItemIds.FertilizerQualities.ContainsKey(dirt.fertilizer.Value) ? ItemIds.FertilizerQualities[dirt.fertilizer.Value] : 0;
            double chanceForGoldQuality = 0.2 * ((double)player.FarmingLevel / 10.0) + 0.2 * (double)fertilizerQualityLevel * (((double)player.FarmingLevel + 2.0) / 12.0) + 0.01;
            double chanceForSilverQuality = Math.Min(0.75, chanceForGoldQuality * 2.0);

            int cropQuality = SObject.lowQuality;
            if (fertilizerQualityLevel >= 3 && r.NextDouble() < chanceForGoldQuality / 2.0)
            {
                cropQuality = SObject.bestQuality;
            }
            else if (r.NextDouble() < chanceForGoldQuality)
            {
                cropQuality = SObject.highQuality;
            }
            else if (r.NextDouble() < chanceForSilverQuality || fertilizerQualityLevel >= 3)
            {
                cropQuality = SObject.medQuality;
            }
            if (harvestId == ItemIds.Fiber || harvestId == ItemIds.QiFruit)
            {
                cropQuality = SObject.lowQuality;
            }

            int numOfHarvests = 1;
            if (crop.minHarvest.Value > 1 || crop.maxHarvest.Value > 1)
            {
                int maxHarvestIncrease = crop.maxHarvestIncreasePerFarmingLevel.Value > 0 ? player.FarmingLevel / crop.maxHarvestIncreasePerFarmingLevel.Value : 0;
                numOfHarvests = r.Next(crop.minHarvest.Value, Math.Max(crop.minHarvest.Value + 1, crop.maxHarvest.Value + 1 + maxHarvestIncrease));
            }
            if (crop.chanceForExtraCrops.Value > 0)
            {
                while (r.NextDouble() < Math.Min(0.9, crop.chanceForExtraCrops.Value)) numOfHarvests++;
            }

            var firstHarvest = crop.programColored.Value
                ? new ColoredObject(harvestId, 1, crop.tintColor.Value) { Quality = cropQuality }
                : new SObject(harvestId, 1, false, -1, cropQuality);
            harvests.Add(firstHarvest);

            if (crop.harvestMethod.Value != 1)
            {
                var isLucky = r.NextDouble() < player.team.AverageLuckLevel() / 1500f + player.team.AverageDailyLuck() / 1200f + 9.9999997473787516E-05;
                if (isLucky) numOfHarvests *= 2;
            }

            if (harvestId == ItemIds.Sunflower)
            {
                harvestId = ItemIds.SunflowerSeed;
                numOfHarvests = r.Next(1, 4);
            }

            var extraHarvest = crop.programColored.Value ? new ColoredObject(harvestId, 1, crop.tintColor.Value) : new SObject(harvestId, 1);
            extraHarvest.Stack = numOfHarvests - 1;

            var price = Convert.ToInt32(Game1.objectInformation[harvestId].Split('/')[1]);
            exp = (int)Math.Round(16f * Math.Log(0.018 * (double)price + 1f, Math.E));

            harvests.Add(extraHarvest);

            if (harvestId == ItemIds.Wheat && r.NextDouble() < 0.4)
            {
                harvests.Add(new SObject(ItemIds.Hay, 1));
            }
            else if (harvestId == ItemIds.Fiber && r.NextDouble() < 0.1)
            {
                harvests.Add(new SObject(ItemIds.MixedSeeds, 1));
            }

            return harvests;
        }
    }
}
