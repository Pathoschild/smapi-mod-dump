/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

// If THEME_MANAGER_PRE_314 is defined, we'll use SMAPI 3.13 and earlier
// APIs for compatibility. If it is not defined, we'll use the new APIs
// for content added in 3.14. Just comment or uncomment this line as
// necessary for your build target.
// #define THEME_MANAGER_PRE_314
#pragma warning disable IDE0003
#nullable enable

using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

#if DEBUG
using Newtonsoft.Json;
using System.Net;
using System.Threading;
#endif

namespace Leclair.Stardew.Common.UI {
#if DEBUG
	internal class UpdateData
	{
		public string? Version { get; set; }
	}
#endif

	/// <summary>
	/// A manifest has necessary metadata for a theme for display in theme
	/// selection UI, for performing automatic theme selection, and for
	/// loading assets correctly from the filesystem.
	/// </summary>
	public class ThemeManifest {
		#region Constructor

		internal ThemeManifest(string uniqueID, string name, IReadOnlyDictionary<string, string>? localizedNames, string? translationKey, IManifest providingMod, string[]? supportedMods, string[]? unsupportedMods, string? fallbackTheme, string? assetPrefix, bool? overrideRedirection) {
			UniqueID = uniqueID;
			Name = name;
			LocalizedNames = localizedNames;
			TranslationKey = translationKey;
			ProvidingMod = providingMod;
			SupportedMods = supportedMods;
			UnsupportedMods = unsupportedMods;
			FallbackTheme = fallbackTheme;
			AssetPrefix = assetPrefix;
			OverrideRedirection = overrideRedirection;
		}

		#endregion

		#region Identification

		/// <summary>
		/// The unique ID of this theme.
		/// </summary>
		public string UniqueID { get; }

		/// <summary>
		/// The name of this theme, used if no localized name is available
		/// matching the current game locale.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// A mapping of locale codes to human-readable theme names, suitable
		/// for display in a user interface for selecting themes. If
		/// <see cref="TranslationKey"/> is present, this is only used if the
		/// translation does not have a value.
		/// </summary>
		public IReadOnlyDictionary<string, string>? LocalizedNames { get; }

		/// <summary>
		/// A translation key to use for getting a localized name for this
		/// theme. This provides an alternative to <see cref="LocalizedNames"/>
		/// for translating themes.
		///
		/// If set, the translation is pulled from the
		/// <see cref="ITranslationHelper"/> of the mod that provides this theme.
		/// </summary>
		public string? TranslationKey { get; }

		/// <summary>
		/// The manifest of the mod that provides this theme.
		/// </summary>
		public IManifest ProvidingMod { get; }

		#endregion

		#region Theme Selection

		/// <summary>
		/// An array of <see cref="IManifest.UniqueID"/>s of mods that this theme
		/// is meant to support. When <see cref="IThemeManager{DataT}.SelectedThemeId"/>
		/// is set to <c>automatic</c>, this theme will potentially be selected if
		/// any of the listed mods are present and loaded.
		/// </summary>
		public string[]? SupportedMods { get; }

		/// <summary>
		/// An array of <see cref="IManifest.UniqueID"/>s of mods that this theme
		/// does <b>not</b> support. When <see cref="IThemeManager{DataT}.SelectedThemeId"/>
		/// is set to <c>automatic</c>, this theme will never be selected if any
		/// of the listed mods are present and loaded.
		/// </summary>
		public string[]? UnsupportedMods { get; }

		#endregion

		#region Asset Loading

		/// <summary>
		/// The <see cref="UniqueID"/> of another theme. If a requested asset is
		/// not available in this theme, try loading it from that other theme
		/// before falling back to loading it from the <c>default</c> theme.
		/// </summary>
		public string? FallbackTheme { get; }

		/// <summary>
		/// A string that is prepended to asset paths when loading assets from
		/// this theme.
		/// </summary>
		public string? AssetPrefix { get; }

		/// <summary>
		/// When set to a non-null value, assets for this theme will either be
		/// forced to use asset redirection, or forced to <b>not</b> use asset
		/// redirection, overriding the default behavior from the
		/// <see cref="IThemeManager{DataT}"/> instance.
		/// </summary>
		public bool? OverrideRedirection { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Check to see if any loaded mods are in this theme's For block.
		/// </summary>
		/// <param name="registry">Your own mod's IModRegistry helper</param>
		/// <returns>true if there are any matching mods</returns>
		public bool MatchesForAutomatic(IModRegistry registry) {
			if (this.UnsupportedMods != null)
				foreach (string mod in this.UnsupportedMods) {
					if (!string.IsNullOrEmpty(mod) && registry.IsLoaded(mod))
						return false;
				}

			if (this.SupportedMods != null)
				foreach (string mod in this.SupportedMods) {
					if (!string.IsNullOrEmpty(mod) && registry.IsLoaded(mod))
						return true;
				}

			return false;
		}

		#endregion
	}

	/// <summary>
	/// This event is emitted by <see cref="ThemeManager{DataT}"/> whenever the
	/// current theme changes, whether because the themes were reload or
	/// because the user selected a different theme.
	/// </summary>
	/// <typeparam name="DataT">Your mod's theme data type</typeparam>
	public class ThemeChangedEventArgs<DataT> : EventArgs {
		/// <summary>
		/// The theme ID of the old theme
		/// </summary>
		public string OldId;
		/// <summary>
		/// The theme ID of the new theme.
		/// </summary>
		public string NewId;

		/// <summary>
		/// The theme data of the old theme
		/// </summary>
		public DataT? OldData;
		/// <summary>
		/// The theme data of the new theme
		/// </summary>
		public DataT NewData;

		public ThemeChangedEventArgs(string oldId, DataT? oldData, string newID, DataT newData) {
			OldId = oldId;
			NewId = newID;
			OldData = oldData;
			NewData = newData;
		}
	}

