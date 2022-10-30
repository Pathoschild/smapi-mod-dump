/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/maxvollmer/DeepWoodsMod
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using System;
using static DeepWoodsMod.DeepWoodsRandom;
using static DeepWoodsMod.DeepWoodsSettings;
using System.Collections.Generic;
using System.Linq;
using DeepWoodsMod.API.Impl;
using xTile.Dimensions;

namespace DeepWoodsMod
{
    public class DeepWoodsMonsters
    {
        public class MonsterDecider
        {
            public int MinLevel { get; set; }
            public Chance Chance { get; set; }

            // For JSON serialization
            public MonsterDecider() { }

            public MonsterDecider(int minLevel, Chance chance)
            {
                MinLevel = minLevel;
                Chance = chance;
            }

            public MonsterDecider(int minLevel, int chance)
            {
                MinLevel = minLevel;
                Chance = new Chance(chance);
            }
        }

        private DeepWoods deepWoods;
        private DeepWoodsRandom random;

        private DeepWoodsMonsters(DeepWoods deepWoods, DeepWoodsRandom random)
        {
            this.deepWoods = deepWoods;
            this.random = random;
        }

        public static void AddMonsters(DeepWoods deepWoods, DeepWoodsRandom random, HashSet<Location> blockedLocations)
        {
            new DeepWoodsMonsters(deepWoods, random).AddMonsters(blockedLocations);
        }

        private void AddMonsters(HashSet<Location> blockedLocations)
        {
            if (!Game1.IsMasterGame)
                return;

            if (deepWoods.isLichtung.Value && !deepWoods.isInfested.Value)
                return;

            deepWoods.characters.Clear();

            if (!deepWoods.isInfested.Value && Settings.Monsters.MonsterDensity <= 0)
                return;

            int mapWidth = deepWoods.mapWidth.Value;
            int mapHeight = deepWoods.mapHeight.Value;

            // Calculate maximum theoretical amount of monsters for the current map.
            int maxMonsters = (mapWidth * mapHeight) / Math.Max(1, DeepWoodsSettings.Settings.Generation.MinTilesForMonsters);
            int minMonsters = Math.Min(deepWoods.level.Value, maxMonsters);

            // Get a random value from 0 to maxMonsters, using a "two dice" method, where the center numbers are more likely than the edges.
            // If, for example, maxMonsters is 100, it is much more likely to get something close to 50 than close to 100 or 0.
            // We then take the maximum of either minMonsters or the result, making sure we always have at least minMonsters monsters.
            int numMonsters = Math.Max(minMonsters, this.random.GetRandomValue(minMonsters, Math.Max(minMonsters, maxMonsters / 2)) + this.random.GetRandomValue(minMonsters, Math.Max(minMonsters, maxMonsters / 2)));

            if (deepWoods.isInfested.Value)
            {
                numMonsters *= 2;
            }

            if (deepWoods.GetCombatLevel() <= 1 || this.random.CheckChance(Settings.Monsters.ChanceForHalfMonsterCount))
            {
                numMonsters /= 2;
            }

            if (!deepWoods.isInfested.Value && Settings.Monsters.MonsterDensity < 100)
            {
                numMonsters = (int)(numMonsters * Settings.Monsters.MonsterDensity / 100);
            }
            else if (deepWoods.isInfested.Value && Settings.Monsters.InfestedMonsterDensity < 100)
            {
                numMonsters = (int)(numMonsters * Settings.Monsters.InfestedMonsterDensity / 100);
            }

            if (deepWoods.isInfested.Value)
            {
                // minimum 10 monsters in infested level
                numMonsters = Math.Max(numMonsters, 10);
            }

            if (numMonsters == 0)
                return;

            List<int> allTilesInRandomOrder = Enumerable.Range(0, mapWidth * mapHeight).OrderBy(n => Game1.random.Next()).ToList();
            int allTilesCount = allTilesInRandomOrder.Count();

            int numMonstersPlaced = 0;
            Monster monster = null;
            for (int i = 0, j = 0; i < numMonsters && j < allTilesCount; j++)
            {
                int tileIndex = allTilesInRandomOrder[j];
                int x = tileIndex % mapWidth;
                int y = tileIndex / mapWidth;

                if (monster == null)
                {
                    monster = CreateRandomMonster(new Vector2(x, y));
                }
                if (CanPlaceMonsterHere(deepWoods, blockedLocations, x, y, monster))
                {
                    monster.Position = new Vector2(x * 64f, y * 64f) - new Vector2(0, monster.Sprite.SpriteHeight - 64);
                    deepWoods.addCharacter(monster);
                    monster = null;
                    i++;
                    numMonstersPlaced++;
                }
            }
        }

