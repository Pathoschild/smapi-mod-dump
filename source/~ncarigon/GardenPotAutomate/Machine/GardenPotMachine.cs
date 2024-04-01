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
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace GardenPotAutomate {
    internal class GardenPotMachine : IMachine {
        private readonly IndoorPot Machine;
        private readonly Stack<Item> ItemDrops = new();
        private readonly Config Config;
        private readonly IRecipe[] SeedRecipes, FertilizerRecipes, WateringCanRecipes;
        private readonly Farmer FakeFarmer;

        public GameLocation Location { get; }
        public Rectangle TileArea { get; }
        public string MachineTypeID { get; } = "GardenPotAutomate/GardenPotMachine";

        public GardenPotMachine(Config config, IndoorPot indoorPot, GameLocation location, Vector2 indoorPotTile) {
            this.Config = config;
            this.Machine = indoorPot;
            this.Location = location;
            this.TileArea = new Rectangle((int)indoorPotTile.X, (int)indoorPotTile.Y, 1, 1);
            this.FakeFarmer = new Farmer() {
                currentLocation = location,
                // MARGO throws errors if this isn't set
                mostRecentlyGrabbedItem = new SObject()
            };
            this.SeedRecipes = new[] { new SeedRecipe(indoorPot, this.FakeFarmer, config) };
            this.FertilizerRecipes = new[] { new FertilizerRecipe(indoorPot, this.FakeFarmer, config) };
            this.WateringCanRecipes = new[] { new WateringCanRecipe(indoorPot, location, config) };
        }

        public ITrackedStack? GetOutput() {
            if (!this.Config.Enabled || !(this.Config.HarvestCrops || this.Config.HarvestFlowers))
                return null;

            if (!ItemDrops.Any()) {
                var items = this.Harvest();
                foreach (var item in items) {
                    ItemDrops.Push(item);
                }
            }
            return ItemDrops.Any()
                ? new TrackedItem(ItemDrops.Peek(), onReduced: OnOutputReduced)
                : null;
        }

        public MachineState GetState() {
            // clear our any remaining drops first
            if (ItemDrops.Any())
                return MachineState.Done;

            // ignore bushes
            if (Machine.bush.Value is not null)
                return MachineState.Disabled;

            // forage crop is ready
            if (Machine.heldObject.Value is not null)
                return MachineState.Done;

            var dirt = Machine.hoeDirt.Value;

            // normal crop is ready
            if (dirt?.readyForHarvest() ?? false)
                return MachineState.Done;

            // check if dead crop and clear
            if (dirt?.crop?.dead.Value ?? false)
                dirt.destroyCrop(false);

            if (// no crop, ready to plant
                dirt?.crop is null
                // no fertilizer, ready to fertilize
                || dirt?.fertilizer?.Value is null
                // not watered, ready to water
                || (dirt?.state?.Value ?? 0) != 1
            ) {
                return MachineState.Empty;
            }

            // must to growing
            return MachineState.Processing;
        }

        public bool SetInput(IStorage input) {
            if (!this.Config.Enabled || !(this.Config.PlantSeeds || this.Config.ApplyFertilizers || this.Config.UseWateringCan))
                return false;

            var dirt = Machine.hoeDirt?.Value;

            if (dirt?.crop is null
                && input.TryGetIngredient(this.SeedRecipes, out IConsumable? consumable, out IRecipe? recipe)
                && recipe is SeedRecipe
            ) {
                var seed = consumable.Take();
                if (seed is not null) {
                    seed.Stack--;
                    // don't return true here since other items may be needed
                }
            }
            if (dirt?.fertilizer?.Value is null
                && input.TryGetIngredient(this.FertilizerRecipes, out consumable, out recipe)
                && recipe is FertilizerRecipe
            ) {
                var seed = consumable.Take();
                if (seed is not null) {
                    seed.Stack--;
                    // don't return true here since other items may be needed
                }
            }
            if ((dirt?.state?.Value ?? 0) != 1
                && input.TryGetIngredient(this.WateringCanRecipes, out _, out recipe)
                && recipe is WateringCanRecipe
            ) {
                // water level reduction is handled in recipe
                // don't return true here since other items may be needed
            }

            // force sprite refresh
            this.Machine.actionOnPlayerEntry();

            // stop processing if all checks fail
            return !(dirt?.crop is null && dirt?.fertilizer?.Value is null && (dirt?.state?.Value ?? 0) != 1);
        }

        private void OnOutputReduced(Item item) {
            if (this.Machine.bush.Value is not null)
                return;

            if (ItemDrops.TryPop(out _)) {
                if (ItemDrops.Count == 0) {
                    HoeDirt? dirt = Machine.hoeDirt?.Value;
                    if (dirt?.crop is not null && !dirt.crop.RegrowsAfterHarvest())
                        dirt.destroyCrop(false);
                    Machine.heldObject.Value = null;
                }
            }
        }

        private Item[] Harvest() {
            var soil = this.Machine?.hoeDirt?.Value;
            var crop = soil?.crop;
            var held = this.Machine?.heldObject?.Value;

            if (held is null && (crop is null || crop.dead.Value))
                return Array.Empty<SObject>();

            if (!Config.HarvestFlowers
                && ((held is not null && held.Category == SObject.flowersCategory)
                || (crop is not null && new SObject(crop!.indexOfHarvest.Value, 1).Category == SObject.flowersCategory)
            )) {
                return Array.Empty<SObject>();
            }

            var xTile = (int)soil!.Tile.X;
            var yTile = (int)soil!.Tile.Y;
            var items = new List<Item>();

            // Added logic to handle wild seed crops since they are a "heldObject"
            if (held is not null) {
                // add a fake "Spring Onion", "(O)399" crop which we can detect and swap later
                crop = new Crop(true, "1", xTile, yTile, null);
            }
            if (crop is not null) {
                // tracking for patches
                Patches.Items = new Action<Item>(i => {
                    if (held is not null && i.ParentSheetIndex == 399) {
                        // check for and swap out the fake "Spring Onion" to handle held forage items
                        held.Quality = ((SObject)i).Quality;
                        held.Stack = ((SObject)i).Stack;
                        items.Add(held);
                    } else {
                        items.Add(i);
                    }
                });

                /* INFO: There seems to be a bug with the IndoorPot class
                 * where the 'owner' gets set to '0' somewhere between
                 * Utility.tryToPlaceItem() and new GardenPotMachine().
                 * This means the 'owner' is always invalid and defaults
                 * to the MasterPlayer. The result is that Experience and
                 * Professions will always reference the MasterPlayer even
                 * when a farmhand placed the IndoorPot down. If that bug
                 * ever gets fixed, this should accomodate it correctly.
                 * 
                 * REF: https://github.com/Pathoschild/StardewMods/tree/stable/Automate#in-multiplayer-who-gets-xp-and-whose-professions-apply
                */
                Patches.Owner = Game1.getFarmer(this.Machine!.owner.Value);

                // config fake farmer
                this.FakeFarmer.professions.Clear();
                // Config.GainExperience is handled in Patches
                if (Config.ApplyProfessions) {
                    foreach (var p in Patches.Owner.professions) {
                        this.FakeFarmer.professions.Add(p);
                    }
                    this.FakeFarmer.foragingLevel.Value = Patches.Owner.ForagingLevel;
                    this.FakeFarmer.foragingLevel.Value = Patches.Owner.FarmingLevel;
                } else {
                    this.FakeFarmer.foragingLevel.Value = 0;
                    this.FakeFarmer.farmingLevel.Value = 0;
                }
                // swap fake farmer in
                Patches.Harvester = this.FakeFarmer;

                // harvest crop
                crop.harvest(xTile, yTile, soil);

                // stop tracking
                Patches.Harvester = null!;
                Patches.Owner = null!;
                Patches.Items = null!;
            }
            return items.ToArray();
        }
    }
}