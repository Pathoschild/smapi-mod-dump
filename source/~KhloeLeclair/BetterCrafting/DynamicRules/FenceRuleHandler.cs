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

public record struct FenceInfo(
	HashSet<string> KnownFences
);

public class FenceRuleHandler : DynamicTypeHandler<FenceInfo> {
	public override string DisplayName => I18n.Filter_Fences();

	public override string Description => I18n.Filter_Fences_About();

	public override Texture2D Texture => ItemRegistry.GetData("(O)322").GetTexture();

	public override Rectangle Source => ItemRegistry.GetData("(O)322").GetSourceRect();

	public override bool AllowMultiple => false;

	public override bool HasEditor => false;

	public override bool DoesRecipeMatch(IRecipe recipe, Lazy<Item?> item, FenceInfo state) {
		if (item.Value is not Item i)
			return false;

		return state.KnownFences.Contains(i.ItemId);
	}

	public override IClickableMenu? GetEditor(IClickableMenu parent, IDynamicRuleData data) {
		return null;
	}

	public override IFlowNode[]? GetExtraInfo(FenceInfo state) {
		return null;
	}

	public override FenceInfo ParseStateT(IDynamicRuleData data) {
		var fences = DataLoader.Fences(Game1.content).Keys.ToHashSet();
		return new FenceInfo(fences);
	}
}
