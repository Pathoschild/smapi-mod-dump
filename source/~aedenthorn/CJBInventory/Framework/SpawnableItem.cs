/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using CJBInventory.Framework.ItemData;

namespace CJBInventory.Framework
{
    /// <summary>A game item with metadata for the spawn menu.</summary>
    internal class SpawnableItem : SearchableItem
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The item's category filter label for the spawn menu.</summary>
        public string Category { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="item">The item metadata.</param>
        /// <param name="category">The item's category filter label for the spawn menu.</param>
        public SpawnableItem(SearchableItem item, string category)
            : base(item)
        {
            this.Category = category;
        }
    }
}
