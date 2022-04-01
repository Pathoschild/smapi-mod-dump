/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Rafseazz/Ridgeside-Village-Mod
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using StardewValley;
using Microsoft.Xna.Framework;

namespace RidgesideVillage
{
    //Corrects the location name in the "X has begun in Y" message
    internal static class Fish
    {
        private static IModHelper Helper { get; set; }



        const int CURIOSITY_LURE = 856;
        const double BASE_CATCH_CHANCE = 0.3;
        const double CATCH_CHANCE_WITH_CURIOLURE = 0.75;  // Curiosity Lure increases chance by 7%
        const int MIN_FISHING = 7;

        static Dictionary<string, int> fishIDs = new Dictionary<string, int>();

        public static void ApplyPatch(Harmony harmony, IModHelper helper)
        {
            Helper = helper;

            harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.getFish)),
               postfix: new HarmonyMethod(typeof(Fish), nameof(Fish.GetFish_Postfix))
               );
        }

        public static void GetFish_Postfix(GameLocation __instance, float millisecondsAfterNibble, int bait, int waterDepth, Farmer who, double baitPotency, Vector2 bobberTile, string locationName, ref StardewValley.Object __result)
        {
            try
            {
                string nameToUse = locationName ?? __instance.Name;

                double catchChance =
                    (who.CurrentTool is StardewValley.Tools.FishingRod rod && rod.getBobberAttachmentIndex() == CURIOSITY_LURE)
                    ? CATCH_CHANCE_WITH_CURIOLURE
                    : BASE_CATCH_CHANCE
                    ;

                int replaceByFishID = -1;
                switch (nameToUse)
                {
                    // Custom locations are added to the game without their prefixes
                    case "Custom_Ridgeside_RidgesideVillage":
                        int fishID = GetFishID("Sockeye Salmon");
                        if (who.FishingLevel >= MIN_FISHING && !CheckCaughtBefore(who, fishID) && new Rectangle(71, 93, 3, 3).Contains((int)bobberTile.X, (int)bobberTile.Y)
                            && Game1.currentSeason.Equals("fall") && Game1.IsRainingHere(Game1.currentLocation))
                        {
                            replaceByFishID = fishID;
                        }
                        break;
                    case "Custom_Ridgeside_RidgeFalls":
                        fishID = GetFishID("Waterfall Snakehead");
                        if (who.FishingLevel >= MIN_FISHING && !CheckCaughtBefore(who, fishID) && new Rectangle(53, 11, 4, 4).Contains((int)bobberTile.X, (int)bobberTile.Y)
                            && (Game1.currentSeason.Equals("spring") || Game1.currentSeason.Equals("summer")) && Game1.timeOfDay >= 2000)
                        {
                            replaceByFishID = fishID;
                        }
                        break;
                    case "Custom_Ridgeside_RidgeForest":
                        fishID = GetFishID("Deep Ridge Angler");
                        if (who.FishingLevel >= MIN_FISHING && !CheckCaughtBefore(who, fishID) && new Rectangle(67, 30, 5, 6).Contains((int)bobberTile.X, (int)bobberTile.Y)
                            && (Game1.currentSeason.Equals("winter") && Game1.timeOfDay >= 1200))
                        {
                            replaceByFishID = fishID;
                        }
                        break;
                    default:
                        return;
                }

                if(replaceByFishID != -1 && Game1.random.NextDouble() <= catchChance)
                {
                    __result = new StardewValley.Object(replaceByFishID, 1);
                    return;
                }
                return;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed in {nameof(GetFish_Postfix)}:\n{ex}");
            }
        }

        private static bool CheckCaughtBefore(Farmer who, int fishID)
        {
            if (Helper.ModRegistry.IsLoaded("DaLion.ImmersiveProfessions") && who.professions.Contains(100 + Farmer.angler))
            {
                return false;
            }
            if (who.fishCaught.ContainsKey(fishID))
            {
                return true;
            }
            return false;
        }

        private static int GetFishID(string name)
        {
            if(fishIDs.TryGetValue(name, out int fishID))
            {
                return fishID;
            }
            else
            {
                fishID = ExternalAPIs.JA.GetObjectId(name);
                if(fishID != -1)
                {
                    fishIDs[name] = fishID;
                    return fishID;
                }
                else
                {
                    Log.Error($"Fish {name} not found.");
                    return -1;
                }
            }
        }
    }

}
