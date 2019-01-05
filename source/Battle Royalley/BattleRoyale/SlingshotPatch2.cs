using Harmony;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Network;
using StardewValley.Projectiles;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleRoyale
{
	class SlingshotPatch2 : Patch
	{
		protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(GameLocation), "UpdateWhenCurrentLocation");

		public static void Postfix(GameLocation __instance, GameTime time)
		{
			if (FreezeTime.TimeFrozen && !(Game1.shouldTimePass() || Game1.isFestival()))
			{
				int i = 0;
				while (i < __instance.projectiles.Count)
				{
					if (__instance.projectiles[i].update(time, __instance))
					{
						__instance.projectiles.RemoveAt(i);
					}
					else
					{
						i++;
					}
				}
			}
		}
	}
}
