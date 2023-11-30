/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunityUpgradeFramework;

internal class CommunityUpgrade
{
    public static StringBuilder StringBuilder = new();

    public string Location;
    public string Name;
    public string Description;
    public Dictionary<int, int> ItemPriceDict;
    public Dictionary<string, int> CurrencyPriceDict;
    public string ThumbnailPath = "";
    public int DaysToBuild;

    public override string ToString()
    {
        StringBuilder.Clear();
        StringBuilder.AppendLine($"\n\t{Name}");
        StringBuilder.AppendLine($"\t\tLocation: {Location}");
        StringBuilder.AppendLine($"\t\tDescription: {Description}");
        StringBuilder.AppendLine($"\t\tItemPrices:");
        StringBuilder.AppendLine($"\t\t\t" + string.Join("\n\t\t\t",
            ItemPriceDict.Select(price => $"{price.Key}: {price.Value}")));
        StringBuilder.AppendLine($"\t\tCurrencyPrices:");
        StringBuilder.AppendLine($"\t\t\t" + string.Join("\n\t\t\t",
            CurrencyPriceDict.Select(price => $"{price.Key}: {price.Value}")));
        StringBuilder.AppendLine($"\t\tThumbnailPath: {ThumbnailPath}");
        StringBuilder.AppendLine($"\t\tDaysToBuild: {DaysToBuild}");

        return StringBuilder.ToString();
    }
}
