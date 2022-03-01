/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;

using Leclair.Stardew.Common.Events;
using Leclair.Stardew.Common.Integrations.GenericModConfigMenu;
using Leclair.Stardew.Common.UI.Overlay;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

using StardewValley;
using StardewValley.Menus;

using Leclair.Stardew.Almanac.Crops;
using Leclair.Stardew.Almanac.Managers;
using Leclair.Stardew.Almanac.Pages;

namespace Leclair.Stardew.Almanac {
	public class ModEntry : ModSubscriber {

		public static readonly int Event_Base   = 11022000;
		public static readonly int Event_Island = 11022001;
		public static readonly int Event_Magic  = 11022002;

		public static ModEntry instance;
		public static ModAPI API;

		private readonly PerScreen<IClickableMenu> CurrentMenu = new();
		private readonly PerScreen<IOverlay> CurrentOverlay = new();

		public ModConfig Config;

		public WeatherManager Weather;

		internal AssetManager Assets;
		internal CropManager Crops;

		internal readonly List<Func<Menus.AlmanacMenu, ModEntry, IAlmanacPage>> PageBuilders = new();

		private GMCMIntegration<ModConfig, ModEntry> GMCMIntegration;

		public override void Entry(IModHelper helper) {
			base.Entry(helper);
			instance = this;
			API = new(this);

			Assets = new(this);

			I18n.Init(Helper.Translation);

			// Read Config
			Config = Helper.ReadConfig<ModConfig>();

			Weather = new WeatherManager(this);

			// Init
			RegisterBuilder(CoverPage.GetPage);
			RegisterBuilder(CropPage.GetPage);
			RegisterBuilder(WeatherPage.GetPage);
			RegisterBuilder(WeatherPage.GetIslandPage);
			RegisterBuilder(TrainPage.GetPage);
			RegisterBuilder(FortunePage.GetPage);
			RegisterBuilder(MinesPage.GetPage);
		}

		public override object GetApi() {
			return API;
		}

		#region Page Management

		void RegisterBuilder(Func<Menus.AlmanacMenu, ModEntry, IAlmanacPage> builder) {
			PageBuilders.Add(builder);
		}

		#endregion

		#region Events

		[Subscriber]
		private void OnButton(object sender, ButtonPressedEventArgs e) {
			if (Game1.activeClickableMenu != null || !(Config.UseKey?.JustPressed() ?? false))
				return;

			Helper.Input.SuppressActiveKeybinds(Config.UseKey);

			// If the player hasn't seen the event where they receive the Almanac, don't
			// let them use it unless it's always available.
			if (! HasAlmanac(Game1.player) )
				return;

			if (Game1.activeClickableMenu != null || Game1.CurrentEvent != null)
				return;

			Game1.activeClickableMenu = new Menus.AlmanacMenu(Game1.Date.Year);
		}


		// We mark this event as high priority so we can change tomorrow's weather
		// before other mods might rely on it.

		[Subscriber]
		[EventPriority(EventPriority.High)]
		private void OnDayStarted(object sender, DayStartedEventArgs e) {
			int seed = GetBaseWorldSeed();

			if (Config.EnableDeterministicLuck && Game1.IsMasterGame) {
				Game1.player.team.sharedDailyLuck.Value = LuckHelper.GetLuckForDate(seed, Game1.Date);
			}

			if (Config.EnableDeterministicWeather) {
				WorldDate tomorrow = new WorldDate(Game1.Date);
				tomorrow.TotalDays++;

				// Main Weather
				Game1.weatherForTomorrow = Weather.GetWeatherForDate(seed, tomorrow, GameLocation.LocationContext.Default);
				if (Game1.IsMasterGame)
					Game1.netWorldState.Value.GetWeatherForLocation(GameLocation.LocationContext.Default).weatherForTomorrow.Value = Game1.weatherForTomorrow;

				// Island Weather
				if (Game1.IsMasterGame && Utility.doesAnyFarmerHaveOrWillReceiveMail("Visited_Island")) {
					var ctx = GameLocation.LocationContext.Island;

					Game1.netWorldState.Value.GetWeatherForLocation(ctx)
						.weatherForTomorrow.Value = Weather
							.GetWeatherForDate(seed, tomorrow, ctx);
				}
			}
		}

