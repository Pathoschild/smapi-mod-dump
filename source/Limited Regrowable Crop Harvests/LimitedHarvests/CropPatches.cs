/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Adradis/StardewMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley.TerrainFeatures;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using StardewValley.GameData.Crops;
using StardewValley.Characters;

namespace LimitedHarvests
{
    internal class CropPatches
    {
        public static String KeyCurrentHarvests = "LimitedHarvests.CurrentHarvests";
        public static String KeyMaximumHarvests = "LimitedHarvests.MaximumHarvests";
        public static String KeyOverride = "HarvestOverride";

        private enum Randomization { None, PerCrop, Total }
        private static IMonitor DebugMonitor;
        public static List<HoeDirt> CropsToDelete = new List<HoeDirt>();
        private static Random TotalRandom = new Random();

        /*************
         ** Patches **
         *************/

        public static void Patch(Harmony harmony, IMonitor monitor)
        {
            DebugMonitor = monitor;

            harmony.Patch(
                original: AccessTools.Method(typeof(HoeDirt), nameof(HoeDirt.plant)),
                postfix: new HarmonyMethod(typeof(CropPatches), nameof(Plant_Postfix))
                );

            harmony.Patch(
                original: AccessTools.Method(typeof(Crop), nameof(Crop.harvest)),
                prefix: new HarmonyMethod(typeof(CropPatches), nameof(Harvest_Prefix))
                );

            harmony.Patch(
                original: AccessTools.Method(typeof(Crop), nameof(Crop.harvest)),
                postfix: new HarmonyMethod(typeof(CropPatches), nameof(Harvest_Postfix))
                );
        }

        internal static void Harvest_Prefix(out int __state, Crop __instance)
        {
            __state = __instance.dayOfCurrentPhase.Value;
        }

        internal static void Plant_Postfix(bool __result, HoeDirt __instance, bool isFertilizer)
        {
            if (__result && !isFertilizer && __instance.crop.RegrowsAfterHarvest())
            {
                InitializeData(__instance.crop);
            }
        }

        internal static void Harvest_Postfix(ref Crop __instance, int __state, bool __result, HoeDirt soil, JunimoHarvester junimoHarvester)
        {
            if (!__instance.RegrowsAfterHarvest())
            {
                return;
            }

            if (__instance.dayOfCurrentPhase.Value != __state && __instance.fullyGrown.Value)
            {
                if (!__instance.modData.ContainsKey(KeyCurrentHarvests) || !__instance.modData.ContainsKey(KeyMaximumHarvests))
                {
                    // Fallback initialization - Should only ever fire if mod is added to an existing save.
                    InitializeData(__instance);
                }

                int currentHarvests = 1000;
                int maximumHarvests = -1000;

                Int32.TryParse(__instance.modData[KeyCurrentHarvests], out currentHarvests);
                Int32.TryParse(__instance.modData[KeyMaximumHarvests], out maximumHarvests);

                if (maximumHarvests == -1 && ModEntry.Instance.Config.AllowOverride == true)
                {
                    // Harvest override, infinite harvests so don't check further.
                    // If someone has disabled the override, it will fall through and destroy the crop (Max value of -1)
                    return;
                }

                if (junimoHarvester == null || ModEntry.Instance.Config.GentleJunimos == false)
                {
                    __instance.modData[KeyCurrentHarvests] = (currentHarvests + 1).ToString();
                }

                if (HarvestLimitReached(__instance))
                {
                    __instance.Kill();
                    __instance.updateDrawMath(__instance.tilePosition);
                    DelayedAction.functionAfterDelay( () => { DelayedCropDestroy(soil, true); }, 500);
                }
            }
        }

        private static void DelayedCropDestroy(HoeDirt soil, bool animation)
        {
            if (soil == null || soil.crop == null)
            {
                return;
            }
            if (soil.crop.dead.Value)
            {
                soil.destroyCrop(animation);
            }
        }

