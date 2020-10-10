/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using StardewTree = StardewValley.TerrainFeatures.Tree;

namespace Pathoschild.Stardew.LookupAnything.Framework.Constants
{
    /// <summary>Indicates a tree's growth stage.</summary>
    internal enum WildTreeGrowthStage
    {
        Seed = StardewTree.seedStage,
        Sprout = StardewTree.sproutStage,
        Sapling = StardewTree.saplingStage,
        Bush = StardewTree.bushStage,
        SmallTree = StardewTree.treeStage - 1, // an intermediate stage between bush and tree, no constant
        Tree = StardewTree.treeStage
    }
}
