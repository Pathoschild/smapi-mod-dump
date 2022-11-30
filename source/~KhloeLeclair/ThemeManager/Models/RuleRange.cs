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

namespace Leclair.Stardew.ThemeManager.Models;

public struct RuleRange {

	public static readonly RuleRange ALL = new RuleRange();

	public int MinOffset;
	public int MaxOffset;

	public int MinHit;
	public int MaxHit;

	public RuleRange() {
		MinOffset = 0;
		MaxOffset = int.MaxValue;
		MinHit = 0;
		MaxHit = int.MaxValue;
	}

	public RuleRange(int minOffset = 0, int maxOffset = int.MaxValue, int minHit = 0, int maxHit = int.MaxValue) {
		MinOffset = minOffset;
		MaxOffset = maxOffset;
		MinHit = minHit;
		MaxHit = maxHit;
	}

	public bool Test(int offset, int hit) {
		return offset >= MinOffset && offset <= MaxOffset &&
			hit >= MinHit && hit <= MaxHit;
	}

	public static bool TryParse(string input, out RuleRange result) {
		if (string.IsNullOrEmpty(input)) {
			result = default;
			return false;
		}

		try {
			result = Parse(input);
			return true;
		} catch {
			result = default;
			return false;
		}
	}

	public static RuleRange Parse(string input) {
		if (string.Equals(input, "*") || string.Equals(input, "all", StringComparison.OrdinalIgnoreCase))
			return ALL;

		string? hitpart;
		string? offsetpart;

		int idx = input.IndexOf('|');
		if (idx == -1) {
			hitpart = input;
			offsetpart = null;
		} else if (idx == 0) {
			hitpart = null;
			offsetpart = input[1..];
		} else {
			hitpart = input[..idx];
			offsetpart = input[(idx + 1)..];
		}

		ParsePart(hitpart, out int minHit, out int maxHit);
		ParsePart(offsetpart, out int minOffset, out int maxOffset);

		return new RuleRange(minOffset, maxOffset, minHit, maxHit);
	}

	private static void ParsePart(string? input, out int min, out int max) {
		min = int.MinValue;
		max = int.MaxValue;

		if (string.IsNullOrEmpty(input))
			return;

		string[] parts = input.Split(',');
		foreach(string part in parts) {
			if (part.StartsWith(">=")) {
				int val = int.Parse(part.AsSpan(2));
				if (val > min)
					min = val;
			} else if (part.StartsWith('>')) {
				int val = int.Parse(part.AsSpan(1)) + 1;
				if (val > min)
					min = val;
			} else if (part.StartsWith("<=")) {
				int val = int.Parse(part.AsSpan(2));
				if (val < max)
					max = val;
			} else if (part.StartsWith('<')) {
				int val = int.Parse(part.AsSpan(1)) + 1;
				if (val < max)
					max = val;
			} else
				min = max = int.Parse(part);
		}

		if (max < min)
			min = max;
		if (min > max)
			max = min;
	}

}
