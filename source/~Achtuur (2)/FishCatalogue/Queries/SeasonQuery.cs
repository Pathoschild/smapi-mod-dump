/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FishCatalogue.Queries;

//LOCATION_SEASON Here summer winter
internal class SeasonQuery : IQuery
{
    public Texture2D Icon => throw new NotImplementedException();

    private static Regex ParseRegex = new(@"^SEASON\s+(?<location>\w+)\s+(?<season>(\w+\s?)+)");
    private static Regex ParseRegexLocation = new(@"^LOCATION_SEASON\s+(?<location>\w+)\s+(?<season>(\w+\s?)+)");
    private string query;
    private List<Season> seasons;
    public SeasonQuery(string query)
    {
        this.query = query;
        this.seasons = new();
        Parse();
    }

    public static bool IsValidQuery(string query)
    {
        return ParseRegex.IsMatch(query) || ParseRegexLocation.IsMatch(query);
    }

    private void Parse()
    {
        Match match = ParseRegex.Match(query);
        Match location_match = ParseRegexLocation.Match(query);
        if (!location_match.Success && !match.Success)
            throw new ArgumentException("Invalid query format");

        if (location_match.Groups["location"].Value != "Here")
            throw new ArgumentException("Location not 'Here'");

        this.seasons = SeasonsFromMatch(match)
            .Union(SeasonsFromMatch(location_match))
            .ToList();
    }

    private IEnumerable<Season> SeasonsFromMatch(Match reg_match)
    {
        return reg_match.Groups["season"].Value
            .Split(' ')
            .Select(s => string_to_season(s))
            .OfType<Season>();
    }


    public string Description()
    {
        StringBuilder sb = new();
        foreach(Season s in seasons)
        {
            sb.AppendLine($"Season: {season_to_string(s)}");
        }
        return sb.ToString();
    }

    public bool IsTrue()
    {
        return seasons.Contains(Game1.season);
    }

    private Season? string_to_season(string s)
    {
        switch (s)
        {
            case "spring":
                return Season.Spring;
            case "summer":
                return Season.Summer;
            case "fall":
                return Season.Fall;
            case "winter":
                return Season.Winter;
            default:
                return null;
        }
    }

    private string season_to_string(Season s)
    {
        switch (s)
        {
            case Season.Spring:
                return "Spring";
            case Season.Summer:
                return "Summer";
            case Season.Fall:
                return "Fall";
            case Season.Winter:
                return "Winter";
            default:
                return null;
        }
    }
}
