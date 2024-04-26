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
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Intrinsics.X86;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Newtonsoft.Json;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley.BellsAndWhistles;

namespace Leclair.Stardew.ThemeManager;


/// <summary>
/// A managed asset is a special wrapper around an asset that you can request
/// from Theme Manager. Managed assets will be updated automatically whenever
/// an asset is invalidated.
///
/// Specifically, a managed asset's cached value will be marked as stale if
/// an invalidation happens, and it will be loaded again the next time you
/// try to access its value.
///
/// You'll almost always be working with a typed <see cref="IManagedAsset{TValue}"/>
/// instead of this interface.
/// </summary>
public interface IManagedAsset {

	/// <summary>
	/// The name of the asset being managed.
	/// </summary>
	IAssetName AssetName { get; }

	/// <summary>
	/// Make the managed asset load its value. If the asset is already loaded
	/// and isn't stale, this will do nothing.
	/// </summary>
	void Load();

	/// <summary>
	/// Whether or not the managed asset has already loaded.
	/// </summary>
	bool IsLoaded { get; }

	/// <summary>
	/// Whether or not the managed asset is currently stale because its asset
	/// was invalidated.
	/// </summary>
	bool IsStale { get; }

	/// <summary>
	/// Mark the managed asset as stale, so that it's reloaded the next time
	/// its value is accessed or <see cref="Load"/> is called.
	/// </summary>
	void MarkStale();

	/// <summary>
	/// An event that's fired when the managed asset is marked as stale.
	/// </summary>
	event Action? MarkedStale;

	/// <summary>
	/// The raw value of a managed asset. You will likely always want to use
	/// the <see cref="IManagedAsset{TValue}.Value"/> instead, but in case
	/// you're working with multi-typed collections this may be useful.
	/// </summary>
	object? RawValue { get; }

}

/// <summary>
/// A managed asset with a type, allowing you to access its value directly.
/// Please note that the value may be <c>null</c> .
/// </summary>
/// <typeparam name="TValue">The type of the asset.</typeparam>
public interface IManagedAsset<TValue> : IManagedAsset where TValue : notnull {

	TValue? Value { get; }

}

/// <summary>
/// This is a simple implementation of <see cref="IManagedAsset{TValue}"/>
/// for wrapping an asset loaded from <see cref="IModContentHelper"/>.
/// While this implementation, by itself, does not function to automatically
/// update when the asset is invalidated, it can be used in the event that
/// Theme Manager is not installed to make development easier.
/// </summary>
/// <typeparam name="TValue">The type of the asset.</typeparam>
public class FallbackManagedAsset<TValue> : IManagedAsset<TValue> where TValue : notnull {

	private readonly IModHelper Helper;
	private readonly IMonitor? Monitor;

	private TValue? _Value;

	#region Life Cycle

	public FallbackManagedAsset(string path, IModHelper helper, IMonitor? monitor = null) {
		Path = path;
		Helper = helper;
		Monitor = monitor;
	}

	#endregion

	#region Properties

	public string Path { get; }

	public IAssetName AssetName => Helper.ModContent.GetInternalAssetName(Path);

	public bool IsLoaded { get; private set; }
	public bool IsStale { get; private set; }

	public object? RawValue => Value;

	public TValue? Value {
		get {
			if (!IsLoaded || IsStale)
				Load();

			return _Value;
		}
	}

	public event Action? MarkedStale;

	#endregion

	#region Methods

	public void Load() {
		if (IsLoaded && !IsStale)
			return;

		IsLoaded = true;
		IsStale = false;

		try {
			_Value = Helper.ModContent.Load<TValue>(Path);
		} catch(Exception ex) {
			Monitor?.Log($"Failed loading asset from '{Path}' for managed asset: {ex}", LogLevel.Error);
		}
	}

	public void MarkStale() {
		IsStale = true;
		MarkedStale?.Invoke();
	}

	#endregion

}


/// <summary>
/// A variable set is a dictionary of variables. Variables support inheritance
/// from fall back themes, can reference other variables, and can reference
/// variables from game themes.
///
/// You are almost always going to be dealing with <see cref="IVariableSet{TValue}"/>
/// rather than this untyped interface.
/// </summary>
public interface IVariableSet {

