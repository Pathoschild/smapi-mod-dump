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
using HarmonyLib;
using Pathoschild.Stardew.Automate;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using SObject = StardewValley.Object;

namespace GardenPotAutomate {
    /// <summary>
    /// A general recipe for using a watering can
    /// </summary>
    internal class WateringCanRecipe : IRecipe {
        public Func<Item, bool> Input { get; }
        public int InputCount { get; } = 1;
        public Func<Item, Item> Output { get; } = _ => new SObject();
        public Func<Item, int> Minutes { get; } = _ => 0;

        private readonly IndoorPot IndoorPot;
        private readonly GameLocation Location;
        private readonly ModConfig Config;

        public WateringCanRecipe(IndoorPot indoorPot, GameLocation location, ModConfig config) {
            this.IndoorPot = indoorPot;
            this.Config = config;
            this.Location = location;
            this.Input = this.TryAction;
        }

        public bool AcceptsInput(ITrackedStack stack) {
            return stack.Type == ItemType.Tool
                && TryGetWateringCan(stack, out var can)
                && Input(can);
        }

        public bool TryAction(Item item) => TryAction(this.IndoorPot, item, this.Location, this.Config);

        public static bool TryAction(IndoorPot indoorPot, Item item, GameLocation location, ModConfig config) {
            if (
                // are we enabled?
                config.Enabled
                // can we use watering cans?
                && config.UseWateringCan
                // and is it a watering can
                && item is WateringCan can
                // we can only water crops, not bushes
                && (indoorPot?.hoeDirt?.Value?.state ?? 1) != 1
            ) {
                var farmer = Game1.getFarmer(indoorPot!.owner.Value);
                // can has water or is enchanted
                if ((can?.WaterLeft ?? 0) > 0 || (can?.isBottomless ?? false) || farmer.hasWateringCanEnchantment) {
                    // actually try to water
                    indoorPot.performToolAction(can, location);
                    // did it work?
                    if ((indoorPot?.hoeDirt?.Value?.state ?? 0) == 1) {
                        if ((can?.WaterLeft ?? 0) > 0 && !(can?.isBottomless ?? false)) {
                            // reduce watering can on each use
                            can!.WaterLeft -= 1;
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Automate doesn't seem to allow access to the underlying Item from a consumable, only a copy of it, so we access the private field
        /// </summary>
        private static bool TryGetWateringCan(ITrackedStack stack, out WateringCan can) {
            can = null!;
            try {
                if (stack is TrackedItem item
                    && AccessTools.FieldRefAccess<TrackedItem, Item>(item, "Item") is WateringCan wc
                    && ((wc?.WaterLeft ?? 0) > 0 || (wc?.isBottomless ?? false))
                ) {
                    can = wc!;
                    return true;
                }
            } catch { }
            return false;
        }
    }
}