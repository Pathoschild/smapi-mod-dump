/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Linq;

using Leclair.Stardew.Common.Integrations;

using StardewValley;
using StardewValley.TerrainFeatures;

using UltimateFertilizer;

namespace Leclair.Stardew.CloudySkies.Integrations.UltimateFertilizer;

public class UFIntegration : BaseAPIIntegration<IUltimateFertilizerApi, ModEntry> {

	public UFIntegration(ModEntry mod) : base(mod, "fox_white25.ultimate_fertilizer", "1.1.5") { }


	public bool ApplyFertilizer(HoeDirt dirt, string fertilizerId, Farmer who) {
		if (IsLoaded)
			return API.ApplyFertilizerOnDirt(dirt, fertilizerId, who);

		if (!dirt.CanApplyFertilizer(fertilizerId))
			return false;

		dirt.fertilizer.Value = fertilizerId;
		dirt.applySpeedIncreases(who);
		return true;
	}

	public bool RemoveFertilizer(HoeDirt dirt, string? fertilizerId, Farmer who) {
		if (dirt.fertilizer.Value is null)
			return false;

		if (!IsLoaded) {
			if (!string.IsNullOrEmpty(fertilizerId) && dirt.fertilizer.Value != fertilizerId)
				return false;

			dirt.fertilizer.Value = null;
			dirt.applySpeedIncreases(who);
			return true;
		}

		if (string.IsNullOrEmpty(fertilizerId)) {
			dirt.fertilizer.Value = null;
			dirt.applySpeedIncreases(who);
			return true;
		}

		string[] applied = dirt.fertilizer.Value.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		if (applied.Length == 0 || !applied.Contains(fertilizerId))
			return false;

		dirt.fertilizer.Value = string.Join('|', applied.Where(x => x != fertilizerId));
		dirt.applySpeedIncreases(who);
		return true;
	}

}