		[Subscriber]
		private void OnGameLaunched(object sender, GameLaunchedEventArgs e) {
			// More Init
			Crops = new(this);
			RegisterConfig();

			Helper.ConsoleCommands.Add("al_update", "Invalidate cached data.", (name, args) => {
				Crops.Invalidate();
				Weather.Invalidate();
			});

			Helper.ConsoleCommands.Add("al_forecast", "Get the forecast for the loaded save.", (name, args) => {
				int seed = GetBaseWorldSeed();
				WorldDate date = new WorldDate(Game1.Date);
				for (int i = 0; i < 4 * 28; i++) {
					int weather = Weather.GetWeatherForDate(seed, date, GameLocation.LocationContext.Default);
					Log($"Date: {date.Localize()} -- Weather: {WeatherHelper.GetWeatherName(weather)}");
					date.TotalDays++;
				}
			});
		}

		[Subscriber]
		private void OnMenuChanged(object sender, MenuChangedEventArgs e) {
			IClickableMenu menu = Game1.activeClickableMenu;
			if (CurrentMenu.Value == menu)
				return;

			CurrentMenu.Value = menu;

			if (CurrentOverlay.Value != null) {
				CurrentOverlay.Value.Dispose();
				CurrentOverlay.Value = null;
			}

			if (!HasAlmanac(Game1.player) || ! Config.ShowAlmanacButton)
				return;

			InventoryPage page = null;

			if (menu is InventoryPage)
				page = menu as InventoryPage;
			else if (menu is GameMenu gm) {
				for(int i = 0; i < gm.pages.Count; i++) {
					if (gm.pages[i] is InventoryPage)
						page = gm.pages[i] as InventoryPage;
				}
			}

			if (page != null)
				CurrentOverlay.Value = new Overlays.InventoryOverlay(page, Game1.player);
		}

		#endregion

		#region Configuration

		public void SaveConfig() {
			Helper.WriteConfig(Config);
		}

		public void ResetConfig() {
			Config = new();
		}

		public bool HasGMCM() {
			return GMCMIntegration?.IsLoaded ?? false;
		}

		public void OpenGMCM() {
			if (HasGMCM())
				GMCMIntegration.OpenMenu();
		}

