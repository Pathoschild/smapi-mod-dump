/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System.Reflection.Emit;

using HarmonyLib;

using Leclair.Stardew.CloudySkies.Models;

using Microsoft.Xna.Framework;

using StardewValley;

namespace Leclair.Stardew.CloudySkies.Patches;

internal static class PatchHelper {

	private static ModEntry? Mod;

	public static void Init(ModEntry mod) {
		Mod = mod;
	}

	#region Data Access

	internal static WeatherData? GetWeatherData(GameLocation? location = null) {
		string? weather = (location ?? Game1.currentLocation)?.GetWeather()?.Weather;
		if (Mod is not null && weather is not null && Mod.TryGetWeather(weather, out var weatherData))
			return weatherData;

		return null;
	}

	internal static CachedTintData? GetTintData(GameLocation? location = null) {
		return Mod?.GetCachedTint(location);
	}


	internal static int? AsInt(this CodeInstruction instr) {
		if (instr.opcode == OpCodes.Ldc_I4_0)
			return 0;
		if (instr.opcode == OpCodes.Ldc_I4_1)
			return 1;
		if (instr.opcode == OpCodes.Ldc_I4_2)
			return 2;
		if (instr.opcode == OpCodes.Ldc_I4_3)
			return 3;
		if (instr.opcode == OpCodes.Ldc_I4_4)
			return 4;
		if (instr.opcode == OpCodes.Ldc_I4_5)
			return 5;
		if (instr.opcode == OpCodes.Ldc_I4_6)
			return 6;
		if (instr.opcode == OpCodes.Ldc_I4_7)
			return 7;
		if (instr.opcode == OpCodes.Ldc_I4_8)
			return 8;
		if (instr.opcode == OpCodes.Ldc_I4_M1)
			return -1;
		if (instr.opcode != OpCodes.Ldc_I4 && instr.opcode != OpCodes.Ldc_I4_S)
			return null;

		if (instr.operand is int val)
			return val;
		if (instr.operand is byte bval)
			return bval;
		if (instr.operand is sbyte sbval)
			return sbval;
		if (instr.operand is short shval)
			return shval;
		if (instr.operand is ushort ushval)
			return ushval;

		return null;
	}

	internal static double? AsDouble(this CodeInstruction instr) {
		if (instr.operand is double dval)
			return dval;
		if (instr.operand is float fval)
			return fval;

		return instr.AsInt();
	}

	#endregion

	#region Color Access

	internal static bool HasAmbientColor(GameLocation? location) {
		var tint = GetTintData(location);
		if (!tint.HasValue)
			return Game1.IsRainingHere(location);

		return tint.Value.HasAmbientColor;
	}

	internal static Color GetAmbientColor(GameLocation? location) {
		var tint = GetTintData(location);
		if (!tint.HasValue)
			return Game1.IsRainingHere(location)
				? new Color(255, 200, 80)
				: Color.White;

		if (tint.Value.EndTime == int.MaxValue)
			return tint.Value.StartAmbientColor;

		int minutes = Utility.CalculateMinutesBetweenTimes(tint.Value.StartTime, Game1.timeOfDay) / 10;
		float progress = (minutes + (Game1.gameTimeInterval / (float) Game1.realMilliSecondsPerGameTenMinutes)) / tint.Value.DurationInTenMinutes;

		if (tint.Value.StartAmbientColor == tint.Value.EndAmbientColor)
			return tint.Value.StartAmbientColor;
		else
			return new Color(
				(byte) Utility.Lerp(tint.Value.StartAmbientColor.R, tint.Value.EndAmbientColor.R, progress),
				(byte) Utility.Lerp(tint.Value.StartAmbientColor.G, tint.Value.EndAmbientColor.G, progress),
				(byte) Utility.Lerp(tint.Value.StartAmbientColor.B, tint.Value.EndAmbientColor.B, progress)
			);
	}

	internal static bool HasLightingTint(GameLocation? location) {
		var tint = GetTintData(location);
		if (!tint.HasValue)
			return Game1.IsRainingHere(location);

		return tint.Value.HasLightingTint;
	}

