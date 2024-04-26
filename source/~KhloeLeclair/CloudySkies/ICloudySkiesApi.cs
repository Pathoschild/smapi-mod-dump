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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Xna.Framework;

using StardewValley.Network;

namespace Leclair.Stardew.CloudySkies;

/// <summary>
/// This is the public API surface of Cloudy Skies. Please note that you
/// cannot use this to add data currently, but that may become possible
/// in the future. This is just a quick API for initial data access.
/// </summary>
public interface ICloudySkiesApi {

	/// <summary>
	/// Enumerate all the custom weather conditions we know about.
	/// </summary>
	IEnumerable<IWeatherData> GetAllCustomWeather();

	/// <summary>
	/// Try to get a custom weather condition by id.
	/// </summary>
	/// <param name="id">The Id of the weather condition to get.</param>
	/// <param name="data">The data, if it exists.</param>
	/// <returns>Whether or not the data exists.</returns>
	bool TryGetWeather(string id, [NotNullWhen(true)]  out IWeatherData? data);

	/// <summary>
	/// Force Cloudy Skies to re-evaluate its current layers. This can
	/// be used in case a game state query has changed and you expect
	/// the layers to change somehow.
	/// </summary>
	/// <param name="weatherId">A specific weather to invalidate the layer cache for.</param>
	void RegenerateLayers(string? weatherId = null);

}


/// <summary>
/// The data resource for any given custom weather type. This is, for
/// the time being, read-only in the API. Modify the data resource if
/// you want to change the weather, please!
/// </summary>
public interface IWeatherData {

	/// <summary>
	/// This weather condition's unique Id. This is the Id that you
	/// would check with game state queries, etc.
	/// </summary>
	string Id { get; }

	#region Display

	/// <summary>
	/// A display name to show the player when this weather condition
	/// should be referenced by name. This is a tokenizable string.
	/// </summary>
	string DisplayName { get; }

	#region Icon

	/// <summary>
	/// The name of the texture containing this weather's icon.
	/// </summary>
	string? IconTexture { get; }

	/// <summary>
	/// The location of this weather's icon within the texture. All
	/// weather icons are 12 by 8 pixels.
	/// </summary>
	Point IconSource { get; }

	#endregion

	#region Television

	/// <summary>
	/// The name of the texture containing this weather's TV animation.
	/// </summary>
	string? TVTexture { get; }

	/// <summary>
	/// The location of this weather's TV animation within the texture.
	/// Each frame of a TV animation is 13 by 13 pixels, and frames are
	/// arranged in a horizontal line.
	/// </summary>
	Point TVSource { get; }

	/// <summary>
	/// How many frames long this weather's TV animation is. Default is 4.
	/// </summary>
	int TVFrames { get; }

	/// <summary>
	/// A forecast string that will be displayed when the player checks
	/// tomorrow's forecast using a television. This is a
	/// tokenizable string. May be null.
	/// </summary>
	string? Forecast { get; }

	/// <summary>
	/// A dictionary of forecast strings, by context. These are all
	/// tokenizable strings. May be null.
	/// </summary>
	Dictionary<string, string>? ForecastByContext { get; }

	#endregion

	#region Weather Totem

	/// <summary>
	/// An optional string to display to the player when this weather
	/// condition is triggered using a custom weather totem. This is
	/// a tokenizable string.
	/// </summary>
	string? TotemMessage { get; }

	/// <summary>
	/// A sound to play immediately when using the totem. The base
	/// game's Rain Totem uses the <c>thunder</c> sound.
	/// </summary>
	string? TotemSound { get; }

	/// <summary>
	/// A sound to play 2 seconds after using a totem, as the animation
	/// ends. The base game's Rain Totem uses the <c>rainsound</c> sound.
	/// </summary>
	string? TotemAfterSound { get; }

	/// <summary>
	/// A color to flash the screen when using a totem. The base game's
	/// Rain Totem uses the color <c>slateblue</c>.
	/// </summary>
	Color? TotemScreenTint { get; }

	/// <summary>
	/// A texture to use for displaying particles when using a totem.
	/// The base game's Rain Totem uses the texture <c>LooseSprites\Cursors</c>
	/// </summary>
	string? TotemParticleTexture { get; }

	/// <summary>
	/// The source rectangle of the texture to use when displaying
	/// particles. Defaults to the entire texture. The base game's
	/// Rain Totem uses <c>648, 1045, 52, 33</c>.
	/// </summary>
	Rectangle? TotemParticleSource { get; }

