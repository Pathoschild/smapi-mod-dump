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

using StardewModdingAPI;

using System;

using StardewValley.GameData.Buffs;

using Newtonsoft.Json.Linq;

using Microsoft.Xna.Framework.Graphics;

using StardewValley.GameData.Locations;



#if IS_CLOUDY_SKIES
using Leclair.Stardew.Common.Types;

namespace Leclair.Stardew.CloudySkies;
#else
namespace Leclair.Stardew.CloudySkies;

/// <summary>
/// An <c>IModAssetEditor</c> is a special type of <see cref="IDictionary"/>
/// that works with SMAPI's API proxying to allow you to edit another
/// mod's data assets from a C# mod.
///
/// Unlike a normal dictionary, this custom <see cref="IDictionary"/> will
/// potentially throw <see cref="ArgumentException"/> when adding/assigning
/// values if they do not match the internal types.
///
/// To get around that, you are expected to use <see cref="GetOrCreate(string)"/>
/// and <see cref="Create{TValue}"/> to make instances using the correct
/// internal types, which you can then modify as needed.
/// </summary>
/// <typeparam name="TModel">An interface describing the internal model.</typeparam>
public interface IModAssetEditor<TModel> : IDictionary<string, TModel> {

	/// <summary>
	/// Get the data entry with the given key. If one does not exist, create
	/// a new entry, add it to the dictionary, and return that.
	/// </summary>
	/// <param name="key">The key to get an entry for.</param>
	TModel GetOrCreate(string key);

	/// <summary>
	/// Creates an instance of the provided type. This should be used to create
	/// instances of <typeparamref name="TValue"/>, where <typeparamref name="TValue"/>
	/// is an interface existing within <typeparamref name="TModel"/>.
	///
	/// For example, if <typeparamref name="TModel"/> has a property referencing a
	/// <c>ISomeOtherModel</c> and you need to create an instance of that, then
	/// you'll need to call <c>Create</c> with <typeparamref name="TValue"/> set to
	/// <c>ISomeOtherModel</c>.
	/// </summary>
	TValue Create<TValue>();

}
#endif


[Flags]
public enum LightingTweenMode {

	None = 0,
	Before = 1,
	After = 2,
	Both = 3

}


/// <summary>
/// This is the public API surface of Cloudy Skies.
/// </summary>
public interface ICloudySkiesApi {

	/// <summary>
	/// The name of Cloudy Skies' custom weather type data asset.
	/// </summary>
	string WeatherAssetName { get; }

	/// <summary>
	/// The name of Cloudy Skies' custom location context extension
	/// data asset.
	/// </summary>
	string ContextAssetName { get; }

	/// <summary>
	/// Enumerate all the custom weather conditions we know about.
	/// </summary>
	IEnumerable<IWeatherData> GetAllCustomWeather();

	/// <summary>
	/// Enumerate all the custom location context extension data we have.
	/// </summary>
	IEnumerable<ILocationContextExtensionData> GetAllContextData();

	/// <summary>
	/// Get an editor for Cloudy Skies' weather type data asset, so you can modify
	/// it from a C# mod. Make sure to use the provided
	/// <see cref="IModAssetEditor{TModel}.GetOrCreate(string)"/> and
	/// <see cref="IModAssetEditor{TModel}.Create{TValue}"/> methods
	/// as appropriate.
	/// </summary>
	/// <param name="assetData">The <see cref="IAssetData"/> you get
	/// in your asset requested event handler.</param>
	/// <returns>A dictionary-like tool for editing the data asset.</returns>
	IModAssetEditor<IWeatherData> GetWeatherEditor(IAssetData assetData);

	/// <summary>
	/// Get an editor for Cloudy Skies' custom location context extension
	/// data asset, so you can modify it from a C# mod. Make sure to use
	/// the provided <see cref="IModAssetEditor{TModel}.GetOrCreate(string)"/>
	/// and <see cref="IModAssetEditor{TModel}.Create{TValue}"/> methods
	/// as appropriate.
	/// </summary>
	/// <param name="assetData">The <see cref="IAssetData"/> you get
	/// in your asset requested event handler.</param>
	/// <returns>A dictionary-like tool for editing the data asset.</returns>
	IModAssetEditor<ILocationContextExtensionData> GetContextEditor(IAssetData assetData);

	/// <summary>
	/// Try to get a custom weather condition by id.
	/// </summary>
	/// <param name="id">The Id of the weather condition to get.</param>
	/// <param name="data">The data, if it exists.</param>
	/// <returns>Whether or not the data exists.</returns>
	bool TryGetWeather(string id, [NotNullWhen(true)] out IWeatherData? data);

	/// <summary>
	/// Try to get existing location context extension data by Id.
	/// </summary>
	/// <param name="id">The Id of the location context to get extension data for.</param>
	/// <param name="data">The data, if it exists.</param>
	/// <returns>Whether or not the data exists.</returns>
	bool TryGetContextData(string id, [NotNullWhen(true)] out ILocationContextExtensionData? data);

	/// <summary>
	/// Force Cloudy Skies to re-evaluate its current layers. This can
	/// be used in case a game state query has changed and you expect
	/// the layers to change somehow.
	/// </summary>
	/// <param name="weatherId">A specific weather to invalidate the layer cache for.</param>
	void RegenerateLayers(string? weatherId = null);

