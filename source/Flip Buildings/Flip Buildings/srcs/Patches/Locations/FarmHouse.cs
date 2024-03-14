/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/FlipBuildings
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using FlipBuildings.Utilities;

namespace FlipBuildings.Patches
{
	internal class FarmHousePatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(FarmHouse), nameof(FarmHouse.getPorchStandingSpot)),
				postfix: new HarmonyMethod(typeof(FarmHousePatch), nameof(GetPorchStandingSpotPostfix))
			);
		}

		private static void GetPorchStandingSpotPostfix(ref Point __result)
		{
			if (!Game1.getFarm().GetMainFarmHouse().modData.ContainsKey(ModDataKeys.FLIPPED))
				return;

			Point mainFarmHouseEntry = Game1.getFarm().GetMainFarmHouseEntry();

			mainFarmHouseEntry.X -= 2;
			__result = mainFarmHouseEntry;
		}
	}
}
