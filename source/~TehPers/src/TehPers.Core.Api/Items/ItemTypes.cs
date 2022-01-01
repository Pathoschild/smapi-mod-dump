/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using StardewValley;

namespace TehPers.Core.Api.Items
{
    /// <summary>
    /// Standard <see cref="Item"/> type names.
    /// </summary>
    public static class ItemTypes
    {
        /// <summary>
        /// The type name for craftable items. This is different than the type name for objects
        /// even though they use the same class.
        /// </summary>
        public const string BigCraftable = "BigCraftable";

        /// <summary>
        /// The type name for boots.
        /// </summary>
        public const string Boots = "Boots";

        /// <summary>
        /// The type name for clothing.
        /// </summary>
        public const string Clothing = "Clothing";

        /// <summary>
        /// The type name for flooring. This is different than the type name for wallpapers even
        /// though they use the same class.
        /// </summary>
        public const string Flooring = "Flooring";

        /// <summary>
        /// The type name for furniture.
        /// </summary>
        public const string Furniture = "Furniture";

        /// <summary>
        /// The type name for hats.
        /// </summary>
        public const string Hat = "Hat";

        /// <summary>
        /// The type name for objects.
        /// </summary>
        public const string Object = "Object";

        /// <summary>
        /// The type name for rings.
        /// </summary>
        public const string Ring = "Ring";

        /// <summary>
        /// The type name for tools.
        /// </summary>
        public const string Tool = "Tool";

        /// <summary>
        /// The type name for wallpapers. This is different than the type name for flooring even
        /// though they use the same class.
        /// </summary>
        public const string Wallpaper = "Wallpaper";

        /// <summary>
        /// The type name for weapons (both melee and sling shots).
        /// </summary>
        public const string Weapon = "Weapon";

        /// <summary>
        /// The type name for unknown item types.
        /// </summary>
        public const string Unknown = "Unknown";
    }
}