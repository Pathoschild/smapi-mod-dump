/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using StardewModdingAPI;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>A set of variables describing the spawn settings for an "area" on a single map. Used primarily by subclasses in most versions.</summary>
        private class SpawnArea
        {
            public string UniqueAreaID { get; set; } = "";
            public string MapName { get; set; } = "Farm";
            public int MinimumSpawnsPerDay { get; set; } = 0;
            public int MaximumSpawnsPerDay { get; set; } = 0;

            public string[] AutoSpawnTerrainTypes //supports a previously used name for IncludeTerrainTypes (changed in v1.7.0)
            {
                set
                {
                    IncludeTerrainTypes = value;
                }
            }

            private string[] includeTerrainTypes = new string[0];
            public string[] IncludeTerrainTypes
            {
                get
                {
                    return includeTerrainTypes ?? new string[0]; //return default if null
                }
                set
                {
                    includeTerrainTypes = value;
                }
            }

            private string[] excludeTerrainTypes = new string[0];
            public string[] ExcludeTerrainTypes
            {
                get
                {
                    return excludeTerrainTypes ?? new string[0]; //return default if null
                }
                set
                {
                    excludeTerrainTypes = value;
                }
            }

            public string[] IncludeAreas //supports a previously used name for IncludeCoordinates (changed in v1.7.0)
            {
                set
                {
                    IncludeCoordinates = value;
                }
            }

            private string[] includeCoordinates = new string[0];
            public string[] IncludeCoordinates
            {
                get
                {
                    return includeCoordinates ?? new string[0]; //return default if null
                }
                set
                {
                    includeCoordinates = value;
                }
            }

            public string[] ExcludeAreas //supports a previously used name for ExcludeCoordinates (changed in v1.7.0)
            {
                set
                {
                    ExcludeCoordinates = value;
                }
            }

            private string[] excludeCoordinates = new string[0];
            public string[] ExcludeCoordinates
            {
                get
                {
                    return excludeCoordinates ?? new string[0]; //return default if null
                }
                set
                {
                    excludeCoordinates = value;
                }
            }

            public string StrictTileChecking { get; set; } = "Maximum";

            private SpawnTiming spawnTiming = new SpawnTiming();
            public SpawnTiming SpawnTiming
            {
                get
                {
                    return spawnTiming ?? new SpawnTiming(); //return default if null
                }
                set
                {
                    spawnTiming = value;
                }
            }

            private ExtraConditions extraConditions = new ExtraConditions();
            public ExtraConditions ExtraConditions
            {
                get
                {
                    return extraConditions ?? new ExtraConditions(); //return default if null
                }
                set
                {
                    extraConditions = value;
                }
            }

            public int? DaysUntilSpawnsExpire { get; set; } = null;

            public SpawnArea()
            {

            }
        }
    }
}