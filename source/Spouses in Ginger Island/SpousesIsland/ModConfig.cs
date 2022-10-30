/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/SpousesIsland
**
*************************************************/

using SpousesIsland;
using System.Collections.Generic;

namespace SpousesIsland
{
    internal class ModConfig
    {
        //general
        public int CustomChance { get; set; } = 10;
        public bool ScheduleRandom { get; set; } = false;
        public bool CustomRoom { get; set; } = false;

        //children-related
        public bool Allow_Children { get; set; } = true;
        public bool UseFurnitureBed { get; set; } = false;
        public string Childbedcolor { get; set; } = "1"; //if not using furniture bed
        public bool UseModSchedule { get;set; } = true;
        public Dictionary<string, ChildSchedule[]> ChildSchedules { get; set; } = new()
        {
            {
                "default1", 
                new ChildSchedule[]
                    {
                        new ChildSchedule("620", "IslandFarmHouse", 20, 10, 3),
                        new ChildSchedule("1100", "IslandWest", 74, 43 ,3),
                        new ChildSchedule("1400", "IslandWest", 83, 36, 3),
                        new ChildSchedule("1700","IslandWest", 91, 37, 2),
                        new ChildSchedule("a1900","IslandFarmHouse", 15, 12, 0),
                        new ChildSchedule("2000","IslandFarmHouse", 30, 15, 2),
                        new ChildSchedule("2100","IslandFarmHouse", 35, 14, 3)
                    }
            },
            {
                "default2",
                new ChildSchedule[]
                    {
                        new ChildSchedule("620", "IslandFarmHouse", 19, 10, 3),
                        new ChildSchedule("1100", "IslandWest", 75, 46 ,3),
                        new ChildSchedule("1400", "IslandWest", 84, 38, 3),
                        new ChildSchedule("1700","IslandWest", 93, 36, 2),
                        new ChildSchedule("a1900","IslandFarmHouse", 15, 14, 0),
                        new ChildSchedule("2000","IslandFarmHouse", 27, 14, 2),
                        new ChildSchedule("2100","IslandFarmHouse", 36, 14, 2)
                    }
            }
        };

        /* devan stuff *
         * which (arguably) falls into children-related
         * they're a babysitter
         */
        public bool NPCDevan { get; set; } = false;
        public bool SeasonalDevan { get; set; } = false;

        //spouses integrated
        public bool Allow_Abigail { get; set; } = true;
        public bool Allow_Alex { get; set; } = true;
        public bool Allow_Elliott { get; set; } = true;
        public bool Allow_Emily { get; set; } = true;
        public bool Allow_Haley { get; set; } = true;
        public bool Allow_Harvey { get; set; } = true;
        public bool Allow_Krobus { get; set; } = true;
        public bool Allow_Leah { get; set; } = true;
        public bool Allow_Maru { get; set; } = true;
        public bool Allow_Penny { get; set; } = true;
        public bool Allow_Sam { get; set; } = true;
        public bool Allow_Sebastian { get; set; } = true;
        public bool Allow_Shane { get; set; } = true;
        public bool Allow_Claire { get; set; } = true;
        public bool Allow_Lance { get; set; } = true;
        public bool Allow_Magnus { get; set; } = true;
        public bool Allow_Olivia { get; set; } = true;
        public bool Allow_Sophia { get; set; } = true;
        public bool Allow_Victor { get; set; } = true;

        //debug
        public bool Verbose { get; set; } = false;
        public bool Debug { get; set; } = false;
    }
}