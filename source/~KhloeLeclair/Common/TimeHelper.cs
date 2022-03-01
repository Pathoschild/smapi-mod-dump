/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using StardewValley;

namespace Leclair.Stardew.Common {
	public static class TimeHelper {

		private static LocalizedContentManager.LanguageCode? Code = null;
		private static string AMString;
		private static string PMString;

		private static void CacheAMPM() {
			if (Code.HasValue && Code.Value == LocalizedContentManager.CurrentLanguageCode)
				return;

			Code = LocalizedContentManager.CurrentLanguageCode;
			AMString = Game1.content.LoadString("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10370");
			PMString = Game1.content.LoadString("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10371");
		}

		public static bool IsTwelveHour() {
			switch (LocalizedContentManager.CurrentLanguageCode) {
				case LocalizedContentManager.LanguageCode.ru:
				case LocalizedContentManager.LanguageCode.pt:
				case LocalizedContentManager.LanguageCode.es:
				case LocalizedContentManager.LanguageCode.de:
				case LocalizedContentManager.LanguageCode.th:
				case LocalizedContentManager.LanguageCode.fr:
				case LocalizedContentManager.LanguageCode.tr:
				case LocalizedContentManager.LanguageCode.hu:
					return false;
				default:
					return true;
			}
		}

		public static string FormatTime(int time) {
			// Limit it to one day.
			time %= 2400;

			int hours = time / 100;
			int minutes = time % 100;

			bool twelve = IsTwelveHour();
			bool is_pm = twelve && hours >= 12;

			if (is_pm)
				hours -= 12;

			if (twelve && hours == 0 && LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.zh)
				hours = 12;

			string hourPad = ""; // hours < 10 ? "0" : "";
			string minutePad = minutes < 10 ? "0" : "";
			string ampm;

			if (twelve) {
				CacheAMPM();
				ampm = is_pm ? PMString : AMString;

			} else
				ampm = "";

			return $"{hourPad}{hours}:{minutePad}{minutes}{ampm}";
		}

	}
}
