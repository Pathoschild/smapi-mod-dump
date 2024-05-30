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

using Leclair.Stardew.BetterCrafting.Integrations.SpaceCore;
using Leclair.Stardew.BetterCrafting.Models;
using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Crafting;
using Leclair.Stardew.Common.UI;
using Leclair.Stardew.Common.UI.FlowNode;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.GameData.Buffs;
using StardewValley.GameData.Objects;
using StardewValley.Menus;
using StardewValley.TokenizableStrings;

namespace Leclair.Stardew.BetterCrafting.DynamicRules;

public enum FilterType {
	Invalid,
	Buff,
	Stat,
	SpaceCore
};

public record struct BuffFilterInfo(
	FilterType Type,
	ISCSkill? SCSkill,
	int Stat,
	string Value
);

public class BuffRuleHandler : DynamicTypeHandler<BuffFilterInfo>, IOptionInputRuleHandler {

	public static readonly Dictionary<string, int> StatMap = new() {
		{ "Farming", Buff.farming },
		{ "Fishing", Buff.fishing },
		{ "Mining", Buff.mining },
		{ "Luck", Buff.luck },
		{ "Foraging", Buff.foraging },
		{ "MaxEnergy", Buff.maxStamina },
		{ "Magnetism", Buff.magneticRadius },
		{ "Speed", Buff.speed },
		{ "Defense", Buff.defense },
		{ "Attack", Buff.attack }
	};

	private readonly ModEntry Mod;

	public BuffRuleHandler(ModEntry mod) {
		Mod = mod;
	}

	public override string DisplayName => I18n.Filter_NewBuff();

	public override string? Description => I18n.Filter_NewBuff_About();

	public override Texture2D Texture => Game1.buffsIcons;

	public override Rectangle Source => Game1.getSourceRectForStandardTileSheet(Game1.buffsIcons, 16, 16, 16);

	public override Texture2D GetTexture(BuffFilterInfo state) {
		if (state.Type == FilterType.SpaceCore)
			return state.SCSkill?.SafeGetTexture() ?? Game1.mouseCursors;

		else if (state.Type == FilterType.Stat)
			return Game1.mouseCursors;

		else if (state.Type == FilterType.Buff) {
			var buff = DataLoader.Buffs(Game1.content).GetValueOrDefault(state.Value);
			if (buff?.IconTexture is null)
				return Game1.mouseCursors;

			return Game1.content.Load<Texture2D>(buff.IconTexture);
		}

		return base.GetTexture(state);
	}

	public override Rectangle GetSource(BuffFilterInfo state) {
		if (state.Type == FilterType.SpaceCore) {
			var tex = state.SCSkill?.SafeGetTexture();
			return tex is null ? new Rectangle(268, 470, 16, 16) : tex.Bounds;

		} else if (state.Type == FilterType.Stat)
			return new Rectangle(10 + 10 * state.Stat, 428, 10, 10);

		else if (state.Type == FilterType.Buff) {
			var buff = DataLoader.Buffs(Game1.content).GetValueOrDefault(state.Value);
			if (buff?.IconTexture is null)
				return new Rectangle(268, 470, 16, 16);

			return Game1.getSourceRectForStandardTileSheet(GetTexture(state), buff.IconSpriteIndex, 16, 16);
		}

		return base.GetSource(state);
	}

	public override bool AllowMultiple => true;

	public override bool HasEditor => true;