		private void RegisterConfig() {
			GMCMIntegration = new(this, () => Config, ResetConfig, SaveConfig);
			if (!GMCMIntegration.IsLoaded)
				return;

			GMCMIntegration.Register(true);

			GMCMIntegration
				.Add(
					I18n.Settings_Button,
					I18n.Settings_ButtonDesc,
					c => c.ShowAlmanacButton,
					(c, v) => c.ShowAlmanacButton = v
				)
				.Add(
					I18n.Settings_Available,
					I18n.Settings_AvailableDesc,
					c => c.AlmanacAlwaysAvailable,
					(c, v) => c.AlmanacAlwaysAvailable = v
				)
				.Add(
					I18n.Settings_Island,
					I18n.Settings_IslandDesc,
					c => c.IslandAlwaysAvailable,
					(c, v) => c.IslandAlwaysAvailable = v
				)
				.Add(
					I18n.Settings_Magic,
					I18n.Settings_MagicDesc,
					c => c.MagicAlwaysAvailable,
					(c, v) => c.MagicAlwaysAvailable = v
				);

			GMCMIntegration
				.AddLabel(I18n.Settings_Controls, null, "bindings")
				.AddLabel(I18n.Settings_Crops, null, "page:crop")
				.AddLabel(I18n.Settings_Weather, null, "page:weather")
				.AddLabel(I18n.Settings_Train, null, "page:train")
				.AddLabel(I18n.Settings_Fortune, null, "page:fortune");

			GMCMIntegration.StartPage("bindings", I18n.Settings_Controls);
			GMCMIntegration
				.Add(
					I18n.Settings_Controls_Almanac,
					I18n.Settings_Controls_AlmanacDesc,
					c => c.UseKey,
					(c, v) => c.UseKey = v
				);

			GMCMIntegration.StartPage("page:crop", I18n.Settings_Crops);

			GMCMIntegration
				.Add(
					I18n.Settings_Enable,
					I18n.Settings_EnableDesc,
					c => c.ShowCrops,
					(c, v) => c.ShowCrops = v
				);

			GMCMIntegration.AddLabel(""); // Spacer

			GMCMIntegration.AddLabel(I18n.Settings_Crops_Preview);
			GMCMIntegration.AddParagraph(I18n.Settings_Crops_PreviewDesc);

			GMCMIntegration
				.Add(
					I18n.Settings_Crops_Preview_Enable,
					null,
					c => c.ShowPreviews,
					(c, v) => c.ShowPreviews = v
				);

			GMCMIntegration
				.Add(
					I18n.Settings_Crops_Preview_Sprite,
					I18n.Settings_Crops_Preview_SpriteDesc,
					c => c.PreviewUseHarvestSprite,
					(c, v) => c.PreviewUseHarvestSprite = v
				);

			GMCMIntegration
				.Add(
					I18n.Settings_Crops_Preview_Plantonfirst,
					I18n.Settings_Crops_Preview_PlantonfirstDesc,
					c => c.PreviewPlantOnFirst,
					(c, v) => c.PreviewPlantOnFirst = v
				);

			GMCMIntegration.StartPage("page:weather", I18n.Settings_Weather);

			GMCMIntegration
				.Add(
					I18n.Settings_Enable,
					I18n.Settings_EnableDesc,
					c => c.ShowWeather,
					(c, v) => c.ShowWeather = v
				);

			GMCMIntegration
				.SetTitleOnly(true)
				.Add(
					I18n.Settings_Weather_Deterministic,
					I18n.Settings_Weather_DeterministicDesc,
					c => c.EnableDeterministicWeather,
					(c, v) => c.EnableDeterministicWeather = v
				)
				.Add(
					I18n.Settings_Weather_Rules,
					I18n.Settings_Weather_RulesDesc,
					c => c.EnableWeatherRules,
					(c, v) => c.EnableWeatherRules = v
				)
				.SetTitleOnly(false);

			GMCMIntegration.StartPage("page:train", I18n.Settings_Train);

			GMCMIntegration
				.Add(
					I18n.Settings_Enable,
					I18n.Settings_EnableDesc,
					c => c.ShowTrains,
					(c, v) => c.ShowTrains = v
				);

			GMCMIntegration.StartPage("page:fortune", I18n.Settings_Fortune);

			GMCMIntegration
				.Add(
					I18n.Settings_Enable,
					I18n.Settings_EnableDesc,
					c => c.ShowFortunes,
					(c, v) => c.ShowFortunes = v
				)
				.Add(
					I18n.Settings_Fortune_Exact,
					I18n.Settings_Fortune_ExactDesc,
					c => c.ShowExactLuck,
					(c, v) => c.ShowExactLuck = v
				);

			GMCMIntegration
				.SetTitleOnly(true)
				.Add(
					I18n.Settings_Fortune_Deterministic,
					I18n.Settings_Fortune_DeterministicDesc,
					c => c.EnableDeterministicLuck,
					(c, v) => c.EnableDeterministicLuck = v
				)
				.SetTitleOnly(false);
		}

		#endregion

		public bool HasAlmanac(Farmer who) {
			return Config.AlmanacAlwaysAvailable || who.eventsSeen.Contains(Event_Base);
		}

		public bool HasIsland(Farmer who) {
			return Config.IslandAlwaysAvailable || who.eventsSeen.Contains(Event_Island);
		}

		public bool HasMagic(Farmer who) {
			return Config.MagicAlwaysAvailable || who.eventsSeen.Contains(Event_Magic);
		}

		public int GetBaseWorldSeed() {
			// TODO: Check configuration?
			return (int) Game1.uniqueIDForThisGame;
		}

	}
}
