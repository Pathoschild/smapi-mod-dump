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

using Newtonsoft.Json;

using Leclair.Stardew.Common.Enums;

namespace Leclair.Stardew.Almanac.Models {

	public enum RuleContext {
		Default,
		Island,
		All
	};

	public enum RuleWeather {
		Sun = 0,
		Rain = 1,
		Wind = 2,
		Storm = 3,
		Festival = 4,
		Snow = 5
	};

	public enum RuleWeight {
		None,
		Start,
		Middle,
		End
	};

	public struct RuleDateRange {
		public int Start { get; set; }
		public int End { get; set; }

		public RuleDateRange(int start, int end) {
			Start = start;
			End = end;
		}
	}

	public struct RulePatternEntry {
		public RuleWeather Weather { get; set; }
		public float Weight { get; set; } = 1;

		public RulePatternEntry(RuleWeather weather, float weight) {
			Weather = weather;
			Weight = weight;
		}
	}

	public class WeatherRule {
		public string Id { get; set; }
		public string[] Contexts { get; set; }

		public bool Enabled { get; set; } = true;		
		public int Priority { get; set; } = 0;

		public int FirstYear { get; set; } = 1;
		public int LastYear { get; set; } = int.MaxValue;

		public int[] ValidYears { get; set; } = null;

		public Season[] ValidSeasons { get; set; } = new[] {
			Season.Spring,
			Season.Summer,
			Season.Fall,
			Season.Winter,
		};

		public TimeScale Period { get; set; }
		public RuleDateRange[] Dates { get; set; }

		public RulePatternEntry[][] WeightedPattern { get; set; }
		public RuleWeather[][] Pattern { get; set; }

		public RuleWeight Weight { get; set; } = RuleWeight.None;

		[JsonIgnore]
		public int[] ValidSeasonIndices {
			get {
				int[] ret = new int[ValidSeasons.Length];
				for(int i = 0; i < ret.Length; i++)
					ret[i] = ((int)ValidSeasons[i]);

				return ret;
			}
		}

		[JsonIgnore]
		public RulePatternEntry[][] CalculatedPattern {
			get {
				if (WeightedPattern != null)
					return WeightedPattern;

				if (Pattern == null)
					return null;

				RulePatternEntry[][] result = new RulePatternEntry[Pattern.Length][];

				for (int i = 0; i < result.Length; i++) {
					RuleWeather[] input = Pattern[i];
					result[i] = new RulePatternEntry[input.Length];
					for (int j = 0; j < input.Length; j++) {
						result[i][j] = new RulePatternEntry {
							Weather = input[j],
							Weight = GetDefaultWeight(input[j])
						};
					}
				}

				return result;
			}
		}

		public static float GetDefaultWeight(RuleWeather weather) {
			if (weather == RuleWeather.Storm)
				return 0.25f;

			if (weather == RuleWeather.Festival)
				return 0f;

			return 1f;
		}
	}
}
