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
using Shockah.CommonModCode;
using StardewModdingAPI;
using StardewValley;
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
		internal static GameLocation? CurrentLocation;
		internal static Vector2? SprinklerTileOverride;

		internal static void Apply(Harmony harmony)
		{
			try
			{
				harmony.Patch(
					original: AccessTools.Method(typeof(SObject), nameof(SObject.GetSprinklerTiles)),
					prefix: new HarmonyMethod(typeof(VanillaPatches), nameof(Object_GetSprinklerTiles_Prefix)),
					postfix: new HarmonyMethod(typeof(VanillaPatches), nameof(Object_GetSprinklerTiles_Postfix))
				);

				harmony.Patch(
					original: AccessTools.Method(typeof(SObject), nameof(SObject.placementAction)),
					prefix: new HarmonyMethod(typeof(VanillaPatches), nameof(Object_placementAction_Prefix))
				);

				harmony.Patch(
					original: AccessTools.Method(typeof(SObject), nameof(SObject.IsInSprinklerRangeBroadphase)),
					prefix: new HarmonyMethod(typeof(VanillaPatches), nameof(Object_IsInSprinklerRangeBroadphase_Prefix)),
					postfix: new HarmonyMethod(typeof(VanillaPatches), nameof(Object_IsInSprinklerRangeBroadphase_Postfix))
				);

				harmony.Patch(
					original: AccessTools.Method(typeof(SObject), nameof(SObject.ApplySprinklerAnimation)),
					prefix: new HarmonyMethod(typeof(VanillaPatches), nameof(Object_ApplySprinklerAnimation_Prefix))
				);

				harmony.Patch(
					original: AccessTools.Method(typeof(Game1), "handlePostFarmEventActions"),
					postfix: new HarmonyMethod(typeof(VanillaPatches), nameof(Game1_handlePostFarmEventActions_Postfix))
				);

				foreach (var nestedType in typeof(SObject).GetTypeInfo().DeclaredNestedTypes)
				{
					if (!nestedType.DeclaredFields.Where(f => f.FieldType == typeof(SObject) && f.Name.EndsWith("__this")).Any())
						continue;
					if (!nestedType.DeclaredFields.Where(f => f.FieldType == typeof(GameLocation) && f.Name == "location").Any())
						continue;

					foreach (var method in nestedType.DeclaredMethods)
					{
						if (!method.Name.StartsWith("<DayUpdate>"))
							continue;

						harmony.Patch(
							original: method,
							prefix: new HarmonyMethod(typeof(VanillaPatches), nameof(Object_DayUpdatePostFarmEventOvernightActionsDelegate_Prefix))
						);
						goto done;
					}
				}

				FlexibleSprinklers.Instance.Monitor.Log($"Could not patch base methods - FlexibleSprinklers probably won't work.\nReason: Cannot patch DayUpdate/PostFarmEventOvernightActions/Delegate.", LogLevel.Error);
				done:;
			}
			catch (Exception e)
			{
				FlexibleSprinklers.Instance.Monitor.Log($"Could not patch base methods - FlexibleSprinklers probably won't work.\nReason: {e}", LogLevel.Error);
			}
		}

		private static List<Vector2> Object_GetSprinklerTiles_Result(SObject __instance)
		{
			if (SprinklerTileOverride is not null)
			{
				var result = new List<Vector2> { SprinklerTileOverride.Value };
				SprinklerTileOverride = null;
				return result;
			}
			
			if (CurrentLocation is null)
			{
				FlexibleSprinklers.Instance.Monitor.Log("Location should not be null - potential mod conflict.", LogLevel.Error);
				return new List<Vector2>();
			}

			if (FlexibleSprinklers.Instance.SprinklerBehavior is ISprinklerBehavior.Independent independent)
			{
				return independent.GetSprinklerTiles(
					new GameLocationMap(CurrentLocation, FlexibleSprinklers.Instance.CustomWaterableTileProviders),
					new IntPoint((int)__instance.TileLocation.X, (int)__instance.TileLocation.Y),
					FlexibleSprinklers.Instance.GetSprinklerInfo(__instance)
				).Select(e => new Vector2(e.X, e.Y)).ToList();
			}
			else
			{
				return new List<Vector2>();
			}
		}

		private static bool Object_GetSprinklerTiles_Prefix(SObject __instance, ref List<Vector2> __result)
		{
			if (IsVanillaQueryInProgress)
				return true;
			if (FlexibleSprinklers.Instance.Config.CompatibilityMode)
				return true;
			__result = Object_GetSprinklerTiles_Result(__instance);
			return false;
		}

		private static void Object_GetSprinklerTiles_Postfix(SObject __instance, ref List<Vector2> __result)
		{
			if (IsVanillaQueryInProgress)
				return;
			if (!FlexibleSprinklers.Instance.Config.CompatibilityMode)
				return;
			__result = Object_GetSprinklerTiles_Result(__instance);
		}

		private static bool Object_placementAction_Prefix(GameLocation location)
		{
			CurrentLocation = location;
			return true;
		}

		private static bool Object_IsInSprinklerRangeBroadphase_Result(SObject __instance, Vector2 target)
		{
			if (CurrentLocation == null)
			{
				FlexibleSprinklers.Instance.Monitor.Log("Location should not be null - potential mod conflict.", LogLevel.Error);
				return true;
			}

			var wasVanillaQueryInProgress = IsVanillaQueryInProgress;
			IsVanillaQueryInProgress = true;
			var manhattanDistance = Math.Abs(target.X - __instance.TileLocation.X) + Math.Abs(target.Y - __instance.TileLocation.Y);
			var result = manhattanDistance <= FlexibleSprinklers.Instance.GetFloodFillSprinklerRange(FlexibleSprinklers.Instance.GetSprinklerPower(__instance))
				&& FlexibleSprinklers.Instance.IsTileInRangeOfAnySprinkler(CurrentLocation, target);
			IsVanillaQueryInProgress = wasVanillaQueryInProgress;
			if (result)
				SprinklerTileOverride = target;
			return result;
		}

		private static bool Object_IsInSprinklerRangeBroadphase_Prefix(SObject __instance, Vector2 target, ref bool __result)
		{
			if (FlexibleSprinklers.Instance.Config.CompatibilityMode)
				return true;
			__result = Object_IsInSprinklerRangeBroadphase_Result(__instance, target);
			return false;
		}

		private static void Object_IsInSprinklerRangeBroadphase_Postfix(SObject __instance, Vector2 target, ref bool __result)
		{
			if (!FlexibleSprinklers.Instance.Config.CompatibilityMode)
				return;
			__result = Object_IsInSprinklerRangeBroadphase_Result(__instance, target);
		}

		private static void Object_ApplySprinklerAnimation_Prefix(SObject __instance, GameLocation location)
		{
			// remove all temporary sprites related to this sprinkler
			location.TemporarySprites.RemoveAll(sprite => sprite.id == __instance.TileLocation.X * 4000f + __instance.TileLocation.Y);
		}

		private static bool Object_DayUpdatePostFarmEventOvernightActionsDelegate_Prefix(object __instance)
		{
			var locationField = __instance.GetType().GetTypeInfo().DeclaredFields.First(f => f.FieldType == typeof(GameLocation) && f.Name == "location");
			CurrentLocation = (GameLocation?)locationField.GetValue(__instance);
			IsVanillaQueryInProgress = false;
			return FlexibleSprinklers.Instance.SprinklerBehavior is ISprinklerBehavior.Independent;
		}

		private static void Game1_handlePostFarmEventActions_Postfix()
		{
			if (FlexibleSprinklers.Instance.SprinklerBehavior is not ISprinklerBehavior.Independent)
				FlexibleSprinklers.Instance.ActivateAllSprinklers();
		}
	}
}