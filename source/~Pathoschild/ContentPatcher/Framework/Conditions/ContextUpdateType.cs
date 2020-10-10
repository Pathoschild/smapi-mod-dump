/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System;

namespace ContentPatcher.Framework.Conditions
{
    /// <summary>Indicates a context update type.</summary>
    [Flags]
    public enum ContextUpdateType
    {
        /// <summary>The current player changed location.</summary>
        OnLocationChange = UpdateRate.OnLocationChange,

        /// <summary>All update types.</summary>
        All = UpdateRate.OnDayStart | UpdateRate.OnLocationChange
    }
}
