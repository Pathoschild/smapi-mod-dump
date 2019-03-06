using StardewValley;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;

namespace CleanFarm.CleanTasks
{
    /// <summary>CleanTask for cleaning ResourceClumps on the farm (Boulders, stumps, large logs).</summary>
    class ResourceClumpCleanTask : CleanTask<ResourceClump>
    {
        /// <summary>Names for the corresponding clump indices.</summary>
        private readonly IDictionary<int, string> ClumpNames;

        /// <summary>Creats an instance of the clean task.</summary>
        /// <param name="config">The config object for this mod.</param>
        public ResourceClumpCleanTask(ModConfig config)
            : base(config)
        {
            this.ClumpNames = new Dictionary<int, string>
            {
                { ResourceClump.boulderIndex, "Boulder" },
                { ResourceClump.hollowLogIndex, "Hollow Log" },
                { ResourceClump.stumpIndex, "Large Stump" },
                { ResourceClump.meteoriteIndex, "Meteorite" },
            };
        }

        /// <summary>Can this task be run. Usually checks the config settings to see if it's enabled.</summary>
        public override bool CanRun()
        {
            return (this.Config.RemoveLargeLogs || this.Config.RemoveLargeRocks);
        }

        /// <summary>Runs the clean task.</summary>
        /// <param name="farm">The farm to be cleaned.</param>
        public override void Run(Farm farm)
        {
            RemoveAndRecordItems(farm.resourceClumps, item => item);
        }

        /// <summary>Restores all removed items for debug purposes.</summary>
        /// <param name="farm">The farm to restore the items to.</param>
        /// <returns>The number of items that were restored.</returns>
        public override int RestoreRemovedItems(Farm farm)
        {
            return RestoreItems(farm.resourceClumps);
        }

        /// <summary>Gets the human readable name of an item. Used for reporting the item.</summary>
        /// <param name="item">The item whose name to get.</param>
        protected override string GetItemName(ResourceClump item)
        {
            if (this.ClumpNames.ContainsKey(item.parentSheetIndex.Value))
            {
                return this.ClumpNames[item.parentSheetIndex.Value];
            }
            return item.GetType().ToString();
        }

        /// <summary>Checks if an item meets the criteria to be removed from the farm. This is passed to a 'Where' query in RemoveAndRecordItems.</summary>
        /// <param name="item">The item in question.</param>
        protected override bool ShouldRemoveItem(ResourceClump item)
        {
            int type = item.parentSheetIndex.Value;
            return ((this.Config.RemoveLargeRocks && type == ResourceClump.boulderIndex) ||
                     this.Config.RemoveLargeLogs && (type == ResourceClump.hollowLogIndex || type == ResourceClump.stumpIndex));
        }
    }
}