	/// <summary>
	/// Force Cloudy Skies to re-evaluate its current effects. This can
	/// be used in case a game state query has changed, and you expect
	/// the layers to change somehow.
	/// </summary>
	/// <param name="weatherId">A specific weather to invalidate the effect cache for.</param>
	void RegenerateEffects(string? weatherId = null);

	#region Custom Things

	delegate IWeatherLayer? WeatherLayerFactoryDelegate(ulong layerId, ICustomLayerData data);
	delegate IWeatherEffect? WeatherEffectFactoryDelegate(ulong layerId, ICustomEffectData data);

	/// <summary>
	/// Let Cloudy Skies know that your <see cref="IWeatherLayer"/> or <see cref="IWeatherEffect"/>
	/// uses the asset with the provided name. This will ensure that Cloudy Skies
	/// calls the relevant <see cref="IWeatherLayer.ReloadAssets"/> method if
	/// the asset is invalidated, so that you can update as necessary.
	/// </summary>
	/// <param name="id">The <see cref="IWeatherLayer.Id"/> or <see cref="IWeatherEffect.Id"/></param>
	/// <param name="assetName">The name of the asset.</param>
	void NotifyLoadsAsset(ulong id, string assetName);

	/// <summary>
	/// Register a custom <see cref="IWeatherLayer"/> type with Cloudy Skies.
	/// </summary>
	/// <param name="type">The type name. You should include your mod's Id in this value to ensure it's unique.</param>
	/// <param name="factory">A factory for creating instances of your custom layer.</param>
	/// <returns>Whether or not the layer was registered successfully. If this returns false, that type name was already in use.</returns>
	bool RegisterLayerType(string type, WeatherLayerFactoryDelegate factory);

	/// <summary>
	/// Unregister an <see cref="IWeatherLayer"/> type from Cloudy Skies.
	/// </summary>
	/// <param name="type">The type name.</param>
	/// <returns>Whether or not the layer type was removed successfully.</returns>
	bool UnregisterLayerType(string type);

	/// <summary>
	/// Register a custom <see cref="IWeatherEffect"/> type with Cloudy Skies.
	/// </summary>
	/// <param name="type">The type name. You should include your mod's Id in this value to ensure it's unique.</param>
	/// <param name="factory">A factory for creating instances of your custom effect.</param>
	/// <returns>Whether or not the effect was registered successfully. If this returns false, that type name was already in use.</returns>
	bool RegisterEffectType(string type, WeatherEffectFactoryDelegate factory);

	/// <summary>
	/// Unregister an <see cref="IWeatherEffect"/> type from Cloudy Skies.
	/// </summary>
	/// <param name="type">The type name.</param>
	/// <returns>Whether or not the effect type was removed successfully.</returns>
	bool UnregisterEffectType(string type);

	#endregion

}


/// <summary>
/// An <c>IWeatherLayer</c> represents an active layer that is being rendered
/// by Cloudy Skies. Instances of <c>IWeatherLayer</c> are kept around so long
/// as their underlying data doesn't change, at which point the layer is recreated
/// with the new data.
/// </summary>
public interface IWeatherLayer {

	/// <summary>
	/// The layer's unique Id this session. This Id is used for tracking which
	/// assets are used by which layers, for the purpose of dispatching <see cref="ReloadAssets"/>
	/// calls when an asset is invalidated.
	/// </summary>
	ulong Id { get; }

	/// <summary>
	/// The draw type of this layer, to determine what mode the SpriteBatch
	/// should be in when <see cref="Draw(SpriteBatch, GameTime, RenderTarget2D)"/>
	/// is called.
	/// </summary>
	LayerDrawType DrawType { get; }

	/// <summary>
	/// This method is called whenever any of the assets this <see cref="IWeatherLayer"/>
	/// has reported it uses are invalidated in the cache. You should reload
	/// your assets (textures, etc.) when this is called, if possible and relevant.
	/// </summary>
	void ReloadAssets();

	/// <summary>
	/// This method is called whenever the viewport is resized.
	/// </summary>
	/// <param name="newSize">The new size.</param>
	/// <param name="oldSize">The old size.</param>
	void Resize(Point newSize, Point oldSize);

	/// <summary>
	/// This method is called whenever the viewport moves.
	/// </summary>
	/// <param name="offsetX">The distance moved on the X axis.</param>
	/// <param name="offsetY">The distance moved on the Y axis.</param>
	void MoveWithViewport(int offsetX, int offsetY);

	/// <summary>
	/// This method is called once per game tick to allow the layer
	/// to perform any necessary updates, like moving+updating particles, etc.
	/// </summary>
	/// <param name="time">The current GameTime</param>
	void Update(GameTime time);

	/// <summary>
	/// This method is called once per frame to draw the layer.
	/// </summary>
	/// <param name="batch">The SpriteBatch to draw with.</param>
	/// <param name="time">The current GameTime</param>
	/// <param name="targetScreen">An optional target to render to. Seems unused?</param>
	void Draw(SpriteBatch batch, GameTime time, RenderTarget2D targetScreen);

}

