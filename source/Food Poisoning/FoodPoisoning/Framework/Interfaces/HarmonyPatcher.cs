/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/seferoni/FoodPoisoning
**
*************************************************/

namespace FoodPoisoning.Interfaces;

#region using directives

using FoodPoisoning.Common;
using SObject = StardewValley.Object;

#endregion

internal static class HarmonyPatcher
{
	private static IMonitor Monitor { get; set; } = null!;

	internal static void InitialiseMonitor(IMonitor monitorInstance)
	{
		Monitor = monitorInstance;
	}

	internal static void DoneEating_PostFix(Farmer __instance)
	{
		try
		{
			var foodItem = __instance.itemToEat;

			if (foodItem is null)
			{
				return;
			}

			Utilities.UpdateFoodConsumption((SObject)foodItem);
		}
		catch (Exception ex)
		{
			Monitor.Log($"[Food Poisoning] Method patch failed! \nReason: {ex}", LogLevel.Error);
		}
	}
}