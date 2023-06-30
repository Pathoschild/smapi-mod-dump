/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using Newtonsoft.Json;
using Shockah.Kokoro;
using Shockah.Kokoro.GMCM;
using Shockah.Kokoro.Stardew;
using Shockah.Kokoro.UI;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Shockah.SeasonAffixes;

partial class ModConfig
{
	[JsonProperty] public IDictionary<string, int> SkillLevelIncrease { get; internal set; } = new Dictionary<string, int>();
	[JsonProperty] public IDictionary<string, float> SkillXPIncrease { get; internal set; } = new Dictionary<string, float>();
}

internal sealed class SkillAffix : ISeasonAffix
{
	private static readonly int DefaultLevelIncrease = 3;
	private static readonly float DefaultVanillaXPIncrease = 0.2f;
	private static readonly float DefaultCustomXPIncrease = 0.25f;

	private static SeasonAffixes Mod
		=> SeasonAffixes.Instance;

	private static bool IsHarmonySetup = false;

	public ISkill Skill { get; init; }

	private static string ShortID => "Skill";
	public string UniqueID { get; init; }
	public string LocalizedName => Skill.Name;
	public TextureRectangle Icon => Skill.Icon ?? new(Game1.objectSpriteSheet, new(0, 64, 16, 16));

	private Lazy<IReadOnlySet<string>> LazyTags { get; init; }

	public string LocalizedDescription
	{
		get
		{
			var parts = new List<string>();
			if (Skill is VanillaSkill && LevelIncreaseConfig > 0)
				parts.Add(Mod.Helper.Translation.Get($"affix.positive.{ShortID}.description.level", new { Skill = Skill.Name, LevelIncrease = LevelIncreaseConfig }));
			if (XPIncreaseConfig > 0f || parts.Count == 0)
				parts.Add(Mod.Helper.Translation.Get($"affix.positive.{ShortID}.description.xp", new { Skill = Skill.Name, XPIncrease = $"{(int)(XPIncreaseConfig * 100):0.##}%" }));
			return string.Join(" ", parts);
		}
	}

