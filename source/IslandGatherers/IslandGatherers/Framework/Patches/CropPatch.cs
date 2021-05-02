/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/IslandGatherers
**
*************************************************/

using Harmony;
using IslandGatherers.Framework.Objects;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IslandGatherers.Framework.Patches
{
    internal class CropPatch
    {
        private static IMonitor monitor;
        private readonly System.Type _object = typeof(Crop);

        internal CropPatch(IMonitor modMonitor)
        {
            monitor = modMonitor;
        }

        internal void Apply(HarmonyInstance harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(Crop.harvest), new[] { typeof(int), typeof(int), typeof(HoeDirt), typeof(JunimoHarvester) }), prefix: new HarmonyMethod(GetType(), nameof(HarvestPrefix)));
        }

        [HarmonyPriority(Priority.Low)]
        private static bool HarvestPrefix(Crop __instance, Vector2 ___tilePosition, int xTile, int yTile, HoeDirt soil, JunimoHarvester junimoHarvester = null)
        {
            Object cropObj = new Object(__instance.indexOfHarvest, 1);
            string cropName = "Unknown";
            if (cropObj != null)
            {
                cropName = cropObj.DisplayName;
            }

            if (soil is null)
            {
                monitor.Log($"Crop ({cropName}) at {xTile}, {yTile} is missing HoeDirt, unable to process!", LogLevel.Trace);
                return true;
            }
            if (soil.currentLocation is null)
            {
                monitor.Log($"Crop ({cropName}) at {xTile}, {yTile} is missing currentLocation (bad GameLocation?), unable to process!", LogLevel.Trace);
                return true;
            }

            if (soil.currentLocation.numberOfObjectsWithName("Parrot Pot") == 0)
            {
                return true;
            }

            // If any farmer is in a location with a Parrot Pot and the farmer is not in bed, skip logic
            if (soil.currentLocation.farmers.Any(f => !f.isInBed))
            {
                return true;
            }

            // Get the nearby HarvestStatue, which will be placing the harvested crop into
            ParrotPot statueObj = soil.currentLocation.objects.Pairs.First(p => p.Value.Name == "Parrot Pot").Value as ParrotPot;

            if ((bool)__instance.dead)
            {
                return false;
            }

            bool success = false;
            if ((bool)__instance.forageCrop)
            {
                Object o = null;
                System.Random r2 = new System.Random((int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2 + xTile * 1000 + yTile * 2000);
                switch ((int)__instance.whichForageCrop)
                {
                    case 1:
                        o = new Object(399, 1);
                        break;
                    case 2:
                        soil.shake((float)System.Math.PI / 48f, (float)System.Math.PI / 40f, (float)(xTile * 64) < Game1.player.Position.X);
                        return false;
                }
                if (Game1.player.professions.Contains(16))
                {
                    o.Quality = 4;
                }
                else if (r2.NextDouble() < (double)((float)Game1.player.ForagingLevel / 30f))
                {
                    o.Quality = 2;
                }
                else if (r2.NextDouble() < (double)((float)Game1.player.ForagingLevel / 15f))
                {
                    o.Quality = 1;
                }
                Game1.stats.ItemsForaged += (uint)o.Stack;

                // Try to add the forage crop to the HarvestStatue's inventory
                if (statueObj.addItem(o) != null)
                {
                    // Statue is full, flag it as being eaten
                    statueObj.ateCrops = true;
                }

                return false;
            }
            else if ((int)__instance.currentPhase >= __instance.phaseDays.Count - 1 && (!__instance.fullyGrown || (int)__instance.dayOfCurrentPhase <= 0))
            {
                int numToHarvest = 1;
                int cropQuality = 0;
                int fertilizerQualityLevel = 0;
                if ((int)__instance.indexOfHarvest == 0)
                {
                    return false;
                }
                System.Random r = new System.Random(xTile * 7 + yTile * 11 + (int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame);
                switch ((int)soil.fertilizer)
                {
                    case 368:
                        fertilizerQualityLevel = 1;
                        break;
                    case 369:
                        fertilizerQualityLevel = 2;
                        break;
                    case 919:
                        fertilizerQualityLevel = 3;
                        break;
                }

                double chanceForGoldQuality = 0.2 * ((double)Game1.player.FarmingLevel / 10.0) + 0.2 * (double)fertilizerQualityLevel * (((double)Game1.player.FarmingLevel + 2.0) / 12.0) + 0.01;
                double chanceForSilverQuality = System.Math.Min(0.75, chanceForGoldQuality * 2.0);
                if (fertilizerQualityLevel >= 3 && r.NextDouble() < chanceForGoldQuality / 2.0)
                {
                    cropQuality = 4;
                }
                else if (r.NextDouble() < chanceForGoldQuality)
                {
                    cropQuality = 2;
                }
                else if (r.NextDouble() < chanceForSilverQuality || fertilizerQualityLevel >= 3)
                {
                    cropQuality = 1;
                }
                if ((int)__instance.minHarvest > 1 || (int)__instance.maxHarvest > 1)
                {
                    int max_harvest_increase = 0;
                    if (__instance.maxHarvestIncreasePerFarmingLevel.Value > 0)
                    {
                        max_harvest_increase = Game1.player.FarmingLevel / (int)__instance.maxHarvestIncreasePerFarmingLevel;
                    }
                    numToHarvest = r.Next(__instance.minHarvest, System.Math.Max((int)__instance.minHarvest + 1, (int)__instance.maxHarvest + 1 + max_harvest_increase));
                }
                if ((double)__instance.chanceForExtraCrops > 0.0)
                {
                    while (r.NextDouble() < System.Math.Min(0.9, __instance.chanceForExtraCrops))
                    {
                        numToHarvest++;
                    }
                }
                if ((int)__instance.indexOfHarvest == 771 || (int)__instance.indexOfHarvest == 889)
                {
                    cropQuality = 0;
                }

                Object harvestedItem = (__instance.programColored ? new ColoredObject(__instance.indexOfHarvest, 1, __instance.tintColor)
                {
                    Quality = cropQuality
                } : new Object(__instance.indexOfHarvest, 1, isRecipe: false, -1, cropQuality));
                if ((int)__instance.harvestMethod == 1)
                {
                    if (statueObj.addItem(harvestedItem.getOne()) != null)
                    {
                        // Statue is full, flag it as being eaten
                        statueObj.ateCrops = true;
                    }
                    success = true;
                }
                else if (statueObj.addItem(harvestedItem.getOne()) is null)
                {
                    Vector2 initialTile = new Vector2(xTile, yTile);

                    if (r.NextDouble() < Game1.player.team.AverageLuckLevel() / 1500.0 + Game1.player.team.AverageDailyLuck() / 1200.0 + 9.9999997473787516E-05)
                    {
                        numToHarvest *= 2;
                    }
                    success = true;
                }
                else
                {
                    // Statue is full, flag it as being eaten
                    statueObj.ateCrops = true;
                }
                if (success)
                {
                    if ((int)__instance.indexOfHarvest == 421)
                    {
                        __instance.indexOfHarvest.Value = 431;
                        numToHarvest = r.Next(1, 4);
                    }
                    int price = System.Convert.ToInt32(Game1.objectInformation[__instance.indexOfHarvest].Split('/')[1]);
                    harvestedItem = (__instance.programColored ? new ColoredObject(__instance.indexOfHarvest, 1, __instance.tintColor) : new Object(__instance.indexOfHarvest, 1));
                    float experience = (float)(16.0 * System.Math.Log(0.018 * (double)price + 1.0, System.Math.E));

                    for (int i = 0; i < numToHarvest - 1; i++)
                    {
                        if (statueObj.addItem(harvestedItem.getOne()) != null)
                        {
                            // Statue is full, flag it as being eaten
                            statueObj.ateCrops = true;
                        }
                    }
                    if ((int)__instance.indexOfHarvest == 262 && r.NextDouble() < 0.4)
                    {
                        Object hay_item = new Object(178, 1);
                        if (statueObj.addItem(hay_item.getOne()) != null)
                        {
                            // Statue is full, flag it as being eaten
                            statueObj.ateCrops = true;
                        }
                    }
                    else if ((int)__instance.indexOfHarvest == 771)
                    {
                        if (r.NextDouble() < 0.1)
                        {
                            Object mixedSeeds_item = new Object(770, 1);
                            if (statueObj.addItem(mixedSeeds_item.getOne()) != null)
                            {
                                // Statue is full, flag it as being eaten
                                statueObj.ateCrops = true;
                            }
                        }
                    }
                    if ((int)__instance.regrowAfterHarvest == -1)
                    {
                        return false;
                    }
                    __instance.fullyGrown.Value = true;
                    if (__instance.dayOfCurrentPhase.Value == (int)__instance.regrowAfterHarvest)
                    {
                        __instance.updateDrawMath(___tilePosition);
                    }
                    __instance.dayOfCurrentPhase.Value = __instance.regrowAfterHarvest;
                }
            }

            return false;
        }
    }
}
