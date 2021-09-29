/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/dtomlinson-ga/StatsAsTokens
**
*************************************************/

using HarmonyLib;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;

namespace StatsAsTokens
{
	internal class HarmonyPatches
	{
		/*********
		** FoodEatenToken patch
		*********/

		public static void FoodEatenPatch()
		{
			try
			{
				Harmony harmony = new(Globals.Manifest.UniqueID);
				harmony.Patch(
					original: typeof(Farmer).GetMethod("eatObject"),
					prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(eatObject_Prefix))
				);

				Globals.Monitor.Log("Patched eatObject() successfully");
			}
			catch (Exception ex)
			{
				Globals.Monitor.Log($"Exception encountered while patching method {nameof(eatObject_Prefix)}: {ex}");
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Harmony patch - match original method naming convention")]
		public static void eatObject_Prefix(Farmer __instance, StardewValley.Object o)
		{
			string foodID = o.parentSheetIndex.ToString();
			Farmer f = __instance;

			string pType;
			if (f.IsMainPlayer && f.IsLocalPlayer)
			{
				pType = "hostPlayer";
			}
			else if (f.IsLocalPlayer)
			{
				pType = "localPlayer";
			}
			else
			{
				return;
			}

			Dictionary<string, Dictionary<string, int>> foodDict = FoodEatenToken.foodEatenDict.Value;
			foodDict[pType][foodID] = foodDict[pType].ContainsKey(foodID) ? foodDict[pType][foodID] + 1 : 1;
		}


		/*********
		** TreesFelledToken patch
		*********/

		public static void TreeFelledPatch()
		{
			try
			{
				Harmony harmony = new(Globals.Manifest.UniqueID);
				harmony.Patch(
					original: AccessTools.Method(typeof(Tree), "performTreeFall"),
					prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(performTreeFall_Prefix))
				);

				Globals.Monitor.Log("Patched performTreeFall() successfully");
			}
			catch (Exception ex)
			{
				Globals.Monitor.Log($"Exception encountered while patching method {nameof(performTreeFall_Prefix)}: {ex}");
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Harmony patch - match original method naming convention")]
		public static void performTreeFall_Prefix(Tree __instance, Tool t)
		{
			Farmer owner = t?.getLastFarmerToUse();

			if (owner == null)
			{
				return;
			}

			string pType;
			if (owner.IsMainPlayer)
			{
				pType = "hostPlayer";
			}
			else if (owner.IsLocalPlayer)
			{
				pType = "localPlayer";
			}
			else
			{
				return;
			}

			string treeType = __instance.treeType.ToString();

			// condense palm trees (palm and palm2) into one entry
			if (treeType == "9")
			{
				treeType = "6";
			}

			Dictionary<string, Dictionary<string, int>> treeDict = TreesFelledToken.treesFelledDict.Value;
			treeDict[pType][treeType] = treeDict[pType].ContainsKey(treeType) ? treeDict[pType][treeType] + 1 : 1;
		}

	}
}
