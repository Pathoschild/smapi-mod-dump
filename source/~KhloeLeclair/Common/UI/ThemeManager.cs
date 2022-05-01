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
#define THEME_MANAGER_PRE_314
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

namespace Leclair.Stardew.Common.UI;

#if DEBUG
internal class UpdateData
{
	public string? Version { get; set; }
}
#endif

/// <summary>
/// BaseThemeData represents the barest possible set of theme data, only
/// including properties that ThemeManager itself uses.
///
/// You should subclass BaseThemeData for your own mod, adding any
/// appropriate values that you would like to be controllable via theme.
///
/// If you need an example of what sort of data you could add to a theme,
/// my mod <see href="https://www.nexusmods.com/stardewvalley/mods/11022">Almanac</see>
/// makes extensive use of themes to customize how its own GUI draws,
/// with custom margins on elements and colors. You can find its theme
/// class <see href="https://github.com/KhloeLeclair/StardewMods/blob/main/Almanac/Models/Theme.cs">here</see>.
/// </summary>
public class BaseThemeData
{
	/// <summary>
	/// LocalizedNames is a mapping of locale codes to human-readable theme
	/// names suitable for display in a theme selector.
	/// </summary>
	public Dictionary<string, string>? LocalizedNames { get; set; }

	/// <summary>
	/// For is a list of mod UniqueIDs that this theme is meant to support.
	/// When the current theme is set to Automatic, this theme will be
	/// selected if one of the mods listed is loaded.
	/// </summary>
	public string[]? For { get; set; }

	/// <summary>
	/// AssetPrefix is prepended to all asset file paths when loading
	/// assets from this theme.
	/// </summary>
	public string AssetPrefix { get; set; } = "assets";

	/// <summary>
	/// When <see cref="PreventRedirection"/> is enabled, assets for this
	/// theme will not be loaded through the game's content loading
	/// pipeline and Content Patcher will <b>not</b> be able to affect them.
	/// </summary>
	public bool PreventRedirection { get; set; } = false;

	#region Methods

	/// <summary>
	/// Check to see if any loaded mods are in this theme's For block.
	/// </summary>
	/// <param name="registry">Your own mod's IModRegistry helper</param>
	/// <returns>true if there are any matching mods</returns>
	public bool HasMatchingMod(IModRegistry registry)
	{
		if (For != null)
			foreach (string mod in For)
			{
				if (!string.IsNullOrEmpty(mod) && registry.IsLoaded(mod))
					return true;
			}

		return false;
	}

	#endregion
}

/// <summary>
/// This event is emitted by ThemeManager whenever the current theme
/// changes, whether because the themes were reload or because the user
/// selected a different theme.
/// </summary>
/// <typeparam name="DataT">Your mod's BaseThemeData subclass</typeparam>
public class ThemeChangedEventArgs<DataT> : EventArgs where DataT : BaseThemeData
{

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

	public ThemeChangedEventArgs(string oldId, DataT? oldData, string newID, DataT newData)
	{
		OldId = oldId;
		NewId = newID;
		OldData = oldData;
		NewData = newData;
	}
}

/// <summary>
/// SimpleManifest is a basic class with manifest properties we support
/// loading from theme.json files when reading themes from your mod's
/// "assets/themes" directory.
///
/// These values, if present, are used when constructing the temporary
/// content pack. None of these are required.
/// </summary>
internal class SimpleManifest
{
	public string? UniqueID { get; set; }
	public string? Name { get; set; }
	public string? Description { get; set; }
	public string? Author { get; set; }
	public string? Version { get; set; }
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
	IContentPack Content
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
/// <typeparam name="DataT">Your mod's BaseThemeData subclass</typeparam>
public class ThemeManager<DataT> where DataT : BaseThemeData, new()
{

	public static readonly SemanticVersion Version = new("2.0.0");

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

	#endregion

	#region Public Fields

#if THEME_MANAGER_PRE_314
	/// <summary>
	/// The <c>Loader</c> is a private <see cref="IAssetLoader"/> that we
	/// use for redirecting asset loading through game content, allowing
	/// Content Patcher (and other mods) to modify them.
	/// </summary>
	public readonly ThemeAssetLoader Loader;
#endif

