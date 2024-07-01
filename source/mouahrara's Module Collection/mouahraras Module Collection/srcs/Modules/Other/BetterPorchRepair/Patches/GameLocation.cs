/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/mouahrarasModuleCollection
**
*************************************************/

using System;
using HarmonyLib;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using mouahrarasModuleCollection.Other.BetterPorchRepair.Utilities;

namespace mouahrarasModuleCollection.Other.BetterPorchRepair.Patches
{
	internal class GameLocationPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.isCollidingPosition), new Type[] { typeof(Rectangle), typeof(xTile.Dimensions.Rectangle), typeof(bool), typeof(int), typeof(bool), typeof(Character), typeof(bool), typeof(bool), typeof(bool), typeof(bool) }),
				postfix: new HarmonyMethod(typeof(GameLocationPatch), nameof(IsCollidingPositionPostfix))
			);
		}

		private static void IsCollidingPositionPostfix(GameLocation __instance, Rectangle position, ref bool __result)
		{
			if (!ModEntry.Config.OtherBetterPorchRepair)
				return;
			if (__instance is not Farm farm || __result)
				return;

			Building farmhouse = farm.GetMainFarmHouse();

			if (farmhouse is not null && position.Intersects(new Rectangle((farmhouse.tileX.Value + (CompatibilityUtility.IsFlipBuildingsLoaded && farmhouse.modData.ContainsKey(CompatibilityUtility.flippedKey) ? farmhouse.tilesWide.Value : 0)) * Game1.tileSize, (farmhouse.tileY.Value + 3) * Game1.tileSize, 0, Game1.tileSize)))
			{
				__result = true;
			}
		}
	}
}
