/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Bouhm/stardew-valley-mods
**
*************************************************/

namespace Bouhm.Shared.Locations
{
    // Used for syncing only the necessary data
    internal class LocationData
    {
        /*********
        ** Accessors
        *********/
        public string LocationName { get; set; }
        public float X { get; set; }
        public float Y { get; set; }


        /*********
        ** Public methods
        *********/
        public LocationData(string locationName, float x, float y)
        {
            this.LocationName = locationName;
            this.X = x;
            this.Y = y;
        }
    }
}
