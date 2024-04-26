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


//TIME 0600 1800
internal class TimeQuery : IQuery
{
    public Texture2D Icon => throw new NotImplementedException();
    private static Regex ParseRegex = new(@"^TIME\s+(?<start_time>\d+)(\s(?<end_time>\d+))?");
    private string query;
    private int start_time;
    private int end_time;

    public static bool IsValidQuery(string query)
    {
        return ParseRegex.IsMatch(query);
    }

    public TimeQuery(string query)
    {
        this.query = query;
        Parse();
    }

    
    private void Parse()
    {
        Match match = ParseRegex.Match(query);
        if (!match.Success)
            throw new ArgumentException("Invalid query format");

        start_time = int.Parse(match.Groups["start_time"].Value);
        end_time = int.Parse(match.Groups["end_time"].Value);
    }

    public string Description()
    {
        throw new NotImplementedException();
    }

    public bool IsTrue()
    {
        return Game1.timeOfDay >= start_time && Game1.timeOfDay <= end_time;
    }
}
