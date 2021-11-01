/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/BattleRoyalley-Year2
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleRoyale.Utils
{
    internal class MapUtils
    {
        // Used to replant trees after match
        private readonly static List<TileLocation> trees = new List<TileLocation>();
        private readonly static int SlugFestWall = 3; // Iron fence

        private readonly static Point[] SlugFestWallCorners = new Point[2]
        {
            new Point(21, 61),
            new Point(38, 75)
        };

        public static void StoreTreesAndGrass()
        {
            if (trees.Count != 0)
                return;

            foreach (GameLocation gameLocation in Game1.locations)
            {
                foreach (var pair in gameLocation.terrainFeatures.Pairs)
                {
                    Vector2 vector = pair.Key;
                    TerrainFeature feature = pair.Value;
                    if (feature is Tree tree && tree.growthStage.Value >= 5)
                        trees.Add(new TileLocation(gameLocation.Name, (int)vector.X, (int)vector.Y));
                }
            }
        }

        public static void ReplantTrees()
        {
            Random random = new Random();
            // Delete all trees
            foreach (GameLocation gameLocation in Game1.locations)
            {
                foreach (Vector2 vector in gameLocation.terrainFeatures.Keys.ToList())
                {
                    TerrainFeature feature = gameLocation.terrainFeatures[vector];
                    if (feature is Tree tree && tree.growthStage.Value >= 5)
                        gameLocation.terrainFeatures.Remove(vector);
                }
            }

            // Replant
            foreach (TileLocation treeLocation in trees)
            {
                treeLocation.GetGameLocation().terrainFeatures.Remove(treeLocation.CreateVector2());
                treeLocation.GetGameLocation().terrainFeatures.Add(treeLocation.CreateVector2(), new Tree(random.Next(1, 4), 100));
            }
        }

        public static void RemoveObjectsAndDebris()
        {
            // Wipe objects
            foreach (GameLocation location in Game1.locations)
            {
                if (location.name == "Mountain")
                {
                    foreach (var tmp in location.Objects.Keys.Where(pos => pos.X < 95).ToList())
                        location.Objects.Remove(tmp);

                    foreach (var debris in location.debris)
                    {
                        foreach (var tmp in debris.Chunks.Where(chunk => chunk.position.X < 95).ToList())
                            debris.Chunks.Remove(tmp);
                    }
                }
                else
                {
                    location.Objects.Clear();
                    location.debris.Clear();
                }
            }
        }

        public static void RemoveResourceClumps()
        {
            // Remove resource clumps (e.g. boulders) other than stumps (600) and hollow logs (602)
            var clumps = Game1.getFarm().resourceClumps;
            foreach (ResourceClump clump in clumps.Where(a => a.parentSheetIndex.Value != 600 || a.parentSheetIndex.Value != 602).ToList())
                clumps.Remove(clump);

            Forest forest = (Forest)Game1.getLocationFromName("Forest");
            foreach (Vector2 vector in forest.terrainFeatures.Keys.ToList())
            {
                TerrainFeature tf = forest.terrainFeatures[vector];
                if (tf is HoeDirt)// && (tf as HoeDirt).crop?.forageCrop)
                    forest.terrainFeatures.Remove(vector);
            }
        }

        public static void RemoveStonesAndWeeds()
        {
            // Remove small stones and weeds
            var farmObjects = Game1.getFarm().objects;
            foreach (Vector2 tilePos in farmObjects.Keys.ToList())
            {
                if (farmObjects[tilePos].name == "Stone" || farmObjects[tilePos].name == "Weeds")
                    farmObjects.Remove(tilePos);
            }
        }

        public static void RemoveCharacters()
        {
            // Remove horses/NPCs
            foreach (GameLocation loc in Game1.locations)
            {
                foreach (NPC npc in loc.characters.ToList())
                    loc.characters.Remove(npc);
            }
        }

        public static void SetupGingerIsland()
        {
            foreach (GameLocation location in Game1.locations)
            {
                if (!(location is IslandLocation))
                    return;

                IslandLocation islandLocation = location as IslandLocation;

                islandLocation.buriedNutPoints.Clear();
            }
        }

        public static void SetupSpecialRound()
        {
            if (ModEntry.BRGame.IsSpecialRoundType(SpecialRoundType.SLUGFEST))
            {
                GameLocation town = Game1.getLocationFromName("Town");

                Point topLeft = SlugFestWallCorners[0];
                Point bottomRight = SlugFestWallCorners[1];
                Point topRight = new Point(bottomRight.X, topLeft.Y);
                Point bottomLeft = new Point(topLeft.X, bottomRight.Y);

                for (int i = topLeft.X; i <= topRight.X; i++)
                {
                    Vector2 pos = new Vector2(i, topLeft.Y);
                    Fence fence = new Fence(pos, SlugFestWall, false);
                    town.setObject(pos, fence);
                }
                for (int i = bottomLeft.X; i <= bottomRight.X; i++)
                {
                    Vector2 pos = new Vector2(i, bottomLeft.Y);
                    Fence fence = new Fence(pos, SlugFestWall, false);
                    town.setObject(pos, fence);
                }
                for (int i = topLeft.Y; i <= bottomLeft.Y; i++)
                {
                    Vector2 pos = new Vector2(topLeft.X, i);
                    Fence fence = new Fence(pos, SlugFestWall, false);
                    town.setObject(pos, fence);
                }
                for (int i = topRight.Y; i <= bottomRight.Y; i++)
                {
                    Vector2 pos = new Vector2(topRight.X, i);
                    Fence fence = new Fence(pos, SlugFestWall, false);
                    town.setObject(pos, fence);
                }
            }
        }

        public static void PrepareLobby()
        {
            Mountain mountain = (Mountain)Game1.getLocationFromName("Mountain");

            foreach (Warp warp in mountain.warps.ToList())
            {
                if (warp.X > 95)
                    mountain.warps.Remove(warp);
            }
        }

        public static void RefreshMap()
        {
            StoreTreesAndGrass();
            ReplantTrees();
            RemoveObjectsAndDebris();

            SetupGingerIsland();

            RemoveResourceClumps();
            RemoveStonesAndWeeds();
            RemoveCharacters();

            SetupSpecialRound();
        }
    }
}