	#endregion

	#endregion

	#region Behavior - Music

	/// <summary>
	/// If this is set to a non-null value, this weather condition will
	/// override the playing music in the same way the base game's raining
	/// condition does, using the named audio cue.
	/// </summary>
	string? MusicOverride { get; }

	/// <summary>
	/// The frequency the <see cref="MusicOverride"/> should be played at when
	/// the player is in an outside location.
	/// This does not affect all sound cues. Default is 100.
	/// </summary>
	float MusicFrequencyOutside { get; }

	/// <summary>
	/// The frequency the <see cref="MusicOverride"/> should be played at when
	/// the player is in an inside location.
	/// This does not affect all sound cues. Default is 100.
	/// </summary>
	float MusicFrequencyInside { get; }

	#endregion

	#region Behavior - Vanilla Flags

	/// <summary>
	/// Controls the value of <see cref="LocationWeather.IsRaining"/>.
	/// </summary>
	bool IsRaining { get; }

	/// <summary>
	/// Controls the value of <see cref="LocationWeather.IsSnowing"/>.
	/// </summary>
	bool IsSnowing { get; }

	/// <summary>
	/// Controls the value of <see cref="LocationWeather.IsLightning"/>.
	/// </summary>
	bool IsLightning { get; }

	/// <summary>
	/// Controls the value of <see cref="LocationWeather.IsDebrisWeather"/>.
	/// </summary>
	bool IsDebrisWeather { get; }

	/// <summary>
	/// Controls the value of <see cref="LocationWeather.IsGreenRain"/>.
	/// </summary>
	bool IsGreenRain { get; }

	#endregion

	#region Behavior - Other

	/// <summary>
	/// If this is set to true, this weather will cause maps to use night
	/// tiles and to have darkened windows, similar to how rain functions.
	/// </summary>
	bool UseNightTiles { get; }

	/// <summary>
	/// Whether or not critters should attempt to spawn in outdoor maps
	/// while this weather condition is active. Defaults to true.
	/// </summary>
	bool SpawnCritters { get; }

	/// <summary>
	/// Whether or not frogs should attempt to spawn. If this is left at
	/// <c>null</c>, the default logic will be used. <see cref="SpawnCritters"/>
	/// must be enabled for frogs to spawn.
	/// </summary>
	bool? SpawnFrogs { get; }

	/// <summary>
	/// Whether or not cloud shadow critters should attempt to spawn. If
	/// this is left at <c>null</c>, the default logic will be used.
	/// <see cref="SpawnCritters"/> must be enabled for cloud shadows to spawn.
	/// </summary>
	bool? SpawnClouds { get; }

	#endregion

	#region Screen Tinting

	/// <summary>
	/// The ambient color that should be used for lighting when this weather
	/// condition is active. In the base game, only rainy weather uses this,
	/// with the color <c>255, 200, 80</c>.
	/// </summary>
	Color? AmbientColor { get; }

	/// <summary>
	/// This color, if set, is drawn to the screen in lighting mode during
	/// the Draw Lightmap phase of the world rendering. In the base game,
	/// only rainy weather uses this, with the color <c>orangered</c>.
	/// </summary>
	Color? LightingTint { get; }

	/// <summary>
	/// This opacity is pre-multiplied with <see cref="LightingTint"/>. You
	/// should use it if you want your tint to have an opacity, rather than
	/// directly using the alpha channel.
	///
	/// In the base game, rainy weather uses <c>0.45</c> for this value.
	/// </summary>
	float LightingTintOpacity { get; }

	/// <summary>
	/// This color, if set, is drawn as a screen overlay after the lighting
	/// phase of world rendering. In the base game, only rainy weather uses
	/// this, with the color <c>blue</c>. Green Rain uses the color
	/// <c>0, 120, 150</c>.
	/// </summary>
	Color? PostLightingTint { get; }

	/// <summary>
	/// This opacity is pre-multiplied with <see cref="PostLightingTint"/>.
	/// You should use it if you want your tint to have an opacity, rather
	/// than directly using the alpha channel.
	///
	/// In the base game, rainy weather uses <c>0.2</c> and green rain
	/// uses <c>0.22</c> for this value.
	/// </summary>
	float PostLightingTintOpacity { get; }

	#endregion

}
