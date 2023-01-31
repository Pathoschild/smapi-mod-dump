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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Leclair.Stardew.BetterCrafting.Patches;

using StardewValley;
using StardewValley.Objects;

namespace Leclair.Stardew.BetterCrafting.Models;


public class ItemEqualityComparer : IEqualityComparer<Item> {

	public static readonly ItemEqualityComparer Instance = new();

	public bool Equals(Item? first, Item? second) {
		if (first is null || second is null)
			return first == second;

		try {
			Item_Patches.OverrideStackSize = true;
			if (first.canStackWith(second))
				return true;

		} finally {
			Item_Patches.OverrideStackSize = false;
		}

		bool justCompare = false;

		// Equality for other things.

		if (first is CombinedRing || second is CombinedRing)
			return false;

		if (first is Ring && second is Ring)
			justCompare = true;

		if (first is Boots fboots && second is Boots sboots) {
			justCompare = true;
			if (fboots.indexInColorSheet.Value != sboots.indexInColorSheet.Value)
				return false;
		}

		if (first is Clothing fclothes && second is Clothing sclothes) {
			justCompare = true;
			if (fclothes.clothesColor.Value != sclothes.clothesColor.Value)
				return false;
		}

		if (first is Hat fhat && second is Hat shat) {
			return fhat.which.Value == shat.which.Value
				&& first.Name.Equals(second.Name);
		}

		// Technically we could compare tools, but we're never going to be
		// recycling those really so...

		if (justCompare)
			return first.ParentSheetIndex == second.ParentSheetIndex
				&& first.Name.Equals(second.Name);

		return false;
	}

	public int GetHashCode(Item obj) {
		return obj.GetHashCode();
	}

}
