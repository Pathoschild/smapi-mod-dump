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

using Leclair.Stardew.BetterCrafting.Models;
using Leclair.Stardew.Common.Crafting;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace Leclair.Stardew.BetterCrafting.DynamicRules;

public class StorageRuleHandler : IDynamicRuleHandler {

	public static readonly string[] VANILLA_CHESTS = new string[] {
		"(BC)130", // Chest
		"(BC)165", // Auto-Grabber
		"(BC)216", // Mini-Fridge
		"(BC)232", // Stone Chest
		"(BC)248", // Mini Shipping Bin
		"(BC)256", // Junimo Chest
		"(BC)275", // Hopper
		"(BC)BigChest",
		"(BC)BigStoneChest",
		"(O)913",  // Enricher (for Sprinkler)
	};

	public string DisplayName => I18n.Filter_Storage();

	public string Description => I18n.Filter_Storage_About();

	public Texture2D Texture => ItemRegistry.GetData("(BC)130").GetTexture();

	public Rectangle Source => ItemRegistry.GetData("(BC)130").GetSourceRect();

	public bool AllowMultiple => false;

	public bool HasEditor => false;

	public bool DoesRecipeMatch(IRecipe recipe, Lazy<Item?> item, object? state) {
		if (item.Value is not SObject sobj)
			return false;

		if (sobj is Chest || sobj is StorageFurniture)
			return true;

		if (sobj.bigCraftable.Value
			&& Game1.bigCraftableData.TryGetValue(sobj.ItemId, out var data)
			&& data.CustomFields is not null
			&& data.CustomFields.TryGetValue("furyx639.ExpandedStorage/Enabled", out string? isExpanded)
			&& isExpanded == "true"
		)
			return true;

		string qid = sobj.QualifiedItemId;
		foreach (string chest in VANILLA_CHESTS)
			if (chest == qid) return true;

		return false;
	}

	public IClickableMenu? GetEditor(IClickableMenu parent, IDynamicRuleData data) {
		return null;
	}

	public object? ParseState(IDynamicRuleData data) {
		return null;
	}
}
