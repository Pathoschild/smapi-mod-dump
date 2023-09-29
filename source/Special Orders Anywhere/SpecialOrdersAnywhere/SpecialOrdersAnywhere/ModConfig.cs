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
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace AcidicNic.SpecialOrdersAnywhere {
    public class ModConfig {
        public SButton ActivateKey { get; set; } = SButton.P;
        public SButton CycleLeftKey { get; set; } = SButton.OemOpenBrackets;
        public SButton CycleRightKey { get; set; } = SButton.OemCloseBrackets;

        public bool enableCalendar { get; set; } = true;
        public bool enableDailyQuests { get; set; } = true;
        public bool enableSpecialOrders { get; set; } = true;
        public bool enableQiSpecialOrders { get; set; } = true;
        public bool enableJournal { get; set; } = false;

        public bool SpecialOrdersBeforeUnlocked { get; set; } = false;
        public bool QiBeforeUnlocked { get; set; } = false;

        internal int MenuLen;

        public ModConfig()
        {
            this.MenuLen = 0;

            if (enableCalendar)
                this.MenuLen++;
            if (enableDailyQuests)
                this.MenuLen++;
            if (enableSpecialOrders)
                this.MenuLen++;
            if (enableQiSpecialOrders)
                this.MenuLen++;
            if (enableQiSpecialOrders)
                this.MenuLen++;
        }
    }
}
