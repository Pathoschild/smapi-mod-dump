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
using System.Collections.Generic;

namespace BattleRoyale.Network
{
    class SendDeathAnimation : NetworkMessage
    {
        public SendDeathAnimation()
        {
            MessageType = Utils.NetworkUtils.MessageTypes.SEND_DEATH_ANIMATION;
        }

        public override void Receive(Farmer source, List<object> data)
        {
            Utils.FarmerUtils.PlayDeathAnimation(source);
        }
    }
}
