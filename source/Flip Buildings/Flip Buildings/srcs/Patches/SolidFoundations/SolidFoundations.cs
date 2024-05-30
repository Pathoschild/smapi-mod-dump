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
using FlipBuildings.Utilities;

namespace FlipBuildings.Patches.SF
{
	internal class SolidFoundationsPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(CompatibilityUtility.SFSolidFoundationsType, "LoadContentPacks", new Type[] { typeof(bool) }),
				postfix: new HarmonyMethod(typeof(BuildingDataUtility), nameof(BuildingDataUtility.LoadContent))
			);
		}
	}
}
