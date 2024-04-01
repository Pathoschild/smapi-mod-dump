/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using Shockah.CommonModCode.GMCM;
using Shockah.Kokoro.GMCM;
using Shockah.ProjectFluent.ContentPatcher;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Linq;

namespace Shockah.ProjectFluent
{
	public class ModEntry : Mod
	{
		public static ModEntry Instance { get; private set; } = null!;
		public IFluentApi Api { get; private set; } = null!;
		private IGameLocale? LocaleOverride { get; set; }
		private bool IsConfigRegistered { get; set; } = false;

		private Harmony Harmony { get; init; } = new Harmony("Shockah.ProjectFluent");
		private MemoryMonitor MemoryMonitor { get; init; } = new MemoryMonitor();

		internal ModConfig Config { get; private set; } = null!;
		internal IFluent<string> Fluent { get; private set; } = null!;

		private IModDirectoryProvider ModDirectoryProvider { get; set; } = null!;
		private IFluentPathProvider FluentPathProvider { get; set; } = null!;
		private IModTranslationsProvider ModTranslationsProvider { get; set; } = null!;
		private IFallbackFluentProvider FallbackFluentProvider { get; set; } = null!;
		private IContentPackParser ContentPackParser { get; set; } = null!;
		private IContentPackManager ContentPackManager { get; set; } = null!;
		private IContentPackProvider ContentPackProvider { get; set; } = null!;
		private IModFluentPathProvider ModFluentPathProvider { get; set; } = null!;
		private II18nDirectoryProvider I18nDirectoryProvider { get; set; } = null!;
		private IFluentValueFactory FluentValueFactory { get; set; } = null!;
		private IFluentFunctionManager FluentFunctionManager { get; set; } = null!;
		private IFluentFunctionProvider FluentFunctionProvider { get; set; } = null!;
		private IContextfulFluentFunctionProvider ContextfulFluentFunctionProvider { get; set; } = null!;
		private IFluentProvider FluentProvider { get; set; } = null!;

		public ModEntry()
		{
			Instance = this;

			I18nIntegration.Monitor = MemoryMonitor;
			I18nIntegration.EarlySetup(Harmony);
		}

		public override void Entry(IModHelper helper)
		{
			Instance = this;
			I18nIntegration.Monitor = Monitor;
			MemoryMonitor.FlushToMonitor(Monitor);

			ModDirectoryProvider = new ModDirectoryProvider(helper.ModRegistry);
			FluentPathProvider = new FluentPathProvider();
			ModTranslationsProvider = new ModTranslationsProvider(helper.ModRegistry);
			FallbackFluentProvider = new FallbackFluentProvider(ModTranslationsProvider);
			ContentPackParser = new ContentPackParser(ModManifest.Version, helper.ModRegistry);
			var contentPackManager = new ContentPackManager(Monitor, helper.ContentPacks, ContentPackParser);
			ContentPackManager = contentPackManager;
			ContentPackProvider = new SerialContentPackProvider(
				contentPackManager,
				new AssetContentPackProvider(Monitor, helper.Data, helper.Events.Content, ContentPackParser)
			);
			ModFluentPathProvider = new SerialModDirectoryFluentPathProvider(
				new ContentPackAdditionalModFluentPathProvider(helper.ModRegistry, ContentPackProvider, FluentPathProvider, ModDirectoryProvider),
				new ModFluentPathProvider(ModDirectoryProvider, FluentPathProvider),
				new ModFluentPathProvider(ModDirectoryProvider, FluentPathProvider, DefaultLocale)
			);
			I18nDirectoryProvider = new ContentPackI18nDirectoryProvider(helper.ModRegistry, ContentPackProvider, ModDirectoryProvider);
			FluentValueFactory = new FluentValueFactory();
			var fluentFunctionManager = new FluentFunctionManager();
			FluentFunctionManager = fluentFunctionManager;
			var builtInFluentFunctionProvider = new BuiltInFluentFunctionProvider(ModManifest, helper.ModRegistry, FluentValueFactory, ModTranslationsProvider);
			FluentFunctionProvider = new SerialFluentFunctionProvider(
				builtInFluentFunctionProvider,
				fluentFunctionManager
			);
			ContextfulFluentFunctionProvider = new ContextfulFluentFunctionProvider(ModManifest, FluentFunctionProvider);
			FluentProvider = new FluentProvider(Monitor, FallbackFluentProvider, ModFluentPathProvider, ContextfulFluentFunctionProvider);

			builtInFluentFunctionProvider.FluentProvider = FluentProvider;
			Api = new FluentApi(FluentProvider, FluentFunctionManager, FluentValueFactory, DefaultLocale);
			Config = helper.ReadConfig<ModConfig>();
			Fluent = Api.GetLocalizationsForCurrentLocale(ModManifest);
			UpdateLocaleOverride();

			helper.Events.GameLoop.GameLaunched += OnGameLaunched;
			helper.Events.Content.AssetsInvalidated += OnAssetsInvalidated;
			I18nIntegration.Setup(Harmony, I18nDirectoryProvider);
		}

