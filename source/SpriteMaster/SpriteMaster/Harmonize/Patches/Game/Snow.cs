/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

#define USE_MULTIDRAW

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Configuration;
using SpriteMaster.Configuration.Preview;
using SpriteMaster.Core;
using SpriteMaster.Extensions;
using SpriteMaster.Types;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;

namespace SpriteMaster.Harmonize.Patches.Game;

internal static class Snow {
	private static Dictionary<Bounds, List<SnowWeatherDebris>> MappedWeatherDebris = new();
	private static List<WeatherDebris> AllWeatherDebris = new();

	internal readonly struct SnowState {
		internal Dictionary<Bounds, List<SnowWeatherDebris>> MappedWeatherDebris { get; init; }
		internal List<WeatherDebris> AllWeatherDebris { get; init; }

		internal static SnowState Backup() => new() {
			MappedWeatherDebris = new(Snow.MappedWeatherDebris),
			AllWeatherDebris = new(Snow.AllWeatherDebris)
		};

		internal void Restore() {
			Snow.MappedWeatherDebris = MappedWeatherDebris;
			Snow.AllWeatherDebris = AllWeatherDebris;
		}
	}

	internal sealed class SnowWeatherDebris : WeatherDebris {
		internal float Rotation;
		internal readonly float RotationRate;
		internal readonly float Scale;

		internal SnowWeatherDebris(XVector2 position, int which, float rotationVelocity, float dx, float dy, float rotation, float rotationRate, float scale) :
			base(position, which, rotationVelocity, dx, dy) {
			Rotation = rotation;
			RotationRate = rotationRate;
			Scale = scale;
		}

		internal void Update() {
			update();
			Rotation = (Rotation + RotationRate) % (2.0f * MathF.PI);
		}
	}

	private static bool IsPuffersnow = false;

	private static bool ShouldDrawSnow => Game1.IsSnowingHere() && Game1.currentLocation.IsOutdoors && Game1.currentLocation is not Desert;

	private static readonly Lazy<XTexture2D> FishTexture = new(() => SpriteMaster.Self.Helper.GameContent.Load<XTexture2D>(@"LooseSprites\AquariumFish"));

	public readonly record struct DrawWeatherState(bool Overridden, PrecipitationType? PreviousOverride);

