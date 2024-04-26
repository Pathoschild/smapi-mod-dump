/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/seferoni/FoodPoisoning
**
*************************************************/

namespace FoodPoisoning.Common;

#region using directives

using SObject = StardewValley.Object;

#endregion

internal static class Utilities
{
	private static string NauseatedID { get; } = "25";
	internal static void ApplyNauseated(SObject foodObject)
	{
		int newDuration = GetDurationByFood(foodObject);

		Buff nauseated = new(NauseatedID)
		{
			millisecondsDuration = newDuration,
			source = "Food Poisoning",
			glow = Microsoft.Xna.Framework.Color.White
		};

		Game1.player.buffs.Apply(nauseated);
	}

	internal static int GetDurationByFood(SObject foodObject)
	{
		int duration = ModEntry.Config.BaseDuration;

		if (IsFoodHarmful(foodObject))
		{
			duration += ModEntry.Config.HarmfulDurationOffset;
		}

		return duration * 1000;
	}

	internal static int GetPercentageChanceByFood(SObject foodObject)
	{
		int chance = ModEntry.Config.BasePoisoningChance;

		if (IsFoodHarmful(foodObject))
		{
			chance += ModEntry.Config.HarmfulChanceOffset;
		}

		return chance;
	}

	internal static bool IsFoodHarmful(SObject foodObject)
	{
		if (foodObject.Edibility >= ModEntry.Config.HarmfulThreshold)
		{
			return false;
		}

		return true;
	}

	internal static bool IsFoodSafe(SObject foodObject)
	{
		if (foodObject.HasContextTag("ginger_item"))
		{
			return true;
		}

		if (foodObject.Category == SObject.CookingCategory)
		{
			return true;
		}

		if (foodObject.Category == SObject.artisanGoodsCategory)
		{
			return true;
		}

		if (foodObject.Category == SObject.FruitsCategory)
		{
			return true;
		}

		return false;
	}

	internal static void UpdateFoodConsumption(SObject foodObject)
	{
		if (IsFoodSafe(foodObject))
		{
			return;
		}

		if (Game1.random.Next(1, 100) > GetPercentageChanceByFood(foodObject))
		{
			return;
		}

		ApplyNauseated(foodObject);
	}
}