using StardewModdingAPI;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        //a set of variables including a map name, terrain type(s) to auto-detect, and manually defined included/excluded areas for object spawning
        private class SpawnArea
        {
            public string UniqueAreaID { get; set; } = ""; //added in version 1.4.0

            public string MapName { get; set; } = "Farm";

            //custom properties were added in version 1.5.0 to handle configuration errors
            private int minimumSpawnsPerDay = 0;
            public int MinimumSpawnsPerDay
            {
                get
                {
                    if (minimumSpawnsPerDay > maximumSpawnsPerDay) //if the min and max are in the wrong order
                    {
                        //swap min and max
                        int temp = minimumSpawnsPerDay;
                        minimumSpawnsPerDay = maximumSpawnsPerDay;
                        maximumSpawnsPerDay = temp;
                    }

                    return minimumSpawnsPerDay;
                }
                set
                {
                    minimumSpawnsPerDay = value;
                }
            }

            private int maximumSpawnsPerDay = 0;
            public int MaximumSpawnsPerDay
            {
                get
                {
                    if (minimumSpawnsPerDay > maximumSpawnsPerDay) //if the min and max are in the wrong order
                    {
                        //swap min and max
                        int temp = minimumSpawnsPerDay;
                        minimumSpawnsPerDay = maximumSpawnsPerDay;
                        maximumSpawnsPerDay = temp;
                    }

                    return maximumSpawnsPerDay;
                }
                set
                {
                    maximumSpawnsPerDay = value;
                }
            }

            private string[] autoSpawnTerrainTypes = new string[0];
            public string[] AutoSpawnTerrainTypes
            {
                get
                {
                    return autoSpawnTerrainTypes ?? new string[0]; //return default if null
                }
                set
                {
                    autoSpawnTerrainTypes = value;
                }
            }

            private string[] includeAreas = new string[0];
            public string[] IncludeAreas
            {
                get
                {
                    return includeAreas ?? new string[0]; //return default if null
                }
                set
                {
                    includeAreas = value;
                }
            }

            private string[] excludeAreas = new string[0];
            public string[] ExcludeAreas
            {
                get
                {
                    return excludeAreas ?? new string[0]; //return default if null
                }
                set
                {
                    excludeAreas = value;
                }
            }

            public string StrictTileChecking { get; set; } = "High"; //added in version 1.1.0; default used here to automatically fill it in with SMAPI's json interface

            private ExtraConditions extraConditions; //added in version 1.3.0
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