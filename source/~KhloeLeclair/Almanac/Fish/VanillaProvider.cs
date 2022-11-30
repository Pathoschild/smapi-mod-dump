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
using System.Text.RegularExpressions;

using Leclair.Stardew.Common;

using StardewValley;
using StardewValley.GameData.FishPond;
using StardewValley.Tools;
using StardewModdingAPI;

using Leclair.Stardew.Almanac.Models;

namespace Leclair.Stardew.Almanac.Fish;

public class VanillaProvider : IFishProvider {

	public static readonly Regex WHITESPACE_REGEX = new(@"\s\s+|\n", RegexOptions.Compiled);

	public readonly ModEntry Mod;

	public VanillaProvider(ModEntry mod) {
		Mod = mod;
	}

	public string Name => nameof(VanillaProvider);
	public int Priority => 0;

	public IEnumerable<FishInfo> GetFish() {
		Dictionary<int, string> data = Game1.content.Load<Dictionary<int, string>>(@"Data\Fish");
		List<FishInfo> result = new();

		Dictionary<int, Dictionary<SubLocation, List<int>>> locations = Mod.Fish.GetFishLocations();

		List<FishPondData> pondData = Game1.content.Load<List<FishPondData>>(@"Data\FishPondData");

		foreach (var entry in data) {
			locations.TryGetValue(entry.Key, out Dictionary<SubLocation, List<int>>? locs);

			try {
				FishInfo? info = GetFishInfo(entry.Key, entry.Value, locs, pondData);
				if (info.HasValue)
					result.Add(info.Value);
			} catch(Exception ex) {
				ModEntry.Instance.Log($"Unable to process fish: {entry.Key}", LogLevel.Warn, ex);
			}
		}

		return result;
	}

	private static FishInfo? GetFishInfo(int id, string data, Dictionary<SubLocation, List<int>>? locations, List<FishPondData> pondData) {
		if (string.IsNullOrEmpty(data))
			return null;

		if (FishHelper.SkipFish(Game1.player, id))
			return null;

		string[] bits = data.Split('/');
		SObject obj = new(id, 1);

		if (bits.Length < 7 || obj is null)
			return null;

		int minSize;
		int maxSize;

		int[]? seasons = null;

		TrapFishInfo? trap = null;
		CatchFishInfo? caught = null;

		if (bits[1].Equals("trap")) {
			// Trap Fish
			WaterType type;
			if (bits[4] == "freshwater")
				type = WaterType.Freshwater;
			else if (bits[4] == "ocean")
				type = WaterType.Ocean;
			else
				throw new ArgumentOutOfRangeException("location", bits[4], "location must be freshwater or ocean");

			minSize = Convert.ToInt32(bits[5]);
			maxSize = Convert.ToInt32(bits[6]);

			trap = new(type);

			seasons = new int[WorldDate.MonthsPerYear];
			for (int i = 0; i < seasons.Length; i++)
				seasons[i] = i;

		} else {
			if (bits.Length < 13)
				return null;

			minSize = Convert.ToInt32(bits[3]);
			maxSize = Convert.ToInt32(bits[4]);
			int minLevel = Convert.ToInt32(bits[12]);

			int[] rawTimes = bits[5].Split(' ').Select(x => Convert.ToInt32(x)).ToArray();
			TimeOfDay[] times = new TimeOfDay[rawTimes.Length / 2];

			for (int i = 0, j = 0; i < times.Length && j < rawTimes.Length; i++, j += 2) {
				times[i] = new(
					rawTimes[j],
					rawTimes[j + 1]
				);
			}

			FishWeather weather;
			switch (bits[7]) {
				case "sunny":
					weather = FishWeather.Sunny;
					break;
				case "rainy":
					weather = FishWeather.Rainy;
					break;
				case "both":
					weather = FishWeather.Any;
					break;
				default:
					throw new ArgumentOutOfRangeException("weather", bits[7]);
			}

			caught = new(
				Locations: locations,
				Times: times,
				Weather: weather,
				Minlevel: minLevel
			);

			if (locations != null)
				seasons = locations.Values.SelectMany(x => x).Distinct().ToArray();
			else
				seasons = null;
		}

		string desc = obj.getDescription();
		if (desc != null)
			desc = WHITESPACE_REGEX.Replace(desc, " ");


		// Fish Ponds
		FishPondData? pond = null;
		if (pondData != null && ! obj.HasContextTag("fish_legendary"))
			foreach (var entry in pondData) {
				bool matched = true;
				foreach (string tag in entry.RequiredTags) {
					if (!obj.HasContextTag(tag)) {
						matched = false;
						break;
					}
				}

				if (!matched)
					continue;

				pond = entry;
				break;
			}

		PondInfo? pondInfo = null;
		if (pond != null) {
			// Taken from FishPond
			if (pond.SpawnTime == -1) {
				int price = obj.Price;
				pond.SpawnTime = price > 30 ? (price > 80 ? (price > 120 ? (price > 250 ? 5 : 4) : 3) : 2) : 1;
			}

			int initial = 10;

			if (pond.PopulationGates != null) {
				foreach (int key in pond.PopulationGates.Keys)
					if (key >= initial)
						initial = key - 1;
			}

			pondInfo = new(
				Initial: initial,
				SpawnTime: pond.SpawnTime,
				ProducedItems: pond.ProducedItems.Select(x => x.ItemID).Distinct().Select(x => (x == 812 ? FishHelper.GetRoeForFish(obj) : new SObject(x, 1)) as Item).ToList()
			);
		}

		return new FishInfo(
			Id: id.ToString(), // bits[0],
			Item: obj,
			Name: obj.DisplayName,
			Description: desc,
			Sprite: SpriteHelper.GetSprite(obj),
			Legendary: FishingRod.isFishBossFish(id),
			MinSize: minSize,
			MaxSize: maxSize,
			NumberCaught: who => {
				if (who.fishCaught.TryGetValue(id, out int[] caught) && caught.Length >= 2)
					return caught[0];
				return 0;
			},
			BiggestCatch: who => {
				if (who.fishCaught.TryGetValue(id, out int[] caught) && caught.Length >= 2)
					return caught[1];
				return 0;
			},
			Seasons: seasons,
			TrapInfo: trap,
			CatchInfo: caught,
			PondInfo: pondInfo
		);
	}
}