/// <summary>
/// An <c>IWeatherEffect</c> is an effect that is applied to each player
/// in a location experiencing a weather condition with the effect. This
/// code runs locally, so you should use <see cref="StardewValley.Game1.player">
/// and keep in mind that this won't run in locations where the player isn't.
///
/// For general location effects, you should use triggers or custom C#
/// that checks the current weather Id.
/// </summary>
public interface IWeatherEffect {

	/// <summary>
	/// The effect's unique Id this session. This Id is used for tracking which
	/// assets are used by which effects, for the purpose of dispatching <see cref="ReloadAssets"/>
	/// calls when an asset is invalidated.
	/// </summary>
	ulong Id { get; }

	/// <summary>
	/// How often should this effect's <see cref="Update(GameTime)"/> method
	/// be called, in ticks.
	/// </summary>
	uint Rate { get; }

	/// <summary>
	/// This method is called whenever any of the assets this <see cref="IWeatherEffect"/>
	/// has reported it uses are invalidated in the cache. You should reload
	/// your assets (textures, etc.) when this is called, if possible and relevant.
	/// </summary>
	void ReloadAssets() { }

	/// <summary>
	/// This method is called once per <see cref="Rate"/> ticks, and should
	/// be used to update your effect.
	/// </summary>
	/// <param name="time">The current GameTime</param>
	void Update(GameTime time);

	/// <summary>
	/// This method is called when an <see cref="IWeatherEffect"/> instance is
	/// about to be destroyed because it is no longer relevant and should be
	/// used to undo any relevant state.
	/// </summary>
	void Remove() { }

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
	string Id { get; set; }

	#region Display

	/// <summary>
	/// A display name to show the player when this weather condition
	/// should be referenced by name. This is a tokenizable string.
	/// </summary>
	string DisplayName { get; set; }

	#region Icon

	/// <summary>
	/// The name of the texture containing this weather's icon.
	/// </summary>
	string? IconTexture { get; set; }

	/// <summary>
	/// The location of this weather's icon within the texture. All
	/// weather icons are 12 by 8 pixels.
	/// </summary>
	Point IconSource { get; set; }

	#endregion

	#region Television

	/// <summary>
	/// The name of the texture containing this weather's TV animation.
	/// </summary>
	string? TVTexture { get; set; }

	/// <summary>
	/// The location of this weather's TV animation within the texture.
	/// Each frame of a TV animation is 13 by 13 pixels, and frames are
	/// arranged in a horizontal line.
	/// </summary>
	Point TVSource { get; set; }

	/// <summary>
	/// How many frames long this weather's TV animation is. Default is 4.
	/// </summary>
	int TVFrames { get; set; }

	/// <summary>
	/// How long should each frame of the TV animation be displayed?
	/// Default is 150.
	/// </summary>
	float TVSpeed { get; set; }

	/// <summary>
	/// A forecast string that will be displayed when the player checks
	/// tomorrow's forecast using a television. This is a
	/// tokenizable string. May be null.
	/// </summary>
	string? Forecast { get; set; }

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
	string? TotemMessage { get; set; }

	/// <summary>
	/// A sound to play immediately when using the totem. The base
	/// game's Rain Totem uses the <c>thunder</c> sound.
	/// </summary>
	string? TotemSound { get; set; }

	/// <summary>
	/// A sound to play 2 seconds after using a totem, as the animation
	/// ends. The base game's Rain Totem uses the <c>rainsound</c> sound.
	/// </summary>
	string? TotemAfterSound { get; set; }

	/// <summary>
	/// A color to flash the screen when using a totem. The base game's
	/// Rain Totem uses the color <c>slateblue</c>.
	/// </summary>
	Color? TotemScreenTint { get; set; }

	/// <summary>
	/// A texture to use for displaying particles when using a totem.
	/// The base game's Rain Totem uses the texture <c>LooseSprites\Cursors</c>
	/// </summary>
	string? TotemParticleTexture { get; set; }

	/// <summary>
	/// The source rectangle of the texture to use when displaying
	/// particles. Defaults to the entire texture. The base game's
	/// Rain Totem uses <c>648, 1045, 52, 33</c>.
	/// </summary>
	Rectangle? TotemParticleSource { get; set; }

	#endregion

	#endregion

	#region Behavior - Music

	/// <summary>
	/// If this is set to a non-null value, this weather condition will
	/// override the playing music in the same way the base game's raining
	/// condition does, using the named audio cue.
	/// </summary>
	string? MusicOverride { get; set; }

	/// <summary>
	/// The frequency the <see cref="MusicOverride"/> should be played at when
	/// the player is in an outside location.
	/// This does not affect all sound cues. Default is 100.
	/// </summary>
	float MusicFrequencyOutside { get; set; }

	/// <summary>
	/// The frequency the <see cref="MusicOverride"/> should be played at when
	/// the player is in an inside location.
	/// This does not affect all sound cues. Default is 100.
	/// </summary>
	float MusicFrequencyInside { get; set; }

	/// <summary>
	/// An optional list of <see cref="LocationMusicData"/> entries. If
	/// this is set, these will be used when <see cref="StardewValley.GameLocation.GetLocationSpecificMusic"/>
	/// is called in order to override music selection with more nuance than
	/// <see cref="MusicOverride"/> offers. This will override the behavior
	/// of <see cref="MusicOverride"/> if there is a matching entry.
	/// </summary>
	List<LocationMusicData>? SoftMusicOverrides { get; set; }

