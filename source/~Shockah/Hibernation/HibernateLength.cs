/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Shockah.CommonModCode.Stardew;
using System;
using System.Text.RegularExpressions;

namespace Shockah.Hibernation
{
	public enum HibernateLengthUnit
	{
		Nights,
		Weeks,
		Seasons,
		Years,
		Forever
	}

	public readonly struct HibernateLength
	{
		private static readonly Regex LengthRegex = new("(\\d+)([ndwsy])");

		public readonly int Value;
		public readonly HibernateLengthUnit Unit;

		public HibernateLength(int value, HibernateLengthUnit unit)
		{
			this.Value = value;
			this.Unit = unit;
		}

		public static HibernateLength? ParseOrNull(string toParse)
		{
			if (toParse.Trim().Equals("forever", StringComparison.OrdinalIgnoreCase))
				return new(1, HibernateLengthUnit.Forever);

			var match = LengthRegex.Match(toParse);
			if (!match.Success)
				return null;
			int number = int.Parse(match.Groups[1].Value);
			return match.Groups[2].Value.ToLower() switch
			{
				"n" or "d" => new(number, HibernateLengthUnit.Nights),
				"w" => new(number, HibernateLengthUnit.Weeks),
				"s" => new(number, HibernateLengthUnit.Seasons),
				"y" => new(number, HibernateLengthUnit.Years),
				_ => throw new InvalidOperationException("This should not happen."),
			};
		}

		public override string ToString()
		{
			return Unit switch
			{
				HibernateLengthUnit.Nights => $"{Value}d",
				HibernateLengthUnit.Weeks => $"{Value}w",
				HibernateLengthUnit.Seasons => $"{Value}s",
				HibernateLengthUnit.Years => $"{Value}y",
				HibernateLengthUnit.Forever => "Forever",
				_ => throw new ArgumentException($"{nameof(HibernateLengthUnit)} has an invalid value.")
			};
		}

		public int GetDayCount()
		{
			return Unit switch
			{
				HibernateLengthUnit.Nights => Value,
				HibernateLengthUnit.Weeks => Value * 7,
				HibernateLengthUnit.Seasons => Value * WorldDateExt.DaysPerSeason,
				HibernateLengthUnit.Years => Value * WorldDateExt.DaysPerSeason * 4,
				HibernateLengthUnit.Forever => int.MaxValue,
				_ => throw new ArgumentException($"{nameof(HibernateLengthUnit)} has an invalid value.")
			};
		}

		public string GetLocalized()
		{
			return Unit switch
			{
				HibernateLengthUnit.Nights => Hibernation.Instance.Helper.Translation.Get("hibernateLength.unit.nights", new { Value = Value }),
				HibernateLengthUnit.Weeks => Hibernation.Instance.Helper.Translation.Get("hibernateLength.unit.weeks", new { Value = Value }),
				HibernateLengthUnit.Seasons => Hibernation.Instance.Helper.Translation.Get("hibernateLength.unit.seasons", new { Value = Value }),
				HibernateLengthUnit.Years => Hibernation.Instance.Helper.Translation.Get("hibernateLength.unit.years", new { Value = Value }),
				HibernateLengthUnit.Forever => Hibernation.Instance.Helper.Translation.Get("hibernateLength.unit.forever"),
				_ => throw new ArgumentException($"{nameof(HibernateLengthUnit)} has an invalid value.")
			};
		}
	}
}