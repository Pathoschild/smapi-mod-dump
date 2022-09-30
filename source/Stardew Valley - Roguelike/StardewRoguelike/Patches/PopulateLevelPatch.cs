/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewRoguelike.Extensions;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Reflection;

namespace StardewRoguelike.Patches
{
    internal class PopulateLevelPatch : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(MineShaft), "populateLevel");

        public static bool Prefix(MineShaft __instance)
        {
            __instance.loadObjects();  // Loads trees for the egg hunt floor

            if (!__instance.IsNormalFloor())
                return false;

            Random mineRandom = (Random)__instance.GetType().GetField("mineRandom", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance);

            __instance.GetType().GetField("ghostAdded", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(__instance, false);
            int stonesLeftOnThisLevel = 0;

            __instance.objects.Clear();
            __instance.terrainFeatures.Clear();
            __instance.resourceClumps.Clear();
            __instance.debris.Clear();
            __instance.characters.Clear();

            int level = Roguelike.GetLevelFromMineshaft(__instance);

            int maxMonsters = level > Roguelike.ScalingOrder[^1] ? Roguelike.MaximumMonstersPerFloorPostLoop : Roguelike.MaximumMonstersPerFloorPreLoop;
            if (Curse.AnyFarmerHasCurse(CurseType.MoreEnemiesLessHealth))
                maxMonsters *= 2;

            double stoneChance;
            if (DebugCommands.ForcedStoneChance > 0f)
                stoneChance = DebugCommands.ForcedStoneChance;
            else
            {
                stoneChance = mineRandom.Next(10, 25) / 100.0;
                stoneChance += Game1.player.team.AverageLuckLevel() / 100.0;
            }

            double monsterChance;
            if (DebugCommands.ForcedMonsterChance > 0f)
                monsterChance = DebugCommands.ForcedMonsterChance;
            else
            {
                monsterChance = 0.022;
                if (Roguelike.HardMode)
                    monsterChance += 0.011;

                monsterChance += 0.01 * __instance.GetAdditionalDifficulty();
                monsterChance *= Game1.getOnlineFarmers().Count;

                if (Curse.AnyFarmerHasCurse(CurseType.MoreEnemiesLessHealth))
                    monsterChance *= 2;
            }

            double gemStoneChance;
            if (DebugCommands.ForcedGemChance > 0f)
                gemStoneChance = DebugCommands.ForcedGemChance;
            else
                gemStoneChance = 0.005 + (Game1.player.team.AverageLuckLevel() / 200.0);

            int monstersSpawned = 0;

            int barrelsAdded = 0;
            bool firstTime = true;
            if (mineRandom.NextDouble() < 0.55 + (Game1.player.team.AverageLuckLevel() / 100))
            {
                int numBarrels = mineRandom.Next(6);
                numBarrels += (int)Math.Ceiling(Game1.player.team.AverageLuckLevel() * numBarrels);
                for (int k = 0; k < numBarrels; k++)
                {
                    Point p;
                    Point motion;
                    if (mineRandom.NextDouble() < 0.33)
                    {
                        p = new Point(mineRandom.Next(__instance.map.GetLayer("Back").LayerWidth), 0);
                        motion = new Point(0, 1);
                    }
                    else if (mineRandom.NextDouble() < 0.5)
                    {
                        p = new Point(0, mineRandom.Next(__instance.map.GetLayer("Back").LayerHeight));
                        motion = new Point(1, 0);
                    }
                    else
                    {
                        p = new Point(__instance.map.GetLayer("Back").LayerWidth - 1, mineRandom.Next(__instance.map.GetLayer("Back").LayerHeight));
                        motion = new Point(-1, 0);
                    }
                    while (__instance.isTileOnMap(p.X, p.Y))
                    {
                        p.X += motion.X;
                        p.Y += motion.Y;
                        if (__instance.isTileClearForMineObjects(p.X, p.Y))
                        {
                            Vector2 objectPos5 = new(p.X, p.Y);
                            __instance.objects.Add(objectPos5, new BreakableContainer(objectPos5, 118, __instance));
                            break;
                        }
                    }
                }
            }

            for (int j = 0; j < __instance.map.GetLayer("Back").LayerWidth; j++)
            {
                for (int l = 0; l < __instance.map.GetLayer("Back").LayerHeight; l++)
                {
                    __instance.checkForMapAlterations(j, l);
                    MethodInfo canAdd = __instance.GetType().GetMethod("canAdd");
                    if (__instance.isTileClearForMineObjects(j, l))
                    {
                        if (mineRandom.NextDouble() <= stoneChance)
                        {
                            Vector2 objectPos4 = new(j, l);
                            if (__instance.Objects.ContainsKey(objectPos4))
                                continue;

                            StardewValley.Object stone = null;
                            if (gemStoneChance != 0.0 && mineRandom.NextDouble() < gemStoneChance + gemStoneChance + __instance.mineLevel / 24000.0)
                            {
                                stone = new(objectPos4, __instance.getRandomGemRichStoneForThisLevel(__instance.mineLevel), "Stone", canBeSetDown: true, canBeGrabbed: false, isHoedirt: false, isSpawnedObject: false)
                                {
                                    MinutesUntilReady = 5
                                };
                            }
                            if (stone is not null)
                            {
                                __instance.Objects.Add(objectPos4, stone);
                                stonesLeftOnThisLevel++;
                            }
                        }

                        // MONSTER SPAWNING

                        else if (mineRandom.NextDouble() <= monsterChance && __instance.getDistanceFromStart(j, l) > 5f && monstersSpawned < maxMonsters)
                        {
                            Monster monsterToAdd = __instance.BuffMonsterIfNecessary(__instance.getMonsterForThisLevel(__instance.mineLevel, j, l));
                            if (monsterToAdd is GreenSlime && __instance.GetAdditionalDifficulty() > 0 && mineRandom.NextDouble() < Math.Min(__instance.GetAdditionalDifficulty() * 0.1f, 0.5f))
                            {
                                if (mineRandom.NextDouble() < 0.001)
                                    (monsterToAdd as GreenSlime).stackedSlimes.Value = 4;
                                else
                                    (monsterToAdd as GreenSlime).stackedSlimes.Value = 2;
                            }
                            if (monsterToAdd is Leaper)
                            {
                                float partner_chance = (__instance.GetAdditionalDifficulty() + 1) * 0.3f;
                                if (mineRandom.NextDouble() < (double)partner_chance && monstersSpawned < maxMonsters)
                                    monstersSpawned += __instance.TryToAddMonster(__instance.BuffMonsterIfNecessary(new Leaper(Vector2.Zero)), j - 1, l) ? 1 : 0;
                                if (mineRandom.NextDouble() < (double)partner_chance && monstersSpawned < maxMonsters)
                                    monstersSpawned += __instance.TryToAddMonster(__instance.BuffMonsterIfNecessary(new Leaper(Vector2.Zero)), j + 1, l) ? 1 : 0;
                                if (mineRandom.NextDouble() < (double)partner_chance && monstersSpawned < maxMonsters)
                                    monstersSpawned += __instance.TryToAddMonster(__instance.BuffMonsterIfNecessary(new Leaper(Vector2.Zero)), j, l - 1) ? 1 : 0;
                                if (mineRandom.NextDouble() < (double)partner_chance && monstersSpawned < maxMonsters)
                                    monstersSpawned += __instance.TryToAddMonster(__instance.BuffMonsterIfNecessary(new Leaper(Vector2.Zero)), j, l + 1) ? 1 : 0;
                            }
                            if (monsterToAdd is Grub)
                            {
                                if (mineRandom.NextDouble() < 0.4 && monstersSpawned < maxMonsters)
                                    monstersSpawned += __instance.TryToAddMonster(__instance.BuffMonsterIfNecessary(new Grub(Vector2.Zero)), j - 1, l) ? 1 : 0;
                                if (mineRandom.NextDouble() < 0.4 && monstersSpawned < maxMonsters)
                                    monstersSpawned += __instance.TryToAddMonster(__instance.BuffMonsterIfNecessary(new Grub(Vector2.Zero)), j + 1, l) ? 1 : 0;
                                if (mineRandom.NextDouble() < 0.4 && monstersSpawned < maxMonsters)
                                    monstersSpawned += __instance.TryToAddMonster(__instance.BuffMonsterIfNecessary(new Grub(Vector2.Zero)), j, l - 1) ? 1 : 0;
                                if (mineRandom.NextDouble() < 0.4 && monstersSpawned < maxMonsters)
                                    monstersSpawned += __instance.TryToAddMonster(__instance.BuffMonsterIfNecessary(new Grub(Vector2.Zero)), j, l + 1) ? 1 : 0;
                            }
                            else if (monsterToAdd is DustSpirit)
                            {
                                if (mineRandom.NextDouble() < 0.6 && monstersSpawned < maxMonsters)
                                    monstersSpawned += __instance.TryToAddMonster(__instance.BuffMonsterIfNecessary(new DustSpirit(Vector2.Zero)), j - 1, l) ? 1 : 0;
                                if (mineRandom.NextDouble() < 0.6 && monstersSpawned < maxMonsters)
                                    monstersSpawned += __instance.TryToAddMonster(__instance.BuffMonsterIfNecessary(new DustSpirit(Vector2.Zero)), j + 1, l) ? 1 : 0;
                                if (mineRandom.NextDouble() < 0.6 && monstersSpawned < maxMonsters)
                                    monstersSpawned += __instance.TryToAddMonster(__instance.BuffMonsterIfNecessary(new DustSpirit(Vector2.Zero)), j, l - 1) ? 1 : 0;
                                if (mineRandom.NextDouble() < 0.6 && monstersSpawned < maxMonsters)
                                    monstersSpawned += __instance.TryToAddMonster(__instance.BuffMonsterIfNecessary(new DustSpirit(Vector2.Zero)), j, l + 1) ? 1 : 0;
                            }
                            if (monsterToAdd.GetBoundingBox().Width > 64 && !__instance.isTileClearForMineObjects(j + 1, l))
                                continue;

                            monsterToAdd.hasSpecialItem.Value = false;

                            Roguelike.AdjustMonster(__instance, ref monsterToAdd);

                            if (level >= Roguelike.DangerousThreshold)
                                monsterToAdd.DamageToFarmer = (int)Math.Round(monsterToAdd.DamageToFarmer * 1.75f);

                            monstersSpawned++;
                            __instance.characters.Add(monsterToAdd);
                        }

                        // RESOURCE CLUMP SPAWNING

                        else if (mineRandom.NextDouble() <= 0.005 && !__instance.isDarkArea() && !__instance.mustKillAllMonstersToAdvance() && (__instance.GetAdditionalDifficulty() <= 0 || (__instance.getMineArea() == 40 && __instance.mineLevel % 40 < 30)))
                        {
                            if (!__instance.isTileClearForMineObjects(j + 1, l) || !__instance.isTileClearForMineObjects(j, l + 1) || !__instance.isTileClearForMineObjects(j + 1, l + 1))
                            {
                                continue;
                            }
                            Vector2 objectPos2 = new(j, l);
                            int whichClump = ((mineRandom.NextDouble() < 0.5) ? 752 : 754);
                            int mineArea = __instance.getMineArea();
                            if (mineArea == 40)
                            {
                                if (__instance.GetAdditionalDifficulty() > 0)
                                {
                                    whichClump = 600;
                                    if (mineRandom.NextDouble() < 0.1)
                                        whichClump = 602;
                                }
                                else
                                {
                                    whichClump = ((mineRandom.NextDouble() < 0.5) ? 756 : 758);
                                }
                            }
                            __instance.resourceClumps.Add(new ResourceClump(whichClump, 2, 2, objectPos2));
                        }

                        // ADDITIONAL DIFFICULTY STUFF

                        else if (__instance.GetAdditionalDifficulty() > 0)
                        {
                            if (__instance.getMineArea() == 40 && __instance.mineLevel % 40 < 30 && mineRandom.NextDouble() < 0.01 && __instance.getTileIndexAt(j, l - 1, "Buildings") != -1)
                                __instance.terrainFeatures.Add(new(j, l), new Tree(8, 5));
                            else if (__instance.getMineArea() == 40 && __instance.mineLevel % 40 < 30 && mineRandom.NextDouble() < 0.1 && (__instance.getTileIndexAt(j, l - 1, "Buildings") != -1 || __instance.getTileIndexAt(j - 1, l, "Buildings") != -1 || __instance.getTileIndexAt(j, l + 1, "Buildings") != -1 || __instance.getTileIndexAt(j + 1, l, "Buildings") != -1 || __instance.terrainFeatures.ContainsKey(new Vector2(j - 1, l)) || __instance.terrainFeatures.ContainsKey(new Vector2(j + 1, l)) || __instance.terrainFeatures.ContainsKey(new Vector2(j, l - 1)) || __instance.terrainFeatures.ContainsKey(new Vector2(j, l + 1))))
                                __instance.terrainFeatures.Add(new(j, l), new Grass((__instance.mineLevel >= 50) ? 6 : 5, (__instance.mineLevel >= 50) ? 1 : mineRandom.Next(1, 5)));
                            else if (__instance.getMineArea() == 80 && !__instance.isDarkArea() && mineRandom.NextDouble() < 0.1 && (__instance.getTileIndexAt(j, l - 1, "Buildings") != -1 || __instance.getTileIndexAt(j - 1, l, "Buildings") != -1 || __instance.getTileIndexAt(j, l + 1, "Buildings") != -1 || __instance.getTileIndexAt(j + 1, l, "Buildings") != -1 || __instance.terrainFeatures.ContainsKey(new Vector2(j - 1, l)) || __instance.terrainFeatures.ContainsKey(new Vector2(j + 1, l)) || __instance.terrainFeatures.ContainsKey(new Vector2(j, l - 1)) || __instance.terrainFeatures.ContainsKey(new Vector2(j, l + 1))))
                                __instance.terrainFeatures.Add(new(j, l), new Grass(4, mineRandom.Next(1, 5)));
                        }
                    }

                    // BREAKABLES SPAWNING (barrels, crates, etc)

                    else if (__instance.isContainerPlatform(j, l) && __instance.isTileLocationTotallyClearAndPlaceable(j, l) && mineRandom.NextDouble() < 0.4 && (firstTime || (bool)canAdd.Invoke(__instance, new object[] { 0, barrelsAdded })))
                    {
                        Vector2 objectPos = new(j, l);
                        __instance.objects.Add(objectPos, new BreakableContainer(objectPos, 118, __instance));
                        barrelsAdded++;
                        if (firstTime)
                            __instance.updateMineLevelData(0);
                    }
                }
            }

            if (monstersSpawned < Roguelike.MinimumMonstersPerFloor)
            {
                int monstersToSpawn = Roguelike.MinimumMonstersPerFloor - monstersSpawned;
                __instance.SpawnMonsters(monstersToSpawn);
            }

            // UNIQUE FEATURES BASED ON MINE LEVEL
            __instance.tryToAddAreaUniques();

            // SPAWN LADDER IF NO ENEMIES WERE SPAWNED
            if (__instance.mustKillAllMonstersToAdvance() && __instance.EnemyCount <= 1)
            {
                Vector2 tileBeneathLadder = (Vector2)__instance.GetType().GetField("tileBeneathLadder", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance);
                __instance.characters.Add(new Bat(tileBeneathLadder * 64f + new Vector2(256f, 256f)));
            }

            __instance.tryToAddOreClumps();
            __instance.GetType().GetProperty("stonesLeftOnThisLevel", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(__instance, stonesLeftOnThisLevel);

            return false;
        }
    }
}
