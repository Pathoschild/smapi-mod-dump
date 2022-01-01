/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/BattleRoyalley-Year2
**
*************************************************/

using BattleRoyale.Utils;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;

namespace BattleRoyale
{
    class BombRadiusChange : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(GameLocation), "explode");

        public static bool Prefix(GameLocation __instance, Vector2 tileLocation, int radius, Farmer who)
        {
            if (!ModEntry.BRGame.InProgress)
                return true;

            radius -= 1;
            int damage_amount = Game1.random.Next(40, 60);

            bool insideCircle = false;
            __instance.updateMap();
            Vector2 currentTile = new(Math.Min(__instance.map.Layers[0].LayerWidth - 1, Math.Max(0f, tileLocation.X - (float)radius)), Math.Min(__instance.map.Layers[0].LayerHeight - 1, Math.Max(0f, tileLocation.Y - (float)radius)));
            bool[,] circleOutline = Game1.getCircleOutlineGrid(radius);
            Rectangle areaOfEffect = new((int)(tileLocation.X - (float)radius - 1f) * 64, (int)(tileLocation.Y - (float)radius - 1f) * 64, (radius * 2 + 1) * 64, (radius * 2 + 1) * 64);
            if (damage_amount > 0)
            {
                __instance.damageMonster(areaOfEffect, damage_amount, damage_amount, isBomb: true, who);
            }
            else
            {
                __instance.damageMonster(areaOfEffect, radius * 6, radius * 8, isBomb: true, who);
            }
            List<TemporaryAnimatedSprite> sprites = new()
            {
                new TemporaryAnimatedSprite(23, 9999f, 6, 1, new Vector2(currentTile.X * 64f, currentTile.Y * 64f), flicker: false, Game1.random.NextDouble() < 0.5)
                {
                    light = true,
                    lightRadius = radius,
                    lightcolor = Color.Black,
                    alphaFade = 0.03f - radius * 0.003f,
                    Parent = __instance
                }
            };
            ModEntry.BRGame.Helper.Reflection.GetMethod(__instance, "rumbleAndFade").Invoke(300 + radius * 100);

            Round round = ModEntry.BRGame.GetActiveRound();
            if (round != null)
            {
                foreach (Farmer farmer in __instance.farmers)
                {
                    if (farmer.GetBoundingBox().Intersects(areaOfEffect) && !farmer.onBridge.Value)
                        FarmerUtils.TakeDamage(farmer, DamageSource.PLAYER, damage_amount, who.UniqueMultiplayerID);
                }
            }

            for (int k = __instance.terrainFeatures.Count() - 1; k >= 0; k--)
            {
                KeyValuePair<Vector2, TerrainFeature> n = __instance.terrainFeatures.Pairs.ElementAt(k);
                if (n.Value.getBoundingBox(n.Key).Intersects(areaOfEffect) && n.Value.performToolAction(null, radius / 2, n.Key, __instance))
                {
                    __instance.terrainFeatures.Remove(n.Key);
                }
            }
            for (int j = 0; j < radius * 2 + 1; j++)
            {
                for (int l = 0; l < radius * 2 + 1; l++)
                {
                    if (j == 0 || l == 0 || j == radius * 2 || l == radius * 2)
                    {
                        insideCircle = circleOutline[j, l];
                    }
                    else if (circleOutline[j, l])
                    {
                        insideCircle = !insideCircle;
                        if (!insideCircle)
                        {
                            if (__instance.objects.ContainsKey(currentTile) && __instance.objects[currentTile].onExplosion(who, __instance))
                            {
                                __instance.destroyObject(currentTile, who);
                            }
                            if (Game1.random.NextDouble() < 0.45)
                            {
                                if (Game1.random.NextDouble() < 0.5)
                                {
                                    sprites.Add(new TemporaryAnimatedSprite(362, Game1.random.Next(30, 90), 6, 1, new Vector2(currentTile.X * 64f, currentTile.Y * 64f), flicker: false, Game1.random.NextDouble() < 0.5)
                                    {
                                        delayBeforeAnimationStart = Game1.random.Next(700)
                                    });
                                }
                                else
                                {
                                    sprites.Add(new TemporaryAnimatedSprite(5, new Vector2(currentTile.X * 64f, currentTile.Y * 64f), Color.White, 8, flipped: false, 50f)
                                    {
                                        delayBeforeAnimationStart = Game1.random.Next(200),
                                        scale = (float)Game1.random.Next(5, 15) / 10f
                                    });
                                }
                            }
                        }
                    }
                    if (insideCircle)
                    {
                        __instance.explosionAt(currentTile.X, currentTile.Y);
                        if (__instance.objects.ContainsKey(currentTile) && __instance.objects[currentTile].onExplosion(who, __instance))
                        {
                            __instance.destroyObject(currentTile, who);
                        }
                        if (Game1.random.NextDouble() < 0.45)
                        {
                            if (Game1.random.NextDouble() < 0.5)
                            {
                                sprites.Add(new TemporaryAnimatedSprite(362, Game1.random.Next(30, 90), 6, 1, new Vector2(currentTile.X * 64f, currentTile.Y * 64f), flicker: false, Game1.random.NextDouble() < 0.5)
                                {
                                    delayBeforeAnimationStart = Game1.random.Next(700)
                                });
                            }
                            else
                            {
                                sprites.Add(new TemporaryAnimatedSprite(5, new Vector2(currentTile.X * 64f, currentTile.Y * 64f), Color.White, 8, flipped: false, 50f)
                                {
                                    delayBeforeAnimationStart = Game1.random.Next(200),
                                    scale = (float)Game1.random.Next(5, 15) / 10f
                                });
                            }
                        }
                        sprites.Add(new TemporaryAnimatedSprite(6, new Vector2(currentTile.X * 64f, currentTile.Y * 64f), Color.White, 8, Game1.random.NextDouble() < 0.5, Vector2.Distance(currentTile, tileLocation) * 20f));
                    }
                    currentTile.Y += 1f;
                    currentTile.Y = Math.Min(__instance.map.Layers[0].LayerHeight - 1, Math.Max(0f, currentTile.Y));
                }
                currentTile.X += 1f;
                currentTile.Y = Math.Min(__instance.map.Layers[0].LayerWidth - 1, Math.Max(0f, currentTile.X));
                currentTile.Y = tileLocation.Y - (float)radius;
                currentTile.Y = Math.Min(__instance.map.Layers[0].LayerHeight - 1, Math.Max(0f, currentTile.Y));
            }
            ModEntry.BRGame.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue().broadcastSprites(__instance, sprites);
            radius /= 2;
            circleOutline = Game1.getCircleOutlineGrid(radius);
            currentTile = new Vector2((int)(tileLocation.X - (float)radius), (int)(tileLocation.Y - (float)radius));
            for (int i = 0; i < radius * 2 + 1; i++)
            {
                for (int m = 0; m < radius * 2 + 1; m++)
                {
                    if (i == 0 || m == 0 || i == radius * 2 || m == radius * 2)
                    {
                        insideCircle = circleOutline[i, m];
                    }
                    else if (circleOutline[i, m])
                    {
                        insideCircle = !insideCircle;
                        if (!insideCircle && !__instance.objects.ContainsKey(currentTile) && Game1.random.NextDouble() < 0.9 && __instance.doesTileHaveProperty((int)currentTile.X, (int)currentTile.Y, "Diggable", "Back") != null && !__instance.isTileHoeDirt(currentTile))
                        {
                            __instance.checkForBuriedItem((int)currentTile.X, (int)currentTile.Y, explosion: true, detectOnly: false, who);
                            __instance.makeHoeDirt(currentTile);
                        }
                    }
                    if (insideCircle && !__instance.objects.ContainsKey(currentTile) && Game1.random.NextDouble() < 0.9 && __instance.doesTileHaveProperty((int)currentTile.X, (int)currentTile.Y, "Diggable", "Back") != null && !__instance.isTileHoeDirt(currentTile))
                    {
                        __instance.checkForBuriedItem((int)currentTile.X, (int)currentTile.Y, explosion: true, detectOnly: false, who);
                        __instance.makeHoeDirt(currentTile);
                    }
                    currentTile.Y += 1f;
                    currentTile.Y = Math.Min(__instance.map.Layers[0].LayerHeight - 1, Math.Max(0f, currentTile.Y));
                }
                currentTile.X += 1f;
                currentTile.Y = Math.Min(__instance.map.Layers[0].LayerWidth - 1, Math.Max(0f, currentTile.X));
                currentTile.Y = tileLocation.Y - radius;
                currentTile.Y = Math.Min(__instance.map.Layers[0].LayerHeight - 1, Math.Max(0f, currentTile.Y));
            }

            return false;
        }
    }
}
