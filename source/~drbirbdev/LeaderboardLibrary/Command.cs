/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System.Collections.Generic;
using BirbShared;
using BirbShared.Command;
using StardewModdingAPI;

namespace LeaderboardLibrary
{
    [CommandClass(Prefix = "leaderboard_")]
    class Command
    {
        private static Dictionary<string, ILeaderboardAPI> Apis = new Dictionary<string, ILeaderboardAPI>();

        private static void LazyInitAPI(string modId)
        {
            if (!Apis.ContainsKey(modId))
            {
                Apis[modId] = new LeaderboardAPI(modId);
            }
        }

        private static void PrintRecords(params Dictionary<string, string>[] records)
        {
            foreach (Dictionary<string, string> record in records)
            {
                Log.Info($"{record["UserUUID"]} {record["DateTime"]} {record["Name"]} {record["Farm"]} {record["Score"]}");
            }
        }

        [CommandMethod("Get local rank for the player")]
        public static void GetLocalRank(string modId, string stat)
        {
            LazyInitAPI(modId);
            int localRank = Apis[modId].GetLocalRank(stat);
            Log.Info($"Local Rank is {localRank}");
        }

        [CommandMethod("Get local top score records")]
        public static void GetLocalTopN(string modId, string stat, int count = 10)
        {
            LazyInitAPI(modId);
            List<Dictionary<string, string>> localTop = Apis[modId].GetLocalTopN(stat, count);
            PrintRecords(localTop.ToArray());
        }

        [CommandMethod("Get personal best record")]
        public static void GetPersonalBest(string modId, string stat)
        {
            LazyInitAPI(modId);
            Dictionary<string, string> personalBest = Apis[modId].GetPersonalBest(stat);
            PrintRecords(personalBest);
        }

        [CommandMethod("Get global rank for the player")]
        public static void GetRank(string modId, string stat)
        {
            LazyInitAPI(modId);
            int globalRank = Apis[modId].GetRank(stat);
            Log.Info($"Global Rank is {globalRank}");
        }

        [CommandMethod("Get global top score records")]
        public static void GetTopN(string modId, string stat, int count = 10)
        {
            LazyInitAPI(modId);
            List<Dictionary<string, string>> globalTop = Apis[modId].GetTopN(stat, count);
            PrintRecords(globalTop.ToArray());
        }

        [CommandMethod("Refresh leaderboard cache")]
        public static void RefreshCache(string modId, string stat)
        {
            LazyInitAPI(modId);
            Apis[modId].RefreshCache(stat);
        }

        [CommandMethod("Delete leaderboard cache")]
        public static void DeleteCache()
        {
            foreach (string key in ModEntry.LocalModData.LocalLeaderboards.Keys)
            {
                ModEntry.LocalModData.LocalLeaderboards[key].Clear();
            }
            foreach (string key in ModEntry.LocalModData.TopLeaderboards.Keys)
            {
                ModEntry.LocalModData.TopLeaderboards[key].Clear();
            }
        }

        [CommandMethod("Upload new score to leaderboard")]
        public static void UploadScore(string modId, string stat, int score)
        {
#if DEBUG
            LazyInitAPI(modId);
            Apis[modId].UploadScore(stat, score);
#else
            Log.Info("Nice try");
#endif
        }

        [CommandMethod("Print leaderboard tracking data for the current user")]
        public static void PrintUserInfo()
        {
            Log.Info($"User UUID = {ModEntry.GlobalModData.GetValueForScreen(0).UserUUID} and Secret starts with {ModEntry.GlobalModData.GetValueForScreen(0).Secret.Substring(0, 3)}");
        }

        [CommandMethod("Dumps the contents of the local cache")]
        public static void DumpCache()
        {
            if (!Context.IsWorldReady)
            {
                Log.Info("Cannot dump cache until world is loaded");
                return;
            }

            Log.Info("Local Records");

            foreach (string modId in ModEntry.LocalModData.LocalLeaderboards.Keys)
            {
                Log.Info($"ModId = {modId}");
                foreach (string stat in ModEntry.LocalModData.LocalLeaderboards[modId].Keys)
                {
                    Log.Info($"    Stat = {stat}");
                    foreach (LeaderboardStat record in ModEntry.LocalModData.LocalLeaderboards[modId][stat] )
                    {
                        Log.Info($"        User = {record.UserUUID}");
                        Log.Info($"            Name = {record.Name}");
                        Log.Info($"            Farm = {record.Farm}");
                        Log.Info($"            Score = {record.Score}");
                        Log.Info($"            Time = {record.DateTime}");
                    }
                }
            }

            Log.Info("Global Records");
            foreach (string modId in ModEntry.LocalModData.TopLeaderboards.Keys)
            {
                Log.Info($"ModId = {modId}");
                foreach (string stat in ModEntry.LocalModData.TopLeaderboards[modId].Keys)
                {
                    Log.Info($"    Stat = {stat}");
                    foreach (LeaderboardStat record in ModEntry.LocalModData.TopLeaderboards[modId][stat])
                    {
                        Log.Info($"        User = {record.UserUUID}");
                        Log.Info($"            Name = {record.Name}");
                        Log.Info($"            Farm = {record.Farm}");
                        Log.Info($"            Score = {record.Score}");
                        Log.Info($"            Time = {record.DateTime}");
                    }
                }
            }

            Log.Info("Local Users");
            foreach (string playerUuid in ModEntry.LocalModData.MultiplayerUUIDs)
            {
                Log.Info($"    {playerUuid}");
            }
        }
    }
}
