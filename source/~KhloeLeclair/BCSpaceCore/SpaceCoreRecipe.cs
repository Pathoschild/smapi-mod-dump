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
using System.Linq;

using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Crafting;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SpaceCore;

using StardewValley;

using Leclair.Stardew.BetterCrafting.Models;


namespace Leclair.Stardew.BCSpaceCore {
	class SpaceCoreRecipe : IRecipe {

		public readonly CustomCraftingRecipe Recipe;

		public SpaceCoreRecipe(string name, CustomCraftingRecipe recipe, ModEntry mod) {
			Recipe = recipe;
			Name = name;

			Ingredients = recipe.Ingredients.Select<CustomCraftingRecipe.IngredientMatcher, IIngredient>(val => {
				string cls = val.GetType().FullName;

				if (cls.Equals("SpaceCore.CustomCraftingRecipe+ObjectIngredientMatcher")) {
					int? index = mod.Helper.Reflection.GetField<int>(val, "objectIndex", false)?.GetValue();
					if (index.HasValue)
						return new BaseIngredient(index.Value, val.Quantity);

				} else if (cls.Equals("DynamicGameAssets.DGACustomCraftingRecipe+DGAIngredientMatcher")) {
					var matcher = mod.Helper.Reflection.GetMethod(val, "ItemMatches", false);
					if (matcher != null)
						return new DGAIngredient(matcher, val);
				}

				return new SpaceCoreIngredient(val);

			}).ToArray();

			Item example = CreateItem();
			SortValue = example?.ParentSheetIndex ?? 0;
			QuantityPerCraft = example?.Stack ?? 1;
			Stackable = (example?.maximumStackSize() ?? 1) > 1;
		}


		// Identity

		public int SortValue { get; }

		public string Name { get; private set; }
		public string DisplayName => Recipe.Name;
		public string Description => Recipe.Description;

		public virtual int GetTimesCrafted(Farmer who) {
			// TODO: Cooking?

			if (who.craftingRecipes.ContainsKey(Name))
				return who.craftingRecipes[Name];

			return 0;
		}
		public CraftingRecipe CraftingRecipe => null;


		// Display

		public Texture2D Texture => Recipe.IconTexture;
		public Rectangle SourceRectangle => Recipe.IconSubrect ?? Texture.Bounds;

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

		public int QuantityPerCraft { get; }

		public IIngredient[] Ingredients { get; }

		// Creation

		public bool Stackable { get; }

		public Item CreateItem() {
			return Recipe.CreateResult();
		}

	}
}
