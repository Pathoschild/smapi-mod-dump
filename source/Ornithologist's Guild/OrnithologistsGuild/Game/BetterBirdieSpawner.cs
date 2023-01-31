/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/greyivy/OrnithologistsGuild
**
*************************************************/

using System;
using System.Linq;
using DynamicGameAssets.Game;
using Microsoft.Xna.Framework;
using OrnithologistsGuild.Content;
using OrnithologistsGuild.Game;
using OrnithologistsGuild.Game.Critters;
using OrnithologistsGuild.Models;
using StardewValley;
using StardewValley.BellsAndWhistles;

namespace OrnithologistsGuild
{
    public class BetterBirdieSpawner
    {
        public static void AddBirdies(GameLocation location, double chance = 0, bool onScreen = false)
        {
            // If AddBirdies is called before warp is complete, ContentPactcher conditions will not be up to date with the new location
            if (Game1.isWarping)
            {
                ModEntry.Instance.Monitor.Log("Deferring AddBirdies until after warp...");

                EventHandler<StardewModdingAPI.Events.WarpedEventArgs> handlerPlayerWarped = null;
                handlerPlayerWarped = delegate
                {
                    ModEntry.Instance.Monitor.Log("...warped, calling AddBirdies");
                    AddBirdies(location, chance, onScreen);

                    ModEntry.Instance.Helper.Events.Player.Warped -= handlerPlayerWarped;
                };

                ModEntry.Instance.Helper.Events.Player.Warped += handlerPlayerWarped;

                return;
            }

            if (!location.IsOutdoors) return;

            // TODO redo chance system
            // Original SDV source:
            // `double chance1 = Math.Max(0.15, Math.Min(0.5, (double) (this.map.Layers[0].LayerWidth * this.map.Layers[0].LayerHeight) / 15000.0));
            // ...followed by other chances for other types of birds, e.g. woodpecker (which is 1/5th of `chance1`)
            chance = Math.Clamp(chance, 0.15, 0.35);

            ModEntry.Instance.Monitor.Log($"AddBirdies onScreen={onScreen} chance={chance}");

            // First, get locations of all bird feeders
            foreach (var overlaidDict in location.Objects)
            {
                foreach (var obj in overlaidDict.Values)
                {
                    if (typeof(CustomBigCraftable).IsAssignableFrom(obj.GetType()))
                    {
                        var feeder = (CustomBigCraftable)obj;

                        // Only attract birds if there is food
                        if (feeder.MinutesUntilReady > 0)
                        {
                            var feederDef = FeederDef.FromFeeder(feeder);
                            if (feederDef != null)
                            {
                                var foodDef = FoodDef.FromFeeder(feeder);
                                if (foodDef != null)
                                {
                                    AddBirdiesNearFeeder(location, feeder, feederDef, foodDef, chance, onScreen);
                                }
                            }
                        }
                    }
                }
            }

            if (chance > 0) AddRandomBirdies(location, chance, onScreen);
        }

        private static void AddRandomBirdies(GameLocation location, double chance, bool onScreen)
        {
            ModEntry.Instance.Monitor.Log("AddRandomBirdies");

            BirdieDef flockBirdieDef = null;

            // Chance to add another flock
            int flocksAdded = 0;
            while ((ModEntry.debug_AlwaysSpawn != null && flocksAdded == 0) || Game1.random.NextDouble() < chance / (flocksAdded + 1)) // Chance lowers after every flock
            {
                // Determine flock parameters
                flockBirdieDef = ModEntry.debug_AlwaysSpawn == null ? GetRandomBirdieDef() : ModEntry.debug_AlwaysSpawn;
                if (flockBirdieDef == null) return;

                int flockSize = Game1.random.Next(1, flockBirdieDef.MaxFlockSize + 1);

                // Try 50 times to find an empty patch within the location
                for (int trial = 0; trial < 50; trial++)
                {
                    // Get a random tile on the map
                    var randomTile = location.getRandomTile();

                    if (Utility.isOnScreen(randomTile * Game1.tileSize, Game1.tileSize) != onScreen) continue;
                    if (!BetterBirdie.CanSpawnAtOrRelocateTo(location, randomTile, flockBirdieDef)) continue;

                    ModEntry.Instance.Monitor.Log($"Found clear location at {randomTile}, adding flock of {flockSize} {flockBirdieDef.ID}");

                    // Spawn birdies
                    for (int index = 0; index < flockSize; ++index)
                    {
                        // 5% chance to spawn bird perched
                        Perch perch = null;
                        if (Game1.random.NextDouble() < 0.05)
                        {
                            perch = Perch.GetRandomAvailablePerch(location, flockBirdieDef);
                            // Ensure perch is/isn't onscreen
                            if (perch != null && Utility.isOnScreen(Utilities.XY(perch.Position), Game1.tileSize) != onScreen) perch = null;
                        }

                        if (perch == null) {
                            var tile = Utility.getTranslatedVector2(randomTile, Game1.random.Next(4), 1f);
                            location.addCritter((Critter)new BetterBirdie(flockBirdieDef, tile));
                        } else {
                            location.addCritter(new BetterBirdie(flockBirdieDef, Vector2.Zero, perch));
                        }
                    }

                    flocksAdded++;

                    break;
                }
            }
        }

