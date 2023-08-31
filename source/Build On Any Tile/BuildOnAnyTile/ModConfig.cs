/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/BuildOnAnyTile
**
*************************************************/

namespace BuildOnAnyTile
{
    /// <summary>A collection of this mod's config.json file settings.</summary>
    public class ModConfig
    {
        public bool BuildOnAllTerrainFeatures { get; set; } = false;
        public bool BuildOnOtherBuildings { get; set; } = true;
        public bool BuildOnWater { get; set; } = true;
        public bool BuildOnImpassableTiles { get; set; } = true;
        public bool BuildOnNoFurnitureTiles { get; set; } = true;
        public bool BuildOnCaveAndShippingZones { get; set; } = true;

        /// <summary>Indicates whether all config.json features are currently enabled.</summary>
        /// <returns>True if every option is enabled; false otherwise.</returns>
        public bool EverythingEnabled()
        {
            return BuildOnAllTerrainFeatures && BuildOnOtherBuildings && BuildOnWater && BuildOnImpassableTiles && BuildOnNoFurnitureTiles && BuildOnCaveAndShippingZones; //return true if all of these are true
        }

        /// <summary>Indicates whether all config.json features are currently disabled.</summary>
        /// <returns>True if every option is disabled; false otherwise.</returns>
        public bool EverythingDisabled()
        {
            return !BuildOnAllTerrainFeatures && !BuildOnOtherBuildings && !BuildOnWater && !BuildOnImpassableTiles && !BuildOnNoFurnitureTiles && !BuildOnCaveAndShippingZones; //return true if all of these are false
        }
    }
}
