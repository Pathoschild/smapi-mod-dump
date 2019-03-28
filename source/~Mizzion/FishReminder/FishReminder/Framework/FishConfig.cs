namespace FishReminder.Framework
{
    internal class FishConfig
    {
        public bool SendReminderMailDaily { get; set; } = false;
        public bool SendReminderMailWeekly { get; set; } = false;
        public bool SendReminderMailMonthly { get; set; } = true;
    }
}