	public IEnumerable<KeyValuePair<string, string>> GetOptions(bool cooking) {
		List<KeyValuePair<string, string>> result = new();

		// We need to scan all the recipes we've got, and figure out which buffs
		// we get from them.
		Dictionary<string, int> SpaceCoreBuffs = [];
		Dictionary<string, int> NormalBuffs = [];
		Dictionary<int, int> StatBuffs = [];

		var objects = DataLoader.Objects(Game1.content);

		foreach (var recipe in Mod.Recipes.GetRecipes(cooking)) {
			var item = recipe.CreateItemSafe();
			if (item is null ||
				item.GetItemTypeId() != ItemRegistry.type_object ||
				!objects.TryGetValue(item.ItemId, out var data) ||
				data.Buffs is null ||
				data.Buffs.Count == 0
			)
				continue;

			foreach (var buff in data.Buffs) {
				// First, count unique buff Ids.
				if (buff.Id != "Drink" && buff.Id != "Food" && !string.IsNullOrEmpty(buff.BuffId))
					NormalBuffs[buff.BuffId] = NormalBuffs.GetValueOrDefault(buff.BuffId) + 1;

				// Second, count stat buffs
				foreach (int stat in EnumerateStatBuffs(buff.CustomAttributes))
					StatBuffs[stat] = StatBuffs.GetValueOrDefault(stat) + 1;

				// Finally, SpaceCore buffs.
				if (buff.CustomFields is not null)
					foreach (var pair in buff.CustomFields)
						if (pair.Key.StartsWith(SCIntegration.SKILL_BUFF_PREFIX)) {
							string skillId = pair.Key[SCIntegration.SKILL_BUFF_PREFIX.Length..];
							SpaceCoreBuffs[skillId] = SpaceCoreBuffs.GetValueOrDefault(skillId) + 1;
						}
			}
		}

		// Alright, now that we have all the buffs, and the recipe counts, we
		// need to actually build the entries for them.
		Dictionary<string, int> FinalCounts = [];

		for (int stat = 0; stat < 12; stat++) {
			int count = StatBuffs.GetValueOrDefault(stat);
			if (count == 0 && (stat == 3 || stat == 6))
				continue;

			string name = Game1.content.LoadString(@"Strings\UI:ItemHover_Buff" + stat, "").Trim();
			string scount = I18n.Filter_RecipeCount(count);
			string key = $"stat:{stat}";
			FinalCounts[key] = count;

			result.Add(new(key, $"{name}\n@h{scount}"));
		}

		var BuffData = DataLoader.Buffs(Game1.content);
		foreach (var pair in NormalBuffs) {
			if (!BuffData.TryGetValue(pair.Key, out var buff))
				continue;

			string name = TokenParser.ParseText(buff.DisplayName);
			string count = I18n.Filter_RecipeCount(pair.Value);
			FinalCounts[pair.Key] = pair.Value;

			result.Add(new(pair.Key, $"{name} @>@h({pair.Key})\n@<{count}"));
		}

		foreach (var pair in SpaceCoreBuffs) {
			var buff = Mod.intSCore?.GetSkill(pair.Key);
			if (buff is null)
				continue;

			string name = buff.SafeGetName();
			string count = I18n.Filter_RecipeCount(pair.Value);
			string key = $"sc:{pair.Key}";
			FinalCounts[key] = pair.Value;

			result.Add(new(key, $"{name} @>@h({key})\n@<{count}"));
		}

		result.Sort((a, b) => {
			int aCount = FinalCounts.GetValueOrDefault(a.Key);
			int bCount = FinalCounts.GetValueOrDefault(b.Key);

			if (aCount != 0 && bCount == 0)
				return -1;
			if (aCount == 0 && bCount != 0)
				return 1;

			return a.Value.CompareTo(b.Value);
		});

		return result;
	}

	public string HelpText => string.Empty;

	public static IEnumerable<int> EnumerateStatBuffs(BuffAttributesData? data) {
		if (data is null)
			yield break;

		if (data.FarmingLevel > 0)
			yield return Buff.farming;
		if (data.FishingLevel > 0)
			yield return Buff.fishing;
		if (data.MiningLevel > 0)
			yield return Buff.mining;
		if (data.LuckLevel > 0)
			yield return Buff.luck;
		if (data.ForagingLevel > 0)
			yield return Buff.foraging;
		if (data.MaxStamina > 0)
			yield return Buff.maxStamina;
		if (data.MagneticRadius > 0)
			yield return Buff.magneticRadius;
		if (data.Speed > 0)
			yield return Buff.speed;
		if (data.Defense > 0)
			yield return Buff.defense;
		if (data.Attack > 0)
			yield return Buff.attack;
	}

