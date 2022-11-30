/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Reflection;

namespace StardewRoguelike.Patches
{
    internal class OreClumpsPatch : Patch
    {
		protected override PatchDescriptor GetPatchDescriptor() => new(typeof(MineShaft), "tryToAddOreClumps");
		public static bool Prefix(MineShaft __instance)
        {
			Random mineRandom = (Random)__instance.GetType().GetField("mineRandom", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(__instance)!;

			if (!(mineRandom.NextDouble() < 0.55))
				return false;

			Vector2 endPoint = __instance.getRandomTile();
			for (int tries = 0; tries < 1 || mineRandom.NextDouble() < 0.25; tries++)
			{
				if (__instance.isTileLocationTotallyClearAndPlaceable(endPoint) && __instance.isTileOnClearAndSolidGround(endPoint) && __instance.doesTileHaveProperty((int)endPoint.X, (int)endPoint.Y, "Diggable", "Back") is null)
				{
                    SObject ore = new(endPoint, 764, "Stone", canBeSetDown: true, canBeGrabbed: false, isHoedirt: false, isSpawnedObject: false);
					ore.MinutesUntilReady = 8;
					Utility.recursiveObjectPlacement(ore, (int)endPoint.X, (int)endPoint.Y, 0.949999988079071, 0.30000001192092896, __instance, "Dirt", (ore.ParentSheetIndex == 668) ? 1 : 0, 0.05000000074505806, (ore.ParentSheetIndex != 668) ? 1 : 2);
				}
				endPoint = __instance.getRandomTile();
			}

			return false;
		}
    }
}
