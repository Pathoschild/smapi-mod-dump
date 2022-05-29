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
using System.Diagnostics.CodeAnalysis;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;

namespace Leclair.Stardew.ThemeManager;

/// <summary>
/// This theme data represents basic colors being used by the game.
/// </summary>
public interface IBaseTheme {

	Color? TextColor { get; }

	Color? TextShadowColor { get; }

	Color? TextShadowAltColor { get; }

	Color? ErrorTextColor { get; }

	Color? HoverColor { get; }

	Color? ButtonHoverColor { get; }

	Dictionary<int, Color> SpriteTextColors { get; }
}

/// <summary>
/// This event is emitted by <see cref="ITypedThemeManager{DataT}"/> whenever the
/// current theme changes. This can happen when themes are reload, or when
/// the user selects a different theme.
/// </summary>
public interface IThemeChangedEvent<DataT> {

	/// <summary>
	/// The theme ID of the previously active theme.
	/// </summary>
	string OldId { get; }

	/// <summary>
	/// The theme ID of the newly active theme.
	/// </summary>
	string NewId { get; }

	/// <summary>
	/// The theme data of the previously active theme.
	/// </summary>
	DataT? OldData { get; }

	/// <summary>
	/// The theme data of the newly active theme.
	/// </summary>
	DataT NewData { get; }

}

/// <summary>
/// A manifest has necessary metadata for a theme for display in theme
/// selection UI, for performing automatic theme selection, and for
/// loading assets correctly from the filesystem.
/// </summary>
public interface IThemeManifest {

	#region Identification

	/// <summary>
	/// The unique ID of this theme.
	/// </summary>
	string UniqueID { get; }

	/// <summary>
	/// The name of this theme, used if no localized name is available
	/// matching the current game locale.
	/// </summary>
	string Name { get; }

	/// <summary>
	/// A mapping of locale codes to human-readable theme names, suitable
	/// for display in a user interface for selecting themes. If
	/// <see cref="TranslationKey"/> is present, this is only used if the
	/// translation does not have a value.
	/// </summary>
	IReadOnlyDictionary<string, string>? LocalizedNames { get; }

	/// <summary>
	/// A translation key to use for getting a localized name for this
	/// theme. This provides an alternative to <see cref="LocalizedNames"/>
	/// for translating themes.
	///
	/// If set, the translation is pulled from the
	/// <see cref="ITranslationHelper"/> of the mod that provides this theme.
	/// </summary>
	string? TranslationKey { get; }

	/// <summary>
	/// The manifest of the mod that provides this theme.
	/// </summary>
	IManifest ProvidingMod { get; }

	#endregion

	#region Theme Selection

	/// <summary>
	/// An array of <see cref="IManifest.UniqueID"/>s of mods that this theme
	/// is meant to support. When <see cref="ITypedThemeManager{DataT}.SelectedThemeId"/>
	/// is set to <c>automatic</c>, this theme will potentially be selected if
	/// any of the listed mods are present and loaded.
	/// </summary>
	string[]? SupportedMods { get; }

	/// <summary>
	/// An array of <see cref="IManifest.UniqueID"/>s of mods that this theme
	/// does <b>not</b> support. When <see cref="ITypedThemeManager{DataT}.SelectedThemeId"/>
	/// is set to <c>automatic</c>, this theme will never be selected if any
	/// of the listed mods are present and loaded.
	/// </summary>
	string[]? UnsupportedMods { get; }

	#endregion

	#region Asset Loading

	/// <summary>
	/// The <see cref="UniqueID"/> of another theme. If a requested asset is
	/// not available in this theme, try loading it from that other theme
	/// before falling back to loading it from the <c>default</c> theme.
	/// </summary>
	string? FallbackTheme { get; }

	/// <summary>
	/// A string that is prepended to asset paths when loading assets from
	/// this theme.
	/// </summary>
	string? AssetPrefix { get; }

