/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ncarigon/StardewValleyMods
**
*************************************************/

using System;
using Pathoschild.Stardew.Automate;
using StardewValley;
using StardewValley.Objects;
using SObject = StardewValley.Object;

namespace GardenPotAutomate {
    internal class FertilizerRecipe : IRecipe {
        public Func<Item, bool> Input { get; }
        public int InputCount { get; } = 1;
        public Func<Item, Item> Output { get; } = _ => new SObject();
        public Func<Item, int> Minutes { get; } = _ => 0;

        private readonly IndoorPot IndoorPot;
        private readonly Config Config;
        private readonly Farmer TempFarmer;

        public FertilizerRecipe(IndoorPot indoorPot, Farmer tempFarmer, Config config) {
            this.IndoorPot = indoorPot;
            this.Config = config;
            this.Input = this.TryApply;
            this.TempFarmer = tempFarmer;
        }

        public bool AcceptsInput(ITrackedStack stack) {
            return stack.Type == ItemRegistry.type_object && Input(stack.Sample);
        }

        public bool TryApply(Item item) {
            return
                // are we enabled?
                this.Config.Enabled
                // can we apply fertilizer?
                && this.Config.ApplyFertilizers
                // is it a fertilizer?
                && (item?.Category.Equals(SObject.fertilizerCategory) ?? false)
                // check if fertilizer is possible
                && (this.IndoorPot?.hoeDirt?.Value.CanApplyFertilizer(item?.QualifiedItemId) ?? false)
                // actually try to place the item
                && (this.IndoorPot?.performObjectDropInAction(item, false, this.TempFarmer) ?? false);
        }
    }
}