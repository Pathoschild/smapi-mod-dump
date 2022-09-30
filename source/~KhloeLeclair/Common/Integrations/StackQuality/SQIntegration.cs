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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

using GenericModConfigMenu;
using StardewModdingAPI;

using StardewMods.Common.Integrations.StackQuality;

using SObject = StardewValley.Object;

namespace Leclair.Stardew.Common.Integrations.StackQuality;

public class SQIntegration : BaseAPIIntegration<IStackQualityApi, Mod> {

	public SQIntegration(Mod self) : base(self, "furyx639.StackQuality", "1.0.0-beta.6") {

	}

	/// <summary>
	/// Consume an item.
	/// </summary>
	/// <param name="obj">The item stack to consume from.</param>
	/// <param name="amount">The quantity to consume.</param>
	/// <param name="nullified">Whether or not the item was emptied completely.</param>
	/// <param name="passed_quality">Whether or not there are remaining items at a higher quality than we're allowed to consume.</param>
	/// <param name="max_quality">The maximum quality to consume.</param>
	/// <returns>The number of items remaining to consume.</returns>
	public int ConsumeItem(SObject obj, int amount, out bool nullified, out bool passed_quality, int max_quality = int.MaxValue) {
		passed_quality = false;
		nullified = false;

		if (!TryGetStacks(obj, out int[]? stacks))
			return amount;

		nullified = true;
		bool changed = false;

		for(int quality = 0; quality < stacks.Length; quality++) {
			int qual = quality == 3 ? 4 : quality;
			int amt = stacks[quality];

			if (qual > max_quality)
				passed_quality |= amt > 0;

			else {
				int count = Math.Min(amount, amt);

				amount -= count;
				stacks[quality] = amt - count;
				changed = count > 0;
			}

			if (stacks[quality] > 0)
				nullified = false;
		}

		// Don't bother writing the new stacks if we nullified.
		if (changed && ! nullified)
			UpdateStacks(obj, stacks);

		return amount;
	}

	public int CountItem(SObject obj, out bool passed_quality, int max_quality = int.MaxValue) {
		int amount = 0;
		passed_quality = false;

		if (!TryGetStacks(obj, out int[]? stacks))
			return amount;

		for (int quality = 0; quality < stacks.Length; quality++) {
			int qual = quality == 3 ? 4 : quality;
			int amt = stacks[quality];

			if (qual > max_quality)
				passed_quality |= amt > 0;

			else
				amount += amt;
		}

		return amount;
	}

	public bool TryGetStacks(SObject obj, [NotNullWhen(true)] out int[]? stacks) {
		if (!IsLoaded) {
			stacks = null;
			return false;
		}

		return API.GetStacks(obj, out stacks);
	}

	public bool UpdateStacks(SObject obj, int[] stacks) {
		if (!IsLoaded)
			return false;

		API.UpdateStacks(obj, stacks);
		return true;
	}


}
