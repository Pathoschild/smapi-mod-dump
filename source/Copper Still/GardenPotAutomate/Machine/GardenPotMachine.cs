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
using System.Xml.Linq;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace GardenPotAutomate {
    /// <summary>
    /// An automation machine to handle planting, harvesting, and watering a garden pot.
    /// </summary>
    internal class GardenPotMachine : IMachine {
        private readonly IndoorPot Machine;
        private readonly Stack<HarvestTracker> ItemDrops;
        private readonly IRecipe[][] Recipes;
        private readonly ModConfig Config;

        public GameLocation Location { get; }
        public Rectangle TileArea { get; }
        public string MachineTypeID { get; } = "GardenPotAutomate/GardenPotMachine";

        public GardenPotMachine(ModConfig config, IndoorPot indoorPot, GameLocation location, Vector2 indoorPotTile) {
            Config = config;
            Machine = indoorPot;
            Location = location;
            TileArea = new Rectangle((int)indoorPotTile.X, (int)indoorPotTile.Y, 1, 1);
            ItemDrops = new Stack<HarvestTracker>();
            Recipes = new IRecipe[][] {
                new[]{ new SeedRecipe(indoorPot, location, config) },
                new[]{ new FertilizerRecipe(indoorPot, location, config) },
                new[]{ new WateringCanRecipe(indoorPot, location, config) }
            };
        }

        public ITrackedStack? GetOutput() {
            if (!this.Config.Enabled || !(this.Config.HarvestCrops || this.Config.HarvestFlowers))
                return null;

            var drops = ItemDrops;
            if (!drops.Any()) {
                var items = this.Harvest();
                foreach (var item in items) {
                    drops.Push(new HarvestTracker(item, drops.Count == 0));
                }
            }

            return drops.Any()
                ? new TrackedItem(drops.Peek().Item, onReduced: OnOutputReduced)
                : null;
        }

        public MachineState GetState() {
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
                dirt.destroyCrop(dirt.currentTileLocation, false, Location);

            if (// no crop, ready to plant
                dirt?.crop is null
                // no fertilizer, ready to fertilize
                || (dirt?.fertilizer?.Value ?? 0) == 0
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
                && input.TryGetIngredient(Recipes[0], out IConsumable? consumable, out IRecipe? recipe)
                && recipe is SeedRecipe
            ) {
                var seed = consumable.Take();
                if (seed is not null) {
                    seed.Stack--;
                    // don't return true here since other items may be needed
                }
            }
            if ((dirt?.fertilizer?.Value ?? 0) == 0
                && input.TryGetIngredient(Recipes[1], out consumable, out recipe)
                && recipe is FertilizerRecipe
            ) {
                var seed = consumable.Take();
                if (seed is not null) {
                    seed.Stack--;
                    // don't return true here since other items may be needed
                }
            }
            if ((dirt?.state?.Value ?? 0) != 1
                && input.TryGetIngredient(Recipes[2], out _, out recipe)
                && recipe is WateringCanRecipe
            ) {
                // water level reduction is handled in recipe
                // don't return true here since other items may be needed
            }

            // stop processing if all checks fail
            return !(dirt?.crop is null && (dirt?.fertilizer?.Value ?? 0) == 0 && (dirt?.state?.Value ?? 0) != 1);
        }

        private void OnOutputReduced(Item item) {
            if (this.Machine.bush.Value is not null)
                return;

            var drops = ItemDrops;
            if (drops.Any()) {
                if (drops.Peek().FirstDrop) {
                    HoeDirt? dirt = Machine.hoeDirt?.Value;
                    if (dirt?.crop is not null && dirt.crop.regrowAfterHarvest.Value == -1)
                        dirt.destroyCrop(dirt.currentTileLocation, false, Location);

                    Machine.heldObject.Value = null;
                }
                drops.Pop();
            }
        }

        /// <summary>
        /// Mostly a replicated routine from StardewValley.Crop.harvest(int xTile, int yTile, HoeDirt soil, JunimoHarvester junimoHarvester = null)
        /// </summary>
        /// <returns></returns>
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

            var xTile = (int)soil!.currentTileLocation.X;
            var yTile = (int)soil!.currentTileLocation.Y;

            var owner = Game1.getFarmer(this.Machine?.owner.Value ?? -1);
            var multiplayer = Traverse.Create<Game1>().Field("multiplayer").GetValue<Multiplayer>();

            var items = new List<Item>();

            bool flag;
            // Added logic to handle wild seed crops since they are a "heldObject"
            if (held is not null) {
                SObject? @object = held;

                if (@object.Stack == 0)
                    @object.Stack = 1;
                int howMuch = 3;
                Random random = new((int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2 + xTile * 1000 + yTile * 2000);

                if (Config.ApplyProfessions) {
                    if (owner.professions.Contains(16)) {
                        @object.Quality = 4;
                    } else if (random.NextDouble() < (double)(owner.ForagingLevel / 30f)) {
                        @object.Quality = 2;
                    } else if (random.NextDouble() < (double)(owner.ForagingLevel / 15f)) {
                        @object.Quality = 1;
                    }
                }

                Game1.stats.ItemsForaged += (uint)@object.Stack;
                items.Add(@object);

                if (Config.GainExperience)
                    owner.gainExperience(2, howMuch);

                return items.ToArray();
            } else if (crop is not null && crop.forageCrop.Value) {
                SObject @object = null!;
                int howMuch = 3;
                Random random = new((int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2 + xTile * 1000 + yTile * 2000);
                switch (crop!.whichForageCrop.Value) {
                    case 1:
                        @object = new SObject(399, 1);
                        break;
                    case 2:
                        soil.shake(MathF.PI / 48f, MathF.PI / 40f, (xTile * 64) < owner.Position.X);
                        return items.ToArray();
                }

                if (Config.ApplyProfessions) {
                    if (owner.professions.Contains(16)) {
                        @object.Quality = 4;
                    } else if (random.NextDouble() < (double)(owner.ForagingLevel / 30f)) {
                        @object.Quality = 2;
                    } else if (random.NextDouble() < (double)(owner.ForagingLevel / 15f)) {
                        @object.Quality = 1;
                    }
                }

                Game1.stats.ItemsForaged += (uint)@object.Stack;
                items.Add(@object);

                if (Config.GainExperience)
                    owner.gainExperience(2, howMuch);

                return items.ToArray();
            } else if (crop is not null && crop.currentPhase.Value >= crop.phaseDays.Count - 1 && (!crop.fullyGrown.Value || crop.dayOfCurrentPhase.Value <= 0)) {
                var num = 1;
                var quality = 0;
                var num2 = 0;
                if (crop.indexOfHarvest.Value == 0) {
                    return items.ToArray();
                }

                Random random2 = new(xTile * 7 + yTile * 11 + (int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame);
                switch (soil.fertilizer.Value) {
                    case 368:
                        num2 = 1;
                        break;
                    case 369:
                        num2 = 2;
                        break;
                    case 919:
                        num2 = 3;
                        break;
                }

                var num3 = 0.2 * (owner.FarmingLevel / 10.0) + 0.2 * num2 * ((owner.FarmingLevel + 2.0) / 12.0) + 0.01;
                var num4 = Math.Min(0.75, num3 * 2.0);
                if (num2 >= 3 && random2.NextDouble() < num3 / 2.0) {
                    quality = 4;
                } else if (random2.NextDouble() < num3) {
                    quality = 2;
                } else if (random2.NextDouble() < num4 || num2 >= 3) {
                    quality = 1;
                }

                if (crop.minHarvest.Value > 1 || crop.maxHarvest.Value > 1) {
                    var num5 = 0;
                    if (crop.maxHarvestIncreasePerFarmingLevel.Value > 0) {
                        num5 = owner.FarmingLevel / crop.maxHarvestIncreasePerFarmingLevel.Value;
                    }

                    num = random2.Next(crop.minHarvest.Value, Math.Max(crop.minHarvest.Value + 1, crop.maxHarvest.Value + 1 + num5));
                }

                if (crop.chanceForExtraCrops.Value > 0.0) {
                    while (random2.NextDouble() < Math.Min(0.9, crop.chanceForExtraCrops.Value)) {
                        num++;
                    }
                }

                if (crop.indexOfHarvest.Value == 771 || crop.indexOfHarvest.Value == 889) {
                    quality = 0;
                }

                var tileLocationPoint = new Point((int)Machine!.TileLocation.X / 64, (int)Machine!.TileLocation.Y / 64);

                SObject object2 = (crop.programColored.Value ? new ColoredObject(crop.indexOfHarvest.Value, 1, crop.tintColor.Value) {
                    Quality = quality
                } : new SObject(crop.indexOfHarvest.Value, 1, isRecipe: false, -1, quality));
                if (crop.harvestMethod.Value == 1) {
                    DelayedAction.playSoundAfterDelay("daggerswipe", 150, this.Location);

                    if (Utility.isOnScreen(tileLocationPoint, 64, this.Location)) {
                        this.Location.playSound("harvest");
                    }

                    if (Utility.isOnScreen(tileLocationPoint, 64, this.Location)) {
                        DelayedAction.playSoundAfterDelay("coin", 260, this.Location);
                    }

                    items.Add(object2.getOne());

                    flag = true;
                } else {
                    Vector2 vector2 = new(xTile, yTile);
                    items.Add(object2.getOne());

                    if (random2.NextDouble() < owner.team.AverageLuckLevel() / 1500.0 + owner.team.AverageDailyLuck() / 1200.0 + 9.9999997473787516E-05) {
                        num *= 2;
                        if (Utility.isOnScreen(tileLocationPoint, 64, this.Location)) {
                            this.Location.playSound("dwoop");
                        }
                    } else if (crop.harvestMethod.Value == 0) {
                        if (Utility.isOnScreen(tileLocationPoint, 64, this.Location)) {
                            this.Location.playSound("harvest");
                        }

                        if (Utility.isOnScreen(tileLocationPoint, 64, this.Location)) {
                            DelayedAction.playSoundAfterDelay("coin", 260, this.Location);
                        }

                        if (crop.regrowAfterHarvest.Value == -1 && this.Location.Equals(Game1.currentLocation)) {
                            multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(17, new Vector2(vector2.X * 64f, vector2.Y * 64f), Color.White, 7, Game1.random.NextDouble() < 0.5, 125f));
                            multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(14, new Vector2(vector2.X * 64f, vector2.Y * 64f), Color.White, 7, Game1.random.NextDouble() < 0.5, 50f));
                        }
                    }

                    flag = true;
                }

                if (flag) {
                    if (crop.indexOfHarvest.Value == 421) {
                        crop.indexOfHarvest.Value = 431;
                        num = random2.Next(1, 4);
                    }

                    object2 = (crop.programColored.Value ? new ColoredObject(crop.indexOfHarvest.Value, 1, crop.tintColor.Value) : new SObject(crop.indexOfHarvest.Value, 1));

                    if (Config.GainExperience) {
                        var num6 = Convert.ToInt32(Game1.objectInformation[crop.indexOfHarvest.Value].Split('/')[1]);
                        var num7 = (float)(16.0 * Math.Log(0.018 * num6 + 1.0, Math.E));
                        owner.gainExperience(0, (int)Math.Round(num7));
                    }

                    for (var i = 0; i < num - 1; i++) {
                        items.Add(object2.getOne());
                    }

                    if (crop.indexOfHarvest.Value == 262 && random2.NextDouble() < 0.4) {
                        SObject object3 = new(178, 1);
                        items.Add(object3.getOne());
                    } else if (crop.indexOfHarvest.Value == 771) {
                        if (soil != null && soil.currentLocation != null) {
                            soil.currentLocation.playSound("cut");
                        }

                        if (random2.NextDouble() < 0.1) {
                            SObject object4 = new(770, 1);
                            items.Add(object4.getOne());
                        }
                    }

                    if (crop.regrowAfterHarvest.Value == -1) {
                        return items.ToArray();
                    }

                    crop.fullyGrown.Value = true;
                    if (crop.dayOfCurrentPhase.Value == crop.regrowAfterHarvest.Value) {
                        var tilePosition = Traverse.Create(crop).Field("tilePosition").GetValue<Vector2>();
                        crop.updateDrawMath(tilePosition);
                    }

                    crop.dayOfCurrentPhase.Value = crop.regrowAfterHarvest.Value;
                }
            }

            return items.ToArray();
        }
    }
}