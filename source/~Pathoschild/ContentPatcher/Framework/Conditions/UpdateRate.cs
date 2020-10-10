/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

namespace ContentPatcher.Framework.Conditions
{
    /// <summary>Indicates when a patch should be updated.</summary>
    /// <remarks>Used as bit flags in <see cref="ContextUpdateType"/>.</remarks>
    internal enum UpdateRate
    {
        /// <summary>The patch updates once at the start of each day.</summary>
        OnDayStart = 1,

        /// <summary>The patch updates each time the current player changes location, and at the start of each day.</summary>
        OnLocationChange = 2
    }
}
