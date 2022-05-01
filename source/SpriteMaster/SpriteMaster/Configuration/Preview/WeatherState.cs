/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Extensions;
using StardewValley;
using System;
using System.Collections.Generic;
using static SpriteMaster.Harmonize.Patches.Game.Snow;

namespace SpriteMaster.Configuration.Preview;

struct WeatherState : IDisposable {
	// SDV
	internal readonly List<WeatherDebris>? DebrisWeather { get; init; }
	internal readonly RainDrop[]? RainDrops { get; init; }
	internal readonly XNA.Vector2 SnowPos { get; init; }
	internal readonly float GlobalWind { get; init; }
	internal readonly float WindGust { get; init; }
	internal readonly bool IsDebrisWeather { get; init; }
	internal readonly bool IsRaining { get; init; }
	internal readonly bool IsSnowing { get; init; }
	internal readonly bool IsLightning { get; init; }

	// SM
	internal readonly SnowState SnowWeatherState { get; init; }

	internal static WeatherState Backup() => new() {
		IsDebrisWeather = Game1.isDebrisWeather,
		DebrisWeather = Game1.debrisWeather is null ? null : new(Game1.debrisWeather),
		GlobalWind = WeatherDebris.globalWind,
		WindGust = Game1.windGust,
		RainDrops = Game1.rainDrops.Clone<RainDrop>(),
		IsRaining = Game1.isRaining,
		IsSnowing = Game1.isSnowing,
		IsLightning = Game1.isLightning,
		SnowPos = Game1.snowPos,

		SnowWeatherState = SnowState.Backup()
	};

	internal readonly void Restore() {
		Game1.isDebrisWeather = IsDebrisWeather;
		Game1.debrisWeather = DebrisWeather;
		WeatherDebris.globalWind = GlobalWind;
		Game1.windGust = WindGust;
		Game1.rainDrops = RainDrops;
		Game1.isRaining = IsRaining;
		Game1.isSnowing = IsSnowing;
		Game1.isLightning = IsLightning;
		Game1.snowPos = SnowPos;

		SnowWeatherState.Restore();
	}

	public readonly void Dispose() => Restore();
}
