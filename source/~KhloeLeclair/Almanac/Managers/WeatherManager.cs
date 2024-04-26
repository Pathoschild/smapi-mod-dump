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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Leclair.Stardew.Common.Enums;
using Leclair.Stardew.Common.Events;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.GameData;

using Leclair.Stardew.Almanac.Models;
using StardewValley.GameData.LocationContexts;

namespace Leclair.Stardew.Almanac.Managers;

public class WeatherManager : BaseManager {

	// Rules
	private Dictionary<string, WeatherRule>? Rules;
	private WeatherRule[]? SortedRules;
	private bool RulesLoaded = false;

	private readonly object RuleLock = new();

	// Cache
	private ulong CachedSeed;
	private int CachedYear;

	private readonly Dictionary<string, string[]> CachedWeather = new();

	public WeatherManager(ModEntry mod) : base(mod) { }

	public void Invalidate() {
		WithCache(() => {
			CachedSeed = 0;
			CachedYear = -1;
			CachedWeather.Clear();
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
			WithCache(() => {
				CachedSeed = 0;
				CachedYear = -1;
				CachedWeather.Clear();
			});
		}

	#endregion

	#region Lock Helpers

		private void WithRules(Action action) {
			lock (RuleLock) {
				action();
			}
		}

		private void WithCache(Action action) {
			lock ((CachedWeather as ICollection).SyncRoot) {
				action();
			}
		}

	#endregion

	#region Data Access

		public IEnumerable<WeatherRule> GetRules() {
			if (!RulesLoaded)
				LoadRules();
			return SortedRules!;
		}

		public string GetWeatherForDate(ulong seed, int day) {
			return GetWeatherForDate(seed, day, Game1.currentLocation);
		}

		public string GetWeatherForDate(ulong seed, int day, GameLocation location) {
			WorldDate date = new();
			date.TotalDays = day;
			return GetWeatherForDate(seed, date, location.GetLocationContext(), location.GetLocationContextId());
		}

		public string GetWeatherForDate(ulong seed, WorldDate date, LocationContextData context, string contextID) {
			UpdateCache(seed, date.Year, context, contextID);

			string[] Forecast = CachedWeather[contextID];
			return Forecast[date.SeasonIndex * ModEntry.DaysPerMonth + date.DayOfMonth - 1];
		}

	#endregion

	#region Calculation

		private void UpdateCache(ulong seed, int year, LocationContextData context, string contextID ) {
			lock((CachedWeather as ICollection).SyncRoot) {

				if (CachedSeed != seed || CachedYear != year)
					CachedWeather.Clear();

				if (CachedWeather.ContainsKey(contextID))
					return;

				CachedSeed = seed;
				CachedYear = year;

				string[] forecast = new string[ModEntry.DaysPerMonth * 4];
				bool[] RuledDates = new bool[forecast.Length];

				WorldDate date = new(year, "spring", 1);

				// First, we calculate the raw weather on any given date.
				for(int i = 0; i < forecast.Length; i++) {
					forecast[i] = WeatherHelper.GetRawWeatherForDate(seed, date, context, contextID);
					// Standard weather is overwritten on some dates. Don't
					// let us modify the weather on those days with rules.
					// TODO: See how this changes for different contexts.
					if (context == Game1.currentLocation.GetLocationContext() && Game1.getWeatherModificationsForDate(date, "didnotchange") != "didnotchange")
						RuledDates[i] = true;

					date.TotalDays++;
				}

				if (Mod.Config.EnableWeatherRules) {
					// Now, we need to start processing rules.
					IEnumerable<WeatherRule> rules = GetRules();

					foreach (WeatherRule rule in rules) {
						if (rule == null || !rule.Enabled)
							continue;

						if (rule.FirstYear > year || rule.LastYear < year)
							continue;

						if (rule.ValidYears != null && rule.ValidYears.Contains(year))
							continue;

						if (rule.Contexts != null) {
							bool matched = false;
							foreach (string ctx in rule.Contexts) {
								if (contextID.Equals(ctx, StringComparison.OrdinalIgnoreCase)) {
									matched = true;
									break;
								}
							}
							if (!matched)
								continue;
						}
						ExecuteRule(rule, seed, year, forecast, RuledDates);
					}
				}
			
				CachedWeather[contextID] = forecast;
			}
		}

		private void ExecuteRule(WeatherRule rule, ulong seed, int year, string[] Weather, bool[] RuledDates) {
			var pattern = rule.CalculatedPattern;
			if (pattern == null)
				return;
			int[] seasons = rule.ValidSeasonIndices;
			int daysPerYear = WorldDate.MonthsPerYear * ModEntry.DaysPerMonth;
			int weeksPerYear = daysPerYear / 7;
			if (rule.Period == TimeScale.Total) {
				Log($"Weather rule has period \"Total\" which is unsupported for weather rules. Treating as \"Year\".", LogLevel.Warn);
				ExecuteRuleInner(rule, pattern, seed, year, 0, Weather, RuledDates);
			} else if (rule.Period == TimeScale.Year) {
				ExecuteRuleInner(rule, pattern, seed, year, 0, Weather, RuledDates);
			} else if (rule.Period == TimeScale.Season) {
				for(int i = 0; i < 4; i++) {
					int first = (i * ModEntry.DaysPerMonth);
					if (!seasons.Contains(i) && !seasons.Contains(-1))
					continue;

					ArraySegment<string> weatherSeg = new(Weather, first, ModEntry.DaysPerMonth);
					ArraySegment<bool> ruledSeg = new(RuledDates, first, ModEntry.DaysPerMonth);

					ExecuteRuleInner(rule, pattern, seed, year, first, weatherSeg, ruledSeg);
				}
			} else if (rule.Period == TimeScale.Week) {
				for (int i = 0; i < weeksPerYear; i++) {
					int first = (i * 7);

					if (!seasons.Contains(i / 4) && !seasons.Contains(-1))
						continue;

					ArraySegment<string> weatherSeg = new(Weather, first, 7);
					ArraySegment<bool> ruledSeg = new(RuledDates, first, 7);

					ExecuteRuleInner(rule, pattern, seed, year, first, weatherSeg, ruledSeg);
				}
			}
		}

		private static void ExecuteRuleInner(WeatherRule rule, RulePatternEntry[][] pattern, ulong seed, int year, int offset, ArraySegment<string> Weather, ArraySegment<bool> RuledDates) {
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

			Random rnd = new((int)seed + (year * 112) + offset);

			// TODO: Apply weights when picking a random match.
			int idx = rnd.Next(0, matches.Count);
			if (idx == matches.Count)
				idx--;

			ApplyPattern(rnd, matches[idx].Offset, pattern, Weather, RuledDates);
		}

		private static void ApplyPattern(Random rnd, int offset, RulePatternEntry[][] pattern, ArraySegment<string> Weather, ArraySegment<bool> RuledDates) {
			for (int i = 0; i < pattern.Length; i++) {
				int idx = offset + i - 1;
				if (idx < 0 || idx > Weather.Count)
					continue;

				RuledDates[idx] = true;
				string current = Weather[idx];
				bool matched = false;
				float total = 0f;
				foreach (var item in pattern[i]) {
					total += item.Weight;
					if (current == item.Weather) {
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
						Weather[idx] = item.Weather;
						break;
					}

					val -= item.Weight;
				}
			}
		}

		private static int GetPatternCost(int offset, RulePatternEntry[][] pattern, ArraySegment<string> Weather, ArraySegment<bool> RuledDates) {
			int cost = 0;
			for (int i = 0; i < pattern.Length; i++) {
				int idx = offset + i - 1;

				if (idx < 0 || idx > Weather.Count)
					return -1;

				string current = Weather[idx];
				bool matched = false;
				foreach (var item in pattern[i]) {
					if (current == item.Weather) { 
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

	public static void HydrateRules(Dictionary<string, WeatherRule> rules, WeatherRule[] additional) {
		foreach (WeatherRule rule in additional) {
			if (!string.IsNullOrEmpty(rule.Id))
				rules[rule.Id] = rule;
		}
	}

	[MemberNotNull(nameof(Rules))]
	[MemberNotNull(nameof(SortedRules))]
	public void LoadRules() {
		lock (RuleLock) {
			const string path = "assets/weather_rules.json";
			Rules = new();
			WeatherRule[]? newRules = null;

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
		}
	}

		#endregion

		#region Daily Update

		public void UpdateForNewDay() {
			if (! Mod.Config.EnableDeterministicWeather)
				return;

			if (!Game1.IsMasterGame)
				return;

			var contexts = Game1.content.Load<Dictionary<string, LocationContextData>>(@"Data\LocationContexts");
			WorldDate date = new(Game1.year, Game1.currentSeason, Game1.dayOfMonth + 1);
			foreach(var entry in contexts) {
				var weather = Game1.netWorldState.Value.GetWeatherForLocation(entry.Key);
				weather.weatherForTomorrow.Value = GetWeatherForDate(Mod.GetBaseWorldSeed(), date, entry.Value, entry.Key);
			}

			// Since we've changed weather, we need to re-copy.
			foreach(var entry in contexts) {
				if (entry.Value.CopyWeatherFromLocation == null)
					continue;

				try {
					Game1.netWorldState.Value
						.GetWeatherForLocation(entry.Key)
						.CopyFrom(
							Game1.netWorldState.Value
							.GetWeatherForLocation(
								entry.Value.CopyWeatherFromLocation
							));
				} catch(Exception ex) {
					Log($"Unable to copy weather from context {entry.Key} to {entry.Value.CopyWeatherFromLocation}.", LogLevel.Warn, ex, LogLevel.Trace);
				}
			}
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
