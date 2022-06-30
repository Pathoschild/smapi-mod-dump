/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI;

namespace MarketDay
{
    internal class ModConfig
    {
        public bool Progression { get; set; } = true;
        
        public bool OnNextDayIfFestival { get; set; } = true;

        public int DayOfWeek { get; set; } = 6;
        public bool UseAdvancedOpeningOptions { get; set; }
        public bool OpenOnMon { get; set; }
        public bool OpenOnTue { get; set; }
        public bool OpenOnWed { get; set; }
        public bool OpenOnThu { get; set; }
        public bool OpenOnFri { get; set; }
        public bool OpenOnSat { get; set; } = true;
        public bool OpenOnSun { get; set; }
        public bool OpenInSpring { get; set; } = true;
        public bool OpenInSummer { get; set; } = true;
        public bool OpenInFall { get; set; } = true;
        public bool OpenInWinter { get; set; } = true;
        public bool OpenWeek1 { get; set; } = true;
        public bool OpenWeek2 { get; set; } = true;
        public bool OpenWeek3 { get; set; } = true;
        public bool OpenWeek4 { get; set; } = true;
        public int OpeningTime { get; set; } = 8;
        public int ClosingTime { get; set; } = 18;
        public int NumberOfShops { get; set; } = 6;
        public bool OpenInRain { get; set; }
        public bool OpenInSnow { get; set; }
        public bool GMMCompat { get; set; } = true;
        public int RestockItemsPerHour { get; set; } = 3;
        public float StallVisitChance { get; set; } = 0.9f;
        public bool ReceiveMessages { get; set; } = true;
        public bool PeekIntoChests { get; set; }
        public bool RuinTheFurniture { get; set; }
        public Dictionary<string, bool> ShopsEnabled = new();
        public bool VerboseLogging { get; set; }
        public bool ShowShopPositions { get; set; }
        public bool NPCVisitors { get; set; } = true;
        public bool NPCOwnerRescheduling { get; set; } = true;
        public bool NPCVisitorRescheduling { get; set; } = true;
        public bool NPCScheduleReplacement { get; set; } = true;
        public int NumberOfTownieVisitors { get; set; } = 25;
        public int NumberOfRandomVisitors { get; set; } = 4;
        public bool AlwaysMarketDay { get; set; }
        public bool DebugKeybinds { get; set; }
        public SButton OpenConfigKeybind { get; set; } = SButton.V;
        public SButton WarpKeybind { get; set; } = SButton.Z;
        public SButton ReloadKeybind { get; set; } = SButton.R;
        public SButton StatusKeybind { get; set; } = SButton.Q;
    }
}
