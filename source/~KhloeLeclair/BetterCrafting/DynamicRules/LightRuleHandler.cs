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

namespace Leclair.Stardew.BetterCrafting.DynamicRules;

public class LightRuleHandler : IDynamicRuleHandler {
	public string DisplayName => I18n.Filter_Light();

	public string Description => I18n.Filter_Light_About();

	public Texture2D Texture => Game1.objectSpriteSheet;

	public Rectangle Source => Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 93, 16, 16);

	public bool AllowMultiple => false;

	public bool HasEditor => false;

	public bool DoesRecipeMatch(IRecipe recipe, Lazy<Item?> item, object? state) {
		if (item.Value is not SObject sobj)
			return false;

		// The basics that should work for 99% of items.
		if (sobj.isLamp.Value || sobj.HasContextTag("light_source"))
			return true;

		// Furniture Items
		if (sobj is Furniture furn && furn.furniture_type.Value is int ftype && (ftype == Furniture.fireplace || ftype == Furniture.torch || ftype == Furniture.sconce))
			return true;

		// Jack-O-Lantern is not tagged.
		if (sobj.QualifiedItemId == "(O)746")
			return true;

		return false;
	}

	public IClickableMenu? GetEditor(IClickableMenu parent, IDynamicRuleData type) {
		return null;
	}

	public object? ParseState(IDynamicRuleData type) {
		return null;
	}
}
