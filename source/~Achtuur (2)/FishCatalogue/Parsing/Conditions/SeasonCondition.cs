/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using AchtuurCore.Framework.Borders;
using StardewValley;
using StardewValley.ItemTypeDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishCatalogue.Parsing.Conditions;
internal class SeasonCondition : BaseCondition
{
    List<Season> seasons;

    public SeasonCondition(IEnumerable<Season> seasons)
    {
        this.seasons = seasons.ToList();
    }

    public override string Description()
    {
        if (seasons.Count == 4)
            return "Any";
        IEnumerable<string> time_strings = seasons.Select(season => SeasonToText(season));
        return string.Join("\n", time_strings);
    }

    protected override string ItemID()
    {
        return "22";
    }

    public override bool IsTrue()
    {
        return seasons.Contains(Game1.season);

    }

    private string SeasonToText(Season season)
    {
        switch (season)
        {
            case Season.Spring: return "Spring";
            case Season.Summer: return "Summer";
            case Season.Fall: return "Fall";
            case Season.Winter: return "Winter";
            default: return "WTF SEASON???";
        }
    }
}
