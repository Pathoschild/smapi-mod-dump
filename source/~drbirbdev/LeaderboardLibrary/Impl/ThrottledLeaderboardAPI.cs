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
using BirbShared;

namespace LeaderboardLibrary
{
    class ThrottledLeaderboardAPI : ChainableLeaderboardAPI
    {
        public DateTime RefreshNextCall = DateTime.MinValue;
        public DateTime UploadNextCall = DateTime.MinValue;

        private ILeaderboardAPI DelegateApi;
        public override ILeaderboardAPI Delegate => DelegateApi;

        public ThrottledLeaderboardAPI(string modId)
        {
            this.DelegateApi = new MultiplayerLeaderboardAPI(modId);
        }

        public override bool RefreshCache(string stat)
        {
            if (DateTime.UtcNow > RefreshNextCall)
            {
                Delegate.RefreshCache(stat);
                RefreshNextCall = DateTime.UtcNow.AddSeconds(5);
            }
            else
            {
                Log.Warn($"{stat} was throttled when calling RefreshCache");
                return false;
            }
            return true;
        }

        public override bool UploadScore(string stat, int score)
        {
            int oldScore = 0;
            Dictionary<string, string> oldRecord = Delegate.GetLocalTopN(stat, 10).Find((match) => match["UserUUID"] == (ModEntry.GlobalModData.Value.UserUUID));
            if (oldRecord is not null)
            {
                int.TryParse(oldRecord["Score"], out oldScore);
            }

            if (score > oldScore)
            {
                if (DateTime.UtcNow > UploadNextCall)
                {
                    Delegate.UploadScore(stat, score);
                    UploadNextCall = DateTime.UtcNow.AddSeconds(5);
                }
                else
                {
                    Log.Warn($"{stat} was throttled when calling UploadScore");
                    return false;
                }
            }
            return true;
        }
    }
}