	/// <summary>
	/// When set to a non-null value, assets for this theme will either be
	/// forced to use asset redirection, or forced to <b>not</b> use asset
	/// redirection, overriding the default behavior from the
	/// <see cref="ITypedThemeManager{DataT}"/> instance.
	/// </summary>
	bool? OverrideRedirection { get; }

	#endregion

}


public interface IThemeManager {

	#region Basic Values

	/// <summary>
	/// The manifest of the mod that this <see cref="ITypedThemeManager{DataT}"/>
	/// instance is supporting.
	/// </summary>
	IManifest ModManifest { get; }

	/// <summary>
	/// A string that is prepended to asset names when redirecting
	/// asset loading through game content to allow Content Patcher access
	/// to your mod's themed resources.
	/// </summary>
	string AssetLoaderPrefix { get; }

	/// <summary>
	/// The relative path to where your mod keeps its embedded themes.
	/// By default, this is <c>assets/themes</c>. If this is <c>null</c>,
	/// no embedded themes will be loaded.
	/// </summary>
	string? EmbeddedThemesPath { get; }

	/// <summary>
	/// Whether or not <see cref="ITypedThemeManager{DataT}"/> is redirecting
	/// asset loading through <see cref="IGameContentHelper"/> to allow
	/// other mods, such as Content Patcher, to modify theme assets.
	/// </summary>
	bool UsingAssetRedirection { get; }

	/// <summary>
	/// A string that is prepended to asset paths when loading assets from
	/// the <c>default</c> theme. This prefix is not added to paths when
	/// loading assets from other themes, as themes have their own
	/// <c>AssetPrefix</c> to use.
	/// </summary>
	string? DefaultAssetPrefix { get; set; }

	#endregion

	#region Theme Enumeration

	/// <summary>
	/// Get the human readable name of a theme, optionally in a given locale.
	/// If no locale is given, the game's current locale will be used.
	/// </summary>
	/// <param name="themeId">The <see cref="IThemeManifest.UniqueID"/> of the
	/// theme you want to get the name of.</param>
	/// <param name="locale">The locale to get the name in, if possible.</param>
	string GetThemeName(string themeId, string? locale = null);

	/// <summary>
	/// Get an enumeration of the available themes, suitable for display in
	/// a configuration menu such as Generic Mod Config Menu. This list will
	/// always include an <c>automatic</c> and <c>default</c> entry.
	///
	/// The keys are <see cref="IThemeManifest.UniqueID"/>s while the values
	/// are human readable names in the current (or selected) locale.
	/// </summary>
	/// <param name="locale">The locale to get names in, if possible.</param>
	IReadOnlyDictionary<string, string> GetThemeChoices(string? locale = null);

	/// <summary>
	/// Get an enumeration of the available themes, suitable for display in
	/// a configuration menu such as Generic Mod Config Menu. This list will
	/// always include an <c>automatic</c> and <c>default</c> entry.
	///
	/// The keys are <see cref="IThemeManifest.UniqueID"/>s while the values
	/// are methods returning human readable names in the current locale.
	/// </summary>
	IReadOnlyDictionary<string, Func<string>> GetThemeChoiceMethods();

	/// <summary>
	/// Get an enumeration of the manifests of every loaded theme. This will
	/// not contain an entry for the <c>default</c> theme.
	/// </summary>
	IReadOnlyCollection<IThemeManifest> GetThemeManifests();

	/// <summary>
	/// Check to see if a given theme has been loaded.
	/// </summary>
	/// <param name="themeId">The <see cref="IThemeManifest.UniqueID"/> of the
	/// theme we're interested in.</param>
	bool HasTheme(string themeId);

	/// <summary>
	/// Try to get the <see cref="IThemeManifest"/> instance for a specific
	/// theme, if it's loaded.
	/// </summary>
	/// <param name="themeId">The <see cref="IThemeManifest.UniqueID"/> of the
	/// theme we want the manifest for.</param>
	/// <param name="manifest">The <see cref="IThemeManifest"/> instance for
	/// the requested theme, or <c>null</c> if the theme is not loaded or
	/// has no manifest. Only the <c>default</c> theme has no manifest.</param>
	bool TryGetManifest(string themeId, [NotNullWhen(true)] out IThemeManifest? manifest);

