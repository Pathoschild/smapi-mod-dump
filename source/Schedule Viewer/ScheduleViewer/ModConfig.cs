/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/BinaryLip/ScheduleViewer
**
*************************************************/

using StardewModdingAPI;

namespace ScheduleViewer
{
    internal class ModConfig
    {
        public enum SortType : ushort
        {
            AlphabeticalAscending = 0,
            AlphabeticalDescending = 1,
            HeartsAscending = 2,
            HeartsDescending = 3
        }

        public SButton ShowSchedulesKey { get; set; } = SButton.V;
        public bool UseAddress { get; set; } = true;
        public bool DisableHover { get; set; } = false;
        public bool UseLargerFontForScheduleDetails { get; set; } = false;
        public SortType NPCSortOrder { get; set; } = SortType.AlphabeticalAscending;
        public bool OnlyShowMetNPCs { get; set; } = false;
        public bool OnlyShowSocializableNPCs { get; set; } = true;
    }
}