	/// <summary>
	/// LoadableManifest is a basic class with manifest properties we support
	/// loading from theme.json files. These values are used for building a
	/// proper <see cref="ThemeManifest"/> instance for a theme.
	/// </summary>
	internal class LoadableManifest {
		public string? UniqueID { get; set; }
		public string? Name { get; set; }
		public Dictionary<string, string>? LocalizedNames { get; set; }
		public string? TranslationKey { get; set; }
		public string[]? SupportedMods { get; set; }
		public string[]? For { get; set; }
		public string[]? UnsupportedMods { get; set; }
		public string? FallbackTheme { get; set; }
		public string? AssetPrefix { get; set; } = "assets";
		public bool? OverrideRedirection { get; set; }
	}

	/// <summary>
	/// Theme records group together a theme's <typeparamref name="DataT"/>
	/// and <see cref="IContentPack"/> instances for convenience.
	/// </summary>
	/// <typeparam name="DataT">Your mod's BaseThemeData subclass</typeparam>
	/// <param name="Data">The theme's theme data.</param>
	/// <param name="Content">The theme's content pack.</param>
	internal record Theme<DataT>(
		DataT Data,
		ThemeManifest Manifest,
		IContentPack Content,
		string? RelativePath
	);

	/// <summary>
	/// ThemeManager is a standalone class for adding theme support to
	/// Stardew Valley mods written in C#. It handles discovery, selection,
	/// asset loading with Content Patcher support, and emitting an event
	/// when the theme changes.
	///
	/// Ideally, this should be a drop in replacement for most mods, only
	/// requiring them to replace their <see cref="IModContentHelper.Load{T}(string)"/>
	/// calls with <see cref="Load{T}(string)"/>.
	/// </summary>
	/// <typeparam name="DataT">Your mod's theme data type</typeparam>
	public class ThemeManager<DataT> where DataT : class, new() {

		public static readonly SemanticVersion Version = new("3.0.0");

		public static readonly string ContentPatcher_UniqueID = "Pathoschild.ContentPatcher";

		#region Internal Fields

		/// <summary>
		/// Your mod. We use the manifest, helper, and monitor frequently.
		/// </summary>
		private readonly Mod Mod;

		/// <summary>
		/// A dictionary mapping all known themes to their own UniqueIDs.
		/// </summary>
		private readonly Dictionary<string, Theme<DataT>> Themes = new();

		/// <summary>
		/// Storage of the <see cref="DefaultTheme"/>. This should not be accessed directly,
		/// instead prefering the use of <see cref="DefaultTheme"/> (which is public).
		/// </summary>
		private DataT _DefaultTheme;

		/// <summary>
		/// The currently active theme. We store this directly to avoid needing
		/// constant dictionary lookups when working with a theme.
		/// </summary>
		private Theme<DataT>? BaseThemeData = null;

		/// <summary>
		/// Assets in the process of being loaded are stored in this
		/// dictionary so we can continue to take advantage of types when
		/// loading assets.
		/// </summary>
		private readonly Dictionary<string, Func<object>> LoadingAssets = new();

		private readonly Dictionary<IManifest, IContentPack> ModContentPacks = new();

#if THEME_MANAGER_PRE_314
		/// <summary>
		/// The <c>Loader</c> is a private <see cref="IAssetLoader"/> that we
		/// use for redirecting asset loading through game content, allowing
		/// Content Patcher (and other mods) to modify them.
		/// </summary>
		private readonly ThemeAssetLoader Loader;
#endif

		#endregion

		#region Public Fields

		/// <summary>
		/// A string that is prepended to asset names when redirecting
		/// asset loading through game content to allow Content Patcher access
		/// to your mod's themed resources.
		/// </summary>
		public string AssetLoaderPrefix { get; }

		/// <summary>
		/// The relative path to where your mod keeps its embedded themes.
		/// By default, this is <c>assets/themes</c>. If this is <c>null</c>,
		/// no embedded themes will be loaded.
		/// </summary>
		public string? EmbeddedThemesPath { get; }

		/// <summary>
		/// Whether or not <see cref="ThemeManager{DataT}"/> is redirecting
		/// asset loading through <see cref="IGameContentHelper"/> to allow
		/// other mods, such as Content Patcher, to modify theme assets.
		/// </summary>
		public bool UsingAssetRedirection { get; }

		/// <summary>
		/// A string that is prepended to asset paths when loading assets from
		/// the <c>default</c> theme. This prefix is not added to paths when
		/// loading assets from other themes, as themes have their own
		/// <c>AssetPrefix</c> to use.
		/// </summary>
		public string? DefaultAssetPrefix { get; set; }

		#endregion

		#region Constructor

