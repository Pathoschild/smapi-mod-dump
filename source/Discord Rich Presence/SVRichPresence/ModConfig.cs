using StardewModdingAPI;

namespace SVRichPresence {
	class ModConfig {
		public SButton ReloadConfigButton = SButton.F5;
		public bool ShowGlobalPlayTime = false;
		public MenuPresence MenuPresence = new MenuPresence();
		public GamePresence GamePresence = new GamePresence();
	}
	class MenuPresence {
		public bool ForceSmallImage = false;
		public string Details = "";
		public string State = "In Menus";
		public string LargeImageText = "{{ Activity }}";
		public string SmallImageText = "";
	}
	class GamePresence : MenuPresence {
		public bool ShowSeason = true;
		public bool ShowFarmType = true;
		public bool ShowWeather = true;
		public bool ShowPlayerCount = true;
		public bool ShowPlayTime = true;

		public GamePresence() {
			Details = "{{ FarmName }} | {{ Money }}";
			State = "{{ GameInfo }}";
			SmallImageText = "{{ Date }}";
		}
	}
}
