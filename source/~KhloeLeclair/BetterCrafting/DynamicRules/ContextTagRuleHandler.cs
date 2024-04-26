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

public record struct TagFilterInfo(
	string Tag
);

public class ContextTagRuleHandler : DynamicTypeHandler<TagFilterInfo>, IOptionInputRuleHandler {

	public readonly ModEntry Mod;

	public ContextTagRuleHandler(ModEntry mod) {
		Mod = mod;
	}

	public override string DisplayName => I18n.Filter_ContextTag();

	public override string Description => I18n.Filter_ContextTag_About();

	public override Texture2D Texture => ItemRegistry.GetData("(O)79").GetTexture();

	public override Rectangle Source => ItemRegistry.GetData("(O)79").GetSourceRect();

	public override bool AllowMultiple => true;

	public override bool HasEditor => true;

	public IEnumerable<KeyValuePair<string, string>> GetOptions(bool cooking) {
		Dictionary<string, int> ByTag = new();

		foreach (var recipe in Mod.Recipes.GetRecipes(cooking))
			if (recipe.CreateItemSafe() is Item i) {
				foreach (string tag in i.GetContextTags())
					ByTag[tag] = 1 + ByTag.GetValueOrDefault(tag);
			}

		List<KeyValuePair<string, string>> result = new();

		foreach (var pair in ByTag) {
			string tag = pair.Key;
			if (string.IsNullOrWhiteSpace(tag))
				continue;

			// Skip auto ID tags.
			if (tag.StartsWith("id_"))
				continue;

			// Skip auto item_ tags with only one item.
			if (tag.StartsWith("item_") && pair.Value < 2)
				continue;

			string count = I18n.Filter_RecipeCount(pair.Value);
			result.Add(new(tag, $"{tag}\n{count}"));
		}

		result.Sort((a, b) => a.Key.CompareTo(b.Key));

		return result;
	}

	public string HelpText => string.Empty;

	public override bool DoesRecipeMatch(IRecipe recipe, Lazy<Item?> item, TagFilterInfo state) {
		if (item.Value is not Item i || state.Tag is null)
			return false;

		return i.HasContextTag(state.Tag);
	}

	public override IClickableMenu? GetEditor(IClickableMenu parent, IDynamicRuleData data) {
		return null;
	}

	public override IFlowNode[]? GetExtraInfo(TagFilterInfo state) {
		if (string.IsNullOrWhiteSpace(state.Tag))
			return null;

		return FlowHelper.Builder()
			.Text(" ")
			.Text(state.Tag, shadow: false)
			.Build();
	}

	public override TagFilterInfo ParseStateT(IDynamicRuleData type) {
		if (!type.Fields.TryGetValue("Input", out var token))
			return default;

		string? rawInput = (string?) token;
		if (string.IsNullOrWhiteSpace(rawInput))
			return default;

		return new TagFilterInfo(rawInput.Trim());
	}

}
