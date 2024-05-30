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
using StardewValley.Tools;

namespace DwarvishMattock
{
	public class ObjectPatches
	{
		public static bool performToolAction_Prefix(ref Object __instance, Tool t, ref bool __result)
		{
			// If the tool is a mattock and this object is one that requires a specific type of tool that is supported,
			// run the function with a stand-in tool instead.
			if (t is Mattock mattock && !mattock.struckObjects.Contains(__instance))
			{
				// Treat the mattock as a pickaxe for stones.
				if (__instance.name.Equals("Stone") || __instance.name.Equals("Boulder"))
				{
					Pickaxe standinPickaxe = mattock.AsPickaxe();
					standinPickaxe.DoFunction(__instance.Location, (int)(__instance.TileLocation.X * 64 + 32), (int)(__instance.TileLocation.Y * 64 + 32), 1, Game1.player);
					mattock.struckObjects.Add(__instance);
					return false;
				}
				// Treat the mattock as an axe for twigs.
				else if (__instance.name.Contains("Twig"))
				{
					Axe standinAxe = mattock.AsAxe();
					standinAxe.DoFunction(__instance.Location, (int)(__instance.TileLocation.X * 64 + 32), (int)(__instance.TileLocation.Y * 64 + 32), 1, Game1.player);
					mattock.struckObjects.Add(__instance);
					return false;
				}
			}

			// Otherwise, just do the default functionality.
			return true;
		}
	}
}