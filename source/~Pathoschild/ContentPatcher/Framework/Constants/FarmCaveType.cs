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
    /// <summary>A farm cave type.</summary>
    internal enum FarmCaveType
    {
        /// <summary>The player hasn't chosen a farm cave yet.</summary>
        None = Farmer.caveNothing,

        /// <summary>The fruit bat cave.</summary>
        Bats = Farmer.caveBats,

        /// <summary>The mushroom cave.</summary>
        Mushrooms = Farmer.caveMushrooms
    }
}