	[Harmonize(
		typeof(Game1),
		"drawWeather",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		critical: false
	)]
	public static bool DrawWeather(Game1 __instance, GameTime? time, RenderTarget2D? target_screen, ref DrawWeatherState __state) {
		__state = new(false, null);

		if (!Config.IsEnabled || !Config.Extras.Snow.Enabled) {
			return true;
		}

		bool isPrecipitationSnow = Scene.Current?.Precipitation == PrecipitationType.Snow;

		if ((!ShouldDrawSnow || Scene.Current is not null) && !isPrecipitationSnow) {
			return true;
		}

		if (MappedWeatherDebris.Count == 0) {
			return true;
		}

		var batchSortMode = DrawState.CurrentSortMode;
		var batchBlendState = DrawState.CurrentBlendState;
		var batchSamplerState = DrawState.CurrentSamplerState;

		Game1.spriteBatch.End();
		Game1.spriteBatch.Begin(
			sortMode: IsPuffersnow ? SpriteSortMode.Texture : SpriteSortMode.Deferred,
			blendState: IsPuffersnow ? BlendState.AlphaBlend : BlendState.Additive,
			samplerState: Config.Resample.IsEnabled ? SamplerState.LinearClamp : SamplerState.PointClamp,
			rasterizerState: Game1.spriteBatch.GraphicsDevice.RasterizerState
		);

		try {
			if (__instance.takingMapScreenshot) {
				XTexture2D drawTexture = IsPuffersnow ? FishTexture.Value : Game1.mouseCursors;

				foreach (var _item in AllWeatherDebris) {
					var item = (SnowWeatherDebris)_item;
					var position = new XVector2(
						Game1.random.Next(Game1.viewport.Width - item.sourceRect.Width * 3),
						Game1.random.Next(Game1.viewport.Height - item.sourceRect.Height * 3)
					);

					Game1.spriteBatch.Draw(
						texture: drawTexture,
						position: position,
						sourceRectangle: IsPuffersnow ? new(0, 0, 24, 24) : item.sourceRect,
						color: XColor.White,
						rotation: item.Rotation,
						origin: XVector2.Zero,
						scale: item.Scale,
						effects: SpriteEffects.None,
						layerDepth: 1E-06f
					);
				}
			}
			else if (Game1.viewport.X > -Game1.viewport.Width) {
				if (IsPuffersnow) {
					XTexture2D drawTexture = FishTexture.Value;

					foreach (var weatherDebris in MappedWeatherDebris.Values) {
						foreach (SnowWeatherDebris item in weatherDebris) {
							Game1.spriteBatch.Draw(
								texture: drawTexture,
								position: item.position,
								sourceRectangle: new(0, 0, 24, 24),
								color: XColor.White,
								rotation: item.Rotation,
								origin: XVector2.Zero,
								scale: item.Scale,
								effects: SpriteEffects.None,
								layerDepth: 1E-06f
							);
						}
					}
				}
				else {
					XTexture2D drawTexture = Game1.mouseCursors;

					foreach (var (source, list) in MappedWeatherDebris) {
#if !USE_MULTIDRAW
						foreach (SnowWeatherDebris item in list) {
							Game1.spriteBatch.Draw(
								texture: drawTexture,
								position: item.position,
								sourceRectangle: source, 
								color: XColor.White,
								rotation: item.Rotation,
								origin: XVector2.Zero,
								scale: item.Scale,
								effects: SpriteEffects.None,
								layerDepth: 1E-06f
							);
						}
#else
						Span<OnDrawImpl.DrawInstance> instances = stackalloc OnDrawImpl.DrawInstance[list.Count];
						for (int i = 0; i < instances.Length; ++i) {
							instances[i] = new() {
								Position = list[i].position,
								Scale = list[i].Scale,
								Rotation = list[i].Rotation
							};
						}

						Game1.spriteBatch.DrawMulti(
							texture: drawTexture,
							source: source,
							color: XColor.White,
							origin: XVector2.Zero,
							effects: SpriteEffects.None,
							layerDepth: 1E-06F,
							instances: instances
						);
#endif
					}
				}
			}
		}
		finally {
			Game1.spriteBatch.End();
			Game1.spriteBatch.Begin(batchSortMode, batchBlendState, batchSamplerState);
		}

		__state = new(true, PrecipitationPatches.PrecipitationOverride);
		PrecipitationPatches.PrecipitationOverride = PrecipitationType.None;
		Game1.netWorldState.Value.GetWeatherForLocation(GameLocation.LocationContext.Default).isSnowing.Value = false;
		return true;
	}

	[Harmonize(
		typeof(Game1),
		"drawWeather",
		Harmonize.Fixation.Postfix,
		Harmonize.PriorityLevel.First,
		critical: false
	)]
	public static void DrawWeather(Game1 __instance, GameTime time, RenderTarget2D target_screen, DrawWeatherState __state) {
		if (!__state.Overridden) {
			return;
		}

		PrecipitationPatches.PrecipitationOverride = __state.PreviousOverride;
		Game1.netWorldState.Value.GetWeatherForLocation(GameLocation.LocationContext.Default).isSnowing.Value = true;
	}

	private static float PreviousWind = 0.0f;

	[Harmonize(
		typeof(Game1),
		"updateWeather",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		instance: false,
		critical: false
	)]
	public static bool UpdateWeather(GameTime time) {
		if (!Config.IsEnabled || !Config.Extras.Snow.Enabled) {
			return true;
		}

		bool isPrecipitationSnow = Scene.Current?.Precipitation == PrecipitationType.Snow;

		if ((!ShouldDrawSnow || Scene.Current is not null) && !isPrecipitationSnow) {
			return true;
		}

		if (Game1.windGust == 0f) {
			if (Game1.random.NextDouble() < 0.001) {
				Game1.windGust += Game1.random.Next(-10, -1) / 100f;
				PreviousWind = WeatherDebris.globalWind;
				WeatherDebris.globalWind += Game1.windGust;
				if (Game1.soundBank is not null) {
					Game1.wind = Game1.soundBank.GetCue("wind");
					Game1.wind.Play();
				}
			}
		}
		else {
			if (Game1.random.NextDouble() < 0.007) {
				Game1.windGust = 0f;
				WeatherDebris.globalWind = PreviousWind;
			}
		}

		// Update particles
		foreach (var item in AllWeatherDebris) {
			(item as SnowWeatherDebris)!.Update();
		}

		if (Scene.Current is null) {
			Game1.updateDebrisWeatherForMovement(AllWeatherDebris);
		}

		return true;
	}

