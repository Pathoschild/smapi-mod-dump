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
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley;
using System;
using System.Reflection;
using Shockah.Kokoro.UI;
using System.Collections.Generic;

namespace Shockah.Kokoro.Stardew;

public interface ISkill : IEquatable<ISkill>
{
	string UniqueID { get; }
	TextureRectangle? Icon { get; }
	string Name { get; }
	int MaxLevel { get; }

	int GetBaseLevel(Farmer player);
	int GetBuffedLevel(Farmer player);
	int GetXP(Farmer player);

	int GetLevelXP(int level);

	void GrantXP(Farmer player, int xp);
}

public static class SkillExt
{
	private static readonly int[] OrderedSkillIndexes = [0, 3, 2, 1, 4, 5];

	public static IEnumerable<ISkill> GetAllSkills()
	{
		foreach (var skill in VanillaSkill.GetAllSkills())
			yield return skill;
		foreach (var skill in SpaceCoreSkill.GetAllSkills())
			yield return skill;
	}

	public static ISkill GetSkill(int? vanillaSkillIndex, string? spaceCoreSkillName)
	{
		if (spaceCoreSkillName is not null)
			return new SpaceCoreSkill(spaceCoreSkillName);
		else if (vanillaSkillIndex is not null && vanillaSkillIndex.Value is Farmer.farmingSkill or Farmer.miningSkill or Farmer.foragingSkill or Farmer.fishingSkill or Farmer.combatSkill or Farmer.luckSkill)
			return new VanillaSkill(vanillaSkillIndex.Value);
		else
			throw new ArgumentException($"Invalid values of arguments {nameof(vanillaSkillIndex)} ({vanillaSkillIndex}) and {nameof(spaceCoreSkillName)} ({spaceCoreSkillName}).");
	}

	public static ISkill GetSkillFromUI(int? uiSkillIndex, string? spaceCoreSkillName)
	{
		int? skillIndex = uiSkillIndex is null ? null : OrderedSkillIndexes.Length > uiSkillIndex ? OrderedSkillIndexes[uiSkillIndex.Value] : uiSkillIndex;
		return GetSkill(skillIndex, spaceCoreSkillName);
	}
}

