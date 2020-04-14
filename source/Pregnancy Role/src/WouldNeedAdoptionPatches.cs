using Harmony;
using StardewModdingAPI;
using StardewValley;
using System;

namespace PregnancyRole
{
	internal class WouldNeedAdoptionPatches
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;
		protected static HarmonyInstance Harmony => ModEntry.Instance.harmony;

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
