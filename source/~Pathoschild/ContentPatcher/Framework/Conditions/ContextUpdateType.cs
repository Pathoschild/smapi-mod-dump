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
    /// <summary>A context update type.</summary>
    public enum ContextUpdateType
    {
        /// <summary>All patches should be updated.</summary>
        All = -1,

        /// <summary>The in-game clock changed.</summary>
        OnTimeChange = UpdateRate.OnTimeChange,

        /// <summary>The current player changed location.</summary>
        OnLocationChange = UpdateRate.OnLocationChange
    }
}
