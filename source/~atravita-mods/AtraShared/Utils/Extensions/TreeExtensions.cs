/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using StardewValley.TerrainFeatures;

namespace AtraShared.Utils.Extensions;

/// <summary>
/// Extensions on trees.
/// </summary>
public static class TreeExtensions
{
    /// <summary>
    /// Checks to see if a tree is a palm tree.
    /// </summary>
    /// <param name="tree">Tree to check.</param>
    /// <returns>True if palm tree, false otherwise.</returns>
    public static bool IsPalmTree(this Tree? tree)
        => tree is not null && (tree.treeType.Value == Tree.palmTree || tree.treeType.Value == Tree.palmTree2);
}