		/// <summary>
		/// Create a new ThemeManager.
		/// </summary>
		/// <param name="mod">Your mod's Mod class. We need access to your
		/// mod's ModManifest, Helper, and Monitor.</param>
		/// <param name="selectedThemeId">The initial selected theme ID. When
		/// Discover is run, this value will be used to immediately select the
		/// desired theme. <seealso cref="SelectedThemeId"/></param>
		/// <param name="defaultTheme">The <typeparamref name="DataT"/> instance
		/// to use when the default theme is selected.
		/// <seealso cref="DefaultTheme"/></param>
		/// <param name="embeddedThemesPath">The path to search your mod for
		/// embedded themes at. <seealso cref="EmbeddedThemesPath"/></param>
		/// <param name="assetPrefix">The asset prefix to use when
		/// the default theme is selected.
		/// <seealso cref="DefaultAssetPrefix"/></param>
		/// <param name="assetLoaderPrefix">A prefix prepended to all asset
		/// paths when redirecting asset loading through IAssetLoader. By
		/// default, this value is <c>Mods/{yourMod.UniqueId}/Themes</c>.
		/// <seealso cref="AssetLoaderPrefix"/></param>
		/// <param name="forceAssetRedirection">If set to a value, override
		/// the default asset redirection behavior.
		/// <seealso cref="UsingAssetRedirection"/></param>
		public ThemeManager(
			Mod mod,
			string selectedThemeId = "automatic",
			DataT? defaultTheme = null,
			string? embeddedThemesPath = "assets/themes",
			string? assetPrefix = "assets",
			string? assetLoaderPrefix = null,
			bool? forceAssetRedirection = null
		) {
			// Store the basic initial values.
			Mod = mod;
			SelectedThemeId = selectedThemeId;
			DefaultAssetPrefix = assetPrefix;
			EmbeddedThemesPath = embeddedThemesPath;

			_DefaultTheme = defaultTheme ?? new DataT();

			// Log our version.
			Log($"Using Theme Manager {Version}");

			// Detect Content Patcher
			bool hasCP = Mod.Helper.ModRegistry.IsLoaded(ContentPatcher_UniqueID);

			if (hasCP && forceAssetRedirection.HasValue && !forceAssetRedirection.Value)
				Log("Content Patcher is present. However, asset redirection has been explicitly disabled.");
			else if (hasCP)
				Log("Content Patcher is present. Asset redirection will be enabled.");
			else if (forceAssetRedirection.HasValue && forceAssetRedirection.Value)
				Log("Content Patcher is NOT present. However, asset redirection has been explicitly enabled.");
			else
				Log("Content Patcher is NOT present. Asset redirection will be disabled.");

			UsingAssetRedirection = forceAssetRedirection ?? hasCP;

			// Always run the AssetLoaderPrefix through NormalizeAssetName,
			// otherwise we'll run into issues actually using our custom
			// asset loader.
			AssetLoaderPrefix = PathUtilities.NormalizeAssetName(
				string.IsNullOrEmpty(assetLoaderPrefix) ?
					Path.Join("Mods", mod.ModManifest.UniqueID, "Themes") :
					assetLoaderPrefix
				);

			// Register an event listener so we can provide assets.
#if THEME_MANAGER_PRE_314
			Loader = new ThemeAssetLoader(this);
			Mod.Helper.Content.AssetLoaders.Add(Loader);
#else
			Mod.Helper.Events.Content.AssetRequested += OnAssetRequested;
#endif

#if DEBUG
			// Perform an update check when running a debug build, to
			// notify developers when an updated ThemeManager is available.
			PerformVersionCheck();
#endif
		}

		#endregion

		#region Properties

		/// <summary>
		/// The I18nPrefix is prepended to translation keys generated by
		/// ThemeManager. Currently, the only keys ThemeManager generates are
		/// for the Automatic and Default themes.
		/// </summary>
		public string I18nPrefix { get; set; } = "theme";

		#endregion

		#region Logging

		private void Log(string message, LogLevel level = LogLevel.Trace, Exception? ex = null, LogLevel? exLevel = null) {
			Mod.Monitor.Log($"[ThemeManager] {message}", level: level);
			if (ex != null)
				Mod.Monitor.Log($"[ThemeManager] Details:\n{ex}", level: exLevel ?? level);
		}

		#endregion

		#region Verison Checking

#if DEBUG
		/// <summary>
		/// Perform a version check against the GitHub repository, and log a
		/// message if this version of ThemeManager is out of date. This should
		/// only happen for debug builds, and even then only when a debugger
		/// is attached to ensure that end users aren't warned about updating
		/// something that has nothing to do with them.
		/// </summary>
		private void PerformVersionCheck()
		{
			if (!System.Diagnostics.Debugger.IsAttached)
				return;

			new Thread(() =>
			{
				Log("Checking for updates...");
				try
				{
					using WebClient client = new();

					string result = client.DownloadString("https://raw.githubusercontent.com/KhloeLeclair/Stardew-ThemeManager/main-4/latest.json");
					UpdateData? data = JsonConvert.DeserializeObject<UpdateData>(result);

					if (data is not null && data.Version is not null && SemanticVersion.TryParse(data.Version, out ISemanticVersion? parsed))
					{
						if (Version.IsOlderThan(parsed))
						{
							Log($"ThemeManager is out of date. Using: {Version}, Latest: {parsed}", LogLevel.Alert);
							Log("Please update ThemeManager from https://github.com/KhloeLeclair/Stardew-ThemeManager", LogLevel.Alert);
							Log("If you are not a mod developer, you can ignore this message as it is not intended for you.", LogLevel.Alert);
						}
						else
						{
							Log("ThemeManager is up to date.", LogLevel.Trace);
						}
					}
				}
				catch (Exception ex)
				{
					Log($"Could not check for updated version of ThemeManager.", LogLevel.Warn, ex);
				}
			}).Start();
	}
#endif

		#endregion

		#region Console Commands

		/// <summary>
		/// This method is designed for use as a console command. Using it
		/// invalidates all of our loaded resources and repeats theme
		/// discovery. A <see cref="ThemeChanged"/> event will be dispatched,
		/// and a message will be logged to the console notifying the user
		/// that the themes were reloaded.
		/// </summary>
		public void PerformReloadCommand() {
			Invalidate();
			Discover();
			Log($"Reloaded themes. You may need to reopen menus.", LogLevel.Info);
		}

		[SuppressMessage("Style", "IDE0060", Justification = "Provided for ease of use with SMAPI's API.")]
		public void PerformReloadCommand(string name, string[] args) {
			PerformReloadCommand();
		}

		/// <summary>
		/// This method is designed for use as a console command. This
		/// command logs the available, selected, and active themes as
		/// well as allowing you to easily change the current theme or
		/// to reload themes.
		/// </summary>
		/// <param name="args">Arguments passed to the console command</param>
		/// <returns>True if the <see cref="SelectedThemeId"/> changed as a
		/// result of this command. In such an event, you may wish to update
		/// the user's config with their selection.</returns>
		public bool PerformThemeCommand(string[] args) {
			if (args.Length > 0 && !string.IsNullOrEmpty(args[0])) {
				string key = args[0].Trim();

				// Check for reload first.
				if (key.Equals("reload", StringComparison.OrdinalIgnoreCase)) {
					PerformReloadCommand();
					return false;
				}

				string? selected = null;
				var themes = GetThemeChoices();

				// Check for unique ID matches first.
				foreach (var pair in themes) {
					if (pair.Key.Equals(key, StringComparison.OrdinalIgnoreCase)) {
						selected = pair.Key;
						break;
					}
				}

				// Then check for unique ID partial matches.
				if (selected == null)
					foreach (var pair in themes) {
						if (pair.Key.Contains(key, StringComparison.OrdinalIgnoreCase)) {
							selected = pair.Key;
							break;
						}
					}

				// Then select for display name partial matches.
				if (selected == null)
					foreach (var pair in themes) {
						if (GetThemeName(pair.Key).Contains(key, StringComparison.OrdinalIgnoreCase)) {
							selected = pair.Key;
							break;
						}
					}

				// If we've selected something, actually select it.
				if (selected != null) {
					SelectTheme(selected);
					Log($"Selected Theme: {selected}", LogLevel.Info);
					return true;
				}

				Log($"Unable to match theme: {key}", LogLevel.Warn);
				return false;
			}

			// If we've gotten here, we should log a list of all available
			// themes for the user's benefit.
			Log($"Available Themes:", LogLevel.Info);
			foreach (var pair in GetThemeChoices()) {
				bool selected = pair.Key == SelectedThemeId;
				bool used = pair.Key == ActiveThemeId;

				string selection = selected ?
					(used ? "=>" : " >") :
					(used ? "= " : "  ");

				Log($" {selection} [{pair.Key}]: {pair.Value}", LogLevel.Info);
			}

			return false;
		}

