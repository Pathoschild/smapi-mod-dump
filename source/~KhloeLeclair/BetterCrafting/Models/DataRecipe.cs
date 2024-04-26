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

using Leclair.Stardew.BetterCrafting.Integrations.SpaceCore;
using Leclair.Stardew.BetterCrafting.Patches;
using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Crafting;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;

using StardewValley;
using StardewValley.Internal;
using StardewValley.TokenizableStrings;
using StardewValley.Triggers;

namespace Leclair.Stardew.BetterCrafting.Models;

public class DataRecipe : IRecipe {

	private readonly ModEntry Mod;
	public readonly JsonRecipeData Data;
	public readonly SpriteInfo Sprite;

	public DataRecipe(ModEntry mod, JsonRecipeData data) {
		Mod = mod;
		Data = data;

		var ings = new List<IIngredient>();

		foreach (var ingredient in Data.Ingredients) {
			switch (ingredient.Type) {
				case IngredientType.Currency:
					ings.Add(new CurrencyIngredient(ingredient.Currency, ingredient.Quantity, ingredient.RecycleRate, ingredient.Condition));
					break;
				case IngredientType.Item:
					string[]? tags = ingredient.ContextTags != null && ingredient.ContextTags.Length > 0 ? ingredient.ContextTags : null;
					string? itemId = !string.IsNullOrEmpty(ingredient.ItemId) ? ingredient.ItemId : null;

					if (tags != null && itemId != null || tags != null && ingredient.Query != null || itemId != null && ingredient.Query != null )
						ings.Add(new ErrorIngredient());
					if (tags == null && itemId == null && ingredient.Query == null)
						ings.Add(new ErrorIngredient());
					else if (itemId != null && ItemRegistry.GetData(itemId) == null)
						ings.Add(new ErrorIngredient());
					else if (itemId != null && tags == null && ingredient.RecycleItem == null && ingredient.DisplayName == null && ingredient.Icon.Type == CategoryIcon.IconType.Item && ingredient.Icon.ItemId == null)
						ings.Add(new BaseIngredient(itemId, ingredient.Quantity, ingredient.RecycleRate, ingredient.Condition));
					else {
						Func<Item, bool> itemMatches;
						if (itemId != null)
							itemMatches = item => item.ItemId == itemId || item.QualifiedItemId == itemId;
						else if (tags != null)
							itemMatches = delegate (Item item) {
								foreach (string tag in tags) {
									if (!item.HasContextTag(tag))
										return false;
								}
								return true;
							};
						else if (ingredient.Query != null) {
							long id = Mod.ItemCache.GetNextCachedQueryId();

							itemMatches = delegate (Item item) {
								foreach (Item other in Mod.ItemCache.GetItems(id, ingredient.Query)) {
									if (ItemEqualityComparer.Instance.Equals(item, other))
										return true;
								}
								return false;
							};
						}
						else
							itemMatches = _ => false;

						Lazy<Item?> item = new(() => Mod.ItemCache.GetMatchingItems(itemMatches).FirstOrDefault());
						SpriteInfo? sprite = GetSpriteFromIcon(ingredient.Icon, () => item.Value) ?? SpriteHelper.GetSprite(ItemRegistry.Create("(O)0"))!;

						Func<string> displayName = !string.IsNullOrEmpty(ingredient.DisplayName)
							? () => ingredient.DisplayName
						: () => item.Value?.DisplayName ?? "???";

						Func<Item?>? recycleTo = null;
						if (ingredient.RecycleItem != null)
							recycleTo = delegate () {
								if (string.IsNullOrEmpty(ingredient.RecycleItem.Condition) || GameStateQuery.CheckConditions(ingredient.RecycleItem.Condition, Game1.currentLocation, Game1.player)) {
									ItemQueryContext ctx = new ItemQueryContext(Game1.currentLocation, Game1.player, Game1.random);
									Item result = ItemQueryResolver.TryResolveRandomItem(ingredient.RecycleItem, ctx, avoidRepeat: false, null, null, null, (query, error) => {
										Mod.Log($"Error attempting to spawn item for custom recipe {Name} with query '{query}': {error}", LogLevel.Error);
									});

									return result;
								}

								return null;
							};

						ings.Add(new MatcherIngredient(
							itemMatches,
							ingredient.Quantity,
							displayName,
							() => sprite.Texture,
							sprite.BaseSource,
							recycleTo,
							ingredient.RecycleRate,
							ingredient.Condition
						));
					}

					break;
			}
		}

		Ingredients = ings.ToArray();

		Sprite = GetSpriteFromIcon(Data.Icon, CreateItem) ?? SpriteHelper.GetSprite(ItemRegistry.Create("(O)0"))!;
	}

