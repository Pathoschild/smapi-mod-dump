/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace StardustCore.Events.Preconditions
{
    public class LocationPrecondition:EventPrecondition
    {
        private string locationName;
        private GameLocation location;


        public LocationPrecondition()
        {

        }

        public LocationPrecondition(GameLocation Location)
        {
            this.locationName = Location.NameOrUniqueName;
            this.location = Location;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Location">The name of the location.</param>
        /// <param name="IsStructure">The location is a building on the farm.</param>
        public LocationPrecondition(string Location, bool IsStructure=false)
        {
            this.locationName = Location;
            this.location = Game1.getLocationFromName(Location,IsStructure);
        }

        public override bool meetsCondition()
        {
            return Game1.player.currentLocation == this.location;
        }

        public override string ToString()
        {
            return "Location " + this.locationName;
        }
    }
}
