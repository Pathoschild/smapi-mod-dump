using Microsoft.Xna.Framework;
using Spawn_Monsters.Monsters;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;

namespace Spawn_Monsters.MonsterSpawning
{
    class Spawner
    {

        private IMonitor Monitor;
        private static Spawner instance;
        private readonly List<Monster> spawnedMonsters;
        private readonly Random random;

        public Spawner() {
            instance = this;
            random = new Random();
            spawnedMonsters = new List<Monster>();
        }

        public static Spawner GetInstance() {
            if (instance == null) {
                instance = new Spawner();
            }
            return instance;
        }

        public void RegisterMonitor(IMonitor monitor) {
            Monitor = monitor;
        }

        public static bool IsOkToPlace(MonsterData.Monster monster, Vector2 tile) {
            if (monster == MonsterData.Monster.Duggy) {
                if (Game1.currentLocation.map.GetLayer("Back").Tiles[(int)tile.X, (int)tile.Y] != null) {
                    if (Game1.currentLocation.map.GetLayer("Back").Tiles[(int)tile.X, (int)tile.Y].TileIndexProperties.ContainsKey("Diggable")) {
                        return true;
                    } else if (!Game1.currentLocation.map.GetLayer("Back").Tiles[(int)tile.X, (int)tile.Y].TileIndexProperties.ContainsKey("Diggable") && Game1.currentLocation.map.GetLayer("Back").Tiles[(int)tile.X, (int)tile.Y].TileIndex == 0) {
                        return true;
                    }
                }
                return false;
            }
            return true;
        }

        public bool SpawnMonster(MonsterData.Monster monster, Vector2 location) {
            //spawn monster
            if (IsOkToPlace(monster, location)) {
                MonsterData monsterData = MonsterData.GetMonsterData(monster);
                object[] args = monsterData.ConstructorArgs;

                if (monster == MonsterData.Monster.BlackSlime) {
                    args[1] = new Color(40 + random.Next(10), 40 + random.Next(10), 40 + random.Next(10));
                }

                args[0] = location;
                Monster m = (Monster)Activator.CreateInstance(monsterData.Type, args);
                m.currentLocation = Game1.currentLocation;

                if (monster == MonsterData.Monster.GraySlime) {
                    int num = Game1.random.Next(120, 200);
                    ((GreenSlime)m).color.Value = new Color(num, num, num);
                    while (Game1.random.NextDouble() < 0.33)
                        m.objectsToDrop.Add(380);
                    m.Speed = 1;
                } else if (monster == MonsterData.Monster.Duggy || monster == MonsterData.Monster.WildernessGolem) {
                    m.setTileLocation(location); //For Tile-Locked Monsters like Duggy
                }

                Game1.currentLocation.addCharacter(m);
                spawnedMonsters.Add(m);
                Monitor.Log("Spawned " + monster + " at " + location, LogLevel.Debug);

                return true;
            } else {
                return false;
            }
        }

        public void SpawnMonster(MonsterData.Monster monster, Vector2 location, int amount) {
            for (int i = 0; i < amount; i++) {
                if (!SpawnMonster(monster, location)) {
                    Monitor.Log("You may not place this monster there.", LogLevel.Info);
                    break;
                }
            }
            Monitor.Log($"Spawned {amount} {MonsterData.GetMonsterData(monster).Displayname} at {location}", LogLevel.Info);
        }

        public void KillEverything() {
            List<Monster> toKill = new List<Monster>();

            //Determine Monsters to kill
            foreach (Monster m in spawnedMonsters) {
                //GreenSlime, BigSlime, Bug, Grub, Monster, MetalHead
                if (m.GetType() != typeof(GreenSlime) && m.GetType() != typeof(BigSlime) && m.GetType() != typeof(Bug) && m.GetType() != typeof(Grub) && m.GetType() != typeof(MetalHead)) {
                    toKill.Add(m);
                }
            }

            Monitor.Log("Removing " + toKill.Count + " Monsters", LogLevel.Trace);
            foreach (Monster m in toKill) {
                Monitor.Log("Removed " + m.getTextureName(), LogLevel.Trace);
                Game1.removeThisCharacterFromAllLocations(m);
                spawnedMonsters.Remove(m);
            }
            if (toKill.Count > 0) {
                Game1.addHUDMessage(new HUDMessage("Removed " + toKill.Count + " Monsters to prevent saving-errors.", 2));
            }
        }
    }
}
