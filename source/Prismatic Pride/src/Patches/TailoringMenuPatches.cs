/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/prismaticpride
**
*************************************************/

using Harmony;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;

namespace PrismaticPride
{
	internal static class TailoringMenuPatches
	{
		private static IModHelper Helper => ModEntry.Instance.Helper;
		private static IMonitor Monitor => ModEntry.Instance.Monitor;
		private static ColorData ColorData => ModEntry.Instance.colorData;
		private static HarmonyInstance Harmony => ModEntry.Instance.harmony;

		public static void Apply ()
		{
			Harmony.Patch (
				original: AccessTools.Method (typeof (TailoringMenu),
					nameof (TailoringMenu.CraftItem)),
				prefix: new HarmonyMethod (typeof (TailoringMenuPatches),
					nameof (TailoringMenuPatches.CraftItem_Prefix))
			);
		}

#pragma warning disable IDE1006

		public static bool CraftItem_Prefix (TailoringMenu __instance,
			ref Item __result, Item left_item, Item right_item)
		{
			try
			{
				// First/left item must be boots.
				if (left_item is not Boots boots)
					return true;

				// Second/right item must be a prismatic shard.
				if (!Utility.IsNormalObjectAtParentSheetIndex (right_item, 74))
					return true;

				// Use the Prismatic Boots index with Leather Boots as fallback.
				int index = ModEntry.Instance.bootsSheetIndex;
				if (index == -1) index = 506;

				// In case the boots are already prismatic, return them
				// unmodified to preserve any stats already on them.
				if (boots.indexInTileSheet.Value != index)
				{
					// Clone the original boots as a stat source, since the stats
					// will change as soon as the index is changed.
					Boots statSource = new (boots.indexInTileSheet.Value);
					boots.indexInTileSheet.Value = index;
					boots.applyStats (statSource); // includes reloadData
				}

				// Return the modified original boots, since otherwise they
				// would remain unconsumed.
				__result = boots;
				return false;
			}
			catch (Exception e)
			{
				Monitor.Log ($"Failed in {nameof (CraftItem_Prefix)}:\n{e}",
					LogLevel.Error);
				Monitor.Log (e.StackTrace, LogLevel.Trace);
			}
			return true;
		}
	}
}
