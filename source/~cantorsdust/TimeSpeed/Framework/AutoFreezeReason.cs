/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cantorsdust/StardewMods
**
*************************************************/

namespace TimeSpeed.Framework
{
    /// <summary>The reasons for automated time freezes.</summary>
    internal enum AutoFreezeReason
    {
        /// <summary>No freeze currently applies.</summary>
        None,

        /// <summary>Time was automatically frozen based on the location per <see cref="ModConfig.TickLengthByLocation"/>.</summary>
        FrozenForLocation,

        /// <summary>Time was automatically frozen per <see cref="ModConfig.FreezeTimeAt"/>.</summary>
        FrozenAtTime
    }
}
