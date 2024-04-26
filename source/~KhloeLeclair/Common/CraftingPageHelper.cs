/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using System;
using System.Reflection;

using StardewValley;
using StardewValley.Menus;

namespace Leclair.Stardew.Common;

public static class CraftingPageHelper {

	[Obsolete("No longer required in 1.6")]
	public static bool IsCooking(this CraftingPage menu) {
		return menu.cooking;
	}

	[Obsolete("No longer required in 1.6")]
	public static bool GetHeldItem(this CraftingPage menu, out Item? item) {
		item = menu.heldItem;
		return item is not null;
	}

	[Obsolete("No longer required in 1.6")]
	public static bool SetHeldItem(this CraftingPage menu, Item? item) {
		menu.heldItem = item;
		return true;
	}
}
