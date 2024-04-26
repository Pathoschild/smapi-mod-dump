/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using HarmonyLib;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using SVCrop = StardewValley.Crop;
using SVObject = StardewValley.Object;
using SVSeason = StardewValley.Season;

namespace Randomizer
{
    public class WildSeedAdjustments
	{
		/// <summary>
		/// This is the method to repalce the existing Crop.getRandomWildCropForSeason
		/// This will make the seed grow a crop of an actual appropriate type
		/// </summary>
		/// <param name="season">The relevant season</param>
		/// <returns>The ID of the random wild crop</returns>
		internal static string GetRandomWildCropForSeason(SVSeason season)
		{
            List<string> wildCropIDs;
			switch (season)
			{
				case SVSeason.Spring:
					wildCropIDs = ItemList.GetForagables(Seasons.Spring)
						.Where(x => x.ShouldBeForagable).Select(x => x.QualifiedId).ToList();
					break;
				case SVSeason.Summer:
					wildCropIDs = ItemList.GetForagables(Seasons.Summer)
						.Where(x => x.ShouldBeForagable).Select(x => x.QualifiedId).ToList();
					break;
				case SVSeason.Fall:
					wildCropIDs = ItemList.GetForagables(Seasons.Fall)
						.Where(x => x.ShouldBeForagable).Select(x => x.QualifiedId).ToList();
					break;
				case SVSeason.Winter:
					wildCropIDs = ItemList.GetForagables(Seasons.Winter)
						.Where(x => x.ShouldBeForagable).Select(x => x.QualifiedId).ToList();
					break;
				default:
					Globals.ConsoleWarn($"GetRandomWildCropForSeason was passed an unexpected season value: {season}. Returning the ID for horseradish.");
					return ItemList.GetQualifiedId(ObjectIndexes.WildHorseradish);
			}

			return RNG.GetRandomValueFromListUsingRNG(wildCropIDs, Game1.random);
		}

        /// <summary>
        /// The prefix path for the wild crop replacement - this will replace the original function
        /// More info on this here: https://harmony.pardeike.net/articles/patching-prefix.html
        /// </summary>
        /// <param name="__instance">The SVOBject instance (unused)</param>
        /// <param name="season">The season passed to the original function</param>
        /// <param name="__result">The value that we want the function to return</param>
        /// <returns>Whether we should fall back to the original function's code</returns>
        [HarmonyPatch(typeof(SVCrop))]
        internal static bool GetRandomWildCropForSeason_Prefix(
			SVObject __instance,
			SVSeason season,
			ref string __result)
        {
            try
            {
				__result = GetRandomWildCropForSeason(season);
                return false;
            }
            catch (Exception ex)
            {
                Globals.ConsoleError($"Failed to grow a new wild crop in {nameof(GetRandomWildCropForSeason_Prefix)}, growing the default instead.\n{ex}");
                return true;
            }
        }

        /// <summary>
        /// Replaces the Crop.getRandomWildCropForSeason method in Stardew Valley's Crop.cs 
        /// with this file's GetRandomWildCropForSeason method
        /// 
		/// Note that harmony should only be used as a last resort, so we should consider
		/// moving away from it if it's ever possible
        /// </summary>
        public static void ReplaceGetRandomWildCropForSeason()
		{
            var harmony = new Harmony(Globals.ModRef.ModManifest.UniqueID);
            harmony.Patch(
               original: AccessTools.Method(typeof(SVCrop), nameof(SVCrop.getRandomWildCropForSeason)),
               prefix: new HarmonyMethod(typeof(WildSeedAdjustments), nameof(GetRandomWildCropForSeason_Prefix))
            );
        }
	}
}
