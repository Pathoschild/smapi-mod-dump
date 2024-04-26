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

using Leclair.Stardew.BetterCrafting.Models;
using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Crafting;
using Leclair.Stardew.Common.UI;
using Leclair.Stardew.Common.UI.FlowNode;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;

namespace Leclair.Stardew.BetterCrafting.DynamicRules;

public record struct CategoryFilterInfo(
	int? Category
);

public class CategoryRuleHandler : DynamicTypeHandler<CategoryFilterInfo>, IOptionInputRuleHandler {

	public static readonly Dictionary<int, int> REMAP = new() {
		{ -12, -2 },
		{ -25, -7 },
		{ -27, -26 },
		{ -18, -5 },
		{ -14, -5 },
		{ -6, -5 },
		{ -16, -15 }
	};

	public readonly ModEntry Mod;

	public CategoryRuleHandler(ModEntry mod) {
		Mod = mod;
	}

	public override string DisplayName => I18n.Filter_Category();

	public override string Description => I18n.Filter_Category_About();

	public override Texture2D Texture => Game1.mouseCursors;

	public override Rectangle Source => new(325, 318, 25, 18);

	public override bool AllowMultiple => true;

	public override bool HasEditor => true;

	public string GetCategoryName(int category) {
		string name = SObject.GetCategoryDisplayName(category).Trim();

		if (category == SObject.equipmentCategory)
			name = I18n.Item_Category_Equipment();

		/*switch (category) {
			case SObject.mineralsCategory: // -12
			case SObject.GemCategory: // -2
				break;

			case SObject.ingredientsCategory: // -25
			case SObject.CookingCategory: // -7
				break;

			case SObject.syrupCategory: // -27
			case SObject.artisanGoodsCategory: // -26
				break;

			case SObject.sellAtPierresAndMarnies: // -18
			case SObject.meatCategory: // -14
			case SObject.MilkCategory: // -6
			case SObject.EggCategory: // -5
				break;

			case SObject.buildingResources: // -16
			case SObject.metalResources: // -15
				break;
		}*/

		return name;
	}

	public IEnumerable<KeyValuePair<string, string>> GetOptions(bool cooking) {
		Dictionary<int, int> ByCategory = new();

		foreach (var recipe in Mod.Recipes.GetRecipes(cooking))
			if (recipe.CreateItemSafe() is Item i) {
				int cat = i.Category;
				if (REMAP.TryGetValue(cat, out int value))
					cat = value;
				ByCategory[cat] = 1 + ByCategory.GetValueOrDefault(cat);
			}

		List<KeyValuePair<string, string>> result = new();

		// TODO: SpaceCore categories?

		foreach (var pair in ByCategory) {
			int category = pair.Key;

			string color = SObject.GetCategoryColor(category).ToHex();
			string name = GetCategoryName(category);

			if (string.IsNullOrWhiteSpace(name))
				continue;

			string count = I18n.Filter_RecipeCount(pair.Value);
			result.Add(new($"{category}", $"@C{{{color}}}{name}@C{{}} @>@h({category})\n@<{count}"));
		}

		result.Sort((a, b) => a.Value.CompareTo(b.Value));

		return result;
	}

	public string HelpText => string.Empty;

	public override bool DoesRecipeMatch(IRecipe recipe, Lazy<Item?> item, CategoryFilterInfo state) {
		if (item.Value is not Item i || state.Category is null)
			return false;

		int cat = i.Category;
		if (REMAP.TryGetValue(cat, out int value))
			cat = value;

		return cat == state.Category.Value;
	}

	public override IClickableMenu? GetEditor(IClickableMenu parent, IDynamicRuleData data) {
		return null;
	}

	public override IFlowNode[]? GetExtraInfo(CategoryFilterInfo state) {
		if (!state.Category.HasValue)
			return null;

		return FlowHelper.Builder()
			.Text(" ")
			.Text(GetCategoryName(state.Category.Value), shadow: false, color: SObject.GetCategoryColor(state.Category.Value))
			.Build();
	}

	public override CategoryFilterInfo ParseStateT(IDynamicRuleData type) {
		if (!type.Fields.TryGetValue("Input", out var token))
			return default;

		string? rawInput = (string?) token;
		if (string.IsNullOrEmpty(rawInput) || ! int.TryParse(rawInput, out int category))
			return default;

		return new CategoryFilterInfo(category);
	}

}
