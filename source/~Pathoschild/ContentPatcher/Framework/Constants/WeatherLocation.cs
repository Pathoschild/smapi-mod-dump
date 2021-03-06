/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using StardewValley;

namespace ContentPatcher.Framework.Constants
{
    /// <summary>A general world area defined by the game.</summary>
    internal enum LocationContext
    {
        /// <summary>The Ginger Island areas.</summary>
        Island = GameLocation.LocationContext.Island,

        /// <summary>The valley (i.e. non-island) areas.</summary>
        Valley = GameLocation.LocationContext.Default
    }
}
