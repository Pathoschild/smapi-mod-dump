/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using HarmonyLib;

using Newtonsoft.Json.Linq;

using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Events;
using Leclair.Stardew.Common.Integrations.GenericModConfigMenu;
using Leclair.Stardew.Common.UI;
using Leclair.Stardew.Common.UI.Overlay;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;

using Leclair.Stardew.Almanac.Crops;
using Leclair.Stardew.Almanac.Fish;
using Leclair.Stardew.Almanac.Managers;
using Leclair.Stardew.Almanac.Pages;

namespace Leclair.Stardew.Almanac;

public class ModEntry : ModSubscriber {

	public static readonly string NPCMapLocationPath = "Mods/Bouhm.NPCMapLocations/NPCs";

	public static readonly string Mail_Prefix = "leclair.almanac";

	public static readonly string Mail_Has_Base = $"{Mail_Prefix}.has_base";
	public static readonly string Mail_Has_Island = $"{Mail_Prefix}.has_island";
	public static readonly string Mail_Has_Magic = $"{Mail_Prefix}.has_magic";

	public static readonly string Mail_Seen_Base = $"{Mail_Prefix}.saw_base";
	public static readonly string Mail_Seen_Island = $"{Mail_Prefix}.saw_island";
	public static readonly string Mail_Seen_Magic = $"{Mail_Prefix}.saw_magic";

	/*public static readonly int Event_Base   = 11022000;
	public static readonly int Event_Island = 11022001;
	public static readonly int Event_Magic  = 11022002;*/

	public static int DaysPerMonth { get; private set; } = WorldDate.DaysPerMonth;

#nullable disable
	public static ModEntry Instance { get; private set; }
	public static ModAPI API { get; private set; }
#nullable enable

	internal Harmony? Harmony;

	private readonly PerScreen<IClickableMenu?> CurrentMenu = new();
	private readonly PerScreen<IOverlay?> CurrentOverlay = new();

#nullable disable
	public ModConfig Config;

	public WeatherManager Weather;
	public LuckManager Luck;
	public NoticesManager Notices;
	internal CropManager Crops;
	internal FishManager Fish;
	internal BookManager Books;

	internal AssetManager Assets;

	internal ThemeManager<Models.Theme> ThemeManager;
#nullable enable

	internal Models.Theme Theme => ThemeManager.Theme;

	internal readonly List<Func<Menus.AlmanacMenu, ModEntry, IAlmanacPage?>> PageBuilders = new();

	internal Dictionary<string, Models.HeadSize>? HeadSizes;

	private GMCMIntegration<ModConfig, ModEntry>? GMCMIntegration;

	internal Integrations.ContentPatcher.CPIntegration? intCP;
	internal Integrations.LuckSkill.LSIntegration? intLS;
	internal Integrations.JsonAssets.JAIntegration? intJA;
	internal Integrations.MoreGiantCrops.MGCIntegration? intMGC;

	private bool ShouldCloseGMCM = false;

	public override void Entry(IModHelper helper) {
		base.Entry(helper);
		SpriteHelper.SetHelper(helper);
		RenderHelper.SetHelper(helper);

		Instance = this;
		API = new(this);
		Harmony = new Harmony(ModManifest.UniqueID);

		// Patches
		// Patches.GameMenu_Patches.Patch(this);
		SpriteText_Patches.Patch(Harmony, Monitor);

		Assets = new(this);

		I18n.Init(Helper.Translation);

		// Read Config
		Config = Helper.ReadConfig<ModConfig>();

		Crops = new(this);
		Fish = new(this);
		Weather = new(this);
		Luck = new(this);
		Notices = new(this);
		Books = new(this);

		ThemeManager = new(this, Config.Theme);
		ThemeManager.ThemeChanged += OnThemeChanged;
		ThemeManager.Discover();

		// Init
		RegisterBuilder(CoverPage.GetPage);
		RegisterBuilder(CropPage.GetPage);
		RegisterBuilder(WeatherPage.GetPage);
		RegisterBuilder(WeatherPage.GetIslandPage);
		RegisterBuilder(TrainPage.GetPage);
		RegisterBuilder(FortunePage.GetPage);
		RegisterBuilder(MinesPage.GetPage);
		RegisterBuilder(NoticesPage.GetPage);
		RegisterBuilder(FishingPage.GetPage);
	}