	/// <summary>
	/// This method is called by Theme Manager when populating a theme data
	/// class with variable sets, and is used internally by variable sets to
	/// allow fall back theme support to work.
	/// </summary>
	/// <param name="manager">The manager controlling the theme this
	/// variable set is a part of.</param>
	/// <param name="manifest">The manifest for the theme this
	/// variable set is a part of.</param>
	void SetReferences(IThemeManager? manager, IThemeManifest? manifest);

	/// <summary>
	/// The raw values represent the raw strings that were read from the
	/// theme's source file. These values may still have "$" characters
	/// prefixing their key names.
	/// </summary>
	IReadOnlyDictionary<string, string>? RawValues { get; set; }

	/// <summary>
	/// Inherited values from fall back themes, combined with the <see cref="RawValues"/>
	/// from this theme. All keys will have had their "$" characters stripped
	/// at this time. The values from <see cref="DefaultValues"/> will not
	/// be included.
	/// </summary>
	IReadOnlyDictionary<string, string> InheritedValues { get; }

	/// <summary>
	/// An optional dictionary of default values for variables. These values
	/// have the lowest priority, only being used if the variable is not
	/// present in the fall back theme(s) or this theme.
	/// </summary>
	IReadOnlyDictionary<string, string>? DefaultValues { get; set; }

}

/// <summary>
/// A variable set is a dictionary of variables. Variables support inheritance
/// from fall back themes, can reference other variables, and can reference
/// variables from game themes.
/// </summary>
/// <typeparam name="TValue">The type of variable.</typeparam>
public interface IVariableSet<TValue> : IVariableSet, IReadOnlyDictionary<string, TValue> {

	/// <summary>
	/// The calculated, final variables
	/// </summary>
	IReadOnlyDictionary<string, TValue> CalculatedValues { get; }

}

/// <summary>
/// The VariableSetConverter is a <see cref="JsonConverter"/> instance that can
/// be used as a proxy for Theme Manager's <see cref="IVariableSet"/> converter,
/// allowing you to use it with the <see cref="JsonConverterAttribute"/>
/// attribute without needing a direct dependency.
///
/// To use this, simply use the <see cref="JsonConverterAttribute"/> as you
/// would normally, using this type. Then, in your code, once you've acquired
/// the Theme Manager API call <see cref="SetConverter(JsonConverter?)"/> with
/// the <see cref="JsonConverter"/> instance exposed by Theme Manager's API.
///
/// In the event that Theme Manager is not installed and does not load, this
/// will simply read and write a null value.
/// </summary>
public class VariableSetConverter : JsonConverter {

	private static JsonConverter? MainConverter;

	public static void SetConverter(JsonConverter? mainConverter) {
		MainConverter = mainConverter;
	}

	public override bool CanConvert(Type objectType) {
		return MainConverter?.CanConvert(objectType) ?? false;
	}

	public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) {
		if (MainConverter is null)
			return null;
		return MainConverter.ReadJson(reader, objectType, existingValue, serializer);
	}

	public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) {
		if (MainConverter is null)
			writer.WriteNull();
		else
			MainConverter.WriteJson(writer, value, serializer);
	}
}


/// <summary>
/// This interface represents the necessary information to render a BmFont.
/// BmFonts are used primarily by <see cref="SpriteText"/> for drawing text.
///
/// They're loaded from <c>.fnt</c> files.
/// </summary>
public interface IBmFontData {

	/// <summary>
	/// The raw font file. This is a <see cref="BmFont.FontFile"/> but
	/// we use the type `object` so consumers won't need to
	/// reference BmFont.
	/// </summary>
	object File { get; }

	/// <summary>
	/// A map of characters. This is a <c>Dictionary<char, BmFont.FontChar></c>
	/// but we use the type `object` so consumers won't need to
	/// reference BmFont.
	/// </summary>
	object CharacterMap { get; }

	/// <summary>
	/// A list of loaded textures for each <see cref="FontPage" />
	/// </summary>
	List<Texture2D> FontPages { get; }

	/// <summary>
	/// The initial pixel zoom that should be used for this font.
	/// </summary>
	float PixelZoom { get; }
}

/// <summary>
/// This theme data represents basic colors being used by the game.
/// </summary>
public interface IGameTheme {

	#region Variable Lookup

	/// <summary>
	/// Try to get a color variable, or return <c>null</c> if there is no
	/// variable with the provided name.
	/// </summary>
	/// <param name="key">The variable to get.</param>
	Color? GetColorVariable(string key);

