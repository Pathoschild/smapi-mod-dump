/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using xTile.Dimensions;

namespace Circuit.Events
{
    internal class NaturesWrath : EventBase
    {
        public NaturesWrath(EventType eventType) : base(eventType) { }

        public override string GetDisplayName()
        {
            return "Nature's Wrath";
        }

        public override string GetChatWarningMessage()
        {
            return "The forest is moving...";
        }

        public override string GetChatStartMessage()
        {
            return "Nature has unleashed its wrath!";
        }

        public override string GetDescription()
        {
            return "Increases the amount of daily forage, debris, and tree spawns.";
        }
        public override void OnDayStarted()
        {
            foreach (GameLocation location in Game1.locations)
            {
                if (!location.IsOutdoors)
                    continue;

                SpawnWeedsAndStones(location, 25);
                SpawnGrass(location);
                SpawnObjects(location, 15, 25);
            }
        }

        public void SpawnWeedsAndStones(GameLocation location, int amount)
        {
            int numWeedsAndStones = amount;
            if (Game1.IsRainingHere(location))
                numWeedsAndStones *= 2;

            for (int i = 0; i < numWeedsAndStones; i++)
            {
                Vector2 v = new(Game1.random.Next(location.map.Layers[0].LayerWidth), Game1.random.Next(location.map.Layers[0].LayerHeight));
                if (location is IslandWest)
                    v = new Vector2(Game1.random.Next(57, 97), Game1.random.Next(44, 68));

                Vector2 baseVect = Vector2.Zero;
                if ((location is Mountain && v.X + baseVect.X > 100f) || location is IslandNorth)
                    continue;

                bool num = location is Farm || location is IslandWest;
                int checked_tile_x = (int)(v.X + baseVect.X);
                int checked_tile_y = (int)(v.Y + baseVect.Y);
                Vector2 checked_tile = v + baseVect;
                bool is_valid_tile = false;
                bool tile_is_diggable = location.doesTileHaveProperty(checked_tile_x, checked_tile_y, "Diggable", "Back") != null;
                if (num == tile_is_diggable && location.doesTileHaveProperty(checked_tile_x, checked_tile_y, "NoSpawn", "Back") == null && location.doesTileHaveProperty(checked_tile_x, checked_tile_y, "Type", "Back") != "Wood")
                {
                    if (location.isTileLocationTotallyClearAndPlaceable(checked_tile) && !location.objects.ContainsKey(checked_tile))
                            is_valid_tile = true;
                }

                if (!is_valid_tile)
                    continue;

                int whatToAdd;
                if (location is Desert)
                    whatToAdd = 750;
                else
                {
                    if (Game1.random.NextDouble() < 0.5)
                        whatToAdd = ((!(Game1.random.NextDouble() < 0.5)) ? ((Game1.random.NextDouble() < 0.5) ? 343 : 450) : ((Game1.random.NextDouble() < 0.5) ? 294 : 295));
                    else
                        whatToAdd = GameLocation.getWeedForSeason(Game1.random, location.GetSeasonForLocation());

                    if (location is Farm && Game1.random.NextDouble() < 0.05)
                    {
                        location.terrainFeatures.Add(checked_tile, new Tree(Game1.random.Next(3) + 1, Game1.random.Next(3)));
                        continue;
                    }
                }
                if (whatToAdd == -1)
                    continue;

                bool destroyed = false;
                if (location.objects.ContainsKey(v + baseVect))
                {
                    SObject removed = location.objects[v + baseVect];
                    if (removed is Fence || removed is Chest)
                        continue;

                    if (removed.name != null && !removed.Name.Contains("Weed") && !removed.Name.Equals("Stone") && !removed.name.Contains("Twig") && removed.name.Length > 0)
                    {
                        destroyed = true;
                        Game1.debugOutput = removed.Name + " was destroyed";
                    }

                    location.objects.Remove(v + baseVect);
                }

                if (location.terrainFeatures.ContainsKey(v + baseVect))
                {
                    try
                    {
                        destroyed = location.terrainFeatures[v + baseVect] is HoeDirt || location.terrainFeatures[v + baseVect] is Flooring;
                    }
                    catch (Exception)
                    {
                    }
                    if (!destroyed)
                        break;

                    location.terrainFeatures.Remove(v + baseVect);
                }

                location.objects.Add(v + baseVect, new SObject(v + baseVect, whatToAdd, 1));
            }
        }

