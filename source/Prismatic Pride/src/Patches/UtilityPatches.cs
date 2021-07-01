/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/prismaticpride
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;

namespace PrismaticPride
{
	internal static class UtilityPatches
	{
		private static IModHelper Helper => ModEntry.Instance.Helper;
		private static IMonitor Monitor => ModEntry.Instance.Monitor;
		private static ColorData ColorData => ModEntry.Instance.colorData;
		private static HarmonyInstance Harmony => ModEntry.Instance.harmony;

		public static void Apply ()
		{
			Harmony.Patch (
				original: AccessTools.Method (typeof (Utility),
					nameof (Utility.GetPrismaticColor)),
				postfix: new HarmonyMethod (typeof (UtilityPatches),
					nameof (UtilityPatches.GetPrismaticColor_Postfix))
			);
		}

#pragma warning disable IDE1006

		public static void GetPrismaticColor_Postfix (ref Color __result,
			int offset = 0, float speedMultiplier = 1)
		{
			try
			{
				__result = ColorData.getCurrentColor (offset, speedMultiplier);
			}
			catch (Exception e)
			{
				Monitor.Log ($"Failed in {nameof (GetPrismaticColor_Postfix)}:\n{e}",
					LogLevel.Error);
				Monitor.Log (e.StackTrace, LogLevel.Trace);
			}
		}
	}
}
