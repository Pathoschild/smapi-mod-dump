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

using Leclair.Stardew.BetterCrafting.Models;
using Leclair.Stardew.Common.Crafting;
using Leclair.Stardew.Common.UI.FlowNode;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

using SObject = StardewValley.Object;

namespace Leclair.Stardew.BetterCrafting.DynamicRules;

public class LightRuleHandler : IDynamicRuleHandler {
	public string DisplayName => I18n.Filter_Light();

	public string Description => I18n.Filter_Light_About();

	public Texture2D Texture => Game1.objectSpriteSheet;

	public Rectangle Source => Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 93, 16, 16);

	public bool AllowMultiple => false;

	public bool HasEditor => false;

	public bool DoesRecipeMatch(IRecipe recipe, Lazy<Item?> item, object? state) {
		// TODO: List of vanilla torches

		if (item.Value is not SObject sobj)
			return false;

		if (sobj.isLamp.Value)
			return true;

		if (sobj is Torch)
			return true;

		if (sobj.Name.Equals("Bonfire"))
			return true;

		if (sobj is Furniture furn && (furn.furniture_type.Value == 14 || furn.furniture_type.Value == 16))
			return true;

		if (sobj.ParentSheetIndex == 93 || sobj.ParentSheetIndex == 94 || sobj.ParentSheetIndex == 95)
			return true;

		if (!sobj.bigCraftable.Value && sobj.ParentSheetIndex == 746)
			return true;

		return false;
	}

	public IClickableMenu? GetEditor(IClickableMenu parent, IDynamicRuleData type) {
		return null;
	}

	public IFlowNode[]? GetExtraInfo(object? state) {
		return null;
	}

	public object? ParseState(IDynamicRuleData type) {
		return null;
	}
}
