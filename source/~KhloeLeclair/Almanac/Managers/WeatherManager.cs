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

using Leclair.Stardew.Common.Events;
using Leclair.Stardew.Common.Types;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;

using Leclair.Stardew.Almanac.Models;


namespace Leclair.Stardew.Almanac.Managers {
	public class WeatherManager : BaseManager {

		// Rules
		private Dictionary<string, WeatherRule> Rules;
		private WeatherRule[] SortedRules;
		private bool RulesLoaded = false;

		private readonly object RuleLock = new();

		// Cache

		private int CachedSeed;
		private int CachedYear;

		private int[] CachedWeather;
		private int[] CachedIslandWeather;

		private readonly object WeatherCache = new();

		public WeatherManager(ModEntry mod) : base(mod) { }

		public void Invalidate() {
			WithCache(() => {
				CachedSeed = CachedYear = -1;
				CachedWeather = null;
				CachedIslandWeather = null;
			});

			WithRules(() => {
				Rules = null;
				SortedRules = null;
				RulesLoaded = false;
			});
		}

		#region Events

		[Subscriber]
		private void OnSaveLoaded(object sender, SaveLoadedEventArgs e) {
			CachedSeed = CachedYear = -1;
			CachedWeather = null;
			CachedIslandWeather = null;
		}

		#endregion

		#region Lock Helpers

		private void WithRules(Action action) {
			lock (RuleLock) {
				action();
			}
		}

		private void WithCache(Action action) {
			lock (WeatherCache) {
				action();
			}
		}

		#endregion

		#region Data Access

		public IEnumerable<WeatherRule> GetRules() {
			if (!RulesLoaded)
				LoadRules();

			return SortedRules;
		}

		public int GetWeatherForDate(int seed, int day) {
			return GetWeatherForDate(seed, day, GameLocation.LocationContext.Default);
		}

		public int GetWeatherForDate(int seed, int day, GameLocation.LocationContext context) {
			WorldDate date = new();
			date.TotalDays = day;
			return GetWeatherForDate(seed, date, context);
		}

		public int GetWeatherForDate(int seed, WorldDate date, GameLocation.LocationContext context) {
			UpdateCache(seed, date.Year);

			int[] Forecast;
			if (context == GameLocation.LocationContext.Island)
				Forecast = CachedIslandWeather;
			else
				Forecast = CachedWeather;

			return Forecast[date.SeasonIndex * WorldDate.DaysPerMonth + date.DayOfMonth - 1];
		}

		#endregion

		#region Calculation

		private void UpdateCache(int seed, int year) {
			if (CachedSeed == seed && CachedYear == year)
				return;

			CachedSeed = seed;
			CachedYear = year;

			CachedWeather = new int[WorldDate.DaysPerMonth * 4];
			CachedIslandWeather = new int[CachedWeather.Length];

			bool[] RuledDates = new bool[CachedWeather.Length];
			bool[] RuledIslandDates = new bool[CachedIslandWeather.Length];

			WorldDate date = new(year, "spring", 1);

			// First, we calculate the raw weather on any given date.
			for(int i = 1; i <= CachedWeather.Length; i++) {
				CachedWeather[i - 1] = WeatherHelper.GetRawWeatherForDate(seed, date, GameLocation.LocationContext.Default);
				CachedIslandWeather[i - 1] = WeatherHelper.GetRawWeatherForDate(seed, date, GameLocation.LocationContext.Island);

				// Standard weather is overwritten on some dates. Don't
				// let us modify the weather on those days with rules.
				if (Game1.getWeatherModificationsForDate(date, -1) != -1)
					RuledDates[i - 1] = true;

				date.TotalDays++;
			}

			if (!Mod.Config.EnableWeatherRules)
				return;

			// Now, we need to start processing rules.
			IEnumerable<WeatherRule> rules = GetRules();

			foreach (WeatherRule rule in rules) {
				if (rule == null || !rule.Enabled)
					continue;

				if (rule.FirstYear > year || rule.LastYear < year)
					continue;

				if (rule.ValidYears != null && rule.ValidYears.Contains(year))
					continue;

				if (rule.Context == RuleContext.Default || rule.Context == RuleContext.All)
					ExecuteRule(rule, seed, year, CachedWeather, RuledDates);

				if (rule.Context == RuleContext.Island || rule.Context == RuleContext.All)
					ExecuteRule(rule, seed, year, CachedIslandWeather, RuledIslandDates);
			}
		}

		private void ExecuteRule(WeatherRule rule, int seed, int year, int[] Weather, bool[] RuledDates) {
			var pattern = rule.CalculatedPattern;
			if (pattern == null)
				return;

			int[] seasons = rule.ValidSeasonIndices;

			if (rule.Period == RulePeriod.Year) {
				ExecuteRuleInner(rule, pattern, seed, year, 0, Weather, RuledDates);

			} else if (rule.Period == RulePeriod.Season) {
				for(int i = 0; i < 4; i++) {
					int first = (i * WorldDate.DaysPerMonth);

					if (!seasons.Contains(i))
						continue;

					ArraySegment<int> weatherSeg = new(Weather, first, WorldDate.DaysPerMonth);
					ArraySegment<bool> ruledSeg = new(RuledDates, first, WorldDate.DaysPerMonth);

					ExecuteRuleInner(rule, pattern, seed, year, first, weatherSeg, ruledSeg);
				}

			} else if (rule.Period == RulePeriod.Week) {
				for (int i = 0; i < 16; i++) {
					int first = (i * 7);

					if (!seasons.Contains(i / 4))
						continue;

					ArraySegment<int> weatherSeg = new(Weather, first, 7);
					ArraySegment<bool> ruledSeg = new(RuledDates, first, 7);

					ExecuteRuleInner(rule, pattern, seed, year, first, weatherSeg, ruledSeg);
				}
			}
		}

