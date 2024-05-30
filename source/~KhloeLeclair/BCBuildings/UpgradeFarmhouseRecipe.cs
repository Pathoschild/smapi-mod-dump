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
using System.Diagnostics.CodeAnalysis;

using Leclair.Stardew.BetterCrafting;
using Leclair.Stardew.Common;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;

namespace Leclair.Stardew.BCBuildings;

public class UpgradeFarmhouseRecipe : IDynamicDrawingRecipe {

	public readonly ModEntry Mod;

	private Texture2D? _Texture = null;
	private bool IsTextureLoading = false;

	public UpgradeFarmhouseRecipe(ModEntry mod) {
		Mod = mod;

		UpdateIngredients();
	}

	[MemberNotNull(nameof(Ingredients))]
	private void UpdateIngredients() {
		var bc = Mod.BCAPI!;
		List<IIngredient> ingredients = [];

		int amount;

		// If this is set, the player already paid, so just skip that
		// and make it instant when we click it.
		if (Game1.player.daysUntilHouseUpgrade.Value <= 0)
			switch (Game1.player.HouseUpgradeLevel) {
				case 0:
					amount = (int) (10_000 * (Mod.Config.CostCurrency / 100.0));
					if (amount > 0)
						ingredients.Add(bc.CreateCurrencyIngredient(CurrencyType.Money, amount));

					amount = (int) (450 * (Mod.Config.CostMaterial / 100.0));
					if (amount > 0)
						ingredients.Add(bc.CreateBaseIngredient("(O)388", amount));
					break;

				case 1:
					amount = (int) (65_000 * (Mod.Config.CostCurrency / 100.0));
					if (amount > 0)
						ingredients.Add(bc.CreateCurrencyIngredient(CurrencyType.Money, amount));

					amount = (int) (100 * (Mod.Config.CostMaterial / 100.0));
					if (amount > 0)
						ingredients.Add(bc.CreateBaseIngredient("(O)709", amount));
					break;

				case 2:
					amount = (int) (100_000 * (Mod.Config.CostCurrency / 100.0));
					if (amount > 0)
						ingredients.Add(bc.CreateCurrencyIngredient(CurrencyType.Money, amount));
					break;
			}

		var additional = Mod.GetAdditionalCost();
		if (additional is not null)
			ingredients.AddRange(additional);

		Ingredients = ingredients.ToArray();
	}


	// Identity

	public string SortValue => "zz";

	public string Name => "bcbuildings:FarmhouseUpgrade";

	public string DisplayName => I18n.Recipe_UpgradeHouse();

	public string? Description => I18n.Recipe_UpgradeHouse_Desc();

	public bool HasRecipe(Farmer who) {
		return Game1.player.HouseUpgradeLevel < 3;
	}

	public int GetTimesCrafted(Farmer who) {
		return Game1.player.HouseUpgradeLevel;
	}

	public CraftingRecipe? CraftingRecipe => null;

	// Display

	private void ResetTexture() {
		IsTextureLoading = false;
		_Texture = null;
	}

	private void LoadTexture() {
		if (IsTextureLoading)
			return;

		IsTextureLoading = true;

		var home = Utility.getHomeOfFarmer(Game1.player);
		var house = home?.GetContainingBuilding();

		if (house is null) {
			IsTextureLoading = false;
			return;
		}

		Mod.Renderer.RenderBuilding(house, tex => _Texture = tex);
	}

	public bool ShouldDoDynamicDrawing => true;

	public void Draw(SpriteBatch b, Rectangle bounds, Color color, bool ghosted, bool canCraft, float layerDepth, ClickableTextureComponent? cmp) {
		LoadTexture();

		if (cmp != null) {
			cmp.texture = Texture;
			cmp.sourceRect = SourceRectangle;

			cmp.DrawBounded(b, color, layerDepth);

			b.Draw(
				Game1.mouseCursors,
				new Vector2(
					bounds.X + (bounds.Width - 16 * 3),
					bounds.Y + (bounds.Height - 16 * 3)
				),
				new Rectangle(145, 208, 16, 16),
				color,
				0f,
				Vector2.Zero,
				3,
				SpriteEffects.None,
				1f
			);

			return;
		}

		Rectangle source = SourceRectangle;

		float width = source.Width;
		float height = source.Height;

		float bWidth = bounds.Width;
		float bHeight = bounds.Height;

		float s = Math.Min(bWidth / width, bHeight / height);

		width *= s;
		height *= s;

		float offsetX = (bWidth - width) / 2;
		float offsetY = (bHeight - height) / 2;

		b.Draw(
			Texture,
			new Vector2(bounds.X + offsetX, bounds.Y + offsetY),
			source,
			color,
			0f,
			Vector2.Zero,
			s,
			SpriteEffects.None,
			layerDepth
		);
	}

	public Texture2D Texture => _Texture ?? Game1.mouseCursors;
	public Rectangle SourceRectangle => _Texture?.Bounds ?? new Rectangle(173, 423, 16, 16);

	public int GridWidth => 2;

	public int GridHeight => 2;

	// Cost

	public int QuantityPerCraft => 1;

	public IIngredient[] Ingredients { get; private set; }

	// Creation

	public bool Stackable => false;

	public bool AllowRecycling => false;

	public Item? CreateItem() {
		return null;
	}

	public bool CanCraft(Farmer who) {
		if (who.HouseUpgradeLevel >= 3)
			return false;

		var house = Utility.getHomeOfFarmer(who);
		if (house.farmers.Any())
			return false;

		return true;
	}

	public string? GetTooltipExtra(Farmer who) {
		if (CanCraft(who))
			return null;

		var house = Utility.getHomeOfFarmer(who);
		if (house.farmers.Any())
			return I18n.Recipe_UpgradeHouse_ErrorVisitors();

		return I18n.Recipe_UpgradeHouse_ErrorUpgraded();
	}

	public void PerformCraft(IPerformCraftEvent evt) {

		var house = Utility.getHomeOfFarmer(Game1.player);
		if (house is null) {
			evt.Cancel();
			return;
		}

		// Clear the days until upgrade timer.
		if (Game1.player.daysUntilHouseUpgrade.Value >= 0)
			Game1.player.daysUntilHouseUpgrade.Value = -1;

		// Prepare to upgrade the house.
		house.moveObjectsForHouseUpgrade(Game1.player.HouseUpgradeLevel + 1);

		// Upgrade the house.
		Game1.player.HouseUpgradeLevel++;
		house.setMapForUpgradeLevel(Game1.player.HouseUpgradeLevel);

		// Post-upgrade.
		Game1.stats.checkForBuildingUpgradeAchievements();
		Game1.player.autoGenerateActiveDialogueEvent($"houseUpgrade_{Game1.player.HouseUpgradeLevel}");

		// Make sure our ingredients are up to date.
		UpdateIngredients();
		ResetTexture();

		evt.Complete();
	}

}
