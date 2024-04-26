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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Delegates;
using StardewValley.TokenizableStrings;

namespace Leclair.Stardew.CloudySkies;

public partial class ModEntry {

	#region Game State Queries

	private void RegisterQueries() {
		Dictionary<string, GameStateQueryDelegate> entries = new() {
			{ "LOCATION_IGNORE_DEBRIS_WEATHER", LocationIgnoreDebrisWeather },
			{ "WEATHER_IS_RAINING", WeatherRaining },
			{ "WEATHER_IS_SNOWING", WeatherSnowing },
			{ "WEATHER_IS_LIGHTNING", WeatherLightning },
			{ "WEATHER_IS_DEBRIS", WeatherDebris },
			{ "WEATHER_IS_GREEN_RAIN", WeatherGreenRain }
		};

		foreach (var pair in entries) {
			string prefixed = $"leclair.cloudyskies_{pair.Key}";
			GameStateQuery.Register(prefixed, pair.Value);
			if (!GameStateQuery.Exists($"CS_{pair.Key}"))
				GameStateQuery.RegisterAlias($"CS_{pair.Key}", prefixed);
		}
	}

	public static bool WeatherRaining(string[] query, GameStateQueryContext context) {
		GameLocation location = context.Location;
		if (!GameStateQuery.Helpers.TryGetLocationArg(query, 1, ref location, out string? error))
			return GameStateQuery.Helpers.ErrorResult(query, error);

		return location?.GetWeather()?.IsRaining ?? false;
	}

	public static bool WeatherSnowing(string[] query, GameStateQueryContext context) {
		GameLocation location = context.Location;
		if (!GameStateQuery.Helpers.TryGetLocationArg(query, 1, ref location, out string? error))
			return GameStateQuery.Helpers.ErrorResult(query, error);

		return location?.GetWeather()?.IsSnowing ?? false;
	}

	public static bool WeatherLightning(string[] query, GameStateQueryContext context) {
		GameLocation location = context.Location;
		if (!GameStateQuery.Helpers.TryGetLocationArg(query, 1, ref location, out string? error))
			return GameStateQuery.Helpers.ErrorResult(query, error);

		return location?.GetWeather()?.IsLightning ?? false;
	}

	public static bool WeatherDebris(string[] query, GameStateQueryContext context) {
		GameLocation location = context.Location;
		if (!GameStateQuery.Helpers.TryGetLocationArg(query, 1, ref location, out string? error))
			return GameStateQuery.Helpers.ErrorResult(query, error);

		return location?.GetWeather()?.IsDebrisWeather ?? false;
	}

	public static bool WeatherGreenRain(string[] query, GameStateQueryContext context) {
		GameLocation location = context.Location;
		if (!GameStateQuery.Helpers.TryGetLocationArg(query, 1, ref location, out string? error))
			return GameStateQuery.Helpers.ErrorResult(query, error);

		return location?.GetWeather()?.IsGreenRain ?? false;
	}

	public static bool LocationIgnoreDebrisWeather(string[] query, GameStateQueryContext context) {
		GameLocation location = context.Location;
		if (!GameStateQuery.Helpers.TryGetLocationArg(query, 1, ref location, out string? error))
			return GameStateQuery.Helpers.ErrorResult(query, error);

		return location?.ignoreDebrisWeather.Value ?? false;
	}

	#endregion

	#region Custom Weather Totems