        private bool IsTileFree(HashSet<Location> blockedLocations, Vector2 location)
        {
            if (blockedLocations.Contains(new Location((int)location.X, (int)location.Y)))
                return false;

            // No placements on tiles that are covered in forest.
            if (deepWoods.map.GetLayer("Buildings").Tiles[(int)location.X, (int)location.Y] != null)
                return false;

            // No placements on borders, exits and enter locations.
            if (deepWoods.IsLocationOnBorderOrExit(location))
                return false;

            // Don't place anything on water
            if (deepWoods.doesTileHaveProperty((int)location.X, (int)location.Y, "Water", "Back") != null)
                return false;

            return true;
        }

        private bool CanPlaceMonsterHere(DeepWoods deepWoods, HashSet<Location> blockedLocations, int x, int y, Monster monster)
        {
            foreach (NPC npc in deepWoods.characters)
            {
                if (npc.GetBoundingBox().Intersects(monster.GetBoundingBox()))
                    return false;
            }

            if (monster.isGlider.Value)
                return true;

            Microsoft.Xna.Framework.Rectangle rectangle = monster.GetBoundingBox();
            rectangle.X = x;
            rectangle.Y = y;
            rectangle.Width /= 16;
            rectangle.Height /= 16;

            for (int i = 0; i < rectangle.Width; i++)
            {
                for (int j = 0; j < rectangle.Height; j++)
                {
                    if (!IsTileFree(blockedLocations, new Vector2(x + i, y + j)))
                        return false;
                }
            }

            return true;
        }

