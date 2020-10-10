/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/pregnancyrole
**
*************************************************/

using Harmony;
using StardewModdingAPI;
using StardewValley;
using System;

namespace PregnancyRole
{
	internal static class WouldNeedAdoptionPatches
	{
		private static IModHelper Helper => ModEntry.Instance.Helper;
		private static IMonitor Monitor => ModEntry.Instance.Monitor;
		private static HarmonyInstance Harmony => ModEntry.Instance.harmony;
		private static ModConfig Config => ModConfig.Instance;

		public static void Apply ()
		{
			Harmony.Patch (
				original: AccessTools.Method (typeof (NPC),
					nameof (NPC.isGaySpouse)),
				prefix: new HarmonyMethod (typeof (WouldNeedAdoptionPatches),
					nameof (WouldNeedAdoptionPatches.NPC_isGaySpouse_Prefix))
			);
		}

		public static bool NPC_isGaySpouse_Prefix (NPC __instance,
			ref bool __result)
		{
			try
			{
				if (Config.VerboseLogging)
					Monitor.Log ($"Overriding adoption status for pregnancy event", LogLevel.Debug);
				__result = Model.WouldNeedAdoption (__instance);
				return false;
			}
			catch (Exception e)
			{
				Monitor.Log ($"Failed in {nameof (NPC_isGaySpouse_Prefix)}:\n{e}",
					LogLevel.Error);
				return true;
			}
		}
	}
}