		private void ExecuteRuleInner(WeatherRule rule, RulePatternEntry[][] pattern, int seed, int year, int offset, ArraySegment<int> Weather, ArraySegment<bool> RuledDates) {

			Dictionary<int, PossibleMatch> possibleMatches = new();

			RuleDateRange[] dates;
			if (rule.Dates != null && rule.Dates.Length > 0)
				dates = rule.Dates;
			else
				dates = new RuleDateRange[] {
					new(1, Weather.Count)
				};


			int min_cost = int.MaxValue;

			foreach(RuleDateRange range in dates) {
				int start = range.Start;
				int end = range.End;

				if (start < 1) start = 1;				
				if (end > Weather.Count) end = Weather.Count;

				end -= (pattern.Length - 1);
				if (end < start)
					continue;

				for(int i = start; i <= end; i++) {
					if (possibleMatches.ContainsKey(i))
						continue;

					int cost = GetPatternCost(i, pattern, Weather, RuledDates);
					if (cost < 0)
						continue;

					if (cost < min_cost)
						min_cost = cost;

					possibleMatches[i] = new(i, cost);
				}
			}

			// We don't return if we have a min_cost 0 because we want to flag
			// the matching days as rule'd so other rules don't override the
			// valid weather.
			if (possibleMatches.Count == 0)
				return;

			// Sort the matches

			List<PossibleMatch> matches = possibleMatches.Values.Where(match => match.Cost <= min_cost).ToList();
			matches.Sort((a, b) => a.Cost - b.Cost);

			Random rnd = new(seed + (year * 112) + offset);

			// TODO: Apply weights when picking a random match.
			int idx = rnd.Next(0, matches.Count);
			if (idx == matches.Count)
				idx--;

			ApplyPattern(rnd, matches[idx].Offset, pattern, Weather, RuledDates);
		}

		private void ApplyPattern(Random rnd, int offset, RulePatternEntry[][] pattern, ArraySegment<int> Weather, ArraySegment<bool> RuledDates) {

			for (int i = 0; i < pattern.Length; i++) {
				int idx = offset + i - 1;

				if (idx < 0 || idx > Weather.Count)
					continue;

				RuledDates[idx] = true;

				int current = Weather[idx];
				bool matched = false;
				float total = 0f;
				foreach (var item in pattern[i]) {
					total += item.Weight;
					if (current == ((int) item.Weather)) {
						matched = true;
						break;
					}
				}

				if (matched)
					continue;

				float val = (float) rnd.NextDouble() * total;

				foreach (var item in pattern[i]) {
					if (item.Weight <= 0)
						continue;

					if (val <= item.Weight) {
						Weather[idx] = ((int)item.Weather);
						break;
					}

					val -= item.Weight;
				}
			}
		}

		private int GetPatternCost(int offset, RulePatternEntry[][] pattern, ArraySegment<int> Weather, ArraySegment<bool> RuledDates) {

			int cost = 0;

			for (int i = 0; i < pattern.Length; i++) {
				int idx = offset + i - 1;

				if (idx < 0 || idx > Weather.Count)
					return -1;

				int current = Weather[idx];

				bool matched = false;
				foreach (var item in pattern[i]) {
					if (current == ((int)item.Weather)) {
						matched = true;
						break;
					}
				}

				if (!matched) {
					if (RuledDates[idx])
						return -1;
					else
						cost++;
				}
			}

			return cost;
		}

		#endregion

		#region Data Loading

		public void HydrateRules(Dictionary<string, WeatherRule> rules, WeatherRule[] additional) {
			foreach (WeatherRule rule in additional) {
				rules[rule.Id] = rule;
			}
		}

		public void LoadRules() {
			WithRules(() => {
				const string path = "assets/weather_rules.json";
				Rules = new();
				WeatherRule[] newRules = null;

				try {
					newRules = Mod.Helper.Data.ReadJsonFile<WeatherRule[]>(path);
					if (newRules == null)
						Log($"The {path} file is missing or invalid.", LogLevel.Error);
				} catch (Exception ex) {
					Log($"The {path} file is invalid.", LogLevel.Error, ex);
				}

				if (newRules != null)
					HydrateRules(Rules, newRules);

				// Now read rules from content packs.
				foreach(var cp in Mod.Helper.ContentPacks.GetOwned()) {
					if (!cp.HasFile("weather_rules.json"))
						continue;

					newRules = null;
					try {
						newRules = cp.ReadJsonFile<WeatherRule[]>("weather_rules.json");
					} catch (Exception ex) {
						Log($"The weather_rules.json file of {cp.Manifest.Name} is invalid.", LogLevel.Error, ex);
					}

					if (newRules != null)
						HydrateRules(Rules, newRules);
				}

				// Alright, now we need to sort the rules.
				List<WeatherRule> sorted = new();

				foreach(WeatherRule rule in Rules.Values) {
					if (! rule.Enabled) continue;
					sorted.Add(rule);
				}

				sorted.Sort((a,b) => b.Priority - a.Priority);
				SortedRules = sorted.ToArray();

				Log($"Loaded {Rules.Count} total and {SortedRules.Length} enabled weather rules.", LogLevel.Debug);
				RulesLoaded = true;
			});
		}

		#endregion
	}

	internal struct PossibleMatch {
		public int Offset { get; }
		public int Cost { get; }

		internal PossibleMatch(int offset, int cost) {
			Offset = offset;
			Cost = cost;
		}
	}
}
