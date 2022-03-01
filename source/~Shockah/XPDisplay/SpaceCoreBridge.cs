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
using StardewValley;
using System;
using System.Reflection;

namespace Shockah.XPView
{
	internal static class SpaceCoreBridge
	{
		private static readonly string SpaceCoreSkillsQualifiedName = "SpaceCore.Skills, SpaceCore";
		private static readonly string SpaceCoreSkillQualifiedName = "SpaceCore.Skills+Skill, SpaceCore";
		private static readonly string SpaceCoreSkillExtensionsQualifiedName = "SpaceCore.SkillExtensions, SpaceCore";

		private static bool IsReflectionSetup = false;
		private static Func<string, object? /* Skill */> GetSkillDelegate = null!;
		private static Func<Farmer, object /* Skill */, int> GetCustomSkillLevelDelegate = null!;
		private static Func<object /* Skill */, int[]> ExperienceCurveDelegate = null!;
		private static Func<Farmer, object /* Skill */, int> GetCustomSkillExperienceDelegate = null!;

		private static void SetupReflectionIfNeeded()
		{
			if (IsReflectionSetup)
				return;

			Type skillsType = AccessTools.TypeByName(SpaceCoreSkillsQualifiedName);
			Type skillType = AccessTools.TypeByName(SpaceCoreSkillQualifiedName);
			Type skillExtensionsType = AccessTools.TypeByName(SpaceCoreSkillExtensionsQualifiedName);

			MethodInfo getSkillMethod = AccessTools.Method(skillsType, "GetSkill", new Type[] { typeof(string) });
			GetSkillDelegate = (skillName) => getSkillMethod.Invoke(null, new object[] { skillName });

			MethodInfo getCustomSkillLevelMethod = AccessTools.Method(skillExtensionsType, "GetCustomSkillLevel", new Type[] { typeof(Farmer), skillType });
			GetCustomSkillLevelDelegate = (farmer, skill) => (int)getCustomSkillLevelMethod.Invoke(null, new object[] { farmer, skill })!;

			MethodInfo experienceCurveMethod = AccessTools.PropertyGetter(skillType, "ExperienceCurve");
			ExperienceCurveDelegate = (skill) => (int[])experienceCurveMethod.Invoke(skill, null)!;

			MethodInfo getCustomSkillExperienceMethod = AccessTools.Method(skillExtensionsType, "GetCustomSkillExperience", new Type[] { typeof(Farmer), skillType });
			GetCustomSkillExperienceDelegate = (farmer, skill) => (int)getCustomSkillExperienceMethod.Invoke(null, new object[] { farmer, skill })!;

			IsReflectionSetup = true;
		}

		internal static int GetUnmodifiedSkillLevel(string spaceCoreSkillName)
		{
			SetupReflectionIfNeeded();
			object skill = GetSkillDelegate(spaceCoreSkillName)!;
			return GetCustomSkillLevelDelegate(Game1.player, skill);
		}

		internal static int GetLevelXP(int levelIndex, string spaceCoreSkillName)
		{
			SetupReflectionIfNeeded();
			object skill = GetSkillDelegate(spaceCoreSkillName)!;
			int[] experienceCurve = ExperienceCurveDelegate(skill);
			return experienceCurve.Length > levelIndex ? experienceCurve[levelIndex] : int.MaxValue;
		}

		internal static int GetCurrentXP(string spaceCoreSkillName)
		{
			SetupReflectionIfNeeded();
			object skill = GetSkillDelegate(spaceCoreSkillName)!;
			return GetCustomSkillExperienceDelegate(Game1.player, skill);
		}
	}
}
