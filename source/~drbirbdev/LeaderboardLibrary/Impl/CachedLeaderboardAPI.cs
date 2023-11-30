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

class CachedLeaderboardAPI : ILeaderboardAPI
{

    private Dictionary<string, List<LeaderboardStat>> LocalLeaderboards => ModEntry.LocalModData.LocalLeaderboards[this.ModId];
    private Dictionary<string, List<LeaderboardStat>> TopLeaderboards => ModEntry.LocalModData.TopLeaderboards[this.ModId];
    private readonly LeaderboardDAO LeaderboardDAO = new LeaderboardDAO();
    private readonly string ModId;

    public CachedLeaderboardAPI(string modId)
    {
        this.ModId = modId;
    }

    private void LazyInitStat(string stat)
    {
        if (!this.LocalLeaderboards.ContainsKey(stat))
        {
            this.LocalLeaderboards[stat] = new List<LeaderboardStat>();
        }
        if (!this.TopLeaderboards.ContainsKey(stat))
        {
            this.TopLeaderboards[stat] = new List<LeaderboardStat>();
        }
    }

    public int GetLocalRank(string stat)
    {
        this.LazyInitStat(stat);
        return this.LocalLeaderboards[stat].FindIndex((match) => match.UserUUID == ModEntry.GlobalModData.Value.UserUUID) + 1;
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
        return this.GetPlayerStat(stat, ModEntry.GlobalModData.Value.UserUUID).ToApiShape();
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

    public virtual bool RefreshCache(string stat)
    {
        this.LazyInitStat(stat);
        LeaderboardDAO.GetLocalScores(stat).ContinueWith((task) =>
        {
            if (CheckFailures(task, "GetLocalScores"))
            {
                return;
            }
            this.LocalLeaderboards[stat] = task.Result;
            this.LocalLeaderboards[stat].Sort();
            try
            {
                ModEntry.Instance.Helper.Data.WriteJsonFile($"data/cached_leaderboards.json", ModEntry.LocalModData);
            }
            catch (Exception)
            {
                // swallow exception.
            }
        });

        LeaderboardDAO.GetTopScores(stat).ContinueWith((task) =>
        {
            if (CheckFailures(task, "GetTopScores"))
            {
                return;
            }
            this.TopLeaderboards[stat] = task.Result;
            try
            {
                ModEntry.Instance.Helper.Data.WriteJsonFile($"data/cached_leaderboards.json", ModEntry.LocalModData);
            }
            catch (Exception)
            {
                // swallow exception
            }
        });


        return true;
    }

    public virtual bool UploadScore(string stat, int score)
    {
        this.LazyInitStat(stat);
        LeaderboardStat current = this.GetPlayerStat(stat, ModEntry.GlobalModData.Value.UserUUID);
        if (current is null || current.Score < score)
        {
            LeaderboardDAO.UploadScore(stat, score, ModEntry.GlobalModData.Value.UserUUID, Game1.player.Name, Game1.player.farmName.Value, ModEntry.GlobalModData.Value.Secret, this);
            return this.UpdateCache(stat, score, ModEntry.GlobalModData.Value.UserUUID, Game1.player.Name);
        }
        return true;
    }

    public bool UpdateCache(string stat, int score, string userUuid, string userName)
    {
        this.LazyInitStat(stat);
        LeaderboardStat current = this.GetPlayerStat(stat, userUuid);
        if (current is null || current.Score < score)
        {
            if (current is null)
            {
                current = new LeaderboardStat()
                {
                    Stat = stat,
                    UserUUID = userUuid,
                };
                this.LocalLeaderboards[stat].Add(current);
            }
            current.Name = userName;
            current.Farm = Game1.player.farmName.Value;
            current.Score = score;
            current.DateTime = DateTimeOffset.Now.ToUnixTimeSeconds();

            this.LocalLeaderboards[stat].Sort();

            if (this.TopLeaderboards[stat].Count < 10 || this.TopLeaderboards[stat].Last<LeaderboardStat>().Score < score)
            {
                LeaderboardStat existing = this.TopLeaderboards[stat].Find((match) => match.UserUUID == userUuid);
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
                ModEntry.Instance.Helper.Data.WriteJsonFile($"data/cached_leaderboards.json", ModEntry.LocalModData);
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

        }
        return true;
    }

    protected LeaderboardStat GetPlayerStat(string stat, string userUuid)
    {
        this.LazyInitStat(stat);
        foreach (LeaderboardStat leaderboard in this.LocalLeaderboards[stat])
        {
            if (leaderboard.UserUUID == userUuid)
            {
                return leaderboard;
            }
        }
        return null;
    }

    private static bool CheckFailures(Task task, string queryName)
    {
        if (task.IsFaulted)
        {
            Log.Warn(queryName + " failed");
            Log.Warn(task.Exception.Message);
            return true;
        }
        if (task.IsCanceled)
        {
            Log.Warn(queryName + " was canceled");
            return true;
        }
        if (!task.IsCompleted)
        {
            Log.Warn(queryName + " failed to complete");
            return true;
        }
        return false;
    }
}