        Monster CreateRandomMonster(Vector2 location)
        {
            Monster monster = null;

            if (Game1.isDarkOut() && CanHazMonster(Settings.Monsters.Bat))
            {
                monster = new Bat(new Vector2());
            }
            else if (Game1.isDarkOut() && CanHazMonster(Settings.Monsters.Ghost))
            {
                monster = new Ghost(new Vector2());
            }
            else if (CanHazMonster(Settings.Monsters.BigSlime))
            {
                monster = new BigSlime(new Vector2(), GetSlimeLevel());
            }
            else if (CanHazMonster(Settings.Monsters.Grub))
            {
                monster = new Grub(new Vector2(), true);
            }
            else if (CanHazMonster(Settings.Monsters.Fly))
            {
                monster = new Fly(new Vector2(), true);
            }
            else if (CanHazMonster(Settings.Monsters.Brute))
            {
                monster = new ShadowBrute(new Vector2());
            }
            else if (CanHazMonster(Settings.Monsters.Golem))
            {
                monster = new RockGolem(new Vector2(), deepWoods.GetCombatLevel());
            }
            else if (CanHazMonster(Settings.Monsters.RockCrab))
            {
                monster = new RockCrab(new Vector2(), GetRockCrabType());
            }
            else if (CanHazMonster(Settings.Monsters.Bug))
            {
                monster = new Bug(new Vector2(), 0);
                monster.isHardModeMonster.Value = true;
            }
            else if (CanHazMonster(Settings.Monsters.ArmoredBug) && !deepWoods.isInfested.Value)
            {
                monster = new Bug(new Vector2(), 121);
                monster.isHardModeMonster.Value = true;
            }
            else if (Game1.isDarkOut() && CanHazMonster(Settings.Monsters.PutridGhost))
            {
                monster = new Ghost(new Vector2(), "Putrid Ghost");
            }
            else if (Game1.isDarkOut() && CanHazMonster(Settings.Monsters.DustSprite))
            {
                monster = new DustSpirit(new Vector2()); new Leaper();
            }
            else if (Game1.isDarkOut() && CanHazMonster(Settings.Monsters.Spider))
            {
                monster = new Leaper(new Vector2());
            }
            else
            {
                foreach (var modMonster in DeepWoodsAPI.ToShuffledList(ModEntry.GetAPI().Monsters))
                {
                    // Item1 is a mod provided function that returns true if the mod wants to spawn a custom monster at this location.
                    // Item2 is a mod provided function that returns the custom monster.
                    if (modMonster.Item1(deepWoods, location))
                    {
                        monster = modMonster.Item2();
                        break;
                    }
                }
            }

            // No other monster was selected and no mod provided a monster, default to green slime
            if (monster == null)
            {
                monster = new GreenSlime(new Vector2(), GetSlimeLevel());
            }

            if (!Settings.Monsters.DisableBuffedMonsters
                && deepWoods.level.Value >= Settings.Level.MinLevelForBuffedMonsters
                && !this.random.CheckChance(Settings.Monsters.ChanceForUnbuffedMonster))
            {
                BuffMonster(monster);
            }

            if (!Settings.Monsters.DisableDangerousMonsters
                && deepWoods.level.Value >= Settings.Level.MinLevelForDangerousMonsters
                && !this.random.CheckChance(Settings.Monsters.ChanceForNonDangerousMonster))
            {
                monster.isHardModeMonster.Value = true;
            }

            monster.faceDirection(this.random.GetRandomValue(0, 4));

            return monster;
        }

        private void BuffMonster(Monster monster)
        {
            int maxAddedSpeed = deepWoods.GetCombatLevel() / 3 + (deepWoods.level.Value - Settings.Level.MinLevelForBuffedMonsters) / 10;
            int minAddedSpeed = maxAddedSpeed / 3;

            float maxBuff = deepWoods.GetCombatLevel() * 0.5f + (deepWoods.level.Value - Settings.Level.MinLevelForBuffedMonsters) * 0.1f;
            float minBuff = maxBuff * 0.25f;

            monster.addedSpeed = Math.Max(monster.addedSpeed, monster.addedSpeed + Game1.random.Next(minAddedSpeed, maxAddedSpeed));
            monster.missChance.Value = Math.Max(monster.missChance.Value, monster.missChance.Value * GetBuff(minBuff, maxBuff));
            monster.resilience.Value = Math.Max(monster.resilience.Value, (int)(monster.resilience.Value * GetBuff(minBuff, maxBuff)));
            monster.Health = Math.Max(monster.Health, (int)(monster.Health * GetBuff(minBuff, maxBuff)));
            monster.DamageToFarmer = Math.Max(monster.DamageToFarmer, (int)(monster.DamageToFarmer * GetBuff(minBuff, maxBuff)));
        }

        private float GetBuff(float minBuff, float maxBuff)
        {
            return Math.Max(1.0f, (float)((Game1.random.NextDouble() * (maxBuff - minBuff)) + minBuff));
        }

        private bool CanHazMonster(MonsterDecider which)
        {
            return deepWoods.level.Value >= which.MinLevel && this.random.CheckChance(which.Chance);
        }

        private string GetRockCrabType()
        {
            return "Iridium Crab";
        }

        private int GetSlimeLevel()
        {
            if (CanHazMonster(Settings.Monsters.PurpleSlime))
            {
                return 121;
            }
            else if (Game1.currentSeason == "winter" && this.random.CheckChance(Chance.FIFTY_FIFTY))
            {
                return 79;
            }
            else
            {
                return 39;
            }
        }
    }
}
