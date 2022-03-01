/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Types;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;

namespace SpriteMaster.Harmonize.Patches.Game;

static class Snow {
	private struct ParticleData {
		internal float Rotation;
		internal readonly float RotationRate;
		internal readonly float Scale;

		internal ParticleData(float rotation, float rotationRate, float scale) {
			Rotation = rotation;
			RotationRate = rotationRate;
			Scale = scale;
		}

		internal ParticleData Update() {
			return this with { Rotation = (Rotation + RotationRate) % (2.0f * MathF.PI) };
		}
	}

	private static bool IsPuffersnow = false;
	private static readonly List<ParticleData> ParticlesData = new();

	private static bool ShouldDrawSnow => Game1.IsSnowingHere() && Game1.currentLocation.isOutdoors && Game1.currentLocation is not Desert;

	private static readonly Lazy<Texture2D> FishTexture = new(() => SpriteMaster.Self.Helper.Content.Load<Texture2D>("LooseSprites\\AquariumFish", StardewModdingAPI.ContentSource.GameContent));

	[Harmonize(
		typeof(Game1),
		"drawWeather",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		critical: false
	)]
	public static bool DrawWeather(Game1 __instance, GameTime time, RenderTarget2D target_screen) {
		if (!Config.Enabled || !Config.Extras.Snow.Enabled) {
			return true;
		}

		if (!ShouldDrawSnow) {
			return true;
		}

		if (Game1.debrisWeather is null) {
			return true;
		}

		var batchSortMode = DrawState.CurrentSortMode;
		var batchBlendState = DrawState.CurrentBlendState;
		var batchSamplerState = DrawState.CurrentSamplerState;

		Game1.spriteBatch.End();
		Game1.spriteBatch.Begin(SpriteSortMode.Deferred, IsPuffersnow ? BlendState.AlphaBlend : BlendState.Additive, SamplerState.LinearClamp);

		try {
			if (__instance.takingMapScreenshot) {
				int i = 0;
				foreach (WeatherDebris w in Game1.debrisWeather) {
					Vector2 position = w.position;
					w.position = new Vector2(Game1.random.Next(Game1.viewport.Width - w.sourceRect.Width * 3), Game1.random.Next(Game1.viewport.Height - w.sourceRect.Height * 3));
					var data = ParticlesData[i];
					(Texture2D texture, Rectangle sourceRect) = IsPuffersnow ? (FishTexture.Value, new(0, 0, 24, 24)) : (Game1.mouseCursors, w.sourceRect);
					Game1.spriteBatch.Draw(texture, w.position, sourceRect, Color.White, data.Rotation, Vector2.Zero, data.Scale * Config.Extras.Snow.MaximumScale, SpriteEffects.None, 1E-06f);
					w.position = position;
					++i;
				}
			}
			else if (Game1.viewport.X > -Game1.viewport.Width) {
				int i = 0;
				foreach (WeatherDebris item in Game1.debrisWeather) {
					var data = ParticlesData[i];
					(Texture2D texture, Rectangle sourceRect) = IsPuffersnow ? (FishTexture.Value, new(0, 0, 24, 24)) : (Game1.mouseCursors, item.sourceRect);
					Game1.spriteBatch.Draw(texture, item.position, sourceRect, Color.White, data.Rotation, Vector2.Zero, data.Scale * Config.Extras.Snow.MaximumScale, SpriteEffects.None, 1E-06f);
					++i;
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
				if (Game1.soundBank != null) {
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
		int i = 0;
		foreach (WeatherDebris item in Game1.debrisWeather) {
			item.update();
			ParticlesData[i] = ParticlesData[i].Update();
			++i;
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
		ParticlesData.Clear();
		ParticlesData.Capacity = debrisToMake;
		Game1.debrisWeather.Clear();
		Game1.debrisWeather.Capacity = debrisToMake;
		for (int i = 0; i < debrisToMake; i++) {
			Game1.debrisWeather.Add(
				new(
					position: new Vector2(Game1.random.Next(0, screenSize.Width), Game1.random.Next(0, screenSize.Height)),
					which: 3,
					rotationVelocity: 0.0f,
					dx: Game1.random.Next(-10, 0) / 50f,
					dy: Game1.random.Next(10) / 50f
				)
			);
			ParticlesData.Add(new(
				rotation: (float)(Game1.random.NextDouble() * 2.0 * Math.PI),
				rotationRate: (float)(((Game1.random.NextDouble() * 2.0) - 1.0) * Config.Extras.Snow.MaximumRotationSpeed),
				scale: (float)Math.Sqrt((Game1.random.NextDouble() * 0.75) + 0.25)
			));
		}
	}

	internal static void OnWindowResized(Vector2I size) {
		if (Config.Enabled && Config.Extras.Snow.Enabled && Game1.IsSnowingHere()) {
			PopulateDebrisWeatherArray();
		}
	}
}
