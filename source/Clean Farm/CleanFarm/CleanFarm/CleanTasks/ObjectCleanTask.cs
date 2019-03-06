using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;
using SDVObject = StardewValley.Object;
using Microsoft.Xna.Framework;

namespace CleanFarm.CleanTasks
{
    /// <summary>CleanTask for cleaning Objects on the farm (stones, twigs, weeds).</summary>
    class ObjectCleanTask : CleanTask<SDVObject>
    {
        /// <summary>The names of the objects that should be removed, populated from config settings.</summary>
        private readonly IList<string> ObjectsToRemove;

        /// <summary>Creats an instance of the clean task.</summary>
        /// <param name="config">The config object for this mod.</param>
        public ObjectCleanTask(ModConfig config)
            : base(config)
        {
            // Build a list of items to look for.
            this.ObjectsToRemove = new List<string>();
            if (config.RemoveStones) this.ObjectsToRemove.Add("stone");
            if (config.RemoveTwigs)  this.ObjectsToRemove.Add("twig");
            if (config.RemoveWeeds)  this.ObjectsToRemove.Add("weeds");
        }

        /// <summary>Can this task be run. Usually checks the config settings to see if it's enabled.</summary>
        public override bool CanRun()
        {
            return this.ObjectsToRemove.Count > 0;
        }

        /// <summary>Runs the clean task.</summary>
        /// <param name="farm">The farm to be cleaned.</param>
        public override void Run(Farm farm)
        {
            RemoveAndRecordItems(
                farm.objects.Pairs,
                pair => pair.Value,
                pair => farm.objects.Remove(pair.Key));
        }

        /// <summary>Restores all removed items for debug purposes.</summary>
        /// <param name="farm">The farm to restore the items to.</param>
        /// <returns>The number of items that were restored.</returns>
        public override int RestoreRemovedItems(Farm farm)
        {
            return RestoreItems(
                farm.objects.Pairs,
                pair => !farm.objects.ContainsKey(pair.Key),
                pair => farm.objects.Add(pair.Key, pair.Value));
        }

        /// <summary>Gets the human readable name of an item. Used for reporting the item.</summary>
        /// <param name="item">The item whose name to get.</param>
        protected override string GetItemName(SDVObject item)
        {
            return item.name;
        }

        /// <summary>Checks if an item meets the criteria to be removed from the farm. This is passed to a 'Where' query in RemoveAndRecordItems.</summary>
        /// <param name="item">The item in question.</param>
        protected override bool ShouldRemoveItem(SDVObject item)
        {
            return this.ObjectsToRemove.Contains(item.name.ToLower());
        }
    }
}
