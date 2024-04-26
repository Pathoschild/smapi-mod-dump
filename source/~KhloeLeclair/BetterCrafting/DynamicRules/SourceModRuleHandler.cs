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

using StardewModdingAPI;

using StardewValley;
using StardewValley.Menus;

namespace Leclair.Stardew.BetterCrafting.DynamicRules;

public record struct ModFilterInfo(
	string? ModId,
	string? Prefix,
	IModInfo? Info
);

public class SourceModRuleHandler : DynamicTypeHandler<ModFilterInfo>, IOptionInputRuleHandler {

	public readonly ModEntry Mod;

	public SourceModRuleHandler(ModEntry mod) {
		Mod = mod;
	}

	public override string DisplayName => I18n.Filter_Mod();

	public override string Description => I18n.Filter_Mod_About();

	public override Texture2D Texture => Game1.mouseCursors;

	public override Rectangle Source => new(420, 489, 25, 18);

	public override bool AllowMultiple => true;

	public override bool HasEditor => true;

	public IEnumerable<KeyValuePair<string, string>> GetOptions(bool cooking) {
		List<KeyValuePair<string, string>> mods = new();

		Dictionary<string, int> itemCount = new();

		foreach (var other in Mod.Helper.ModRegistry.GetAll())
			itemCount[other.Manifest.UniqueID] = 0;

		bool TryCount(string? id) {
			if (string.IsNullOrEmpty(id))
				return false;

			if (id.StartsWith("bcbuildings:"))
				id = id.Substring(12);

			int idx = id.LastIndexOf("_");
			if (idx <= 1)
				return false;

			string possible_id = id[..idx];
			if (itemCount.TryGetValue(possible_id, out int count)) {
				itemCount[possible_id] = count + 1;
				return true;
			}

			return false;
		}

		foreach (var recipe in Mod.Recipes.GetRecipes(cooking)) {
			if (!TryCount(recipe.Name))
				TryCount(recipe.CreateItemSafe()?.ItemId);
		}

		foreach (var other in Mod.Helper.ModRegistry.GetAll()) {
			string count = I18n.Filter_RecipeCount(itemCount.GetValueOrDefault(other.Manifest.UniqueID));
			mods.Add(new(other.Manifest.UniqueID, $"{other.Manifest.Name} @>@h({other.Manifest.UniqueID})\n@<{count}"));
		}

		mods.Sort((a, b) => {
			int aCount = itemCount.GetValueOrDefault(a.Key);
			int bCount = itemCount.GetValueOrDefault(b.Key);

			if (aCount != 0 && bCount == 0)
				return -1;
			if (aCount == 0 && bCount != 0)
				return 1;

			return a.Value.CompareTo(b.Value);
		});

		return mods;
	}

	public string HelpText => string.Empty;

	private bool isPrefixed(ModFilterInfo state, string? name) {
		return !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(state.Prefix) && name.StartsWith(state.Prefix);
	}

	private bool CheckString(string? id, ModFilterInfo state) {
		if (string.IsNullOrEmpty(id))
			return false;

		if (id.StartsWith("bcbuildings:"))
			id = id.Substring(12);

		return isPrefixed(state, id);
	}

	public override bool DoesRecipeMatch(IRecipe recipe, Lazy<Item?> item, ModFilterInfo state) {
		return CheckString(recipe.Name, state) || CheckString(item.Value?.ItemId, state);
	}

	public override IClickableMenu? GetEditor(IClickableMenu parent, IDynamicRuleData data) {
		return null;
	}

	public override IFlowNode[]? GetExtraInfo(ModFilterInfo state) {
		if (state.Info is null) {
			if (string.IsNullOrEmpty(state.ModId))
				return null;

			return FlowHelper.Builder()
				.Text(" ")
				.Text(state.ModId, shadow: false)
				.Text(" (unloaded)", shadow: false, color: Game1.textColor * 0.5f)
				.Build();
		}

		return FlowHelper.Builder()
			.Text(" ")
			.Text(state.Info.Manifest.Name, shadow: false)
			.Build();
	}

	public override ModFilterInfo ParseStateT(IDynamicRuleData type) {
		if (!type.Fields.TryGetValue("Input", out var token))
			return default;

		string? modId = (string?) token;
		if (string.IsNullOrEmpty(modId))
			return default;

		return new ModFilterInfo(
			ModId: modId,
			Prefix: string.IsNullOrEmpty(modId) ? null : $"{modId}_",
			Info: Mod.Helper.ModRegistry.Get(modId)
		);
	}

}
