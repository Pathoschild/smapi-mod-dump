/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

namespace ContentPatcher.Framework.Patches
{
    /// <summary>A predefined position when moving entries.</summary>
    public enum MoveEntryPosition
    {
        /// <summary>Don't move the entry.</summary>
        None,

        /// <summary>Insert the entry at index 0.</summary>
        Top,

        /// <summary>Append the entry to the end of the list.</summary>
        Bottom
    }
}
