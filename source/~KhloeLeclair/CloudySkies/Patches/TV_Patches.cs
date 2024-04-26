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

using HarmonyLib;

using Leclair.Stardew.Common;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TokenizableStrings;

namespace Leclair.Stardew.CloudySkies.Patches;

public static class TV_Patches {

	private static ModEntry? Mod;
	private static Action<TV, TemporaryAnimatedSprite>? SetAnimatedSprite;

	public static void Patch(ModEntry mod) {
		Mod = mod;

		try {
			SetAnimatedSprite = AccessTools.Field(typeof(TV), "screenOverlay")
				.CreateSetter<TV, TemporaryAnimatedSprite>();

			mod.Harmony.Patch(
				original: AccessTools.Method(typeof(TV), "getWeatherForecast"),
				postfix: new HarmonyMethod(typeof(TV_Patches), nameof(GetWeatherForecast__Postfix))
			);

			mod.Harmony.Patch(
				original: AccessTools.Method(typeof(TV), "getIslandWeatherForecast"),
				postfix: new HarmonyMethod(typeof(TV_Patches), nameof(GetIslandWeatherForecast__Postfix))
			);

			mod.Harmony.Patch(
				original: AccessTools.Method(typeof(TV), "setWeatherOverlay"),
				postfix: new HarmonyMethod(typeof(TV_Patches), nameof(SetWeatherOverlay__Postfix))
			);

		} catch (Exception ex) {
			mod.Log($"Error patching TV.", StardewModdingAPI.LogLevel.Error, ex);
		}

	}

	private static void SetWeatherOverlay__Postfix(TV __instance, bool island = false) {

		try {
			WorldDate tomorrow = new(Game1.Date);
			tomorrow.TotalDays++;

			string weather = Game1.IsMasterGame
				? Game1.weatherForTomorrow : Game1.netWorldState.Value.WeatherForTomorrow;

			weather = Game1.getWeatherModificationsForDate(tomorrow, weather);

			if (Mod is not null && Mod.TryGetWeather(weather, out var weatherData)) {

				TemporaryAnimatedSprite sprite;

				string textureName;
				Point corner;
				int frames;

				if (string.IsNullOrEmpty(weatherData.TVTexture)) {
					textureName = "LooseSprites\\Cursors_1_6";
					corner = new(178, 363);
					frames = 6;

				} else {
					textureName = weatherData.TVTexture;
					corner = weatherData.TVSource;
					frames = weatherData.TVFrames;
				}

				sprite = new TemporaryAnimatedSprite(
					textureName,
					new Rectangle(corner.X, corner.Y, 13, 13),
					100f,
					frames,
					999999,
					__instance.getScreenPosition() + new Vector2(3f, 3f) * __instance.getScreenSizeModifier(),
					flicker: false,
					flipped: false,
					(__instance.boundingBox.Bottom - 1) / 10000f + 0.00002f,
					0f,
					Color.White,
					__instance.getScreenSizeModifier(),
					0f,
					0f,
					0f
				);

				SetAnimatedSprite?.Invoke(__instance, sprite);
			}

		} catch (Exception ex) {
			Mod?.Log($"Error getting weather forecast: {ex}", StardewModdingAPI.LogLevel.Error);
		}

	}

	private static void GetIslandWeatherForecast__Postfix(TV __instance, ref string __result) {

		try {
			WorldDate tomorrow = new(Game1.Date);
			tomorrow.TotalDays++;

			string weather = Game1.IsMasterGame
				? Game1.weatherForTomorrow : Game1.netWorldState.Value.WeatherForTomorrow;

			weather = Game1.getWeatherModificationsForDate(tomorrow, weather);

			if (Mod is not null && Mod.TryGetWeather(weather, out var weatherData)) {
				if (weatherData.ForecastByContext is null || !weatherData.ForecastByContext.TryGetValue("Island", out string? val))
					val = weatherData.Forecast;

				__result = val;

				if (string.IsNullOrEmpty(__result))
					__result = Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13164");
				else
					__result = TokenParser.ParseText(__result);

				__result = Game1.content.LoadString("Strings\\StringsFromCSFiles:TV_IslandWeatherIntro") + __result;

			}

		} catch (Exception ex) {
			Mod?.Log($"Error getting Island weather forecast: {ex}", StardewModdingAPI.LogLevel.Error);
		}

	}

	private static void GetWeatherForecast__Postfix(TV __instance, ref string __result) {

		try {
			WorldDate tomorrow = new(Game1.Date);
			tomorrow.TotalDays++;

			string weather = Game1.IsMasterGame
				? Game1.weatherForTomorrow : Game1.netWorldState.Value.WeatherForTomorrow;

			weather = Game1.getWeatherModificationsForDate(tomorrow, weather);

			if (Mod is not null && Mod.TryGetWeather(weather, out var weatherData)) {
				__result = weatherData.Forecast;
				if (string.IsNullOrEmpty(__result))
					__result = Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13164");
				else
					__result = TokenParser.ParseText(__result);
			}

		} catch(Exception ex) {
			Mod?.Log($"Error getting weather forecast: {ex}", StardewModdingAPI.LogLevel.Error);
		}

	}

}
