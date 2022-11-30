/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using BattleRoyale.Utils;
using StardewValley;
using System;
using System.Collections.Generic;

namespace BattleRoyale.Network
{
    class BroadcastRoundEnd : NetworkMessage
    {
        public BroadcastRoundEnd()
        {
            MessageType = NetworkUtils.MessageTypes.SERVER_BROADCAST_ROUND_END;
        }

        public override void Receive(Farmer source, List<object> data)
        {
            long? winnerId = null;
            if (data.Count == 1)
                winnerId = Convert.ToInt64(data[0]);

            Round round = ModEntry.BRGame.GetActiveRound();
            round.EndRound(winnerId);
        }
    }
}
