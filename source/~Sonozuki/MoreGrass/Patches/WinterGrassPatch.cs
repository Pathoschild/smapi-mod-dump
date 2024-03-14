/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

namespace MoreGrass.Patches;

/// <summary>Contains patches for patching code in the WinterGrass mod.</summary>
internal class WinterGrassPatch
{
    /*********
    ** Internal Methods
    *********/
    /// <summary>The prefix for the WinterGrass.Mod.FixGrassColor() method.</summary>
    /// <returns><see langword="false"/>, meaning the original method will not get ran.</returns>
    /// <remarks>This is used to disable the WinterGrass mod so this mod can handle the textures properly.</remarks>
    internal static bool FixGrassColorPrefix() => false;
}
