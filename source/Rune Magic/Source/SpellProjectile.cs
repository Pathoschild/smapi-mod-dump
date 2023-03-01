/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/facufierro/RuneMagic
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using SpaceCore;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using xTile;

namespace RuneMagic.Source
{
    public class SpellProjectile : Projectile
    {
        public Farmer Source { get; set; }
        public Spell Spell { get; set; }
        public int MinDamage { get; set; }
        public int MaxDamage { get; set; }
        public int BonusDamage { get; set; }
        public int Area { get; set; }
        public float Direction { get; set; }
        public float Velocity { get; set; }
        public Texture2D Texture { get; set; }
        public bool Homing { get; set; }
        public Vector2 Target { get; set; }

        private readonly NetEvent1Field<int, NetInt> rumbleAndFadeEvent = new NetEvent1Field<int, NetInt>();

        public SpellProjectile(Texture2D texture, int minDamage, int maxDamage, int bonusDamage, int area, int velocity, bool homing)
        {
            Source = Game1.player;
            Homing = homing;
            MinDamage = minDamage;
            MaxDamage = maxDamage;
            BonusDamage = bonusDamage;
            Area = area;
            Texture = texture;

            theOneWhoFiredMe.Set(Source.currentLocation, Source);
            damagesMonsters.Value = true;

            position.Value = Source.getStandingPosition();
            position.X -= Source.GetBoundingBox().Width / 1.5f;
            position.Y -= (Source.GetBoundingBox().Height) * 2;

            Velocity = velocity;
            Direction = (float)Math.Atan2(Game1.currentCursorTile.Y * 64 - position.Y, Game1.currentCursorTile.X * 64 - position.X);
            Target = Game1.currentCursorTile;
        }

        public override bool update(GameTime time, GameLocation location)
        {
            Velocity += 0.1f;
            if (Homing)
            {
                var monsters = Game1.currentLocation.characters.OfType<Monster>().ToList();
                var closestMonster = monsters.OrderBy(m => Vector2.Distance(m.position, position)).FirstOrDefault();

                if (closestMonster != null)
                {
                    var monsterCenter = new Vector2(closestMonster.position.X - closestMonster.GetBoundingBox().Width / 4, closestMonster.position.Y - closestMonster.GetBoundingBox().Height / 4);
                    Direction = (float)Math.Atan2(monsterCenter.Y - position.Y, monsterCenter.X - position.X);
                }
            }
            if (Area != 0)
            {
                Direction = (float)Math.Atan2(Target.Y * 64 - position.Y, Target.X * 64 - position.X);
                if (Vector2.Distance(new Vector2(Target.X * 64, Target.Y * 64), position) < 10)
                {
                    Explode(Target, Area);
                    destroyMe = true;
                    return true;
                }
            }

            position.X += (float)Math.Cos(Direction) * Velocity;
            position.Y += (float)Math.Sin(Direction) * Velocity;
            return base.update(time, location);
        }

        public override void draw(SpriteBatch b)
        {
            Vector2 drawPos = Game1.GlobalToLocal(new Vector2(getBoundingBox().X + getBoundingBox().Width / 2, getBoundingBox().Y + getBoundingBox().Height / 2));
            b.Draw(Texture, drawPos, new Rectangle(0, 0, Texture.Width, Texture.Height), Color.White, Direction, new Vector2(Texture.Width / 2, Texture.Height / 2), 2, SpriteEffects.None, (float)((position.Y + (double)(Game1.tileSize * 3 / 2)) / 10000.0));
        }

        public override void behaviorOnCollisionWithOther(GameLocation loc)
        {
            //if (!Homing)
            //    destroyMe = true;
        }

        public override void behaviorOnCollisionWithMineWall(int tileX, int tileY)
        {
            destroyMe = true;
        }

        public override void behaviorOnCollisionWithMonster(NPC n, GameLocation location)
        {
            if (n is not Monster)
                return;
            if (Area == 0)
                location.damageMonster(new Rectangle(n.GetBoundingBox().X, n.GetBoundingBox().Y, 64, 64), MinDamage + BonusDamage, MaxDamage + BonusDamage, false, -100, 100, 0, 0, false, Source);
            else
                Explode(Target, Area);
            destroyMe = true;
        }

        public override void behaviorOnCollisionWithPlayer(GameLocation location, Farmer player)
        {
        }

        public override void behaviorOnCollisionWithTerrainFeature(TerrainFeature t, Vector2 tileLocation, GameLocation location)
        {
            //if (!Homing)
            //    destroyMe = true;
        }

        public override void updatePosition(GameTime time)
        {
            position.X += (float)Math.Cos(Direction) * Velocity;
            position.Y += (float)Math.Sin(Direction) * Velocity;
        }