	[Harmonize(
		typeof(Game1),
		"updateRainDropPositionForPlayerMovement",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		instance: false,
		critical: false
	)]
	public static bool UpdateRainDropPositionForPlayerMovement(int direction, bool overrideConstraints, float speed) {
		if (!Config.IsEnabled || !Config.Extras.Snow.Enabled) {
			return true;
		}

		if (!ShouldDrawSnow) {
			return true;
		}

		if (
			!overrideConstraints && (
				!Game1.IsSnowingHere() ||
				!Game1.currentLocation.IsOutdoors ||
				direction is not (0 or 2) &&
				(
					Game1.player.getStandingX() < Game1.viewport.Width / 2 ||
					Game1.player.getStandingX() > Game1.currentLocation.Map.DisplayWidth - Game1.viewport.Width / 2) ||
					direction is not (1 or 3) && (
						Game1.player.getStandingY() < Game1.viewport.Height / 2 ||
						Game1.player.getStandingY() > Game1.currentLocation.Map.DisplayHeight - Game1.viewport.Height / 2
					)
				)
			) {
			return true;
		}

		Game1.updateDebrisWeatherForMovement(AllWeatherDebris, direction, overrideConstraints, speed);
		return true;
	}

	private const int ReferenceScreenArea = 2560 * 1440;

	[Harmonize(
		typeof(Game1),
		"populateDebrisWeatherArray",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		instance: false,
		critical: false
	)]
	public static bool PopulateDebrisWeatherArray() {
		if (!Config.IsEnabled || !Config.Extras.Snow.Enabled) {
			return true;
		}

		if (!Game1.IsSnowingHere()) {
			return true;
		}

		PopulateWeather((Game1.viewport.Width, Game1.viewport.Height));

		return true;
	}

	internal static void PopulateWeather(Vector2I screenSize) {
		if (!Config.IsEnabled || !Config.Extras.Snow.Enabled) {
			return;
		}

		IsPuffersnow = Game1.random.NextDouble() < Config.Extras.Snow.PuffersnowChance;

		Game1.isDebrisWeather = true;
		int debrisToMake = Game1.random.Next(Config.Extras.Snow.MinimumDensity, Config.Extras.Snow.MaximumDensity);
		int currentScreenArea = screenSize.Width * screenSize.Height;
		double ratio = (double)currentScreenArea / ReferenceScreenArea;
		debrisToMake = (debrisToMake * ratio).RoundToInt();
		MappedWeatherDebris.Clear();
		AllWeatherDebris.Clear();
		for (int i = 0; i < debrisToMake; i++) {
			var debris = new SnowWeatherDebris(
				position: new XVector2(Game1.random.Next(0, screenSize.Width), Game1.random.Next(0, screenSize.Height)),
				which: 3,
				rotationVelocity: 0.0f,
				dx: Game1.random.Next(-10, 0) / 50f,
				dy: Game1.random.Next(10) / 50f,
				rotation: (float)(Game1.random.NextDouble() * 2.0 * Math.PI),
				rotationRate: (float)(((Game1.random.NextDouble() * 2.0) - 1.0) * Config.Extras.Snow.MaximumRotationSpeed),
				scale: (float)Math.Sqrt((Game1.random.NextDouble() * 0.75) + 0.25) * Config.Extras.Snow.MaximumScale
			);

			if (!MappedWeatherDebris.TryGetValue(debris.sourceRect, out var mappedList)) {
				MappedWeatherDebris.Add(debris.sourceRect, mappedList = new List<SnowWeatherDebris>());
			}
			mappedList.Add(debris);
			AllWeatherDebris.Add(debris);
		}
		AllWeatherDebris.Sort((d1, d2) => {
			var sourceDiff = d1.sourceRect.GetHashCode() - d2.sourceRect.GetHashCode();
			if (sourceDiff != 0) {
				return sourceDiff;
			}
			return ((SnowWeatherDebris)d1).Scale.CompareTo(((SnowWeatherDebris)d2).Scale);
		});

		foreach (var mappedList in MappedWeatherDebris.Values) {
			mappedList.Sort((d1, d2) => d1.Scale.CompareTo(d2.Scale));
		}
	}

	internal static void OnWindowResized(Vector2I size) {
		if (!Config.IsEnabled || !Config.Extras.Snow.Enabled) {
			return;
		}

		if (Game1.IsSnowingHere()) {
			PopulateDebrisWeatherArray();
		}
	}
}
