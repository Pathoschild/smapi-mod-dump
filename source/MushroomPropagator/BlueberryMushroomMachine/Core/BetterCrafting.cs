/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/BlueberryMushroomMachine
**
*************************************************/

using BlueberryMushroomMachine.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using Object = StardewValley.Object;

namespace BlueberryMushroomMachine
{
	internal class BetterCraftingIngredient : IIngredient
	{
		private readonly int _index;
		private readonly int _quantity;

		public BetterCraftingIngredient(int index, int quantity)
		{
			this._index = index;
			this._quantity = quantity;
		}

		public bool SupportsQuality => true;

		public string DisplayName => Game1.objectInformation.TryGetValue(this._index, out string value) ? value.Split('/')[4] : null;

		public Texture2D Texture => Game1.objectSpriteSheet;

		public Rectangle SourceRectangle => Game1.getSourceRectForStandardTileSheet(
			tileSheet: Game1.objectSpriteSheet,
			tilePosition: this._index,
			width: Game1.smallestTileSize,
			height: Game1.smallestTileSize);

		public int Quantity => this._quantity;

		public int GetAvailableQuantity(Farmer who, IList<Item> items, IList<IInventory> inventories, int maxQuality)
		{
			List<IList<Item>> lists = inventories.Where((IInventory i) => i.IsValid() && i.CanExtractItems()).Select((IInventory i) => i.GetItems()).ToList();
			lists.Add(who.Items);
			int count = ModEntry.CraftingAPI.CountItem(
				predicate: this.IsItemOk,
				who: who,
				items: lists.SelectMany((IList<Item> list) => list),
				maxQuality: maxQuality);
			return count;
		}

		public void Consume(Farmer who, IList<IInventory> inventories, int maxQuality, bool lowQualityFirst)
		{
			ModEntry.CraftingAPI.ConsumeItems(
				items: new (Func<Item, bool>, int)[] { (this.IsItemOk, this._quantity) },
				who: who,
				inventories: inventories,
				maxQuality: maxQuality,
				lowQualityFirst: lowQualityFirst);
		}

		private bool IsItemOk(Item item)
		{
			return item is not null && item.ParentSheetIndex == this._index && (item is not Object || !((Object)item).bigCraftable.Value);
		}
	}

	internal class BetterCraftingRecipe : IRecipe
	{
		public int SortValue => 0;

		public string Name => ModValues.PropagatorInternalName;

		public string DisplayName => Propagator.PropagatorDisplayName;

		public string Description => Propagator.PropagatorDescription;

		public bool HasRecipe(Farmer who)
		{
			return who.craftingRecipes.ContainsKey(ModValues.PropagatorInternalName);
		}

		public int GetTimesCrafted(Farmer who)
		{
			return who.craftingRecipes.TryGetValue(ModValues.PropagatorInternalName, out int value) ? value : 0;
		}

		public CraftingRecipe CraftingRecipe => new(name: ModValues.PropagatorInternalName);

		public Texture2D Texture => ModEntry.MachineTexture;

		public Rectangle SourceRectangle => new(location: Point.Zero, size: Propagator.MachineSize);

		public int GridHeight => 2;

		public int GridWidth => 1;

		public int QuantityPerCraft => 1;
		
		public IIngredient[] Ingredients => this.GetIngredients();
		
		public bool Stackable => true;

		public bool CanCraft(Farmer who)
		{
			return true;
		}

		public string GetTooltipExtra(Farmer who)
		{
			return null;
		}

		public Item CreateItem()
		{
			return new Propagator();
		}

		private IIngredient[] GetIngredients()
		{
			List<int> values = ModValues.RecipeDataFormat.Split('/')[0].Split(' ').ToList().ConvertAll(int.Parse);
			List<IIngredient> ingredients = new(capacity: values.Count / 2);
			for (int i = 0; i < values.Count; i += 2)
			{
				ingredients.Add(new BetterCraftingIngredient(index: values[i], quantity: values[i + 1]));
			}
			return ingredients.ToArray();
		}
	}

	internal class BetterCraftingRecipeProvider : IRecipeProvider
	{
		public int RecipePriority => int.MaxValue;

		public IRecipe GetRecipe(CraftingRecipe recipe)
		{
			return recipe.name != ModValues.PropagatorInternalName ? null : new BetterCraftingRecipe();
		}

		public bool CacheAdditionalRecipes => true;

		public IEnumerable<IRecipe> GetAdditionalRecipes(bool cooking)
		{
			return null;
		}
	}
}