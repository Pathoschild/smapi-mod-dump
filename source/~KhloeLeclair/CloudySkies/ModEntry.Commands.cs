/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Events;

using StardewModdingAPI;

using StardewValley;
using StardewValley.Network;

namespace Leclair.Stardew.CloudySkies;

public partial class ModEntry {

	[ConsoleCommand("cs_reload", "Force the current weather layers and effects to be recreated.")]
	public void ReloadCommand(string name, string[] args) {
		UncacheLayers(null, true);
		Log($"Invalidated weather cache.", LogLevel.Info);
	}

	[ConsoleCommand("cs_history", "View the recorded weather history.")]
	public void HistoryCommand(string name, string[] args) {

		LoadWeatherHistory();

		List<string[]> table = new();

		int minDay = int.MaxValue;
		int maxDay = int.MinValue;

		string[] headers = new string[1 + WeatherHistory.Count];
		headers[0] = "Date";
		int j = 1;

		foreach (var pair in WeatherHistory) {
			headers[j] = pair.Key;
			j++;
			foreach (int day in pair.Value.Keys) {
				if (minDay > day)
					minDay = day;
				if (maxDay < day)
					maxDay = day;
			}
		}

		for (int i = minDay; i <= maxDay; i++) {
			string[] row = new string[1 + WeatherHistory.Count];
			table.Add(row);

			var date = new WorldDate {
				TotalDays = i
			};
			row[0] = date.Localize();

			j = 1;
			foreach (var pair in WeatherHistory) {

				if (!pair.Value.TryGetValue(i, out string? weather))
					weather = "---";

				row[j] = weather;
				j++;
			}
		}

		StringBuilder sb = new();
		sb.AppendLine("Recorded Weather History:");

		LogTable(sb, headers, table);

		Log(sb.ToString(), LogLevel.Info);
	}


	[ConsoleCommand("cs_list", "List the available weather Ids.")]
	public void ListCommand(string name, string[] args) {
		LoadWeatherData();

		HashSet<string> seen_weather = new();

		Dictionary<string, LocationWeather> contextWeather = new();
		if (Context.IsWorldReady) {
			foreach (var pair in DataLoader.LocationContexts(Game1.content)) {
				if (pair.Value is null || pair.Value.CopyWeatherFromLocation != null || pair.Value.WeatherConditions is null)
					continue;

				foreach (var cond in pair.Value.WeatherConditions) {
					if (!string.IsNullOrEmpty(cond.Weather))
						seen_weather.Add(cond.Weather);
				}

				var weather = Game1.netWorldState.Value.GetWeatherForLocation(pair.Key);
				if (weather is not null)
					contextWeather[pair.Key] = weather;
			}
		}

		List<string[]> table = new();

		foreach (string key in seen_weather) {
			if (Data.ContainsKey(key))
				continue;

			table.Add([
				key,
				"---",
				"---",
				"---",
				string.Join(", ", contextWeather.Where(x => x.Value.Weather == key).Select(x => x.Key)),
				string.Join(", ", contextWeather.Where(x => x.Value.WeatherForTomorrow == key).Select(x => x.Key)),
			]);
		}

		foreach (var entry in Data) {
			int layerCount = entry.Value.Layers is null ? 0 : entry.Value.Layers.Count;
			int effectCount = entry.Value.Effects is null ? 0 : entry.Value.Effects.Count;

			table.Add([
				entry.Key,
				TokenizeText(entry.Value.DisplayName ?? ""),
				$"{layerCount}",
				$"{effectCount}",
				string.Join(", ", contextWeather.Where(x => x.Value.Weather == entry.Key).Select(x => x.Key)),
				string.Join(", ", contextWeather.Where(x => x.Value.WeatherForTomorrow == entry.Key).Select(x => x.Key)),
			]);
		}

		StringBuilder sb = new();
		sb.AppendLine("Available / Detected Weather Conditions:");

		LogTable(sb, [
			"Id",
			"Name",
			"Layers",
			"Effects",
			"Active Today",
			"Tomorrow"
		], table);

		Log(sb.ToString(), LogLevel.Info);
	}

	[ConsoleCommand("cs_tomorrow", "Force tomorrow's weather to have a specific type in your current location.")]
	public void TomorrowCommand(string name, string[] args) {
		if (!Context.IsWorldReady) {
			Log($"Load the game first.", LogLevel.Error);
			return;
		}

		if (!Context.IsMainPlayer) {
			Log($"Only the host can do this.", LogLevel.Error);
			return;
		}

		string input = string.Join(' ', args);
		if (string.IsNullOrWhiteSpace(input)) {
			Log($"Invalid weather Id provided.", LogLevel.Error);
			return;
		}

		// Have a little fun.
		SObject? item = ItemRegistry.Create("(O)789") as SObject; // ItemQueryResolver.TryResolveRandomItem("ALL_ITEMS", new ItemQueryContext()) as SObject;

		UseWeatherTotem(Game1.player, input, item, bypassChecks: true);
	}

	[ConsoleCommand("cs_fix_green_rain", "Remove lingering green rain effects from maps.")]
	public void FixGreenRainCommand(string name, string[] args) {
		if (!Context.IsWorldReady) {
			Log($"Load the game first.", LogLevel.Error);
			return;
		}

		if (!Game1.IsMasterGame) {
			Log($"Only the host can do this.", LogLevel.Error);
			return;
		}

		IEnumerable<GameLocation>? locations = null;

		var parser = ArgumentParser.New()
			.AddPositional<IEnumerable<GameLocation>>("Locations", val => locations = val);

		if (!parser.TryParse(args, out string? error)) {
			Log(error, LogLevel.Error);
			return;
		}

		locations ??= CommonHelper.EnumerateLocations();

		int count = 0;
		if (locations is not null)
			foreach (var location in locations) {
				location.performDayAfterGreenRainUpdate();
				count++;
			}

		Log($"Updated {count} locations.", LogLevel.Info);
	}

}
