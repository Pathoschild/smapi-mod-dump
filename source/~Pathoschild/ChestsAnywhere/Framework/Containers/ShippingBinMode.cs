/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

namespace Pathoschild.Stardew.ChestsAnywhere.Framework.Containers
{
    /// <summary>The type of shipping bin menu to create.</summary>
    internal enum ShippingBinMode
    {
        /// <summary>The normal bidirectional shipping bin.</summary>
        Normal,

        /// <summary>A shipping bin menu for storing items on mobile.</summary>
        MobileStore,

        /// <summary>A shipping bin menu for retrieving items on mobile.</summary>
        MobileTake
    }
}
