using StardewModdingAPI;

namespace Dem1se.CustomReminders
{
    /// <summary> Mod config.json data model </summary>
    class ModConfig
    {
        public SButton CustomRemindersButton { get; set; } = SButton.F2;
        public bool SubtlerReminderSound { get; set; } = false;
        public SButton FarmhandInventoryButton { get; set; } = SButton.E;
    }

    /// <summary> Data model for reminders </summary>
    class ReminderModel
    {
        public ReminderModel(string reminderMessage, int daysSinceStart, int time24hrs)
        {
            ReminderMessage = reminderMessage;
            DaysSinceStart = daysSinceStart;
            Time = time24hrs;
        }
        public string ReminderMessage { get; set; }
        public int DaysSinceStart { get; set; }
        public int Time { get; set; }
    }
}