        private static void AddBirdiesNearFeeder(GameLocation location, CustomBigCraftable feeder, Models.FeederDef feederDef, Models.FoodDef food, double chance, bool onScreen)
        {
            ModEntry.Instance.Monitor.Log("AddBirdiesNearFeeder");

            // Build a rectangle around the feeder based on the range
            var feederRect = Utility.getRectangleCenteredAt(feeder.TileLocation, (feederDef.Range * 2) + 1);

            BirdieDef flockBirdieDef = null;

            // Chance to add another flock
            int flocksAdded = 0;
            while (flocksAdded < feederDef.MaxFlocks && Game1.random.NextDouble() < (chance * 2))
            {
                // Determine flock parameters
                flockBirdieDef = GetRandomFeederBirdieDef(feederDef, food);
                if (flockBirdieDef == null) return;

                int flockSize = Game1.random.Next(1, flockBirdieDef.MaxFlockSize + 1);

                var shouldAddBirdToFeeder = flocksAdded == 0 && Game1.random.NextDouble() < 0.65;
                // Ensure feeder is/isn't onscreen
                if (Utility.isOnScreen(feeder.TileLocation * Game1.tileSize, Game1.tileSize) != onScreen) shouldAddBirdToFeeder = false;
                if (shouldAddBirdToFeeder) flockSize -= 1;

                // Try 50 times to find an empty patch within the feeder range
                for (int trial = 0; trial < 50; trial++)
                {
                    // Get a random tile within the feeder range
                    var randomTile = Utility.getRandomPositionInThisRectangle(feederRect, Game1.random);

                    if (Utility.isOnScreen(randomTile * Game1.tileSize, Game1.tileSize) != onScreen) continue;
                    if (!BetterBirdie.CanSpawnAtOrRelocateTo(location, randomTile, flockBirdieDef)) continue;

                    ModEntry.Instance.Monitor.Log($"Found clear location at {randomTile}, adding flock of {flockSize} {flockBirdieDef.ID}");

                    // Spawn birdies
                    for (int index = 0; index < flockSize; ++index)
                    {
                        var tile = Utility.getTranslatedVector2(randomTile, Game1.random.Next(4), 1f);
                        location.addCritter((Critter)new BetterBirdie(flockBirdieDef, tile));
                    }

                    flocksAdded++;
                    break;
                }

                var perch = new Perch(feeder);
                if (shouldAddBirdToFeeder && perch.GetOccupant(location) == null)
                {
                    location.addCritter((Critter)new BetterBirdie(flockBirdieDef, Vector2.Zero, perch));
                }
            }
        }

        private static BirdieDef GetRandomBirdieDef()
        {
            return Utilities.WeightedRandom<BirdieDef>(ContentPackManager.BirdieDefs.Values, birdieDef => birdieDef.GetContextualWeight(true));
        }

        private static BirdieDef GetRandomFeederBirdieDef(Models.FeederDef feederDef, Models.FoodDef foodDef)
        {
            var usualSuspects = ContentPackManager.BirdieDefs.Values.Where(birdieDef => birdieDef.CanPerchAt(feederDef) && birdieDef.CanEat(foodDef));

            return Utilities.WeightedRandom<BirdieDef>(usualSuspects, birdieDef => birdieDef.GetContextualWeight(true, feederDef, foodDef));
        }
    }
}

