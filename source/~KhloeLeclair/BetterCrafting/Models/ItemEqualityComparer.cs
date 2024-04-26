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
		// If either one is null, they're only equal if they're both null.
		if (first is null || second is null)
			return first == second;

		// Now, the easiest check in 1.6.
		if (first.QualifiedItemId != second.QualifiedItemId)
			return false;

		// SObject specific check for preserved items.
		SObject? fobj = first as SObject;
		if (fobj is not null) {
			if (second is not SObject sobj)
				return false;

			// Make sure they're preserving (or not) the same parent.
			if (fobj.preservedParentSheetIndex.Value != sobj.preservedParentSheetIndex.Value)
				return false;
		}

		// Use the built-in canStackWith method. We need to patch maximumStackSize first
		// though, just to be sure about things.
		try {
			Item_Patches.OverrideStackSize = true;
			if (! first.canStackWith(second))
				return false;

		} finally {
			Item_Patches.OverrideStackSize = false;
		}

		// If we got here, they're probably the same.
		return true;
	}

	public int GetHashCode(Item obj) {
		return obj.GetHashCode();
	}

}
