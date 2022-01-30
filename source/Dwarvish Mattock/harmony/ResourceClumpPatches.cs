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
	public class ResourceClumpPatches
	{
		public static DynamicMethod performToolActionOriginal = null;
		public static bool performToolAction_Prefix(ref ResourceClump __instance, Tool t, int damage, Vector2 tileLocation, GameLocation location, ref bool __result)
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
				switch (__instance.parentSheetIndex.Get())
				{
					case 600: // Stump
					case 602: // Log
					{
						// Treat the mattock as an axe for stumps.
						Axe standinAxe = mattock.asAxe();
						mattock.struckFeatures.Add(__instance);
						__result = (bool) performToolActionOriginal.Invoke(__instance, new object[] { __instance, standinAxe, damage, tileLocation, location });
						return false;
					}
					case 622: // Iridium meteorite
					case 672: // Boulder 1
					case 752: // Boulder 2
					case 754: // Boulder 3
					case 756: // Icy Boulder 1
					case 758: // Icy Boulder 2
					{
						// Treat the mattock as a pickaxe for boulders.
						Pickaxe standinPickaxe = mattock.asPickaxe();
						mattock.struckFeatures.Add(__instance);
						//standinPickaxe.DoFunction(location, (int)(__instance.currentTileLocation.X * 64), (int)(__instance.currentTileLocation.Y * 64), 1, Game1.player);
						__result = (bool) performToolActionOriginal.Invoke(__instance, new object[] { __instance, standinPickaxe, damage, tileLocation, location });
						return false;
					}
				}
			}

			// Otherwise, just do the default functionality.
			return true;
		}
	}
}