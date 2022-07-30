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
    /// <summary>A text operation type.</summary>
    internal enum TextOperationType
    {
        /// <summary>Append text after the target value.</summary>
        Append,

        /// <summary>Prepend text before the target value.</summary>
        Prepend,

        /// <summary>Parse the target text into a list of delimited values, and remove the values matching the search.</summary>
        RemoveDelimited
    }
}
