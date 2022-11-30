/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;

namespace BattleRoyale.Network
{
    class LeaderboardDataSync : NetworkMessage
    {
        public LeaderboardDataSync()
        {
            MessageType = Utils.NetworkUtils.MessageTypes.LEADERBOARD_DATA_SYNC;
        }

        public override void Receive(Farmer source, List<object> data)
        {
            long playerId = Convert.ToInt64(data[0]);
            int kills = Convert.ToInt32(data[1]);
            int deaths = Convert.ToInt32(data[2]);
            int wins = Convert.ToInt32(data[3]);

            UI.LeaderboardPlayer player = ModEntry.Leaderboard.GetPlayer(Game1.getFarmer(playerId));
            player.Kills = kills;
            player.Deaths = deaths;
            player.Wins = wins;

            ModEntry.Leaderboard.Players.Sort();
        }
    }
}
