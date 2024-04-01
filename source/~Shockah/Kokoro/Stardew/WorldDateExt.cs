/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using StardewValley;
using System;
using System.Text.RegularExpressions;

namespace Shockah.Kokoro.Stardew;

public static class WorldDateExt
{
	private static readonly string AllSeasonsRegexPattern = "(?:spring)|(?:summer)|(?:fall)|(?:winter)";
	private static readonly Regex SeasonDayYearRegex = new($"({AllSeasonsRegexPattern})\\s*(\\d+)\\s*y(?:ear)?\\s*(\\d+)", RegexOptions.IgnoreCase);
	private static readonly Regex SeasonYearRegex = new($"({AllSeasonsRegexPattern})\\s*y(?:ear)?\\s(\\d+)", RegexOptions.IgnoreCase);
	private static readonly Regex YearRegex = new($"y(?:ear)?\\s(\\d+)", RegexOptions.IgnoreCase);

	public static int GetDays(this Season season, int year)
		=> GetDaysInSeason((int)season, year);

	public static int GetDaysInSeason(int season, int year)
	{
		string seasonName = Utility.getSeasonNameFromNumber(season);
		int days = 1;
		while (true)
		{
			var dateString = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5678", days, (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es) ? seasonName.ToLower() : seasonName, year);
			if (dateString != Utility.getDateStringFor(days, season, year))
				return days - 1;
			days++;
		}
	}

	public static WorldDate GetByAddingDays(this WorldDate self, int days)
	{
		if (days == 0)
			return self;

		var day = self.DayOfMonth;
		var season = self.SeasonIndex;
		var year = self.Year;

		var daysInSeason = GetDaysInSeason(season, year);

		day += days;
		while (true)
		{
			var oldDay = day;
			var oldSeason = season;
			var oldYear = year;

			if (day > daysInSeason)
			{
				day -= daysInSeason;
				season++;
			}
			else if (day < 0)
			{
				day += daysInSeason;
				season--;
			}
			else if (season >= 4)
			{
				season -= 4;
				year++;
			}
			else if (season < 0)
			{
				season += 4;
				year--;
			}

			if (season != oldSeason || year != oldYear)
				daysInSeason = GetDaysInSeason(season, year);
			if (oldDay == day && oldSeason == season && oldYear == year)
				break;
		}

		return New(year, season, day);
	}

	public static WorldDate New(int year, Season season, int dayOfMonth)
		=> new(year, Enum.GetName(season)!.ToLower(), dayOfMonth);

	public static WorldDate New(int year, int seasonIndex, int dayOfMonth)
		=> New(year, (Season)seasonIndex, dayOfMonth);

	public static WorldDate? ParseDate(string text)
	{
		text = text.Trim();

		var match = SeasonDayYearRegex.Match(text);
		if (match.Success)
		{
			var season = Enum.Parse<Season>(match.Groups[1].Value);
			var day = int.Parse(match.Groups[2].Value);
			var year = int.Parse(match.Groups[3].Value);
			return New(year, season, day);
		}

		match = SeasonYearRegex.Match(text);
		if (match.Success)
		{
			var season = Enum.Parse<Season>(match.Groups[1].Value);
			var year = int.Parse(match.Groups[2].Value);
			return New(year, season, 1);
		}

		match = YearRegex.Match(text);
		if (match.Success)
		{
			var year = int.Parse(match.Groups[1].Value);
			return New(year, Season.Spring, 1);
		}

		return null;
	}

	public static string ToParsable(this WorldDate self)
		=> $"{Enum.GetName(self.Season)!} {self.DayOfMonth} Year {self.Year}";
}