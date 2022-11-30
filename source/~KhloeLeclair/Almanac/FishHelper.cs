/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewModdingAPI;

using Leclair.Stardew.Almanac.Fish;
using Leclair.Stardew.Almanac.Models;
using StardewModdingAPI.Utilities;

namespace Leclair.Stardew.Almanac;

public static class FishHelper {

	public static bool SkipLocation(string key) {
		switch(key) {
			case "fishingGame":
			case "Temp":
			case "BeachNightMarket":
			case "IslandSecret":
			case "Backwoods":
				return true;
		}

		return false;
	}

	public static bool SkipFish(Farmer? who, int id) {
		who ??= Game1.player;

		switch(id) {
			case 898:
			case 899:
			case 900:
			case 901:
			case 902:
				return ! who.team.SpecialOrderRuleActive("LEGENDARY_FAMILY");
		}

		return false;
	}

	public static WaterType GetTrapWaterType(GameLocation location) {
		if (location is Farm farm) {
			// Based on logic from Farm
			if (farm.map.Properties.ContainsKey("FarmOceanCrabPotOverride"))
				return WaterType.Ocean;

			// Farm 6 has both depending on coordinates.
			if (Game1.whichFarm == 6)
				return WaterType.Freshwater | WaterType.Ocean;

			return WaterType.Freshwater;
		}

		// Ginger Island West
		if (location is IslandWest)
			return WaterType.Freshwater | WaterType.Ocean;

		// Beach
		if (location is Beach)
			return WaterType.Ocean;

		// For everything else, just ask it for (0, 0)

		var layer = location.map?.Layers?.First();
		int width = layer?.LayerWidth ?? 0;
		int height = layer?.LayerHeight ?? 0;

		if (width == 0 || height == 0)
			return location.catchOceanCrabPotFishFromThisSpot(0, 0)
				? WaterType.Ocean : WaterType.Freshwater;

		WaterType result = WaterType.None;

		int xStep = Math.Clamp(width / 10, 1, 15);
		int yStep = Math.Clamp(height / 10, 1, 15);

		for (int x = 0; x < width; x += xStep) {
			for (int y = 0; y < height; y += yStep) {
				if (location.catchOceanCrabPotFishFromThisSpot(x, y))
					result |= WaterType.Ocean;
				else
					result |= WaterType.Freshwater;
			}
		}

		return result;
	}

	public static SObject GetRoeForFish(SObject fish) {
		Color color = TailoringMenu.GetDyeColor(fish) ?? Color.Orange;
		if (fish.ParentSheetIndex == 698)
			color = new Color(61, 55, 42);

		ColoredObject result = new(812, 1, color);
		result.name = fish.Name + " Roe";
		result.preserve.Value = SObject.PreserveType.Roe;
		result.preservedParentSheetIndex.Value = fish.ParentSheetIndex;
		result.Price += fish.Price / 2;

		return result;
	}

	public static Dictionary<int, Dictionary<SubLocation, List<int>>> GetFishLocations() {
		Dictionary<int, Dictionary<SubLocation, List<int>>> result = new();

		var locations = Game1.content.Load<Dictionary<string, string>>(@"Data\Locations");

		foreach (var lp in locations) {
			if (SkipLocation(lp.Key))
				continue;

			for (int season = 0; season < WorldDate.MonthsPerYear; season++) {
				try {
					var data = GetLocationFish(lp.Key, season, locations);
					if (data == null)
						continue;

					foreach (var pair in data) {
						int zone = pair.Key;
						SubLocation sl = new(lp.Key, zone);

						foreach (int fish in pair.Value) {
							if (!result.TryGetValue(fish, out var locs))
								result[fish] = locs = new();

							if (locs.TryGetValue(sl, out var seasons))
								seasons.Add(season);
							else
								locs[sl] = new List<int>() { season };
						}
					}
				} catch {
					ModEntry.Instance.Log($"Uh oh: {lp.Key}");
				}
			}
		}

		return result;
	}

