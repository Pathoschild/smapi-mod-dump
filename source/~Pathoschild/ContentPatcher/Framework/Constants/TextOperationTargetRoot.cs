/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using ContentPatcher.Framework.Conditions;

namespace ContentPatcher.Framework.Constants
{
    /// <summary>An allowed root value for a text operation target.</summary>
    internal enum TextOperationTargetRoot
    {
        /// <summary>An entry for an <see cref="PatchType.EditData"/> patch.</summary>
        Entries,

        /// <summary>A field for an <see cref="PatchType.EditData"/> patch.</summary>
        Fields,

        /// <summary>A map property for an <see cref="PatchType.EditMap"/> patch.</summary>
        MapProperties
    }
}