	#endregion

	#region Theme Discovery

	/// <summary>
	/// Perform theme discovery, reloading all themes and updating the active
	/// theme. If the <see cref="SelectedThemeId"/> is <c>automatic</c>, this
	/// may result in the <see cref="ActiveThemeId"/> changing.
	///
	/// This method will always result in a <see cref="ThemeChanged"/> event
	/// being emitted.
	/// </summary>
	/// <param name="checkEmbedded">Whether or not to load embedded themes from
	/// your mod's own directory. If <see cref="EmbeddedThemesPath"/> is
	/// <c>null</c>, this will do nothing.</param>
	/// <param name="checkOwned">Whether or not to load themes from content
	/// packs belonging to your mod.</param>
	/// <param name="checkExternal">Whether or not to load themes from other,
	/// unrelated mods that declare a theme for your mod in their manifest.</param>
	void Discover(
		bool checkEmbedded = true,
		bool checkOwned = true,
		bool checkExternal = true
	);

	#endregion

	#region Theme Selection

	/// <summary>
	/// Select a new theme, and possibly emit a <see cref="ThemeChanged"/>
	/// event if the active theme has changed.
	/// </summary>
	/// <param name="themeId">The <see cref="IThemeManifest.UniqueID"/> of
	/// the specific theme to select, or <c>default</c> or <c>automatic</c>.</param>
	void SelectTheme(string? themeId);

	/// <summary>
	/// The currently selected theme's ID. This value may be <c>automatic</c>,
	/// and so should not be used for determining which theme is active.
	/// </summary>
	string SelectedThemeId { get; }

	#endregion

	#region Default / Active Theme Access

	/// <summary>
	/// The currently active theme's ID. This value may be <c>default</c>, or
	/// the unique ID of a loaded theme. It will never be <c>automatic</c>.
	/// </summary>
	string ActiveThemeId { get; }

	/// <summary>
	/// The currently active theme's manifest. This value may be null if
	/// the active theme is the <c>default</c> theme.
	/// </summary>
	IThemeManifest? ActiveThemeManifest { get; }

	#endregion

	#region Asset Loading

	/// <summary>
	/// Invalidate all cached assets that we provide.
	/// </summary>
	/// <param name="themeId">An optional theme ID to only clear that
	/// theme's assets.</param>
	void Invalidate(string? themeId = null);

	/// <summary>
	/// Load an asset from a theme and return it. Depending on <see cref="UsingAssetRedirection"/>
	/// and <see cref="IThemeManifest.OverrideRedirection"/>, the requested
	/// asset may be loaded from <see cref="IModContentHelper.Load{T}(string)"/>
	/// directly or be redirected through <see cref="IGameContentHelper.Load{T}(string)"/>.
	/// </summary>
	/// <typeparam name="T">The expected data type.</typeparam>
	/// <param name="path">The relative file path.</param>
	/// <param name="themeId">If set, load the asset from the specified theme
	/// rather than the active theme.</param>
	/// <exception cref="ArgumentException">The <paramref name="path"/> is empty
	/// or contains invalid characters.</exception>
	/// <exception cref="ContentLoadException">The content asset couldn't be
	/// loaded (e.g. because it doesn't exist).</exception>
	T Load<T>(string path, string? themeId = null) where T : notnull;

	/// <summary>
	/// Check whether a given file exists in a theme.
	/// </summary>
	/// <param name="path">The relative file path.</param>
	/// <param name="themeId">If set, check for the asset in the specified
	/// theme rather than the active theme.</param>
	/// <param name="useFallback">If true and the asset is not present in
	/// the theme, check for the asset in the <see cref="IThemeManifest.FallbackTheme"/>
	/// theme as well.</param>
	/// <param name="useDefault">If true and the asset is not present in
	/// the theme, check for the asset in the <c>default</c> theme as well.</param>
	bool HasFile(string path, string? themeId = null, bool useFallback = true, bool useDefault = true);

