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

using ContentPatcher;

using Leclair.Stardew.Common.Integrations;

using StardewModdingAPI;

using StardewValley;
using StardewValley.Network;

namespace Leclair.Stardew.CloudySkies.Integrations.ContentPatcher;

public class CPIntegration : BaseAPIIntegration<IContentPatcherAPI, ModEntry> {
	public CPIntegration(ModEntry self)
		: base(self, "Pathoschild.ContentPatcher", "2.0.0")
	{

		if (!IsLoaded)
			return;

		API.RegisterToken(self.ModManifest, "Weather", () => {

			LocationWeather? weather;

			if (Context.IsWorldReady)
				 weather = Game1.currentLocation?.GetWeather();
			else
				weather = SaveGame.loaded?.player?.currentLocation?.GetWeather();

			if (weather is null)
				return null;

			List<string> flags = new();

			if (weather.IsRaining)
				flags.Add("Raining");
			if (weather.IsSnowing)
				flags.Add("Snowing");
			if (weather.IsLightning)
				flags.Add("Lighting");
			if (weather.IsDebrisWeather)
				flags.Add("Debris");
			if (weather.IsGreenRain)
				flags.Add("GreenRain");

			if (flags.Count == 0)
				flags.Add("Sunny");

			if (Self.TryGetWeather(weather.Weather, out var weatherData)) {
				if (weatherData.MusicOverride != null)
					flags.Add("Music");

				if (weatherData.UseNightTiles)
					flags.Add("NightTiles");

			} else {
				// Vanilla Behaviors
				if (weather.IsRaining) {
					flags.Add("Music");
					flags.Add("NightTiles");
				}
			}

			return flags.OrderBy(p => p, StringComparer.OrdinalIgnoreCase).ToArray();
		});

	}
}
