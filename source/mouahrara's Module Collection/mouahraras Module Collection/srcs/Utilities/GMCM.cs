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
		public bool	ShopsBetterAnimalPurchase = true;
		public SButton ShopsBetterAnimalPurchasePreviousKey = SButton.LeftShoulder;
		public SButton ShopsBetterAnimalPurchaseNextKey = SButton.RightShoulder;
		public bool	ShopsGeodesAutoProcess = true;
		public int ShopsGeodesAutoProcessSpeedMultiplier = 2;
		public bool MachinesSafeReplacement = true;
		public bool	OtherFestivalEndTime = true;
		public int	OtherFestivalEndTimeAdditionalTime = 200;
		public bool	UserInterfaceZoom = true;
		public SButton UserInterfaceZoomInKey = SButton.RightTrigger;
		public SButton UserInterfaceZoomOutKey = SButton.LeftTrigger;
		public float UserInterfaceZoomMultiplier = 1.0f;
		public bool	UserInterfaceFastScrolling = true;
		public float UserInterfaceFastScrollingMultiplier = 3.0f;
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
				text: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.Title")
			);
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.Description")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Arcade games",
				text: () => "> " + ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.ArcadeGames.Title")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Machines",
				text: () => "> " + ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.Machines.Title")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Shops",
				text: () => "> " + ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.Shops.Title")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "User Interface",
				text: () => "> " + ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.UserInterface.Title")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Other",
				text: () => "> " + ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.Other.Title")
			);
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => string.Empty
			);
			gmcm.AddSectionTitle(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.Additions.Title")
			);
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.Additions.Description")
			);
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => string.Empty
			);
			gmcm.AddSectionTitle(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.Overhauls.Title")
			);
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.Overhauls.Description")
			);

			// Arcade games
			gmcm.AddPage(
				mod: ModEntry.ModManifest,
				pageId: "Arcade games",
				pageTitle: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.ArcadeGames.Title")
			);
			// gmcm.AddPageLink(
			// 	mod: ModEntry.ModManifest,
			// 	pageId: ModEntry.ModManifest.UniqueID,
			// 	text: () => "@ " + "Back to mouahrara's Module Collection"
			// );
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.ArcadeGames.Description")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Arcade games - Konami code",
				text: () => "> " + ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.ArcadeGames.KonamiCode.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.ArcadeGames.KonamiCode.Tooltip")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Arcade games - Non-realistic leaderboard",
				text: () => "> " + ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.ArcadeGames.NonRealisticLeaderboard.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.ArcadeGames.NonRealisticLeaderboard.Tooltip")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Arcade games - Pay-to-play",
				text: () => "> " + ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.ArcadeGames.PayToPlay.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.ArcadeGames.PayToPlay.Tooltip")
			);
			// Arcade games - Konami code
			gmcm.AddPage(
				mod: ModEntry.ModManifest,
				pageId: "Arcade games - Konami code",
				pageTitle: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.ArcadeGames.KonamiCode.Title")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Arcade games",
				text: () => "@ " + ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.ArcadeGames.BackTo")
			);
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.ArcadeGames.KonamiCode.Description")
			);
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.ArcadeGames.KonamiCode.AdditionalInformation")
			);
			gmcm.AddBoolOption(
				mod: ModEntry.ModManifest,
				name: () => ModEntry.Helper.Translation.Get("GMCM.Modules.Enabled.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.Modules.Enabled.Tooltip"),
				getValue: () => ModEntry.Config.ArcadeGamesPayToPlayKonamiCode,
				setValue: value => ModEntry.Config.ArcadeGamesPayToPlayKonamiCode = value
			);
			// Arcade games - Non-realistic leaderboard
			gmcm.AddPage(
				mod: ModEntry.ModManifest,
				pageId: "Arcade games - Non-realistic leaderboard",
				pageTitle: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.ArcadeGames.NonRealisticLeaderboard.Title")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Arcade games",
				text: () => "@ " + ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.ArcadeGames.BackTo")
			);
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.ArcadeGames.NonRealisticLeaderboard.Description")
			);
			gmcm.AddBoolOption(
				mod: ModEntry.ModManifest,
				name: () => ModEntry.Helper.Translation.Get("GMCM.Modules.Enabled.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.Modules.Enabled.Tooltip"),
				getValue: () => ModEntry.Config.ArcadeGamesPayToPlayNonRealisticLeaderboard,
				setValue: value => ModEntry.Config.ArcadeGamesPayToPlayNonRealisticLeaderboard = value
			);
			// Arcade games - Pay-to-play
			gmcm.AddPage(
				mod: ModEntry.ModManifest,
				pageId: "Arcade games - Pay-to-play",
				pageTitle: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.ArcadeGames.PayToPlay.Title")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Arcade games",
				text: () => "@ " + ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.ArcadeGames.BackTo")
			);
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.ArcadeGames.PayToPlay.Description")
			);
			gmcm.AddBoolOption(
				mod: ModEntry.ModManifest,
				name: () => ModEntry.Helper.Translation.Get("GMCM.Modules.Enabled.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.Modules.Enabled.Tooltip"),
				getValue: () => ModEntry.Config.ArcadeGamesPayToPlay,
				setValue: value => ModEntry.Config.ArcadeGamesPayToPlay = value
			);
			gmcm.AddNumberOption(
				mod: ModEntry.ModManifest,
				name: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.ArcadeGames.PayToPlay.CoinPerJotPKGame.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.ArcadeGames.PayToPlay.CoinPerJotPKGame.Tooltip"),
				getValue: () => ModEntry.Config.ArcadeGamesPayToPlayCoinPerJotPKGame,
				setValue: value => ModEntry.Config.ArcadeGamesPayToPlayCoinPerJotPKGame = value,
				min: 1,
				max: 5,
				interval: 1
			);
			gmcm.AddNumberOption(
				mod: ModEntry.ModManifest,
				name: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.ArcadeGames.PayToPlay.CoinPerJKGame.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.ArcadeGames.PayToPlay.CoinPerJKGame.Tooltip"),
				getValue: () => ModEntry.Config.ArcadeGamesPayToPlayCoinPerJKGame,
				setValue: value => ModEntry.Config.ArcadeGamesPayToPlayCoinPerJKGame = value,
				min: 1,
				max: 5,
				interval: 1
			);

			// Shops
			gmcm.AddPage(
				mod: ModEntry.ModManifest,
				pageId: "Shops",
				pageTitle: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.Shops.Title")
			);
			// gmcm.AddPageLink(
			// 	mod: ModEntry.ModManifest,
			// 	pageId: ModEntry.ModManifest.UniqueID,
			// 	text: () => "@ " + "Back to mouahrara's Module Collection"
			// );
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.Shops.Description")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Shops - Better animal purchase",
				text: () => "> " + ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.Shops.BetterAnimalPurchase.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.Shops.BetterAnimalPurchase.Tooltip")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Shops - Geodes auto-process",
				text: () => "> " + ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.Shops.GeodesAutoProcess.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.Shops.GeodesAutoProcess.Tooltip")
			);
			// Shops - Better animal purchase
			gmcm.AddPage(
				mod: ModEntry.ModManifest,
				pageId: "Shops - Better animal purchase",
				pageTitle: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.Shops.BetterAnimalPurchase.Title")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Shops",
				text: () => "@ " + ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.Shops.BackTo")
			);
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.Shops.BetterAnimalPurchase.Description")
			);
			gmcm.AddBoolOption(
				mod: ModEntry.ModManifest,
				name: () => ModEntry.Helper.Translation.Get("GMCM.Modules.Enabled.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.Modules.Enabled.Tooltip"),
				getValue: () => ModEntry.Config.ShopsBetterAnimalPurchase,
				setValue: value => ModEntry.Config.ShopsBetterAnimalPurchase = value
			);
			gmcm.AddKeybind(
				mod: ModEntry.ModManifest,
				name: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.Shops.BetterAnimalPurchase.SecondaryPreviousKey.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.Shops.BetterAnimalPurchase.SecondaryPreviousKey.Tooltip"),
				getValue: () => ModEntry.Config.ShopsBetterAnimalPurchasePreviousKey,
				setValue: value => ModEntry.Config.ShopsBetterAnimalPurchasePreviousKey = value
			);
			gmcm.AddKeybind(
				mod: ModEntry.ModManifest,
				name: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.Shops.BetterAnimalPurchase.SecondaryNextKey.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.Shops.BetterAnimalPurchase.SecondaryNextKey.Tooltip"),
				getValue: () => ModEntry.Config.ShopsBetterAnimalPurchaseNextKey,
				setValue: value => ModEntry.Config.ShopsBetterAnimalPurchaseNextKey = value
			);
			// Shops - Geodes auto-process
			gmcm.AddPage(
				mod: ModEntry.ModManifest,
				pageId: "Shops - Geodes auto-process",
				pageTitle: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.Shops.GeodesAutoProcess.Title")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Shops",
				text: () => "@ " + ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.Shops.BackTo")
			);
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.Shops.GeodesAutoProcess.Description")
			);
			gmcm.AddBoolOption(
				mod: ModEntry.ModManifest,
				name: () => ModEntry.Helper.Translation.Get("GMCM.Modules.Enabled.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.Modules.Enabled.Tooltip"),
				getValue: () => ModEntry.Config.ShopsGeodesAutoProcess,
				setValue: value => ModEntry.Config.ShopsGeodesAutoProcess = value
			);
			gmcm.AddNumberOption(
				mod: ModEntry.ModManifest,
				name: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.Shops.GeodesAutoProcess.SpeedMultiplier.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.Shops.GeodesAutoProcess.SpeedMultiplier.Tooltip"),
				getValue: () => ModEntry.Config.ShopsGeodesAutoProcessSpeedMultiplier,
				setValue: value => ModEntry.Config.ShopsGeodesAutoProcessSpeedMultiplier = value,
				min: 1,
				max: 20,
				interval: 1
			);

			// Machines
			gmcm.AddPage(
				mod: ModEntry.ModManifest,
				pageId: "Machines",
				pageTitle: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.Machines.Title")
			);
			// gmcm.AddPageLink(
			// 	mod: ModEntry.ModManifest,
			// 	pageId: ModEntry.ModManifest.UniqueID,
			// 	text: () => "@ " + "Back to mouahrara's Module Collection"
			// );
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.Machines.Description")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Machines - Safe replacement",
				text: () => "> " + ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.Machines.SafeReplacement.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.Machines.SafeReplacement.Tooltip")
			);
			// Machines - Safe replacement
			gmcm.AddPage(
				mod: ModEntry.ModManifest,
				pageId: "Machines - Safe replacement",
				pageTitle: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.Machines.SafeReplacement.Title")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Machines",
				text: () => "@ " + ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.Machines.BackTo")
			);
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.Machines.SafeReplacement.Description")
			);
			gmcm.AddBoolOption(
				mod: ModEntry.ModManifest,
				name: () => ModEntry.Helper.Translation.Get("GMCM.Modules.Enabled.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.Modules.Enabled.Tooltip"),
				getValue: () => ModEntry.Config.MachinesSafeReplacement,
				setValue: value => ModEntry.Config.MachinesSafeReplacement = value
			);

			// User Interface
			gmcm.AddPage(
				mod: ModEntry.ModManifest,
				pageId: "User Interface",
				pageTitle: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.UserInterface.Title")
			);
			// gmcm.AddPageLink(
			// 	mod: ModEntry.ModManifest,
			// 	pageId: ModEntry.ModManifest.UniqueID,
			// 	text: () => "@ " + "Back to mouahrara's Module Collection"
			// );
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.UserInterface.Description")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "User Interface - Fast scrolling",
				text: () => "> " + ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.UserInterface.FastScrolling.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.UserInterface.FastScrolling.Tooltip")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "User Interface - Zoom",
				text: () => "> " + ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.UserInterface.Zoom.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.UserInterface.Zoom.Tooltip")
			);
			// User Interface - Fast scrolling
			gmcm.AddPage(
				mod: ModEntry.ModManifest,
				pageId: "User Interface - Fast scrolling",
				pageTitle: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.UserInterface.FastScrolling.Title")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "User Interface",
				text: () => "@ " + ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.UserInterface.BackTo")
			);
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.UserInterface.FastScrolling.Description")
			);
			gmcm.AddBoolOption(
				mod: ModEntry.ModManifest,
				name: () => ModEntry.Helper.Translation.Get("GMCM.Modules.Enabled.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.Modules.Enabled.Tooltip"),
				getValue: () => ModEntry.Config.UserInterfaceFastScrolling,
				setValue: value => ModEntry.Config.UserInterfaceFastScrolling = value
			);
			gmcm.AddNumberOption(
				mod: ModEntry.ModManifest,
				name: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.UserInterface.FastScrolling.SpeedMultiplier.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.UserInterface.FastScrolling.SpeedMultiplier.Tooltip"),
				getValue: () => ModEntry.Config.UserInterfaceFastScrollingMultiplier,
				setValue: value => ModEntry.Config.UserInterfaceFastScrollingMultiplier = value,
				min: 1.0f,
				max: 8.0f,
				interval: 0.25f
			);
			// User Interface - Zoom
			gmcm.AddPage(
				mod: ModEntry.ModManifest,
				pageId: "User Interface - Zoom",
				pageTitle: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.UserInterface.Zoom.Title")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "User Interface",
				text: () => "@ " + ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.UserInterface.BackTo")
			);
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.UserInterface.Zoom.Description")
			);
			gmcm.AddBoolOption(
				mod: ModEntry.ModManifest,
				name: () => ModEntry.Helper.Translation.Get("GMCM.Modules.Enabled.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.Modules.Enabled.Tooltip"),
				getValue: () => ModEntry.Config.UserInterfaceZoom,
				setValue: value => ModEntry.Config.UserInterfaceZoom = value
			);
			gmcm.AddKeybind(
				mod: ModEntry.ModManifest,
				name: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.UserInterface.Zoom.SecondaryZoomInKey.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.UserInterface.Zoom.SecondaryZoomInKey.Tooltip"),
				getValue: () => ModEntry.Config.UserInterfaceZoomInKey,
				setValue: value => ModEntry.Config.UserInterfaceZoomInKey = value
			);
			gmcm.AddKeybind(
				mod: ModEntry.ModManifest,
				name: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.UserInterface.Zoom.SecondaryZoomOutKey.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.UserInterface.Zoom.SecondaryZoomOutKey.Tooltip"),
				getValue: () => ModEntry.Config.UserInterfaceZoomOutKey,
				setValue: value => ModEntry.Config.UserInterfaceZoomOutKey = value
			);
			gmcm.AddNumberOption(
				mod: ModEntry.ModManifest,
				name: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.UserInterface.Zoom.SpeedMultiplier.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.UserInterface.Zoom.SpeedMultiplier.Tooltip"),
				getValue: () => ModEntry.Config.UserInterfaceZoomMultiplier,
				setValue: value => ModEntry.Config.UserInterfaceZoomMultiplier = value,
				min: 0.25f,
				max: 4.0f,
				interval: 0.25f
			);

			// Other
			gmcm.AddPage(
				mod: ModEntry.ModManifest,
				pageId: "Other",
				pageTitle: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.Other.Title")
			);
			// gmcm.AddPageLink(
			// 	mod: ModEntry.ModManifest,
			// 	pageId: ModEntry.ModManifest.UniqueID,
			// 	text: () => "@ " + "Back to mouahrara's Module Collection"
			// );
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.Other.Description")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Other - Festival end time",
				text: () => "> " + ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.Other.FestivalEndTime.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.Other.FestivalEndTime.Tooltip")
			);
			// Other - Festival end time
			gmcm.AddPage(
				mod: ModEntry.ModManifest,
				pageId: "Other - Festival end time",
				pageTitle: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.Other.FestivalEndTime.Title")
			);
			gmcm.AddPageLink(
				mod: ModEntry.ModManifest,
				pageId: "Other",
				text: () => "@ " + ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.Other.BackTo")
			);
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.Other.FestivalEndTime.Description")
			);
			gmcm.AddParagraph(
				mod: ModEntry.ModManifest,
				text: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.Other.FestivalEndTime.AdditionalInformation")
			);
			gmcm.AddBoolOption(
				mod: ModEntry.ModManifest,
				name: () => ModEntry.Helper.Translation.Get("GMCM.Modules.Enabled.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.Modules.Enabled.Tooltip"),
				getValue: () => ModEntry.Config.OtherFestivalEndTime,
				setValue: value => ModEntry.Config.OtherFestivalEndTime = value
			);
			gmcm.AddNumberOption(
				mod: ModEntry.ModManifest,
				name: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.Other.FestivalEndTime.AdditionalTime.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.TweaksAndFeatures.Other.FestivalEndTime.AdditionalTime.Tooltip"),
				getValue: () => ModEntry.Config.OtherFestivalEndTimeAdditionalTime / 100,
				setValue: value => ModEntry.Config.OtherFestivalEndTimeAdditionalTime = value * 100,
				min: 0,
				max: 8,
				interval: 1
			);
		}
	}
}