	public static float GetBuffLevel(int stat, BuffAttributesData? data) {
		if (data == null) return 0f;

		return stat switch {
			Buff.farming => data.FarmingLevel,
			Buff.fishing => data.FishingLevel,
			Buff.mining => data.MiningLevel,
			Buff.luck => data.LuckLevel,
			Buff.foraging => data.ForagingLevel,
			Buff.maxStamina => data.MaxStamina,
			Buff.magneticRadius => data.MagneticRadius,
			Buff.speed => data.Speed,
			Buff.defense => data.Defense,
			Buff.attack => data.Attack,
			_ => 0f,
		};
	}

	public override bool DoesRecipeMatch(IRecipe recipe, Lazy<Item?> item, BuffFilterInfo state) {
		if (state.Type == FilterType.Invalid || item.Value is not SObject sobj || sobj.bigCraftable.Value || sobj.Edibility <= -300 || !Game1.objectData.TryGetValue(sobj.ItemId, out var data))
			return false;

		// Make sure we have buffs.
		if (data.Buffs is null || data.Buffs.Count == 0)
			return false;

		if (state.Type == FilterType.SpaceCore) {
			foreach (ObjectBuffData? buff in data.Buffs) {
				// Check for the field we want, and see if we have a positive value.
				if (buff?.CustomFields is not null && buff.CustomFields.TryGetValue(state.Value, out string? val) && int.TryParse(val, out int value) && value > 0)
					return true;
			}

		} else if (state.Type == FilterType.Buff) {
			foreach (ObjectBuffData? buff in data.Buffs) {
				if (buff.BuffId == state.Value)
					return true;
			}

		} else if (state.Type == FilterType.Stat) {
			foreach (ObjectBuffData? buff in data.Buffs) {
				if (GetBuffLevel(state.Stat, buff.CustomAttributes) > 0f)
					return true;
			}
		}

		// RIP.
		return false;
	}

	public override IClickableMenu? GetEditor(IClickableMenu parent, IDynamicRuleData data) {
		return null;
	}

	public override IFlowNode[]? GetExtraInfo(BuffFilterInfo state) {
		string? name;

		if (state.Type == FilterType.SpaceCore)
			name = state.SCSkill?.SafeGetName();

		else if (state.Type == FilterType.Buff) {
			var buff = DataLoader.Buffs(Game1.content).GetValueOrDefault(state.Value);
			if (string.IsNullOrEmpty(buff?.DisplayName))
				name = state.Value;
			else
				name = TokenParser.ParseText(buff.DisplayName);

		} else if (state.Type == FilterType.Stat)
			name = Game1.content.LoadString(@"Strings\UI:ItemHover_Buff" + state.Stat, "").Trim();

		else
			name = null;

		if (name is null)
			return null;

		return FlowHelper.Builder()
			.Text(name, shadow: false)
			.Build();
	}

	public override BuffFilterInfo ParseStateT(IDynamicRuleData type) {
		if (!type.Fields.TryGetValue("Input", out var token))
			return new() {
				Type = FilterType.Invalid
			};

		string? rawInput = (string?) token;
		if (string.IsNullOrWhiteSpace(rawInput))
			return new() {
				Type = FilterType.Invalid
			};

		if (rawInput.StartsWith("sc:")) {
			string skillId = rawInput[3..];
			ISCSkill? skill = Mod.intSCore?.GetSkill(skillId);
			if (skill != null)
				return new() {
					Type = FilterType.SpaceCore,
					SCSkill = skill,
					Value = $"{SCIntegration.SKILL_BUFF_PREFIX}{skillId}"
				};

		} else if (rawInput.StartsWith("stat:")) {
			string statId = rawInput[5..];
			if (StatMap.TryGetValue(statId, out int stat) || int.TryParse(statId, out stat))
				return new() {
					Type = FilterType.Stat,
					Stat = stat
				};

		} else
			return new() {
				Type = FilterType.Buff,
				Value = rawInput
			};

		return new() {
			Type = FilterType.Invalid,
			Value = rawInput
		};
	}

}
