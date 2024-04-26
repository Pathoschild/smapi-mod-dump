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

public record struct FloorInfo(
	HashSet<string> KnownFloors
);

public class FloorAndPathRuleHandler : DynamicTypeHandler<FloorInfo> {
	public override string DisplayName => I18n.Filter_Floors();

	public override string Description => I18n.Filter_Floors_About();

	public override Texture2D Texture => ItemRegistry.GetData("(O)328").GetTexture();

	public override Rectangle Source => ItemRegistry.GetData("(O)328").GetSourceRect();

	public override bool AllowMultiple => false;

	public override bool HasEditor => false;

	public override bool DoesRecipeMatch(IRecipe recipe, Lazy<Item?> item, FloorInfo state) {
		if (item.Value is not Item i)
			return false;

		return state.KnownFloors.Contains(i.ItemId);
	}

	public override IClickableMenu? GetEditor(IClickableMenu parent, IDynamicRuleData data) {
		return null;
	}

	public override IFlowNode[]? GetExtraInfo(FloorInfo state) {
		return null;
	}

	public override FloorInfo ParseStateT(IDynamicRuleData data) {
		var floors = DataLoader.FloorsAndPaths(Game1.content).Values.Select(val => val.ItemId).ToHashSet();
		return new FloorInfo(floors);
	}
}