	/// <summary>
	/// Try to get a BmFont variable, or return <c>null</c> if there is no
	/// variable with the provided name.
	/// </summary>
	/// <param name="key">The variable to get.</param>
	IBmFontData? GetBmFontVariable(string key);

	/// <summary>
	/// Try to get a BmFont variable, or return <c>null</c> if there is no
	/// variable with the provided name.
	/// </summary>
	/// <param name="key">The variable to get.</param>
	IManagedAsset<IBmFontData>? GetManagedBmFontVariable(string key);

	/// <summary>
	/// Try to get a font variable, or return <c>null</c> if there is no
	/// variable with the provided name.
	/// </summary>
	/// <param name="key">The variable to get.</param>
	SpriteFont? GetFontVariable(string key);

	/// <summary>
	/// Try to get a font variable, or return <c>null</c> if there is no
	/// variable with the provided name.
	/// </summary>
	/// <param name="key">The variable to get.</param>
	IManagedAsset<SpriteFont>? GetManagedFontVariable(string key);

	/// <summary>
	/// Try to get a texture variable, or return <c>null</c> if there is no
	/// variable with the provided name.
	/// </summary>
	/// <param name="key">The variable to get.</param>
	Texture2D? GetTextureVariable(string key);

	/// <summary>
	/// Try to get a texture variable, or return <c>null</c> if there is no
	/// variable with the provided name.
	/// </summary>
	/// <param name="key">The variable to get.</param>
	IManagedAsset<Texture2D>? GetManagedTextureVariable(string key);

	#endregion

	/// <summary>
	/// A dictionary of all valid colors used by the theme. Keys are
	/// not case-sensitive.
	/// </summary>
	IVariableSet<Color> ColorVariables { get; }

	/// <summary>
	/// A dictionary of all valid BmFonts used by the theme. Keys are
	/// not case-sensitive.
	/// </summary>
	IVariableSet<IManagedAsset<IBmFontData>> BmFontVariables { get; }

	/// <summary>
	/// A dictionary of all valid sprite fonts used by the theme. Keys are
	/// not case-sensitive.
	/// </summary>
	IVariableSet<IManagedAsset<SpriteFont>> FontVariables { get; }

	/// <summary>
	/// A dictionary of all valid textures used by the theme. Keys are
	/// not case-sensitive.
	/// </summary>
	IVariableSet<IManagedAsset<Texture2D>> TextureVariables { get; }

	/// <summary>
	/// A dictionary of all index-based sprite text colors that are set by
	/// the theme. You can just use <see cref="SpriteText.getColorFromIndex(int)"/>
	/// rather than checking this list if using the active theme.
	/// </summary>
	Dictionary<int, Color> IndexedSpriteTextColors { get; }

	/// <summary>
	/// A dictionary of dictionaries of sprite text color replacements that
	/// are set by the theme. The key of the inner dictionary is the
	/// <see cref="Color.PackedValue"/> as a long so we can store the
	/// <c>null</c> value as <c>-1</c>.
	/// </summary>
	Dictionary<string, Dictionary<long, Color?>> SpriteTextColorSets { get; }

}

/// <summary>
/// This event is emitted by <see cref="IThemeManager{DataT}"/> whenever the
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
	/// The manifest of the previously active theme.
	/// </summary>
	IThemeManifest? OldManifest { get; }

	/// <summary>
	/// The manifest of the newly active theme.
	/// </summary>
	IThemeManifest? NewManifest { get; }

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
/// This event is emitted by <see cref="IThemeManager{DataT}"/> whenever
/// themes are discovered. This can happen during the initial load, when
/// themes are manually reloaded, when the theme data asset is invalidated,
/// or when <see cref="IThemeManager.Discover(bool, bool, bool)"/> is
/// called manually.
/// </summary>
public interface IThemesDiscoveredEvent<DataT> {

	/// <summary>
	/// A read-only dictionary of all theme manifests.
	/// </summary>
	IReadOnlyDictionary<string, IThemeManifest> Manifests { get; }

	/// <summary>
	/// A read-only dictionary of all theme data.
	/// </summary>
	IReadOnlyDictionary<string, DataT> Data { get; }

}