        private static bool HarvestLimitReached(Crop crop)
        {
            if (crop.modData.ContainsKey(KeyCurrentHarvests) &&
                crop.modData.ContainsKey(KeyMaximumHarvests))
            {
                int currentHarvests = 1000;
                int maximumHarvests = -1000;

                Int32.TryParse(crop.modData[KeyCurrentHarvests], out currentHarvests);
                Int32.TryParse(crop.modData[KeyMaximumHarvests], out maximumHarvests);

                return currentHarvests >= maximumHarvests ? true : false;
            }

            DebugMonitor.Log("Error: Attempted to check a crop that's missing data - This should not happen. Terminating crop for safety.", LogLevel.Error);
            return true;
        }

        private static void InitializeData(Crop crop)
        {
            crop.modData.TryAdd(KeyCurrentHarvests, "0");

            CropData getDays = crop.GetData();

            if (getDays == null)
            {
                DebugMonitor.Log("Error: Attempted to plant a crop whose data returned null. Defaulting to base harvests.", LogLevel.Error);
                crop.modData.TryAdd(KeyMaximumHarvests, ModEntry.Instance.Config.BaseHarvests.ToString()) ;
                return;
            }

            int numHarvests;
            switch (ModEntry.Instance.Config.HarvestCountMethod)
            {
                case "Fixed":
                    numHarvests = ModEntry.Instance.Config.BaseHarvests;
                    break;
                case "Harvests per Season":
                    numHarvests = 28 / getDays.RegrowDays;
                    break;
                case "(Hard) Harvests Per Season":
                    numHarvests = 28 / (getDays.RegrowDays * 2);
                    break;
                default:
                    DebugMonitor.Log("Error: Unable to parse Harvest Method - Defaulting to Fixed.", LogLevel.Error);
                    numHarvests = ModEntry.Instance.Config.BaseHarvests;
                    break;
            }
            
            if (ModEntry.Instance.Config.AllowOverride && getDays.CustomFields != null)
            {
                if (getDays.CustomFields.ContainsKey(KeyOverride))
                {
                    if (!Int32.TryParse(getDays.CustomFields[KeyOverride], out numHarvests))
                    {
                        DebugMonitor.Log("Error: Unable to parse override key for harvested item id: " + getDays.HarvestItemId, LogLevel.Error);
                    }

                    // -1 is an override, disables the increment of currentHarvests for a particular crop.
                    if (numHarvests == -1)
                    {
                        crop.modData.TryAdd(KeyMaximumHarvests, "-1");
                        return;
                    }
                }
            }

            int lowerBound = Math.Max(1, numHarvests - ModEntry.Instance.Config.LowerRandModifier);
            int upperBound = Math.Max(numHarvests, numHarvests + ModEntry.Instance.Config.UpperRandModifier);

            switch (ModEntry.Instance.Config.RandomizationSetting)
            {
                case "None":
                    crop.modData.TryAdd(KeyMaximumHarvests, numHarvests.ToString());
                    break;

                case "Per Crop":
                    int seed = Game1.startingGameSeed.GetHashCode() ^ Game1.stats.DaysPlayed.GetHashCode() ^ getDays.HarvestItemId.GetHashCode();
                    Random random = new Random(seed);
                    crop.modData.TryAdd(KeyMaximumHarvests, random.Next(lowerBound, upperBound + 1).ToString());
                    break;

                case "Total":
                    crop.modData.TryAdd(KeyMaximumHarvests, TotalRandom.Next(lowerBound, upperBound + 1).ToString());
                    break;

                default:
                    DebugMonitor.Log("Error: Unable to determine randomization setting - This should not happen unless someone has manually altered the config file. Defaulting to no randomization.", LogLevel.Error);
                    crop.modData.TryAdd(KeyMaximumHarvests, numHarvests.ToString());
                    break;
            }
        }
    }
}