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
using StardewValley.GameData.LocationContexts;
using StardewValley.GameData.Locations;
using Leclair.Stardew.Common.Enums;
using System.Security.Policy;

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

	public static bool SkipFish(Farmer who, string id) {
		who ??= Game1.player;

		switch(id) {
			case "(O)898":
			case "(O)899":
			case "(O)900":
			case "(O)901":
			case "(O)902":
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

		ColoredObject result = new ColoredObject("812", 1, color);
		result.name = fish.Name + " Roe";
		result.preserve.Value = SObject.PreserveType.Roe;
		result.preservedParentSheetIndex.Value = fish.QualifiedItemId;
		result.Price += fish.sellToStorePrice() / 2;

		return result;
	}

	/// <summary>
	/// Returns locations a fish can be caught
	/// </summary>
	/// <returns> Dictionary<fishItemID, Dictionary<zone/Sublocation.Area, List<seasons>>></returns>
	public static Dictionary<string, Dictionary<SubLocation, List<int>>> GetFishLocations() {
		//Dictionary<SubLocation, seasons>
		Dictionary<string, Dictionary<SubLocation, List<int>>> result = new();
		//Dictionary<locationId, locationData>
		var locations = Game1.content.Load<Dictionary<string, LocationData>>("Data\\Locations");

		foreach (var lp in locations) {
			if (SkipLocation(lp.Key))
				continue;

			for (int season = 0; season < WorldDate.MonthsPerYear; season++) {
				try {
					//Dictionary<zone/Sublocation.Area, List<fishItemId>>
					Dictionary<string, List<string>> data = GetLocationFish(lp.Key, season, locations);
					if (data == null)
						continue;

					foreach (var pair in data) {
						string zone = pair.Key;
						//lp.Key = location name
						SubLocation sl = new(lp.Key, zone);

						foreach (string fish in pair.Value) {
							//If result doesn't have this fish ID, add a new Dictionary<Sublocation, List<seasons>
							if (!result.TryGetValue(fish, out var locs))
								result[fish] = locs = new();
							//If locs has this Sublocation, add the current season to it's List<seasons>
							//Else add a new List<seasons> for this Sublocation with current season
							if (locs.TryGetValue(sl, out var seasons))
								seasons.Add(season);
							else
								locs[sl] = new List<int>() { season };
						}
					}
				} catch {
					ModEntry.Instance.Log($"Error at FishHelper line 156",LogLevel.Warn);
				}
			}
		}

		return result;
	}

	public static Dictionary<string, List<SubLocation>> GetFishLocations(int season) {
		Dictionary<string, List<SubLocation>> result = new();

		var locations = Game1.content.Load<Dictionary<string, LocationData>>("Data\\Locations");
		foreach (var lp in locations) {
			if (SkipLocation(lp.Key))
				continue;

			var data = GetLocationFish(season, lp.Value);
			if (data == null)
				continue;

			foreach (var pair in data) {
				string zone = pair.Key;
				SubLocation sl = new(lp.Key, zone);

				foreach(string fish in pair.Value) {
					if (result.TryGetValue(fish, out var subs))
						subs.Add(sl);
					else
						result.Add(fish, new List<SubLocation>() { sl });
				}
			}
		}

		return result;
	}

	public static Dictionary<string, List<string>> GetLocationFish(GameLocation location, int season) {
		return GetLocationFish(location.Name, season);
	}

	public static Dictionary<string, List<string>> GetLocationFish(string key, int season, Dictionary<string, LocationData>? locations = null) {
		if (key == "BeachNightMarket")
			key = "Beach";

		locations ??= Game1.content.Load<Dictionary<string, LocationData>>("Data\\Locations");
		Dictionary<string, List<string>> result;
		GameLocation loc;
		if (locations.ContainsKey(key) && ContainsFish(locations[key]))
			try {
				result = GetLocationFish(season, locations[key]);
			} catch {
				result = new();
				ModEntry.Instance.Log($"Error at FishHelper line 208", LogLevel.Warn);
			}
		else
			result = new();

		Farm farm;

		try {
			loc = Game1.getLocationFromName(key);
			if (!loc.IsFarm)
				return result;
			farm = (Farm) loc;
		} catch {
			loc = Game1.getLocationFromName(key);
			if (loc == null)
				return result;
			else if (loc is not Farm)
				return result;
			farm = (Farm) loc;
			ModEntry.Instance.Log($"Error at {loc.DisplayName}, farm = loc section", LogLevel.Warn);
		}

		// This line will forever live in shame as a stupid mistake.
		//result = new();

		// If we've got whichFarm 1 then we want to reset the list, as it never
		// falls back to the default getFish behavior.
		switch (Game1.whichFarm) {
			case 1:
				result.Clear();
				break;
		}
		ModEntry.Instance.Log("Calling override...");
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

				try {
					GetLocationFish(season, locations[bits[0]], result);
				} catch {
					ModEntry.Instance.Log($"Error at {getLocName(locations[bits[0]])}, bits section.", LogLevel.Warn);
				}

			}
		}
		try{
			GetLocationFish(season, locations[Game1.GetFarmTypeKey()], result);
		} catch {
			ModEntry.Instance.Log($"Error at {getLocName(locations[Game1.GetFarmTypeKey()])}, farm key section.", LogLevel.Warn);
		}

		return result;
	}

	private static void AddFish(string fishName, string fish, string zone, Dictionary<string, List<string>> existing) {
		if (existing.TryGetValue(zone, out List<string>? result)) {
			if (!result.Contains(fish)) {
				result.Add(fish);
				ModEntry.Instance.Log($"Added fish data entry for {fishName} (ID:{fish}, Zone:{zone})");
			}
		} else
			existing.Add(zone, new() { fish });
	}

	public static Dictionary<string, List<string>> GetLocationFish(int season, LocationData data) {
		return GetLocationFish(season, data, new());
	}

	public static Dictionary<string, List<string>> GetLocationFish(int season, LocationData data, Dictionary<string, List<string>> existing) {
		if (data.Equals(null))
			return existing;

		string name = getLocName(data);
		List<SpawnFishData> entries = data.Fish;
		if (!data.Equals(Game1.content.Load<Dictionary<string, LocationData>>("Data\\Locations")["Default"])) {
			LocationData Default = Game1.content.Load<Dictionary<string, LocationData>>("Data\\Locations")["Default"];
			foreach (SpawnFishData f in Default.Fish)
				if (!entries.Contains(f))
					entries.Add(f);
		}

		entries.Sort((a,b)=> { return a.Precedence.CompareTo(b.Precedence); });

		for (int i = 0; i < entries.Count; i++) {
			string fish = string.IsNullOrEmpty(entries[i].ItemId) ? "(O)213" : entries[i].ItemId;
			string fName = string.IsNullOrEmpty(ItemRegistry.Create(fish).DisplayName)? "Fish" : ItemRegistry.Create(fish).DisplayName;
			//if(fish=="(O)213") { fName = "Fish Taco"; }
			string zone = string.IsNullOrEmpty(entries[i].FishAreaId) ? "No zone" : entries[i].FishAreaId;
			if (fish.StartsWith("(O)") && canAddFish(entries[i], season, data))
				AddFish(fName, fish, zone, existing);
		}
		ModEntry.Instance.Log($"Added fish data for {name}, season {season}");
		return existing;
	}
	public static bool ContainsFish(LocationData loc) {
		List<SpawnFishData> catchables = loc.Fish;
		foreach (SpawnFishData f in catchables) if (f.ItemId.StartsWith("(O)")) return true;
		return false;
	}
	private static string getLocName(LocationData data) {
		string name = data.DisplayName == null ? "No DisplayName" : data.DisplayName;
		string endCheck = name.Substring(name.Length - 7);
		switch (endCheck) {
			case "Name]]]":
				name = "Farm";
				break;
			case ".11190]":
				name = "Town";
				break;
			case ".11174]":
				name = "Beach";
				break;
			case ".11176]":
				name = "Mountain";
				break;
			case ".11186]":
				name = "Forest";
				break;
			case ".11089]":
				name = "Sewer";
				break;
			case ".11062]":
				name = "Desert";
				break;
			case ".11114]":
				name = "Woods";
				break;
		}
		if (name.Length > 25) name = endCheck;
		return name;
	}
	private static bool canAddFish(SpawnFishData fish, int season, LocationData data) {
		if (fish.Season == null && fish.Condition == null) return true;
		if (fish.Season != null) {
			ModEntry.Instance.Log($"Season int: {season}, Season value {fish.Season}");
			switch (season) {
				case 0: if (fish.Season == StardewValley.Season.Spring) return true; break;
				case 1: if (fish.Season == StardewValley.Season.Summer) return true; break;
				case 2: if (fish.Season == StardewValley.Season.Fall) return true; break;
				case 3: if (fish.Season == StardewValley.Season.Winter) return true; break;
			}
			return false;
		}
		string[] conditionBits = fish.Condition.Split(' ');
		if (conditionBits[0] == "LOCATION_SEASON")
			for(int i = 2; i<conditionBits.Length; i++)
				switch (season) {
					case 0: if (conditionBits[i].ToLower() == "spring") return true; break;
					case 1: if (conditionBits[i].ToLower() == "summer") return true; break;
					case 2: if (conditionBits[i].ToLower() == "fall") return true; break;
					case 3: if (conditionBits[i].ToLower() == "winter") return true; break;
				}
		else if(GameStateQuery.CheckConditions(fish.Condition)) return true;
		return false;
	}
}