		public override object GetApi() => Api;

		private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
		{
			ContentPackManager.RegisterAllContentPacks();
			I18nIntegration.ReloadTranslations();
			ContentPatcherIntegration.Setup(Harmony);

			SetupConfig();
		}

		private void OnAssetsInvalidated(object? sender, AssetsInvalidatedEventArgs e)
		{
			foreach (var name in e.Names)
			{
				if (name.IsEquivalentTo("Data/AdditionalLanguages"))
				{
					SetupConfig();
					break;
				}
			}
		}

		private void SetupConfig()
		{
			var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
			if (api is null)
				return;
			GMCMI18nHelper helper = new(
				api, ModManifest, new FluentTranslationSet<string>(Fluent),
				namePattern: "{Key}",
				tooltipPattern: "{Key}.tooltip",
				valuePattern: "{Key}.{Value}"
			);

			if (IsConfigRegistered)
				api.Unregister(ModManifest);

			api.Register(
				ModManifest,
				reset: () => Config = new ModConfig(),
				save: () =>
				{
					Helper.WriteConfig(Config);
					UpdateLocaleOverride();
					SetupConfig();
				}
			);

			helper.AddEnumOption(
				keyPrefix: "config-contentPatcherPatchingMode",
				property: () => Config.ContentPatcherPatchingMode
			);

			helper.AddTextOption(
				keyPrefix: "config-localeOverride",
				property: () => Config.CurrentLocaleOverride
			);

			helper.AddParagraph(
				"config-localeOverrideSubtitle",
				new { Values = Api.AllKnownLocales.Select(l => l.LocaleCode).Join() }
			);

			helper.AddBoolOption(
				keyPrefix: "config-developerMode",
				property: () => Config.DeveloperMode
			);

			IsConfigRegistered = true;
		}

		private void UpdateLocaleOverride()
		{
			LocaleOverride = Config.CurrentLocaleOverride == ""
				? null
				: Api.AllKnownLocales.FirstOrDefault(l => l.LocaleCode.Equals(Config.CurrentLocaleOverride, System.StringComparison.InvariantCultureIgnoreCase));
		}

		#region APIs

		public static IGameLocale DefaultLocale
			=> new BuiltInGameLocale(LocalizedContentManager.LanguageCode.en);

		public IGameLocale CurrentLocale
		{
			get
			{
				return LocaleOverride ?? LocalizedContentManager.CurrentLanguageCode switch
				{
					LocalizedContentManager.LanguageCode.mod => new ModGameLocale(LocalizedContentManager.CurrentModLanguage),
					_ => new BuiltInGameLocale(LocalizedContentManager.CurrentLanguageCode),
				};
			}
		}

		#endregion
	}
}