	#endregion

	#region Behavior - Vanilla Flags

	/// <summary>
	/// Controls the value of <see cref="LocationWeather.IsRaining"/>.
	/// </summary>
	bool IsRaining { get; set; }

	/// <summary>
	/// Controls the value of <see cref="LocationWeather.IsSnowing"/>.
	/// </summary>
	bool IsSnowing { get; set; }

	/// <summary>
	/// Controls the value of <see cref="LocationWeather.IsLightning"/>.
	/// </summary>
	bool IsLightning { get; set; }

	/// <summary>
	/// Controls the value of <see cref="LocationWeather.IsDebrisWeather"/>.
	/// </summary>
	bool IsDebrisWeather { get; set; }

	/// <summary>
	/// Controls the value of <see cref="LocationWeather.IsGreenRain"/>.
	/// </summary>
	bool IsGreenRain { get; set; }

	#endregion

	#region Behavior - Other

	/// <summary>
	/// Whether or not crops and pet bowls should be watered at the start of
	/// the day when this weather is active. If this is not set, the behavior
	/// will default based on the value of <see cref="IsRaining"/>.
	/// </summary>
	bool? WaterCropsAndPets { get; set; }

	/// <summary>
	/// If this is set to true, this weather will cause maps to use night
	/// tiles and to have darkened windows, similar to how rain functions.
	/// </summary>
	bool UseNightTiles { get; set; }

	/// <summary>
	/// Whether or not critters should attempt to spawn in outdoor maps
	/// while this weather condition is active. Defaults to true.
	/// </summary>
	bool SpawnCritters { get; set; }

	/// <summary>
	/// Whether or not frogs should attempt to spawn. If this is left at
	/// <c>null</c>, the default logic will be used. <see cref="SpawnCritters"/>
	/// must be enabled for frogs to spawn.
	/// </summary>
	bool? SpawnFrogs { get; set; }

	/// <summary>
	/// Whether or not cloud shadow critters should attempt to spawn. If
	/// this is left at <c>null</c>, the default logic will be used.
	/// <see cref="SpawnCritters"/> must be enabled for cloud shadows to spawn.
	/// </summary>
	bool? SpawnClouds { get; set; }

	#endregion

	#region Screen Tinting

	/// <summary>
	/// A list of this weather type's screen tint data points.
	/// </summary>
	IList<IScreenTintData> Lighting { get; }

	#endregion

	/// <summary>
	/// A list of this weather type's effects.
	/// </summary>
	public IList<IEffectData> Effects { get; }

	/// <summary>
	/// A list of this weather type's layers.
	/// </summary>
	public IList<ILayerData> Layers { get; }

}

public interface IScreenTintData {

	/// <summary>
	/// The unique Id for this set of screen tint information. This need only
	/// be unique within the <see cref="IWeatherData"/> containing it.
	/// </summary>
	string Id { get; set; }

	/// <summary>
	/// The time of day this screen tint should apply at. By using multiple
	/// screen tint data entries with different time of day values, it is
	/// possible to smoothly blend between them.
	///
	/// The default value, 600, means 6:00 AM. A value of 1830 means 6:30 PM / 18:30.
	///
	/// Setting this to zero or a negative value will automatically adjust
	/// the value based on the current location's truly-dark time, which is
	/// typically 2000 though it varies based on the season.
	/// </summary>
	int TimeOfDay { get; set; }

	/// <summary>
	/// A game state query for determining if this set of screen tint data
	/// should be active or not. 
	/// </summary>
	string? Condition { get; set; }

	/// <summary>
	/// How should this screen tint data be blended with the data around it
	/// in the list?
	/// </summary>
	LightingTweenMode TweenMode { get; set; }

	/// <summary>
	/// The ambient color that should be used for lighting when this weather
	/// condition is active. In the base game, only rainy weather uses this,
	/// with the color <c>255, 200, 80</c>.
	/// </summary>
	Color? AmbientColor { get; set; }

	/// <summary>
	/// The opacity to apply the ambient color with when outdoors. This changes
	/// based on the time of day, tweening to <c>0.93</c> when it becomes dark
	/// outside. The initial value is <c>0</c> if it is not raining,
	/// or <c>0.3</c> if it is raining.
	///
	/// Setting this will override the behavior as it becomes dark out, so
	/// you should use multiple lighting entries with different <see cref="TimeOfDay">
	/// values to implement your own shift, as appropriate.
	/// </summary>
	float? AmbientOutdoorOpacity { get; set; }

	/// <summary>
	/// This color, if set, is drawn to the screen in lighting mode during
	/// the Draw Lightmap phase of the world rendering. In the base game,
	/// only rainy weather uses this, with the color <c>orangered</c>.
	/// </summary>
	Color? LightingTint { get; set; }

	/// <summary>
	/// This opacity is pre-multiplied with <see cref="LightingTint"/>. You
	/// should use it if you want your tint to have an opacity, rather than
	/// directly using the alpha channel.
	///
	/// In the base game, rainy weather uses <c>0.45</c> for this value.
	/// </summary>
	float LightingTintOpacity { get; set; }

