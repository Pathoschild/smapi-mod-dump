/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AcidicNic/StardewValleyMods
**
*************************************************/

using System;
namespace SpecialOrdersAnywhere
{
    public class ModConfig
    {
        public StardewModdingAPI.SButton ActivateKey { get; set; } = StardewModdingAPI.SButton.P;
        public StardewModdingAPI.SButton CycleLeftKey { get; set; } = StardewModdingAPI.SButton.OemOpenBrackets;
        public StardewModdingAPI.SButton CycleRightKey { get; set; } = StardewModdingAPI.SButton.OemCloseBrackets;

        public Boolean SpecialOrdersBeforeUnlocked { get; set; } = false;
        public Boolean QiBeforeUnlocked { get; set; } = false;

        public Boolean enableCalendar { get; set; } = true;
        public Boolean enableDailyQuests { get; set; } = true;
    }
}
