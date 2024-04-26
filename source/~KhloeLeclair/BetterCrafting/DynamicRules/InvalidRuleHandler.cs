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

namespace Leclair.Stardew.BetterCrafting.DynamicRules;

public class InvalidRuleHandler : IDynamicRuleHandler, IExtraInfoRuleHandler {
	public string DisplayName => I18n.Filter_Error();

	public string Description => I18n.Filter_Error_About();

	public Texture2D Texture => Game1.mouseCursors;

	public Rectangle Source => new(268, 470, 16, 16);

	public bool AllowMultiple => true;

	public bool HasEditor => false;

	public bool DoesRecipeMatch(IRecipe recipe, Lazy<Item?> item, object? state) {
		return false;
	}

	public IClickableMenu? GetEditor(IClickableMenu parent, IDynamicRuleData data) {
		return null;
	}

	public IFlowNode[]? GetExtraInfo(object? state) {
		if (state is not IDynamicRuleData data)
			return null;

		return [
			new TextNode(data.Id, new Common.UI.TextStyle(shadow: false))
		];
	}

	public object? ParseState(IDynamicRuleData data) {
		return data;
	}


}