		#endregion

		#region Theme Discovery

		private record LoadingTheme(
			IManifest Mod,
			IContentPack Pack,
			string? RelativePath,
			string ThemeFileName
		);

		private IContentPack GetSelfContentPack() {
			if (ModContentPacks.TryGetValue(Mod.ModManifest, out IContentPack? cp))
				return cp;

			cp = Mod.Helper.ContentPacks.CreateTemporary(
				directoryPath: Mod.Helper.DirectoryPath,
				id: $"{Mod.ModManifest.UniqueID}.theme-loader",
				name: Mod.ModManifest.Name,
				description: Mod.ModManifest.Description,
				author: Mod.ModManifest.Author,
				version: Mod.ModManifest.Version
			);

			ModContentPacks[Mod.ModManifest] = cp;
			return cp;
		}

		private IContentPack? GetModContentPack(IModInfo mod) {
			if (ModContentPacks.TryGetValue(mod.Manifest, out IContentPack? cp))
				return cp;

			if (mod.IsContentPack && mod.GetType().GetProperty("ContentPack", BindingFlags.Instance | BindingFlags.Public)?.GetValue(mod) is IContentPack pack)
				cp = pack;

			else if (mod.GetType().GetProperty("DirectoryPath", BindingFlags.Instance | BindingFlags.Public)?.GetValue(mod) is string str) {
				cp = Mod.Helper.ContentPacks.CreateTemporary(
					directoryPath: str,
					id: $"{Mod.ModManifest.UniqueID}.theme-loader.${mod.Manifest.UniqueID}",
					name: mod.Manifest.Name,
					description: mod.Manifest.Description,
					author: mod.Manifest.Author,
					version: mod.Manifest.Version
				);

			} else
				return null;

			ModContentPacks[mod.Manifest] = cp;
			return cp;
		}

		/// <summary>
		/// Perform theme discovery, reloading all theme data, and then update
		/// the active theme.
		/// </summary>
		/// <param name="checkOwned">Whether or not to load themes from owned content packs</param>
		/// <param name="checkEmbedded">Whether or not to load embedded themes.</param>
		/// <param name="checkExternal">Whether or not to load external themes from other mods.</param>
		/// <returns>The ThemeManager</returns>
		public ThemeManager<DataT> Discover(
			bool checkOwned = true,
			bool checkEmbedded = true,
			bool checkExternal = true
		) {
			lock ((Themes as ICollection).SyncRoot) {
				// Start by wiping the existing theme data.
				Themes.Clear();

				// We want to keep track of packs with custom IDs so that we
				// can use better IDs for embedded packs.
				Dictionary<string, LoadingTheme> packsWithIds = new();

				// If we haven't been forbidden, check for embedded themes and
				// add them to our packs. We do this first so that any content
				// pack themes can override our embedded themes, if they really
				// need to. They shouldn't, however.
				if (checkEmbedded) {
					var embedded = FindEmbeddedThemes();
					if (embedded != null)
						foreach (var cp in embedded)
							packsWithIds[cp.Key] = cp.Value;
				}

				// Now, check for your mod's owned content packs.
				if (checkOwned) {
					var owned = Mod.Helper.ContentPacks.GetOwned();
					if (owned != null)
						foreach (var cp in owned) {
							if (cp.HasFile("theme.json"))
								packsWithIds[cp.Manifest.UniqueID] = new(cp.Manifest, cp, null, "theme.json");
						}
				}

				// Finally, check for external mods that provide themes.
				if (checkExternal) {
					var external = FindExternalThemes();
					if (external != null)
						foreach (var cp in external)
							packsWithIds[cp.Key] = cp.Value;
				}

				// Now, load each of our packs.
				foreach (var cp in packsWithIds) {
					string file = string.IsNullOrEmpty(cp.Value.ThemeFileName) ? "theme.json" : cp.Value.ThemeFileName;
					string fpath = string.IsNullOrEmpty(cp.Value.RelativePath) ? file : Path.Join(cp.Value.RelativePath, file);

					if (!cp.Value.Pack.HasFile(fpath))
						continue;

					LoadableManifest? loadable;
					DataT? data;
					try {
						loadable = cp.Value.Pack.ReadJsonFile<LoadableManifest>(fpath);
						if (loadable is null)
							throw new Exception($"{file} is empty or invalid");

						data = cp.Value.Pack.ReadJsonFile<DataT>(fpath);
						if (data is null)
							throw new Exception($"{file} is empty or invalid");
					} catch (Exception ex) {
						Log($"The theme at {fpath} in {cp.Value.Pack.Manifest.Name} has an invalid theme json file and could not be loaded.", LogLevel.Warn, ex);
						continue;
					}

					string uid = loadable.UniqueID ?? cp.Key;

					ThemeManifest manifest = new(
						uniqueID: uid,
						name: loadable.Name ?? uid,
						localizedNames: loadable.LocalizedNames,
						translationKey: loadable.TranslationKey,
						providingMod: cp.Value.Mod,
						supportedMods: loadable.SupportedMods ?? loadable.For,
						unsupportedMods: loadable.UnsupportedMods,
						fallbackTheme: loadable.FallbackTheme,
						assetPrefix: loadable.AssetPrefix,
						overrideRedirection: loadable.OverrideRedirection
					);

					Themes[cp.Key] = new(data, manifest, cp.Value.Pack, cp.Value.RelativePath);
				}
			}

			// Store our currently selected theme.
			string? oldKey = SelectedThemeId;

			// Clear our data.
			BaseThemeData = null;
			SelectedThemeId = null;
			ActiveThemeId = "default";

			// And select the new theme.
			SelectTheme(oldKey, true);
			return this;
		}