	public override object GetApi() {
		return API;
	}

	#region Loading

	private void LoadHeads() {
		const string path = "assets/heads.json";
		Dictionary<string, Models.HeadSize>? heads = null;

		try {
			heads = Helper.Data.ReadJsonFile<Dictionary<string, Models.HeadSize>>(path);
			if (heads == null)
				Log($"The {path} file is missing or invalid.", LogLevel.Error);
		} catch(Exception ex) {
			Log($"The {path} file is invalid.", LogLevel.Error, ex);
		}

		if (heads == null)
			heads = new();

		// Read any extra data files
		foreach (var cp in Helper.ContentPacks.GetOwned()) {
			if (!cp.HasFile("heads.json"))
				continue;

			Dictionary<string, Models.HeadSize>? extra = null;
			try {
				extra = cp.ReadJsonFile<Dictionary<string, Models.HeadSize>>("heads.json");
			} catch (Exception ex) {
				Log($"The heads.json file of {cp.Manifest.Name} is invalid.", LogLevel.Error, ex);
			}

			if (extra != null)
				foreach (var entry in extra)
					if (!string.IsNullOrEmpty(entry.Key))
						heads[entry.Key] = entry.Value;
		}

		// Now, read the data file used by NPC Map Locations. This is
		// convenient because a lot of mods support it.
		Dictionary<string, JObject>? content = null;

		try {
			content = Helper.GameContent.Load<Dictionary<string, JObject>>(NPCMapLocationPath);

		} catch (Exception) {
			/* Nothing~ */
		}

		if (content != null) {
			int count = 0;

			foreach (var entry in content) {
				if (heads.ContainsKey(entry.Key))
					continue;

				int offset;
				try {
					offset = entry.Value.Value<int>("MarkerCropOffset");
				} catch (Exception) {
					continue;
				}

				heads[entry.Key] = new() {
					OffsetY = offset
				};
				count++;
			}

			Log($"Loaded {count} head offsets from NPC Map Location data.");
		}

		HeadSizes = heads;
	}

	#endregion

	#region Page Management

	void RegisterBuilder(Func<Menus.AlmanacMenu, ModEntry, IAlmanacPage?> builder) {
		PageBuilders.Add(builder);
	}

	#endregion

	#region Events

	private void OnThemeChanged(object? sender, ThemeChangedEventArgs<Models.Theme> e) {
		if (Game1.activeClickableMenu is Menus.AlmanacMenu menu)
			menu.RefreshTheme();
	}

	[Subscriber]
	private void OnButton(object? sender, ButtonPressedEventArgs e) {
		if (!Context.IsWorldReady)
			return;

		if (Game1.activeClickableMenu != null)
			return;

#if DEBUG
		if (e.Button == SButton.F8) {
			GMCMIntegration?.OpenMenu();
			return;
		}
#endif

		if (!(Config.UseKey?.JustPressed() ?? false))
			return;

		Helper.Input.SuppressActiveKeybinds(Config.UseKey);

		// If the player hasn't seen the event where they receive the Almanac, don't
		// let them use it unless it's always available.
		if (! HasAlmanac(Game1.player) )
			return;

		if (Game1.activeClickableMenu != null || Game1.CurrentEvent != null)
			return;

		Game1.activeClickableMenu = new Menus.AlmanacMenu(this, Game1.Date.Year);
	}


	// We mark this event as high priority so we can change tomorrow's weather
	// before other mods might rely on it.

