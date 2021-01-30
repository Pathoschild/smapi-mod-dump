/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FayneAldan/SVRichPresence
**
*************************************************/

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
		
		public GamePresence() {
			Details = "{{ FarmName }} | {{ Money }}";
			State = "{{ GameInfo }}";
			SmallImageText = "{{ Date }}";
		}
	}
}
