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
using System.Text.RegularExpressions;

using Leclair.Stardew.BetterCrafting.Models;
using Leclair.Stardew.Common.Crafting;
using Leclair.Stardew.Common.UI.FlowNode;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


using StardewValley;
using StardewValley.Menus;

namespace Leclair.Stardew.BetterCrafting.DynamicRules;

public record struct FilterInfo(
	string? Input,
	Regex? Regex,
	bool Ingredients,
	bool Likes,
	bool Loves
);

public class SearchRuleHandler : DynamicTypeHandler<FilterInfo>, ISimpleInputRuleHandler {

	public readonly ModEntry Mod;

	public SearchRuleHandler(ModEntry mod) {
		Mod = mod;
	}

	public override string DisplayName => I18n.Filter_Search();

	public override string Description => I18n.Filter_Search_About();

	public override Texture2D Texture => Game1.mouseCursors;

	public override Rectangle Source => new Rectangle(208, 320, 16, 16);

	public override bool AllowMultiple => true;

	public override bool HasEditor => true;

	public string HelpText => I18n.Tooltip_Search_Tip(
		I18n.Search_IngredientPrefix(),
		I18n.Search_LikePrefix(),
		I18n.Search_LovePrefix()
	);

	public override bool DoesRecipeMatch(IRecipe recipe, Lazy<Item?> item, FilterInfo state) {
		if (state.Regex is null)
			return false;

		if (state.Regex.IsMatch(recipe.Name) || state.Regex.IsMatch(recipe.DisplayName))
			return true;

		if (!string.IsNullOrEmpty(recipe.Description) && state.Regex.IsMatch(recipe.Description))
			return true;

		if (state.Ingredients && recipe.Ingredients != null) {
			foreach(var ing in recipe.Ingredients) {
				if (state.Regex.IsMatch(ing.DisplayName))
					return true;
			}
		}

		if (state.Likes || state.Loves) {
			(List<NPC>, List<NPC>)? tastes = Mod.Recipes.GetGiftTastes(recipe);
			if (tastes is not null) {
				if (state.Loves)
					foreach (NPC npc in tastes.Value.Item1) {
						if (state.Regex.IsMatch(npc.displayName))
							return true;
					}

				if (state.Likes)
					foreach (NPC npc in tastes.Value.Item2) {
						if (state.Regex.IsMatch(npc.displayName))
							return true;
					}
			}
		}

		return false;
	}

	public override IClickableMenu? GetEditor(IClickableMenu parent, IDynamicRuleData type) {
		return null;
	}

	public override IFlowNode[]? GetExtraInfo(FilterInfo state) {
		return string.IsNullOrEmpty(state.Input) ? null : new IFlowNode[]{
			new TextNode(" "),
			new TextNode(state.Input, new Common.UI.TextStyle(shadow: false))
		};
	}

	public override FilterInfo ParseStateT(IDynamicRuleData type) {
		if (!type.Fields.TryGetValue("Input", out var token))
			return default;

		string? input = (string?) token;
		if (string.IsNullOrEmpty(input))
			return default;

		input = input.Trim();
		string raw = input;

		string ing_prefix = I18n.Search_IngredientPrefix();
		string like_prefix = I18n.Search_LikePrefix();
		string love_prefix = I18n.Search_LovePrefix();
		bool matched = true;

		bool ingredients = false;
		bool likes = false;
		bool loves = false;

		while (matched) {
			matched = false;

			bool has = !ingredients && input.StartsWith(ing_prefix);
			if (has) {
				ingredients = true;
				matched = true;
				input = input[ing_prefix.Length..].TrimStart();
			}

			has = !likes && input.StartsWith(like_prefix);
			if (has) {
				likes = true;
				matched = true;
				input = input[like_prefix.Length..].TrimStart();
			}

			has = !loves && input.StartsWith(love_prefix);
			if (has) {
				loves = true;
				matched = true;
				input = input[love_prefix.Length..].TrimStart();
			}
		}

		return new FilterInfo(
			Input: raw,
			Regex: new(Regex.Escape(input), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant),
			Ingredients: ingredients,
			Likes: likes,
			Loves: loves
		);
	}
}
