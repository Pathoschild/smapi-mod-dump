/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System.Linq;

using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Crafting;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Leclair.Stardew.BetterCrafting.Models {
	public class BaseRecipe : IRecipe, IRecipeSprite {

		public readonly ModEntry Mod;
		public readonly CraftingRecipe Recipe;

		public BaseRecipe(ModEntry mod, CraftingRecipe recipe) {
			Mod = mod;
			Recipe = recipe;
			Ingredients = recipe.recipeList
				.Select(val => new BaseIngredient(val.Key, val.Value))
				.ToArray();

			Stackable = CreateItem().maximumStackSize() > 1;
		}

		public virtual bool Stackable { get; }

		public virtual int SortValue => Recipe.itemToProduce[0];

		public virtual string Name => Recipe.name;

		public virtual string DisplayName => Recipe.DisplayName;

		public virtual string Description => Recipe.description;

		public virtual int GetTimesCrafted(Farmer who) {
			if (Recipe.isCookingRecipe) {
				int idx = Recipe.getIndexOfMenuView();
				if (who.recipesCooked.ContainsKey(idx))
					return who.recipesCooked[idx];

			} else if (who.craftingRecipes.ContainsKey(Name))
					return who.craftingRecipes[Name];

			return 0;
		}

		public virtual SpriteInfo Sprite => SpriteHelper.GetSprite(CreateItem(), Mod.Helper);

		public virtual Texture2D Texture => Recipe.bigCraftable ?
			Game1.bigCraftableSpriteSheet :
			Game1.objectSpriteSheet;

		public virtual Rectangle SourceRectangle => Recipe.bigCraftable ?
			Game1.getArbitrarySourceRect(Texture, 16, 32, Recipe.getIndexOfMenuView()) :
			Game1.getSourceRectForStandardTileSheet(Texture, Recipe.getIndexOfMenuView(), 16, 16);

		public virtual int GridHeight => Recipe.bigCraftable ? 2 : 1;

		public virtual int GridWidth => 1;

		public virtual int QuantityPerCraft => Recipe.numberProducedPerCraft;

		public virtual IIngredient[] Ingredients { get; private set; }

		public virtual Item CreateItem() {
			return Recipe.createItem();
		}

		public virtual CraftingRecipe CraftingRecipe => Recipe;


	}
}
