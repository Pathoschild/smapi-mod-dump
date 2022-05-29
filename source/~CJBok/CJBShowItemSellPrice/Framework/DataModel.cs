/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CJBok/SDV-Mods
**
*************************************************/

using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace CJBShowItemSellPrice.Framework
{
    /// <summary>Metadata that isn't available from the game data directly.</summary>
    internal record DataModel
    {
        /*********
        ** Accessors
        *********/
        /// <summary>Item categories that can be sold in shops, regardless of what <see cref="SObject.canBeShipped"/> returns.</summary>
        public HashSet<int> ForceSellable { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="forceSellable">Item categories that can be sold in shops, regardless of what <see cref="SObject.canBeShipped"/> returns.</param>
        public DataModel(HashSet<int>? forceSellable)
        {
            this.ForceSellable = forceSellable ?? new();
        }
    }
}
