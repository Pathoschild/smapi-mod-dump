using StardewModdingAPI;

namespace Dem1se.RecurringReminders
{
    /// <summary> Mod config.json data model </summary>
    class ModConfig
    {
        public SButton CustomRemindersButton { get; set; } = SButton.F2;
        public bool SubtlerReminderSound { get; set; } = false;
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