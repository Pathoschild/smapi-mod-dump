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

using Leclair.Stardew.BetterCrafting.Models;
using Leclair.Stardew.Common.Crafting;
using Leclair.Stardew.Common.UI.FlowNode;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;

namespace Leclair.Stardew.BetterCrafting.DynamicRules;

public class FurnitureRuleHandler : IDynamicRuleHandler {
	public string DisplayName => I18n.Filter_Furniture();

	public string Description => I18n.Filter_Furniture_About();

	public Texture2D Texture => ItemRegistry.GetData("(F)294").GetTexture();

	public Rectangle Source => ItemRegistry.GetData("(F)294").GetSourceRect();

	public bool AllowMultiple => false;

	public bool HasEditor => false;

	public bool DoesRecipeMatch(IRecipe recipe, Lazy<Item?> item, object? state) {
		if (item.Value is not Item i)
			return false;

		return i.TypeDefinitionId == ItemRegistry.type_furniture;
	}

	public IClickableMenu? GetEditor(IClickableMenu parent, IDynamicRuleData data) {
		return null;
	}

	public object? ParseState(IDynamicRuleData data) {
		return null;
	}
}