        public void Explode(Vector2 tileLocation, int radius)
        {
            bool flag = false;
            Game1.currentLocation.updateMap();
            Vector2 vector = new(Math.Min(Game1.currentLocation.map.Layers[0].LayerWidth - 1, Math.Max(0f, tileLocation.X - radius)), Math.Min(Game1.currentLocation.map.Layers[0].LayerHeight - 1, Math.Max(0f, tileLocation.Y - radius)));
            bool[,] circleOutlineGrid = Game1.getCircleOutlineGrid(radius);
            Rectangle rectangle = new((int)(tileLocation.X - radius) * 64, (int)(tileLocation.Y - radius) * 64, (radius * 2 + 1) * 64, (radius * 2 + 1) * 64);

            Game1.currentLocation.damageMonster(rectangle, MinDamage + BonusDamage, MaxDamage + BonusDamage, isBomb: true, Source);

            List<TemporaryAnimatedSprite> list = new()
            {
                new TemporaryAnimatedSprite(23, 9999f, 6, 1, new Vector2(vector.X * 64f, vector.Y * 64f), flicker: false, (Game1.random.NextDouble() < 0.5) ? true : false)
                {
                    light = true,
                    lightRadius = radius,
                    lightcolor = Color.Black,
                    alphaFade = 0.03f - (float)radius * 0.003f,
                    Parent = Game1.currentLocation
                }
            };

            rumbleAndFade(300 + radius * 100);

            for (int num = Game1.currentLocation.terrainFeatures.Count() - 1; num >= 0; num--)
            {
                KeyValuePair<Vector2, TerrainFeature> keyValuePair = Game1.currentLocation.terrainFeatures.Pairs.ElementAt(num);
                if (keyValuePair.Value.getBoundingBox(keyValuePair.Key).Intersects(rectangle) && keyValuePair.Value.performToolAction(null, radius / 2, keyValuePair.Key, Game1.currentLocation))
                {
                    Game1.currentLocation.terrainFeatures.Remove(keyValuePair.Key);
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
                            if (Game1.random.NextDouble() < 0.45)
                            {
                                if (Game1.random.NextDouble() < 0.5)
                                {
                                    list.Add(new TemporaryAnimatedSprite(362, Game1.random.Next(30, 90), 6, 1, new Vector2(vector.X * 64f, vector.Y * 64f), flicker: false, (Game1.random.NextDouble() < 0.5))
                                    {
                                        delayBeforeAnimationStart = Game1.random.Next(700)
                                    });
                                }
                                else
                                {
                                    list.Add(new TemporaryAnimatedSprite(5, new Vector2(vector.X * 64f, vector.Y * 64f), Color.OrangeRed, 8, flipped: false, 50f)
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
                        Game1.currentLocation.explosionAt(vector.X, vector.Y);

                        if (Game1.random.NextDouble() < 0.45)
                        {
                            if (Game1.random.NextDouble() < 0.5)
                            {
                                list.Add(new TemporaryAnimatedSprite(362, Game1.random.Next(30, 90), 6, 1, new Vector2(vector.X * 64f, vector.Y * 64f), flicker: false, (Game1.random.NextDouble() < 0.5))
                                {
                                    delayBeforeAnimationStart = Game1.random.Next(700)
                                });
                            }
                            else
                            {
                                list.Add(new TemporaryAnimatedSprite(5, new Vector2(vector.X * 64f, vector.Y * 64f), Color.OrangeRed, 8, flipped: false, 50f)
                                {
                                    delayBeforeAnimationStart = Game1.random.Next(200),
                                    scale = Game1.random.Next(5, 15) / 10f
                                });
                            }
                        }

                        list.Add(new TemporaryAnimatedSprite(6, new Vector2(vector.X * 64f, vector.Y * 64f), Color.OrangeRed, 8, Game1.random.NextDouble() < 0.5, Vector2.Distance(vector, tileLocation) * 20f));
                    }

                    vector.Y += 1f;
                    vector.Y = Math.Min(Game1.currentLocation.map.Layers[0].LayerHeight - 1, Math.Max(0f, vector.Y));
                }

                vector.X += 1f;
                vector.Y = Math.Min(Game1.currentLocation.map.Layers[0].LayerWidth - 1, Math.Max(0f, vector.X));
                vector.Y = tileLocation.Y - radius;
                vector.Y = Math.Min(Game1.currentLocation.map.Layers[0].LayerHeight - 1, Math.Max(0f, vector.Y));
            }

            broadcastSprites(Game1.currentLocation, list);
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
                        if (!flag && !Game1.currentLocation.objects.ContainsKey(vector) && Game1.random.NextDouble() < 0.9 && Game1.currentLocation.doesTileHaveProperty((int)vector.X, (int)vector.Y, "Diggable", "Back") != null && !Game1.currentLocation.isTileHoeDirt(vector))
                        {
                            //Game1.currentLocation.checkForBuriedItem((int)vector.X, (int)vector.Y, explosion: true, detectOnly: false, Source);
                            //Game1.currentLocation.makeHoeDirt(vector);
                        }
                    }

                    if (flag && !Game1.currentLocation.objects.ContainsKey(vector) && Game1.random.NextDouble() < 0.9 && Game1.currentLocation.doesTileHaveProperty((int)vector.X, (int)vector.Y, "Diggable", "Back") != null && !Game1.currentLocation.isTileHoeDirt(vector))
                    {
                        //Game1.currentLocation.checkForBuriedItem((int)vector.X, (int)vector.Y, explosion: true, detectOnly: false, who);
                        //Game1.currentLocation.makeHoeDirt(vector);
                    }

                    vector.Y += 1f;
                    vector.Y = Math.Min(Game1.currentLocation.map.Layers[0].LayerHeight - 1, Math.Max(0f, vector.Y));
                }

                vector.X += 1f;
                vector.Y = Math.Min(Game1.currentLocation.map.Layers[0].LayerWidth - 1, Math.Max(0f, vector.X));
                vector.Y = tileLocation.Y - radius;
                vector.Y = Math.Min(Game1.currentLocation.map.Layers[0].LayerHeight - 1, Math.Max(0f, vector.Y));
            }
        }

        private void rumbleAndFade(int milliseconds)
        {
            rumbleAndFadeEvent.Fire(milliseconds);
        }

        public void broadcastSprites(GameLocation location, List<TemporaryAnimatedSprite> sprites)
        {
            location.temporarySprites.AddRange(sprites);
            if (sprites.Count() == 0 || !Game1.IsMultiplayer)
            {
                return;
            }

            using MemoryStream memoryStream = new MemoryStream();
        }
    }
}