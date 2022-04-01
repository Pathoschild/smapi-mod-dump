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
using SpriteMaster.Core;
using SpriteMaster.Types;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;

namespace SpriteMaster.Harmonize.Patches.Game;

static class Snow {
	private static List<WeatherDebris>? LastDebrisWeather = null;
	private static Dictionary<Bounds, List<SnowWeatherDebris>> MappedWeatherDebris = new();

	private sealed class SnowWeatherDebris : StardewValley.WeatherDebris {
		internal float Rotation;
		internal readonly float RotationRate;
		internal readonly float Scale;

		internal SnowWeatherDebris(Vector2 position, int which, float rotationVelocity, float dx, float dy, float rotation, float rotationRate, float scale) :
			base(position, which, rotationVelocity, dx, dy) {
			Rotation = rotation;
			RotationRate = rotationRate;
			Scale = scale;
		}

		internal void Update() {
			base.update();
			Rotation = (Rotation + RotationRate) % (2.0f * MathF.PI);
		}
	}

	private static bool IsPuffersnow = false;

	private static bool ShouldDrawSnow => Game1.IsSnowingHere() && Game1.currentLocation.isOutdoors.Value && Game1.currentLocation is not Desert;

	private static readonly Lazy<Texture2D> FishTexture = new(() => SpriteMaster.Self.Helper.Content.Load<Texture2D>(@"LooseSprites\AquariumFish", StardewModdingAPI.ContentSource.GameContent));