	/// <summary>
	/// The AssetLoaderPrefix is prepended to asset names when redirecting
	/// asset loading through game content to allow Content Patcher access
	/// to your mod's themed resources.
	/// </summary>
	public readonly string AssetLoaderPrefix;

	/// <summary>
	/// Whether or not we are using IAssetLoader to load assets in a way
	/// that Content Patcher can intercept.
	/// </summary>
	public readonly bool UsingAssetRedirection;

	/// <summary>
	/// The EmbeddedThemesPath is the relative path to where your mod keeps
	/// its embedded themes. By default, this is <c>assets/themes</c>.
	/// </summary>
	public readonly string? EmbeddedThemesPath;

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
	/// <param name="forceAssetRedirection">If set to <c>true</c>, we will
	/// use redirected asset loading even if Content Patcher is not
	/// present in the current environment. If set to <c>false</c>, we will
	/// never use redirected asset loading even if Content Patcher is
	/// present. Leave it at <c>null</c> for the default behavior.
	/// <seealso cref="UsingAssetRedirection"/></param>
	public ThemeManager(
		Mod mod,
		string selectedThemeId = "automatic",
		DataT? defaultTheme = null,
		string? embeddedThemesPath = "assets/themes",
		string? assetPrefix = "assets",
		string? assetLoaderPrefix = null,
		bool? forceAssetRedirection = null
	)
	{
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
	/// An instance of <typeparamref name="DataT"/> for use when no
	/// specific theme is loaded. When assigning a new object to this
	/// property, a theme changed event may be emitted if the default
	/// theme is the active theme.
	/// </summary>
	public DataT DefaultTheme
	{
		get => _DefaultTheme;
		set
		{
			bool is_default = ActiveThemeId == "default";
			DataT? oldData = Theme;
			_DefaultTheme = value ?? new DataT();
			if (is_default)
				ThemeChanged?.Invoke(this, new ThemeChangedEventArgs<DataT>("default", oldData, "default", _DefaultTheme));
		}
	}

	/// <summary>
	/// The I18nPrefix is prepended to translation keys generated by
	/// ThemeManager. Currently, the only keys ThemeManager generates are
	/// for the Automatic and Default themes.
	/// </summary>
	public string I18nPrefix { get; set; } = "theme";

	/// <summary>
	/// The currently selected theme's ID. This value may be <c>automatic</c>,
	/// and thus should not be used for checking which theme is active.
	/// </summary>
	public string? SelectedThemeId { get; private set; } = null;

	/// <summary>
	/// The currently active theme's ID. This value will never be
	/// <c>automatic</c>. It may be <c>default</c>, or the unique ID of a theme.
	/// </summary>
	public string ActiveThemeId { get; private set; } = "default";

	/// <summary>
	/// The currently active theme's <typeparamref name="DataT"/> instance.
	/// If the active theme is <c>default</c>, then this will be the value
	/// of <see cref="DefaultTheme"/>.
	/// </summary>
	public DataT Theme => BaseThemeData?.Data ?? _DefaultTheme;

	/// <summary>
	/// This event is fired whenever the currently active theme changes,
	/// which can happen either when themes are reloaded or when the user
	/// changes their selected theme.
	/// </summary>
	public event EventHandler<ThemeChangedEventArgs<DataT>>? ThemeChanged;

	/// <summary>
	/// The DefaultAssetPrefix is prepended to asset paths when loading
	/// assets from the default theme. This prefix is not added to paths
	/// when loading assets from other themes, as themes have their own
	/// AssetPrefix to use.
	/// </summary>
	public string? DefaultAssetPrefix { get; private set; }

	#endregion

	#region Logging

	private void Log(string message, LogLevel level = LogLevel.Trace, Exception? ex = null, LogLevel? exLevel = null)
	{
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
				using WebClient client = new WebClient();

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
	public void PerformReloadCommand()
	{
		Invalidate();
		Discover();
		Log($"Reloaded themes. You may need to reopen menus.", LogLevel.Info);
	}

	[SuppressMessage("Style", "IDE0060", Justification = "Provided for ease of use with SMAPI's API.")]
	public void PerformReloadCommand(string name, string[] args)
	{
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
	public bool PerformThemeCommand(string[] args)
	{
		if (args.Length > 0 && !string.IsNullOrEmpty(args[0]))
		{
			string key = args[0].Trim();

			// Check for reload first.
			if (key.Equals("reload", StringComparison.OrdinalIgnoreCase))
			{
				PerformReloadCommand();
				return false;
			}

			string? selected = null;
			var themes = GetThemeChoices();

			// Check for unique ID matches first.
			foreach (var pair in themes)
			{
				if (pair.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
				{
					selected = pair.Key;
					break;
				}
			}

			// Then check for unique ID partial matches.
			if (selected == null)
				foreach (var pair in themes)
				{
					if (pair.Key.Contains(key, StringComparison.OrdinalIgnoreCase))
					{
						selected = pair.Key;
						break;
					}
				}

			// Then select for display name partial matches.
			if (selected == null)
				foreach (var pair in themes)
				{
					if (GetThemeName(pair.Key).Contains(key, StringComparison.OrdinalIgnoreCase))
					{
						selected = pair.Key;
						break;
					}
				}

			// If we've selected something, actually select it.
			if (selected != null)
			{
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
		foreach (var pair in GetThemeChoices())
		{
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

	#region Theme Data Access

	/// <summary>
	/// Get the human readable name of a theme, optionally in a given
	/// locale. If no locale is given, the current locale will be read
	/// from your mod's translation helper.
	/// </summary>
	/// <param name="themeId">The ID of theme to get the name for</param>
	/// <param name="locale">The locale to get the name in, if it exists.</param>
	public string GetThemeName(string themeId, string? locale = null)
	{
		// For the default theme, return a translation from the host mod's
		// translation layer.
		if (themeId.Equals("default", StringComparison.OrdinalIgnoreCase))
			return Mod.Helper.Translation.Get($"{I18nPrefix}.default").ToString();

		// Get the theme data. If the theme is the active theme, don't
		// bother with a dictionary lookp.
		Theme<DataT>? theme;
		if (themeId == ActiveThemeId)
			theme = BaseThemeData;
		else
		{
			lock ((Themes as ICollection).SyncRoot)
			{
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

		if (theme.Data.LocalizedNames?.TryGetValue(locale, out string? name) ?? false && name != null)
			return name;

		// Manifest Name
		return theme.Content.Manifest.Name;
	}

	/// <summary>
	/// Get an enumeration of the available themes, suitable for display in
	/// a configuration menu such as Generic Mod Config Menu. This list will
	/// always include an <c>automatic</c> and <c>default</c> entry.
	///
	/// The keys are theme IDs, and the values are human readable names in
	/// the current locale.
	/// </summary>
	/// <param name="locale">The locale to get names for</param>
	/// <returns>An enumeration of available themes</returns>
	public Dictionary<string, string> GetThemeChoices(string? locale = null)
	{
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
	/// The keys are theme IDs, and the values are methods that, when called,
	/// return a human readable name for the theme (in the current locale is
	/// a translation is available).
	/// </summary>
	/// <returns>An enumeration of available themes</returns>
	public Dictionary<string, Func<string>> GetThemeChoiceMethods()
	{
		Dictionary<string, Func<string>> result = new();

		result.Add("automatic", () => Mod.Helper.Translation.Get($"{I18nPrefix}.automatic"));
		result.Add("default", () => Mod.Helper.Translation.Get($"{I18nPrefix}.default"));

		foreach (string theme in Themes.Keys)
			result.Add(theme, () => GetThemeName(theme));

		return result;
	}

	/// <summary>
	/// Check to see if a given theme has been loaded.
	/// </summary>
	/// <param name="themeId">The theme we want to check.</param>
	public bool HasTheme(string themeId)
	{
		return Themes.ContainsKey(themeId);
	}

	/// <summary>
	/// Get the theme data for a specific theme. As this method involves a
	/// dictionary lookup, you might want to cache the result of this and
	/// update it when a <see cref="ThemeChanged"/> event is emitted.
	/// </summary>
	/// <param name="themeId">The theme we want data for.</param>
	public DataT? GetTheme(string themeId)
	{
		if (Themes.TryGetValue(themeId, out var theme))
			return theme?.Data;
		return null;
	}

	#endregion

	#region Theme Discovery

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
	)
	{
		lock ((Themes as ICollection).SyncRoot)
		{
			// Start by wiping the existing theme data.
			Themes.Clear();

			// We want to keep track of packs with custom IDs so that we
			// can use better IDs for embedded packs.
			Dictionary<string, Tuple<string?, IContentPack>> packsWithIds = new();

			// If we haven't been forbidden, check for embedded themes and
			// add them to our packs. We do this first so that any content
			// pack themes can override our embedded themes, if they really
			// need to. They shouldn't, however.
			if (checkEmbedded)
			{
				var embedded = FindEmbeddedThemes();
				if (embedded != null)
					foreach (var cp in embedded)
						packsWithIds[cp.Key] = new(null, cp.Value);
			}

			// Now, check for your mod's owned content packs.
			if (checkOwned)
			{
				var owned = Mod.Helper.ContentPacks.GetOwned();
				if (owned != null)
					foreach (var cp in owned)
						packsWithIds[cp.Manifest.UniqueID] = new(null, cp);
			}

			// Finally, check for external mods that provide themes.
			if (checkExternal)
			{
				var external = FindExternalThemes();
				if (external != null)
					foreach (var cp in external)
						packsWithIds[cp.Key] = cp.Value;
			}

			// Now, load each of our packs.
			foreach (var cp in packsWithIds)
			{
				string file = string.IsNullOrEmpty(cp.Value.Item1) ? "theme.json" : cp.Value.Item1;
				if (!cp.Value.Item2.HasFile(file))
					continue;

				DataT? data;
				try
				{
					data = cp.Value.Item2.ReadJsonFile<DataT>(file);
					if (data is null)
						throw new Exception("theme.json cannot be null");
				}
				catch (Exception ex)
				{
					Log($"The content pack {cp.Value.Item2.Manifest.Name} has an invalid theme json file and could not be loaded.", LogLevel.Warn, ex);
					continue;
				}

				Themes[cp.Key] = new(data, cp.Value.Item2);
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
	private Dictionary<string, IContentPack>? FindEmbeddedThemes()
	{
		if (string.IsNullOrEmpty(EmbeddedThemesPath))
			return null;

		string path = Path.Join(Mod.Helper.DirectoryPath, PathUtilities.NormalizePath(EmbeddedThemesPath));
		if (!Directory.Exists(path))
			return null;

		Dictionary<string, IContentPack> results = new();
		int count = 0;

		// Start iterating subdirectories of our embedded themes folder.
		foreach (string dir in Directory.GetDirectories(path))
		{
			string man_path = Path.Join(dir, "manifest.json");
			string t_path = Path.Join(dir, "theme.json");

			// If the subdirectory has no theme.json, ignore it.
			if (!File.Exists(t_path))
				continue;

			string rel_path = Path.GetRelativePath(Mod.Helper.DirectoryPath, dir);
			string folder = Path.GetRelativePath(path, dir);

			Log($"Found Embedded Theme At: {dir}", LogLevel.Trace);

			SimpleManifest? manifest = null;
			try
			{
				if (File.Exists(man_path))
					manifest = Mod.Helper.Data.ReadJsonFile<SimpleManifest>(
						Path.Join(rel_path, "manifest.json"));
				else
					manifest = Mod.Helper.Data.ReadJsonFile<SimpleManifest>(
						Path.Join(rel_path, "theme.json"));

				if (manifest is null)
					throw new Exception("manifest is empty or invalid");
			}
			catch (Exception ex)
			{
				Log($"Unable to read embedded theme manifest.", LogLevel.Warn, ex);
				continue;
			}

			// TODO: Slugify the folder name.

			var cp = Mod.Helper.ContentPacks.CreateTemporary(
				directoryPath: dir,
				id: manifest.UniqueID ?? $"{Mod.ModManifest.UniqueID}.theme.{folder}",
				name: manifest.Name ?? folder,
				description: manifest.Description ?? $"{Mod.ModManifest.Name} Theme: {manifest.Name}",
				author: manifest.Author ?? Mod.ModManifest.Author,
				version: new SemanticVersion(manifest.Version ?? "1.0.0")
			);

			results[manifest.UniqueID ?? folder] = cp;
			count++;

			Log($"Found Embedded Theme: {cp.Manifest.Name} by {cp.Manifest.Author} ({cp.Manifest.UniqueID})", LogLevel.Trace);
		}

		Log($"Found {count} Embedded Themes.", LogLevel.Trace);
		return results;
	}

	/// <summary>
	/// Search for themes in other mods' manifests.
	/// </summary>
	/// <returns>A dictionary of temporary IContentPacks for each discovered theme.</returns>
	private Dictionary<string, Tuple<string?, IContentPack>> FindExternalThemes()
	{
		Dictionary<string, Tuple<string?, IContentPack>> results = new();
		int count = 0;

		string themeKey = $"{Mod.ModManifest.UniqueID}:theme";
		string themeFile = $"{Mod.ModManifest.UniqueID}.theme.json";

		foreach (var mod in Mod.Helper.ModRegistry.GetAll())
		{
			// For every mod, try reading a special value from its manifest.
			if (mod?.Manifest?.ExtraFields == null)
				continue;

			if (!mod.Manifest.ExtraFields.TryGetValue(themeKey, out object? value))
				continue;

			string file = themeFile;

			// If the value is a boolean, and false for some reason,
			// just skip the mod. If it's true, assume the default
			// filename for our theme JSON.
			if (value is bool bv)
			{
				if (!bv)
					continue;

				// If the value is a string, use that string as a relative
				// filename for the theme.json file, relative to that mod's
				// directory root.
			}
			else if (value is string str)
			{
				file = str;

				// Display a warning for any other value, as we can only handle
				// strings and booleans.
			}
			else
			{
				Log($"Unknown or unsupported value for {themeKey} in mod {mod.Manifest.Name} ({mod.Manifest.UniqueID})", LogLevel.Warn);
				continue;
			}

			// We need to know the root path of this other mod.
			string root;
			try
			{
				if (mod.GetType().GetProperty("DirectoryPath", BindingFlags.Instance | BindingFlags.Public)?.GetValue(mod) is string str)
					root = str;
				else
					throw new ArgumentException("DirectoryPath");

			}
			catch (Exception)
			{
				Log("Unable to get mod directories from SMAPI internals. Disabling theme detection in other mods.", LogLevel.Warn);
				break;
			}

			// Get the full path to the theme file.
			string full_file = Path.Join(root, PathUtilities.NormalizePath(file));

			// Does the file exist?
			if (!File.Exists(full_file))
			{
				Log($"Unable to find {file} in mod {mod.Manifest.Name} ({mod.Manifest.UniqueID})", LogLevel.Warn);
				continue;
			}

			// ... and get the file again, from the joined path. We don't
			// want any of the directory part.
			file = Path.GetFileName(full_file);

			string? folder = Path.GetDirectoryName(full_file);
			if (folder is null)
			{
				Log($"Unable to get directory from path \"{full_file}\". Skipping theme.", LogLevel.Warn);
				continue;
			}

			// Build a temporary content pack for just our theme within
			// this other mod.
			var cp = Mod.Helper.ContentPacks.CreateTemporary(
				directoryPath: folder,
				id: $"{Mod.ModManifest.UniqueID}.theme.{mod.Manifest.UniqueID}",
				name: mod.Manifest.Name,
				description: mod.Manifest.Description,
				author: mod.Manifest.Author,
				version: mod.Manifest.Version
			);

			results[mod.Manifest.UniqueID] = new(file, cp);
			count++;
		}

		Log($"Found {count} External Themes");
		return results;
	}

	#endregion

	#region Select Theme

	/// <summary>
	/// Select a new theme, and possibly emit a <see cref="ThemeChanged"/>
	/// event if doing so has changed the active theme.
	/// </summary>
	/// <param name="themeId">The ID of the theme to select</param>
	/// <param name="postReload">Force ThemeManager to invalidate all
	/// cached resources and emit an event, even if the active theme
	/// didn't change.</param>
	public void SelectTheme(string? themeId, bool postReload = false)
	{
		if (string.IsNullOrEmpty(themeId))
			themeId = "automatic";

		string old_active = ActiveThemeId;
		var old_data = BaseThemeData;

		// Deal with the default theme quickly.
		if (themeId.Equals("default", StringComparison.OrdinalIgnoreCase))
		{
			BaseThemeData = null;
			SelectedThemeId = "default";
			ActiveThemeId = "default";
		}

		// Does this string match something?
		else if (!themeId.Equals("automatic", StringComparison.OrdinalIgnoreCase) && Themes.TryGetValue(themeId, out var theme))
		{
			BaseThemeData = theme;
			SelectedThemeId = themeId;
			ActiveThemeId = themeId;
		}

		// Determine the best theme
		else
		{
			ActiveThemeId = "default";
			BaseThemeData = null;

			string[] ids = Themes.Keys.ToArray();
			for (int i = ids.Length - 1; i >= 0; i--)
			{
				if (!Themes.TryGetValue(ids[i], out var themeData))
					continue;

				if (themeData.Data?.HasMatchingMod(Mod.Helper.ModRegistry) ?? false)
				{
					BaseThemeData = themeData;
					ActiveThemeId = ids[i];
					break;
				}
			}

			SelectedThemeId = "automatic";
		}

		Log($"Selected Theme: {SelectedThemeId} => {GetThemeName(ActiveThemeId)} ({ActiveThemeId})", LogLevel.Trace);

		// Did the active theme actually change?
		if (ActiveThemeId != old_active || postReload)
		{
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

	#region Resource Loading

	/// <summary>
	/// Invalidate all content files that we provide via
	/// <see cref="IModEvents.Content.AssetRequested"/>.
	/// </summary>
	/// <param name="themeId">An optional theme ID to only clear tha
	/// theme's assets.</param>
	public void Invalidate(string? themeId = null)
	{
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
	public T Load<T>(string path, string? themeId = null) where T : notnull
	{
		// Either check the specified theme, or the active theme.
		Theme<DataT>? theme;
		if (themeId != null)
			Themes.TryGetValue(themeId, out theme);
		else
		{
			themeId = ActiveThemeId;
			theme = BaseThemeData;
		}

		// Just go straight to InternalLoad if we're not redirecting this
		// asset in some way. We only redirect if redirection is enabled
		// and this isn't the default theme and this theme doesn't prevent
		// asset redirection.
		if (!UsingAssetRedirection || theme == null || theme.Data.PreventRedirection)
			return InternalLoad<T>(path, themeId);

		string assetPath = PathUtilities.NormalizeAssetName(Path.Join(AssetLoaderPrefix, themeId, path));

		// We might not actually need this, depending on how the asset load
		// works out. It might already be cached, too. Don't actually
		// perform a load, just store a method that will perform a load.
		lock ((LoadingAssets as ICollection).SyncRoot)
		{
			LoadingAssets[assetPath] = () => InternalLoad<T>(path, themeId);
		}

		try
		{
#if THEME_MANAGER_PRE_314
			return Mod.Helper.Content.Load<T>(assetPath, ContentSource.GameContent);
#else
			return Mod.Helper.GameContent.Load<T>(assetPath);
#endif
		}
		finally
		{
			lock ((LoadingAssets as ICollection).SyncRoot)
			{
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
	public bool HasFile(string path, string? themeId = null)
	{
		// Either check the specified theme, or the active theme.
		Theme<DataT>? theme;
		if (themeId != null)
			Themes.TryGetValue(themeId, out theme);
		else
			theme = BaseThemeData;

		if (theme != null)
		{
			string lpath = path;
			if (!string.IsNullOrEmpty(theme.Data.AssetPrefix))
				lpath = Path.Join(theme.Data.AssetPrefix, lpath);

			if (theme.Content.HasFile(lpath))
				return true;
		}

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
	private T InternalLoad<T>(string path, string? themeId = null) where T : notnull
	{
		// Either check the specified theme, or the active theme.
		Theme<DataT>? theme;
		if (themeId != null)
			Themes.TryGetValue(themeId, out theme);
		else
		{
			themeId = ActiveThemeId;
			theme = BaseThemeData;
		}

		// If we have a theme, then try loading from it.
		if (theme != null)
		{
			string lpath = path;
			if (!string.IsNullOrEmpty(theme.Data.AssetPrefix))
				lpath = Path.Join(theme.Data.AssetPrefix, lpath);

			if (theme.Content.HasFile(lpath))
				try
				{
#if THEME_MANAGER_PRE_314
					return theme.Content.LoadAsset<T>(lpath);
#else
					return theme.Content.ModContent.Load<T>(lpath);
#endif
				}
				catch (Exception ex)
				{
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

	private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
	{
		if (e.Name.StartsWith(AssetLoaderPrefix))
		{
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
