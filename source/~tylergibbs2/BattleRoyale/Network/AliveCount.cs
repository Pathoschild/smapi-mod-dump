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
    class AliveCount : NetworkMessage
    {
        public AliveCount()
        {
            MessageType = NetworkUtils.MessageTypes.BROADCAST_ALIVE_COUNT;
        }

        public override void Receive(Farmer source, List<object> data)
        {
            Round round = ModEntry.BRGame.GetActiveRound();
            int howManyPlayersAlive = Convert.ToInt32(data[0]);

            if (round?.overlayUI != null)
                round.overlayUI.AlivePlayers = howManyPlayersAlive;
        }
    }
}
