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
    class SynchronizeTime : NetworkMessage
    {
        public SynchronizeTime()
        {
            MessageType = NetworkUtils.MessageTypes.SYNCHRONIZE_TIME;
        }

        public override void Receive(Farmer source, List<object> data)
        {
            string season = Convert.ToString(data[0]);
            int time = Convert.ToInt32(data[1]);

            TimeUtils.SetTime(season, time);
        }
    }
}
