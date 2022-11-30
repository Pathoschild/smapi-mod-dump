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
    class KickPlayer : NetworkMessage
    {
        public KickPlayer()
        {
            MessageType = NetworkUtils.MessageTypes.KICK_PLAYER;
        }

        public override void Receive(Farmer source, List<object> data)
        {
            if (!source.IsMainPlayer) // Don't let non-hosts kick players.
                return;

            string reason = Convert.ToString(data[1]);
            reason = string.IsNullOrEmpty(reason) ? "No reason given." : reason;

            Game1.client.disconnect();
            DelayedAction.functionAfterDelay(() =>
            {
                Game1.activeClickableMenu = new StardewValley.Menus.DialogueBox($"Kicked. Reason: \"{reason}\"");
            }, 300);
        }
    }
}
