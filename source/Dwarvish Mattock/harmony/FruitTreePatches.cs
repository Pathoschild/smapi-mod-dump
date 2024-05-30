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
using StardewValley.Tools;
using StardewValley.TerrainFeatures;

namespace DwarvishMattock
{
	public class FruitTreePatches
	{
		public static DynamicMethod performToolActionOriginal = null;
		public static bool performToolAction_Prefix(ref FruitTree __instance, Tool t, int explosion, Vector2 tileLocation, ref bool __result)
		{
			// Gotta have the original method available for mattock stand-ins.
			if (performToolActionOriginal == null)
			{
				return true;
			}

			// If the tool is a mattock and this object is one that requires a specific type of tool that is supported,
			// run the function with a stand-in tool instead.
			if (t is Mattock mattock && !mattock.struckFeatures.Contains(__instance))
			{ 
				// Treat the mattock as an axe for stumps.
				Axe standinAxe = mattock.AsAxe();
				mattock.struckFeatures.Add(__instance);
				__result = (bool) performToolActionOriginal.Invoke(__instance, new object[] { __instance, standinAxe, explosion, tileLocation });
				return false;
			}

			// Otherwise, just do the default functionality.
			return true;
		}
	}
}