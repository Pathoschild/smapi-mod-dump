/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/SpousesIsland
**
*************************************************/

using System.Collections.Generic;

namespace SpousesIsland
{
    internal class ModConfig
    {
        //general
        public int CustomChance { get; set; } = 10;
        public bool ScheduleRandom { get; set; } = false;
        public bool CustomRoom { get; set; } = false;

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

        //children-related
        public bool Allow_Children { get; set; } = true;
        public bool UseFurnitureBed { get; set; } = false;
        public string Childbedcolor { get; set; } = "1"; //if not using furniture bed
    }
}