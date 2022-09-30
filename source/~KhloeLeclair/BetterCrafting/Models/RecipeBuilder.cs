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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;

using Leclair.Stardew.BetterCrafting.Integrations.SpaceCore;
using Leclair.Stardew.Common.Crafting;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Leclair.Stardew.BetterCrafting.Models;

public class RecipeBuilder : IRecipeBuilder {

	public CraftingRecipe? Recipe { get; }
	public string Name { get; }

	private int? sortValue;
	private Func<string>? displayName;
	private Func<string?>? description;
	private Func<Farmer, bool>? hasRecipe;
	private Func<Farmer, int>? timesCrafted;
	private Func<Texture2D>? texture;
	private Func<Rectangle?>? source;
	private int? gridWidth;
	private int? gridHeight;

	private bool ingredientsLoaded = false;
	private List<IIngredient>? ingredients;

	private Func<Farmer, bool>? canCraft;
	private Func<Farmer, string?>? tooltipExtra;

	private Action<IPerformCraftEvent>? performCraft;
	private Func<Item?>? createItem;

	private int? quantity;
	private bool? stackable;

	public RecipeBuilder(CraftingRecipe recipe) {
		Recipe = recipe;
		Name = Recipe.name;
	}

	public RecipeBuilder(string name) {
		Recipe = null;
		Name = name;
	}

	#region Identity

	/// <inheritdoc />
	public IRecipeBuilder SortValue(int? value) {
		sortValue = value;
		return this;
	}

	/// <inheritdoc />
	public IRecipeBuilder DisplayName(Func<string>? value) {
		displayName = value;
		return this;
	}

	/// <inheritdoc />
	public IRecipeBuilder Description(Func<string?>? value) {
		description = value;
		return this;
	}

	/// <inheritdoc />
	public IRecipeBuilder HasRecipe(Func<Farmer, bool>? value) {
		hasRecipe = value;
		return this;
	}

	/// <inheritdoc />
	public IRecipeBuilder GetTimesCrafted(Func<Farmer, int>? value) {
		timesCrafted = value;
		return this;
	}

	#endregion

	#region Display

	/// <inheritdoc />
	public IRecipeBuilder Texture(Func<Texture2D>? value) {
		texture = value;
		return this;
	}

	/// <inheritdoc />
	public IRecipeBuilder Source(Func<Rectangle?>? value) {
		source = value;
		return this;
	}

	/// <inheritdoc />
	public IRecipeBuilder GridSize(int width, int height) {
		gridWidth = width;
		gridHeight = height;
		return this;
	}

	/// <inheritdoc />
	public IRecipeBuilder ClearGridSize() {
		gridWidth = null;
		gridHeight = null;
		return this;
	}

	#endregion

	#region Ingredients

	[MemberNotNull(nameof(ingredients))]
	private void LoadIngredients() {
		if (ingredientsLoaded && ingredients is not null)
			return;

		if (Recipe is not null)
			ingredients = Recipe.recipeList
				.Select(val => new BaseIngredient(val.Key, val.Value))
				.ToList<IIngredient>();
		else
			ingredients = new();

		ingredientsLoaded = true;
	}

	/// <inheritdoc />
	public IRecipeBuilder ClearIngredients(Func<IIngredient, bool>? predicate = null) {
		// Special logic for no predicate to avoid creating ingredients only to
		// immediately throw them out.
		if (predicate is null) {
			ingredients ??= new();
			if (ingredients.Count > 0)
				ingredients.Clear();
			ingredientsLoaded = true;
			return this;
		}

		LoadIngredients();
		ingredients.RemoveAll(ing => predicate(ing));
		return this;
	}

	/// <inheritdoc />
	public IRecipeBuilder AddIngredient(IIngredient ingredient) {
		LoadIngredients();
		ingredients.Add(ingredient);
		return this;
	}

	/// <inheritdoc />
	public IRecipeBuilder AddIngredients(IEnumerable<IIngredient> values) {
		LoadIngredients();
		ingredients.AddRange(values);
		return this;
	}

	#endregion

	#region Can Craft

