/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/BuildableGingerIslandFarm
**
*************************************************/

using System;
using HarmonyLib;
using StardewValley.Buildings;

namespace BuildableGingerIslandFarm.Patches
{
	internal class JunimoHutPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(JunimoHut), nameof(JunimoHut.dayUpdate), new Type[] { typeof(int) }),
				postfix: new HarmonyMethod(typeof(JunimoHutPatch), nameof(DayUpdatePostfix))
			);
		}

		private static void DayUpdatePostfix(JunimoHut __instance)
		{
			__instance.shouldSendOutJunimos.Value = true;
		}
	}
}
