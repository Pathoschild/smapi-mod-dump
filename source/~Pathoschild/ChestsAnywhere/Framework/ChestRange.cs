/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

namespace Pathoschild.Stardew.ChestsAnywhere.Framework
{
    /// <summary>A range at which chests should be accessible.</summary>
    internal enum ChestRange
    {
        /// <summary>All chests.</summary>
        Unlimited,

        /// <summary>Chests within the current world area.</summary>
        CurrentWorldArea,

        /// <summary>Chests within the current location.</summary>
        CurrentLocation,

        /// <summary>Don't allow remote access to any chest.</summary>
        None
    }
}
