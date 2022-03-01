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

namespace CJBInventory.Framework.Models
{
    /// <summary>An item category filter rule. See the format documentation in the <c>data.json</c> file.</summary>
    internal class ModDataCategory
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The translation key or literal text for the category display name.</summary>
        public string Label { get; set; }

        /// <summary>The rules for items which match this category.</summary>
        public ModDataCategoryRule When { get; set; }

        /// <summary>The rules for items to ignore.</summary>
        public ModDataCategoryRule Except { get; set; }


        /*********
        ** Public methods
        *********/
        /// <summary>Get whether a given item matches the rules for this category.</summary>
        /// <param name="item">The item to check.</param>
        public bool IsMatch(SearchableItem item)
        {
            return
                this.When != null
                && this.When.IsMatch(item)
                && this.Except?.IsMatch(item) != true;
        }
    }
}