	/// <summary>
	/// This color, if set, is drawn as a screen overlay after the lighting
	/// phase of world rendering. In the base game, only rainy weather uses
	/// this, with the color <c>blue</c>. Green Rain uses the color
	/// <c>0, 120, 150</c>.
	/// </summary>
	Color? PostLightingTint { get; set; }

	/// <summary>
	/// This opacity is pre-multiplied with <see cref="PostLightingTint"/>.
	/// You should use it if you want your tint to have an opacity, rather
	/// than directly using the alpha channel.
	///
	/// In the base game, rainy weather uses <c>0.2</c> and green rain
	/// uses <c>0.22</c> for this value.
	/// </summary>
	float PostLightingTintOpacity { get; set; }

}


#region Effects

public interface IEffectData {

	/// <summary>
	/// An identifier for this specific effect within its parent
	/// weather condition. This need only be unique within the weather
	/// condition itself, so you can feel free to use Ids like <c>cold</c>
	/// </summary>
	string Id { get; set; }

	/// <summary>
	/// The type of effect this data represents.
	/// </summary>
	string Type { get; }

	/// <summary>
	/// How often should this effect update. A value of <c>60</c> is
	/// once per second. Defaults to <c>60</c>.
	/// </summary>
	uint Rate { get; set; }

	#region Conditions

	/// <summary>
	/// A condition that must evaluate to true for this effect to
	/// become active. If not set, this effect will always be active. This
	/// condition is only reevaluated upon location change, an event
	/// starting, or the hour changing.
	/// </summary>
	string? Condition { get; set; }

	/// <summary>
	/// If you set a group, only the first effect in a group will be
	/// active at any given time.
	/// </summary>
	string? Group { get; set; }

	/// <summary>
	/// The type(s) of maps that this effect should be active on. Defaults to Outdoors,
	/// unless you have a Condition checking "LOCATION_IS_INDOORS" or
	/// "LOCATION_IS_OUTDOORS" in order to maintain support for effects added
	/// before this property was added. You should remove those conditions in favor
	/// of setting this value, for performance reasons.
	/// </summary>
	public TargetMapType TargetMapType { get; set; }

	#endregion

}


public interface ICustomEffectData : IEffectData {

	/// <summary>
	/// A dictionary of configuration data for this effect, as parsed
	/// by the game's JSON serializer.
	/// </summary>
	IDictionary<string, JToken> Fields { get; }

}


public interface IBuffEffectData : IEffectData {

	/// <summary>
	/// The Id of the buff to apply to the player.
	/// </summary>
	string? BuffId { get; set; }

	/// <summary>
	/// A display name for this buff.
	/// </summary>
	string? DisplayName { get; set; }

	/// <summary>
	/// A description for this buff.
	/// </summary>
	string? Description { get; set; }

	/// <summary>
	/// The asset name for a texture containing this buff's icon.
	/// </summary>
	string? IconTexture { get; set; }

	/// <summary>
	/// The sprite index for this buff's icon within <see cref="IconTexture"/>.
	/// </summary>
	int IconSpriteIndex { get; set; }

	/// <summary>
	/// The glow color to apply to players when they have this buff.
	/// </summary>
	Color? GlowColor { get; set; }

	/// <summary>
	/// Whether this buff counts as a debuff or not.
	/// </summary>
	bool? IsDebuff { get; set; }

	/// <summary>
	/// The duration, in milliseconds, for which this buff should remain
	/// on the player after the effect is no longer active. This can be
	/// set to <c>-2</c> for a buff that should last for the rest of the day.
	/// </summary>
	int LingerDuration { get; set; }

	/// <summary>
	/// The maximum duration the buff should remain on after the effect
	/// is no longer active. If this is set to a number larger than
	/// <see cref="LingerDuration"/>, then a random value between
	/// <see cref="LingerDuration"/> and <see cref="LingerMaxDuration"/>
	/// will be selected.
	/// </summary>
	int LingerMaxDuration { get; set; }

	/// <summary>
	/// Extra attributes to apply for this buff.
	/// </summary>
	BuffAttributesData? Effects { get; set; }

	/// <summary>
	/// The custom fields for this buff. These may be used for, as an
	/// example, adding SpaceCore skills to a buff.
	/// </summary>
	Dictionary<string, string> CustomFields { get; }

}


public interface IModifyHealthEffectData : IEffectData {

	/// <summary>
	/// The chance that this effect applies on any given update.
	/// </summary>
	float Chance { get; set; }

	/// <summary>
	/// The amount to change the player's health by when this effect applies.
	/// </summary>
	int Amount { get; set; }

	/// <summary>
	/// The minimum value the player's health can reach. Please note that
	/// this is a rough value, as the <see cref="StardewValley.Farmer.takeDamage(int, bool, StardewValley.Monsters.Monster)"/>
	/// method has a randomness factor to how much damage is actually applied.
	/// </summary>
	int MinValue { get; set; }

	/// <summary>
	/// The maximum value the player's health can reach.
	/// </summary>
	int MaxValue { get; set; }

}


public interface IModifyStaminaEffectData : IEffectData {

	/// <summary>
	/// The chance that this effect applies on any given update.
	/// </summary>
	float Chance { get; set; }

