
namespace TVAnnouncements
{
    class ModConfig
    {
        public bool ShowDailyLuck { get; set; }
        public bool ShowDailyLuckNumber { get; set; }
        public bool ShowWeatherForcast { get; set; }
        public bool ShowQueenOfSauce { get; set; }
        public int NotificationDuration { get; set; }

        public ModConfig()
        {
            this.ShowDailyLuck = true;
            this.ShowDailyLuckNumber = true;
            this.ShowWeatherForcast = false;
            this.ShowQueenOfSauce = true;
            this.NotificationDuration = 8000;
        }
    }
}
