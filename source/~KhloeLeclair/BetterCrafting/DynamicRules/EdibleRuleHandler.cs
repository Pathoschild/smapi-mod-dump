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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;

namespace Leclair.Stardew.BetterCrafting.DynamicRules;

public class EdibleRuleHandler : IDynamicRuleHandler {

	public string DisplayName => I18n.Filter_Edible();

	public string Description => I18n.Filter_Edible_About();

	public Texture2D Texture => ItemRegistry.GetData("(O)403").GetTexture();

	public Rectangle Source => ItemRegistry.GetData("(O)403").GetSourceRect();

	public bool AllowMultiple => false;

	public bool HasEditor => false;

	public bool DoesRecipeMatch(IRecipe recipe, Lazy<Item?> item, object? state) {
		if (item.Value is not SObject sobj)
			return false;

		return sobj.Edibility != -300;
	}

	public IClickableMenu? GetEditor(IClickableMenu parent, IDynamicRuleData type) {
		return null;
	}

	public object? ParseState(IDynamicRuleData type) {
		return null;
	}

}