		/// <summary>
		/// Search for loose themes in your mod's embedded themes folder.
		/// </summary>
		/// <returns>A dictionary of temporary IContentPacks for each embedded theme.</returns>
		private Dictionary<string, LoadingTheme>? FindEmbeddedThemes() {
			if (string.IsNullOrEmpty(EmbeddedThemesPath))
				return null;

			string path = Path.Join(Mod.Helper.DirectoryPath, PathUtilities.NormalizePath(EmbeddedThemesPath));
			if (!Directory.Exists(path))
				return null;

			Dictionary<string, LoadingTheme> results = new();
			int count = 0;

			// Start iterating subdirectories of our embedded themes folder.
			foreach (string dir in Directory.GetDirectories(path)) {
				string t_path = Path.Join(dir, "theme.json");

				// If the subdirectory has no theme.json, ignore it.
				if (!File.Exists(t_path))
					continue;

				string rel_path = Path.GetRelativePath(Mod.Helper.DirectoryPath, dir);
				string folder = Path.GetRelativePath(path, dir);

				Log($"Found Embedded Theme At: {dir}", LogLevel.Trace);

				LoadableManifest? manifest = null;
				try {
					manifest = Mod.Helper.Data.ReadJsonFile<LoadableManifest>(
						Path.Join(rel_path, "theme.json"));

					if (manifest is null)
						throw new Exception("theme.json is empty or invalid");
				} catch (Exception ex) {
					Log($"Unable to read embedded theme manifest.", LogLevel.Warn, ex);
					continue;
				}

				// TODO: Slugify the folder name.

				var cp = GetSelfContentPack();

				string uid = manifest.UniqueID ?? $"embedded.{folder}";

				results[uid] = new(Mod.ModManifest, cp, rel_path, "theme.json");
				count++;

				string name = manifest.Name ?? uid;

				Log($"Found Embedded Theme: {name} ({uid})", LogLevel.Trace);
			}

			Log($"Found {count} Embedded Themes.", LogLevel.Trace);
			return results;
		}

		/// <summary>
		/// Search for themes in other mods' manifests.
		/// </summary>
		/// <returns>A dictionary of temporary IContentPacks for each discovered theme.</returns>
		private Dictionary<string, LoadingTheme> FindExternalThemes() {
			Dictionary<string, LoadingTheme> results = new();
			int count = 0;

			string themeKey = $"{Mod.ModManifest.UniqueID}:theme";
			string themeFile = $"{Mod.ModManifest.UniqueID}.theme.json";

			foreach (var mod in Mod.Helper.ModRegistry.GetAll()) {
				// For every mod, try reading a special value from its manifest.
				if (mod?.Manifest?.ExtraFields == null)
					continue;

				if (!mod.Manifest.ExtraFields.TryGetValue(themeKey, out object? value))
					continue;

				string file = themeFile;

				// If the value is a boolean, and false for some reason,
				// just skip the mod. If it's true, assume the default
				// filename for our theme JSON.
				if (value is bool bv) {
					if (!bv)
						continue;

					// If the value is a string, use that string as a relative
					// filename for the theme.json file, relative to that mod's
					// directory root.
				} else if (value is string str) {
					file = str;

					// Display a warning for any other value, as we can only handle
					// strings and booleans.
				} else {
					Log($"Unknown or unsupported value for {themeKey} in mod {mod.Manifest.Name} ({mod.Manifest.UniqueID})", LogLevel.Warn);
					continue;
				}

				// Get an IContentPack for this mod.
				var cp = GetModContentPack(mod);
				if (cp is null) {
					Log($"Unable to get IContentPack for mod: {mod.Manifest.UniqueID}", LogLevel.Warn);
					continue;
				}

				file = PathUtilities.NormalizePath(file);

				if (!cp.HasFile(file)) {
					Log($"Unable to find {file} in mod {mod.Manifest.Name} ({mod.Manifest.UniqueID})", LogLevel.Warn);
					continue;
				}

				// Get the relative path for the file, and just the filename.
				string? folder = Path.GetDirectoryName(file);
				file = Path.GetFileName(file);

				results[mod.Manifest.UniqueID] = new(mod.Manifest, cp, folder, file);
				count++;
			}

			Log($"Found {count} External Themes");
			return results;
		}

		#endregion

		#region Theme Enumeration

		/// <summary>
		/// Get the human readable name of a theme, optionally in a given locale.
		/// If no locale is given, the game's current locale will be used.
		/// </summary>
		/// <param name="themeId">The <see cref="ThemeManifest.UniqueID"/> of the
		/// theme you want to get the name of.</param>
		/// <param name="locale">The locale to get the name in, if possible.</param>
		public string GetThemeName(string themeId, string? locale = null) {
			// For the default theme, return a translation from the host mod's
			// translation layer.
			if (themeId.Equals("default", StringComparison.OrdinalIgnoreCase))
				return Mod.Helper.Translation.Get($"{I18nPrefix}.default").ToString();

			// Get the theme data. If the theme is the active theme, don't
			// bother with a dictionary lookp.
			Theme<DataT>? theme;
			if (themeId == ActiveThemeId)
				theme = BaseThemeData;
			else {
				lock ((Themes as ICollection).SyncRoot) {
					if (!Themes.TryGetValue(themeId, out theme))
						return themeId;
				}
			}

			// If we don't have theme data, just return the themeId.
			if (theme == null)
				return themeId;

			// Check for the translation in our theme data.
			if (string.IsNullOrEmpty(locale))
				locale = Mod.Helper.Translation.Locale;

			// 
			if (!string.IsNullOrEmpty(theme.Manifest.TranslationKey)) {
				var tls = theme.Content.Translation.GetInAllLocales(theme.Manifest.TranslationKey);
				if (tls != null && tls.TryGetValue(locale, out var tl)) {
					string? result = tl.ToString();
					if (!string.IsNullOrEmpty(result))
						return result;
				}
			}

			if (theme.Manifest.LocalizedNames?.TryGetValue(locale, out string? name) ?? false && name != null)
				return name;

			// Manifest Name
			return theme.Manifest.Name;
		}