	[Subscriber]
	[EventPriority(EventPriority.High)]
	private void OnDayStarted(object? sender, DayStartedEventArgs e) {
		ulong seed = GetBaseWorldSeed();

		if (Config.EnableDeterministicLuck && Game1.IsMasterGame) {
			Game1.player.team.sharedDailyLuck.Value = Luck.GetLuckForDate(seed, Game1.Date);
		}

		if (Config.EnableDeterministicWeather) {
			WorldDate tomorrow = new(Game1.Date);
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

	public void Invalidate() {
		Assets.Invalidate();
		Notices.Invalidate();
		Crops.Invalidate();
		Fish.Invalidate();
		Weather.Invalidate();
	}

	[Subscriber]
	private void OnGameLaunched(object? sender, GameLaunchedEventArgs e) {
		// Integrations
		intCP = new(this);
		intLS = new(this);
		intJA = new(this);
		intMGC = new(this);

		// More Init
		RegisterConfig();

		// Commands
		Helper.ConsoleCommands.Add("al_books", "List all available books.", (name, args) => {
			Books.Invalidate();
			foreach (var book in Books.GetAllBooks())
				Log($"Book: {book.Id}");
		});

		Helper.ConsoleCommands.Add("ts", "Parse a tokenstring.", (name, args) => {
			string input = string.Join(' ', args);

			Log($" Input: {input}");
			Log($"Result: {StringTokenizer.ParseString(input, item: Game1.player?.CurrentItem, monitor: Monitor, trace: true)}");
		});

		Helper.ConsoleCommands.Add("gsq", "Run a GameStateQuery", (name, args) => {
			int seed = -1;
			try {
				seed = int.Parse(args[0]);
			} catch { }

			string query;
			if (seed == -1)
				query = string.Join(' ', args);
			else
				query = string.Join(' ', args[1..]);

			Random rnd = seed == -1 ? Game1.random : new Random(seed);

			Log($" Query: {query}");
			if (seed != -1)
				Log($"  Seed: {seed}");
			Log($"Result: {GameStateQuery.CheckConditions(query, rnd: rnd, item: Game1.player.CurrentItem, monitor: Monitor, trace: true)}");
		});

		Helper.ConsoleCommands.Add("al_update", "Invalidate cached data.", (name, args) => {
			Invalidate();
			if (Game1.activeClickableMenu is Menus.AlmanacMenu almanac)
				almanac.RefreshPages();
		});

		Helper.ConsoleCommands.Add("al_debug", "Print some debugging information.", (name, args) => {
			Log($"Location: {Game1.currentLocation.Name}", LogLevel.Info);
			Log($"Is Farm: {Game1.currentLocation is Farm}", LogLevel.Info);
			if (Game1.currentLocation is Farm farm) {
				Log($"  - Which: {Game1.whichFarm}", LogLevel.Info);
				Log($"  - Fish Override: {farm.getMapProperty("FarmFishLocationOverride")}", LogLevel.Info);
			}
			Log($"  - Fish Sample: {Game1.currentLocation.getFish(0f, 0, 4, Game1.player, 0, Microsoft.Xna.Framework.Vector2.Zero).Name}", LogLevel.Info);
		});

		Helper.ConsoleCommands.Add("al_retheme", "Reload all themes.", ThemeManager.PerformReloadCommand);

		Helper.ConsoleCommands.Add("al_theme", "List all themes, or switch to a new theme.", (name, args) => {
			if (ThemeManager.PerformThemeCommand(args)) {
				Config.Theme = ThemeManager.SelectedThemeId!;
				SaveConfig();
			}
		});

		Helper.ConsoleCommands.Add("al_forecast", "Get the forecast for the loaded save.", (name, args) => {
			ulong seed = GetBaseWorldSeed();
			WorldDate date = new(Game1.Date);
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

		// Did we close the Almanac menu after opening it?
		if (ShouldCloseGMCM) {
			if (menu != null) {
				string name = menu.GetType().FullName ?? menu.GetType().Name;

				// If we're on the specific page for a mod, then
				// everything is fine and we can continue.
				if (name.Equals("GenericModConfigMenu.Framework.SpecificModConfigMenu"))
					return;

				// If we're on the all mods menu, then we don't
				// want this. YEET.
				if (name.Equals("GenericModConfigMenu.Framework.ModConfigMenu")) {
					CommonHelper.YeetMenu(menu);
					Game1.activeClickableMenu = null;
					CurrentMenu.Value = null;
					ShouldCloseGMCM = false;
					return;
				}
			}

			ShouldCloseGMCM = false;
		}

		// TODO: Add our pages to shop menus, if we have pages that are
		// to be added to shops.

		if (!HasAlmanac(Game1.player) || Config.AlmanacButtonPos == ButtonPosition.Disabled)
			return;

		InventoryPage? page = null;

		if (menu is InventoryPage)
			page = menu as InventoryPage;
		else if (menu is GameMenu gm) {
			for(int i = 0; i < gm.pages.Count; i++) {
				if (gm.pages[i] is InventoryPage)
					page = gm.pages[i] as InventoryPage;
			}
		}

		if (page != null)
			CurrentOverlay.Value = new Overlays.InventoryOverlay(page);
	}

	[Subscriber]
	private void OnSaveLoaded(object sender, SaveLoadedEventArgs e) {
		// Load Data
		LoadHeads();

		// Detect Season Length
		WorldDate day = new(1, "summer", 1);
		day.TotalDays--;
		DaysPerMonth = day.DayOfMonth;

		if (DaysPerMonth != WorldDate.DaysPerMonth)
			Log($"Using Non-Standard Days Per Month: {DaysPerMonth}");

		// Apply Player Flags
		UpdateFlags();
	}

	#endregion

	#region Configuration

	public void SaveConfig() {
		Helper.WriteConfig(Config);
		UpdateFlags();
	}

	public void ResetConfig() {
		Config = new();
	}

	[MemberNotNullWhen(true, nameof(GMCMIntegration))]
	public bool HasGMCM() {
		return GMCMIntegration?.IsLoaded ?? false;
	}

	public void OpenGMCM() {
		if (HasGMCM()) {
			ShouldCloseGMCM = true;
			GMCMIntegration.OpenMenu();
		}
	}

	private void RegisterConfig() {
		GMCMIntegration = new(this, () => Config, ResetConfig, SaveConfig);
		if (!GMCMIntegration.IsLoaded)
			return;

		GMCMIntegration.Register(true);

		GMCMIntegration
			.AddChoice(
				I18n.Settings_Button,
				I18n.Settings_ButtonDesc,
				c => c.AlmanacButtonPos,
				(c, v) => c.AlmanacButtonPos = v,
				new Dictionary<ButtonPosition, Func<string>> {
					[ButtonPosition.Disabled] = I18n.Settings_Button_Disabled,
					[ButtonPosition.TopLeft] = I18n.Settings_Button_LeftTop,
					[ButtonPosition.BottomLeft] = I18n.Settings_Button_LeftBottom,
					[ButtonPosition.OrganizeRight] = I18n.Settings_Button_OrganizeRight,
					[ButtonPosition.TrashRight] = I18n.Settings_Button_TrashRight,
					[ButtonPosition.TrashDown] = I18n.Settings_Button_TrashDown
				}
			)
			.Add(
				I18n.Settings_RestoreState,
				I18n.Settings_RestoreState_Desc,
				c => c.RestoreAlmanacState,
				(c, v) => c.RestoreAlmanacState = v
			)
			.Add(
				I18n.Settings_CycleTime,
				I18n.Settings_CycleTime_Desc,
				c => (float) c.CycleTime / 1000f,
				(c, v) => c.CycleTime = (int) Math.Floor(v * 1000),
				min: 0.25f,
				max: 10f,
				interval: 0.25f
			)
			.AddChoice(
				I18n.Settings_Theme,
				I18n.Settings_ThemeDesc,
				c => c.Theme,
				(c, v) => {
					c.Theme = v;
					ThemeManager.SelectTheme(v);
				},
				ThemeManager.GetThemeChoiceMethods()
			)
			.AddLabel("") // Spacer
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
			.AddLabel(I18n.Settings_Fish, null, "page:fishing")
			.AddLabel(I18n.Settings_Notices, null, "page:notices")
			.AddLabel(I18n.Settings_Fortune, null, "page:fortune")
			.AddLabel(I18n.Settings_Mines, null, "page:mines");

		GMCMIntegration
			.AddLabel("")
			.Add(
				I18n.Settings_Debug,
				I18n.Settings_Debug_Desc,
				c => c.DebugMode,
				(c, v) => c.DebugMode = v
			);

		GMCMIntegration
			.StartPage("bindings", I18n.Settings_Controls)
			.Add(
				I18n.Settings_Controls_Almanac,
				I18n.Settings_Controls_AlmanacDesc,
				c => c.UseKey,
				(c, v) => c.UseKey = v
			);

		GMCMIntegration
			.StartPage("page:crop", I18n.Settings_Crops)
			.Add(
				I18n.Settings_Enable,
				I18n.Settings_EnableDesc,
				c => c.ShowCrops,
				(c, v) => c.ShowCrops = v
			)

			.AddLabel("") // Spacer
			.AddLabel(I18n.Settings_Crops_Preview)
			.AddParagraph(I18n.Settings_Crops_PreviewDesc)

			.Add(
				I18n.Settings_Crops_Preview_Enable,
				null,
				c => c.ShowPreviews,
				(c, v) => c.ShowPreviews = v
			)
			.Add(
				I18n.Settings_Crops_Preview_Sprite,
				I18n.Settings_Crops_Preview_SpriteDesc,
				c => c.PreviewUseHarvestSprite,
				(c, v) => c.PreviewUseHarvestSprite = v
			)
			.Add(
				I18n.Settings_Crops_Preview_Plantonfirst,
				I18n.Settings_Crops_Preview_PlantonfirstDesc,
				c => c.PreviewPlantOnFirst,
				(c, v) => c.PreviewPlantOnFirst = v
			);

		GMCMIntegration
			.StartPage("page:weather", I18n.Settings_Weather)
			.Add(
				I18n.Settings_Enable,
				I18n.Settings_EnableDesc,
				c => c.ShowWeather,
				(c, v) => c.ShowWeather = v
			)

			.AddLabel("") // Spacer

			.Add(
				I18n.Settings_ForecastLength,
				I18n.Settings_ForecastLength_Desc,
				c => c.WeatherForecastLength,
				(c, v) => c.WeatherForecastLength = v,
				min: -1,
				max: WorldDate.MonthsPerYear * ModEntry.DaysPerMonth,
				format: val => {
					if (val == -1)
						return I18n.Settings_Unlimited();
					return I18n.Settings_Days(val);
				}
			)

			.AddLabel("") // Spacer

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

		GMCMIntegration
			.StartPage("page:train", I18n.Settings_Train)
			.Add(
				I18n.Settings_Enable,
				I18n.Settings_EnableDesc,
				c => c.ShowTrains,
				(c, v) => c.ShowTrains = v
			);

		GMCMIntegration
			.StartPage("page:fishing", I18n.Settings_Fish)
			.Add(
				I18n.Settings_Enable,
				I18n.Settings_EnableDesc,
				c => c.ShowFishing,
				(c, v) => c.ShowFishing = v
			)

			.AddLabel("") // Spacer

			.Add(
				I18n.Settings_Fish_Legendary,
				I18n.Settings_Fish_LegendaryDesc,
				c => c.FishShowLegendary,
				(c, v) => c.FishShowLegendary = v
			)

			.AddLabel("") // Spacer

			.Add(
				I18n.Settings_Fish_ShowTank,
				I18n.Settings_Fish_ShowTankDesc,
				c => c.ShowFishTank,
				(c, v) => c.ShowFishTank = v
			)
			.Add(
				I18n.Settings_Fish_DecorateTank,
				I18n.Settings_Fish_DecorateTankDesc,
				c => c.DecorateFishTank,
				(c, v) => c.DecorateFishTank = v
			);

		GMCMIntegration
			.StartPage("page:fortune", I18n.Settings_Fortune)
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
			)

			.AddLabel("") // Spacer

			.Add(
				I18n.Settings_ForecastLength,
				I18n.Settings_ForecastLength_Desc,
				c => c.LuckForecastLength,
				(c, v) => c.LuckForecastLength = v,
				min: -1,
				max: WorldDate.MonthsPerYear * ModEntry.DaysPerMonth,
				format: val => {
					if (val == -1)
						return I18n.Settings_Unlimited();
					return I18n.Settings_Days(val);
				}
			)

			.AddLabel("") // Spacer

			.SetTitleOnly(true)
			.Add(
				I18n.Settings_Fortune_Deterministic,
				I18n.Settings_Fortune_DeterministicDesc,
				c => c.EnableDeterministicLuck,
				(c, v) => c.EnableDeterministicLuck = v
			)
			.SetTitleOnly(false);

		GMCMIntegration
			.StartPage("page:mines", I18n.Settings_Mines)
			.Add(
				I18n.Settings_Enable,
				I18n.Settings_EnableDesc,
				c => c.ShowMines,
				(c, v) => c.ShowMines = v
			);

		GMCMIntegration
			.StartPage("page:notices", I18n.Settings_Notices)
			.Add(
				I18n.Settings_Enable,
				I18n.Settings_EnableDesc,
				c => c.ShowNotices,
				(c, v) => c.ShowNotices = v
			)

			.AddLabel("") // Spacer

			.Add(
				I18n.Settings_Notices_Anniversaries,
				I18n.Settings_Notices_AnniversariesDesc,
				c => c.NoticesShowAnniversaries,
				(c, v) => c.NoticesShowAnniversaries = v
			)
			.Add(
				I18n.Settings_Notices_Festivals,
				I18n.Settings_Notices_FestivalsDesc,
				c => c.NoticesShowFestivals,
				(c, v) => c.NoticesShowFestivals = v
			)
			.Add(
				I18n.Settings_Notices_Gathering,
				I18n.Settings_Notices_GatheringDesc,
				c => c.NoticesShowGathering,
				(c, v) => c.NoticesShowGathering = v
			)
			.AddChoice(
				I18n.Settings_Notices_Merchant,
				I18n.Settings_Notices_MerchantDesc,
				c => c.NoticesShowMerchant,
				(c, v) => c.NoticesShowMerchant = v,
				new Dictionary<MerchantMode, Func<string>> {
					[MerchantMode.Disabled] = I18n.Settings_Notices_Merchant_Disabled,
					[MerchantMode.Visit] = I18n.Settings_Notices_Merchant_Visit,
					[MerchantMode.Stock] = I18n.Settings_Notices_Merchant_Stock
				}
			)
			.Add(
				I18n.Settings_Notices_Trains,
				I18n.Settings_Notices_TrainsDesc,
				c => c.NoticesShowTrains,
				(c, v) => c.NoticesShowTrains = v
			);
	}

	#endregion

	#region Flags and Access

	public void UpdateFlags() {
		foreach (var who in Game1.getOnlineFarmers()) {
			// Try checking which players are running on the local machine,
			// and update their mail flags.
			bool local = who.IsLocalPlayer;
			if ( ! local ) {
				var wm = Helper.Multiplayer.GetConnectedPlayer(who.UniqueMultiplayerID);
				if (wm != null)
					local = wm.IsSplitScreen;
			}

			if (local)
				UpdateFlags(who);
		}
	}

	private static void ToggleMail(Farmer who, string key, bool has) {
		if (has)
			AddMail(who, key);
		else
			RemoveMail(who, key);
	}

	private static void AddMail(Farmer who, string key) {
		if (!who.mailReceived.Contains(key))
			who.mailReceived.Add(key);
	}

	private static void RemoveMail(Farmer who, string key) {
		if (who.mailReceived.Contains(key))
			who.mailReceived.Remove(key);
	}

	public void UpdateFlags(Farmer who) {
		bool seen_base = who.mailReceived.Contains(Mail_Seen_Base);
		bool seen_magic = who.mailReceived.Contains(Mail_Seen_Magic);
		bool seen_island = who.mailReceived.Contains(Mail_Seen_Island);

		ToggleMail(who, Mail_Has_Base, Config.AlmanacAlwaysAvailable || seen_base);
		ToggleMail(who, Mail_Has_Magic, Config.MagicAlwaysAvailable || seen_magic);
		ToggleMail(who, Mail_Has_Island, Config.IslandAlwaysAvailable || seen_island);
	}

	public bool HasAlmanac(Farmer who) {
		return Config.AlmanacAlwaysAvailable || who.mailReceived.Contains(Mail_Has_Base);
	}

	public bool HasIsland(Farmer who) {
		return Config.IslandAlwaysAvailable || who.mailReceived.Contains(Mail_Has_Island);
	}

	public bool HasMagic(Farmer who) {
		return Config.MagicAlwaysAvailable || who.mailReceived.Contains(Mail_Has_Magic);
	}

	#endregion

	public ulong GetBaseWorldSeed() {
		// TODO: Save custom seeds on a per-save basis?
		return Config.CustomSeed ?? Game1.uniqueIDForThisGame;
	}

	public bool DoesTranslationExist(string key) {
		return Helper.Translation.ContainsKey(key);
	}

	public string GetSubLocationName(Models.SubLocation sub) {
		if (sub.Area == -1)
			return I18n.Location_SubAny();

		string name = sub.Key;
		string key = $"location.{name}.{sub.Area}";
		if (DoesTranslationExist(key))
			return Helper.Translation.Get(key).ToString();

		switch(name) {
			case "UndergroundMine":
				return I18n.Location_SubFloor(sub.Area);

			case "Forest":
				if (sub.Area == 0)
					return I18n.Location_Forest_River();
				if (sub.Area == 1)
					return I18n.Location_Forest_Pond();
				break;

			case "IslandWest":
				if (sub.Area == 1)
					return I18n.Location_Island_Ocean();
				if (sub.Area == 2)
					return I18n.Location_Island_Freshwater();
				break;
		}

		return sub.Area.ToString();
	}

	public string? GetLocationName(GameLocation? location) {
		return GetLocationName(location?.Name, location);
	}

	[return: NotNullIfNotNull("name")]
	public string? GetLocationName(string? name, GameLocation? location) {
		if (string.IsNullOrEmpty(name))
			return null;

		string? key = $"location.{name}";
		if (DoesTranslationExist(key))
			return Helper.Translation.Get(key).ToString();

		key = null;

		if (name.StartsWith("UndergroundMine") || name == "Mine") {
			if (location is MineShaft shaft && shaft.mineLevel > 120 && shaft.mineLevel != 77377)
				key = "Strings\\StringsFromCSFiles:MapPage.cs.11062";
			else
				key = "Strings\\StringsFromCSFiles:MapPage.cs.11098";

		} else if (location is IslandLocation)
			key = "Strings\\StringsFromCSFiles:IslandName";

		else {
			switch(name) {
				case "AdventureGuild":
					key = "Strings\\StringsFromCSFiles:MapPage.cs.11099";
					break;
				case "AnimalShop":
					key = "Strings\\StringsFromCSFiles:MapPage.cs.11068";
					break;
				case "ArchaeologyHouse":
					key = "Strings\\StringsFromCSFiles:MapPage.cs.11086";
					break;
				case "BathHouse_Entry":
				case "BathHouse_MensLocker":
				case "BathHouse_Pool":
				case "BathHouse_WomensLocker":
					key = "Strings\\StringsFromCSFiles:MapPage.cs.11110";
					break;
				case "Club":
				case "Desert":
				case "SandyHouse":
				case "SandyShop":
				case "SkullCave":
					key = "Strings\\StringsFromCSFiles:MapPage.cs.11062";
					break;
				case "CommunityCenter":
					key = "Strings\\StringsFromCSFiles:MapPage.cs.11117";
					break;
				case "ElliottHouse":
					key = "Strings\\StringsFromCSFiles:MapPage.cs.11088";
					break;
				case "FishShop":
					key = "Strings\\StringsFromCSFiles:MapPage.cs.11107";
					break;
				case "HarveyRoom":
				case "Hospital":
					key = "Strings\\StringsFromCSFiles:MapPage.cs.11076";
					break;
				case "JoshHouse":
					key = "Strings\\StringsFromCSFiles:MapPage.cs.11092";
					break;
				case "ManorHouse":
					key = "Strings\\StringsFromCSFiles:MapPage.cs.11085";
					break;
				case "Railroad":
				case "WitchWarpCave":
					key = "Strings\\StringsFromCSFiles:MapPage.cs.11119";
					break;
				case "ScienceHouse":
				case "SebastianRoom":
					key = "Strings\\StringsFromCSFiles:MapPage.cs.11094";
					break;
				case "SeedShop":
					key = "Strings\\StringsFromCSFiles:MapPage.cs.11078";
					break;
				case "Temp":
					if (location?.Map.Id.Contains("Town") ?? false)
						key = "Strings\\StringsFromCSFiles:MapPage.cs.11190";
					break;
				case "Trailer_Big":
					key = "Strings\\StringsFromCSFiles:MapPage.PamHouse";
					break;
				case "WizardHouse":
				case "WizardHouseBasement":
					key = "Strings\\StringsFromCSFiles:MapPage.cs.11067";
					break;
				case "Woods":
					key = "Strings\\StringsFromCSFiles:MapPage.cs.11114";
					break;

				case "Backwoods":
				case "Tunnel":
					key = "Strings\\StringsFromCSFiles:MapPage.cs.11180";
					break;
				case "Barn":
				case "Big Barn":
				case "Big Coop":
				case "Big Shed":
				case "Cabin":
				case "Coop":
				case "Deluxe Barn":
				case "Deluxe Coop":
				case "Farm":
				case "FarmCave":
				case "FarmHouse":
				case "Greenhouse":
				case "Shed":
				case "Slime Hutch":
					return Game1.content.LoadString(
						"Strings\\StringsFromCSFiles:MapPage.cs.11064",
						Game1.player.farmName.Value
					);
				case "Beach":
					key = "Strings\\StringsFromCSFiles:MapPage.cs.11174";
					break;
				case "Forest":
					key = "Strings\\StringsFromCSFiles:MapPage.cs.11186";
					break;
				case "Mountain":
					key = "Strings\\StringsFromCSFiles:MapPage.cs.11176";
					break;
				case "Saloon":
					key = "Strings\\StringsFromCSFiles:MapPage.cs.11172";
					break;
				case "Town":
					key = "Strings\\StringsFromCSFiles:MapPage.cs.11190";
					break;
				case "Sewer":
					key = @"Strings\StringsFromCSFiles:MapPage.cs.11089";
					break;
			}
		}

		if (key != null)
			return Game1.content.LoadString(key);

		Monitor.LogOnce($"Unable to locate translation key for GameLocation: {name}", LogLevel.Debug);

		if (Config.DebugMode)
			return $"(no-i18n: {name})";

		return name;
	}

}