	/// <summary>
	/// The amount to change the player's stamina by when this effect applies.
	/// </summary>
	int Amount { get; set; }

	/// <summary>
	/// The minimum value the player's stamina can reach.
	/// </summary>
	int MinValue { get; set; }

	/// <summary>
	/// The maximum value the player's stamina can reach.
	/// </summary>
	int MaxValue { get; set; }

}


public interface ITriggerEffectData : IEffectData {

	/// <summary>
	/// The actions to run whenever this effect becomes active.
	/// </summary>
	List<string> ApplyActions { get; }

	/// <summary>
	/// The actions to run whenever this effect updates. This happens
	/// every <see cref="IEffectData.Rate"/> ticks at most.
	/// </summary>
	List<string> Actions { get; }

	/// <summary>
	/// The actions to run whenever this effect is removed.
	/// </summary>
	List<string> RemoveActions { get; }

}


#endregion

#region Layers

[Flags]
public enum TargetMapType {
	Outdoors = 1,
	Indoors = 2
}


public enum LayerDrawType {
	Normal,
	Lighting
}


public interface ILayerData {

	/// <summary>
	/// An identifier for this specific layer within its parent weather
	/// condition. This need only be unique within the weather
	/// condition itself, so you can feel free to use Ids like <c>rain</c>
	/// </summary>
	string Id { get; set; }

	/// <summary>
	/// The type of layer this data represents.
	/// </summary>
	string Type { get; }

	#region Conditions

	/// <summary>
	/// A condition that must evaluate to true for this layer to be displayed.
	/// If not set, the layer will always be displayed. This condition is
	/// only reevaluated upon location change or the hour changing.
	/// </summary>
	public string? Condition { get; set; }

	/// <summary>
	/// If you set a group, only the first layer in a group will be
	/// displayed at any given time. This can be used to make layers that
	/// display in some situations, with fall-back layers in other situations.
	/// </summary>
	public string? Group { get; set; }

	/// <summary>
	/// The type(s) of maps that this layer should render on. Defaults to Outdoors.
	/// </summary>
	public TargetMapType TargetMapType { get; set; }

	#endregion

	#region Shared Rendering

	public LayerDrawType Mode { get; set; }

	#endregion

}


public interface ICustomLayerData : ILayerData {

	/// <summary>
	/// A dictionary of configuration data for this layer, as parsed
	/// by the game's JSON serializer.
	/// </summary>
	IDictionary<string, JToken> Fields { get; }

}


public interface IColorLayerData : ILayerData {

	/// <summary>
	/// The color this color layer should draw to the screen.
	/// </summary>
	Color? Color { get; set; }

	/// <summary>
	/// The opacity to draw the color with.
	/// </summary>
	float Opacity { get; set; }

}

public interface IDebrisLayerData : ILayerData {

	/// <summary>
	/// As an alternative to providing your own <see cref="Texture"/>
	/// and <see cref="Sources"/>, you can use the game's built-in
	/// seasonal debris sprites. If this is set to <c>-1</c>, it
	/// will use the current season. Setting this to <c>999</c> has
	/// a special easter egg behavior that will use random objects.
	/// </summary>
	int UseSeasonal { get; set; }

	/// <summary>
	/// The asset name of a texture to use for drawing this
	/// debris layer. If this isn't set, we will fall back
	/// to using the native seasonal sprites as described
	/// by <see cref="UseSeasonal"/>.
	/// </summary>
	string? Texture { get; set; }

	/// <summary>
	/// A list of source rectangles to use for the particles of
	/// this debris layer. Each of these rectangles is specifically
	/// for the first frame of animation.
	///
	/// If <see cref="ShouldAnimate"/> is true, it is assumed that
	/// we have multiple frames and that they are laid out in a
	/// horizontal line. We expect the same number of sprites that
	/// the base game's debris sprites have.
	/// </summary>
	List<Rectangle>? Sources { get; set; }

	/// <summary>
	/// When set to true, this debris layer's sprites will be
	/// flipped horizontally when drawn.
	/// </summary>
	bool FlipHorizontal { get; set; }

	/// <summary>
	/// When set to true, this debris layer's sprites will be
	/// flipped vertically when drawn.
	/// </summary>
	bool FlipVertical { get; set; }

	/// <summary>
	/// The color to draw this debris layer's sprites with.
	/// </summary>
	Color? Color { get; set; }

	/// <summary>
	/// The opacity to draw this debris layer's sprites with.
	/// </summary>
	float Opacity { get; set; }

	/// <summary>
	/// The scale to draw this debris layer's sprites at.
	/// </summary>
	float Scale { get; set; }

	/// <summary>
	/// Whether or not the particles for this debris layer can
	/// enter a blowing state, where they move upwards.
	/// </summary>
	bool CanBlow { get; set; }

	/// <summary>
	/// The minimum number of particles to spawn for this
	/// debris layer.
	/// </summary>
	int MinCount { get; set; }

	/// <summary>
	/// The maximum number of particles to spawn for this
	/// debris layer.
	/// </summary>
	int MaxCount { get; set; }

	/// <summary>
	/// The minimum amount of time any frame should be displayed,
	/// in milliseconds.
	/// </summary>
	int MinTimePerFrame { get; set; }