/// <summary>
/// A manifest has necessary metadata for a theme for display in theme
/// selection UI, for performing automatic theme selection, and for
/// loading assets correctly from the file system.
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

	/// <summary>
	/// If a theme is non-selectable, it cannot be selected as the active
	/// theme and will not be displayed in the UI to users. Non-selectable
	/// themes are potentially useful for use in conjunction with
	/// <see cref="FallbackTheme"/>.
	/// </summary>
	bool NonSelectable { get; }

	#endregion

	#region Theme Selection

	/// <summary>
	/// An array of <see cref="IManifest.UniqueID"/>s of mods that this theme
	/// is meant to support. When <see cref="IThemeManager{DataT}.SelectedThemeId"/>
	/// is set to <c>automatic</c>, this theme will potentially be selected if
	/// any of the listed mods are present and loaded.
	/// </summary>
	string[]? SupportedMods { get; }

	/// <summary>
	/// An array of <see cref="IManifest.UniqueID"/>s of mods that this theme
	/// does <b>not</b> support. When <see cref="IThemeManager{DataT}.SelectedThemeId"/>
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
	/// <see cref="IThemeManager{DataT}"/> instance.
	/// </summary>
	bool? OverrideRedirection { get; }

	#endregion

}


public interface IThemeManager {

	#region Basic Values

	/// <summary>
	/// The manifest of the mod that this <see cref="IThemeManager"/>
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
	/// A string that represents the path used when redirecting theme
	/// data through game content to allow Content Patcher access to
	/// your mod's theme data. By default, this value is
	/// <c>Mods/{yourMod.UniqueId}/ThemeData</c>
	/// </summary>
	string ThemeLoaderPath { get; }

	/// <summary>
	/// The relative path to where your mod keeps its embedded themes.
	/// By default, this is <c>assets/themes</c>. If this is <c>null</c>,
	/// no embedded themes will be loaded.
	/// </summary>
	string? EmbeddedThemesPath { get; }

	/// <summary>
	/// Whether or not this <see cref="IThemeManager"/> is redirecting
	/// theme data loading through <see cref="IGameContentHelper"/> to
	/// allow other mods, such as Content Patcher, to modify theme data.
	/// </summary>
	bool UsingThemeRedirection { get; }

	/// <summary>
	/// Whether or not this <see cref="IThemeManager"/> is redirecting
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

	/// <summary>
	/// Get the <see cref="IThemeManifest"/> instance of a specific theme,
	/// if it's loaded.
	/// 
	/// An alternative version of <see cref="TryGetManifest(string, out IThemeManifest?)"/>
	/// that doesn't use "out", as that can cause issues with Pintail, the API
	/// proxy service.
	/// </summary>
	/// <param name = "themeId" > The < see cref="IThemeManifest.UniqueID"/> of the
	/// theme we want the manifest for.</param>
	/// <returns>The <see cref="IThemeManifest"/> instance for
	/// the requested theme, or <c>null</c> if the theme is not loaded or
	/// has no manifest. Only the <c>default</c> theme has no manifest.</returns>
	IThemeManifest? GetManifest(string themeId);

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

	#region Content Patcher

	/// <summary>
	/// Register a simple Content Patcher token that returns the
	/// <see cref="ActiveThemeId"/>. This token is registered for your mod,
	/// so you need to have a dependency on Content Patcher in your mod's
	/// manifest or Content Patcher will reject it.
	/// </summary>
	/// <param name="name">The name of the token.</param>
	void RegisterCPToken(string name = "Theme");

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
	/// Get the name of an asset, as generated to load a given asset through
	/// the game content pipeline. This may return <c>null</c> if the asset
	/// won't be loaded from a theme.
	/// </summary>
	/// <param name="path">The relative file path.</param>
	/// <param name="themeId">The theme the asset name should be generated with.</param>
	/// <param name="stripFileExtensions">When true, the file extension will
	/// be stripped when generating asset names for working with GameContent.</param>
	IAssetName? GetAssetName(string path, string? themeId, bool stripFileExtensions = true);

	/// <summary>
	/// Invalidate all cached assets that we provide.
	/// </summary>
	/// <param name="themeId">An optional theme ID to only clear that
	/// theme's assets.</param>
	void Invalidate(string? themeId = null);

	/// <summary>
	/// Invalidate a specific asset we provide.
	/// </summary>
	/// <param name="path">The path to the asset.</param>
	/// <param name="themeId">An optional theme ID to only clear that
	/// theme's asset.</param>
	/// <param name="stripFileExtensions">When true, the file extension will
	/// be stripped when generating asset names for working with GameContent.</param>
	void Invalidate(string path, string? themeId = null, bool stripFileExtensions = true);

