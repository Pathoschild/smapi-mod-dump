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
using Shockah.Kokoro;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SObject = StardewValley.Object;

namespace Shockah.FlexibleSprinklers
{
	internal static class VanillaPatches
	{
		internal static bool IsVanillaQueryInProgress = false;
		internal static Vector2? SprinklerTileOverride;

		internal static void Apply(Harmony harmony)
		{
			harmony.TryPatch(
				monitor: ModEntry.Instance.Monitor,
				original: () => AccessTools.DeclaredMethod(typeof(SObject), nameof(SObject.GetSprinklerTiles)),
				prefix: new HarmonyMethod(typeof(VanillaPatches), nameof(Object_GetSprinklerTiles_Prefix)),
				postfix: new HarmonyMethod(typeof(VanillaPatches), nameof(Object_GetSprinklerTiles_Postfix))
			);

			harmony.TryPatch(
				monitor: ModEntry.Instance.Monitor,
				original: () => AccessTools.DeclaredMethod(typeof(SObject), nameof(SObject.IsInSprinklerRangeBroadphase)),
				prefix: new HarmonyMethod(typeof(VanillaPatches), nameof(Object_IsInSprinklerRangeBroadphase_Prefix)),
				postfix: new HarmonyMethod(typeof(VanillaPatches), nameof(Object_IsInSprinklerRangeBroadphase_Postfix))
			);

			harmony.TryPatch(
				monitor: ModEntry.Instance.Monitor,
				original: () => AccessTools.DeclaredMethod(typeof(SObject), nameof(SObject.ApplySprinklerAnimation)),
				prefix: new HarmonyMethod(typeof(VanillaPatches), nameof(Object_ApplySprinklerAnimation_Prefix))
			);

			foreach (var method in typeof(SObject).GetTypeInfo().DeclaredMethods)
			{
				if (!method.Name.StartsWith("<DayUpdate>"))
					continue;

				harmony.TryPatch(
					monitor: ModEntry.Instance.Monitor,
					original: () => method,
					prefix: new HarmonyMethod(typeof(VanillaPatches), nameof(Object_DayUpdatePostFarmEventOvernightActionsDelegate_Prefix))
				);
				goto done;
			}

			ModEntry.Instance.Monitor.Log($"Could not patch base methods - FlexibleSprinklers probably won't work.\nReason: Cannot patch DayUpdate/PostFarmEventOvernightActions/Delegate.", LogLevel.Error);
			done:;
		}

		private static List<Vector2> Object_GetSprinklerTiles_Result(SObject __instance)
		{
			if (SprinklerTileOverride is not null)
			{
				var result = new List<Vector2> { SprinklerTileOverride.Value };
				SprinklerTileOverride = null;
				return result;
			}
			if (__instance.Location is null)
				return [];

			if (ModEntry.Instance.SprinklerBehavior is ISprinklerBehavior.Independent independent)
			{
				return independent.GetSprinklerTiles(
					new GameLocationMap(__instance.Location, ModEntry.Instance.CustomWaterableTileProviders),
					ModEntry.Instance.GetSprinklerInfo(__instance)
				).Select(e => new Vector2(e.X, e.Y)).ToList();
			}
			else
			{
				return [];
			}
		}

		private static bool Object_GetSprinklerTiles_Prefix(SObject __instance, ref List<Vector2> __result)
		{
			if (IsVanillaQueryInProgress)
				return true;
			if (ModEntry.Instance.Config.CompatibilityMode)
				return true;
			__result = Object_GetSprinklerTiles_Result(__instance);
			return false;
		}

		private static void Object_GetSprinklerTiles_Postfix(SObject __instance, ref List<Vector2> __result)
		{
			if (IsVanillaQueryInProgress)
				return;
			if (!ModEntry.Instance.Config.CompatibilityMode)
				return;
			__result = Object_GetSprinklerTiles_Result(__instance);
		}

		private static bool Object_IsInSprinklerRangeBroadphase_Result(SObject __instance, Vector2 target)
		{
			if (__instance.Location is null)
				return true;

			var wasVanillaQueryInProgress = IsVanillaQueryInProgress;
			IsVanillaQueryInProgress = true;
			var manhattanDistance = Math.Abs(target.X - __instance.TileLocation.X) + Math.Abs(target.Y - __instance.TileLocation.Y);
			var result = manhattanDistance <= ModEntry.Instance.GetSprinklerMaxRange(__instance) && ModEntry.Instance.IsTileInRangeOfAnySprinkler(__instance.Location, new IntPoint((int)target.X, (int)target.Y));
			IsVanillaQueryInProgress = wasVanillaQueryInProgress;
			if (result)
				SprinklerTileOverride = target;
			return result;
		}

		private static bool Object_IsInSprinklerRangeBroadphase_Prefix(SObject __instance, Vector2 target, ref bool __result)
		{
			if (ModEntry.Instance.Config.CompatibilityMode)
				return true;
			__result = Object_IsInSprinklerRangeBroadphase_Result(__instance, target);
			return false;
		}

		private static void Object_IsInSprinklerRangeBroadphase_Postfix(SObject __instance, Vector2 target, ref bool __result)
		{
			if (!ModEntry.Instance.Config.CompatibilityMode)
				return;
			__result = Object_IsInSprinklerRangeBroadphase_Result(__instance, target);
		}

		private static void Object_ApplySprinklerAnimation_Prefix(SObject __instance)
		{
			if (__instance.Location is null)
				return;

			// remove all temporary sprites related to this sprinkler
			for (int i = __instance.Location.TemporarySprites.Count - 1; i >= 0; i--)
			{
				var sprite = __instance.Location.TemporarySprites[i];
				if (sprite.id == __instance.TileLocation.X * 4000f + __instance.TileLocation.Y)
					__instance.Location.TemporarySprites.RemoveAt(i);
			}
		}

		private static bool Object_DayUpdatePostFarmEventOvernightActionsDelegate_Prefix()
		{
			return false;
		}
	}
}