	public static Dictionary<int, List<SubLocation>> GetFishLocations(int season) {
		Dictionary<int, List<SubLocation>> result = new();

		var locations = Game1.content.Load<Dictionary<string, string>>(@"Data\Locations");
		foreach (var lp in locations) {
			if (SkipLocation(lp.Key))
				continue;

			var data = GetLocationFish(season, lp.Value);
			if (data == null)
				continue;

			foreach (var pair in data) {
				int zone = pair.Key;
				SubLocation sl = new(lp.Key, zone);

				foreach(int fish in pair.Value) {
					if (result.TryGetValue(fish, out var subs))
						subs.Add(sl);
					else
						result.Add(fish, new List<SubLocation>() { sl });
				}
			}
		}

		return result;
	}

	public static Dictionary<int, List<int>> GetLocationFish(GameLocation location, int season) {
		return GetLocationFish(location.Name, season);
	}

	public static Dictionary<int, List<int>> GetLocationFish(string key, int season, Dictionary<string, string>? locations = null) {
		if (key == "BeachNightMarket")
			key = "Beach";

		locations ??= Game1.content.Load<Dictionary<string, string>>(@"Data\Locations");
		Dictionary<int, List<int>> result;

		if (locations.ContainsKey(key))
			result = GetLocationFish(season, locations[key]);
		else
			result = new();

		Farm farm;

		try {
			var loc = Game1.getLocationFromName(key);
			if (loc is not Farm)
				return result;
			farm = (Farm) loc;
		} catch {
			return result;
		}

		// This line will forever live in shame as a stupid mistake.
		//result = new();

		// If we've got whichFarm 1 then we want to reset the list, as it never
		// falls back to the default getFish behavior.
		switch(Game1.whichFarm) {
			case 1:
				result.Clear();
				break;
		}

		string ovr = farm.getMapProperty("FarmFishLocationOverride");
		if (!string.IsNullOrEmpty(ovr)) {
			string[] bits = ovr.Split(' ');
			if (
				bits.Length > 1 &&
				!string.IsNullOrEmpty(bits[0]) &&
				float.TryParse(bits[1], out float value) &&
				value > 0 &&
				locations.ContainsKey(bits[0])
			) {
				// If the value is 1 or higher, we will never not use the
				// override, so clear the list.
				if (value >= 1)
					result.Clear();

				GetLocationFish(season, locations[bits[0]], result);
			}
		}

		switch(Game1.whichFarm) {
			case 1:
				GetLocationFish(season, locations["Forest"], result, true, 1);
				GetLocationFish(season, locations["Town"], result, true, 1);
				break;
			case 2:
				AddFish(734, -1, result);
				GetLocationFish(season, locations["Forest"], result, true, 1);
				break;
			case 3:
				GetLocationFish(season, locations["Forest"], result, true, 0);
				break;
			case 4:
				GetLocationFish(season, locations["Mountain"], result, true);
				break;
			case 5:
				GetLocationFish(season, locations["Forest"], result, true, 1);
				break;
			case 6:
				AddFish(152, -1, result);
				AddFish(723, -1, result);
				AddFish(393, -1, result);
				AddFish(719, -1, result);
				AddFish(718, -1, result);
				GetLocationFish(season, locations["Beach"], result, true);
				break;
		}

		return result;
	}

	private static void AddFish(int fish, int zone, Dictionary<int, List<int>> existing) {
		if (existing.TryGetValue(zone, out List<int>? result)) {
			if (!result.Contains(fish))
				result.Add(fish);
		} else
			existing.Add(zone, new() { fish });
	}

	public static Dictionary<int, List<int>> GetLocationFish(int season, string? data) {
		return GetLocationFish(season, data, new());
	}

	public static Dictionary<int, List<int>> GetLocationFish(int season, string? data, Dictionary<int, List<int>> existing, bool no_zones = false, int limit_zone = -1) {
		if (string.IsNullOrEmpty(data))
			return existing;

		string[] entries = data.Split('/')[4 + season].Split(' ', StringSplitOptions.RemoveEmptyEntries);

		for (int i = 0; (i + 1) < entries.Length; i += 2) {
			if (int.TryParse(entries[i], out int fish) && int.TryParse(entries[i + 1], out int zone)) {
				if (limit_zone != -1 && zone != -1 && limit_zone != zone)
					continue;

				AddFish(fish, no_zones ? -1 : zone, existing);
			} else
				ModEntry.Instance.Log($"Invalid fish data entry for season {season} (Fish ID:{entries[i]}, Zone:{entries[i + 1]})", LogLevel.Warn);
		}

		return existing;
	}
}
