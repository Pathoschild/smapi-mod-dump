/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using Newtonsoft.Json;
using StardewModdingAPI.Utilities;

namespace Leclair.Stardew.Almanac {

	/*public enum UnlockSource {
		Almanac,
		MagicAlmanac,
		IslandAlmanac,
		Shop,
		Flag
	};

	public class PageUnlock {
		public UnlockSource Source { get; set; }
		public int Cost { get; set; } = 0;
		public string Shop { get; set; } = null;
		public string Flag { get; set; } = null;

		public PageUnlock() { }
		public PageUnlock(UnlockSource source) {
			Source = source;
		}
		public PageUnlock(string shop, int cost) {
			Source = UnlockSource.Shop;
			Shop = shop;
			Cost = cost;
		}
		public PageUnlock(string flag) {
			Source = UnlockSource.Flag;
			Flag = flag;
		}
	}*/

	public enum ButtonPosition {
		Disabled,
		TopLeft,
		BottomLeft,
		TrashRight,
		TrashDown,
		OrganizeRight
	}

	public class ModConfig {

		public string Theme { get; set; } = "automatic";

		public bool DebugMode { get; set; } = false;

		// General
		public bool AlmanacAlwaysAvailable { get; set; } = false;

		public bool IslandAlwaysAvailable { get; set; } = false;

		public bool MagicAlwaysAvailable { get; set; } = false;

		public ButtonPosition AlmanacButtonPos { get; set; } = ButtonPosition.OrganizeRight;

		public bool RestoreAlmanacState { get; set; } = true;

		public int CycleTime { get; set; } = 1000;

		// Custom Seed
		public ulong? CustomSeed { get; set; } = null;

		// Page Sources
		/*public PageUnlock SourceCrop { get; set; } = new(UnlockSource.Almanac);
		public PageUnlock SourceWeather { get; set; } = new(UnlockSource.Almanac);
		public PageUnlock SourceIslandWeather { get; set; } = new(UnlockSource.IslandAlmanac);
		public PageUnlock SourceFortune { get; set; } = new(UnlockSource.MagicAlmanac);
		public PageUnlock SourceMines { get; set; } = new("QiGemShop", 30);
		public PageUnlock SourceTrain { get; set; } = new(UnlockSource.Almanac);
		public PageUnlock SourceNotices { get; set; } = new(UnlockSource.Almanac);
		public PageUnlock SourceFish { get; set; } = new(UnlockSource.Almanac);*/

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
		public int WeatherForecastLength { get; set; } = -1;

		// Fishing Page
		public bool ShowFishing { get; set; } = true;
		public bool ShowFishTank { get; set; } = true;
		public bool DecorateFishTank { get; set; } = true;
		public bool FishShowLegendary { get; set; } = false;


		// Fortune Page
		public bool ShowFortunes { get; set; } = true;
		public bool EnableDeterministicLuck { get; set; } = true;
		public bool ShowExactLuck { get; set; } = false;
		public int LuckForecastLength { get; set; } = -1;

		// Train Page
		public bool ShowTrains { get; set; } = true;

		// Mines Page
		public bool ShowMines { get; set; } = true;

		// Notices
		public bool ShowNotices { get; set; } = true;

		public bool NoticesShowAnniversaries { get; set; } = true;
		public bool NoticesShowFestivals { get; set; } = true;
		public bool NoticesShowGathering { get; set; } = true;
		public MerchantMode NoticesShowMerchant { get; set; } = MerchantMode.Visit;
		public bool NoticesShowTrains { get; set; } = false;
	}

	public enum MerchantMode {
		Disabled,
		Visit,
		Stock
	};
}
