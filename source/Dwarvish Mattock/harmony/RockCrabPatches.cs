/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AHilyard/DwarvishMattock
**
*************************************************/

using StardewValley;
using StardewValley.Monsters;
using Netcode;
using StardewValley.Audio;
using Microsoft.Xna.Framework;

namespace DwarvishMattock
{
	public class RockCrabPatches
	{
		public static bool hitWithTool_Prefix(ref RockCrab __instance, Tool t, ref bool __result, ref bool ___waiter, ref NetBool ___shellGone, ref NetInt ___shellHealth, NetBool ___isStickBug)
		{
			if (___isStickBug.Value)
			{
				__result = false;
				return false;
			}

			// Affect the rock crab like a pickaxe would if this is a mattock.
			if (t is Mattock && t.getLastFarmerToUse() != null && ___shellHealth.Value > 0)
			{
				__instance.currentLocation.playSound("hammer", null, null, SoundContext.Default);

				___shellHealth.Value = ___shellHealth.Value - 1;
				__instance.shake(500);
				___waiter = false;
				__instance.moveTowardPlayerThreshold.Value = 3;
				__instance.setTrajectory(Utility.getAwayFromPlayerTrajectory(__instance.GetBoundingBox(), t.getLastFarmerToUse()));
				if (___shellHealth.Value <= 0)
				{
					Point tile = __instance.TilePoint;
					___shellGone.Value = true;
					__instance.moveTowardPlayer(-1);
					__instance.currentLocation.playSound("stoneCrack", null, null, SoundContext.Default);
					Game1.createRadialDebris(__instance.currentLocation, 14, tile.X, tile.Y, Game1.random.Next(2, 7), false, -1, false, null);
					Game1.createRadialDebris(__instance.currentLocation, 14, tile.X, tile.Y, Game1.random.Next(2, 7), false, -1, false, null);
				}
				__result = true;
				return false;
			}

			// Otherwise, just do the default functionality.
			return true;
		}
	}
}