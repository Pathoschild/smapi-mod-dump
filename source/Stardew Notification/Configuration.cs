namespace StardewNotification
{
	public class Configuration
    {
		// General Notifications
		public bool notifyBirthdays { get; set; } = true;
		public bool notifyBirthdayReminder { get; set; } = true;
		public int birthdayReminderTime { get; set; } = 1700; // 5:00 pm
		public bool notifyFestivals { get; set; } = true;
		public bool notifyTravelingMerchant { get; set; } = true;
		public bool notifyToolUpgrade { get; set; } = true;
		public bool notifyQueenOfSauce { get; set; } = true;
		public bool notifyMaxLuck { get; set; } = true;
		public bool notifySeasonalForage { get; set; } = true;

		// Harvest Notifications
		public bool notifyFarmCave { get; set; } = true;
		public bool notifyGreenhouseCrops { get; set; } = true;

		// Production Notifications
        public bool notifyShed { get; set; } = true;
		public bool notifyFarm { get; set; } = true;
		public bool notifyGreenhouse { get; set; } = true;
		public bool notifyCellar { get; set; } = true;
    }
}
