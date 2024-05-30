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

using Leclair.Stardew.BetterCrafting;
using Leclair.Stardew.Common;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;

namespace Leclair.Stardew.BCBuildings;

public class RenovateFarmhouseRecipe : IDynamicDrawingRecipe {

	public readonly ModEntry Mod;

	public HouseRenovation Renovation;

	private Texture2D? _Texture = null;
	private bool IsTextureLoading = false;

	public RenovateFarmhouseRecipe(ModEntry mod, HouseRenovation renovation) {
		Mod = mod;
		Renovation = renovation;

		Name = $"bcbuildings:renovation:{Renovation.Name}";

		Ingredients = Renovation.Price == 0 ? [] : [
			Mod.BCAPI!.CreateCurrencyIngredient(CurrencyType.Money, Renovation.Price)
		];
	}

	// Identity

	public string SortValue => "zz";

	public string Name { get; }

	public string DisplayName => Renovation.DisplayName;

	public string? Description => Renovation.getDescription();

	public bool HasRecipe(Farmer who) {
		return Game1.player.HouseUpgradeLevel >= 2;
	}

	public int GetTimesCrafted(Farmer who) {
		return 0;
	}

	public CraftingRecipe? CraftingRecipe => null;

	// Display

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

	public void Draw(SpriteBatch b, Rectangle bounds, Color _, bool ghosted, bool canCraft, float layerDepth, ClickableTextureComponent? cmp) {

		Color color = Color.White;
		if (cmp is not null && cmp.scale > cmp.baseScale)
			color = Color.Wheat;

		Color textColor = Game1.textColor;

		if (!canCraft) {
			color = Color.DimGray * 0.4f;
			textColor *= 0.4f;
		}

		if (ghosted)
			color = Color.Black * 0.35f;

		RenderHelper.DrawBox(b, Game1.mouseCursors, new Rectangle(384, 396, 15, 15), bounds.X, bounds.Y, bounds.Width, bounds.Height, color, scale: 2, drawShadow: false, draw_layer: layerDepth);

		if (ghosted)
			return;

		LoadTexture();
		if (_Texture is not null) {
			int height = bounds.Height - 32;

			b.Draw(
				_Texture,
				new Vector2(bounds.X + 16, bounds.Y + 16),
				_Texture.Bounds,
				color,
				0f,
				Vector2.Zero,
				((float) height / _Texture.Bounds.Width),
				SpriteEffects.None,
				MathF.BitIncrement(layerDepth)
			);
		}

		if (bounds.Width < 64)
			return;

		Vector2 size;
		string toDraw = DisplayName;
		bool trimmed = false;

		while (true) {
			if (toDraw.Length == 0)
				return;

			size = Game1.smallFont.MeasureString(toDraw + (trimmed ? "..." : ""));

			if (size.X > bounds.Width - 56) {
				trimmed = true;
				int idx = toDraw.LastIndexOf(' ');
				if (idx == -1)
					toDraw = toDraw.Substring(0, toDraw.Length / 2);
				else
					toDraw = toDraw.Substring(0, idx);
			} else
				break;
		}

		b.DrawString(Game1.smallFont, toDraw + (trimmed ? "..." : ""), new Vector2(bounds.X + 56, bounds.Y + (bounds.Height - size.Y) / 2), textColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
	}

	public Texture2D Texture => Game1.mouseCursors;
	public Rectangle SourceRectangle => new Rectangle(173, 423, 16, 16);

	public int GridWidth => 10;

	public int GridHeight => 1;

	// Cost

	public int QuantityPerCraft => 1;

	public IIngredient[] Ingredients { get; }

	// Creation

	public bool Stackable => false;

	public bool AllowRecycling => false;

	public Item? CreateItem() {
		return null;
	}

	public bool CanCraft(Farmer who) {
		return who.HouseUpgradeLevel >= 2;
	}

	public string? GetTooltipExtra(Farmer who) {
		return null;
	}

	public void PerformCraft(IPerformCraftEvent evt) {

		evt.Complete();

		HouseRenovation.OnPurchaseRenovation(Renovation, Game1.player, 1);

	}


}
