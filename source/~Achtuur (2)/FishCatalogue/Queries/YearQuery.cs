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
internal class YearQuery : IQuery
{
    public Texture2D Icon => throw new NotImplementedException();

    private static Regex ParseRegex = new(@"^YEAR\s+(?<min_year>\d+)\s+(?<max_year>\d+)");
    private string query;
    private int min_year;
    private int max_year;

    public static bool IsValidQuery(string query)
    {
        return ParseRegex.IsMatch(query);
    }

    public YearQuery(string query)
    {
        this.query = query;
        Parse();
    }

    private void Parse()
    {
        Match match = ParseRegex.Match(query);
        if (!match.Success)
            throw new ArgumentException("Invalid query format");
        
        min_year = int.Parse(match.Groups["min_year"].Value);
        // default to unlimited
        max_year = int.MaxValue; 
        if (match.Groups["max_year"].Value is string max_year_str)
            max_year = int.Parse(max_year_str);
    }

    public string Description()
    {
        return $"Year: {min_year} - {max_year}";
    }

    public bool IsTrue()
    {
        return Game1.year >= min_year && Game1.year <= max_year;
    }
}
