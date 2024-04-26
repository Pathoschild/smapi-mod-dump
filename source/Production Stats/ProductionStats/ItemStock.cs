/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FlameHorizon/ProductionStats
**
*************************************************/

using StardewValley;

namespace ProductionStats;

public class ItemStock(Item item)
{
    public Item Item { get; set; } = item;
    public int Count { get; set; } = 0;
}