	/// <summary>
	/// The maximum amount of time any frame should be displayed,
	/// in milliseconds.
	/// </summary>
	int MaxTimePerFrame { get; set; }

	/// <summary>
	/// Whether or not this debris layer should draw animated sprites.
	/// If this is set to false, only the first frame will ever
	/// be drawn.
	/// </summary>
	bool ShouldAnimate { get; set; }

}


public interface IShaderLayerData : ILayerData {

	/// <summary>
	/// The name of a shader to use. This should either be the Id of
	/// a built-in shader, or the absolute file path of a shader file.
	/// </summary>
	string? Shader { get; set; }

	/// <summary>
	/// An optional color to draw the shader with. This may be unsupported
	/// depending on the shader.
	/// </summary>
	Color? Color { get; set; }

	/// <summary>
	/// A dictionary of configuration data for this shader, as parsed
	/// by the game's JSON serializer.
	/// </summary>
	IDictionary<string, JToken> Fields { get; }

}


public interface IRainLayerData : ILayerData {

	/// <summary>
	/// The asset name of a texture to use for drawing this rain layer. If
	/// this is not set, then this rain layer will use the game's native
	/// rain sprites.
	/// </summary>
	string? Texture { get; set; }

	/// <summary>
	/// A source rectangle for the first frame of the rain animation within
	/// the provided texture. If <see cref="Texture"/> is not set, this is
	/// ignored and the source is automatically determined using the game's
	/// native rain sprites.
	/// </summary>
	Rectangle? Source { get; set; }

	/// <summary>
	/// How many frames of animation the provided rain texture has. This is
	/// ignored if <see cref="Texture"/> is not set.
	/// </summary>
	int Frames { get; set; }

	/// <summary>
	/// When set to true, this rain layer's sprites will be
	/// flipped horizontally when drawn.
	/// </summary>
	bool FlipHorizontal { get; set; }

	/// <summary>
	/// When set to true, this rain layer's sprites will be
	/// flipped vertically when drawn.
	/// </summary>
	bool FlipVertical { get; set; }

	/// <summary>
	/// The color to draw this rain layer's sprites with.
	/// </summary>
	Color? Color { get; set; }

	/// <summary>
	/// The opacity to draw this rain layer's sprites with.
	/// </summary>
	float Opacity { get; set; }

	/// <summary>
	/// The scale to draw this rain layer's sprites at.
	/// </summary>
	float Scale { get; set; }

	/// <summary>
	/// How many times to draw this rain layer's sprites per drawing
	/// pass. The base game's Green Rain effect uses this with a
	/// value of <c>2</c>. All other rain uses a value of <c>1</c>.
	/// </summary>
	int Vibrancy { get; set; }

	/// <summary>
	/// How many rain particles to draw. This can be used to make the
	/// rain lighter or heavier.
	/// </summary>
	int Count { get; set; }

	/// <summary>
	/// The speed this rain layer's particles should move.
	/// </summary>
	Vector2 Speed { get; set; }

}


public interface ITextureScrollLayerData : ILayerData {

	/// <summary>
	/// Whether or not this is a snow layer. When this is true, <see cref="Texture"/>
	/// and <see cref="Source"/> will have default values that use the base game's
	/// native snow sprites. Additionally, when this is true, <see cref="Opacity"/>
	/// will be multiplied by the player's snow transparency setting.
	/// </summary>
	bool IsSnow { get; set; }

	/// <summary>
	/// The asset name of a texture to use for drawing this layer. This is required
	/// if <see cref="IsSnow"/> is false.
	/// </summary>
	string? Texture { get; set; }

	/// <summary>
	/// A source rectangle for the first frame of the animation within
	/// the provided texture. If <see cref="Texture"/> is not set, this is
	/// ignored and the source is automatically determined using the game's
	/// native snow sprites.
	/// </summary>
	Rectangle? Source { get; set; }

	/// <summary>
	/// How many frames of animation the provided texture has. This is
	/// ignored if <see cref="Texture"/> is not set.
	/// </summary>
	int? Frames { get; set; }

	/// <summary>
	/// How long each frame should be displayed, in milliseconds.
	/// </summary>
	int TimePerFrame { get; set; }

	/// <summary>
	/// When set to true, this layer's sprites will be
	/// flipped horizontally when drawn.
	/// </summary>
	bool FlipHorizontal { get; set; }

	/// <summary>
	/// When set to true, this layer's sprites will be
	/// flipped vertically when drawn.
	/// </summary>
	bool FlipVertical { get; set; }

	/// <summary>
	/// The color to draw this layer's sprites with.
	/// </summary>
	Color? Color { get; set; }

	/// <summary>
	/// The opacity to draw this layer's sprites with. If <see cref="IsSnow"/>
	/// is true, this will be multiplied by the user's snow transparency setting.
	/// </summary>
	float Opacity { get; set; }

	/// <summary>
	/// The scale to draw this layer's sprites at.
	/// </summary>
	float Scale { get; set; }

	/// <summary>
	/// The speed this layer's position should change.
	/// </summary>
	Vector2 Speed { get; set; }

	/// <summary>
	/// The speed the layer's position should change, relative to the movement
	/// of the viewport.
	/// </summary>
	Vector2 ViewSpeed { get; set; }

}


#endregion