		/// <summary>
		/// Get an enumeration of the available themes, suitable for display in
		/// a configuration menu such as Generic Mod Config Menu. This list will
		/// always include an <c>automatic</c> and <c>default</c> entry.
		///
		/// The keys are <see cref="ThemeManifest.UniqueID"/>s while the values
		/// are human readable names in the current (or selected) locale.
		/// </summary>
		/// <param name="locale">The locale to get names in, if possible.</param>
		public IReadOnlyDictionary<string, string> GetThemeChoices(string? locale = null) {
			Dictionary<string, string> result = new();

			result.Add("automatic", Mod.Helper.Translation.Get($"{I18nPrefix}.automatic"));
			result.Add("default", Mod.Helper.Translation.Get($"{I18nPrefix}.default"));

			foreach (string theme in Themes.Keys)
				result.Add(theme, GetThemeName(theme, locale));

			return result;
		}

		/// <summary>
		/// Get an enumeration of the available themes, suitable for display in
		/// a configuration menu such as Generic Mod Config Menu. This list will
		/// always include an <c>automatic</c> and <c>default</c> entry.
		///
		/// The keys are <see cref="ThemeManifest.UniqueID"/>s while the values
		/// are methods returning human readable names in the current locale.
		/// </summary>
		public IReadOnlyDictionary<string, Func<string>> GetThemeChoiceMethods() {
			Dictionary<string, Func<string>> result = new();

			result.Add("automatic", () => Mod.Helper.Translation.Get($"{I18nPrefix}.automatic"));
			result.Add("default", () => Mod.Helper.Translation.Get($"{I18nPrefix}.default"));

			foreach (string theme in Themes.Keys)
				result.Add(theme, () => GetThemeName(theme));

			return result;
		}

		/// <summary>
		/// Get an enumeration of the manifests of every loaded theme. This will
		/// not contain an entry for the <c>default</c> theme.
		/// </summary>
		public IReadOnlyCollection<ThemeManifest> GetThemeManifests() {
			return Themes.Values.Select(x => x.Manifest).ToList().AsReadOnly();
		}

		/// <summary>
		/// Check to see if a given theme has been loaded.
		/// </summary>
		/// <param name="themeId">The <see cref="ThemeManifest.UniqueID"/> of the
		/// theme we're interested in.</param>
		public bool HasTheme(string themeId) {
			return Themes.ContainsKey(themeId);
		}

		/// <summary>
		/// Try to get the <typeparamref name="DataT"/> instance for a specific
		/// theme, if it's loaded.
		///
		/// As this method uses a dictionary lookup internally, you should cache
		/// the result if you use it frequently for best performance. If you do
		/// cache the result, make sure to update your cache whenever the
		/// <see cref="ThemeChanged"/> event is emitted.
		/// </summary>
		/// <param name="themeId">The <see cref="ThemeManifest.UniqueID"/> of the
		/// theme we want the data instance for.</param>
		/// <param name="theme">The <typeparamref name="DataT"/> instance for
		/// the requested theme, or <c>null</c> if the theme is not loaded.</param>
		public bool TryGetTheme(string themeId, [NotNullWhen(true)] out DataT? theme) {
			if (Themes.TryGetValue(themeId, out var tdata)) {
				theme = tdata.Data;
				return true;
			}

			theme = null;
			return false;
		}

		/// <summary>
		/// Try to get the <see cref="ThemeManifest"/> instance for a specific
		/// theme, if it's loaded.
		/// </summary>
		/// <param name="themeId">The <see cref="ThemeManifest.UniqueID"/> of the
		/// theme we want the manifest for.</param>
		/// <param name="manifest">The <see cref="ThemeManifest"/> instance for
		/// the requested theme, or <c>null</c> if the theme is not loaded or
		/// has no manifest. Only the <c>default</c> theme has no manifest.</param>
		public bool TryGetManifest(string themeId, [NotNullWhen(true)] out ThemeManifest? manifest) {
			if (Themes.TryGetValue(themeId, out var tdata)) {
				manifest = tdata.Manifest;
				return true;
			}

			manifest = null;
			return false;
		}

		#endregion

		#region Theme Selection

		/// <summary>
		/// The currently selected theme's ID. This value may be <c>automatic</c>,
		/// and thus should not be used for checking which theme is active.
		/// </summary>
		public string? SelectedThemeId { get; private set; }

		/// <summary>
		/// Select a new theme, and possibly emit a <see cref="ThemeChanged"/>
		/// event if doing so has changed the active theme.
		/// </summary>
		/// <param name="themeId">The ID of the theme to select</param>
		/// <param name="postReload">Force ThemeManager to invalidate all
		/// cached resources and emit an event, even if the active theme
		/// didn't change.</param>
		public void SelectTheme(string? themeId, bool postReload = false) {
			if (string.IsNullOrEmpty(themeId))
				themeId = "automatic";

			string old_active = ActiveThemeId;
			var old_data = BaseThemeData;

			// Deal with the default theme quickly.
			if (themeId.Equals("default", StringComparison.OrdinalIgnoreCase)) {
				BaseThemeData = null;
				SelectedThemeId = "default";
				ActiveThemeId = "default";
			}

			// Does this string match something?
			else if (!themeId.Equals("automatic", StringComparison.OrdinalIgnoreCase) && Themes.TryGetValue(themeId, out var theme)) {
				BaseThemeData = theme;
				SelectedThemeId = themeId;
				ActiveThemeId = themeId;
			}

			// Determine the best theme
			else {
				ActiveThemeId = "default";
				BaseThemeData = null;

				string[] ids = Themes.Keys.ToArray();
				for (int i = ids.Length - 1; i >= 0; i--) {
					if (!Themes.TryGetValue(ids[i], out var themeData))
						continue;

					if (themeData.Manifest.MatchesForAutomatic(Mod.Helper.ModRegistry)) {
						BaseThemeData = themeData;
						ActiveThemeId = ids[i];
						break;
					}
				}

				SelectedThemeId = "automatic";
			}

			Log($"Selected Theme: {SelectedThemeId} => {GetThemeName(ActiveThemeId)} ({ActiveThemeId})", LogLevel.Trace);

			// Did the active theme actually change?
			if (ActiveThemeId != old_active || postReload) {
				// Invalidate old resources to kick them out of memory when
				// we're no longer using that theme. If this is being called
				// after a theme reload, however, invalidate everything in
				// case other themes have been manually used.
				Invalidate(postReload ? null : old_active);

				// And emit our event.
				ThemeChanged?.Invoke(this, new(old_active, old_data?.Data, ActiveThemeId, Theme));
			}
		}

