/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley.TerrainFeatures;
using System;
using System.Reflection;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class FruitTreeDayUpdatePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal FruitTreeDayUpdatePatch()
		{
			Original = typeof(FruitTree).MethodNamed(nameof(FruitTree.dayUpdate));
			Postfix = new HarmonyMethod(GetType(), nameof(FruitTreeDayUpdatePostfix));
		}

		#region harmony patches

		/// <summary>Patch to increase Abrorist fruit tree growth speed.</summary>
		[HarmonyPostfix]
		private static void FruitTreeDayUpdatePostfix(ref FruitTree __instance)
		{
			try
			{
				if (Util.Professions.DoesAnyPlayerHaveProfession("Arborist", out _) && __instance.daysUntilMature.Value % 4 == 0)
					--__instance.daysUntilMature.Value;
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod().Name}:\n{ex}", LogLevel.Error);
			}
		}

		#endregion harmony patches
	}
}