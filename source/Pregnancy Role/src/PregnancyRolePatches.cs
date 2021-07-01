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
using StardewValley.Events;
using System;
using System.Collections.Generic;

namespace PregnancyRole
{
	internal static class PregnancyRolePatches
	{
		private static IModHelper Helper => ModEntry.Instance.Helper;
		private static IMonitor Monitor => ModEntry.Instance.Monitor;
		private static HarmonyInstance Harmony => ModEntry.Instance.harmony;
		private static ModConfig Config => ModConfig.Instance;

		private static readonly Dictionary<Farmer, bool> GenderOverrides = new ();

		public static void Apply ()
		{
			Harmony.Patch (
				original: AccessTools.Method (typeof (FarmerTeam),
					"handleIncomingProposal"),
				prefix: new HarmonyMethod (typeof (PregnancyRolePatches),
					nameof (PregnancyRolePatches.Prefix)),
				postfix: new HarmonyMethod (typeof (PregnancyRolePatches),
					nameof (PregnancyRolePatches.Postfix))
			);

			Harmony.Patch (
				original: AccessTools.Method (typeof (FarmerTeam),
					"genderedKey"),
				prefix: new HarmonyMethod (typeof (PregnancyRolePatches),
					nameof (PregnancyRolePatches.genderedKey_Prefix))
			);

			Harmony.Patch (
				original: AccessTools.Method (typeof (PlayerCoupleBirthingEvent),
					nameof (PlayerCoupleBirthingEvent.setUp)),
				prefix: new HarmonyMethod (typeof (PregnancyRolePatches),
					nameof (PregnancyRolePatches.Prefix)),
				postfix: new HarmonyMethod (typeof (PregnancyRolePatches),
					nameof (PregnancyRolePatches.Postfix))
			);

			Harmony.Patch (
				original: AccessTools.Method (typeof (QuestionEvent),
					nameof (QuestionEvent.setUp)),
				prefix: new HarmonyMethod (typeof (PregnancyRolePatches),
					nameof (PregnancyRolePatches.Prefix)),
				postfix: new HarmonyMethod (typeof (PregnancyRolePatches),
					nameof (PregnancyRolePatches.Postfix))
			);
		}

#pragma warning disable IDE1006

		public static bool Prefix ()
		{
			try
			{
				long? spouseID = Game1.player.team.GetSpouse
					(Game1.player.UniqueMultiplayerID);
				if (spouseID.HasValue)
				{
					Game1.otherFarmers.TryGetValue (spouseID.Value,
						out Farmer spouse);
					if (spouse != null)
						OverrideFarmers (Game1.player, spouse);
				}
			}
			catch (Exception e)
			{
				Monitor.Log ($"Failed in {nameof (Prefix)}:\n{e}",
					LogLevel.Error);
			}
			return true;
		}

		public static void Postfix ()
		{
			try
			{
				ClearOverrides ();
			}
			catch (Exception e)
			{
				Monitor.Log ($"Failed in {nameof (Postfix)}:\n{e}",
					LogLevel.Error);
			}
		}

		public static bool genderedKey_Prefix (ref string __result,
			string baseKey, Farmer farmer)
		{
			try
			{
				if (!GenderOverrides.TryGetValue (farmer, out bool isMale))
					isMale = farmer.IsMale;
				__result = baseKey + (isMale ? "_Male" : "_Female");
				return false;
			}
			catch (Exception e)
			{
				Monitor.Log ($"Failed in {nameof (genderedKey_Prefix)}:\n{e}",
					LogLevel.Error);
				return true;
			}
		}

#pragma warning restore IDE1006

		private static void OverrideFarmers (Farmer farmer1, Farmer farmer2)
		{
			if (GenderOverrides.ContainsKey (farmer1) ||
					GenderOverrides.ContainsKey (farmer2))
				return;

			if (Config.VerboseLogging)
				Monitor.Log ($"Overriding farmer genders for pregnancy event", LogLevel.Debug);

			Role role1 = Model.GetPregnancyRole (farmer1);
			Role role2 = Model.GetPregnancyRole (farmer2);

			if (role1 == Role.Adopt || role2 == Role.Adopt)
			{
				OverrideFarmer (farmer1, true);
				OverrideFarmer (farmer2, true);
			}
			else
			{
				OverrideFarmer (farmer1, role1 == Role.Make);
				OverrideFarmer (farmer2, role2 == Role.Make);
			}
		}

		private static void OverrideFarmer (Farmer farmer, bool isMale)
		{
			GenderOverrides[farmer] = farmer.IsMale;
			farmer.IsMale = isMale;
		}

		private static void ClearOverrides ()
		{
			if (Config.VerboseLogging)
				Monitor.Log ($"Resetting farmer genders", LogLevel.Debug);

			foreach (Farmer farmer in GenderOverrides.Keys)
				farmer.IsMale = GenderOverrides[farmer];
			GenderOverrides.Clear ();
		}
	}
}
