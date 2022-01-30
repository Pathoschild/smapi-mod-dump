/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AHilyard/DwarvishMattock
**
*************************************************/

using System.Reflection.Emit;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Tools;

namespace DwarvishMattock
{
	public class WoodsPatches
	{
		public static DynamicMethod performToolActionOriginal = null;

		public static bool performToolAction_Prefix(ref Woods __instance, Tool t, int tileX, int tileY, ref bool __result)
		{
			// Gotta have the original method available for mattock stand-ins.
			if (performToolActionOriginal == null)
			{
				return true;
			}

			// If the tool is a mattock, perform the default functionality with a standin axe so the stumps
			// can be chopped properly.
			if (t is Mattock mattock)
			{
				Axe standinAxe = mattock.asAxe();

				Point p = new Point(tileX * 64 + 32, tileY * 64 + 32);
				for (int i = __instance.stumps.Count - 1; i >= 0; i--)
				{
					if (!mattock.struckFeatures.Contains(__instance.stumps[i]) && __instance.stumps[i].getBoundingBox(__instance.stumps[i].tile.Get()).Contains(p))
					{
						mattock.struckFeatures.Add(__instance.stumps[i]);
						if (__instance.stumps[i].performToolAction(standinAxe, 1, __instance.stumps[i].tile.Get(), __instance))
						{
							__instance.stumps.RemoveAt(i);
						}
						__result = true;
						return false;
					}
				}
				__result = false;
				return false;
			}

			// Otherwise, just do the default functionality.
			return true;
		}
	}
}