		#endregion

		#region Default / Active Theme Access

		/// <summary>
		/// An instance of <typeparamref name="DataT"/> to be used when no
		/// specific theme is loaded and active. If you assign a new value to
		/// this property, a theme change event may be emitted if the currently
		/// active theme is the default theme.
		/// </summary>
		public DataT DefaultTheme {
			get => _DefaultTheme;
			set {
				bool is_default = ActiveThemeId == "default";
				DataT? oldData = Theme;
				_DefaultTheme = value ?? new DataT();
				if (is_default)
					ThemeChanged?.Invoke(this, new ThemeChangedEventArgs<DataT>("default", oldData, "default", _DefaultTheme));
			}
		}

		/// <summary>
		/// The currently active theme's ID. This value may be <c>default</c>, or
		/// the unique ID of a loaded theme. It will never be <c>automatic</c>.
		/// </summary>
		public string ActiveThemeId { get; private set; } = "default";

		/// <summary>
		/// The currently active theme's manifest. This value may be <c>null</c>
		/// if the active theme is the <c>default</c> theme.
		/// </summary>
		public ThemeManifest? ActiveThemeManifest => BaseThemeData?.Manifest;

		/// <summary>
		/// The currently active theme's <typeparamref name="DataT"/> instance.
		/// If the active theme is <c>default</c>, then this will be the same
		/// as the value of <see cref="DefaultTheme"/>.
		/// </summary>
		public DataT ActiveTheme => BaseThemeData?.Data ?? _DefaultTheme;

		/// <summary>
		/// An alternative to <see cref="ActiveTheme"/>. The currently active theme's
		/// <typeparamref name="DataT"/> instance.
		/// </summary>
		public DataT Theme => BaseThemeData?.Data ?? _DefaultTheme;

		/// <summary>
		/// This event is fired whenever the currently active theme changes, which
		/// can happen either when themes are reloaded or when the user changes
		/// their selected theme.
		/// </summary>
		public event EventHandler<ThemeChangedEventArgs<DataT>>? ThemeChanged;

		#endregion

		#region Asset Loading

		/// <summary>
		/// Invalidate all content files that we provide via
		/// <see cref="IModEvents.Content.AssetRequested"/>.
		/// </summary>
		/// <param name="themeId">An optional theme ID to only clear tha
		/// theme's assets.</param>
		public void Invalidate(string? themeId = null) {
			string key = AssetLoaderPrefix;
			if (!string.IsNullOrEmpty(themeId))
				key = PathUtilities.NormalizeAssetName(Path.Join(AssetLoaderPrefix, themeId));

#if THEME_MANAGER_PRE_314
			Mod.Helper.Content.InvalidateCache(info => info.AssetName.StartsWith(key));
#else
			Mod.Helper.GameContent.InvalidateCache(info => info.Name.StartsWith(key));
#endif
		}

		/// <summary>
		/// Load content from a theme (if not already cached), and return it.
		/// Depending on the asset redirection status and whether or not the
		/// requested file is present in the active theme, this method may
		/// use <see cref="IModContentHelper.Load{T}(string)"/>
		/// or <see cref="IGameContentHelper.Load{T}(string)"/>.
		/// </summary>
		/// <typeparam name="T">The expected data type.</typeparam>
		/// <param name="path">The relative file path.</param>
		/// <param name="themeId">Load the resource from the specified theme rather than the active theme.</param>
		/// <exception cref="System.ArgumentException">The <paramref name="path"/> is empty or contains invalid characters.</exception>
		/// <exception cref="Microsoft.Xna.Framework.Content.ContentLoadException">The content asset couldn't be loaded (e.g. because it doesn't exist).</exception>
		public T Load<T>(string path, string? themeId = null) where T : notnull {
			// Either check the specified theme, or the active theme.
			Theme<DataT>? theme;
			if (themeId != null)
				Themes.TryGetValue(themeId, out theme);
			else {
				themeId = ActiveThemeId;
				theme = BaseThemeData;
			}

			// Does this theme have this file?
			if (theme is not null && !HasFile(path, themeId, false, false)) {
				// If not, does the fallback theme have it? If so, then load it.
				if (!string.IsNullOrEmpty(theme.Manifest.FallbackTheme) && HasFile(path, theme.Manifest.FallbackTheme, false, false))
					return Load<T>(path, theme.Manifest.FallbackTheme);
			}

			// Just go straight to InternalLoad if we're not redirecting this
			// asset in some way. We only redirect if redirection is enabled
			// and this isn't the default theme and this theme doesn't prevent
			// asset redirection.
			bool redirect = theme?.Manifest?.OverrideRedirection ?? (theme != null && UsingAssetRedirection);
			if (!redirect)
				return InternalLoad<T>(path, themeId);

			string assetPath = PathUtilities.NormalizeAssetName(Path.Join(AssetLoaderPrefix, themeId, path));

			// We might not actually need this, depending on how the asset load
			// works out. It might already be cached, too. Don't actually
			// perform a load, just store a method that will perform a load.
			lock ((LoadingAssets as ICollection).SyncRoot) {
				LoadingAssets[assetPath] = () => InternalLoad<T>(path, themeId);
			}

			try {
#if THEME_MANAGER_PRE_314
				return Mod.Helper.Content.Load<T>(assetPath, ContentSource.GameContent);
#else
				return Mod.Helper.GameContent.Load<T>(assetPath);
#endif
			} finally {
				lock ((LoadingAssets as ICollection).SyncRoot) {
					LoadingAssets.Remove(assetPath);
				}
			}
		}

