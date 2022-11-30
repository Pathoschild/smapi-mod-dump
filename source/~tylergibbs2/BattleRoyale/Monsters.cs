/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using BattleRoyale.Utils;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;

namespace BattleRoyale
{
    class Monsters
    {
        private static readonly int MonsterInitialSpawnSeconds = 30;
        private static readonly int MonsterFrequencySeconds = 35;
        private static readonly int MonsterSpawnMinRadius = 7;
        private static readonly int MonsterSpawnMaxRadius = 11;

        private static readonly int DelayAfterAnnounce = 3000;

        private static readonly int[] MonsterSpawnCountRange = new int[2] { 3, 6 };

        private static DateTime? WaitingFor = null;

        private static readonly Dictionary<string, double> MonsterTypes = new()
        {
            { "Frost Jelly",    0.18  },
            { "Lava Bat",       0.18  },
            { "Shadow Shaman",  0.13  },
            { "Metal Head",     0.13  },
            { "Lava Crab",      0.13  },
            { "Squid Kid",      0.05  },
            { "Shadow Brute",   0.075 },
            { "Sludge",         0.075 },
            { "Serpent",        0.025 },
            { "Haunted Skull",  0.025 }
        };

        public static Monster GetRandomMonster()
        {
            double total = 0;
            double amount = Game1.random.NextDouble();

            foreach (var kvp in MonsterTypes)
            {
                string monsterName = kvp.Key;
                double weight = kvp.Value;

                total += weight;
                if (amount <= total)
                    return GetMonster(monsterName);
            }

            return GetMonster("Frost Jelly");
        }

        public static Monster GetMonster(string name)
        {
            switch (name)
            {
                case "Frost Jelly":
                    return new GreenSlime(Vector2.Zero, 79);
                case "Sludge":
                    return new GreenSlime(Vector2.Zero, 81);
                case "Squid Kid":
                    return new SquidKid(Vector2.Zero);
                case "Metal Head":
                    return new MetalHead(name, Vector2.Zero);
                case "Lava Crab":
                    return new LavaCrab(Vector2.Zero);
                case "Shadow Brute":
                    return new ShadowBrute(Vector2.Zero);
                case "Lava Bat":
                    return new Bat(Vector2.Zero, 81);
                case "Haunted Skull":
                    return new Bat(Vector2.Zero, 77377);
                case "Shadow Shaman":
                    return new ShadowShaman(Vector2.Zero);
                case "Serpent":
                    return new Serpent(Vector2.Zero);
                default:
                    break;
            }
            return new Monster(name, Vector2.Zero);
        }

        public static void SpawnMonstersAroundPlayer(Farmer player, int amount, int minRadius, int maxRadius)
        {
            int playerX = player.getTileX();
            int playerY = player.getTileY();

            int spawned = 0;
            int iterations = 0;
            int radius = minRadius;

            Monster monster = GetRandomMonster();
            while (spawned < amount && iterations < 100)
            {
                iterations++;

                int tileX = Game1.random.Next(playerX - radius, playerX + radius);
                int tileY = Game1.random.Next(playerY - radius, playerY + radius);
                if (tileX == playerX && tileY == playerY || !player.currentLocation.isTileLocationTotallyClearAndPlaceable(new Vector2(tileX, tileY)))
                {
                    if (radius < maxRadius)
                        radius++;
                    continue;
                }

                radius = minRadius;

                monster.position.Set(new Vector2(tileX * 64, tileY * 64));
                monster.objectsToDrop.Clear();
                monster.coinsToDrop.Value = 0;
                player.currentLocation.characters.Add(monster);

                spawned++;
                monster = GetRandomMonster();
            }
        }

        public static void Reset()
        {
            WaitingFor = null;
        }

        public static void Check()
        {
            Round round = ModEntry.BRGame.GetActiveRound();
            if (round == null || !round.InProgress || !round.IsSpecialRoundType(SpecialRoundType.MONSTERS))
                return;

            if (WaitingFor == null)
                WaitingFor = DateTime.Now.AddSeconds(MonsterInitialSpawnSeconds);

            if (DateTime.Now < WaitingFor)
                return;

            WaitingFor = DateTime.Now.AddSeconds(MonsterFrequencySeconds);

            NetworkUtils.SendChatMessageToAllPlayers("Monsters are rising!");

            DelayedAction.functionAfterDelay(() =>
            {
                foreach (Farmer player in round.AlivePlayers)
                {
                    int toSpawn = Game1.random.Next(MonsterSpawnCountRange[0], MonsterSpawnCountRange[1] + 1);
                    SpawnMonstersAroundPlayer(player, toSpawn, MonsterSpawnMinRadius, MonsterSpawnMaxRadius);
                }
            }, DelayAfterAnnounce);
        }
    }
}
