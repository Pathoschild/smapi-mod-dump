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
	public class TerrainFeaturePatches
	{
		public static DynamicMethod performToolActionOriginal = null;
		public static bool performToolAction_Prefix(ref TerrainFeature __instance, Tool t, int damage, Vector2 tileLocation, ref bool __result)
		{
			ModEntry.M.Log("performToolAction_Prefix A", StardewModdingAPI.LogLevel.Info);
			// Gotta have the original method available for mattock stand-ins.
			if (performToolActionOriginal == null)
			{
				return true;
			}

			ModEntry.M.Log("performToolAction_Prefix B", StardewModdingAPI.LogLevel.Info);

			// If the tool is a mattock and this object is one that requires a specific type of tool that is supported,
			// run the function with a stand-in tool instead.
			if (t is Mattock mattock)
			{ 
				if (__instance is Tree || __instance is FruitTree || __instance is Bush || __instance is GiantCrop || __instance is Flooring)
				{
					ModEntry.M.Log("Struck a feature:" + __instance.GetType().ToString(), StardewModdingAPI.LogLevel.Info);

					// Treat the mattock as an axe for various terrain features.
					Axe standinAxe = mattock.AsAxe();
					__result = (bool) performToolActionOriginal.Invoke(__instance, new object[] { __instance, standinAxe, damage, tileLocation });
					return false;
				}
				else if (__instance is ResourceClump clump)
				{
					switch (clump.parentSheetIndex.Get())
					{
						case 600: // Stump
						case 602: // Log
						{
							// Treat the mattock as an axe for stumps.
							Axe standinAxe = mattock.AsAxe();
							__result = (bool) performToolActionOriginal.Invoke(__instance, new object[] { __instance, standinAxe, damage, tileLocation });
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
							Pickaxe standinPickaxe = (t as Mattock).AsPickaxe();
							standinPickaxe.DoFunction(__instance.Location, (int)(__instance.Tile.X * 64), (int)(__instance.Tile.Y * 64), 1, Game1.player);
							return false;
						}
					}
				}
			}

			// Otherwise, just do the default functionality.
			return true;
		}
	}
}