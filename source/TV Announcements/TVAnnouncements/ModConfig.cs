/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Zyin055/TVAnnouncements
**
*************************************************/


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
