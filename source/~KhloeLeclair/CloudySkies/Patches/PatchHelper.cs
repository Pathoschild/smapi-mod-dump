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
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

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
		var wd = GetWeatherData(location);
		if (wd is null)
			return Game1.IsRainingHere(location);

		return wd.AmbientColor.HasValue;
	}

	internal static Color GetAmbientColor(GameLocation? location) {
		var wd = GetWeatherData(location);
		if (wd is not null)
			return wd.AmbientColor ?? Color.White;

		if (Game1.IsRainingHere(location))
			return new Color(255, 200, 80);

		return Color.White;
	}

	internal static bool HasLightingTint(GameLocation? location) {
		var wd = GetWeatherData(location);
		if (wd is null)
			return Game1.IsRainingHere(location);

		return wd.LightingTint.HasValue;
	}

	internal static Color GetLightingTint(GameLocation? location) {
		var wd = GetWeatherData(location);
		if (wd is not null) {
			if (wd.LightingTint.HasValue)
				return wd.LightingTint.Value * wd.LightingTintOpacity;
			return Color.White;
		}

		return Color.OrangeRed * 0.45f;
	}

	internal static bool HasPostLightingTint(GameLocation? location) {
		var wd = GetWeatherData(location);
		if (wd is null)
			return Game1.IsRainingHere(location);

		return wd.PostLightingTint.HasValue;
	}

	internal static Color GetPostLightingTint(GameLocation? location) {
		var wd = GetWeatherData(location);
		if (wd is not null) {
			if (wd.PostLightingTint.HasValue)
				return wd.PostLightingTint.Value * wd.PostLightingTintOpacity;
			return Color.Transparent;
		}

		if (Game1.IsGreenRainingHere(location))
			return new Color(0, 120, 150) * 0.22f;

		return Color.Blue * 0.2f;
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

}
