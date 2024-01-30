/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

namespace FarmVisitors
{
    internal class ModConfig
    {
        public int CustomChance { get; set; } = 20;
        public int GiftChance { get; set; } = 80;
        public int MaxVisitsPerDay { get; set; } = 3;
        public string Blacklist { get; set; } = "";
        public int StartingHours { get; set; } = 800;
        public int EndingHours { get; set; } = 2000;
        public int Duration { get; set; } = 6;
        public bool Verbose { get; set; }
        public bool Debug { get; set; }
	    public string InLawComments { get; set; } = "VanillaOnly";
        public bool ReplacerCompat { get; set; }
        public bool NeedsConfirmation { get; set; }
        public bool RejectionDialogue { get; set; } = true;
        public bool WalkOnFarm { get; set; } = true;
        public bool Shed { get; set; } = true;
        //public bool AnimalHomes { get; set; }
        public bool Greenhouse { get; set; } = true;
        public bool UniqueDialogue { get; set; } = true;
        public bool Sleepover { get; set; }
        public int SleepoverMinHearts { get; set; } = 3;
        public int SleepoverChance { get; set; } = 10;
    }
}
