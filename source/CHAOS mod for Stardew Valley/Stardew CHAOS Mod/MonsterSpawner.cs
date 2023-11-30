/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DevWithMaj/Stardew-CHAOS
**
*************************************************/

using StardewModdingAPI;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley;
using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Stardew_CHAOS_Mod
{
    internal class MonsterSpawner
    {
        // this is to clear monsters on room entry, makes spawning easier on start when player doesn't have sword
        public static List<Monster> monsters = new List<Monster>();

        public static void ClearMonsters()
        {
            foreach (var monster in monsters)
            {
                monster.takeDamage(1000, 0, 0, false, 0, "skeletonDie"); // putting this because you  have to have a sound
            }
            monsters.Clear();
        }

        public static void SpawnSlimes(GameLocation location, int amount, int spawnRangeInTiles)
        {
            Random random = new Random();
            Vector2 playerTileLocation = Game1.player.getTileLocation();
            for (int i = 0; i < amount; i++)
            {
                // iterate 5 times to find a valid tile
                for (int attempt = 0; attempt < 5; attempt++)
                {
                    int xDelta = random.Next(-spawnRangeInTiles, spawnRangeInTiles);
                    int yDelta = random.Next(-spawnRangeInTiles, spawnRangeInTiles);
                    Vector2 spawnLocation = new Vector2(playerTileLocation.X + xDelta, playerTileLocation.Y + yDelta);
                    if (IsOkToSpawnMonster(spawnLocation))
                    {
                        BigSlime slime = new BigSlime(Game1.player.getStandingPosition(), 50);
                        slime.currentLocation = location;
                        slime.setTileLocation(spawnLocation);
                        location.addCharacter(slime);
                        monsters.Add(slime);
                        break;
                    }
                }
            }
        }

        public static void SpawnBats(GameLocation location, int amount, int spawnRangeInTiles)
        {
            Random random = new Random();
            Vector2 playerTileLocation = Game1.player.getTileLocation();
            for (int i = 0; i < amount; i++)
            {
                // iterate 5 times to find a valid tile
                for (int attempt = 0; attempt < 5; attempt++)
                {
                    int xDelta = random.Next(-spawnRangeInTiles, spawnRangeInTiles);
                    int yDelta = random.Next(-spawnRangeInTiles, spawnRangeInTiles);
                    Vector2 spawnLocation = new Vector2(playerTileLocation.X + xDelta, playerTileLocation.Y + yDelta);
                    if (IsOkToSpawnMonster(spawnLocation))
                    {
                        Bat bat = new Bat(Game1.player.getStandingPosition());
                        bat.currentLocation = location;
                        bat.setTileLocation(spawnLocation);
                        location.addCharacter(bat);
                        monsters.Add(bat);
                        break;
                    }
                }
            }
        }

        public static void SpawnSerpents(GameLocation location, int amount, int spawnRangeInTiles)
        {
            Random random = new Random();
            Vector2 playerTileLocation = Game1.player.getTileLocation();
            for (int i = 0; i < amount; i++)
            {
                // iterate 5 times to find a valid tile
                for (int attempt = 0; attempt < 5; attempt++)
                {
                    int xDelta = random.Next(-spawnRangeInTiles, spawnRangeInTiles);
                    int yDelta = random.Next(-spawnRangeInTiles, spawnRangeInTiles);
                    Vector2 spawnLocation = new Vector2(playerTileLocation.X + xDelta, playerTileLocation.Y + yDelta);
                    if (IsOkToSpawnMonster(spawnLocation))
                    {
                        Serpent bat = new Serpent(Game1.player.getStandingPosition());
                        bat.currentLocation = location;
                        bat.setTileLocation(spawnLocation);
                        location.addCharacter(bat);
                        monsters.Add(bat);
                        break;
                    }
                }
            }
        }

        // in case i want more functionality with this
        private static bool IsOkToSpawnMonster(Vector2 tile)
        {
            if (Game1.currentLocation.isTileLocationTotallyClearAndPlaceableIgnoreFloors(tile))
            {
                return true;
            }
            return false;
        }
    }

    // This DESTROYS the code it really does not like generic monsters, only leaving it here cuz I spent a lot of time making it generic
    /*        private void SpawnMonsters<Monster>(GameLocation location, int amount) where Monster: NPC, new()
            {
                Random random = new Random();
                NetPosition playerPos = Game1.player.position;
                for (int i = 0; i < amount; i++)
                {
                    int xMultiplyer = random.Next(-15, 15);
                    int yMultiplyer = random.Next(-15, 15);
                    int x = 100 * xMultiplyer;
                    int y = 100 * yMultiplyer;
                    Monster monster = new Monster();
                    monster.Position = new Vector2(playerPos.X + x, playerPos.Y + y);
                    location.addCharacter(monster);
                }
            }*/

}