        public void SpawnGrass(GameLocation location)
        {
            int numberOfNewWeeds = 60;
            int grassType = location.GetSeasonForLocation() == "winter" ? Grass.frostGrass : Grass.springGrass;

            for (int i = 0; i < numberOfNewWeeds; i++)
            {
                int numberOfTries = 0;
                while (numberOfTries < 3)
                {
                    int xCoord = Game1.random.Next(location.map.DisplayWidth / 64);
                    int yCoord = Game1.random.Next(location.map.DisplayHeight / 64);
                    Vector2 tileLocation = new(xCoord, yCoord);
                    location.objects.TryGetValue(tileLocation, out var o);

                    if (o is null && location.doesTileHaveProperty(xCoord, yCoord, "Diggable", "Back") != null && location.isTileLocationOpen(new Location(xCoord, yCoord)) && !location.isTileOccupied(tileLocation) && location.doesTileHaveProperty(xCoord, yCoord, "Water", "Back") == null)
                    {
                        string noSpawn = location.doesTileHaveProperty(xCoord, yCoord, "NoSpawn", "Back");
                        if (noSpawn != null && (noSpawn.Equals("Grass") || noSpawn.Equals("All") || noSpawn.Equals("True")))
                            continue;

                        int numberOfWeeds = Game1.random.Next(1, 3);
                        location.terrainFeatures.Add(tileLocation, new Grass(grassType, numberOfWeeds));
                    }

                    numberOfTries++;
                }
            }

            foreach (var tf in location.terrainFeatures.Values)
            {
                if (tf is Grass grass)
                {
                    grass.grassType.Value = (byte)grassType;
                    grass.loadSprite();
                }
            }
        }

        public void SpawnObjects(GameLocation location, int rangeMin, int rangeMax)
        {
            Random r = new((int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed);
            Dictionary<string, string> locationData = Game1.content.Load<Dictionary<string, string>>("Data\\Locations");
            if (locationData.ContainsKey(location.Name))
            {
                string rawData = locationData[location.Name].Split('/')[Utility.getSeasonNumber(location.GetSeasonForLocation())];
                if (!rawData.Equals("-1"))
                {
                    string[] split = rawData.Split(' ');
                    int numberToSpawn = r.Next(rangeMin, rangeMax);
                    for (int i = 0; i < numberToSpawn; i++)
                    {
                        for (int j = 0; j < 11; j++)
                        {
                            int xCoord = r.Next(location.map.DisplayWidth / 64);
                            int yCoord = r.Next(location.map.DisplayHeight / 64);
                            Vector2 tileLocation = new(xCoord, yCoord);
                            location.objects.TryGetValue(tileLocation, out var o);
                            int whichObject = r.Next(split.Length / 2) * 2;
                            if (o is null && location.doesTileHaveProperty(xCoord, yCoord, "Spawnable", "Back") != null && !location.doesEitherTileOrTileIndexPropertyEqual(xCoord, yCoord, "Spawnable", "Back", "F") && r.NextDouble() < Convert.ToDouble(split[whichObject + 1], CultureInfo.InvariantCulture) && location.isTileLocationTotallyClearAndPlaceable(xCoord, yCoord) && location.getTileIndexAt(xCoord, yCoord, "AlwaysFront") == -1 && location.getTileIndexAt(xCoord, yCoord, "Front") == -1 && !location.isBehindBush(tileLocation) && (Game1.random.NextDouble() < 0.1 || !location.isBehindTree(tileLocation)) && location.dropObject(new SObject(tileLocation, Convert.ToInt32(split[whichObject]), null, canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: true), new Vector2(xCoord * 64, yCoord * 64), Game1.viewport, initialPlacement: true))
                            {
                                location.numberOfSpawnedObjectsOnMap++;
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
