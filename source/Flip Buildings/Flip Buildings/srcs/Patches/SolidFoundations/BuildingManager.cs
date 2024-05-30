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
using StardewValley.Buildings;
using StardewValley.GameData.Buildings;
using FlipBuildings.Utilities;

namespace FlipBuildings.Patches.SF
{
	internal class BuildingManagerPatch
	{
		internal static Building Building;

		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(CompatibilityUtility.SFBuildingManagerType, "GetSpecificBuildingModel", new Type[] { typeof(string) }),
				postfix: new HarmonyMethod(typeof(BuildingManagerPatch), nameof(BuildingManagerGetSpecificBuildingModelPostfix))
			);
		}

		public static void WrapPrefixInstance(object[] __args)
		{
			Building = (Building)__args[0];
		}

		public static void WrapPrefixBuilding(Building building)
		{
			Building = building;
		}

		public static void WrapPostfix()
		{
			Building = null;
		}

		private static void BuildingManagerGetSpecificBuildingModelPostfix(ref BuildingData __result)
		{
			if (Building is not null && __result is not null)
			{
				__result = Building.GetData();
			}
		}
	}
}
