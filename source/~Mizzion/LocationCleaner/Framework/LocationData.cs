/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace LocationCleaner.Framework
{
    internal class LocationData
    {
        public GameLocation location { get; set; }
        public IList<SObject> locObj { get; set; }
        public IList<TerrainFeature> locTerrain { get; set; }
        public IList<ResourceClump> locResource { get; set; }

        public LocationData(string locationName)
        {
            location = Game1.getLocationFromName(locationName);
            locObj = location.objects.Values.ToList();
            locTerrain = location.terrainFeatures.Values.ToList();

            IList<ResourceClump> resourceClumps =
                (location as Farm)?.resourceClumps
                ?? (IList<ResourceClump>)(location as Woods)?.stumps
                ?? new List<ResourceClump>();
            locResource = resourceClumps;
        }
    }
}
