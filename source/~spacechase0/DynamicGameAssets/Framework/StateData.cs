/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;

namespace DynamicGameAssets.Framework
{
    internal class StateData
    {
        public Dictionary<string, List<ShopEntry>> TodaysShopEntries { get; set; } = new();

        public int AnimationFrames { get; set; } = 0;
    }
}
