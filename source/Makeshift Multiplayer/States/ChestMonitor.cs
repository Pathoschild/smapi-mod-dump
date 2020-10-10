/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMP
**
*************************************************/

using StardewValley.Objects;
using StardewValleyMP.Packets;

namespace StardewValleyMP.States
{
    class ChestMonitor : SpecificMonitor<StardewValley.Object, Chest, ChestState, ChestUpdatePacket>
    {
        public ChestMonitor( LocationCache loc )
        :   base( loc, loc.loc.objects, 
                  (obj) => new ChestState(obj),
                  (loc_, pos) => new ChestUpdatePacket(loc_, pos) )
        {
        }
    }
}
