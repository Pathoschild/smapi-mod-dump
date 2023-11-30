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

namespace LeaderboardLibrary;

public interface ILeaderboardAPI
{
    /// <summary>
    /// Upload a new score for a given stat.  Will only upload to the internet if the new score beats the old personal best.
    /// This is an expensive operation, so only do this infrequently if you expect personal bests to be beaten often.  Abusing this API call can lead to your mod getting throttled or denied.
    /// </summary>
    /// <param name="stat">The name of the stat</param>
    /// <param name="score">The new score of the stat</param>
    /// <returns>True if the upload succeeded (or was unneccessary).  Doesn't account for async failures.</returns>
    bool UploadScore(string stat, int score);

    /// <summary>
    /// Get the global top scorers for a given stat.  Retrieves this data from a cache.  Call RefreshCache to populate the cache before calling this API.
    /// </summary>
    /// <param name="stat">The name of the stat</param>
    /// <param name="count">The number of top scores to get.  Max 10.</param>
    /// <returns>A list of top score records.  Records are a dictionary with "Stat", "Name", "Farm", "Score", and "DateTime" values.</returns>
    List<Dictionary<string, string>> GetTopN(string stat, int count);

    /// <summary>
    /// Get the local top scorers for a given stat.  Retrieves this data from a cache.
    /// </summary>
    /// <param name="stat">The name of the stat</param>
    /// <param name="count">The number of top scores to get.</param>
    /// <returns>A list of local top score records.  Records are a dictionary with "Stat", "Name", "Farm", "Score", and "DateTime" values.</returns>
    List<Dictionary<string, string>> GetLocalTopN(string stat, int count);

    /// <summary>
    /// UNIMPLEMENTED
    /// Get the personal rank of the player for a given stat.  Retrieves this data from a cache.  Call RefreshCache to populate the cache before calling this API.
    /// </summary>
    /// <param name="stat">The name of the stat</param>
    /// <returns>A rank for the players personal best score.</returns>
    int GetRank(string stat);

    /// <summary>
    /// Get the personal rank for this farm of the player for a given stat.  Retrieves this data from a cache.  Call RefreshCache to populate the cache before calling this API.
    /// </summary>
    /// <param name="stat">The name of the stat</param>
    /// <returns>A local rank for the players personal best score.</returns>
    int GetLocalRank(string stat);

    /// <summary>
    /// Get the personal best score of the player.  Retrieves this data from a cache.  Call RefreshCache to populate the cache before calling this API.
    /// </summary>
    /// <param name="stat">The name of the stat</param>
    /// <returns>The players personal best score record.  Records are a dictionary with "Stat", "Name", "Farm", "Score", and "DateTime" values.</returns>
    Dictionary<string, string> GetPersonalBest(string stat);

    /// <summary>
    /// Refresh the cached scores.
    /// This is an expensive operation, so only do this infrequently.  Abusing this API call can lead to your mod getting throttled or denied.
    /// </summary>
    /// <param name="stat"></param>
    /// <returns>True if the refresh succeeded (or was unneccessary).  Doesn't account for async failures.</returns>
    bool RefreshCache(string stat);
}
