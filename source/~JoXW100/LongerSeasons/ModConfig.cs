/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/


namespace LongerSeasons
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public bool ExtendBerry { get; set; } = true;

        public int DaysPerMonth { get; set; } = 28;
        public int MonthsPerSeason { get; set; } = 1;
    }
}
