/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/futroo/Stardew-Valley-Mods
**
*************************************************/

using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace ShippingBinSummary
{
    internal record DataModel
    {
        public HashSet<int> ForceSellable { get; }

    public DataModel(HashSet<int>? forceSellable)
    {
        this.ForceSellable = forceSellable ?? new();
    }
}
}
