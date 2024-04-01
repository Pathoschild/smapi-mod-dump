/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;

namespace MultiSave
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public bool AutoSaveDaily { get; set; } = true;
        public int AutoSaveOnDayOfWeek { get; set; } = 0;
        public int AutoSaveOnDayOfMonth { get; set; } = 0;
        public int MaxDaysOldToKeep { get; set; } = 7;
        public SButton SaveButton { get; set; } = SButton.None;
    }
}
