namespace TimeNotifier.Models
{
    /// <summary>
    /// 
    /// </summary>
    class TimeConfig
    {
        /// <summary>
        /// Show if below's propertyname should be shown.
        /// </summary>
        public bool showCallerName = true;
        /// <summary>
        /// Alerts if true in-game every full hour.
        /// </summary>
        public bool AlertOnFullHour = true;
        /// <summary>
        /// Alerts if the minute of the current hour is achived. Zero to disable.
        /// </summary>
        public int AlertSpecificMinute = 30;
        /// <summary>
        /// Alerts every X minute, since last notification. Zero to disable.
        /// </summary>
        public int AlertEveryXMinute = 5;
    }
}
