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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewModdingAPI;

namespace Leclair.Stardew.CloudySkies;

public class ModApi : ICloudySkiesApi {

	private readonly ModEntry Mod;
	private readonly IManifest Other;

	public ModApi(ModEntry mod, IManifest other) {
		Mod = mod;
		Other = other;
	}

	public void RegenerateLayers(string? weatherId = null) {
		Mod.UncacheLayers(weatherId);
	}

	public IEnumerable<IWeatherData> GetAllCustomWeather() {
		Mod.LoadWeatherData();
		foreach (var data in Mod.Data)
			yield return data.Value;
	}

	public bool TryGetWeather(string id, [NotNullWhen(true)] out IWeatherData? data) {
		if (Mod.TryGetWeather(id, out var weather)) {
			data = weather;
			return data is not null;
		}

		data = null;
		return false;
	}

}
