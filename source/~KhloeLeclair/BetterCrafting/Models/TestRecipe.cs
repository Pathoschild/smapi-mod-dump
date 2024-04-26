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

using Leclair.Stardew.Common.Crafting;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Leclair.Stardew.BetterCrafting.Models;

internal class TestRecipe : IRecipe {

	public TestRecipe() {
		Ingredients = new IIngredient[] {
			new ErrorIngredient(),
			new BaseIngredient(-777, 1),
			new BaseIngredient(-2, 1),
			new BaseIngredient(-4, 1),
			new BaseIngredient(-5, 1),
			new BaseIngredient(-6, 1),
			new BaseIngredient(-7, 1),
			new BaseIngredient(-8, 1),
			new BaseIngredient(-12, 1),
			new BaseIngredient(-14, 1),
			new BaseIngredient(-19, 1),
			new BaseIngredient(-20, 1),
			new BaseIngredient(-74, 1),
			new BaseIngredient(-75, 1),
			new BaseIngredient(-3, 1),
			new BaseIngredient(-79, 1),
			new BaseIngredient(-80, 1),
			new BaseIngredient(-81, 1),
			new BaseIngredient(-1, 1),
			new CurrencyIngredient(CurrencyType.Money, 10),
			new CurrencyIngredient(CurrencyType.FestivalPoints, 10),
			new CurrencyIngredient(CurrencyType.ClubCoins, 10),
			new CurrencyIngredient(CurrencyType.QiGems, 10),
		};
	}

	public string SortValue => "42069";

	public string Name => "test:recipe:invalid";

	public string DisplayName => "Test Recipe";

	public string? Description => "This recipe exists to test ingredients, and is not otherwise valid and doesn't do anything.";

	public bool AllowRecycling => true;

	public CraftingRecipe? CraftingRecipe => null;

	public Texture2D Texture => Game1.mouseCursors;

	public Rectangle SourceRectangle => new(268, 470, 16, 16);

	public int GridHeight => 1;

	public int GridWidth => 1;

	public int QuantityPerCraft => 50;

	public IIngredient[]? Ingredients { get; }

	public bool Stackable => true;

	public bool CanCraft(Farmer who) {
		return false;
	}

	public Item? CreateItem() {
		return ItemRegistry.Create("(O)THISdoesNOTexistEVER");
	}

	public int GetTimesCrafted(Farmer who) {
		return 42069;
	}

	public string? GetTooltipExtra(Farmer who) {
		return "This is tool-tip extra.";
	}

	public bool HasRecipe(Farmer who) {
		return true;
	}
}
