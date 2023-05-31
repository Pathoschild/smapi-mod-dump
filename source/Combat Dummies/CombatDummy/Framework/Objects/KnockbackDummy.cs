/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/CombatDummy
**
*************************************************/

using CombatDummy.Framework.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Monsters;
using System;
using Object = StardewValley.Object;

namespace CombatDummy.Framework.Objects
{
    internal class KnockbackDummy
    {
        public static bool IsValid(Object instance)
        {
            if (instance is null || instance.Name != ModDataKeys.KNOCKBACK_DUMMY_NAME)
            {
                return false;
            }

            return true;
        }

        public static Vector2 GetVelocity(Object knockbackDummy)
        {
            Vector2 velocity = new Vector2();
            if (knockbackDummy.modData.TryGetValue(ModDataKeys.DUMMY_VELOCITY_X, out var rawVelocityX))
            {
                velocity.X = float.Parse(rawVelocityX.ToString());
            }
            if (knockbackDummy.modData.TryGetValue(ModDataKeys.DUMMY_VELOCITY_Y, out var rawVelocityY))
            {
                velocity.Y = float.Parse(rawVelocityY.ToString());
            }

            return velocity;
        }

        public static void SetVelocity(Object knockbackDummy, Vector2 velocity)
        {
            knockbackDummy.modData[ModDataKeys.DUMMY_VELOCITY_X] = velocity.X.ToString();
            knockbackDummy.modData[ModDataKeys.DUMMY_VELOCITY_Y] = velocity.Y.ToString();
        }

        public static Vector2 GetPosition(Object knockbackDummy)
        {
            Vector2 position = new Vector2();
            if (knockbackDummy.modData.TryGetValue(ModDataKeys.DUMMY_NEXT_POSITION_X, out var rawPositionX))
            {
                position.X = float.Parse(rawPositionX.ToString());
            }
            if (knockbackDummy.modData.TryGetValue(ModDataKeys.DUMMY_NEXT_POSITION_Y, out var rawPositionY))
            {
                position.Y = float.Parse(rawPositionY.ToString());
            }

            return position;
        }

        public static Vector2 GetLastPosition(Object knockbackDummy)
        {
            Vector2 position = new Vector2();
            if (knockbackDummy.modData.TryGetValue(ModDataKeys.DUMMY_LAST_POSITION_X, out var rawPositionX))
            {
                position.X = float.Parse(rawPositionX.ToString());
            }
            if (knockbackDummy.modData.TryGetValue(ModDataKeys.DUMMY_LAST_POSITION_Y, out var rawPositionY))
            {
                position.Y = float.Parse(rawPositionY.ToString());
            }

            return position;
        }

        public static void SetPosition(Object knockbackDummy, Vector2 position, Vector2 velocity)
        {
            knockbackDummy.modData[ModDataKeys.DUMMY_LAST_POSITION_X] = position.X.ToString();
            knockbackDummy.modData[ModDataKeys.DUMMY_LAST_POSITION_Y] = position.Y.ToString();

            position.X += velocity.X;
            position.Y -= velocity.Y;

            knockbackDummy.modData[ModDataKeys.DUMMY_NEXT_POSITION_X] = position.X.ToString();
            knockbackDummy.modData[ModDataKeys.DUMMY_NEXT_POSITION_Y] = position.Y.ToString();
        }

