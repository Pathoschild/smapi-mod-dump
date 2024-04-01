/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AcidicNic/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace AcdicNic.Stardew.MenuCycle.Framework
{
    public class ModConfig
    {
        public KeybindList ActivateKey { get; set; } = new KeybindList(SButton.P);
        public KeybindList CycleLeftKey { get; set; } = new KeybindList(SButton.OemOpenBrackets);
        public KeybindList CycleRightKey { get; set; } = new KeybindList(SButton.OemCloseBrackets);

        public bool EnableCalendar { get; set; } = true;
        public bool EnableDailyQuests { get; set; } = true;
        public bool EnableSpecialOrders { get; set; } = true;
        public bool EnableQiSpecialOrders { get; set; } = true;
        public bool EnableJournal { get; set; } = false;

        public bool SpecialOrdersBeforeUnlocked { get; set; } = false;
        public bool QiBeforeUnlocked { get; set; } = false;
    }
}
