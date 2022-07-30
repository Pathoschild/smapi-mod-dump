/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

namespace ContentPatcher.Framework.Constants
{
    /// <summary>Which delimited values should be removed or replaced by a text operation.</summary>
    internal enum TextOperationReplaceMode
    {
        /// <summary>Remove the first value which matches the search.</summary>
        First,

        /// <summary>Remove the last value which matches the search.</summary>
        Last,

        /// <summary>Remove all values which match the search.</summary>
        All
    }
}