	[Harmonize(
		typeof(Game1),
		"drawWeather",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		critical: false
	)]
	public static bool DrawWeather(Game1 __instance, GameTime time, RenderTarget2D target_screen) {
		if (!Config.Enabled || !Config.Extras.Snow.Enabled) {
			if (ShouldDrawSnow) {
				if (LastDebrisWeather is null || Game1.debrisWeather.Count != 0) {
					LastDebrisWeather = Game1.debrisWeather;
					Game1.debrisWeather = new();
				}
			}
			else {
				LastDebrisWeather = null;
			}
			return true;
		}

		if (!ShouldDrawSnow) {
			return true;
		}

		if (Game1.debrisWeather.Count == 0 && LastDebrisWeather is not null && LastDebrisWeather.Count != 0) {
			Game1.debrisWeather = LastDebrisWeather;
			LastDebrisWeather = null;
		}

		if (Game1.debrisWeather is null || Game1.debrisWeather.Count == 0) {
			return true;
		}

		var batchSortMode = DrawState.CurrentSortMode;
		var batchBlendState = DrawState.CurrentBlendState;
		var batchSamplerState = DrawState.CurrentSamplerState;

		Game1.spriteBatch.End();
		Game1.spriteBatch.Begin(
			IsPuffersnow ? SpriteSortMode.Texture : SpriteSortMode.Deferred,
			IsPuffersnow ? BlendState.AlphaBlend : BlendState.Additive, SamplerState.LinearClamp
		);

		try {
			if (__instance.takingMapScreenshot) {
				Texture2D drawTexture = IsPuffersnow ? FishTexture.Value : Game1.mouseCursors;

				foreach (WeatherDebris _item in Game1.debrisWeather) {
					var item = (SnowWeatherDebris)_item;
					var position = new Vector2(
						Game1.random.Next(Game1.viewport.Width - item.sourceRect.Width * 3),
						Game1.random.Next(Game1.viewport.Height - item.sourceRect.Height * 3)
					);

					Game1.spriteBatch.Draw(
						texture: drawTexture,
						position: position,
						sourceRectangle: IsPuffersnow ? new(0, 0, 24, 24) : item.sourceRect,
						color: Color.White,
						rotation: item.Rotation,
						origin: Vector2.Zero,
						scale: item.Scale,
						effects: SpriteEffects.None,
						layerDepth: 1E-06f
					);
				}
			}
			else if (Game1.viewport.X > -Game1.viewport.Width) {
				if (IsPuffersnow) {
					Texture2D drawTexture = FishTexture.Value;

					foreach (var mappedListPair in MappedWeatherDebris) {
						var source = mappedListPair.Key;

						foreach (SnowWeatherDebris item in mappedListPair.Value) {
							Game1.spriteBatch.Draw(
								texture: drawTexture,
								position: item.position,
								sourceRectangle: new(0, 0, 24, 24),
								color: Color.White,
								rotation: item.Rotation,
								origin: Vector2.Zero,
								scale: item.Scale,
								effects: SpriteEffects.None,
								layerDepth: 1E-06f
							);
						}
					}
				}
				else {
					Texture2D drawTexture = Game1.mouseCursors;

					foreach (var mappedListPair in MappedWeatherDebris) {
						var source = mappedListPair.Key;
						var list = mappedListPair.Value;

#if !USE_MULTIDRAW
						foreach (SnowWeatherDebris item in list) {
							Game1.spriteBatch.Draw(
								texture: drawTexture,
								position: item.position,
								sourceRectangle: source, 
								color: Color.White,
								rotation: item.Rotation,
								origin: Vector2.Zero,
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
							color: Color.White,
							origin: Vector2.Zero,
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

		return false;
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
		if (!Config.Enabled || !Config.Extras.Snow.Enabled) {
			return true;
		}

		if (!ShouldDrawSnow) {
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
		foreach (WeatherDebris _item in Game1.debrisWeather) {
			var item = (SnowWeatherDebris)_item;
			item.Update();
		}

		return false;
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
		if (!Config.Enabled || !Config.Extras.Snow.Enabled) {
			return true;
		}

		if (!ShouldDrawSnow) {
			return true;
		}

		if (!overrideConstraints && (!Game1.IsSnowingHere() || !Game1.currentLocation.IsOutdoors || direction != 0 && direction != 2 && (Game1.player.getStandingX() < Game1.viewport.Width / 2 || Game1.player.getStandingX() > Game1.currentLocation.Map.DisplayWidth - Game1.viewport.Width / 2) || direction != 1 && direction != 3 && (Game1.player.getStandingY() < Game1.viewport.Height / 2 || Game1.player.getStandingY() > Game1.currentLocation.Map.DisplayHeight - Game1.viewport.Height / 2))) {
			return true;
		}

		Game1.updateDebrisWeatherForMovement(Game1.debrisWeather, direction, overrideConstraints, speed);
		return false;
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
		if (!Config.Enabled || !Config.Extras.Snow.Enabled) {
			return true;
		}

		if (!Game1.IsSnowingHere()) {
			return true;
		}

		PopulateWeather((Game1.viewport.Width, Game1.viewport.Height));

		return false;
	}

	private static void PopulateWeather(Vector2I screenSize) {
		IsPuffersnow = Game1.random.NextDouble() < Config.Extras.Snow.PuffersnowChance;

		Game1.isDebrisWeather = true;
		int debrisToMake = Game1.random.Next(Config.Extras.Snow.MinimumDensity, Config.Extras.Snow.MaximumDensity);
		int currentScreenArea = screenSize.Width * screenSize.Height;
		double ratio = (double)currentScreenArea / ReferenceScreenArea;
		debrisToMake = (int)Math.Round(debrisToMake * ratio);
		MappedWeatherDebris.Clear();
		Game1.debrisWeather.Clear();
		Game1.debrisWeather.Capacity = debrisToMake;
		for (int i = 0; i < debrisToMake; i++) {
			var debris = new SnowWeatherDebris(
				position: new Vector2(Game1.random.Next(0, screenSize.Width), Game1.random.Next(0, screenSize.Height)),
				which: 3,
				rotationVelocity: 0.0f,
				dx: Game1.random.Next(-10, 0) / 50f,
				dy: Game1.random.Next(10) / 50f,
				rotation: (float)(Game1.random.NextDouble() * 2.0 * Math.PI),
				rotationRate: (float)(((Game1.random.NextDouble() * 2.0) - 1.0) * Config.Extras.Snow.MaximumRotationSpeed),
				scale: (float)Math.Sqrt((Game1.random.NextDouble() * 0.75) + 0.25) * Config.Extras.Snow.MaximumScale
			);

			Game1.debrisWeather.Add(debris);

			if (!MappedWeatherDebris.TryGetValue(debris.sourceRect, out var mappedList)) {
				MappedWeatherDebris.Add(debris.sourceRect, mappedList = new List<SnowWeatherDebris>());
			}
			mappedList.Add(debris);
		}
		Game1.debrisWeather.Sort((d1, d2) => {
			var sourceDiff = d1.sourceRect.GetHashCode() - d2.sourceRect.GetHashCode();
			if (sourceDiff != 0) {
				return sourceDiff;
			}
			return ((SnowWeatherDebris)d1).Scale.CompareTo(((SnowWeatherDebris)d2).Scale);
		});

		foreach (var mappedList in MappedWeatherDebris) {
			mappedList.Value.Sort((d1, d2) => d1.Scale.CompareTo(d2.Scale));
		}
		LastDebrisWeather = Game1.debrisWeather;
	}

	internal static void OnWindowResized(Vector2I size) {
		if (Config.Enabled && Config.Extras.Snow.Enabled && Game1.IsSnowingHere()) {
			PopulateDebrisWeatherArray();
		}
	}
}