public record VanillaSkill(
	int SkillIndex
) : ISkill
{
	public static VanillaSkill Farming { get; private set; } = new(Farmer.farmingSkill);
	public static VanillaSkill Mining { get; private set; } = new(Farmer.miningSkill);
	public static VanillaSkill Foraging { get; private set; } = new(Farmer.foragingSkill);
	public static VanillaSkill Fishing { get; private set; } = new(Farmer.fishingSkill);
	public static VanillaSkill Combat { get; private set; } = new(Farmer.combatSkill);
	public static VanillaSkill Luck { get; private set; } = new(Farmer.luckSkill);

	public static string CropsAspect { get; private set; } = $"{Farming.UniqueID}:Crops";
	public static string AnimalsAspect { get; private set; } = $"{Farming.UniqueID}:Animals";
	public static string FlowersAspect { get; private set; } = $"{Farming.UniqueID}:Flowers";
	public static string MetalAspect { get; private set; } = $"{Mining.UniqueID}:Metal";
	public static string GemAspect { get; private set; } = $"{Mining.UniqueID}:Gem";
	public static string WoodcuttingAspect { get; private set; } = $"{Foraging.UniqueID}:Woodcutting";
	public static string GatheringAspect { get; private set; } = $"{Foraging.UniqueID}:Gathering";
	public static string TappingAspect { get; private set; } = $"{Foraging.UniqueID}:Tapping";
	public static string FishingAspect { get; private set; } = $"{Fishing.UniqueID}:Fishing";
	public static string TrappingAspect { get; private set; } = $"{Fishing.UniqueID}:Trapping";
	public static string PondsAspect { get; private set; } = $"{Fishing.UniqueID}:Ponds";

	private static int[]? XPValues;
	private static DateTime? LastUpdateTime;
	private static WeakReference<IClickableMenu>? LastMenu;

	public static IEnumerable<ISkill> GetAllSkills()
	{
		yield return Farming;
		yield return Mining;
		yield return Foraging;
		yield return Fishing;
		yield return Combat;

		if (ModEntry.Instance.Helper.ModRegistry.IsLoaded("spacechase0.LuckSkill"))
			yield return Luck;
	}

	public string UniqueID
		=> SkillIndex switch
		{
			Farmer.farmingSkill => "Farming",
			Farmer.miningSkill => "Mining",
			Farmer.foragingSkill => "Foraging",
			Farmer.fishingSkill => "Fishing",
			Farmer.combatSkill => "Combat",
			Farmer.luckSkill => "Luck",
			_ => throw new ArgumentException($"{nameof(SkillIndex)} has an invalid value.")
		};

	public TextureRectangle? Icon
		=> new(
			Game1.mouseCursors,
			SkillIndex switch
			{
				Farmer.farmingSkill => new(10, 428, 10, 10),
				Farmer.miningSkill => new(30, 428, 10, 10),
				Farmer.foragingSkill => new(60, 428, 10, 10),
				Farmer.fishingSkill => new(20, 428, 10, 10),
				Farmer.combatSkill => new(120, 428, 10, 10),
				Farmer.luckSkill => new(50, 428, 10, 10),
				_ => throw new ArgumentException($"{nameof(SkillIndex)} has an invalid value.")
			}
		);

	public string Name
		=> SkillIndex switch
		{
			Farmer.farmingSkill => Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11604"),
			Farmer.miningSkill => Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11605"),
			Farmer.foragingSkill => Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11606"),
			Farmer.fishingSkill => Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11607"),
			Farmer.combatSkill => Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11608"),
			Farmer.luckSkill => Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11609"),
			_ => throw new ArgumentException($"{nameof(SkillIndex)} has an invalid value.")
		};

	public int MaxLevel
	{
		get
		{
			UpdateXPValuesIfNeeded();
			return XPValues!.Length;
		}
	}

	public bool Equals(ISkill? other)
		=> other is VanillaSkill skill && skill.SkillIndex == SkillIndex;

	public int GetBaseLevel(Farmer player)
		=> player.GetUnmodifiedSkillLevel(SkillIndex);

	public int GetBuffedLevel(Farmer player)
		=> player.GetSkillLevel(SkillIndex);

	public int GetLevelXP(int level)
	{
		UpdateXPValuesIfNeeded();
		if (level <= 0)
			return 0;
		else if (level > MaxLevel)
			return int.MaxValue;
		else
			return XPValues![level - 1];
	}

	public int GetXP(Farmer player)
		=> player.experiencePoints[SkillIndex];

	public void GrantXP(Farmer player, int xp)
		=> player.gainExperience(SkillIndex, xp);

	private static void UpdateXPValuesIfNeeded()
	{
		static void UpdateXPValues()
		{
			int maxLevel = Farmer.checkForLevelGain(0, int.MaxValue);
			if (maxLevel <= 0)
			{
				XPValues = [];
				return;
			}

			int maxValue = int.MaxValue;
			int minValue = 0;
			int[] values = new int[maxLevel];
			for (int level = maxLevel; level > 0; level--)
			{
				while (maxValue - minValue > 1)
				{
					int midValue = minValue + (maxValue - minValue) / 2;
					int stepLevel = Farmer.checkForLevelGain(0, midValue);
					if (stepLevel >= level)
						maxValue = midValue;
					else
						minValue = midValue;
				}

				values[level - 1] = maxValue;
				minValue = 0;
			}

			XPValues = values;
			LastMenu = Game1.activeClickableMenu is null ? null : new(Game1.activeClickableMenu);
			LastUpdateTime = DateTime.UtcNow;
		}

		if (XPValues is null)
		{
			UpdateXPValues();
			return;
		}

		if (LastMenu is null || !LastMenu.TryGetTarget(out IClickableMenu? lastMenu))
			lastMenu = null;
		if (!ReferenceEquals(lastMenu, Game1.activeClickableMenu))
		{
			UpdateXPValues();
			return;
		}
		else if (LastUpdateTime is null || LastUpdateTime.Value.AddSeconds(1) > DateTime.UtcNow)
		{
			UpdateXPValues();
			return;
		}
	}
}

