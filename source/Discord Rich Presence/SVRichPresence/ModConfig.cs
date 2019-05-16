using StardewModdingAPI;

namespace SVRichPresence {
	internal class ModConfig {
		public SButton ReloadConfigButton = SButton.F5;
		public bool ShowGlobalPlayTime = false;
		public MenuPresence MenuPresence = new MenuPresence();
		public GamePresence GamePresence = new GamePresence();
	}

	internal class MenuPresence {
		public bool ForceSmallImage = false;
		public string Details = "";
		public string State = "In Menus";
		public string LargeImageText = "{{ Activity }}";
		public string SmallImageText = "";
	}

	internal class GamePresence : MenuPresence {
		public bool ShowSeason = true;
		public bool ShowFarmType = true;
		public bool ShowWeather = true;
		public bool ShowPlayTime = true;
		public bool AllowAskToJoin = true;
		
		public GamePresence() {
			Details = "{{ FarmName }} | {{ Money }}";
			State = "{{ GameInfo }}";
			SmallImageText = "{{ Date }}";
		}
	}
}
