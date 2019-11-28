using System;
using System.Collections.Generic;
using System.Text;
using StardewValley.TerrainFeatures;

namespace Common.Objects
{
    internal class Trees
    {


        internal enum TreeType
        {
            Oak = Tree.bushyTree,
            Maple = Tree.leafyTree,
            Pine = Tree.pineTree,
            Palm = Tree.palmTree,
            BigMushroom = Tree.mushroomTree
        }

        internal enum TreeStage
        {
            Seed = StardewValley.TerrainFeatures.Tree.seedStage,
            Sprout = StardewValley.TerrainFeatures.Tree.sproutStage,
            Sapling = StardewValley.TerrainFeatures.Tree.saplingStage,
            Bush = StardewValley.TerrainFeatures.Tree.bushStage,
            SmallTree = StardewValley.TerrainFeatures.Tree.treeStage - 1, // an intermediate stage between bush and tree, no constant
            Tree = StardewValley.TerrainFeatures.Tree.treeStage
        }
    }
}
