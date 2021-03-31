/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using Harmony;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using TheLion.Common.Extensions;

namespace TheLion.AwesomeProfessions
{
	internal class HoeDirtApplySpeedIncreasesPatch : BasePatch
	{
		private const int _speedGroId = 465, _deluxeSpeedGroId = 466, _hyperSpeedGroId = 918;

		/// <summary>Construct an instance.</summary>
		internal HoeDirtApplySpeedIncreasesPatch() { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(HoeDirt), name: "applySpeedIncreases"),
				prefix: new HarmonyMethod(GetType(), nameof(HoeDirtApplySpeedIncreasesPrefix))
			);
		}

		#region harmony patches
		/// <summary>Patch to globalize Agriculturist crop growth speed bonus.</summary>
		protected static bool HoeDirtApplySpeedIncreasesPrefix(ref HoeDirt __instance, Farmer who)
		{
			if (__instance.crop == null)
				return false; // don't run original logic

			bool anyPlayerIsAgriculturist = Utility.AnyPlayerHasProfession("agriculturist", out int n);
			bool shouldApplyPaddyBonus = __instance.currentLocation != null && __instance.paddyWaterCheck(__instance.currentLocation, __instance.currentTileLocation);
			
			if (!(__instance.fertilizer.Value.AnyOf(_speedGroId, _deluxeSpeedGroId, _hyperSpeedGroId) || anyPlayerIsAgriculturist || shouldApplyPaddyBonus))
				return false; // don't run original logic

			__instance.crop.ResetPhaseDays();
			int totalDaysOfCropGrowth = 0;
			for (int i = 0; i < __instance.crop.phaseDays.Count - 1; ++i)
				totalDaysOfCropGrowth += __instance.crop.phaseDays[i];

			float speedIncrease = __instance.fertilizer.Value switch
			{
				_speedGroId => 0.1f,
				_deluxeSpeedGroId => 0.25f,
				_hyperSpeedGroId => 0.33f,
				_ => 0f
			};

			if (shouldApplyPaddyBonus) speedIncrease += 0.25f;

			if (anyPlayerIsAgriculturist) speedIncrease += 0.1f * n;

			int daysToRemove = (int)Math.Ceiling(totalDaysOfCropGrowth * speedIncrease);
			int tries = 0;
			while (daysToRemove > 0 && tries < 3)
			{
				for (int i = 0; i < __instance.crop.phaseDays.Count; ++i)
				{
					if ((i > 0 || __instance.crop.phaseDays[i] > 1) && __instance.crop.phaseDays[i] != 99999)
					{
						--__instance.crop.phaseDays[i];
						--daysToRemove;
					}
					
					if (daysToRemove <= 0) break;
				}
				++tries;
			}

			return false; // don't run original logic
		}
		#endregion harmony patches
	}
}
