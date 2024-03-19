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
using System.Linq;
using System.Threading.Tasks;
using BirbCore.Attributes;
using StardewValley;

namespace LeaderboardLibrary;

sealed class CachedLeaderboardApi(string modId) : ILeaderboardApi
{
    private Dictionary<string, List<LeaderboardStat>> LocalLeaderboards =>
        ModEntry.LocalModData.LocalLeaderboards[modId];

    private Dictionary<string, List<LeaderboardStat>> TopLeaderboards => ModEntry.LocalModData.TopLeaderboards[modId];
    private readonly LeaderboardDao _leaderboardDao = new();

    private void LazyInitStat(string stat)
    {
        if (!this.LocalLeaderboards.ContainsKey(stat))
        {
            this.LocalLeaderboards[stat] = [];
        }

        if (!this.TopLeaderboards.ContainsKey(stat))
        {
            this.TopLeaderboards[stat] = [];
        }
    }

    public int GetLocalRank(string stat)
    {
        this.LazyInitStat(stat);
        return this.LocalLeaderboards[stat]
            .FindIndex(match => match.UserUUID == ModEntry.GLOBAL_MOD_DATA.Value.UserUuid) + 1;
    }

    public List<Dictionary<string, string>> GetLocalTopN(string stat, int count)
    {
        this.LazyInitStat(stat);
        if (count > this.LocalLeaderboards[stat].Count)
        {
            count = this.LocalLeaderboards[stat].Count;
        }

        return LeaderboardStat.ToApiList(this.LocalLeaderboards[stat].GetRange(0, count));
    }

    public Dictionary<string, string> GetPersonalBest(string stat)
    {
        this.LazyInitStat(stat);
        return this.GetPlayerStat(stat, ModEntry.GLOBAL_MOD_DATA.Value.UserUuid).ToApiShape();
    }

    public int GetRank(string stat)
    {
        throw new NotImplementedException();
    }

    public List<Dictionary<string, string>> GetTopN(string stat, int count)
    {
        this.LazyInitStat(stat);
        if (count > this.TopLeaderboards[stat].Count)
        {
            count = this.TopLeaderboards[stat].Count;
        }

        return LeaderboardStat.ToApiList(this.TopLeaderboards[stat].GetRange(0, count));
    }

    public bool RefreshCache(string stat)
    {
        this.LazyInitStat(stat);
        LeaderboardDao.GetLocalScores(stat).ContinueWith(task =>
        {
            if (CheckFailures(task, "GetLocalScores"))
            {
                return;
            }

            this.LocalLeaderboards[stat] = task.Result;
            this.LocalLeaderboards[stat].Sort();
            try
            {
                ModEntry.Instance.Helper.Data.WriteJsonFile("data/cached_leaderboards.json", ModEntry.LocalModData);
            }
            catch (Exception)
            {
                // swallow exception.
            }
        });

        LeaderboardDao.GetTopScores(stat).ContinueWith(task =>
        {
            if (CheckFailures(task, "GetTopScores"))
            {
                return;
            }

            this.TopLeaderboards[stat] = task.Result;
            try
            {
                ModEntry.Instance.Helper.Data.WriteJsonFile("data/cached_leaderboards.json", ModEntry.LocalModData);
            }
            catch (Exception)
            {
                // swallow exception
            }
        });


        return true;
    }

    public bool UploadScore(string stat, int score)
    {
        this.LazyInitStat(stat);
        LeaderboardStat current = this.GetPlayerStat(stat, ModEntry.GLOBAL_MOD_DATA.Value.UserUuid);
        if (current is not null && !(current.Score < score))
        {
            return true;
        }

        LeaderboardDao.UploadScore(stat, score, ModEntry.GLOBAL_MOD_DATA.Value.UserUuid, Game1.player.Name,
            Game1.player.farmName.Value, ModEntry.GLOBAL_MOD_DATA.Value.Secret, this);
        return this.UpdateCache(stat, score, ModEntry.GLOBAL_MOD_DATA.Value.UserUuid, Game1.player.Name);
    }

    public bool UpdateCache(string stat, int score, string userUuid, string userName)
    {
        this.LazyInitStat(stat);
        LeaderboardStat current = this.GetPlayerStat(stat, userUuid);
        if (current is not null && !(current.Score < score))
        {
            return true;
        }

        if (current is null)
        {
            current = new LeaderboardStat
            {
                Stat = stat,
                UserUUID = userUuid
            };
            this.LocalLeaderboards[stat].Add(current);
        }

        current.Name = userName;
        current.Farm = Game1.player.farmName.Value;
        current.Score = score;
        current.DateTime = DateTimeOffset.Now.ToUnixTimeSeconds();

        this.LocalLeaderboards[stat].Sort();

        if (this.TopLeaderboards[stat].Count < 10 || this.TopLeaderboards[stat].Last().Score < score)
        {
            LeaderboardStat existing = this.TopLeaderboards[stat].Find(match => match.UserUUID == userUuid);
            if (existing is not null)
            {
                this.TopLeaderboards[stat].Remove(existing);
            }

            this.TopLeaderboards[stat].Add(current);
            this.TopLeaderboards[stat].Sort();
            if (this.TopLeaderboards[stat].Count > 10)
            {
                this.TopLeaderboards[stat].RemoveAt(10);
            }
        }

        try
        {
            ModEntry.Instance.Helper.Data.WriteJsonFile("data/cached_leaderboards.json", ModEntry.LocalModData);
        }
        catch (Exception e)
        {
            Log.Error($"Failed to update the cache.  Could multiple mods be updating high scores at once?.\n" +
                      $"Score: {score}\n" +
                      $"Stat : {stat}\n" +
                      $"Name : {userName}\n" +
                      $"Farm : {Game1.player.farmName}\n" +
                      $"UUID : {userUuid}\n");
            Log.Error(e.Message);
        }

        return true;
    }

    private LeaderboardStat GetPlayerStat(string stat, string userUuid)
    {
        this.LazyInitStat(stat);
        return this.LocalLeaderboards[stat].FirstOrDefault(leaderboard => leaderboard.UserUUID == userUuid);
    }

    private static bool CheckFailures(Task task, string queryName)
    {
        if (task.IsFaulted)
        {
            Log.Warn(queryName + " failed");
            Log.Warn(task.Exception?.Message);
            return true;
        }

        if (task.IsCanceled)
        {
            Log.Warn(queryName + " was canceled");
            return true;
        }

        if (task.IsCompleted)
        {
            return false;
        }

        Log.Warn(queryName + " failed to complete");
        return true;

    }
}
