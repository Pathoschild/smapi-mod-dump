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
    internal class WateringCanRecipe : IRecipe {
        public Func<Item, bool> Input { get; }
        public int InputCount { get; } = 1;
        public Func<Item, Item> Output { get; } = _ => new SObject();
        public Func<Item, int> Minutes { get; } = _ => 0;

        private readonly IndoorPot IndoorPot;
        private readonly GameLocation Location;
        private readonly Config Config;

        public WateringCanRecipe(IndoorPot indoorPot, GameLocation location, Config config) {
            this.IndoorPot = indoorPot;
            this.Config = config;
            this.Location = location;
            this.Input = this.TryAction;
        }

        public bool AcceptsInput(ITrackedStack stack) {
            return stack.Type == ItemRegistry.type_tool
                && TryGetWateringCan(stack, out var can)
                && Input(can);
        }

        public bool TryAction(Item item) {
            if (
                // are we enabled?
                this.Config.Enabled
                // can we use watering cans?
                && this.Config.UseWateringCan
                // and is it a watering can
                && item is WateringCan can
                // we can only water crops, not bushes
                && (this.IndoorPot?.hoeDirt?.Value?.state?.Value ?? 1) != 1
            ) {
                var farmer = Game1.getFarmer(this.IndoorPot!.owner.Value);
                // can has water or is enchanted
                if ((can?.WaterLeft ?? 0) > 0 || (can?.isBottomless?.Value ?? false) || farmer.hasWateringCanEnchantment) {
                    // actually try to water
                    this.IndoorPot.performToolAction(can);
                    // did it work?
                    if ((this.IndoorPot?.hoeDirt?.Value?.state?.Value ?? 0) == 1) {
                        if ((can?.WaterLeft ?? 0) > 0 && !(can?.isBottomless?.Value ?? false)) {
                            // reduce watering can on each use
                            can!.WaterLeft -= 1;
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool TryGetWateringCan(ITrackedStack stack, out WateringCan can) {
            // Automate doesn't seem to allow access to the underlying Item from a consumable, only a copy of it, so we access the private field
            can = null!;
            try {
                if (stack is TrackedItem item
                    && AccessTools.FieldRefAccess<TrackedItem, Item>(item, "Item") is WateringCan wc
                    && ((wc?.WaterLeft ?? 0) > 0 || (wc?.isBottomless?.Value ?? false))
                ) {
                    can = wc!;
                    // bug fix to help when some mods spawn in fake upgraded watering cans which don't have their max level set correctly
                    var max = 0;
                    switch (can.UpgradeLevel) {
                        case 0 when can.waterCanMax != 40:
                            max = 40;
                            break;
                        case 1 when can.waterCanMax != 55:
                            max = 55;
                            break;
                        case 2 when can.waterCanMax != 70:
                            max = 70;
                            break;
                        case 3 when can.waterCanMax != 85:
                            max = 85;
                            break;
                        case 4 when can.waterCanMax != 100:
                            max = 100;
                            break;
                    }
                    if (max != 0) {
                        var lvl = can.WaterLeft / (float)can.waterCanMax;
                        can.waterCanMax = max;
                        can.WaterLeft = (int)(max * lvl);
                    }
                    return true;
                }
            } catch { }
            return false;
        }
    }
}