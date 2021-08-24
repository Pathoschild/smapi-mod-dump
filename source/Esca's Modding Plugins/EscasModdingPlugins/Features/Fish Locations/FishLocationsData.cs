/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/EscasModdingPlugins
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System;

namespace EscasModdingPlugins
{
    /// <summary>The format used by each data value in the FishLocations asset.</summary>
    public class FishLocationsData : TileData
    {
        /// <summary>The replacement return value of <see cref="GameLocation.getFishingLocation"/> (if not null). Decides which group(s) of fish will be used from the Data/Locations asset.</summary>
        public int? UseZone { get; set; } = null;
        /// <summary>The replacement locationName value in <see cref="GameLocation.getFish"/> (if not null). Decides which location key will be used from the Data/Locations asset.</summary>
        public string UseLocation { get; set; } = null;
        /// <summary>The replacement return value of <see cref="GameLocation.catchOceanCrabPotFishFromThisSpot"/> (if not null). Decides whether crab pots will use ocean or freshwater data from the Data/Fish asset.</summary>
        public bool? UseOceanCrabPots { get; set; } = null;

        public override bool TryParse(string raw)
        {
            /*
            Valid string formats for this class:
                "UseZone"
                "UseZone UseLocation"
                "UseZone UseLocation UseOceanCrabPots"
            */

            string[] splitValue = raw.Split(new char[0], StringSplitOptions.RemoveEmptyEntries); //split the property around whitespace and remove empty entries

            if (splitValue.Length > 0) //if 1 entry exists
            {
                int useZone;
                string useLocation = null;
                bool? useOceanCrabPots = null;

                if (!int.TryParse(splitValue[0], out useZone)) //if the first entry is NOT a valid integer
                {
                    Monitor?.LogOnce($"Invalid tile property: \"{raw}\". Reason: \"{splitValue[0]}\" is not a valid integer.", LogLevel.Debug);
                    return false; //use default data
                }

                if (splitValue.Length > 1) //if 2 entries exist
                    useLocation = splitValue[1]; //use the second entry as a string

                if (splitValue.Length > 2) //if 3 entries exist
                {
                    if (!bool.TryParse(splitValue[2], out bool useOcean)) //if the third entry is NOT a valid boolean
                    {
                        Monitor?.LogOnce($"Invalid tile property: \"{raw}\". Reason: \"{splitValue[2]}\" is not a valid boolean (true or false).", LogLevel.Debug);
                        return false;
                    }
                    else
                        useOceanCrabPots = useOcean;
                }

                //parsing succeeded; apply all parsed data to this instance
                UseZone = useZone;
                UseLocation = useLocation;
                UseOceanCrabPots = useOceanCrabPots;
                return true;
            }
            else
            {
                Monitor?.LogOnce($"Invalid tile property: \"{raw}\". Reason: Property value is empty.", LogLevel.Debug);
                return false;
            }
        }
    }
}
