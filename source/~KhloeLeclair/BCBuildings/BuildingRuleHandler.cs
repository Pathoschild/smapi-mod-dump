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
using StardewValley.GameData.Buildings;
using StardewValley.Menus;

namespace Leclair.Stardew.BCBuildings;

public class BuildingRuleHandler : IDynamicRuleHandler {

	public readonly ModEntry Mod;

	public readonly BuildingData Building;

	private Lazy<Texture2D> _Texture;

	public BuildingRuleHandler(ModEntry mod) {
		Mod = mod;

		var buildings = DataLoader.Buildings(Game1.content);
		if (!buildings.TryGetValue("Shed", out var building))
			building = buildings.First().Value;

		Building = building;
		_Texture = new Lazy<Texture2D>(() => Mod.Helper.GameContent.Load<Texture2D>(Building.Texture));
	}

	public string DisplayName => I18n.Filter_Name();

	public string Description => I18n.Filter_About();

	public Texture2D Texture => _Texture.Value;

	public Rectangle Source => Building.SourceRect.IsEmpty ? Texture.Bounds : Building.SourceRect;

	public bool AllowMultiple => false;

	public bool HasEditor => false;

	public bool DoesRecipeMatch(IRecipe recipe, Lazy<Item?> item, object? state) {
		return recipe.Name?.StartsWith("bcbuildings:") ?? false;
	}

	public IClickableMenu? GetEditor(IClickableMenu parent, IDynamicRuleData data) {
		return null;
	}

	public object? ParseState(IDynamicRuleData data) {
		return null;
	}
}
