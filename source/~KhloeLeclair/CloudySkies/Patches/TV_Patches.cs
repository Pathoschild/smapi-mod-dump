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

using HarmonyLib;

using Leclair.Stardew.CloudySkies.Menus;
using Leclair.Stardew.Common;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;

using StardewValley;
using StardewValley.Objects;

namespace Leclair.Stardew.CloudySkies.Patches;

public static class TV_Patches {

	private static ModEntry? Mod;
	private static Func<TV, int>? GetCurrentChannel;
	private static Action<TV, TemporaryAnimatedSprite>? SetAnimatedSprite;

	private static bool DidPatch = false;

	public static void Patch(ModEntry mod) {
		Mod = mod;

		try {
			GetCurrentChannel = AccessTools.Field(typeof(TV), "currentChannel")
				.CreateGetter<TV, int>();

			SetAnimatedSprite = AccessTools.Field(typeof(TV), "screenOverlay")
				.CreateSetter<TV, TemporaryAnimatedSprite>();

			mod.Harmony.Patch(
				original: AccessTools.Method(typeof(TV), "getWeatherForecast", [typeof(string)]),
				postfix: new HarmonyMethod(typeof(TV_Patches), nameof(GetWeatherForecast__Postfix))
			);

			mod.Harmony.Patch(
				original: AccessTools.Method(typeof(TV), nameof(TV.proceedToNextScene)),
				prefix: new HarmonyMethod(typeof(TV_Patches), nameof(TV_proceedToNextScene__Prefix))
			);

			mod.Harmony.Patch(
				original: AccessTools.Method(typeof(TV), "getIslandWeatherForecast"),
				postfix: new HarmonyMethod(typeof(TV_Patches), nameof(GetIslandWeatherForecast__Postfix))
			);

			mod.Harmony.Patch(
				original: AccessTools.Method(typeof(TV), "setWeatherOverlay", [typeof(string)]),
				postfix: new HarmonyMethod(typeof(TV_Patches), nameof(SetWeatherOverlay__Postfix))
			);

			mod.Harmony.Patch(
				original: AccessTools.Method(typeof(TV), nameof(TV.draw), [typeof(SpriteBatch), typeof(int), typeof(int), typeof(float)]),
				postfix: new HarmonyMethod(typeof(TV_Patches), nameof(Draw__Postfix))
			);

			DidPatch = true;

		} catch (Exception ex) {
			mod.Log($"Error patching TV. Is the game version pre-1.6.6?", StardewModdingAPI.LogLevel.Error, ex);
		}

	}

	private static void Draw__Postfix(TV __instance, SpriteBatch spriteBatch) {
		try {
			if (Game1.activeClickableMenu is TVWeatherMenu menu && menu.Television == __instance)
				menu.DrawWorld(spriteBatch);

		} catch (Exception ex) {
			Mod?.Log($"Error in TV Draw postfix: {ex}", LogLevel.Error, once: true);
		}
	}


	private static bool TV_proceedToNextScene__Prefix(TV __instance) {

		try {
			if (!DidPatch || Mod is null || !Mod.Config.ReplaceTVMenu || GetCurrentChannel?.Invoke(__instance) != 2)
				return true;

			CommonHelper.YeetMenu(Game1.activeClickableMenu);

			Game1.activeClickableMenu = new TVWeatherMenu(Mod, __instance);
			return false;

		} catch (Exception ex) {
			Mod?.Log($"Error handling TV proceedToNextScene: {ex}", LogLevel.Error, once: true);
		}

		return true;

	}


	private static void SetWeatherOverlay__Postfix(TV __instance, string weatherId) {

		try {
			if (Mod is not null && Mod.TryGetWeather(weatherId, out var weatherData)) {

				TemporaryAnimatedSprite sprite;

				string textureName;
				Point corner;
				int frames;
				float speed;

				if (string.IsNullOrEmpty(weatherData.TVTexture)) {
					textureName = "LooseSprites\\Cursors_1_6";
					corner = new(178, 363);
					frames = 6;
					speed = 80f;

				} else {
					textureName = weatherData.TVTexture;
					corner = weatherData.TVSource;
					frames = weatherData.TVFrames;
					speed = weatherData.TVSpeed;
				}

				if (frames < 1)
					frames = 1;

				sprite = new TemporaryAnimatedSprite(
					textureName,
					new Rectangle(corner.X, corner.Y, 13, 13),
					speed,
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

			string weather = Game1.netWorldState.Value.GetWeatherForLocation("Island").WeatherForTomorrow;
			weather = Game1.getWeatherModificationsForDate(tomorrow, weather);

			if (Mod is not null && Mod.TryGetWeather(weather, out var weatherData)) {
				if (weatherData.ForecastByContext is null || !weatherData.ForecastByContext.TryGetValue("Island", out string? val))
					val = weatherData.Forecast;

				string? result = val;

				if (string.IsNullOrEmpty(result))
					__result = Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13164");
				else
					__result = Mod.TokenizeText(result);

				__result = Game1.content.LoadString("Strings\\StringsFromCSFiles:TV_IslandWeatherIntro") + __result;

			}

		} catch (Exception ex) {
			Mod?.Log($"Error getting Island weather forecast: {ex}", StardewModdingAPI.LogLevel.Error);
		}

	}

	private static void GetWeatherForecast__Postfix(TV __instance, string weatherId, ref string __result) {

		try {
			if (Mod is not null && Mod.TryGetWeather(weatherId, out var weatherData)) {
				string? result = weatherData.Forecast;
				if (string.IsNullOrEmpty(result))
					__result = Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13164");
				else
					__result = Mod.TokenizeText(result);
			}

		} catch (Exception ex) {
			Mod?.Log($"Error getting weather forecast: {ex}", StardewModdingAPI.LogLevel.Error);
		}

	}

}
