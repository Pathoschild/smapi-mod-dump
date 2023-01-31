/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraBase.Toolkit.StringHandler;

using CommunityToolkit.Diagnostics;

namespace AtraShared.Utils.Extensions;

public static class ItemExtensions
{
    public static bool MatchesTagList(this Item item, string tagList)
    {
        Guard.IsNotNull(item);
        Guard.IsNotNull(tagList);

        foreach (SpanSplitEntry tag in tagList.StreamSplit(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (!item.CheckOrTag(tag))
            {
                return false;
            }
        }

        return true;
    }

    private static bool CheckOrTag(this Item item, ReadOnlySpan<char> split)
    {
        foreach (SpanSplitEntry tag in split.StreamSplit('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (item.HasContextTag(tag))
            {
                return true;
            }
        }

        return false;
    }
}
