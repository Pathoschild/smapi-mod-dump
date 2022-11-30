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
using System.Collections.Generic;

namespace BattleRoyale.Network
{
    class ReturnToLobby : NetworkMessage
    {
        public ReturnToLobby()
        {
            MessageType = NetworkUtils.MessageTypes.RETURN_TO_LOBBY;
        }

        public override void Receive(Farmer source, List<object> data)
        {
            ModEntry.BRGame.ReturnToLobby();
        }
    }
}