	/// <summary>
	/// Get an <see cref="IManagedAsset{TValue}"/> instance for an asset. This
	/// uses similar logic to <see cref="Load{T}(string, string?, bool, bool)"/>
	/// but <b>does not</b> support assets that aren't being loaded through
	/// asset redirection.
	/// </summary>
	/// <typeparam name="T">The expected data type.</typeparam>
	/// <param name="path">The relative file path.</param>
	/// <param name="themeId">If set, load the asset from the specified theme
	/// rather than the active theme.</param>
	/// <param name="stripFileExtensions">When true, the file extension will
	/// be stripped when generating asset names for working with the
	/// game content pipeline.</param>
	/// <param name="allowFallback">When true, if an asset does not exist in
	/// the requested theme, we'll try loading it from the theme's specified
	/// fall back theme if one exists.</param>
	/// <exception cref="ArgumentException">The <paramref name="path"/> is empty
	/// or contains invalid characters.</exception>
	/// <exception cref="ContentLoadException">The content asset couldn't be
	/// loaded (e.g. because it doesn't exist).</exception>
	IManagedAsset<T> GetManagedAsset<T>(string path, string? themeId = null, bool stripFileExtensions = true, bool allowFallback = true) where T : notnull;

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
	/// <param name="stripFileExtensions">When true, the file extension will
	/// be stripped when generating asset names for working with the
	/// game content pipeline.</param>
	/// <param name="allowFallback">When true, if an asset does not exist in
	/// the requested theme, we'll try loading it from the theme's specified
	/// fall back theme if one exists.</param>
	/// <exception cref="ArgumentException">The <paramref name="path"/> is empty
	/// or contains invalid characters.</exception>
	/// <exception cref="ContentLoadException">The content asset couldn't be
	/// loaded (e.g. because it doesn't exist).</exception>
	T Load<T>(string path, string? themeId = null, bool stripFileExtensions = true, bool allowFallback = true) where T : notnull;

	/// <summary>
	/// Check whether an asset exists in a theme.
	/// </summary>
	/// <param name="path">The relative file path.</param>
	/// <param name="themeId">If set, check for the asset in the specified
	/// theme rather than the active theme.</param>
	/// <param name="stripFileExtensions">When true, the file extension will
	/// be stripped when generating asset names for working with the
	/// game content pipeline.</param>
	/// <param name="allowFallback">When true, if an asset does not exist in
	/// the requested theme, we'll check to see if it exists in the theme's
	/// specified fall back theme if one exists.</param>
	/// <param name="allowDefault">When true, if an asset does not exist
	/// elsewhere we'll check to see if it exists in the base mod's assets.</param>
	bool DoesAssetExist<T>(string path, string? themeId = null, bool stripFileExtensions = true, bool allowFallback = true, bool allowDefault = true) where T : notnull;

	#endregion
}


/// <summary>
/// <c>IThemeManager</c> implements a system for adding theme support to
/// Stardew Valley mods. It handles discovery, selection, asset loading,
/// Content Patcher support, and emitting events when the theme changes.
/// </summary>
/// <typeparam name="DataT">Your mod's theme data type</typeparam>
public interface IThemeManager<DataT> : IThemeManager where DataT : new() {

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

	/// <summary>
	/// Get the <typeparamref name="DataT"/> instance for a specific
	/// theme, if it's loaded.
	///
	/// As this method uses a dictionary lookup internally, you should cache
	/// the result if you use it frequently for best performance. If you do
	/// cache the result, make sure to update your cache whenever the
	/// <see cref="ThemeChanged"/> event is emitted.
	///
	/// This is an alternate method that does not use out, because Pintail.
	/// </summary>
	/// <param name="themeId">The <see cref="IThemeManifest.UniqueID"/> of the
	/// theme we want the data instance for.</param>
	/// <returns>The <typeparamref name="DataT"/> instance for
	/// the requested theme, or <c>null</c> if the theme is not loaded.</returns>
	DataT? GetTheme(string themeId);

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
	event Action<IThemeChangedEvent<DataT>>? ThemeChanged;

	/// <summary>
	/// This event is fired whenever themes are discovered and theme data has
	/// been loaded, but before theme selection runs. This can be used to
	/// perform any extra processing of theme data.
	/// </summary>
	event Action<IThemesDiscoveredEvent<DataT>>? ThemesDiscovered;

	#endregion
}

public partial interface IThemeManagerApi {

	#region Game Themes

