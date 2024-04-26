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

using StardewValley.Network;

namespace Leclair.Stardew.CloudySkies.Patches;

public static class LocationWeather_Patches {

	private static ModEntry? Mod;

	public static void Patch(ModEntry mod) {
		Mod = mod;

		try {

			mod.Harmony.Patch(
				original: AccessTools.Method(typeof(LocationWeather), nameof(LocationWeather.UpdateDailyWeather)),
				postfix: new HarmonyMethod(typeof(LocationWeather_Patches), nameof(UpdateDailyWeather__Postfix))
			);

		} catch(Exception ex) {
			mod.Log($"Error patching LocationWeather. Weather may not work correctly.", StardewModdingAPI.LogLevel.Error, ex);
		}

	}

	#region Helpers

	#endregion

	private static void UpdateDailyWeather__Postfix(LocationWeather __instance) {

		if (Mod is null || __instance.Weather is null || !Mod.TryGetWeather(__instance.Weather, out var weatherData))
			return;

		__instance.IsRaining = weatherData.IsRaining;
		__instance.IsSnowing = weatherData.IsSnowing;
		__instance.IsLightning = weatherData.IsLightning;
		__instance.IsDebrisWeather = weatherData.IsDebrisWeather;
		__instance.IsGreenRain = weatherData.IsGreenRain;

	}
	

}
