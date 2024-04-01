/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mus-candidus/MiceInTheValley
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.TerrainFeatures;


namespace MiceInTheValley.Framework {
    // A heavily modified rabbit :-)
    internal class Mouse : Critter {
        // Enum doesn't adhere to naming conventions because it matches sprite names.
        internal enum Species { mouse, mouse_white, mouse_tiger };

        private int characterCheckTimer_ = 200;
        private bool running_;

        private readonly Vector2 direction_;
        private readonly IMonitor monitor_;
        private readonly SoundEffectInstance sound_;

        public Mouse(IMonitor monitor, Vector2 position, Vector2 direction, float speed, Species species, SoundEffect sound, ModConfig config) {
            monitor_      = monitor;
            sound_        = sound.CreateInstance();
            sound_.Volume = config.Volume;
            sound_.Pitch  = config.Pitch;
            direction_    = direction;

            base.position = position * 64f;
            position.Y += 48f;
            baseFrame = 0;
            sprite = new AnimatedSprite(species.ToString(), baseFrame, 16, 16);
            sprite.loop = true;
            startingPosition = position;
        }

        public override bool update(GameTime time, GameLocation environment) {
            characterCheckTimer_ -= time.ElapsedGameTime.Milliseconds;
            if (characterCheckTimer_ <= 0 && !running_) {
                if (Utility.isOnScreen(position, -32)) {
                    running_ = true;
                    monitor_.Log($"Go, mousy! {position}");

                    int offset = 0;
                    // Our custom mouse sprite provides flipped textures.
                    // so flipping means selecting different texture frames.
                    if (direction_ == Vector2.UnitX) {
                        offset = 0;
                    }
                    else if (direction_ == -Vector2.UnitY) {
                        offset = 10;
                    }
                    else if (direction_ == -Vector2.UnitX) {
                        offset = 20;
                    }
                    else if (direction_ == Vector2.UnitY) {
                        offset = 30;
                    }
                    else {
                        monitor_.Log($"Invalid direction ({direction_})", LogLevel.Error);
                    }

                    sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame> {
                        new FarmerSprite.AnimationFrame(baseFrame + offset + 0, 40),
                        new FarmerSprite.AnimationFrame(baseFrame + offset + 1, 40),
                        new FarmerSprite.AnimationFrame(baseFrame + offset + 2, 40),
                        new FarmerSprite.AnimationFrame(baseFrame + offset + 3, 40),
                    });

                    if (Game1.random.NextDouble() < 0.5) {
                        sound_.Play();
                        monitor_.Log("Squeak!");
                    }

                    sprite.loop = true;
                }
                characterCheckTimer_ = 200;
            }

            if (running_) {
                int speed = Game1.random.Next(10);
                if (direction_ == Vector2.UnitX) {
                    position.X += speed;
                }
                else if (direction_ == -Vector2.UnitY) {
                    position.Y -= speed;
                }
                else if (direction_ == -Vector2.UnitX) {
                    position.X -= speed;
                }
                else if (direction_ == Vector2.UnitY) {
                    position.Y += speed;
                }
                else {
                    this.monitor_.Log($"Invalid direction ({direction_})", LogLevel.Error);
                }

                if (characterCheckTimer_ <= 0) {
                    characterCheckTimer_ = 200;
                    if (environment.largeTerrainFeatures != null) {
                        Rectangle value = new Rectangle((int) position.X + 32, (int) position.Y - 32, 4, 192);
                        foreach (LargeTerrainFeature largeTerrainFeature in environment.largeTerrainFeatures) {
                            if (largeTerrainFeature is Bush && largeTerrainFeature.getBoundingBox().Intersects(value)) {
                                (largeTerrainFeature as Bush).performUseAction(largeTerrainFeature.Tile);
                                return true;
                            }
                        }
                    }
                }
            }
            return base.update(time, environment);
        }

        public override void draw(SpriteBatch b) {
            // Scaling factor of mouse and shadow texture.
            const float scale = 3f;
            var mouseOrigin  = Game1.GlobalToLocal(Game1.viewport, position + new Vector2(0f, -20f));
            var shadowOrigin = Game1.GlobalToLocal(Game1.viewport, position + new Vector2(scale * 8f, scale * 6f));

            // Our custom mouse sprite provides flipped textures so flip must be false.
            sprite.draw(b, mouseOrigin, (position.Y + 64f) / 10000f, 0, 0, Color.White, false, scale);
            b.Draw(Game1.shadowTexture, shadowOrigin, Game1.shadowTexture.Bounds, Color.White, 0f,
                    new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y),
                    3f + Math.Max(-3f, (yJumpOffset + yOffset) / 16f),
                    SpriteEffects.None, (position.Y - 1f) / 10000f);
        }
    }
}