	private int LevelIncreaseConfig
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get => Mod.Config.SkillLevelIncrease.TryGetValue(Skill.UniqueID, out var value) ? value : DefaultLevelIncrease;
	}

	private float DefaultXPIncrease
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get => Skill is VanillaSkill ? DefaultVanillaXPIncrease : DefaultCustomXPIncrease;
	}

	private float XPIncreaseConfig
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get => Mod.Config.SkillXPIncrease.TryGetValue(Skill.UniqueID, out var value) ? value : DefaultXPIncrease;
	}

	public SkillAffix(ISkill skill)
	{
		this.UniqueID = $"{Mod.ModManifest.UniqueID}.{ShortID}:{skill.UniqueID}";
		this.Skill = skill;
		this.LazyTags = new(() =>
		{
			if (Skill.Equals(VanillaSkill.Farming))
				return new HashSet<string> { VanillaSkill.CropsAspect, VanillaSkill.FlowersAspect, VanillaSkill.AnimalsAspect };
			else if (Skill.Equals(VanillaSkill.Mining))
				return new HashSet<string> { VanillaSkill.MetalAspect, VanillaSkill.GemAspect };
			else if (Skill.Equals(VanillaSkill.Foraging))
				return new HashSet<string> { VanillaSkill.WoodcuttingAspect, VanillaSkill.GatheringAspect, VanillaSkill.TappingAspect };
			else if (Skill.Equals(VanillaSkill.Fishing))
				return new HashSet<string> { VanillaSkill.FishingAspect, VanillaSkill.TrappingAspect, VanillaSkill.PondsAspect };
			else
				return new HashSet<string> { Skill.UniqueID };
		});
	}

	public IReadOnlySet<string> Tags => LazyTags.Value;

	public int GetPositivity(OrdinalSeason season)
		=> 1;

	public int GetNegativity(OrdinalSeason season)
		=> 0;

	public double GetProbabilityWeight(OrdinalSeason season)
	{
		if (XPIncreaseConfig == 0f && (Skill is not VanillaSkill || LevelIncreaseConfig == 0f))
			return 0; // invalid config - skipping affix
		if (XPIncreaseConfig > 0f && Skill is not VanillaSkill && Game1.getAllFarmers().All(player => Skill.GetBaseLevel(player) >= Skill.MaxLevel))
			return 0; // the skill only grants XP, but all players are maxed out - skipping affix
		return Math.Min(2.0 / Mod.AllAffixes.Values.Count(affix => affix is SkillAffix), 1.0);
	}

	public void OnRegister()
		=> Apply(Mod.Harmony);

	public void OnActivate(AffixActivationContext context)
	{
		Mod.Helper.Events.GameLoop.DayStarted += OnDayStarted;
		Mod.Helper.Events.GameLoop.DayEnding += OnDayEnding;

		if (!(context is AffixActivationContext.Choice or AffixActivationContext.SaveLoadedOrUnloaded) && Skill is VanillaSkill skill)
			ModifySkillLevel(Game1.player, skill, LevelIncreaseConfig);
	}

	public void OnDeactivate(AffixActivationContext context)
	{
		Mod.Helper.Events.GameLoop.DayStarted -= OnDayStarted;
		Mod.Helper.Events.GameLoop.DayEnding -= OnDayEnding;

		if (!(context is AffixActivationContext.Choice or AffixActivationContext.SaveLoadedOrUnloaded) && Skill is VanillaSkill skill)
			ModifySkillLevel(Game1.player, skill, -LevelIncreaseConfig);
	}

	public void SetupConfig(IManifest manifest)
	{
		var api = Mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu")!;

		if (Skill is VanillaSkill)
		{
			api.AddNumberOption(
				Mod.ModManifest,
				getValue: () => Mod.Config.SkillLevelIncrease.TryGetValue(Skill.UniqueID, out var value) ? value : DefaultLevelIncrease,
				setValue: value =>
				{
					if (value == DefaultLevelIncrease)
						Mod.Config.SkillLevelIncrease.Remove(Skill.UniqueID);
					else
						Mod.Config.SkillLevelIncrease[Skill.UniqueID] = value;
				},
				name: () => Mod.Helper.Translation.Get($"affix.positive.{ShortID}.config.levelIncrease.name"),
				tooltip: () => Mod.Helper.Translation.Get($"affix.positive.{ShortID}.config.levelIncrease.tooltip"),
				min: 0, max: 5, interval: 1
			);
		}

		api.AddNumberOption(
			Mod.ModManifest,
			getValue: () => Mod.Config.SkillXPIncrease.TryGetValue(Skill.UniqueID, out var value) ? value : DefaultXPIncrease,
			setValue: value =>
			{
				if (value == DefaultXPIncrease)
					Mod.Config.SkillXPIncrease.Remove(Skill.UniqueID);
				else
					Mod.Config.SkillXPIncrease[Skill.UniqueID] = value;
			},
			name: () => Mod.Helper.Translation.Get($"affix.positive.{ShortID}.config.xpIncrease.name"),
			tooltip: () => Mod.Helper.Translation.Get($"affix.positive.{ShortID}.config.xpIncrease.tooltip"),
			min: 0f, max: 2f, interval: 0.01f,
			formatValue: value => $"{(int)(value * 100):0.##}%"
		);
	}

	private void OnDayStarted(object? sender, DayStartedEventArgs e)
	{
		if (Skill is not VanillaSkill skill)
			return;
		ModifySkillLevel(Game1.player, skill, LevelIncreaseConfig);
	}

	private void OnDayEnding(object? sender, DayEndingEventArgs e)
	{
		if (Skill is not VanillaSkill skill)
			return;
		ModifySkillLevel(Game1.player, skill, -LevelIncreaseConfig);
	}

	private void Apply(Harmony harmony)
	{
		if (IsHarmonySetup)
			return;
		IsHarmonySetup = true;

		//harmony.TryPatch(
		//	monitor: Mod.Monitor,
		//	original: () => AccessTools.Method(typeof(Farmer), nameof(Farmer.ClearBuffs)),
		//	postfix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(Farmer_ClearBuffs_Postfix)))
		//);

		harmony.TryPatch(
			monitor: Mod.Monitor,
			original: () => AccessTools.Method(typeof(Farmer), nameof(Farmer.gainExperience)),
			prefix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(Farmer_gainExperience_Prefix)))
		);

		if (Mod.Helper.ModRegistry.IsLoaded("spacechase0.SpaceCore"))
		{
			harmony.TryPatch(
				monitor: Mod.Monitor,
				original: () => AccessTools.Method(AccessTools.TypeByName("SpaceCore.Skills, SpaceCore"), "AddExperience"),
				prefix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(SpaceCore_Skills_AddExperience_Prefix)))
			);
		}
	}

	private static void ModifySkillLevel(Farmer player, VanillaSkill skill, int levels)
	{
		switch (skill.SkillIndex)
		{
			case Farmer.farmingSkill:
				player.addedFarmingLevel.Value += levels;
				break;
			case Farmer.miningSkill:
				player.addedMiningLevel.Value += levels;
				break;
			case Farmer.foragingSkill:
				player.addedForagingLevel.Value += levels;
				break;
			case Farmer.fishingSkill:
				player.addedFishingLevel.Value += levels;
				break;
			case Farmer.combatSkill:
				player.addedCombatLevel.Value += levels;
				break;
			case Farmer.luckSkill:
				player.addedLuckLevel.Value += levels;
				break;
			default:
				throw new ArgumentException($"{nameof(skill.SkillIndex)} has an invalid value.");
		}
	}

	private static void Farmer_gainExperience_Prefix(int which, ref int howMuch)
	{
		var affix = Mod.ActiveAffixes.OfType<SkillAffix>().FirstOrDefault(affix => affix.Skill is VanillaSkill skill && skill.SkillIndex == which);
		if (affix is null)
			return;
		howMuch = (int)Math.Ceiling(howMuch * (1f + affix.XPIncreaseConfig));
	}

	private static void SpaceCore_Skills_AddExperience_Prefix(string skillName, ref int amt)
	{
		var affix = Mod.ActiveAffixes.OfType<SkillAffix>().FirstOrDefault(affix => affix.Skill is SpaceCoreSkill skill && skill.SkillName == skillName);
		if (affix is null)
			return;
		amt = (int)Math.Ceiling(amt * (1f + affix.XPIncreaseConfig));
	}
}