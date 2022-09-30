/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/FarmhouseVisits
**
*************************************************/

namespace FarmVisitors
{
    internal class ModConfig
    {
        public int CustomChance { get; set; } = 20;
        public int MaxVisitsPerDay { get; set; } = 3;
        public string Blacklist { get; set; } = "";
        public int StartingHours { get; set; } = 800;
        public int EndingHours { get; set; } = 2000;
        public int Duration { get; set; } = 6;
        public bool Verbose { get; set; } = false;
        public bool Debug { get; set; } = false;
	    public string InLawComments { get; set; } = "VanillaOnly";
        public bool ReplacerCompat { get; set; } = false;
        public bool NeedsConfirmation { get; set; } = false;
        public bool RejectionDialogue { get; set; } = true;
        public bool Gifts { get; set; } = true;
        public bool WalkOnFarm { get; set; } = false;
    }
}
