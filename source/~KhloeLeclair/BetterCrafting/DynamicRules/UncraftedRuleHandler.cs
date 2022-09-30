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
using Leclair.Stardew.Common.UI.FlowNode;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;

namespace Leclair.Stardew.BetterCrafting.DynamicRules;

internal class UncraftedRuleHandler : IDynamicRuleHandler {

	public string DisplayName => I18n.Filter_Uncrafted();
	public string Description => I18n.Filter_Uncrafted_About();

	public Texture2D Texture => Game1.mouseCursors;

	public Rectangle Source => new(32, 672, 16, 16);

	public bool AllowMultiple => false;

	public bool HasEditor => false;

	public IClickableMenu? GetEditor(IClickableMenu parent, IDynamicRuleData type) => null;

	public object? ParseState(IDynamicRuleData type) {
		return null;
	}

	public bool DoesRecipeMatch(IRecipe recipe, Lazy<Item?> item, object? state) {
		return recipe.GetTimesCrafted(Game1.player) <= 0;
	}

}