/// <summary>
/// This data model represents the custom data we associate with
/// location contexts. As we've got a lot of it, we use an extension
/// data object rather than <see cref="LocationContextData.CustomFields"/>
/// </summary>
public interface ILocationContextExtensionData {

	/// <summary>
	/// The Id of the location context this model extends. This should
	/// match a <see cref="LocationContextData"/> entry in
	/// `Data/LocationContexts`
	/// </summary>
	string Id { get; set; }

	/// <summary>
	/// A display name for this location context, to be used when
	/// this location context should be presented to the user in
	/// some way. As an example, the TV's Weather channel uses
	/// this field when listing the available location contexts.
	///
	/// This is a tokenizable string.
	/// </summary>
	string? DisplayName { get; set; }

	/// <summary>
	/// A map of weather type Ids to whether or not a custom weather totem
	/// for that weather type should be allowed to work in this
	/// location context.
	/// </summary>
	Dictionary<string, bool> AllowWeatherTotem { get; set; }

	#region TV: Weather Channel

	/// <summary>
	/// Whether or not this location context should be included in the
	/// TV's Weather channel.
	/// </summary>
	bool IncludeInWeatherChannel { get; set; }

	/// <summary>
	/// An optional game state query for determining whether or not
	/// the player should currently see this location context in
	/// the weather channel's menu.
	/// </summary>
	string? WeatherChannelCondition { get; set; }

	/// <summary>
	/// An optional string that is pre-pended to the forecast message
	/// for the current weather condition.
	/// </summary>
	string? WeatherForecastPrefix { get; set; }

	#region Textures

	/// <summary>
	/// Optional. An asset name for a texture that should be displayed
	/// as the background of the weather channel when viewing the weather
	/// for this location context. This can be used to change the
	/// background behind the meteorologist.
	/// </summary>
	string? WeatherChannelBackgroundTexture { get; set; }

	/// <summary>
	/// Optional. The position of the top-left corner of the first frame
	/// of the weather channel background. Each background frame is 42 by 28
	/// pixels. Ignored if <see cref="WeatherChannelBackgroundTexture"/> is
	/// not set.
	/// </summary>
	Point WeatherChannelBackgroundSource { get; set; }

	/// <summary>
	/// Optional. How many frames should the background of the weather
	/// channel have? Default is 1. Ignored if
	/// <see cref="WeatherChannelBackgroundTexture"/> is not set.
	/// </summary>
	int WeatherChannelBackgroundFrames { get; set; }

	/// <summary>
	/// Optional. How long should each frame of the background be displayed?
	/// Default is 150f. Ignored if <see cref="WeatherChannelBackgroundTexture"/>
	/// is not set.
	/// </summary>
	float WeatherChannelBackgroundSpeed { get; set; }

	/// <summary>
	/// Optional. An asset name for a texture that should be displayed
	/// as the foreground of the weather channel when viewing the weather
	/// for this location context. This can be used to change the appearance
	/// of the meteorologist.
	/// </summary>
	string? WeatherChannelOverlayTexture { get; set; }

	/// <summary>
	/// Optional. The position of the top-left corner of the first frame
	/// of the weather channel intro overlay. Each overlay frame is 42 by 28
	/// pixels. Ignored if <see cref="WeatherChannelOverlayTexture"/>
	/// is not set.
	///
	/// The frames are assumed to be arranged in a horizontal line.
	/// 
	/// The intro overlay is the overlay displayed before the actual weather
	/// appears on the screen.
	/// </summary>
	Point WeatherChannelOverlayIntroSource { get; set; }

	/// <summary>
	/// Optional. How many frames should the intro overlay of the weather
	/// channel have? Default is 1. Ignored if
	/// <see cref="WeatherChannelOverlayTexture"/> is not set.
	/// </summary>
	int WeatherChannelOverlayIntroFrames { get; set; }

	/// <summary>
	/// Optional. How long should each frame of the intro overlay be
	/// displayed? Default is 150f. Ignored if
	/// <see cref="WeatherChannelOverlayTexture"/> is not set.
	/// </summary>
	float WeatherChannelOverlayIntroSpeed { get; set; }

	/// <summary>
	/// Optional. The position of the top-left corner of the first
	/// frame of the weather channel overlay that is displayed
	/// when the actual weather is being displayed. Each overlay
	/// frame is 42 by 28 pixels.
	///
	/// The weather overlay is the overlay displayed when the actual
	/// weather is appearing on screen.
	///
	/// If this is not set, it will be assumed that it directly
	/// follows the intro overlay's frames.
	/// </summary>
	Point? WeatherChannelOverlayWeatherSource { get; set; }

	/// <summary>
	/// Optional. How many frames should the weather overlay of
	/// the weather channel have? Default is 1. Ignored if
	/// <see cref="WeatherChannelOverlayTexture"/> is not set.
	/// </summary>
	int WeatherChannelOverlayWeatherFrames { get; set; }

	/// <summary>
	/// Optional. How long should each frame of the weather overlay
	/// be displayed? Default is 150f. Ignored if
	/// <see cref="WeatherChannelOverlayTexture"/> is not set.
	/// </summary>
	float WeatherChannelOverlayWeatherSpeed { get; set; }

	#endregion

	#endregion

}
