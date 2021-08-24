/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Linq;
using System.Reflection;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class MonsterFindPlayerPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal MonsterFindPlayerPatch()
		{
			Original = typeof(Monster).MethodNamed(name: "findPlayer");
			Prefix = new HarmonyMethod(GetType(), nameof(MonsterFindPlayerPrefix));
		}

		#region harmony patches

		/// <summary>Patch to override monster aggro.</summary>
		[HarmonyPrefix]
		private static bool MonsterFindPlayerPrefix(Monster __instance, ref Farmer __result)
		{
			try
			{
				__result = Game1.player;
				if (__instance.currentLocation == null)
					return false; // don't run original logic

				if (__instance is GreenSlime && __instance.currentLocation.DoesAnyPlayerHereHaveProfession("Piper", out var pipers))
				{
					var distanceToClosestPiper = double.MaxValue;
					foreach (var piper in pipers)
					{
						var distanceToThisPiper = __instance.DistanceToCharacter(piper);
						if (distanceToThisPiper >= distanceToClosestPiper) continue;

						__result = piper;
						distanceToClosestPiper = distanceToThisPiper;
					}

					return false; // don't run original logic
				}

				//if (__instance.currentLocation.DoesAnyPlayerHereHaveProfession("Brute", out var brutes))
				//{
				//	var distanceToClosestBrute = double.MaxValue;
				//	foreach (var brute in brutes)
				//	{
				//		var distanceToThisBrute = __instance.DistanceToCharacter(brute);
				//		if (distanceToThisBrute >= distanceToClosestBrute) continue;

				//		__result = brute;
				//		distanceToClosestBrute = distanceToThisBrute;

				//	}

				//	return false; // don't run original logic
				//}

				var distanceToClosestPlayer = double.MaxValue;
				foreach (var farmer in __instance.currentLocation.farmers)
				{
					if (ModEntry.ActivePeerSuperModes.TryGetValue(Util.Professions.IndexOf("Hunter"),
						out var peerIDs) && peerIDs.Any(id => id == farmer.UniqueMultiplayerID)) continue;

					var distanceToThisPlayer = __instance.DistanceToCharacter(farmer);
					if (distanceToThisPlayer >= distanceToClosestPlayer) continue;

					__result = farmer;
					distanceToClosestPlayer = distanceToThisPlayer;
				}

				//if (__result.IsLocalPlayer && ModEntry.IsSuperModeActive &&
				//    ModEntry.SuperModeIndex == Util.Professions.IndexOf("Hunter"))
				//	__result = null;

				return false; // run original logic
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod().Name}:\n{ex}", LogLevel.Error);
				return true; // default to original logic
			}
		}

		#endregion harmony patches
	}
}