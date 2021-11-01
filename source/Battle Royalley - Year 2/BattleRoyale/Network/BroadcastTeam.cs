/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/BattleRoyalley-Year2
**
*************************************************/

using BattleRoyale.Utils;
using StardewValley;
using System;
using System.Collections.Generic;

namespace BattleRoyale.Network
{
    class BroadcastTeam : NetworkMessage
    {
        public BroadcastTeam()
        {
            MessageType = NetworkUtils.MessageTypes.BROADCAST_TEAM;
        }

        public override void Receive(Farmer source, List<object> data)
        {
            string team = Convert.ToString(data[0]);
            DelayedAction.functionAfterDelay(() =>
            {
                FarmerUtils.SetTeam(team);
            }, 750);
        }
    }
}
