/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/FlipBuildings
**
*************************************************/

using System;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using FlipBuildings.Utilities;
using StardewValley;

namespace FlipBuildings.Patches.SF
{
	internal class BuildingPatchPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(CompatibilityUtility.SFBuildingPatchType, "DoActionPrefix", new Type[] { typeof(Building), typeof(Vector2), typeof(Farmer), typeof(bool).MakeByRefType() }),
				prefix: new HarmonyMethod(typeof(BuildingManagerPatch), nameof(BuildingManagerPatch.WrapPrefixInstance))
			);
			harmony.Patch(
				original: AccessTools.Method(CompatibilityUtility.SFBuildingPatchType, "DoActionPrefix", new Type[] { typeof(Building), typeof(Vector2), typeof(Farmer), typeof(bool).MakeByRefType() }),
				postfix: new HarmonyMethod(typeof(BuildingManagerPatch), nameof(BuildingManagerPatch.WrapPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(CompatibilityUtility.SFBuildingPatchType, "UpdateInteriorWarpsPostfix", new Type[] { typeof(Building), typeof(GameLocation) }),
				prefix: new HarmonyMethod(typeof(BuildingManagerPatch), nameof(BuildingManagerPatch.WrapPrefixInstance))
			);
			harmony.Patch(
				original: AccessTools.Method(CompatibilityUtility.SFBuildingPatchType, "UpdateInteriorWarpsPostfix", new Type[] { typeof(Building), typeof(GameLocation) }),
				postfix: new HarmonyMethod(typeof(BuildingManagerPatch), nameof(BuildingManagerPatch.WrapPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(CompatibilityUtility.SFBuildingPatchType, "UpdatePostfix", new Type[] { typeof(Building), typeof(GameTime) }),
				prefix: new HarmonyMethod(typeof(BuildingManagerPatch), nameof(BuildingManagerPatch.WrapPrefixInstance))
			);
			harmony.Patch(
				original: AccessTools.Method(CompatibilityUtility.SFBuildingPatchType, "UpdatePostfix", new Type[] { typeof(Building), typeof(GameTime) }),
				postfix: new HarmonyMethod(typeof(BuildingManagerPatch), nameof(BuildingManagerPatch.WrapPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(CompatibilityUtility.SFBuildingPatchType, "LoadPostfix", new Type[] { typeof(Building) }),
				prefix: new HarmonyMethod(typeof(BuildingManagerPatch), nameof(BuildingManagerPatch.WrapPrefixInstance))
			);
			harmony.Patch(
				original: AccessTools.Method(CompatibilityUtility.SFBuildingPatchType, "LoadPostfix", new Type[] { typeof(Building) }),
				postfix: new HarmonyMethod(typeof(BuildingManagerPatch), nameof(BuildingManagerPatch.WrapPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(CompatibilityUtility.SFBuildingPatchType, "DayUpdatePostfix", new Type[] { typeof(Building), typeof(int) }),
				prefix: new HarmonyMethod(typeof(BuildingManagerPatch), nameof(BuildingManagerPatch.WrapPrefixInstance))
			);
			harmony.Patch(
				original: AccessTools.Method(CompatibilityUtility.SFBuildingPatchType, "DayUpdatePostfix", new Type[] { typeof(Building), typeof(int) }),
				postfix: new HarmonyMethod(typeof(BuildingManagerPatch), nameof(BuildingManagerPatch.WrapPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(CompatibilityUtility.SFBuildingPatchType, "GetBuildingSourceRect", new Type[] { typeof(Building) }),
				prefix: new HarmonyMethod(typeof(BuildingManagerPatch), nameof(BuildingManagerPatch.WrapPrefixInstance))
			);
			harmony.Patch(
				original: AccessTools.Method(CompatibilityUtility.SFBuildingPatchType, "GetBuildingSourceRect", new Type[] { typeof(Building) }),
				postfix: new HarmonyMethod(typeof(BuildingManagerPatch), nameof(BuildingManagerPatch.WrapPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(CompatibilityUtility.SFBuildingPatchType, "BuildingPostfix", new Type[] { typeof(Building) }),
				prefix: new HarmonyMethod(typeof(BuildingManagerPatch), nameof(BuildingManagerPatch.WrapPrefixInstance))
			);
			harmony.Patch(
				original: AccessTools.Method(CompatibilityUtility.SFBuildingPatchType, "BuildingPostfix", new Type[] { typeof(Building) }),
				postfix: new HarmonyMethod(typeof(BuildingManagerPatch), nameof(BuildingManagerPatch.WrapPostfix))
			);
		}
	}
}
