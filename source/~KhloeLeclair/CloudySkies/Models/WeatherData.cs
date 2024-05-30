/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System.Collections.Generic;

using Leclair.Stardew.CloudySkies.LayerData;
using Leclair.Stardew.Common.Serialization;
using Leclair.Stardew.Common.Serialization.Converters;

using Microsoft.Xna.Framework;

using Newtonsoft.Json;

using StardewValley.GameData.Locations;

namespace Leclair.Stardew.CloudySkies.Models;

public class WeatherData : IWeatherData {

	public string Id { get; set; } = string.Empty;

	public string DisplayName { get; set; } = string.Empty;

	public string? TotemMessage { get; set; }

	public string? TotemSound { get; set; }

	public string? TotemAfterSound { get; set; }

	[JsonConverter(typeof(ColorConverter))]
	public Color? TotemScreenTint { get; set; }

	public string? TotemParticleTexture { get; set; }

	public Rectangle? TotemParticleSource { get; set; }

	public string? Forecast { get; set; }

	public Dictionary<string, string>? ForecastByContext { get; set; }

	#region Icons

	public string? IconTexture { get; set; }
	public Point IconSource { get; set; } = Point.Zero;

	public string? TVTexture { get; set; }
	public Point TVSource { get; set; } = Point.Zero;
	public int TVFrames { get; set; } = 4;

	public float TVSpeed { get; set; } = 150f;

	#endregion

	#region Music

	public string? MusicOverride { get; set; }

	public float MusicFrequencyOutside { get; set; } = 100f;

	public float MusicFrequencyInside { get; set; } = 100f;

	public List<LocationMusicData>? SoftMusicOverrides { get; set; }

	#endregion

	#region Vanilla Flags

	public bool IsRaining { get; set; } = false;

	public bool IsSnowing { get; set; } = false;

	public bool IsLightning { get; set; } = false;

	public bool IsDebrisWeather { get; set; } = false;

	public bool IsGreenRain { get; set; } = false;

	#endregion

	#region Behavior Modifications

	public bool? WaterCropsAndPets { get; set; }

	public bool UseNightTiles { get; set; } = false;

	public bool SpawnCritters { get; set; } = true;

	public bool? SpawnFrogs { get; set; }

	public bool? SpawnClouds { get; set; }

	#endregion

	#region Screen Tint

	[JsonConverter(typeof(AbstractListConverter<ScreenTintData, IScreenTintData>))]
	public IList<IScreenTintData> Lighting { get; set; } = new List<IScreenTintData>();

	#region Legacy Screen Ting

	[JsonConverter(typeof(ColorConverter))]
	public Color? AmbientColor { get; set; }

	public float? AmbientOutdoorOpacity { get; set; }


	[JsonConverter(typeof(ColorConverter))]
	public Color? LightingTint { get; set; }

	public float LightingTintOpacity { get; set; } = 1f;

	[JsonConverter(typeof(ColorConverter))]
	public Color? PostLightingTint { get; set; }

	public float PostLightingTintOpacity { get; set; } = 1f;

	#endregion

	#endregion

	#region Layers and Effects

	[JsonConverter(typeof(AbstractListConverter<BaseEffectData, IEffectData>))]
	public IList<IEffectData> Effects { get; set; } = new List<IEffectData>();

	[JsonConverter(typeof(AbstractListConverter<BaseLayerData, ILayerData>))]
	public IList<ILayerData> Layers { get; set; } = new List<ILayerData>();
	//public List<BaseLayerData> Layers { get; set; } = new();

	#endregion

}
