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

namespace LeaderboardLibrary
{
    public class LeaderboardAPI : ChainableLeaderboardAPI
    {
        private string ModId;

        private ILeaderboardAPI DelegateAPI;
        public override ILeaderboardAPI Delegate => DelegateAPI;

        public LeaderboardAPI(string modId)
        {
            ModId = modId;
            DelegateAPI = new ThrottledLeaderboardAPI(modId);
        }


        public override int GetLocalRank(string stat)
        {
            try
            {
                return Delegate.GetLocalRank($"{ModId}:{stat}");
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
                return -1;
            }
        }

        public override List<Dictionary<string, string>> GetLocalTopN(string stat, int count)
        {
            if (count > 10)
            {
                throw new ArgumentException("Cannot get more than 10 top scores from GetLocalTopN API (Do you really have more than 10 players?)");
            }
            try
            {
                return Delegate.GetLocalTopN($"{ModId}:{stat}", count);
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
                return null;
            }
        }

        public override Dictionary<string, string> GetPersonalBest(string stat)
        {
            try
            {
                return Delegate.GetPersonalBest($"{ModId}:{stat}");
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
                return null;
            }
        }

        public override int GetRank(string stat)
        {
            throw new NotImplementedException("GetRank API is not currently implemented");
        }

        public override List<Dictionary<string, string>> GetTopN(string stat, int count)
        {
            if (count > 10)
            {
                throw new ArgumentException("Cannot get more than 10 top scores from GetTopN API");
            }
            try
            {
                return Delegate.GetTopN($"{ModId}:{stat}", count);
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
                return null;
            }
        }

        public override bool RefreshCache(string stat)
        {
            try
            {
                Delegate.RefreshCache($"{ModId}:{stat}");
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
                return false;
            }
            return true;
        }

        public override bool UploadScore(string stat, int score)
        {
            try
            {
                Delegate.UploadScore($"{ModId}:{stat}", score);
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
                return false;
            }
            return true;
        }
    }
}
