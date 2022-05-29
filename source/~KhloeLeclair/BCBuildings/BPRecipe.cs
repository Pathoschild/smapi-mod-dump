/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using System.Collections.Generic;

using Leclair.Stardew.Common;
using Leclair.Stardew.BetterCrafting;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Locations;

namespace Leclair.Stardew.BCBuildings; 

public class BPRecipe : IRecipe {

	public readonly ModEntry Mod;
	public readonly BluePrint Blueprint;

	public readonly string? RawQuery;
	public readonly GameStateQuery.ParsedQuery? Query;

	public BPRecipe(BluePrint blueprint, ModEntry mod) {
		Mod = mod;
		Name = $"blueprint:{blueprint.name}";
		Blueprint = blueprint;

		if (mod.ApiBlueprints.TryGetValue(blueprint.name, out var data))
			RawQuery = data.Item3;

		if (!string.IsNullOrEmpty(RawQuery))
			Query = GameStateQuery.ParseConditions(RawQuery);

		List<IIngredient> ingredients = new();

		if (blueprint.itemsRequired != null)
			foreach (var entry in blueprint.itemsRequired)
				ingredients.Add(mod.BCAPI!.CreateBaseIngredient(entry.Key, entry.Value));

		if (blueprint.moneyRequired > 0)
			ingredients.Add(mod.BCAPI!.CreateCurrencyIngredient(CurrencyType.Money, blueprint.moneyRequired));

		Ingredients = ingredients.ToArray();

		if (mod.BuildingSources.TryGetValue(blueprint.name, out Rectangle? source)) {
			SourceRectangle = (source.HasValue && !source.Value.IsEmpty) ? source.Value : Blueprint.texture.Bounds;
		} else
			SourceRectangle = Blueprint.sourceRectForMenuView;
	}

	// Identity

	public int SortValue { get; }
	public string Name { get; }
	public string DisplayName => Blueprint.displayName;
	public string Description => Blueprint.description;

	public virtual bool HasRecipe(Farmer who) {
		return true;
	}

	public virtual int GetTimesCrafted(Farmer who) {
		return 0;
	}

	public CraftingRecipe? CraftingRecipe => null;

	// Display

	public Texture2D Texture => Blueprint.texture;
	public Rectangle SourceRectangle { get; }

	public int GridHeight {
		get {
			Rectangle rect = SourceRectangle;
			if (rect.Height > rect.Width)
				return 2;
			return 1;
		}
	}

	public int GridWidth {
		get {
			Rectangle rect = SourceRectangle;
			if (rect.Width > rect.Height)
				return 2;
			return 1;
		}
	}

	// Cost

	public int QuantityPerCraft => 1;
	public IIngredient[] Ingredients { get; }

	// Creation

	public bool Stackable => false;

	public Item? CreateItem() {
		return null;
	}

	public bool CanCraft(Farmer who) {
		if (who.currentLocation is not BuildableGameLocation bgl)
			return false;

		if (Blueprint.isUpgrade()) {
			return bgl.isBuildingConstructed(Blueprint.nameOfBuildingToUpgrade);
		}

		if (Query.HasValue && !Query.Value.Evaluate())
			return false;

		return true;
	}

	public string? GetTooltipExtra(Farmer who) {
		if (who.currentLocation is not BuildableGameLocation bgl)
			return I18n.Error_NotBuildable();

		if (Blueprint.isUpgrade() && !bgl.isBuildingConstructed(Blueprint.nameOfBuildingToUpgrade)) {
			string other = new BluePrint(Blueprint.nameOfBuildingToUpgrade).displayName;
			return I18n.Error_CantUpgrade(other);
		}

		if (Query.HasValue && !Query.Value.Evaluate())
			return I18n.Error_UnmetCondition();

		return null;
	}

	public void PerformCraft(IPerformCraftEvent evt) {

		var menu = new BuildMenu(Blueprint, Blueprint.isUpgrade() ? ActionType.Upgrade : ActionType.Build, evt, Mod);
		var old_menu = Game1.activeClickableMenu;

		Game1.activeClickableMenu = menu;

		menu.exitFunction = () => {
			Game1.activeClickableMenu = old_menu;
			evt.Cancel();
		};
	}

}
