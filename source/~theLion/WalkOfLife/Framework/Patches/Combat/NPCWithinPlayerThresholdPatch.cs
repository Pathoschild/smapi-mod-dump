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
	internal class NPCWithinPlayerThresholdPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal NPCWithinPlayerThresholdPatch()
		{
			Original = typeof(NPC).MethodNamed(nameof(NPC.withinPlayerThreshold), new[] { typeof(int) });
			Prefix = new HarmonyMethod(GetType(), nameof(NPCWithinPlayerThresholdPrefix));
		}

		#region harmony patch

		/// <summary>Patch to make Hunter invisible in Super Mode.</summary>
		[HarmonyTranspiler]
		private static bool NPCWithinPlayerThresholdPrefix(NPC __instance, ref bool __result)
		{
			try
			{
				if (__instance is not Monster monster) return true; // run original method

				var foundPlayer = ModEntry.Reflection.GetMethod(monster, name: "findPlayer").Invoke<Farmer>();
				if (!foundPlayer.IsLocalPlayer || !ModEntry.IsSuperModeActive ||
					ModEntry.SuperModeIndex != Util.Professions.IndexOf("Hunter")) return true; // run original method

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
