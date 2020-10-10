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
    /// <summary>The patch type.</summary>
    internal enum PatchType
    {
        /// <summary>Load the initial version of the file.</summary>
        Load,

        /// <summary>Edit an image.</summary>
        EditImage,

        /// <summary>Edit a data file.</summary>
        EditData,

        /// <summary>Edit a map after it's loaded.</summary>
        EditMap,

        /// <summary>Include patches from another JSON file.</summary>
        Include
    }
}
