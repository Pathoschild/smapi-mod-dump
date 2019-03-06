using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using System.Linq;

namespace CleanFarm.CleanTasks
{
    /// <summary>CleanTask for cleaning TerrainFeatures from the farm (saplings, grass).</summary>
    class TerrainFeatureCleanTask : CleanTask<TerrainFeature>
    {
        /// <summary>The max growth stage of trees to allow. All trees below this stage are removed.</summary>
        private readonly int MaxGrowthStage = Tree.saplingStage;

        /// <summary>The names of the growth stages indexed by the growth stage. Used to lookup the tree name.</summary>
        private readonly IList<string> GrowthStageNames;

        /// <summary>Creats an instance of the clean task.</summary>
        /// <param name="config">The config object for this mod.</param>
        public TerrainFeatureCleanTask(ModConfig config)
            : base(config)
        {
            this.MaxGrowthStage = (int)MathHelper.Clamp(config.MaxTreeGrowthStageToAllow, Tree.sproutStage, Tree.treeStage);

            this.GrowthStageNames = new List<string>() { "Tree Seed", "Tree Sprout", "Tree Sapling", "Tree Bush", "Small Tree" };
        }

        /// <summary>Can this task be run. Usually checks the config settings to see if it's enabled.</summary>
        public override bool CanRun()
        {
            return this.Config.RemoveSaplings || this.Config.RemoveGrass || this.Config.RemoveStumps;
        }

        /// <summary>Runs the clean task.</summary>
        /// <param name="farm">The farm to be cleaned.</param>
        public override void Run(Farm farm)
        {
            RemoveAndRecordItems(
                farm.terrainFeatures.Pairs,
                pair => pair.Value,
                pair => farm.terrainFeatures.Remove(pair.Key));
        }

        /// <summary>Restores all removed items for debug purposes.</summary>
        /// <param name="farm">The farm to restore the items to.</param>
        /// <returns>The number of items that were restored.</returns>
        public override int RestoreRemovedItems(Farm farm)
        {
            return RestoreItems(
                farm.terrainFeatures.Pairs,
                pair => !farm.terrainFeatures.ContainsKey(pair.Key),
                pair => farm.terrainFeatures.Add(pair.Key, pair.Value));
        }

        /// <summary>Gets the human readable name of an item. Used for reporting the item.</summary>
        /// <param name="item">The item whose name to get.</param>
        protected override string GetItemName(TerrainFeature item)
        {
            if (item is Tree)
            {
                var tree = item as Tree;
                if (tree.stump.Value)
                    return "Tree Stump";

                // Use the name of the growth stage if valid.
                return tree.growthStage.Value >= 0 && tree.growthStage.Value < this.GrowthStageNames.Count 
                    ? this.GrowthStageNames[tree.growthStage.Value] 
                    : "Tree";
            }

            if (item is Grass)
                return "Grass";

            // Default
            return item.ToString();
        }

        /// <summary>Checks if an item meets the criteria to be removed from the farm. This is passed to a 'Where' query in RemoveAndRecordItems.</summary>
        /// <param name="item">The item in question.</param>
        protected override bool ShouldRemoveItem(TerrainFeature item)
        {
            if (item is Tree)
            {
                var tree = item as Tree;
                return ((this.Config.RemoveSaplings && tree.growthStage.Value < this.MaxGrowthStage) ||
                        (this.Config.RemoveStumps && tree.stump.Value));
            }
            return (item is Grass && this.Config.RemoveGrass);
        }
    }
}
