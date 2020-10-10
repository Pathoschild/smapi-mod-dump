/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/hootless/StardewMods
**
*************************************************/

namespace BusLocations.Framework
{
    internal class BusLoc
    {
        public string MapName { get; set; }
        public string DisplayName { get; set; }
        public int DestinationX { get; set; }
        public int DestinationY { get; set; }
        public int ArrivalFacing { get; set; }
        public int TicketPrice { get; set; }
    }
}