	#endregion
}


/// <summary>
/// <c>IThemeManager</c> implements a system for adding theme support to
/// Stardew Valley mods. It handles discovery, selection, asset loading,
/// Content Patcher support, and emitting events when the theme changes.
/// </summary>
/// <typeparam name="DataT">Your mod's theme data type</typeparam>
public interface ITypedThemeManager<DataT> : IThemeManager where DataT : new() {

	#region Theme Enumeration

	/// <summary>
	/// Try to get the <typeparamref name="DataT"/> instance for a specific
	/// theme, if it's loaded.
	///
	/// As this method uses a dictionary lookup internally, you should cache
	/// the result if you use it frequently for best performance. If you do
	/// cache the result, make sure to update your cache whenever the
	/// <see cref="ThemeChanged"/> event is emitted.
	/// </summary>
	/// <param name="themeId">The <see cref="IThemeManifest.UniqueID"/> of the
	/// theme we want the data instance for.</param>
	/// <param name="theme">The <typeparamref name="DataT"/> instance for
	/// the requested theme, or <c>null</c> if the theme is not loaded.</param>
	bool TryGetTheme(string themeId, [NotNullWhen(true)] out DataT? theme);

	#endregion

	#region Default / Active Theme Access

	/// <summary>
	/// An instance of <typeparamref name="DataT"/> to be used when no
	/// specific theme is loaded and active. If you assign a new value to
	/// this property, a theme change event may be emitted if the currently
	/// active theme is the default theme.
	/// </summary>
	DataT DefaultTheme { get; set; }

	/// <summary>
	/// The currently active theme's <typeparamref name="DataT"/> instance.
	/// If the active theme is <c>default</c>, then this will be the same
	/// as the value of <see cref="DefaultTheme"/>.
	/// </summary>
	DataT ActiveTheme { get; }

	/// <summary>
	/// An alternative to <see cref="ActiveTheme"/>. The currently active theme's
	/// <typeparamref name="DataT"/> instance.
	/// </summary>
	DataT Theme { get; }

	/// <summary>
	/// This event is fired whenever the currently active theme changes, which
	/// can happen either when themes are reloaded or when the user changes
	/// their selected theme.
	/// </summary>
	event EventHandler<IThemeChangedEvent<DataT>>? ThemeChanged;

	#endregion
}

public interface IThemeManagerApi {

	#region Game Themes

	IBaseTheme BaseTheme { get; }

	event EventHandler<IThemeChangedEvent<IBaseTheme>>? BaseThemeChanged;

	#endregion

	#region Custom Themes

	/// <summary>
	/// Try to get an existing <see cref="ITypedThemeManager{DataT}"/> instance
	/// for a mod. This will never create a new instance. If
	/// <typeparamref name="DataT"/> does not match the type used when
	/// creating the existing manager, this method will return <c>false</c>
	/// rather than throwing an <see cref="InvalidCastException"/>.
	/// </summary>
	/// <typeparam name="DataT">The type for the mod's theme data.</typeparam>
	/// <param name="modManifest">The mod's manifest.</param>
	/// <param name="themeManager">The <see cref="ITypedThemeManager{DataT}"/>
	/// instance, if one exists.</param>
	bool TryGetManager<DataT>(IManifest modManifest, [NotNullWhen(true)] out ITypedThemeManager<DataT>? themeManager) where DataT : class, new();

