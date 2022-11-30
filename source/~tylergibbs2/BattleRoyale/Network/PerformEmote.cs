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
    class PerformEmote : NetworkMessage
    {
        public PerformEmote()
        {
            MessageType = NetworkUtils.MessageTypes.PERFORM_EMOTE;
        }

        public override void Receive(Farmer source, List<object> data)
        {
            long whoId = Convert.ToInt64(data[0]);
            string emote = Convert.ToString(data[1]);

            Farmer who = Game1.getFarmer(whoId);
            who.performPlayerEmote(emote);
        }
    }
}