	/// <inheritdoc />
	public IRecipeBuilder CanCraft(Func<Farmer, bool>? value) {
		canCraft = value;
		return this;
	}

	/// <inheritdoc />
	public IRecipeBuilder GetTooltipExtra(Func<Farmer, string?>? value) {
		tooltipExtra = value;
		return this;
	}

	#endregion

	#region Crafting and Output

	/// <inheritdoc />
	public IRecipeBuilder OnPerformCraft(Action<IPerformCraftEvent>? value) {
		performCraft = value;
		return this;
	}

	/// <inheritdoc />
	public IRecipeBuilder Item(Func<Item?>? value) {
		createItem = value;
		return this;
	}

	/// <inheritdoc />
	public IRecipeBuilder Quantity(int? value) {
		quantity = value;
		return this;
	}

	/// <inheritdoc />
	public IRecipeBuilder Stackable(bool? value) {
		stackable = value;
		return this;
	}

	#endregion

	#region Output

	/// <inheritdoc />
	public IRecipe Build() {
		LoadIngredients();

		return new BuiltRecipe(
			recipe: Recipe,
			name: Name,
			sortValue: sortValue,
			displayName: displayName,
			description: description,
			hasRecipe: hasRecipe,
			timesCrafted: timesCrafted,
			texture: texture,
			source: source,
			gridWidth: gridWidth,
			gridHeight: gridHeight,
			ingredients: ingredients.ToArray(),
			canCraft: canCraft,
			tooltipExtra: tooltipExtra,
			performCraft: performCraft,
			createItem: createItem,
			quantity: quantity,
			stackable: stackable
		);
	}

	#endregion

}

public class BuiltRecipe : IRecipe, IRecipeWithCaching {

	public static readonly Rectangle ERROR_SOURCE = new(268, 470, 16, 16);

	private readonly Func<string>? displayName;
	private readonly Func<string?>? description;
	private readonly Func<Farmer, bool>? hasRecipe;
	private readonly Func<Farmer, int>? timesCrafted;
	private readonly Func<Texture2D>? texture;
	private readonly Func<Rectangle?>? source;
	private readonly int? gridWidth;
	private readonly int? gridHeight;

	private readonly Func<Farmer, bool>? canCraft;
	private readonly Func<Farmer, string?>? tooltipExtra;

	private readonly Action<IPerformCraftEvent>? performCraft;
	private readonly Func<Item?>? createItem;

	// Cached Stuff
	private bool InvalidTexture;
	private Texture2D? _texture;
	private Rectangle? _source;
	private bool gridCalculated;
	private int _w;
	private int _h;


	public BuiltRecipe(CraftingRecipe? recipe, string name, int? sortValue, Func<string>? displayName, Func<string?>? description, Func<Farmer, bool>? hasRecipe, Func<Farmer, int>? timesCrafted, Func<Texture2D>? texture, Func<Rectangle?>? source, int? gridWidth, int? gridHeight, IIngredient[] ingredients, Func<Farmer, bool>? canCraft, Func<Farmer, string?>? tooltipExtra, Action<IPerformCraftEvent>? performCraft, Func<Item?>? createItem, int? quantity, bool? stackable) {
		CraftingRecipe = recipe;
		Name = name;
		Ingredients = ingredients;

		this.displayName = displayName;
		this.description = description;
		this.hasRecipe = hasRecipe;
		this.timesCrafted = timesCrafted;
		this.texture = texture;
		this.source = source;
		this.gridWidth = gridWidth;
		this.gridHeight = gridHeight;
		this.canCraft = canCraft;
		this.tooltipExtra = tooltipExtra;
		this.performCraft = performCraft;
		this.createItem = createItem;

		Item? example = CreateItem();

		SortValue = sortValue ?? example?.ParentSheetIndex ?? 0;
		QuantityPerCraft = quantity ?? example?.Stack ?? 1;
		Stackable = stackable ?? (example?.maximumStackSize() ?? 1) > 1;
	}

	public void ClearCache() {
		InvalidTexture = false;
		_texture = null;
		_source = null;
		gridCalculated = false;
	}

	#region Identity

	/// <inheritdoc />
	public int SortValue { get; }