public record SpaceCoreSkill(
	string SkillName
) : ISkill
{
	private static readonly string SpaceCoreSkillsQualifiedName = "SpaceCore.Skills, SpaceCore";
	private static readonly string SpaceCoreSkillQualifiedName = "SpaceCore.Skills+Skill, SpaceCore";
	private static readonly string SpaceCoreSkillExtensionsQualifiedName = "SpaceCore.SkillExtensions, SpaceCore";

	private static bool IsReflectionSetup = false;
	private static Func<string[]> GetSkillListDelegate = null!;
	private static Func<string, object? /* Skill */> GetSkillDelegate = null!;
	private static Func<Farmer, object /* Skill */, int> GetCustomSkillLevelDelegate = null!;
	private static Func<object /* Skill */, int[]> ExperienceCurveDelegate = null!;
	private static Func<Farmer, object /* Skill */, int> GetCustomSkillExperienceDelegate = null!;
	private static Action<Farmer, string, int> AddExperienceDelegate = null!;
	private static Func<object /* Skill */, string> GetNameDelegate = null!;
	private static Func<object /* Skill */, Texture2D?> GetSkillsPageIconDelegate = null!;

	public static IEnumerable<ISkill> GetAllSkills()
	{
		if (!ModEntry.Instance.Helper.ModRegistry.IsLoaded("spacechase0.SpaceCore"))
			yield break;
		SetupReflectionIfNeeded();
		foreach (var skillName in GetSkillListDelegate())
			yield return new SpaceCoreSkill(skillName);
	}

	public string UniqueID
		=> SkillName;

	public TextureRectangle? Icon
	{
		get
		{
			SetupReflectionIfNeeded();
			object skill = GetSkillDelegate(SkillName)!;
			var texture = GetSkillsPageIconDelegate(skill);
			return texture is null ? null : new(texture, new(0, 0, texture.Width, texture.Height));
		}
	}

	public string Name
	{
		get
		{
			SetupReflectionIfNeeded();
			object skill = GetSkillDelegate(SkillName)!;
			return GetNameDelegate(skill);
		}
	}

	public int MaxLevel
	{
		get
		{
			SetupReflectionIfNeeded();
			object skill = GetSkillDelegate(SkillName)!;
			int[] experienceCurve = ExperienceCurveDelegate(skill);
			return experienceCurve.Length;
		}
	}

	public bool Equals(ISkill? other)
		=> other is SpaceCoreSkill skill && skill.SkillName == SkillName;

	public int GetBaseLevel(Farmer player)
	{
		SetupReflectionIfNeeded();
		object skill = GetSkillDelegate(SkillName)!;
		return GetCustomSkillLevelDelegate(Game1.player, skill);
	}

	public int GetBuffedLevel(Farmer player)
		=> GetBaseLevel(player);

	public int GetLevelXP(int level)
	{
		SetupReflectionIfNeeded();

		if (level <= 0)
			return 0;
		else if (level > MaxLevel)
			return int.MaxValue;

		SetupReflectionIfNeeded();
		object skill = GetSkillDelegate(SkillName)!;
		int[] experienceCurve = ExperienceCurveDelegate(skill);
		return experienceCurve[level - 1];
	}

	public int GetXP(Farmer player)
	{
		SetupReflectionIfNeeded();
		object skill = GetSkillDelegate(SkillName)!;
		return GetCustomSkillExperienceDelegate(Game1.player, skill);
	}

	public void GrantXP(Farmer player, int xp)
	{
		SetupReflectionIfNeeded();
		AddExperienceDelegate(player, SkillName, xp);
	}

	private static void SetupReflectionIfNeeded()
	{
		if (IsReflectionSetup)
			return;

		Type skillsType = AccessTools.TypeByName(SpaceCoreSkillsQualifiedName);
		Type skillType = AccessTools.TypeByName(SpaceCoreSkillQualifiedName);
		Type skillExtensionsType = AccessTools.TypeByName(SpaceCoreSkillExtensionsQualifiedName);

		MethodInfo getSkillListMethod = AccessTools.Method(skillsType, "GetSkillList", []);
		GetSkillListDelegate = () => (string[])getSkillListMethod.Invoke(null, null)!;

		MethodInfo getSkillMethod = AccessTools.Method(skillsType, "GetSkill", [typeof(string)]);
		GetSkillDelegate = (skillName) => getSkillMethod.Invoke(null, [skillName]);

		MethodInfo getCustomSkillLevelMethod = AccessTools.Method(skillExtensionsType, "GetCustomSkillLevel", [typeof(Farmer), skillType]);
		GetCustomSkillLevelDelegate = (farmer, skill) => (int)getCustomSkillLevelMethod.Invoke(null, [farmer, skill])!;

		MethodInfo experienceCurveMethod = AccessTools.PropertyGetter(skillType, "ExperienceCurve");
		ExperienceCurveDelegate = (skill) => (int[])experienceCurveMethod.Invoke(skill, null)!;

		MethodInfo getCustomSkillExperienceMethod = AccessTools.Method(skillExtensionsType, "GetCustomSkillExperience", [typeof(Farmer), skillType]);
		GetCustomSkillExperienceDelegate = (farmer, skill) => (int)getCustomSkillExperienceMethod.Invoke(null, [farmer, skill])!;

		MethodInfo addExperienceMethod = AccessTools.Method(skillsType, "AddExperience", [typeof(Farmer), typeof(string), typeof(int)]);
		AddExperienceDelegate = (farmer, skill, xp) => addExperienceMethod.Invoke(null, [farmer, skill, xp]);

		MethodInfo getNameMethod = AccessTools.Method(skillType, "GetName");
		GetNameDelegate = (skill) => (string)getNameMethod.Invoke(skill, null)!;

		MethodInfo getSkillsPageIconMethod = AccessTools.PropertyGetter(skillType, "SkillsPageIcon");
		GetSkillsPageIconDelegate = (skill) => getSkillsPageIconMethod.Invoke(skill, null) as Texture2D;

		IsReflectionSetup = true;
	}
}