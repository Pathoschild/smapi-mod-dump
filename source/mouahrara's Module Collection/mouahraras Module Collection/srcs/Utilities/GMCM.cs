/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/mouahrarasModuleCollection
**
*************************************************/

using StardewModdingAPI;

namespace mouahrarasModuleCollection.Utilities
{
	public sealed class ModConfig
	{
		public bool	ArcadeGamesPayToPlay = true;
		public int	ArcadeGamesPayToPlayCoinPerJotPKGame = 1;
		public int	ArcadeGamesPayToPlayCoinPerJKGame = 1;
		public bool	ArcadeGamesPayToPlayKonamiCode = true;
		public bool	ArcadeGamesPayToPlayNonRealisticLeaderboard = true;
		public bool	ClintsShopSimultaneousServices = true;
		public bool	ClintsShopGeodesAutoProcess = true;
		public int ClintsShopGeodesAutoProcessSpeedMultiplier = 2;
		public bool CrystalariumsSafeReplacement = true;
		public bool	FestivalsEndTime = true;
		public int	FestivalsEndTimeAdditionalTime = 200;
		public bool	FarmViewZoom = true;
		public SButton FarmViewZoomInKey = SButton.RightThumbstickUp;
		public SButton FarmViewZoomOutKey = SButton.RightThumbstickDown;
		public float FarmViewZoomMultiplier = 1.0f;
		public bool	FarmViewFastScrolling = true;
		public float FarmViewFastScrollingMultiplier = 3.0f;
		public bool	MarniesShopAnimalPurchase = true;
	}

	internal class GMCMUtility
	{
		internal static void Initialize()
		{
			ReadConfig();
			Register();
		}

		private static void ReadConfig()
		{
			ModEntry.Config = ModEntry.Helper.ReadConfig<ModConfig>();
		}

