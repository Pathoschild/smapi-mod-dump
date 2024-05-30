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

using HarmonyLib;

using StardewValley;

namespace Leclair.Stardew.BetterCrafting.Patches;

public static class Utility_Patches {

	private static ModEntry? Mod;

	internal static void Patch(ModEntry mod) {
		Mod = mod;

		try {
			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(Utility), nameof(Utility.trashItem)),
				postfix: new HarmonyMethod(typeof(Utility_Patches), nameof(TrashItem__Postfix))
			);
		} catch (Exception ex) {
			mod.Log($"Unable to patch Utility.trashItem. Recovering trashed items will not be possible.", StardewModdingAPI.LogLevel.Warn, ex);
		}
	}

	internal static void AddItemToTrash(Item item) {
		var inv = Mod?.TrashedItems?.Value;
		if (inv is null || item.Stack <= 0)
			return;

		int toRemove = -1;
		bool isRemovingNull = false;

		for (int i = 0; i < inv.Count; i++) {
			Item? existing = inv[i];
			if (existing != null && existing.canStackWith(item)) {
				int addable = Math.Min(item.Stack, existing.maximumStackSize() - existing.Stack);
				if (addable > 0) {
					existing.Stack += addable;
					item.Stack -= addable;
					if (item.Stack <= 0)
						return;
				}
			} else if (toRemove == -1) {
				toRemove = i;
				isRemovingNull = existing is null;
			} else if (existing is null && !isRemovingNull) {
				toRemove = i;
				isRemovingNull = true;
			}
		}

		// Are we just replacing a null slot?
		if (isRemovingNull && toRemove != -1) {
			inv[toRemove] = item;
			return;
		}

		// Alright, the inventory is full, so remove something if we can.
		if (inv.Count > 36 && toRemove != -1)
			inv.RemoveAt(toRemove);

		// If we have space, add the remaining item.
		if (inv.Count < 36)
			inv.Add(item);
	}

	private static void TrashItem__Postfix(Item item) {
		try {
			AddItemToTrash(item);
		} catch (Exception ex) {
			Mod?.Log($"Error adding trashed item to list of trashed items: {ex}", StardewModdingAPI.LogLevel.Warn, once: true);
		}
	}

}
