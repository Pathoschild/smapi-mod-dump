/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/mouahrarasModuleCollection
**
*************************************************/

using System.Linq;
using System.Collections.Generic;
using HarmonyLib;
using StardewValley;
using StardewValley.Minigames;

namespace mouahrarasModuleCollection.TweaksAndFeatures.ArcadeGames.NonRealisticLeaderboard.Patches
{
	internal class NetLeaderboardsPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(NetLeaderboards), nameof(NetLeaderboards.GetScores)),
				prefix: new HarmonyMethod(typeof(NetLeaderboardsPatch), nameof(GetScoresPrefix))
			);
		}

		private static bool IsCalledFromMineCart()
		{
			IEnumerable<System.Type> callingMethods = new System.Diagnostics.StackTrace().GetFrames()
				.Select(frame => frame.GetMethod())
				.Where(method => method != null)
				.Select(method => method.DeclaringType);

			return callingMethods.Any(type => type == typeof(MineCart));
		}

		private static bool GetScoresPrefix(NetLeaderboards __instance, ref List<KeyValuePair<string, int>> __result)
		{
			if (!ModEntry.Config.ArcadeGamesPayToPlayNonRealisticLeaderboard || !IsCalledFromMineCart() || Game1.player.team.junimoKartScores.entries.Count == 0)
				return true;

			__result = new()
			{
				new KeyValuePair<string, int>(Game1.getCharacterFromName("Lewis").displayName, 50000),
				new KeyValuePair<string, int>(Game1.getCharacterFromName("Shane").displayName, 25000),
				new KeyValuePair<string, int>(Game1.getCharacterFromName("Sam").displayName, 10000),
				new KeyValuePair<string, int>(Game1.getCharacterFromName("Abigail").displayName, 5000),
				new KeyValuePair<string, int>(Game1.getCharacterFromName("Vincent").displayName, 250)
			};

			foreach (NetLeaderboardsEntry entry in __instance.entries)
			{
				__result.Add(new KeyValuePair<string, int>(entry.name.Value, entry.score.Value));
			}

			__result.Sort((KeyValuePair<string, int> a, KeyValuePair<string, int> b) => a.Value.CompareTo(b.Value));
			__result.Reverse();

			for (int i = 0; i < __result.Count; i++)
			{
				bool isDuplicate = false;

				for (int j = 0; j < i; j++)
				{
					if (__result[i].Key == __result[j].Key)
					{
						isDuplicate = true;
						break;
					}
				}

				if (isDuplicate)
				{
					__result.RemoveAt(i);
					i--;
				}
			}
			return false;
		}
	}
}
