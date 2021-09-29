/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Reflection;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class MonsterWithinPlayerThresholdPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal MonsterWithinPlayerThresholdPatch()
		{
			Original = typeof(Monster).MethodNamed(nameof(Monster.withinPlayerThreshold), new Type[] { });
			Prefix = new HarmonyMethod(GetType(), nameof(MonsterWithinPlayerThresholdPrefix));
		}

		#region harmony patch

		/// <summary>Patch to make Poacher invisible in super mode.</summary>
		[HarmonyTranspiler]
		private static bool MonsterWithinPlayerThresholdPrefix(Monster __instance, ref bool __result)
		{
			try
			{
				var foundPlayer = ModEntry.ModHelper.Reflection.GetMethod(__instance, name: "findPlayer").Invoke<Farmer>();
				if (!foundPlayer.IsLocalPlayer || !ModEntry.IsSuperModeActive ||
					ModEntry.SuperModeIndex != Util.Professions.IndexOf("Poacher")) return true; // run original method

				__result = false;
				return false; // don't run original method
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod().Name}:\n{ex}", LogLevel.Error);
				return true; // default to original logic
			}
		}

		#endregion harmony patch
	}
}
