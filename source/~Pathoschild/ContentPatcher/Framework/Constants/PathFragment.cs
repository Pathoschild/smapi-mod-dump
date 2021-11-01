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
    /// <summary>A fragment which can be extracted from a path.</summary>
    internal enum PathFragment
    {
        /// <summary>The directory portion of the path, without the file name.</summary>
        DirectoryPath,

        /// <summary>The file name portion of the path (including the extension, if any).</summary>
        FileName,

        /// <summary>The file name portion of the path (excluding the extension).</summary>
        FilenameWithoutExtension
    }
}
