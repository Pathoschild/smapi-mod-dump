/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace Leclair.Stardew.Almanac {
	public class ModConfig {

		// General
		public bool AlmanacAlwaysAvailable { get; set; } = false;

		public bool IslandAlwaysAvailable { get; set; } = false;

		public bool MagicAlwaysAvailable { get; set; } = false;

		public bool ShowAlmanacButton { get; set; } = true;


		// Bindings
		public KeybindList UseKey { get; set; } = KeybindList.Parse("F7");


		// Crop Page
		public bool ShowCrops { get; set; } = true;

		public bool ShowPreviews { get; set; } = true;
		public bool PreviewPlantOnFirst { get; set; } = false;
		public bool PreviewUseHarvestSprite { get; set; } = true;


		// Weather Page
		public bool ShowWeather { get; set; } = true;
		public bool EnableDeterministicWeather { get; set; } = true;

		public bool EnableWeatherRules { get; set; } = true;


		// Fortune Page
		public bool ShowFortunes { get; set; } = true;
		public bool EnableDeterministicLuck { get; set; } = true;
		public bool ShowExactLuck { get; set; } = false;

		// Train Page
		public bool ShowTrains { get; set; } = true;


	}
}
