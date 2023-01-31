/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;
using BirbShared;
using HarmonyLib;
using StardewValley;
using StardewValley.Minigames;

namespace JunimoKartGlobalRankings
{
    [HarmonyPatch(typeof(MineCart), nameof(MineCart.Die))]
    class MineCart_Die
    {
        internal static void Postfix(int ___score)
        {
            try
            {
                if(!ModEntry.LeaderboardAPI.UploadScore("JunimoKartScore", ___score))
                {
                    Log.Error("Failed to upload JunimoKart high score");
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
        }

    }

    [HarmonyPatch(typeof(NetLeaderboards), nameof(NetLeaderboards.AddScore))]
    class NetLeaderboards_AddScore
    {
        internal static bool Prefix(string name, int score)
        {
            try
            {
                if (Game1.player.Name == name)
                {
                    ModEntry.LeaderboardAPI.UploadScore("JunimoKartScore", score);
                }
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(NetLeaderboards), nameof(NetLeaderboards.GetScores))]
    class NetLeaderboards_GetScores
    {
        internal static bool Prefix(ref List<KeyValuePair<string, int>> __result)
        {
            try
            {
                __result = new List<KeyValuePair<string, int>>();
                foreach (Dictionary<string, string> record in ModEntry.LeaderboardAPI.GetTopN("JunimoKartScore", 5))
                {
                    int.TryParse(record["Score"], out int score);
                    __result.Add(new KeyValuePair<string, int>(record["Name"], score));
                }

                __result.Add(new KeyValuePair<string, int>(Game1.getCharacterFromName("Lewis").displayName, 50000));
                __result.Add(new KeyValuePair<string, int>(Game1.getCharacterFromName("Shane").displayName, 25000));
                __result.Add(new KeyValuePair<string, int>(Game1.getCharacterFromName("Sam").displayName, 10000));
                __result.Add(new KeyValuePair<string, int>(Game1.getCharacterFromName("Abigail").displayName, 5000));
                __result.Add(new KeyValuePair<string, int>(Game1.getCharacterFromName("Vincent").displayName, 250));

                __result.Sort(Comparer<KeyValuePair<string, int>>.Create((x, y) => y.Value - x.Value));
                __result.RemoveRange(5, __result.Count - 5);
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(NetLeaderboards), nameof(NetLeaderboards.LoadScores))]
    class NetLeaderboards_LoadScores
    {
        internal static bool Prefix()
        {
            try
            {
                ModEntry.Instance.Helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
            return true;
        }

        private static void GameLoop_SaveLoaded(object sender, StardewModdingAPI.Events.SaveLoadedEventArgs e)
        {
            ModEntry.LeaderboardAPI.RefreshCache("JunimoKartScore");
        }
    }
}