        public static void Update(Object instance, GameTime time, GameLocation location)
        {
            var tileLocation = instance.TileLocation;
            var tilePosition = instance.TileLocation * 64f;
            bool hasMonster = location.isCharacterAtTile(tileLocation) is Monster monster && MonsterDummy.IsValid(monster);

            var knockbackDummy = instance;
            int knockbackCountdown = 0;
            if (knockbackDummy.modData.ContainsKey(ModDataKeys.DUMMY_KNOCKBACK_COUNTDOWN) is true)
            {
                knockbackCountdown = Int32.Parse(knockbackDummy.modData[ModDataKeys.DUMMY_KNOCKBACK_COUNTDOWN]);
            }

            if (hasMonster is false && (knockbackCountdown == int.MaxValue || knockbackCountdown <= 0))
            {
                var dummyMonster = new Monster("Mummy", tilePosition)
                {
                    Speed = 0,
                    DamageToFarmer = 0,
                    stunTime = Int32.MaxValue,
                    HideShadow = true
                };
                dummyMonster.modData[ModDataKeys.MONSTER_DUMMY_FLAG] = true.ToString();
                dummyMonster.modData[ModDataKeys.MONSTER_HOME_POSITION_X] = tileLocation.X.ToString();
                dummyMonster.modData[ModDataKeys.MONSTER_HOME_POSITION_Y] = tileLocation.Y.ToString();
                location.characters.Add(dummyMonster);

                CombatDummy.monitor.Log($"Added dummy monster at the following location: {instance.TileLocation} | {dummyMonster.getTileLocation()}", LogLevel.Trace);
            }

            if (knockbackDummy.modData.TryGetValue(ModDataKeys.IS_DUMMY_ANIMATING, out string rawIsAnimating) && Boolean.Parse(rawIsAnimating) is true)
            {
                var animationFrame = Int32.Parse(knockbackDummy.modData[ModDataKeys.DUMMY_ANIMATION_FRAME]);
                var animationCountdown = Int32.Parse(knockbackDummy.modData[ModDataKeys.DUMMY_ANIMATION_COUNTDOWN]) - time.ElapsedGameTime.Milliseconds;
                if (animationCountdown <= 0)
                {
                    animationFrame += 1;
                    animationCountdown = CombatDummy.ANIMATION_COOLDOWN;
                    if (animationFrame > 2)
                    {
                        animationFrame = 0;
                        knockbackDummy.modData[ModDataKeys.IS_DUMMY_ANIMATING] = false.ToString();
                    }

                    knockbackDummy.modData[ModDataKeys.DUMMY_ANIMATION_FRAME] = animationFrame.ToString();
                }

                knockbackDummy.modData[ModDataKeys.DUMMY_ANIMATION_COUNTDOWN] = animationCountdown.ToString();
            }

            if (knockbackDummy.modData.ContainsKey(ModDataKeys.DUMMY_INVINCIBLE_COUNTDOWN) is true)
            {
                var invincibleCountdown = Int32.Parse(knockbackDummy.modData[ModDataKeys.DUMMY_INVINCIBLE_COUNTDOWN]) - time.ElapsedGameTime.Milliseconds;

                if (invincibleCountdown <= 0)
                {
                    knockbackDummy.modData[ModDataKeys.IS_DUMMY_INVINCIBLE] = false.ToString();
                }
                else
                {
                    knockbackDummy.modData[ModDataKeys.DUMMY_INVINCIBLE_COUNTDOWN] = invincibleCountdown.ToString();
                }
            }

            if (knockbackDummy.modData.ContainsKey(ModDataKeys.DUMMY_DAMAGE_COUNTDOWN) is true)
            {
                var damageCountdown = Int32.Parse(knockbackDummy.modData[ModDataKeys.DUMMY_DAMAGE_COUNTDOWN]) - time.ElapsedGameTime.Milliseconds;

                if (damageCountdown > -10000)
                {
                    knockbackDummy.modData[ModDataKeys.DUMMY_DAMAGE_COUNTDOWN] = damageCountdown.ToString();
                }
            }

            var velocity = GetVelocity(knockbackDummy);
            var position = GetPosition(knockbackDummy);
            var lastPosition = GetLastPosition(knockbackDummy);
            if (position == lastPosition)
            {
                if (knockbackCountdown != int.MaxValue)
                {
                    knockbackCountdown -= time.ElapsedGameTime.Milliseconds;
                }

                if (knockbackCountdown < 0)
                {
                    knockbackCountdown = int.MaxValue;

                    SetVelocity(knockbackDummy, Vector2.Zero);
                    SetPosition(knockbackDummy, knockbackDummy.TileLocation * 64f, new Vector2(0f, 64f));

                    location.temporarySprites.Add(new TemporaryAnimatedSprite(5, position, Color.White, 8, Game1.random.NextDouble() < 0.5, 50f, 0, -1, -1f, 128));
                    //location.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 320, 64, 64), 30f, 8, 0, position, flicker: false, false, 1f, 0f, Color.White, 1f, 0f, 0f, 0f));
                }

                knockbackDummy.modData[ModDataKeys.DUMMY_KNOCKBACK_COUNTDOWN] = knockbackCountdown.ToString();
            }
            else
            {
                velocity = velocity - velocity / 2f;
                SetVelocity(knockbackDummy, velocity);
                SetPosition(knockbackDummy, position, velocity);
            }
        }

        public static void Draw(Object __instance, SpriteBatch spriteBatch, int x, int y, float alpha)
        {
            int animationFrame = __instance.modData.TryGetValue(ModDataKeys.DUMMY_ANIMATION_FRAME, out string rawAnimationFrame) is false ? 0 : Int32.Parse(rawAnimationFrame);

            Vector2 scaleFactor = __instance.getScale() * 4f;
            Vector2 position = Game1.GlobalToLocal(Game1.viewport, KnockbackDummy.GetPosition(__instance));
            Rectangle destination = new Rectangle((int)(position.X - scaleFactor.X / 2f) + ((__instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0), (int)(position.Y - scaleFactor.Y / 2f) + ((__instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0), (int)(64f + scaleFactor.X), (int)(128f + scaleFactor.Y / 2f));

            alpha = __instance.modData.TryGetValue(ModDataKeys.IS_DUMMY_INVINCIBLE, out string rawIsInvincible) is false ? alpha : Boolean.Parse(rawIsInvincible) ? 0.75f : alpha;
            spriteBatch.Draw(CombatDummy.assetManager.KnockbackDummyTexture, destination, new Rectangle(animationFrame * 16, 0, 16, 32), Color.White * alpha, 0f, Vector2.Zero, SpriteEffects.None, __instance.getBoundingBox(new Vector2(x, y)).Bottom / 10000f);

            //Vector2 local = Game1.GlobalToLocal(new Vector2(base.getStandingX(), base.getStandingY() - this.Sprite.SpriteHeight * 4 - 64 + base.yJumpOffset));
            var collectiveDamage = __instance.modData.ContainsKey(ModDataKeys.DUMMY_COLLECTIVE_DAMAGE) ? Int32.Parse(__instance.modData[ModDataKeys.DUMMY_COLLECTIVE_DAMAGE]) : 0;
            var damageCountdown = __instance.modData.ContainsKey(ModDataKeys.DUMMY_KNOCKBACK_COUNTDOWN) ? Int32.Parse(__instance.modData[ModDataKeys.DUMMY_KNOCKBACK_COUNTDOWN]) : 0;

            if (collectiveDamage > 0 && damageCountdown > 0 && damageCountdown != int.MaxValue)
            {
                SpriteText.drawStringHorizontallyCenteredAt(spriteBatch, collectiveDamage.ToString(), (int)position.X + 32, (int)position.Y - 16, color: 5, alpha: 1f);
            }
        }
    }
}
