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
using BirbCore.Attributes;
using StardewModdingAPI;

namespace LeaderboardLibrary;

[SCommand("leaderboard")]
class Command
{
    private static readonly Dictionary<string, ILeaderboardApi> APIS = new();

    private static void LazyInitApi(string modId)
    {
        if (!APIS.ContainsKey(modId))
        {
            APIS[modId] = new LeaderboardApi(modId);
        }
    }

    private static void PrintRecords(params Dictionary<string, string>[] records)
    {
        foreach (Dictionary<string, string> record in records)
        {
            Log.Info($"{record["UserUUID"]} {record["DateTime"]} {record["Name"]} {record["Farm"]} {record["Score"]}");
        }
    }

    [SCommand.Command("Get local rank for the player")]
    public static void GetLocalRank(string modId, string stat)
    {
        LazyInitApi(modId);
        int localRank = APIS[modId].GetLocalRank(stat);
        Log.Info($"Local Rank is {localRank}");
    }

    [SCommand.Command("Get local top score records")]
    public static void GetLocalTopN(string modId, string stat, int count = 10)
    {
        LazyInitApi(modId);
        List<Dictionary<string, string>> localTop = APIS[modId].GetLocalTopN(stat, count);
        PrintRecords(localTop.ToArray());
    }

    [SCommand.Command("Get personal best record")]
    public static void GetPersonalBest(string modId, string stat)
    {
        LazyInitApi(modId);
        Dictionary<string, string> personalBest = APIS[modId].GetPersonalBest(stat);
        PrintRecords(personalBest);
    }

    [SCommand.Command("Get global rank for the player")]
    public static void GetRank(string modId, string stat)
    {
        LazyInitApi(modId);
        int globalRank = APIS[modId].GetRank(stat);
        Log.Info($"Global Rank is {globalRank}");
    }

    [SCommand.Command("Get global top score records")]
    public static void GetTopN(string modId, string stat, int count = 10)
    {
        LazyInitApi(modId);
        List<Dictionary<string, string>> globalTop = APIS[modId].GetTopN(stat, count);
        PrintRecords(globalTop.ToArray());
    }

    [SCommand.Command("Refresh leaderboard cache")]
    public static void RefreshCache(string modId, string stat)
    {
        LazyInitApi(modId);
        APIS[modId].RefreshCache(stat);
    }

    [SCommand.Command("Delete leaderboard cache")]
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

    [SCommand.Command("Upload new score to leaderboard")]
    // ReSharper disable thrice UnusedParameter.Global
    public static void UploadScore(string modId, string stat, int score)
    {
#if DEBUG
        LazyInitApi(modId);
        APIS[modId].UploadScore(stat, score);
#else
        Log.Info("Nice try");
#endif
    }

    [SCommand.Command("Print leaderboard tracking data for the current user")]
    public static void PrintUserInfo()
    {
        Log.Info($"User UUID = {ModEntry.GLOBAL_MOD_DATA.GetValueForScreen(0).UserUuid} and Secret starts with {ModEntry.GLOBAL_MOD_DATA.GetValueForScreen(0).Secret[..3]}");
    }

    [SCommand.Command("Dumps the contents of the local cache")]
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
                foreach (LeaderboardStat record in ModEntry.LocalModData.LocalLeaderboards[modId][stat])
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
        foreach (string playerUuid in ModEntry.LocalModData.MultiplayerUuiDs)
        {
            Log.Info($"    {playerUuid}");
        }
    }
}