		/// <summary>
		/// Get whether a given file exists in the active theme or in the
		/// default theme (aka your mod's assets directory).
		/// </summary>
		/// <param name="path">The relative file path.</param>
		/// <param name="themeId">Check for the resource in the specified theme rather than the active theme.</param>
		/// <param name="useFallback">Whether to check for the file in the theme's fallback theme if one is set.</param>
		/// <param name="useDefault">Whether to check for the file in the default theme if it's not present in the specified theme.</param>
		public bool HasFile(string path, string? themeId = null, bool useFallback = true, bool useDefault = true) {
			// Either check the specified theme, or the active theme.
			Theme<DataT>? theme;
			if (themeId != null)
				Themes.TryGetValue(themeId, out theme);
			else
				theme = BaseThemeData;

			if (theme != null) {
				string lpath = path;
				if (!string.IsNullOrEmpty(theme.Manifest.AssetPrefix))
					lpath = Path.Join(theme.Manifest.AssetPrefix, lpath);

				if (!string.IsNullOrEmpty(theme.RelativePath))
					lpath = Path.Join(theme.RelativePath, lpath);

				if (theme.Content.HasFile(lpath))
					return true;

				// Only fall-back once when using a fallback theme.
				if (useFallback && !string.IsNullOrEmpty(theme.Manifest.FallbackTheme) && HasFile(path, theme.Manifest.FallbackTheme, false, false))
					return true;
			}

			if (!useDefault)
				return false;

			// Now check the default theme.
			if (!string.IsNullOrEmpty(DefaultAssetPrefix))
				path = Path.Join(DefaultAssetPrefix, path);

			string full = Path.Join(Mod.Helper.DirectoryPath, path);
			return File.Exists(full);
		}

		/// <summary>
		/// Actually load the asset, either from the active theme or from the
		/// default theme (aka your mod's assets directory).
		/// </summary>
		/// <typeparam name="T">The expected data type.</typeparam>
		/// <param name="path">The relative file path.</param>
		/// <param name="themeId">Load the resource from the specified theme rather than the active theme.</param>
		/// <exception cref="System.ArgumentException">The key is empty or contains invalid characters.</exception>
		/// <exception cref="Microsoft.Xna.Framework.Content.ContentLoadException">The content asset couldn't be loaded (e.g. because it doesn't exist).</exception>
		private T InternalLoad<T>(string path, string? themeId = null) where T : notnull {
			// Either check the specified theme, or the active theme.
			Theme<DataT>? theme;
			if (themeId != null)
				Themes.TryGetValue(themeId, out theme);
			else {
				themeId = ActiveThemeId;
				theme = BaseThemeData;
			}

			// If we have a theme, then try loading from it.
			if (theme != null) {
				string lpath = path;
				if (!string.IsNullOrEmpty(theme.Manifest.AssetPrefix))
					lpath = Path.Join(theme.Manifest.AssetPrefix, lpath);

				if (!string.IsNullOrEmpty(theme.RelativePath))
					lpath = Path.Join(theme.RelativePath, lpath);

				if (theme.Content.HasFile(lpath))
					try {
#if THEME_MANAGER_PRE_314
						return theme.Content.LoadAsset<T>(lpath);
#else
						return theme.Content.ModContent.Load<T>(lpath);
#endif
					} catch (Exception ex) {
						Log($"Failed to load asset \"{path}\" from theme {themeId}.", LogLevel.Warn, ex);
					}
			}

			// Now fallback to the default theme
			if (!string.IsNullOrEmpty(DefaultAssetPrefix))
				path = Path.Join(DefaultAssetPrefix, path);

#if THEME_MANAGER_PRE_314
			return Mod.Helper.Content.Load<T>(path);
#else
			return Mod.Helper.ModContent.Load<T>(path);
#endif
		}

		#endregion

		#region AssetRequested Handling

#if THEME_MANAGER_PRE_314

		/// <summary>
		/// ThemeAssetLoader is a simple <see cref="IAssetLoader"/> used to
		/// load assets from a theme as a game asset, thus allowing
		/// Content Patcher to modify the file.
		/// </summary>
		public class ThemeAssetLoader : IAssetLoader
		{
			private readonly ThemeManager<DataT> Manager;

			public ThemeAssetLoader(ThemeManager<DataT> manager)
			{
				Manager = manager;
			}

			public bool CanLoad<T>(IAssetInfo asset)
			{
				// We can only load our own assets.
				if (!asset.AssetName.StartsWith(Manager.AssetLoaderPrefix))
					return false;

				// We only load assets that we know we're trying to load.
				return Manager.LoadingAssets.ContainsKey(asset.AssetName);
			}

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8603 // Possible null reference return.
			public T Load<T>(IAssetInfo asset)
			{
				// We can only load our own assets, that we're trying to load.
				if (!asset.AssetName.StartsWith(Manager.AssetLoaderPrefix) || ! Manager.LoadingAssets.ContainsKey(asset.AssetName))
					return (T)(object)null;

				return (T)Manager.LoadingAssets[asset.AssetName]();
			}
#pragma warning restore CS8603 // Possible null reference return.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
		}

#else

		private void OnAssetRequested(object? sender, AssetRequestedEventArgs e) {
			if (e.Name.StartsWith(AssetLoaderPrefix)) {
				// We "load" assets from a dictionary where we store loaded
				// assets temporarily. So if we don't have a matching entry
				// then just return and don't load anything.
				if (!LoadingAssets.ContainsKey(e.Name.BaseName))
					return;

				e.LoadFrom(
					LoadingAssets[e.Name.BaseName],
					priority: AssetLoadPriority.Low
				);
			}
		}

#endif

		#endregion
	}
}