	private SpriteInfo? GetSpriteFromIcon(CategoryIcon icon, Func<Item?> getItem) {
		switch (icon.Type) {
			case CategoryIcon.IconType.Item:
				if (!string.IsNullOrEmpty(icon.ItemId))
					return SpriteHelper.GetSprite(ItemRegistry.Create(icon.ItemId));

				return SpriteHelper.GetSprite(getItem());

			case CategoryIcon.IconType.Texture:
				Texture2D? texture = icon.Source.HasValue ?
					SpriteHelper.GetTexture(icon.Source.Value)
					: null;

				if (!string.IsNullOrEmpty(icon.Path))
					try {
						texture = Mod.Helper.GameContent.Load<Texture2D>(icon.Path) ?? texture;
					} catch (Exception ex) {
						Mod.Log($"Unable to load texture \"{icon.Path}\" for recipe: {Name}", LogLevel.Warn, ex);
					}

				if (texture != null) {
					Rectangle rect = icon.Rect ?? texture.Bounds;
					return new SpriteInfo(
						texture,
						rect,
						baseScale: icon.Scale
					);
				}

				break;
		}

		return null;
	}

	#region IRecipe

	public bool AllowRecycling => Data.AllowRecycling;

	public string SortValue => Data.SortValue ?? Data.Id;

	public string Name => Data.Id;

	public string DisplayName => TokenParser.ParseText(Data.DisplayName ?? Data.Id);

	public string? Description => TokenParser.ParseText(Data.Description);

	public CraftingRecipe? CraftingRecipe => null;

	public Texture2D Texture => Sprite.Texture;

	public Rectangle SourceRectangle => Sprite.BaseSource;

	public int GridHeight {
		get {
			if (Data.GridSize != null)
				return Data.GridSize.Value.Y;

			Rectangle rect = SourceRectangle;
			return rect.Height > rect.Width ? 2 : 1;
		}
	}

	public int GridWidth {
		get {
			if (Data.GridSize != null)
				return Data.GridSize.Value.X;

			Rectangle rect = SourceRectangle;
			return rect.Width > rect.Height ? 2 : 1;
		}
	}

	public int QuantityPerCraft => Data.Output.Select(x => x.MinStack == -1 ? 1 : x.MinStack).Min();

	public IIngredient[]? Ingredients { get; }

	public bool Stackable => Data.AllowBulk;

	public bool CanCraft(Farmer who) {
		return true;
	}

	public Item? CreateItem() {

		ItemQueryContext ctx = new ItemQueryContext(Game1.currentLocation, Game1.player, Game1.random);
		foreach(var entry in Data.Output) {
			if ( string.IsNullOrEmpty(entry.Condition) || GameStateQuery.CheckConditions(entry.Condition, Game1.currentLocation, Game1.player) ) {
				Item result = ItemQueryResolver.TryResolveRandomItem(entry, ctx, avoidRepeat: false, null, null, null, (query, error) => {
					Mod.Log($"Error attempting to spawn item for custom recipe {Name} with query '{query}': {error}", LogLevel.Error);
				});
				if (result != null)
					return result;
			}
		}

		return null;
	}

	public void PerformCraft(IPerformCraftEvent evt) {

		if (Data.ActionsOnCraft != null) {
			foreach (string action in Data.ActionsOnCraft) {
				if (!TriggerActionManager.TryRunAction(action, out string error, out var ex)) {
					Mod.Log($"Error running action after crafting item for recipe {Name}: {error}", LogLevel.Error, ex);
				}
			}
		}

		evt.Complete();
	}

	public int GetTimesCrafted(Farmer who) {
		if (Data.IsCooking) {
			string? id = Data.Output[0].ItemId;
			if (!string.IsNullOrEmpty(id) && who.recipesCooked.TryGetValue(id, out int times))
				return times;
			return 0;

		} else if (who.craftingRecipes.ContainsKey(Name))
			return who.craftingRecipes[Name];

		return 0;
	}

	public string? GetTooltipExtra(Farmer who) {
		return null;
	}

	public bool HasRecipe(Farmer who) {
		if (Data.Default)
			return true;
		else if (Data.IsCooking)
			return who.cookingRecipes.ContainsKey(Name);
		else
			return who.craftingRecipes.ContainsKey(Name);
	}


	#endregion

}
