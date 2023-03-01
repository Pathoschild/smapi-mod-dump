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
    /// A general recipe for planting seeds
    /// </summary>
    internal class SeedRecipe : IRecipe {
        public Func<Item, bool> Input { get; }
        public int InputCount { get; } = 1;
        public Func<Item, Item> Output { get; } = _ => new SObject();
        public Func<Item, int> Minutes { get; } = _ => 0;

        private readonly IndoorPot IndoorPot;
        private readonly GameLocation Location;
        private readonly ModConfig Config;

        public SeedRecipe(IndoorPot indoorPot, GameLocation location, ModConfig config) {
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
                // can we plant seeds?
                && config.PlantSeeds
                // is it a seed?
                && (item?.Category.Equals(SObject.SeedsCategory) ?? false)
                // actually try to place the item
                && (indoorPot?.performObjectDropInAction(item, false, new Farmer() { currentLocation = location }) ?? false);
        }
    }
}