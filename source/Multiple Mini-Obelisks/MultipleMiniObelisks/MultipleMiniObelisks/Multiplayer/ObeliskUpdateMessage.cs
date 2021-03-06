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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipleMiniObelisks.Multiplayer
{
    internal class ObeliskUpdateMessage
    {
        public MiniObelisk Obelisk { get; set; }

        public ObeliskUpdateMessage()
        {

        }

        public ObeliskUpdateMessage(MiniObelisk obelisk)
        {
            this.Obelisk = obelisk;
        }
    }
}
