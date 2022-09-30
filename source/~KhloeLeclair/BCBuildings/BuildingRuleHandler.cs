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

using Leclair.Stardew.BetterCrafting;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;

namespace Leclair.Stardew.BCBuildings;

public class BuildingRuleHandler : IDynamicRuleHandler {

	public readonly ModEntry Mod;

	public BuildingRuleHandler(ModEntry mod) {
		Mod = mod;

		_Source = new(() => {
			if (Mod.BuildingSources.TryGetValue("Shed", out Rectangle? source)) {
				return (source.HasValue && !source.Value.IsEmpty) ?
					source.Value :
					Texture.Bounds;
			}

			return Print.Value.sourceRectForMenuView;
		});

	}

	public readonly Lazy<BluePrint> Print = new(() => new BluePrint("Shed"));
	public readonly Lazy<Rectangle> _Source;

	public string DisplayName => I18n.Filter_Name();

	public string Description => I18n.Filter_About();

	public Texture2D Texture => Print.Value.texture;

	public Rectangle Source => _Source.Value;

	public bool AllowMultiple => false;

	public bool HasEditor => false;

	public bool DoesRecipeMatch(IRecipe recipe, Lazy<Item?> item, object? state) {
		return recipe is BPRecipe || recipe is ActionRecipe;
	}

	public IClickableMenu? GetEditor(IClickableMenu parent, IDynamicRuleData data) {
		return null;
	}

	public object? ParseState(IDynamicRuleData data) {
		return null;
	}
}
