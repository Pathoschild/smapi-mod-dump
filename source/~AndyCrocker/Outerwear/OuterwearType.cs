/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

namespace Outerwear
{
    /// <summary>The types of outerwear.</summary>
    /// <remarks>This determines the spritesheet that the outerwear is based off as well as the layer depth the outerwear is drawn.</remarks>
    public enum OuterwearType
    {
        /// <summary>The outerwear is based off a shirt.</summary>
        Shirt,

        /// <summary>The outerwear is based off an accessory.</summary>
        Accessory,

        /// <summary>The outerwear is based off the hair.</summary>
        Hair,

        /// <summary>The outerwear is based off a hat.</summary>
        Hat,

        /// <summary>The outerwear is based off a pair of trousers.</summary>
        Pants
    }
}
