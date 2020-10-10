/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

namespace FishReminder.Framework
{
    internal class FishConfig
    {
        public bool SendReminderMailDaily { get; set; } = false;
        public bool SendReminderMailWeekly { get; set; } = false;
        public bool SendReminderMailMonthly { get; set; } = true;
    }
}
