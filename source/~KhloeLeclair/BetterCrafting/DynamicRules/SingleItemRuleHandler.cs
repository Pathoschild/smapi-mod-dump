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
using StardewValley.ItemTypeDefinitions;
using StardewValley.Menus;

namespace Leclair.Stardew.BetterCrafting.DynamicRules;

public class SingleItemRuleHandler : IDynamicRuleHandler {

	public readonly string ItemId;
	public readonly Lazy<Item> Item;
	public readonly Lazy<ParsedItemData> Data;

	public SingleItemRuleHandler(int itemId) : this($"{itemId}") { }

	public SingleItemRuleHandler(string itemId) {
		ItemId = itemId;
		Item = new Lazy<Item>(() => ItemRegistry.Create(ItemId, 1));
		Data = new Lazy<ParsedItemData>(() => ItemRegistry.GetDataOrErrorItem(ItemId));
	}

	public string DisplayName => I18n.Filter_Buff(Data.Value.DisplayName);
	public string Description => I18n.Filter_Buff_About(Data.Value.DisplayName);

	public Texture2D Texture => Data.Value.GetTexture();

	public Rectangle Source => Data.Value.GetSourceRect();

	public bool AllowMultiple => false;

	public bool HasEditor => false;

	public IClickableMenu? GetEditor(IClickableMenu parent, IDynamicRuleData type) => null;

	public object? ParseState(IDynamicRuleData type) {
		return null;
	}

	public bool DoesRecipeMatch(IRecipe recipe, Lazy<Item?> item, object? state) {
		return item.Value is not null && Item.Value.canStackWith(item.Value);
	}
}
