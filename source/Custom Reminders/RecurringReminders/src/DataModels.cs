/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dem1se/CustomReminders
**
*************************************************/

using StardewModdingAPI;

namespace Dem1se.RecurringReminders
{
    /// <summary> Mod config.json data model </summary>
    class ModConfig
    {
        public SButton CustomRemindersButton { get; set; } = SButton.F2;
        public bool SubtlerReminderSound { get; set; } = false;
        public bool EnableMobilePhoneApp { get; set; } = true;
    }

    /// <summary> Data model for reminders </summary>
    class RecurringReminderModel
    {
        public RecurringReminderModel(string reminderMessage, int reminderStartDate, int daysInterval, int time24hrs)
        {
            ReminderMessage = reminderMessage;
            ReminderStartDate = reminderStartDate;
            DaysInterval = daysInterval;
            Time = time24hrs;
        }
        public string ReminderMessage { get; set; }
        public int ReminderStartDate { get; set; }
        public int DaysInterval { get; set; }
        public int Time { get; set; }
    }
}