	IGameTheme GameTheme { get; }

	event Action<IThemeChangedEvent<IGameTheme>>? GameThemeChanged;

	#endregion

	#region Variable Sets

	/// <summary>
	/// This <see cref="JsonConverter"/> instance handles <see cref="IVariableSet{TValue}"/>
	/// instances, both reading and writing them. You should store this value
	/// using <see cref="VariableSetConverter.SetConverter(JsonConverter?)"/>
	/// if you're using variable sets in your theme.
	/// </summary>
	JsonConverter VariableSetConverter { get; }

	/// <summary>
	/// Parse a value of <typeparamref name="TValue"/> from a string.
	/// </summary>
	/// <typeparam name="TValue">The desired type</typeparam>
	/// <param name="input">The input to parse</param>
	/// <param name="manager">The Theme Manager instance this variable set
	/// belongs to, if one is known.</param>
	/// <param name="manifest">The theme manifest of the theme this variable set
	/// belongs to, if one is known.</param>
	/// <param name="result">The resulting value</param>
	/// <returns>Whether or not a value was parsed successfully.</returns>
	delegate bool TryParseVariableSetValue<TValue>(string input, IThemeManager? manager, IThemeManifest? manifest, [NotNullWhen(true)] out TValue? result);

	/// <summary>
	/// Register a new type that can be stored within a <see cref="IVariableSet{TValue}"/>.
	/// </summary>
	/// <typeparam name="TValue">The type to be supported.</typeparam>
	/// <param name="parseDelegate">A method for parsing values from strings.</param>
	/// <returns>Whether or not the parser was registered successfully. If
	/// false, another parser for the type was already registered.</returns>
	bool RegisterVariableSetType<TValue>(TryParseVariableSetValue<TValue> parseDelegate);

	/// <summary>
	/// Create a new <see cref="IVariableSet{TValue}"/> instance using the
	/// main implementation of variable sets. This should be used in conjunction
	/// with <see cref="RegisterVariableSetType{TValue}(TryParseVariableSetValue{TValue})"/>.
	/// If you want to create a variable set for <see cref="IBmFontData"/>,
	/// <see cref="Texture2D"/>, or <see cref="SpriteFont"/> you should use
	/// the specific methods for those.
	/// </summary>
	/// <exception cref="ArgumentException">Throws an exception when attempting
	/// to create a variable set with an unsupported type. The default supported
	/// types are: <see cref="Color"/>.</exception>
	IVariableSet<TValue> CreateVariableSet<TValue>();

	/// <summary>
	/// Create a new <see cref="IVariableSet{IManagedAsset{IBmFontData}}"/>
	/// instance for <see cref="IBmFontData"/> using the main
	/// implementation of variable sets.
	/// </summary>
	IVariableSet<IManagedAsset<IBmFontData>> CreateBmFontVariableSet();

	/// <summary>
	/// Create a new <see cref="IVariableSet{IManagedAsset{Texture2D}}"/>
	/// instance for managed textures using the main implementation
	/// of variable sets.
	/// </summary>
	IVariableSet<IManagedAsset<Texture2D>> CreateTextureVariableSet();

	/// <summary>
	/// Create a new <see cref="IVariableSet{IManagedAsset{SpriteFont}}"/>
	/// instance for managed sprite fonts using the main implementation
	/// of variable sets.
	/// </summary>
	IVariableSet<IManagedAsset<SpriteFont>> CreateFontVariableSet();

	#endregion

	#region Custom Themes

	/// <summary>
	/// Try to get an existing <see cref="IThemeManager{DataT}"/> instance
	/// for a mod. This will never create a new instance. If
	/// <typeparamref name="DataT"/> does not match the type used when
	/// creating the existing manager, this method will return <c>false</c>
	/// rather than throwing an <see cref="InvalidCastException"/>.
	/// </summary>
	/// <typeparam name="DataT">The type for the mod's theme data.</typeparam>
	/// <param name="themeManager">The <see cref="IThemeManager{DataT}"/>
	/// instance, if one exists.</param>
	/// <param name="forMod">An optional manifest to get the theme manager
	/// for a specific mod.</param>
	bool TryGetTypedManager<DataT>([NotNullWhen(true)] out IThemeManager<DataT>? themeManager, IManifest? forMod = null) where DataT : class, new();

	IThemeManager<DataT>? GetTypedManager<DataT>(IManifest? forMod = null) where DataT : class, new();

