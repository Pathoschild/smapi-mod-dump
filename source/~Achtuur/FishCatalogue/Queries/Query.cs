/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishCatalogue.Queries;

static class QueryFactory
{
    public static IQuery CreateQuery(string query)
    {
        if (SeasonQuery.IsValidQuery(query))
            return new SeasonQuery(query);
        if (TimeQuery.IsValidQuery(query))
            return new TimeQuery(query);
        if (WeatherQuery.IsValidQuery(query))
            return new WeatherQuery(query);
        if (YearQuery.IsValidQuery(query))
            return new YearQuery(query);
        return null;
    }
}

internal interface IQuery
{
    public Texture2D Icon { get; }

    /// <summary>
    /// Returns true if <c>query</c> is valid query for this type of query
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public static bool IsValidQuery(string query)
    {
        return false;
    }
    public bool IsTrue();
    public string Description();
}
