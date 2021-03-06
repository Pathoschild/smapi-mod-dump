/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/MultipleMiniObelisks
**
*************************************************/

using MultipleMiniObelisks.Objects;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipleMiniObelisks.Multiplayer
{
    internal class ObeliskTeleportRequestMessage
    {
        public MiniObelisk Obelisk { get; set; }
        public long FarmerId { get; set; }

        public ObeliskTeleportRequestMessage()
        {

        }

        public ObeliskTeleportRequestMessage(MiniObelisk obelisk, long farmerId)
        {
            this.Obelisk = obelisk;
            this.FarmerId = farmerId;
        }
    }
}