	/// <inheritdoc />
	public string Name { get; }

	/// <inheritdoc />
	public string DisplayName => displayName?.Invoke() ?? CraftingRecipe?.DisplayName ?? Name;

	/// <inheritdoc />
	public string? Description => description is null ? CraftingRecipe?.description : description();

	/// <inheritdoc />
	public bool HasRecipe(Farmer who) {
		if (hasRecipe is not null)
			return hasRecipe(who);

		if (CraftingRecipe is not null && CraftingRecipe.isCookingRecipe)
			return who.cookingRecipes.ContainsKey(Name);

		return who.craftingRecipes.ContainsKey(Name);
	}

	/// <inheritdoc />
	public int GetTimesCrafted(Farmer who) {
		if (timesCrafted is not null)
			return timesCrafted(who);

		if (CraftingRecipe is not null && CraftingRecipe.isCookingRecipe) {
			int idx = CraftingRecipe.getIndexOfMenuView();
			if (who.recipesCooked.TryGetValue(idx, out int val))
				return val;

		} else if (who.craftingRecipes.TryGetValue(Name, out int val))
			return val;

		return 0;
	}

	/// <inheritdoc />
	public CraftingRecipe? CraftingRecipe { get; }

	#endregion

	#region Display

	[MemberNotNull(nameof(_texture))]
	private void LoadTexture() {
		if (texture is not null)
			_texture = texture();

		else if (CraftingRecipe is not null)
			_texture = CraftingRecipe.bigCraftable ?
				Game1.bigCraftableSpriteSheet :
				Game1.objectSpriteSheet;

		if (_texture is null) {
			InvalidTexture = true;
			_texture = Game1.mouseCursors;
		}
	}

	[MemberNotNull(nameof(_source))]
	private void LoadSource() {
		if (source is not null)
			_source = source();

		else if (CraftingRecipe is not null) {
			_source = CraftingRecipe.bigCraftable ?
				Game1.getArbitrarySourceRect(Texture, 16, 32, CraftingRecipe.getIndexOfMenuView()) :
				Game1.getSourceRectForStandardTileSheet(Texture, CraftingRecipe.getIndexOfMenuView(), 16, 16);
		}

		if (!_source.HasValue)
			_source = Texture.Bounds;
	}

	public Texture2D Texture {
		get {
			if (_texture is null)
				LoadTexture();

			return _texture;
		}
	}

	public Rectangle SourceRectangle {
		get {
			if (!_source.HasValue)
				LoadSource();

			if (InvalidTexture)
				return ERROR_SOURCE;

			return _source.Value;
		}
	}

	private void CalculateGrid() {
		gridCalculated = true;

		if (gridWidth.HasValue && gridHeight.HasValue) {
			_w = Math.Min(4, gridWidth.Value);
			_h = Math.Min(4, gridHeight.Value);
			return;
		}

		Rectangle rect = SourceRectangle;
		_w = 1; _h = 1;

		if (rect.Height > rect.Width)
			_h = 2;
		else if (rect.Width > rect.Height)
			_w = 2;
	}

	public int GridWidth {
		get {
			if (!gridCalculated)
				CalculateGrid();

			return _w;
		}
	}

	public int GridHeight {
		get {
			if (!gridCalculated)
				CalculateGrid();

			return _h;
		}
	}

	#endregion

	#region Cost

	public int QuantityPerCraft { get; }
	public IIngredient[] Ingredients { get; }

	#endregion

	#region Creation

	public bool Stackable { get; }

	public bool CanCraft(Farmer who) {
		if (canCraft is not null)
			return canCraft(who);

		return true;
	}

	public string? GetTooltipExtra(Farmer who) {
		return tooltipExtra?.Invoke(who);
	}

	public Item? CreateItem() {
		if (createItem is not null)
			return createItem();

		if (CraftingRecipe is not null)
			return CraftingRecipe.createItem();

		return null;
	}

	public void PerformCraft(IPerformCraftEvent evt) {
		if (performCraft is not null) {
			performCraft(evt);
		} else {
			if (evt.Item is null)
				evt.Cancel();
			else
				evt.Complete();
		}
	}

	#endregion

}
