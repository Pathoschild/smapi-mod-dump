using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;

namespace PumpkinKing
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {    
            //Harmony patcher
            //https://github.com/KirbyLink/PumpkinKing

            var harmony = HarmonyInstance.Create("com.github.kirbylink.pumpkinking");
            
            //Planting seeds
            var plantOriginal = typeof(HoeDirt).GetMethod("plant");
            var plantPrefix = helper.Reflection.GetMethod(typeof(PumpkinKing.PlantSeeds), "Prefix").MethodInfo;
            var plantPostfix = helper.Reflection.GetMethod(typeof(PumpkinKing.PlantSeeds), "Postfix").MethodInfo;

            //Placing Scarecrow
            var placeOriginal = typeof(StardewValley.Object).GetMethod("placementAction");
            var placePrefix = helper.Reflection.GetMethod(typeof(PumpkinKing.PlacePumpkinKing), "Prefix").MethodInfo;
            var placePostfix = helper.Reflection.GetMethod(typeof(PumpkinKing.PlacePumpkinKing), "Postfix").MethodInfo;
            
            //Removing Scarecrow
            var removeOriginal = typeof(Tool).GetMethod("DoFunction");
            var removePrefix = helper.Reflection.GetMethod(typeof(PumpkinKing.PickUpPumpkinKing), "Prefix").MethodInfo;
            var removePostfix = helper.Reflection.GetMethod(typeof(PumpkinKing.PickUpPumpkinKing), "Postfix").MethodInfo;

            harmony.Patch(plantOriginal, new HarmonyMethod(plantPrefix), new HarmonyMethod(plantPostfix));
            harmony.Patch(placeOriginal, new HarmonyMethod(placePrefix), new HarmonyMethod(placePostfix));
            harmony.Patch(removeOriginal, new HarmonyMethod(removePrefix), new HarmonyMethod(removePostfix));
            
        }

        public static void NewGrowthDays(GameLocation location, Vector2 relativeVector, bool isScarecrow, int offset, Farmer who)
        {
            //Find all HoeDirt within radius
            //relativeVector is Vector of HoeDirt having seed planted or scarecrow being placed

            //Create a list of KVP's of Hoedirts that are growing pumpkins
            List<KeyValuePair<Vector2, HoeDirt>> hoeDirtList = new List<KeyValuePair<Vector2, HoeDirt>>();
            if (isScarecrow)
            {
                foreach (KeyValuePair<Vector2, TerrainFeature> pair in location.terrainFeatures.Pairs)
                {
                    TerrainFeature terrain = pair.Value;
                    Crop crop = (terrain is HoeDirt ? (terrain as HoeDirt).crop : null);
                    bool isGrowingPumpkin = crop != null && crop.indexOfHarvest.Value == 276 && !crop.fullyGrown.Value;

                    if (isGrowingPumpkin && Vector2.Distance(pair.Key, relativeVector) < 9.0)
                    {
                        hoeDirtList.Add(new KeyValuePair<Vector2, HoeDirt>(pair.Key, (pair.Value as HoeDirt)));
                    }
                }
            }
            else
            {
                HoeDirt dirt = (location.terrainFeatures[relativeVector] is HoeDirt ? location.terrainFeatures[relativeVector] as HoeDirt : null);
                if (dirt != null)
                {
                    hoeDirtList.Add(new KeyValuePair<Vector2, HoeDirt>(relativeVector, dirt));
                }
            }
            
            //Set new phaseDays
            foreach (KeyValuePair<Vector2, HoeDirt> dirtPair in hoeDirtList)
            {
                //Get number of scarecrows in range of each HoeDirt
                int numScarecrows = 0;
                foreach (KeyValuePair<Vector2, StardewValley.Object> objPair in location.objects.Pairs)
                {
                    if (objPair.Value.Name.Contains("The Pumpkin King") && Vector2.Distance(dirtPair.Key, objPair.Key) < 9.0)
                    {
                        numScarecrows++;
                    }
                }

                numScarecrows += offset;
                Crop crop = dirtPair.Value.crop;
                int fertlizerType = dirtPair.Value.fertilizer.Value;
                int pumpkinGrowthDays = 13;
                int growthDayChange = 0;
            
                // Get total change for all modifiers
                // Max change is 60% (5 day growth)
                float totalPercentChange = 0.25f * numScarecrows;
                totalPercentChange += (fertlizerType == 465 ? 0.1f : (fertlizerType == 466 ? 0.25f : 0));
                totalPercentChange += (who.professions.Contains(5) ? 0.1f : 0);
                growthDayChange = (int)Math.Ceiling((double)pumpkinGrowthDays * (double)Math.Min(0.6f, totalPercentChange));

                // Reset pumpkin phase days to normal amounts;
                IList<int> phaseDays = new List<int> { 1, 2, 3, 4, 3, 99999 };
                crop.phaseDays.Set(phaseDays);

                // Set new phaseDays
                while (growthDayChange > 0)
                {
                    int i = 0;
                    while (i < crop.phaseDays.Count - 1)
                    {
                        if (crop.phaseDays[i] > 1)
                        {
                            crop.phaseDays[i]--;
                            --growthDayChange;
                        }
                        if (growthDayChange <= 0 || (i == crop.phaseDays.Count - 2 && crop.phaseDays[i] == 1))
                        {
                            break;
                        }
                        i++;
                    }
                }
            }
        }
    }
    
    public static class PlantSeeds
    {

        /* Check if planting fertilizer or is not pumpkins */
        static void Prefix(HoeDirt __instance, bool isFertilizer, int index, ref bool __state)
        {
            if (__instance != null)
            {
                __state = isFertilizer || index != 490;
            }
        }

        static void Postfix(int tileX, int tileY, Farmer who, GameLocation location, ref bool __state, bool __result)
        {
            //If state is true, we didn't make it past first two returns in plant() or this crop isn't a pumpkin
            //If result is false, we couldn't plant the seed for some reason
            //If result is true, we have already updated this HoeDirt.crop
            if (!__state && __result)
            {
                Vector2 hoeDirtVector = new Vector2((float) tileX, (float) tileY);
                ModEntry.NewGrowthDays(location, hoeDirtVector, false, 0, who);
            }
        }
    }

    public static class PlacePumpkinKing
    {
        
        /* Check if item being placed is The Pumpkin King */
        static void Prefix(StardewValley.Object __instance, GameLocation location, ref bool __state)
        {
            if (__instance != null && location.IsFarm)
            {
                __state = __instance != null && __instance.Name.Contains("The Pumpkin King");
            }
        }

        static void Postfix(StardewValley.Object __instance, GameLocation location, Farmer who, int x, int y, bool __state, bool __result)
        {
            /*  
             *  Alter any growing pumpkins in the area by reducing growth rate by 25%
            */

            //If state is true, we we are placing The Pumpkin King
            //If result is true, we have placed the scarecrow
            if (__state && __result)
            {
                Vector2 scarecrowVector = new Vector2((float) (x / 64), (float) (y / 64));
                ModEntry.NewGrowthDays(location, scarecrowVector, true, 0, who);                
            }
        }
    }

    public static class PickUpPumpkinKing
    {

        /* Check if item being removed is The Pumpkin King */
        static void Prefix(Tool __instance, GameLocation location, int x, int y, ref bool __state)
        {
            if (__instance != null && location.IsFarm)
            {
                Vector2 vector = new Vector2((float) (x / 64), (float) (y / 64));
                StardewValley.Object vectorObject = null;
                if (location.objects.ContainsKey(vector))
                {
                    vectorObject = location.objects[vector];
                }
                __state = vectorObject != null && vectorObject.Name.Contains("The Pumpkin King");
            }
        }

        static void Postfix(GameLocation location, int x, int y, Farmer who, bool __state)
        {
            /*  
             *  Remove 25% growth bonus from surrounding pumpkins
            */

            //If state is true, we removed a Pumpkin King on the farm
            if (__state)
            {
                //Find all HoeDirt within radius
                Vector2 scarecrowVector = new Vector2((float)(x / 64), (float)(y / 64));
                ModEntry.NewGrowthDays(location, scarecrowVector, true, -1, who);
            }
        }
    }
}
