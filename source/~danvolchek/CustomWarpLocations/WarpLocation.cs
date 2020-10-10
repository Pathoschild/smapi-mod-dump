/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

namespace CustomWarpLocations
{
    internal class WarpLocation
    {
        public string locationName;
        public int xCoord;
        public int yCoord;

        public WarpLocation(string locationName, int x, int y)
        {
            this.locationName = locationName;
            this.xCoord = x;
            this.yCoord = y;
        }
    }
}
