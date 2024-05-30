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

using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Events;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;

using StardewValley;
using StardewValley.Delegates;
using StardewValley.Network;
using StardewValley.TokenizableStrings;

namespace Leclair.Stardew.CloudySkies;

public partial class ModEntry {

	#region Game State Queries

	protected override void RegisterGameStateQueries() {
		List<string> registered = EventHelper.RegisterGameStateQueries(this, [
				$"{ModManifest.UniqueID}_",
				$"CS_"
			], Monitor.Log);

		registered.AddRange(EventHelper.RegisterGameStateQueries(GetType(), [
				$"{ModManifest.UniqueID}_",
				$"CS_"
			], Monitor.Log));

		if (registered.Count > 0)
			Log($"Registered Game State Query conditions: {string.Join(", ", registered)}", LogLevel.Trace);
	}

	[GSQCondition]
	public static bool WEATHER(string[] query, GameStateQueryContext context) {
		GameLocation location = context.Location;
		if (!GameStateQuery.Helpers.TryGetLocationArg(query, 1, ref location, out string? error) ||
			!ArgUtility.TryGetInt(query, 2, out int offset, out error) ||
			!ArgUtility.TryGet(query, 3, out string? weatherId, out error, allowBlank: false)
		)
			return GameStateQuery.Helpers.ErrorResult(query, error);

		if (offset > 0)
			return GameStateQuery.Helpers.ErrorResult(query, "Cannot get historic data about the future");
		else if (offset < -7)
			return GameStateQuery.Helpers.ErrorResult(query, "Historic data only extends up to 7 days");

		if (Instance.TryGetWeatherHistory(location, Game1.Date.TotalDays + offset, out string? historicId)) {
			if (historicId == weatherId)
				return true;

			int i = 4;
			while (i < query.Length) {
				if (query[i] == historicId)
					return true;
				i++;
			}
		}

		return false;
	}

	private bool TryGetWeatherForGSQ(GameLocation? location, int offset, [NotNullWhen(false)] out string? error, out LocationWeather? weather) {
		if (offset > 0) {
			error = "Cannot get historic data about the future.";
			weather = null;
			return false;
		} else if (offset < -7) {
			error = "Historic data only extends up to 7 days.";
			weather = null;
			return false;
		}

		if (location is null) {
			error = null;
			weather = null;
			return true;
		}

		weather = location.GetWeather();

		if (offset != 0) {
			if (TryGetWeatherHistory(location, Game1.Date.TotalDays + offset, out string? weatherId)) {
				if (weather.Weather != weatherId) {
					// We need to make a new weather object with this data.
					// Unfortunately this is slightly more involved than we'd like.

					if (!Game1.locationContextData.TryGetValue(location.GetLocationContextId(), out var ctxData))
						ctxData = new() {
							WeatherConditions = new()
						};

					weather = new LocationWeather {
						WeatherForTomorrow = weatherId
					};

					weather.UpdateDailyWeather(location.GetLocationContextId(), ctxData, Game1.random);
				}

			} else
				weather = null;
		}

		error = null;
		return true;
	}

	[GSQCondition]
	public static bool WEATHER_IS_RAINING(string[] query, GameStateQueryContext context) {
		GameLocation location = context.Location;
		if (!GameStateQuery.Helpers.TryGetLocationArg(query, 1, ref location, out string? error) ||
			!ArgUtility.TryGetOptionalInt(query, 2, out int offset, out error) ||
			!Instance.TryGetWeatherForGSQ(location, offset, out error, out var weather)
		)
			return GameStateQuery.Helpers.ErrorResult(query, error);

		return weather?.IsRaining ?? false;
	}

	[GSQCondition]
	public static bool WEATHER_IS_SNOWING(string[] query, GameStateQueryContext context) {
		GameLocation location = context.Location;
		if (!GameStateQuery.Helpers.TryGetLocationArg(query, 1, ref location, out string? error) ||
			!ArgUtility.TryGetOptionalInt(query, 2, out int offset, out error) ||
			!Instance.TryGetWeatherForGSQ(location, offset, out error, out var weather)
		)
			return GameStateQuery.Helpers.ErrorResult(query, error);

		return weather?.IsSnowing ?? false;
	}