	/// <summary>
	/// Try to get an existing <see cref="IThemeManager"/> instance for a mod.
	/// This will never create a new instance.
	/// </summary>
	/// <param name="themeManager">The <see cref="IThemeManager"/>
	/// instance, if one exists.</param>
	/// <param name="forMod">An optional manifest to get the theme manager
	/// for a specific mod.</param>
	bool TryGetManager([NotNullWhen(true)] out IThemeManager? themeManager, IManifest? forMod = null);

	IThemeManager? GetManager(IManifest? forMod = null);

	/// <summary>
	/// Get an <see cref="IThemeManager{DataT}"/> for a mod. If there is no
	/// existing instance, create a new one using the supplied parameters.
	///
	/// If there is an existing instance, the parameters are ignored.
	/// </summary>
	/// <typeparam name="DataT">The type for the mod's theme data.</typeparam>
	/// <param name="defaultTheme">A <typeparamref name="DataT"/> instance to
	/// use for the <c>default</c> theme. If one is not provided, a new
	/// instance will be created.</param>
	/// <param name="embeddedThemesPath">The relative path to search for
	/// embedded themes at. See <see cref="IThemeManager.EmbeddedThemesPath"/>.</param>
	/// <param name="assetPrefix">A string prepended to asset paths when
	/// loading assets from the <c>default</c> theme. See
	/// <see cref="IThemeManager.DefaultAssetPrefix"/>.</param>
	/// <param name="assetLoaderPrefix">A string prepended to asset names
	/// when redirecting assets through <see cref="IGameContentHelper"/>.
	/// See <see cref="IThemeManager.AssetLoaderPrefix"/>.</param>
	/// <param name="themeLoaderPath">A string used when redirecting
	/// theme data through <see cref="IGameContentHelper"/>.
	/// See <see cref="IThemeManager.ThemeLoaderPath"/>.</param>
	/// <param name="forceAssetRedirection">If set to a value, override
	/// the default behavior of <see cref="IThemeManager.UsingAssetRedirection"/>.</param>
	/// <param name="forceThemeRedirection">If set to a value, override
	/// the default behavior of <see cref="IThemeManager.UsingThemeRedirection"/></param>
	/// <param name="onThemeChanged">An event handler to call whenever the
	/// theme changes. This is provided as an alternative to registering an
	/// event handler after the API call returns, as theme discovery will
	/// have happened by then and there will not be an initial event dispatch.</param>
	/// <exception cref="InvalidCastException">Thrown when attempting to get a
	/// manager with a different <typeparamref name="DataT"/> than it was
	/// created with.</exception>
	IThemeManager<DataT> GetOrCreateManager<DataT>(
		DataT? defaultTheme = null,
		string? embeddedThemesPath = "assets/themes",
		string? assetPrefix = "assets",
		string? assetLoaderPrefix = null,
		string? themeLoaderPath = null,
		bool? forceAssetRedirection = null,
		bool? forceThemeRedirection = null,
		Action<IThemeChangedEvent<DataT>>? onThemeChanged = null
	) where DataT : class, new();

	#endregion

	#region Color Parsing

	/// <summary>
	/// Parse a color from a string. This supports CSS hex format, CSS rgb()
	/// format, a selection of color names, and basic "[r], [g], [b], [a]"
	/// values separated by commas. This also attempts to use color variables
	/// from the current game theme if your input starts with <c>$</c>.
	/// </summary>
	/// <param name="value">The input string to parse</param>
	/// <param name="color">The resulting color, or null</param>
	/// <returns>Whether or not a color was successfully read.</returns>
	bool TryParseColor(string value, [NotNullWhen(true)] out Color? color);

	#endregion

	#region Font-ed SpriteTextT

	/// <summary>
	/// Draw strings of <see cref="SpriteText"/> with arbitrary fonts.
	/// </summary>
	/// <param name="batch">The SpriteBatch to draw with</param>
	/// <param name="font">The font to draw with</param>
	/// <param name="text">The text to draw</param>
	/// <param name="x">the x coordinate</param>
	/// <param name="y">the y coordinate</param>
	/// <param name="color">the color to draw with, or <c>null</c> for the
	/// default color</param>
	/// <param name="alpha">The transparency to draw with</param>
	void DrawSpriteText(SpriteBatch batch, IBmFontData? font, string text, int x, int y, Color? color, float alpha = 1f);

	#endregion
}
