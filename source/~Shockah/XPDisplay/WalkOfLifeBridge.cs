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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Reflection;

namespace Shockah.XPView
{
	internal static class WalkOfLifeBridge
	{
		private static readonly string WalkOfLifeModEntryQualifiedName = "DaLion.Stardew.Professions.ModEntry, AwesomeProfessions";
		private static readonly string WalkOfLifeModConfigQualifiedName = "DaLion.Stardew.Professions.ModConfig, AwesomeProfessions";
		private static readonly string WalkOfLifeTexturesQualifiedName = "DaLion.Stardew.Professions.Framework.Utility.Textures, AwesomeProfessions";

		private static bool IsReflectionSetup = false;
		private static Func<object /* ModConfig */> ConfigDelegate = null!;
		private static Func<object /* ModConfig */, bool> IsPrestigeEnabledDelegate = null!;
		private static Func<object /* ModConfig */, int> RequiredExpPerExtendedLevelDelegate = null!;
		private static Func<Texture2D> SkillBarTxDelegate = null!;

		private static void SetupReflectionIfNeeded()
		{
			if (IsReflectionSetup)
				return;

			Type modEntryType = AccessTools.TypeByName(WalkOfLifeModEntryQualifiedName);
			Type modConfigType = AccessTools.TypeByName(WalkOfLifeModConfigQualifiedName);
			Type texturesType = AccessTools.TypeByName(WalkOfLifeTexturesQualifiedName);

			MethodInfo configMethod = AccessTools.PropertyGetter(modEntryType, "Config");
			ConfigDelegate = () => configMethod.Invoke(null, null)!;

			MethodInfo isPrestigeEnabledMethod = AccessTools.PropertyGetter(modConfigType, "EnablePrestige");
			IsPrestigeEnabledDelegate = (config) => (bool)isPrestigeEnabledMethod.Invoke(config, null)!;

			MethodInfo requiredExpPerExtendedLevelmethod = AccessTools.PropertyGetter(modConfigType, "RequiredExpPerExtendedLevel");
			RequiredExpPerExtendedLevelDelegate = (config) => (int)(uint)requiredExpPerExtendedLevelmethod.Invoke(config, null)!;

			MethodInfo skillBarTxMethod = AccessTools.PropertyGetter(texturesType, "SkillBarTx");
			SkillBarTxDelegate = () => (Texture2D)skillBarTxMethod.Invoke(null, null)!;

			IsReflectionSetup = true;
		}

		public static bool IsPrestigeEnabled()
		{
			SetupReflectionIfNeeded();
			object config = ConfigDelegate();
			return IsPrestigeEnabledDelegate(config);
		}

		public static int GetRequiredXPPerExtendedLevel()
		{
			SetupReflectionIfNeeded();
			object config = ConfigDelegate();
			return RequiredExpPerExtendedLevelDelegate(config);
		}

		public static (Texture2D, Rectangle) GetExtendedSmallBar()
		{
			SetupReflectionIfNeeded();
			return (SkillBarTxDelegate(), new(0, 0, 7, 9));
		}

		public static (Texture2D, Rectangle) GetExtendedBigBar()
		{
			SetupReflectionIfNeeded();
			return (SkillBarTxDelegate(), new(16, 0, 13, 9));
		}
	}
}