	internal static Color GetLightingTint(GameLocation? location) {
		var tint = GetTintData(location);
		if (!tint.HasValue)
			return Color.OrangeRed * 0.45f;

		if (!tint.Value.HasLightingTint)
			return Color.White;

		if (tint.Value.EndTime == int.MaxValue)
			return tint.Value.StartLightingTint * tint.Value.StartLightingTintOpacity;

		int minutes = Utility.CalculateMinutesBetweenTimes(tint.Value.StartTime, Game1.timeOfDay) / 10;
		float progress = (minutes + (Game1.gameTimeInterval / (float) Game1.realMilliSecondsPerGameTenMinutes)) / tint.Value.DurationInTenMinutes;

		Color color;
		if (tint.Value.StartLightingTint == tint.Value.EndLightingTint)
			color = tint.Value.StartLightingTint;
		else
			color = new Color(
				(byte) Utility.Lerp(tint.Value.StartLightingTint.R, tint.Value.EndLightingTint.R, progress),
				(byte) Utility.Lerp(tint.Value.StartLightingTint.G, tint.Value.EndLightingTint.G, progress),
				(byte) Utility.Lerp(tint.Value.StartLightingTint.B, tint.Value.EndLightingTint.B, progress)
			);

		float opacity = Utility.Lerp(tint.Value.StartLightingTintOpacity, tint.Value.EndLightingTintOpacity, progress);

		return color * opacity;
	}

	internal static bool HasPostLightingTint(GameLocation? location) {
		var tint = GetTintData(location);
		if (!tint.HasValue)
			return Game1.IsRainingHere(location);

		return tint.Value.HasPostLightingTint;
	}

	internal static Color GetPostLightingTint(GameLocation? location) {
		var tint = GetTintData(location);
		if (!tint.HasValue)
			return Game1.IsGreenRainingHere(location)
				? new Color(0, 120, 150) * 0.22f
				: Color.Blue * 0.2f;

		if (!tint.Value.HasPostLightingTint)
			return Color.Transparent;

		if (tint.Value.EndTime == int.MaxValue)
			return tint.Value.StartPostLightingTint * tint.Value.StartPostLightingTintOpacity;

		int minutes = Utility.CalculateMinutesBetweenTimes(tint.Value.StartTime, Game1.timeOfDay) / 10;
		float progress = (minutes + (Game1.gameTimeInterval / (float) Game1.realMilliSecondsPerGameTenMinutes)) / tint.Value.DurationInTenMinutes;

		Color color;
		if (tint.Value.StartPostLightingTint == tint.Value.EndPostLightingTint)
			color = tint.Value.StartPostLightingTint;
		else
			color = new Color(
				(byte) Utility.Lerp(tint.Value.StartPostLightingTint.R, tint.Value.EndPostLightingTint.R, progress),
				(byte) Utility.Lerp(tint.Value.StartPostLightingTint.G, tint.Value.EndPostLightingTint.G, progress),
				(byte) Utility.Lerp(tint.Value.StartPostLightingTint.B, tint.Value.EndPostLightingTint.B, progress)
			);

		float opacity = Utility.Lerp(tint.Value.StartPostLightingTintOpacity, tint.Value.EndPostLightingTintOpacity, progress);

		return color * opacity;
	}

	#endregion

	#region Music

	internal static bool HasMusic(GameLocation? location) {
		return GetMusic(location) != null;
	}

	internal static string? GetMusic(GameLocation? location) {
		var wd = GetWeatherData(location);
		if (wd?.MusicOverride is not null)
			return wd.MusicOverride == "" ? null : wd.MusicOverride;

		if (Game1.IsRainingHere(location))
			return "rain";

		return null;
	}

	internal static string GetMusicOrDefault(GameLocation? location) {
		return GetMusic(location) ?? "rain";
	}

	internal static float GetOutsideFrequency() {
		return GetWeatherData()?.MusicFrequencyOutside ?? 100f;
	}

	internal static float GetInsideFrequency() {
		return GetWeatherData()?.MusicFrequencyInside ?? 15f;
	}

	#endregion

	internal static bool ShouldWaterCropsAndBowls(GameLocation? location) {
		var wd = GetWeatherData(location);
		if (wd != null)
			return wd.WaterCropsAndPets ?? wd.IsRaining;

		return Game1.IsRainingHere(location);
	}

}
