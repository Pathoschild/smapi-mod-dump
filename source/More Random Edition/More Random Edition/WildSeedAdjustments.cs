/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SVCrop = StardewValley.Crop;

namespace Randomizer
{
	public class WildSeedAdjustments
	{

		/// <summary>
		/// This is the method to repalce the existing Crop.getRandomWildCropForSeason
		/// This will make the seed grow a crop of an actual appropriate type
		/// </summary>
		/// <param name="season">The season string: "spring", "summer", "fall", or "winter"</param>
		/// <returns>The ID of the random wild crop</returns>
		public int GetRandomWildCropForSeason(string season)
		{
			List<int> wildCropIDs;
			switch (season)
			{
				case "spring":
					wildCropIDs = ItemList.GetForagables(Seasons.Spring)
						.Where(x => x.ShouldBeForagable).Select(x => x.Id).ToList();
					break;
				case "summer":
					wildCropIDs = ItemList.GetForagables(Seasons.Summer)
						.Where(x => x.ShouldBeForagable).Select(x => x.Id).ToList();
					break;
				case "fall":
					wildCropIDs = ItemList.GetForagables(Seasons.Fall)
						.Where(x => x.ShouldBeForagable).Select(x => x.Id).ToList();
					break;
				case "winter":
					wildCropIDs = ItemList.GetForagables(Seasons.Winter)
						.Where(x => x.ShouldBeForagable).Select(x => x.Id).ToList();
					break;
				default:
					Globals.ConsoleWarn($"GetRandomWildCropForSeason was passed an unexpected season value: {season}. Returning the ID for horseradish.");
					return (int)ObjectIndexes.WildHorseradish;
			}

			return Globals.RNGGetRandomValueFromList(wildCropIDs, true);
		}

		/// <summary>
		/// Replaces the Crop.getRandomWildCRopForSeason method in Stardew Valley's Bundle.cs with this file's GetRandomWildCropForSeason method
		/// NOTE: THIS IS UNSAFE CODE, CHANGE WITH EXTREME CAUTION
		/// </summary>
		public static void ReplaceGetRandomWildCropForSeason()
		{
			MethodInfo methodToReplace = typeof(SVCrop).GetMethod("getRandomWildCropForSeason");
			MethodInfo methodToInject = typeof(WildSeedAdjustments).GetMethod("GetRandomWildCropForSeason");
			Globals.RepointMethod(methodToReplace, methodToInject);
		}
	}
}