	/// <summary>
	/// Get an <see cref="ITypedThemeManager{DataT}"/> for a mod. If there is no
	/// existing instance, create a new one using the supplied parameters.
	///
	/// If there is an existing instance, the parameters are ignored.
	/// </summary>
	/// <typeparam name="DataT">The type for the mod's theme data.</typeparam>
	/// <param name="modManifest">The mod's manifest.</param>
	/// <param name="defaultTheme">A <typeparamref name="DataT"/> instance to
	/// use for the <c>default</c> theme. If one is not provided, a new
	/// instance will be created.</param>
	/// <param name="embeddedThemesPath">The relative path to search for
	/// embedded themes at. See <see cref="ITypedThemeManager{DataT}.EmbeddedThemesPath"/>.</param>
	/// <param name="assetPrefix">A string prepended to asset paths when
	/// loading assets from the <c>default</c> theme. See
	/// <see cref="ITypedThemeManager{DataT}.DefaultAssetPrefix"/>.</param>
	/// <param name="assetLoaderPrefix">A string prepended to asset names
	/// when redirecting assets through <see cref="IGameContentHelper"/>.
	/// See <see cref="ITypedThemeManager{DataT}.AssetLoaderPrefix"/>.</param>
	/// <param name="forceAssetRedirection">If set to a value, override
	/// the default behavior of <see cref="ITypedThemeManager{DataT}.UsingAssetRedirection"/>.</param>
	/// <exception cref="InvalidCastException">Thrown when attempting to get a
	/// manager with a different <typeparamref name="DataT"/> than it was
	/// created with.</exception>
	ITypedThemeManager<DataT> GetOrCreateManager<DataT>(
		IManifest modManifest,
		DataT? defaultTheme = null,
		string? embeddedThemesPath = "assets/themes",
		string? assetPrefix = "assets",
		string? assetLoaderPrefix = null,
		bool? forceAssetRedirection = null
	) where DataT : class, new();

	/// <summary>
	/// Manage a <typeparamref name="DataT"/> instance for a mod using a
	/// <see cref="ITypedThemeManager{DataT}"/>. This uses a <c>ref</c> parameter
	/// to replace the existing theme instance with a new one when the
	/// theme is changed.
	///
	/// If you need to change any of the parameters used to create a theme
	/// manager, you should first call <see cref="GetOrCreateManager{DataT}(IManifest, DataT?, string?, string?, string?, bool?)"/>
	/// before using this method.
	/// </summary>
	/// <typeparam name="DataT">The type for the mod's theme data.</typeparam>
	/// <param name="modManifest">The mod's manifest.</param>
	/// <param name="theme">The default <typeparamref name="DataT"/> instance.
	/// If the <see cref="ITypedThemeManager{DataT}"/> instance was not already
	/// created, this will be used as the <c>default</c> theme's data.</param>
	/// <param name="onThemeChanged">An optional action to be called whenever
	/// the theme is changed or reloaded.</param>
	/// <exception cref="InvalidCastException">Thrown when attempting to get a
	/// manager with a different <typeparamref name="DataT"/> than it was
	/// created with.</exception>
	ITypedThemeManager<DataT> ManageTheme<DataT>(
		IManifest modManifest,
		ref DataT theme,
		EventHandler<IThemeChangedEvent<DataT>>? onThemeChanged = null
	) where DataT : class, new();

	#endregion

	#region Color Parsing

	/// <summary>
	/// Parse a color from a string. This supports CSS hex format, CSS rgb()
	/// format, a selection of color names, and basic "[r], [g], [b], [a]"
	/// values separated by commas.
	/// </summary>
	/// <param name="value">The input string to parse</param>
	/// <param name="color">The resulting color, or null</param>
	/// <returns>Whether or not a color was successfully read.</returns>
	bool TryParseColor(string value, [NotNullWhen(true)] out Color? color);

	#endregion

	#region Colored SpriteText

	/// <summary>
	/// Draw arbitrarily-colored strings of
	/// <see cref="StardewValley.BellsAndWhistles.SpriteText"/>.
	/// </summary>
	/// <param name="batch">The SpriteBatch to draw with</param>
	/// <param name="text">The text to draw</param>
	/// <param name="x">the x coordinate</param>
	/// <param name="y">the y coordinate</param>
	/// <param name="color">the color to draw with, or <c>null</c> for the
	/// default color</param>
	/// <param name="alpha">The transparency to draw with</param>
	void DrawSpriteText(SpriteBatch batch, string text, int x, int y, Color? color, float alpha = 1f);

	#endregion
}
