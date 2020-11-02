/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CJBok/SDV-Mods
**
*************************************************/

using CJBItemSpawner.Framework.ItemData;

namespace CJBItemSpawner.Framework.Models
{
    /// <summary>A unique item identifier.</summary>
    internal class ModItemDataEntry
    {
        /********
        ** Accessors
        ********/
        /// <summary>The item type.</summary>
        public ItemType Type { get; set; }

        /// <summary>The unique item ID for the type.</summary>
        public int ID { get; set; }
    }
}
