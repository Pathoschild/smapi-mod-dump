/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

namespace MoreGrass;

/// <summary>Contains modData key constants.</summary>
public class ModDataConstants
{
    /*********
    ** Constants
    *********/
    /// <summary>The modData for storing the x offset (in the atlas) for grass.</summary>
    /// <remarks>This is the base for offsets, meaning 0 - 3 will be appended to the end (as grass can have up to 4 sprites per object).</remarks>
    public const string GrassOffsetXBase = $"Sonozuki.MoreGrass/GrassOffsetX";

    /// <summary>The modData for storing the y offset (in the atlas) for grass.</summary>
    /// <remarks>This is the base for offsets, meaning 0 - 3 will be appended to the end (as grass can have up to 4 sprites per object).</remarks>
    public const string GrassOffsetYBase = $"Sonozuki.MoreGrass/GrassOffsetY";
}
