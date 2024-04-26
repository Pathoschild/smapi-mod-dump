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

using Leclair.Stardew.Common.Events;

using StardewValley;
using StardewModdingAPI;
using StardewValley.Internal;
using StardewValley.TokenizableStrings;
using StardewValley.Network;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using System.Text;

namespace Leclair.Stardew.CloudySkies;

public partial class ModEntry {

	[ConsoleCommand("cs_reload", "Force the current weather layers and effects to be recreated.")]
	public void ReloadCommand(string name, string[] args) {
		UncacheLayers();
		Log($"Invalidated weather cache.", LogLevel.Info);
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
				TokenParser.ParseText(entry.Value.DisplayName ?? ""),
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

		UseWeatherTotem(Game1.player, input, item);
	}

}