		private static void Register()
		{
			// Get Generic Mod Config Menu's API
			GenericModConfigMenu.IGenericModConfigMenuApi gmcm = ModEntry.Helper.ModRegistry.GetApi<GenericModConfigMenu.IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
			if (gmcm is null)
				return;

			// Register mod
			gmcm.Register(
				mod: ModEntry.ModManifest,
				reset: () => ModEntry.Config = new ModConfig(),
				save: () => ModEntry.Helper.WriteConfig(ModEntry.Config)
			);

			// Main
			gmcm.AddSectionTitle(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.Sections.Tweaks&Features.Title")
			);
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.Sections.Tweaks&Features.Description")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Arcade games",
				text: () => "> " + ModEntry.Helper.Translation.Get("GMCM.Modules.ArcadeGames.Title")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Clint's shop",
				text: () => "> " + ModEntry.Helper.Translation.Get("GMCM.Modules.ClintsShop.Title")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Crystalariums",
				text: () => "> " + ModEntry.Helper.Translation.Get("GMCM.Modules.Crystalariums.Title")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Farm view",
				text: () => "> " + ModEntry.Helper.Translation.Get("GMCM.Modules.FarmView.Title")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Festivals",
				text: () => "> " + ModEntry.Helper.Translation.Get("GMCM.Modules.Festivals.Title")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Marnie's shop",
				text: () => "> " + ModEntry.Helper.Translation.Get("GMCM.Modules.MarniesShop.Title")
			);
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => string.Empty
			);
			gmcm.AddSectionTitle(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.Sections.Additions.Title")
			);
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.Sections.Additions.Description")
			);
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => string.Empty
			);
			gmcm.AddSectionTitle(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.Sections.Overhauls.Title")
			);
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.Sections.Overhauls.Description")
			);


			// Arcade
			gmcm.AddPage(
				mod: ModEntry.ModManifest,
				pageId: "Arcade games",
				pageTitle: () => ModEntry.Helper.Translation.Get("GMCM.Modules.ArcadeGames.Title")
			);
			// gmcm.AddPageLink(
			// 	mod: ModEntry.ModManifest,
			// 	pageId: ModEntry.ModManifest.UniqueID,
			// 	text: () => "@ " + "Back to mouahrara's Module Collection"
			// );
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.Modules.ArcadeGames.Description")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Arcade games - Konami code",
				text: () => "> " + ModEntry.Helper.Translation.Get("GMCM.SubModules.KonamiCode.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.KonamiCode.Tooltip")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Arcade games - Non-realistic leaderboard",
				text: () => "> " + ModEntry.Helper.Translation.Get("GMCM.SubModules.NonRealisticLeaderboard.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.NonRealisticLeaderboard.Tooltip")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Arcade games - Pay-to-play",
				text: () => "> " + ModEntry.Helper.Translation.Get("GMCM.SubModules.PayToPlay.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.PayToPlay.Tooltip")
			);
			// Arcade games - Konami code
			gmcm.AddPage(
				mod: ModEntry.ModManifest,
				pageId: "Arcade games - Konami code",
				pageTitle: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.KonamiCode.Title")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Arcade games",
				text: () => "@ " + ModEntry.Helper.Translation.Get("GMCM.Modules.ArcadeGames.BackTo")
			);
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.KonamiCode.Description")
			);
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.KonamiCode.AdditionalInformation")
			);
			gmcm.AddBoolOption(
				mod: ModEntry.ModManifest,
				name: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.Generic.Enabled.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.Generic.Enabled.Tooltip"),
				getValue: () => ModEntry.Config.ArcadeGamesPayToPlayKonamiCode,
				setValue: value => ModEntry.Config.ArcadeGamesPayToPlayKonamiCode = value
			);
			// Arcade games - Non-realistic leaderboard
			gmcm.AddPage(
				mod: ModEntry.ModManifest,
				pageId: "Arcade games - Non-realistic leaderboard",
				pageTitle: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.NonRealisticLeaderboard.Title")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Arcade games",
				text: () => "@ " + ModEntry.Helper.Translation.Get("GMCM.Modules.ArcadeGames.BackTo")
			);
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.NonRealisticLeaderboard.Description")
			);
			gmcm.AddBoolOption(
				mod: ModEntry.ModManifest,
				name: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.Generic.Enabled.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.Generic.Enabled.Tooltip"),
				getValue: () => ModEntry.Config.ArcadeGamesPayToPlayNonRealisticLeaderboard,
				setValue: value => ModEntry.Config.ArcadeGamesPayToPlayNonRealisticLeaderboard = value
			);
			// Arcade games - Pay-to-play
			gmcm.AddPage(
				mod: ModEntry.ModManifest,
				pageId: "Arcade games - Pay-to-play",
				pageTitle: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.PayToPlay.Title")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Arcade games",
				text: () => "@ " + ModEntry.Helper.Translation.Get("GMCM.Modules.ArcadeGames.BackTo")
			);
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.PayToPlay.Description")
			);
			gmcm.AddBoolOption(
				mod: ModEntry.ModManifest,
				name: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.Generic.Enabled.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.Generic.Enabled.Tooltip"),
				getValue: () => ModEntry.Config.ArcadeGamesPayToPlay,
				setValue: value => ModEntry.Config.ArcadeGamesPayToPlay = value
			);
			gmcm.AddNumberOption(
				mod: ModEntry.ModManifest,
				name: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.PayToPlay.CoinPerJotPKGame.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.PayToPlay.CoinPerJotPKGame.Tooltip"),
				getValue: () => ModEntry.Config.ArcadeGamesPayToPlayCoinPerJotPKGame,
				setValue: value => ModEntry.Config.ArcadeGamesPayToPlayCoinPerJotPKGame = value,
				min: 1,
				max: 5,
				interval: 1
			);
			gmcm.AddNumberOption(
				mod: ModEntry.ModManifest,
				name: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.PayToPlay.CoinPerJKGame.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.PayToPlay.CoinPerJKGame.Tooltip"),
				getValue: () => ModEntry.Config.ArcadeGamesPayToPlayCoinPerJKGame,
				setValue: value => ModEntry.Config.ArcadeGamesPayToPlayCoinPerJKGame = value,
				min: 1,
				max: 5,
				interval: 1
			);

			// Clint's shop
			gmcm.AddPage(
				mod: ModEntry.ModManifest,
				pageId: "Clint's shop",
				pageTitle: () => ModEntry.Helper.Translation.Get("GMCM.Modules.ClintsShop.Title")
			);
			// gmcm.AddPageLink(
			// 	mod: ModEntry.ModManifest,
			// 	pageId: ModEntry.ModManifest.UniqueID,
			// 	text: () => "@ " + "Back to mouahrara's Module Collection"
			// );
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.Modules.ClintsShop.Description")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Clint's shop - Geodes auto-process",
				text: () => "> " + ModEntry.Helper.Translation.Get("GMCM.SubModules.GeodesAutoProcess.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.GeodesAutoProcess.Tooltip")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Clint's shop - Simultaneous services",
				text: () => "> " + ModEntry.Helper.Translation.Get("GMCM.SubModules.SimultaneousServices.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.SimultaneousServices.Tooltip")
			);
			// Clint's shop - Geodes auto-process
			gmcm.AddPage(
				mod: ModEntry.ModManifest,
				pageId: "Clint's shop - Geodes auto-process",
				pageTitle: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.GeodesAutoProcess.Title")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Clint's shop",
				text: () => "@ " + ModEntry.Helper.Translation.Get("GMCM.Modules.ClintsShop.BackTo")
			);
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.GeodesAutoProcess.Description")
			);
			gmcm.AddBoolOption(
				mod: ModEntry.ModManifest,
				name: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.Generic.Enabled.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.Generic.Enabled.Tooltip"),
				getValue: () => ModEntry.Config.ClintsShopGeodesAutoProcess,
				setValue: value => ModEntry.Config.ClintsShopGeodesAutoProcess = value
			);
			gmcm.AddNumberOption(
				mod: ModEntry.ModManifest,
				name: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.GeodesAutoProcess.SpeedMultiplier.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.GeodesAutoProcess.SpeedMultiplier.Tooltip"),
				getValue: () => ModEntry.Config.ClintsShopGeodesAutoProcessSpeedMultiplier,
				setValue: value => ModEntry.Config.ClintsShopGeodesAutoProcessSpeedMultiplier = value,
				min: 1,
				max: 20,
				interval: 1
			);
			// Clint's shop - Simultaneous services
			gmcm.AddPage(
				mod: ModEntry.ModManifest,
				pageId: "Clint's shop - Simultaneous services",
				pageTitle: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.SimultaneousServices.Title")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Clint's shop",
				text: () => "@ " + ModEntry.Helper.Translation.Get("GMCM.Modules.ClintsShop.BackTo")
			);
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.SimultaneousServices.Description")
			);
			gmcm.AddBoolOption(
				mod: ModEntry.ModManifest,
				name: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.Generic.Enabled.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.Generic.Enabled.Tooltip"),
				getValue: () => ModEntry.Config.ClintsShopSimultaneousServices,
				setValue: value => ModEntry.Config.ClintsShopSimultaneousServices = value
			);

			// Crystalariums
			gmcm.AddPage(
				mod: ModEntry.ModManifest,
				pageId: "Crystalariums",
				pageTitle: () => ModEntry.Helper.Translation.Get("GMCM.Modules.Crystalariums.Title")
			);
			// gmcm.AddPageLink(
			// 	mod: ModEntry.ModManifest,
			// 	pageId: ModEntry.ModManifest.UniqueID,
			// 	text: () => "@ " + "Back to mouahrara's Module Collection"
			// );
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.Modules.Crystalariums.Description")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Crystalariums - Safe replacement",
				text: () => "> " + ModEntry.Helper.Translation.Get("GMCM.SubModules.SafeReplacement.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.SafeReplacement.Tooltip")
			);
			// Crystalariums - Safe replacement
			gmcm.AddPage(
				mod: ModEntry.ModManifest,
				pageId: "Crystalariums - Safe replacement",
				pageTitle: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.SafeReplacement.Title")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Crystalariums",
				text: () => "@ " + ModEntry.Helper.Translation.Get("GMCM.Modules.Crystalariums.BackTo")
			);
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.SafeReplacement.Description")
			);
			gmcm.AddBoolOption(
				mod: ModEntry.ModManifest,
				name: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.Generic.Enabled.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.Generic.Enabled.Tooltip"),
				getValue: () => ModEntry.Config.CrystalariumsSafeReplacement,
				setValue: value => ModEntry.Config.CrystalariumsSafeReplacement = value
			);

			// Farm view
			gmcm.AddPage(
				mod: ModEntry.ModManifest,
				pageId: "Farm view",
				pageTitle: () => ModEntry.Helper.Translation.Get("GMCM.Modules.FarmView.Title")
			);
			// gmcm.AddPageLink(
			// 	mod: ModEntry.ModManifest,
			// 	pageId: ModEntry.ModManifest.UniqueID,
			// 	text: () => "@ " + "Back to mouahrara's Module Collection"
			// );
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.Modules.FarmView.Description")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Farm view - Fast scrolling",
				text: () => "> " + ModEntry.Helper.Translation.Get("GMCM.SubModules.FastScrolling.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.FastScrolling.Tooltip")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Farm view - Zoom",
				text: () => "> " + ModEntry.Helper.Translation.Get("GMCM.SubModules.Zoom.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.Zoom.Tooltip")
			);
			// Farm view - Fast scrolling
			gmcm.AddPage(
				mod: ModEntry.ModManifest,
				pageId: "Farm view - Fast scrolling",
				pageTitle: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.FastScrolling.Title")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Farm view",
				text: () => "@ " + ModEntry.Helper.Translation.Get("GMCM.Modules.FarmView.BackTo")
			);
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.FastScrolling.Description")
			);
			gmcm.AddBoolOption(
				mod: ModEntry.ModManifest,
				name: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.Generic.Enabled.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.Generic.Enabled.Tooltip"),
				getValue: () => ModEntry.Config.FarmViewFastScrolling,
				setValue: value => ModEntry.Config.FarmViewFastScrolling = value
			);
			gmcm.AddNumberOption(
				mod: ModEntry.ModManifest,
				name: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.FastScrolling.SpeedMultiplier.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.FastScrolling.SpeedMultiplier.Tooltip"),
				getValue: () => ModEntry.Config.FarmViewFastScrollingMultiplier,
				setValue: value => ModEntry.Config.FarmViewFastScrollingMultiplier = value,
				min: 1.0f,
				max: 8.0f,
				interval: 0.25f
			);
			// Farm view - Zoom
			gmcm.AddPage(
				mod: ModEntry.ModManifest,
				pageId: "Farm view - Zoom",
				pageTitle: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.Zoom.Title")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Farm view",
				text: () => "@ " + ModEntry.Helper.Translation.Get("GMCM.Modules.FarmView.BackTo")
			);
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.Zoom.Description")
			);
			gmcm.AddBoolOption(
				mod: ModEntry.ModManifest,
				name: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.Generic.Enabled.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.Generic.Enabled.Tooltip"),
				getValue: () => ModEntry.Config.FarmViewZoom,
				setValue: value => ModEntry.Config.FarmViewZoom = value
			);
			gmcm.AddKeybind(
				mod: ModEntry.ModManifest,
				name: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.Zoom.SecondaryZoomInKey.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.Zoom.SecondaryZoomInKey.Tooltip"),
				getValue: () => ModEntry.Config.FarmViewZoomInKey,
				setValue: value => ModEntry.Config.FarmViewZoomInKey = value
			);
			gmcm.AddKeybind(
				mod: ModEntry.ModManifest,
				name: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.Zoom.SecondaryZoomOutKey.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.Zoom.SecondaryZoomOutKey.Tooltip"),
				getValue: () => ModEntry.Config.FarmViewZoomOutKey,
				setValue: value => ModEntry.Config.FarmViewZoomOutKey = value
			);
			gmcm.AddNumberOption(
				mod: ModEntry.ModManifest,
				name: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.Zoom.SpeedMultiplier.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.Zoom.SpeedMultiplier.Tooltip"),
				getValue: () => ModEntry.Config.FarmViewZoomMultiplier,
				setValue: value => ModEntry.Config.FarmViewZoomMultiplier = value,
				min: 0.25f,
				max: 4.0f,
				interval: 0.25f
			);

			// Festivals
			gmcm.AddPage(
				mod: ModEntry.ModManifest,
				pageId: "Festivals",
				pageTitle: () => ModEntry.Helper.Translation.Get("GMCM.Modules.Festivals.Title")
			);
			// gmcm.AddPageLink(
			// 	mod: ModEntry.ModManifest,
			// 	pageId: ModEntry.ModManifest.UniqueID,
			// 	text: () => "@ " + "Back to mouahrara's Module Collection"
			// );
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.Modules.Festivals.Description")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Festivals - End time",
				text: () => "> " + ModEntry.Helper.Translation.Get("GMCM.SubModules.EndTime.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.EndTime.Tooltip")
			);
			// Festivals - End time
			gmcm.AddPage(
				mod: ModEntry.ModManifest,
				pageId: "Festivals - End time",
				pageTitle: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.EndTime.Title")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Festivals",
				text: () => "@ " + ModEntry.Helper.Translation.Get("GMCM.Modules.Festivals.BackTo")
			);
			// gmcm.AddParagraph(
			// 	mod: ModEntry.ModManifest,
			// 	text: () => "This sub-module allows you to get back home at the time the festival is supposed to end. The festival end time is determined by the minimum between 1am and the sum of the arrival end time and the duration of the festival.\nThe formula is: min(1am, ArrivalEndTime + FestivalDuration).\n\nFor example, the arrival end time of the Egg Festival is 2pm. Considering the default duration of 2 hours, the festival end time is 4pm."
			// );
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.EndTime.Description")
			);
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.EndTime.AdditionalInformation")
			);
			gmcm.AddBoolOption(
				mod: ModEntry.ModManifest,
				name: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.Generic.Enabled.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.Generic.Enabled.Tooltip"),
				getValue: () => ModEntry.Config.FestivalsEndTime,
				setValue: value => ModEntry.Config.FestivalsEndTime = value
			);
			gmcm.AddNumberOption(
				mod: ModEntry.ModManifest,
				name: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.EndTime.AdditionalTime.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.EndTime.AdditionalTime.Tooltip"),
				getValue: () => ModEntry.Config.FestivalsEndTimeAdditionalTime / 100,
				setValue: value => ModEntry.Config.FestivalsEndTimeAdditionalTime = value * 100,
				min: 0,
				max: 8,
				interval: 1
			);

			// Marnie's shop
			gmcm.AddPage(
				mod: ModEntry.ModManifest,
				pageId: "Marnie's shop",
				pageTitle: () => ModEntry.Helper.Translation.Get("GMCM.Modules.MarniesShop.Title")
			);
			// gmcm.AddPageLink(
			// 	mod: ModEntry.ModManifest,
			// 	pageId: ModEntry.ModManifest.UniqueID,
			// 	text: () => "@ " + "Back to mouahrara's Module Collection"
			// );
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.Modules.MarniesShop.Description")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Marnie's shop - Animal purchase",
				text: () => "> " + ModEntry.Helper.Translation.Get("GMCM.SubModules.AnimalPurchase.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.AnimalPurchase.Tooltip")
			);
			// Marnie's shop - Animal purchase
			gmcm.AddPage(
				mod: ModEntry.ModManifest,
				pageId: "Marnie's shop - Animal purchase",
				pageTitle: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.AnimalPurchase.Title")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Marnie's shop",
				text: () => "@ " + ModEntry.Helper.Translation.Get("GMCM.Modules.MarniesShop.BackTo")
			);
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.AnimalPurchase.Description")
			);
			gmcm.AddBoolOption(
				mod: ModEntry.ModManifest,
				name: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.Generic.Enabled.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.SubModules.Generic.Enabled.Tooltip"),
				getValue: () => ModEntry.Config.MarniesShopAnimalPurchase,
				setValue: value => ModEntry.Config.MarniesShopAnimalPurchase = value
			);
		}
	}
}
