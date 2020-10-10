/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/pomepome/UsefulBombs
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace UsefulBombs
{
    internal class ModEntry : Mod
    {
        internal static IReflectionHelper Reflection { get; private set; }
        internal static ModConfig Config { get; private set; }
        internal static IMonitor Mon { get; private set; }

        public override void Entry(IModHelper helper)
        {
            Reflection = helper.Reflection;
            Config = helper.ReadConfig<ModConfig>();
            Mon = Monitor;

            Cap(ref Config.DamageMultiplier, 1, 3);
            Cap(ref Config.RadiusIncreaseRatio, 0.1f, 0.5f);
            Helper.WriteConfig(Config);

            HarmonyInstance harmony = HarmonyInstance.Create("punyo.usefulbombs");
            MethodInfo methodBase = typeof(GameLocation).GetMethod("explode", BindingFlags.Public | BindingFlags.Instance);
            MethodInfo methodPatcher = typeof(GameLocationPatcher).GetMethod("Prefix", BindingFlags.NonPublic | BindingFlags.Static);
            if (methodBase == null)
            {
                Monitor.Log("Original method null, what's wrong?");
                return;
            }
            if (methodPatcher == null)
            {
                Monitor.Log("Patcher null, what's wrong?");
                return;
            }
            harmony.Patch(methodBase, new HarmonyMethod(methodPatcher), null);
            Monitor.Log($"Patched {methodBase.DeclaringType?.FullName}.{methodBase.Name} by {methodPatcher.DeclaringType?.FullName}.{methodPatcher.Name}");
        }

        private static void Cap(ref float f, float min, float max)
        {
            if (f < min)
            {
                f = min;
            }
            else if (f > max)
            {
                f = max;
            }
        }
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    internal class GameLocationPatcher
    {
        internal static bool Prefix(ref GameLocation __instance, ref Vector2 tileLocation, ref int radius,  ref Farmer who)
        {
            Explode(__instance, ModEntry.Config, tileLocation, radius, who);
            return false;
        }

        internal static void Explode(GameLocation location, ModConfig config, Vector2 tileLocation, int radius, Farmer who)
        {
            if (config.LargerRadius)
            {
                int oRadius = radius;
                radius = (int) (radius * (1 + config.RadiusIncreaseRatio));
                ModEntry.Mon.Log($"Radius changed from {oRadius} to {radius}", LogLevel.Trace);
            }
            bool flag = false;
            IReflectedMethod rumbleAndFade = ModEntry.Reflection.GetMethod(location, "rumbleAndFade");
            IReflectedMethod damagePlayers = ModEntry.Reflection.GetMethod(location, "damagePlayers");
            Multiplayer multiplayer = ModEntry.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
            location.updateMap();
            Vector2 vector = new Vector2(Math.Min(location.map.Layers[0].LayerWidth - 1, Math.Max(0f, tileLocation.X - radius)), Math.Min(location.Map.Layers[0].LayerHeight - 1, Math.Max(0f, tileLocation.Y - radius)));
            bool[,] circleOutlineGrid = Game1.getCircleOutlineGrid(radius);
            Rectangle rectangle = new Rectangle((int)(tileLocation.X - radius - 1f) * 64, (int)(tileLocation.Y - radius - 1f) * 64, (radius * 2 + 1) * 64, (radius * 2 + 1) * 64);
            float minDamage = radius * 6, maxDamage = radius * 8;
            if (config.ModifyDamagesToEnemies)
            {
                minDamage *= config.DamageMultiplier;
                maxDamage *= config.DamageMultiplier;
            }
            location.damageMonster(rectangle, (int)minDamage, (int)maxDamage, true, who);
            List<TemporaryAnimatedSprite> list1 = new List<TemporaryAnimatedSprite>
            {
                new TemporaryAnimatedSprite(23, 9999f, 6, 1, new Vector2(vector.X * 64f, vector.Y * 64f), false,
                    Game1.random.NextDouble() < 0.5)
                {
                    light = true,
                    lightRadius = radius,
                    lightcolor = Color.Black,
                    alphaFade = 0.03f - radius * 0.003f
                }
            };
            List<TemporaryAnimatedSprite> list = list1;

            rumbleAndFade?.Invoke(300 + radius * 100);
            damagePlayers?.Invoke(rectangle, radius * 3);

            for (int num = location.terrainFeatures.Count() - 1; num >= 0; num--)
            {
                KeyValuePair<Vector2, TerrainFeature> keyValuePair = location.terrainFeatures.Pairs.ElementAt(num);
                if (keyValuePair.Value.getBoundingBox(keyValuePair.Key).Intersects(rectangle) && keyValuePair.Value.performToolAction(null, radius / 2, keyValuePair.Key, location))
                {
                    location.terrainFeatures.Remove(keyValuePair.Key);
                }
            }

            if (config.BreakBoulders && location is MineShaft shaft)
            {
                for (int num = shaft.resourceClumps.Count - 1; num >= 0; num--)
                {
                    ResourceClump terrain = shaft.resourceClumps[num];
                    switch (terrain.parentSheetIndex.Value)
                    {
                        case 672:
                        case 752:
                        case 754:
                        case 756:
                        case 758: break;
                        default:   continue;
                    }
                    Vector2 vec = terrain.tile.Value;
                    if (terrain.getBoundingBox(vec).Intersects(rectangle))
                    {
                        int number = (terrain.parentSheetIndex.Value == 672) ? 15 : 10;
                        if (Game1.IsMultiplayer)
                        {
                            Game1.createMultipleObjectDebris(390, (int)vec.X, (int)vec.Y, number, Game1.player.UniqueMultiplayerID);
                        }
                        else
                        {
                            Game1.createRadialDebris(Game1.currentLocation, 390, (int)vec.X, (int)vec.Y, number, false, -1, true);
                        }
                        location.playSound("boulderBreak");
                        Game1.createRadialDebris(Game1.currentLocation, 32, (int)vec.X, (int)vec.Y, Game1.random.Next(6, 12), false);
                        shaft.resourceClumps.RemoveAt(num);
                    }
                }
            }
            for (int i = 0; i < radius * 2 + 1; i++)
            {
                for (int j = 0; j < radius * 2 + 1; j++)
                {
                    if (i == 0 || j == 0 || i == radius * 2 || j == radius * 2)
                    {
                        flag = circleOutlineGrid[i, j];
                    }
                    else if (circleOutlineGrid[i, j])
                    {
                        flag = !flag;
                        if (!flag)
                        {
                            if (location.Objects.ContainsKey(vector) && location.Objects[vector].onExplosion(who, location))
                            {
                                if (config.CollectCrystals && location.Objects[vector].CanBeGrabbed)
                                {
                                    Game1.createObjectDebris(location.Objects[vector].ParentSheetIndex, (int)vector.X, (int)vector.Y);
                                }
                                location.destroyObject(vector, who);
                            }
                            if (Game1.random.NextDouble() < 0.45)
                            {
                                if (Game1.random.NextDouble() < 0.5)
                                {
                                    list.Add(new TemporaryAnimatedSprite(362, Game1.random.Next(30, 90), 6, 1, new Vector2(vector.X * 64f, vector.Y * 64f), false, Game1.random.NextDouble() < 0.5)
                                    {
                                        delayBeforeAnimationStart = Game1.random.Next(700)
                                    });
                                }
                                else
                                {
                                    list.Add(new TemporaryAnimatedSprite(5, new Vector2(vector.X * 64f, vector.Y * 64f), Color.White, 8, false, 50f)
                                    {
                                        delayBeforeAnimationStart = Game1.random.Next(200),
                                        scale = Game1.random.Next(5, 15) / 10f
                                    });
                                }
                            }
                        }
                    }
                    if (flag)
                    {
                        if (location.Objects.ContainsKey(vector) && location.Objects[vector].onExplosion(who, location))
                        {
                            if (config.CollectCrystals && location.Objects.ContainsKey(vector) && location.Objects[vector].CanBeGrabbed)
                            {
                                Game1.createObjectDebris(location.Objects[vector].ParentSheetIndex, (int)vector.X, (int)vector.Y);
                            }
                            location.destroyObject(vector, who);
                        }
                        if (Game1.random.NextDouble() < 0.45)
                        {
                            if (Game1.random.NextDouble() < 0.5)
                            {
                                list.Add(new TemporaryAnimatedSprite(362, Game1.random.Next(30, 90), 6, 1, new Vector2(vector.X * 64f, vector.Y * 64f), false, Game1.random.NextDouble() < 0.5)
                                {
                                    delayBeforeAnimationStart = Game1.random.Next(700)
                                });
                            }
                            else
                            {
                                list.Add(new TemporaryAnimatedSprite(5, new Vector2(vector.X * 64f, vector.Y * 64f), Color.White, 8, false, 50f)
                                {
                                    delayBeforeAnimationStart = Game1.random.Next(200),
                                    scale = Game1.random.Next(5, 15) / 10f
                                });
                            }
                        }
                        list.Add(new TemporaryAnimatedSprite(6, new Vector2(vector.X * 64f, vector.Y * 64f), Color.White, 8, Game1.random.NextDouble() < 0.5, Vector2.Distance(vector, tileLocation) * 20f));
                    }
                    vector.Y += 1f;
                    vector.Y = Math.Min(location.Map.Layers[0].LayerHeight - 1, Math.Max(0f, vector.Y));
                }
                vector.X += 1f;
                vector.Y = Math.Min(location.Map.Layers[0].LayerWidth - 1, Math.Max(0f, vector.X));
                vector.Y = tileLocation.Y - radius;
                vector.Y = Math.Min(location.Map.Layers[0].LayerHeight - 1, Math.Max(0f, vector.Y));
            }
            multiplayer.broadcastSprites(location, list);
            radius /= 2;
            circleOutlineGrid = Game1.getCircleOutlineGrid(radius);
            vector = new Vector2((int)(tileLocation.X - radius), (int)(tileLocation.Y - radius));
            for (int k = 0; k < radius * 2 + 1; k++)
            {
                for (int l = 0; l < radius * 2 + 1; l++)
                {
                    if (k == 0 || l == 0 || k == radius * 2 || l == radius * 2)
                    {
                        flag = circleOutlineGrid[k, l];
                    }
                    else if (circleOutlineGrid[k, l])
                    {
                        flag = !flag;
                        if (!flag && !location.Objects.ContainsKey(vector) && Game1.random.NextDouble() < 0.9 && location.doesTileHaveProperty((int)vector.X, (int)vector.Y, "Diggable", "Back") != null && !location.isTileHoeDirt(vector))
                        {
                            location.checkForBuriedItem((int)vector.X, (int)vector.Y, true, false);
                            location.makeHoeDirt(vector);
                        }
                    }
                    if (flag && !location.Objects.ContainsKey(vector) && Game1.random.NextDouble() < 0.9 && location.doesTileHaveProperty((int)vector.X, (int)vector.Y, "Diggable", "Back") != null && !location.isTileHoeDirt(vector))
                    {
                        location.checkForBuriedItem((int)vector.X, (int)vector.Y, true, false);
                        location.makeHoeDirt(vector);
                    }
                    vector.Y += 1f;
                    vector.Y = Math.Min(location.Map.Layers[0].LayerHeight - 1, Math.Max(0f, vector.Y));
                }
                vector.X += 1f;
                vector.Y = Math.Min(location.Map.Layers[0].LayerWidth - 1, Math.Max(0f, vector.X));
                vector.Y = tileLocation.Y - radius;
                vector.Y = Math.Min(location.Map.Layers[0].LayerHeight - 1, Math.Max(0f, vector.Y));
            }
        }
    }
}
