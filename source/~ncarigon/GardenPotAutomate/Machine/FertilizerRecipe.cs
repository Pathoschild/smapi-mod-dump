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
    /// <summary>
    /// A general recipe for applying ferilizer
    /// </summary>
    internal class FertilizerRecipe : IRecipe {
        public Func<Item, bool> Input { get; }
        public int InputCount { get; } = 1;
        public Func<Item, Item> Output { get; } = _ => new SObject();
        public Func<Item, int> Minutes { get; } = _ => 0;

        private readonly IndoorPot IndoorPot;
        private readonly GameLocation Location;
        private readonly ModConfig Config;

        public FertilizerRecipe(IndoorPot indoorPot, GameLocation location, ModConfig config) {
            this.IndoorPot = indoorPot;
            this.Config = config;
            this.Location = location;
            this.Input = this.TryApply;
        }

        public bool AcceptsInput(ITrackedStack stack) {
            return stack.Type == ItemType.Object && Input(stack.Sample);
        }

        public bool TryApply(Item item) => TryApply(this.IndoorPot, item, this.Location, this.Config);

        public static bool TryApply(IndoorPot indoorPot, Item item, GameLocation location, ModConfig config) {
            return
                // are we enabled?
                config.Enabled
                // can we apply fertilizer?
                && config.ApplyFertilizers
                // is it a fertilizer?
                && (item?.Category.Equals(SObject.fertilizerCategory) ?? false)
                // actually try to place the item
                && (indoorPot?.performObjectDropInAction(item, false, new Farmer() { currentLocation = location }) ?? false);
        }
    }
}