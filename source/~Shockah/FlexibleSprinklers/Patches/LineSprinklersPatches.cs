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
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace Shockah.FlexibleSprinklers
{
	internal static class LineSprinklersPatches
	{
		private static readonly string LineSprinklersModEntryQualifiedName = "LineSprinklers.ModEntry, LineSprinklers";

		private static bool IsDuringDayStarted = false;

		internal static void Apply(Harmony harmony)
		{
			try
			{
				harmony.Patch(
					original: AccessTools.Method(Type.GetType(LineSprinklersModEntryQualifiedName), "OnDayStarted"),
					prefix: new HarmonyMethod(typeof(LineSprinklersPatches), nameof(OnDayStarted_Prefix)),
					postfix: new HarmonyMethod(typeof(LineSprinklersPatches), nameof(OnDayStarted_Postfix))
				);

				harmony.Patch(
					original: AccessTools.Method(Type.GetType(LineSprinklersModEntryQualifiedName), "GetLocations"),
					prefix: new HarmonyMethod(typeof(LineSprinklersPatches), nameof(GetLocations_Prefix))
				);

				harmony.Patch(
					original: AccessTools.Method(typeof(SObject), nameof(SObject.IsSprinkler)),
					postfix: new HarmonyMethod(typeof(LineSprinklersPatches), nameof(Object_IsSprinkler_Postfix))
				);

				harmony.Patch(
					original: AccessTools.Method(typeof(SObject), nameof(SObject.GetBaseRadiusForSprinkler)),
					prefix: new HarmonyMethod(typeof(LineSprinklersPatches), nameof(Object_GetBaseRadiusForSprinkler_Prefix)),
					postfix: new HarmonyMethod(typeof(LineSprinklersPatches), nameof(Object_GetBaseRadiusForSprinkler_Postfix))
				);
			}
			catch (Exception e)
			{
				FlexibleSprinklers.Instance.Monitor.Log($"Could not patch LineSprinklers - they probably won't work.\nReason: {e}", LogLevel.Warn);
			}
		}

		internal static bool OnDayStarted_Prefix()
		{
			IsDuringDayStarted = true;
			return true;
		}

		internal static void OnDayStarted_Postfix()
		{
			IsDuringDayStarted = false;
		}

		internal static bool GetLocations_Prefix(ref IEnumerable<GameLocation> __result)
		{
			if (IsDuringDayStarted)
			{
				__result = new List<GameLocation>();
				return false;
			}
			else
			{
				return true;
			}
		}

		internal static void Object_IsSprinkler_Postfix(SObject __instance, ref bool __result)
		{
			__result = __result || FlexibleSprinklers.Instance.LineSprinklersApi!.GetSprinklerCoverage().ContainsKey(__instance.ParentSheetIndex);
		}

		private static int? Object_GetBaseRadiusForSprinkler_Result(SObject instance)
		{
			if (FlexibleSprinklers.Instance.LineSprinklersApi!.GetSprinklerCoverage().TryGetValue(instance.ParentSheetIndex, out Vector2[]? tilePositions))
			{
				return (int)Math.Sqrt(tilePositions.Length / 2) - 1;
			}
			else
			{
				return null;
			}
		}

		internal static bool Object_GetBaseRadiusForSprinkler_Prefix(SObject __instance, ref int __result)
		{
			if (FlexibleSprinklers.Instance.Config.CompatibilityMode)
				return true;
			var radius = Object_GetBaseRadiusForSprinkler_Result(__instance);
			if (radius is null)
			{
				return true;
			}
			else
			{
				__result = radius.Value;
				return false;
			}
		}

		internal static void Object_GetBaseRadiusForSprinkler_Postfix(SObject __instance, ref int __result)
		{
			if (!FlexibleSprinklers.Instance.Config.CompatibilityMode)
				return;
			var radius = Object_GetBaseRadiusForSprinkler_Result(__instance);
			if (radius is not null)
				__result = radius.Value;
		}
	}
}