	public bool UseWeatherTotem(Farmer who, string weatherId, SObject? item = null) {
		var location = who.currentLocation;

		string contextId = location.GetLocationContextId();
		var context = location.GetLocationContext();

		if (context.RainTotemAffectsContext != null) {
			contextId = context.RainTotemAffectsContext;
			context = LocationContexts.Require(contextId);
		}

		bool allowed = context.AllowRainTotem;

		if (context.CustomFields != null && context.CustomFields.TryGetValue($"{ALLOW_TOTEM_DATA}{weatherId}", out string? val)) {
			if (bool.TryParse(val, out bool result))
				allowed = result;
			else
				allowed = !string.IsNullOrWhiteSpace(val);
		}

		if (!allowed) {
			Game1.showRedMessageUsingLoadString("Strings\\UI:Item_CantBeUsedHere");
			return false;
		}

		bool applied = false;

		if (!Utility.isFestivalDay(Game1.dayOfMonth + 1, Game1.season, contextId)) {
			Game1.netWorldState.Value.GetWeatherForLocation(contextId).WeatherForTomorrow = weatherId;
			applied = true;
		}

		if (applied && contextId == "Default") {
			Game1.netWorldState.Value.WeatherForTomorrow = weatherId;
			Game1.weatherForTomorrow = weatherId;
		}

		TryGetWeather(weatherId, out var weatherData);

		if (applied) {
			string message;
			if (weatherData != null && !string.IsNullOrEmpty(weatherData.TotemMessage))
				message = TokenParser.ParseText(weatherData.TotemMessage);
			else
				message = Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12822");

			Game1.pauseThenMessage(2000, message);
		}

		Game1.screenGlow = false;
		string? sound = weatherData is null ? "thunder" : weatherData.TotemSound;
		if (!string.IsNullOrEmpty(sound))
			location.playSound(sound);

		who.CanMove = false;
		Color color = weatherData?.TotemScreenTint ?? Color.SlateBlue;
		if (color != Color.Transparent)
			Game1.screenGlowOnce(color, hold: false);

		Game1.player.faceDirection(2);
		Game1.player.FarmerSprite.animateOnce([
			new FarmerSprite.AnimationFrame(57, 2000, secondaryArm: false, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true)
		]);

		string? texture = weatherData is null ? "LooseSprites\\Cursors" : weatherData.TotemParticleTexture;
		if (!string.IsNullOrEmpty(texture)) {
			Rectangle bounds;
			if (weatherData is null)
				bounds = new Rectangle(648, 1045, 52, 33);
			else if (weatherData.TotemParticleSource.HasValue)
				bounds = weatherData.TotemParticleSource.Value;
			else {
				Texture2D tex = Game1.content.Load<Texture2D>(texture);
				bounds = tex.Bounds;
			}

			for (int i = 0; i < 6; i++) {
				Game1.Multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(
					texture, bounds,
					9999f, 1, 999,
					who.Position + new Vector2(0f, -128f),
					flicker: false,
					flipped: false,
					1f, 0.01f,
					Color.White * 0.8f,
					2f, 0.01f,
					0f, 0f
				) {
					motion = new Vector2(Game1.random.Next(-10, 11) / 10f, -2f),
					delayBeforeAnimationStart = i * 200
				});

				Game1.Multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(
					texture, bounds,
					9999f, 1, 999,
					who.Position + new Vector2(0f, -128f),
					flicker: false,
					flipped: false,
					1f, 0.01f,
					Color.White * 0.8f,
					1f, 0.01f,
					0f, 0f
				) {
					motion = new Vector2(Game1.random.Next(-30, -10) / 10f, -1f),
					delayBeforeAnimationStart = 100 + i * 200
				});

				Game1.Multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(
					texture, bounds,
					9999f, 1, 999,
					who.Position + new Vector2(0f, -128f),
					flicker: false,
					flipped: false,
					1f, 0.01f,
					Color.White * 0.8f,
					1f, 0.01f,
					0f, 0f
				) {
					motion = new Vector2(Game1.random.Next(10, 30) / 10f, -1f),
					delayBeforeAnimationStart = 200 + i * 200
				});
			}
		}

		if (item is not null) {
			TemporaryAnimatedSprite sprite = new TemporaryAnimatedSprite(0, 9999f, 1, 999, Game1.player.Position + new Vector2(0f, -96f), flicker: false, flipped: false, verticalFlipped: false, 0f) {
				motion = new Vector2(0f, -7f),
				acceleration = new Vector2(0f, 0.1f),
				scaleChange = 0.015f,
				alpha = 1f,
				alphaFade = 0.0075f,
				shakeIntensity = 1f,
				initialPosition = Game1.player.Position + new Vector2(0f, -96f),
				xPeriodic = true,
				xPeriodicLoopTime = 1000f,
				xPeriodicRange = 4f,
				layerDepth = 1f
			};
			sprite.CopyAppearanceFromItemId(item.QualifiedItemId);
			Game1.Multiplayer.broadcastSprites(location, sprite);
		}

		sound = weatherData is null ? "rainsound" : weatherData.TotemAfterSound;
		if (!string.IsNullOrEmpty(sound))
			DelayedAction.playSoundAfterDelay(sound, 2000);

		return true;
	}

	#endregion

}
