/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

namespace Pathoschild.Stardew.LookupAnything.Framework.Data
{
    /// <summary>Metadata for a shop that isn't available from the game data directly.</summary>
    internal class ShopData
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The translation key for the shop name.</summary>
        public string DisplayKey { get; set; }

        /// <summary>The categories of items that the player can sell to this shop.</summary>
        public int[] BuysCategories { get; set; }
    }
}
