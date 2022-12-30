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
using System.Collections.Generic;
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
        public static BirdieDef debugAlwaysSpawn = null;

        public static void AddBirdies(GameLocation location, double chance = 0, bool onlyIfOnScreen = false)
        {
            // If AddBirdies is called before warp is complete, ContentPactcher conditions will not be up to date with the new location
            if (Game1.isWarping)
            {
                ModEntry.Instance.Monitor.Log("Deferring AddBirdies until after warp...");

                EventHandler<StardewModdingAPI.Events.WarpedEventArgs> handlerPlayerWarped = null;
                handlerPlayerWarped = delegate
                {
                    ModEntry.Instance.Monitor.Log("...warped, calling AddBirdies");
                    AddBirdies(location, chance, onlyIfOnScreen);

                    ModEntry.Instance.Helper.Events.Player.Warped -= handlerPlayerWarped;
                };

                ModEntry.Instance.Helper.Events.Player.Warped += handlerPlayerWarped;

                return;
            }

            if (!location.IsOutdoors) return;

            ModEntry.Instance.Monitor.Log("AddBirdies");

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
                                    AddBirdiesNearFeeder(location, feeder, feederDef, foodDef, onlyIfOnScreen);
                                }
                            }
                        }
                    }
                }
            }

            if (chance > 0) AddRandomBirdies(location, chance, onlyIfOnScreen);
        }

        private static void AddRandomBirdies(GameLocation location, double chance, bool onlyIfOnScreen)
        {
            ModEntry.Instance.Monitor.Log("AddRandomBirdies");

            BirdieDef flockBirdieDef = null;

            // Override chance
            if (location is Farm) chance = 0.15;

            // Chance to add another flock
            int flocksAdded = 0;
            while ((debugAlwaysSpawn != null && flocksAdded == 0) || Game1.random.NextDouble() < chance / (flocksAdded + 1)) // Chance lowers after every flock
            {
                // Determine flock parameters
                flockBirdieDef = debugAlwaysSpawn == null ? GetRandomBirdieDef() : debugAlwaysSpawn;
                if (flockBirdieDef == null) return;

                int flockSize = Game1.random.Next(1, flockBirdieDef.MaxFlockSize + 1);

                // Try 50 times to find an empty patch within the location
                for (int trial = 0; trial < 50; trial++)
                {
                    // Get a random tile within the feeder range
                    var randomTile = location.getRandomTile();

                    if (!onlyIfOnScreen || !Utility.isOnScreen(randomTile * Game1.tileSize, Game1.tileSize))
                    {
                        // Get a 3x3 patch around the random tile
                        var randomRect = new Microsoft.Xna.Framework.Rectangle((int)randomTile.X - 1, (int)randomTile.Y - 1, 3, 3);

                        if (!location.isAreaClear(randomRect)) continue;

                        ModEntry.Instance.Monitor.Log($"Found clear location at {randomRect}, adding flock of {flockSize} {flockBirdieDef.ID}");

                        // Spawn birdies
                        List<Critter> crittersToAdd = new List<Critter>();
                        for (int index = 0; index < flockSize; ++index)
                        {
                            Perch perch = null;

                            // 5% chance to spawn bird perched
                            if (Game1.random.NextDouble() < 0.05)
                            {
                                perch = Perch.GetRandomAvailablePerch(location, flockBirdieDef);
                            }

                            if (perch == null) {
                                crittersToAdd.Add((Critter)new BetterBirdie(flockBirdieDef, Vector2.Zero));
                            } else if (!onlyIfOnScreen || !Utility.isOnScreen(Utilities.XY(perch.Position), Game1.tileSize))
                            {
                                location.critters.Add(new BetterBirdie(flockBirdieDef, Vector2.Zero, perch));
                            }
                        }

                        ModEntry.Instance.Helper.Reflection.GetMethod(location, "addCrittersStartingAtTile").Invoke(randomTile, crittersToAdd);

                        flocksAdded++;

                        break;
                    }
                }
            }
        }

        private static void AddBirdiesNearFeeder(GameLocation location, CustomBigCraftable feeder, Models.FeederDef feederDef, Models.FoodDef food, bool onlyIfOnScreen)
        {
            ModEntry.Instance.Monitor.Log("AddBirdiesNearFeeder");

            // Build a rectangle around the feeder based on the range
            var feederRect = Utility.getRectangleCenteredAt(feeder.TileLocation, (feederDef.range * 2) + 1);

            BirdieDef flockBirdieDef = null;

            // Chance to add another flock
            int flocksAdded = 0;
            while (flocksAdded < feederDef.maxFlocks && Game1.random.NextDouble() < 0.3)
            {
                // Determine flock parameters
                flockBirdieDef = GetRandomFeederBirdieDef(feederDef, food);
                if (flockBirdieDef == null) return;

                int flockSize = Game1.random.Next(1, flockBirdieDef.MaxFlockSize + 1);

                var shouldAddBirdToFeeder = flocksAdded == 0 && Game1.random.NextDouble() < 0.65 && (!onlyIfOnScreen || !Utility.isOnScreen(feeder.TileLocation * Game1.tileSize, Game1.tileSize));
                if (shouldAddBirdToFeeder) flockSize -= 1;

                // Try 50 times to find an empty patch within the feeder range
                for (int trial = 0; trial < 50; trial++)
                {
                    // Get a random tile within the feeder range
                    var randomTile = Utility.getRandomPositionInThisRectangle(feederRect, Game1.random);

                    if (location.isTileOnMap(randomTile) && (!onlyIfOnScreen || !Utility.isOnScreen(randomTile * Game1.tileSize, Game1.tileSize)))
                    {
                        // Get a 3x3 patch around the random tile 
                        // var randomRect = new Microsoft.Xna.Framework.Rectangle((int)randomTile.X - 2, (int)randomTile.Y - 2, 5, 5); // TODO revert to 5x5 if needed
                        var randomRect = new Microsoft.Xna.Framework.Rectangle((int)randomTile.X - 1, (int)randomTile.Y - 1, 3, 3);

                        if (!location.isAreaClear(randomRect)) continue;

                        ModEntry.Instance.Monitor.Log($"Found clear location at {randomRect}, adding flock of {flockSize} {flockBirdieDef.ID}");

                        // Spawn birdies
                        List<Critter> crittersToAdd = new List<Critter>();

                        for (int index = 0; index < flockSize; ++index)
                        {
                            crittersToAdd.Add((Critter)new BetterBirdie(flockBirdieDef, Vector2.Zero));
                        }

                        ModEntry.Instance.Helper.Reflection.GetMethod(location, "addCrittersStartingAtTile").Invoke(randomTile, crittersToAdd);

                        flocksAdded++;
                        break;
                    }
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