	[GSQCondition]
	public static bool WEATHER_IS_LIGHTNING(string[] query, GameStateQueryContext context) {
		GameLocation location = context.Location;
		if (!GameStateQuery.Helpers.TryGetLocationArg(query, 1, ref location, out string? error) ||
			!ArgUtility.TryGetOptionalInt(query, 2, out int offset, out error) ||
			!Instance.TryGetWeatherForGSQ(location, offset, out error, out var weather)
		)
			return GameStateQuery.Helpers.ErrorResult(query, error);

		return weather?.IsLightning ?? false;
	}

	[GSQCondition]
	public static bool WEATHER_IS_DEBRIS(string[] query, GameStateQueryContext context) {
		GameLocation location = context.Location;
		if (!GameStateQuery.Helpers.TryGetLocationArg(query, 1, ref location, out string? error) ||
			!ArgUtility.TryGetOptionalInt(query, 2, out int offset, out error) ||
			!Instance.TryGetWeatherForGSQ(location, offset, out error, out var weather)
		)
			return GameStateQuery.Helpers.ErrorResult(query, error);

		return weather?.IsDebrisWeather ?? false;
	}

	[GSQCondition]
	public static bool WEATHER_IS_GREEN_RAIN(string[] query, GameStateQueryContext context) {
		GameLocation location = context.Location;
		if (!GameStateQuery.Helpers.TryGetLocationArg(query, 1, ref location, out string? error) ||
			!ArgUtility.TryGetOptionalInt(query, 2, out int offset, out error) ||
			!Instance.TryGetWeatherForGSQ(location, offset, out error, out var weather)
		)
			return GameStateQuery.Helpers.ErrorResult(query, error);

		return weather?.IsGreenRain ?? false;
	}

	[GSQCondition]
	public static bool LOCATION_IGNORE_DEBRIS_WEATHER(string[] query, GameStateQueryContext context) {
		GameLocation location = context.Location;
		if (!GameStateQuery.Helpers.TryGetLocationArg(query, 1, ref location, out string? error))
			return GameStateQuery.Helpers.ErrorResult(query, error);

		return location?.ignoreDebrisWeather.Value ?? false;
	}

	#endregion

	#region Trigger Actions

	protected override void RegisterTriggerActions() {
		string key = $"{ModManifest.UniqueID}_";
		List<string> registered = EventHelper.RegisterTriggerActions(typeof(Triggers), key, Monitor.Log);
		if (registered.Count > 0)
			Log($"Registered trigger actions: {string.Join(", ", registered)}", LogLevel.Trace);
	}

	#endregion

	#region Custom Weather Totems

	public bool UseWeatherTotem(Farmer who, string weatherId, SObject? item = null, bool bypassChecks = false) {
		var location = who.currentLocation;

		string contextId = location.GetLocationContextId();
		var context = location.GetLocationContext();

		if (!bypassChecks && context.RainTotemAffectsContext != null) {
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

		if (!allowed && !bypassChecks) {
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
				message = TokenizeText(weatherData.TotemMessage);
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

	#region Tokenized Text Stuff

	[return: NotNullIfNotNull(nameof(input))]
	public string? TokenizeText(string? input, Farmer? who = null, Random? rnd = null) {
		if (string.IsNullOrWhiteSpace(input))
			return input;

		bool ParseToken(string[] query, out string? replacement, Random? random, Farmer? player) {
			if (!ArgUtility.TryGet(query, 0, out string? cmd, out string? error))
				return TokenParser.LogTokenError(query, error, out replacement);

			if (cmd is null || !cmd.Equals("LocalizedText")) {
				replacement = null;
				return false;
			}

			if (!ArgUtility.TryGet(query, 1, out string? key, out error))
				return TokenParser.LogTokenError(query, error, out replacement);

			var tl = Helper.Translation.Get(key);
			if (!tl.HasValue()) {
				replacement = null;
				return false;
			}

			Dictionary<int, string> replacements;
			if (query.Length > 2) {
				replacements = new();
				for (int i = 2; i < query.Length; i++) {
					replacements[i - 2] = query[i];
				}

			} else
				replacements = [];

			replacement = TokenParser.ParseText(tl.Tokens(replacements).ToString(), rnd, ParseToken, who);
			return true;
		}

		return TokenParser.ParseText(input, rnd, ParseToken, who);
	}


	#endregion

}
