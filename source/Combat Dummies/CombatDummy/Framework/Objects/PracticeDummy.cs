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
    internal class PracticeDummy
    {
        public static bool IsValid(Object instance)
        {
            if (instance is null || instance.Name != ModDataKeys.PRACTICE_DUMMY_NAME)
            {
                return false;
            }

            return true;
        }

        public static void Update(Object instance, GameTime time, GameLocation location)
        {
            var tileLocation = instance.TileLocation;
            var tilePosition = instance.TileLocation * 64f;
            bool hasMonster = location.isCharacterAtTile(tileLocation) is Monster monster && MonsterDummy.IsValid(monster);

            if (hasMonster is false)
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

            var practiceDummy = instance;
            if (practiceDummy.modData.TryGetValue(ModDataKeys.IS_DUMMY_ANIMATING, out string rawIsAnimating) && Boolean.Parse(rawIsAnimating) is true)
            {
                var animationFrame = Int32.Parse(practiceDummy.modData[ModDataKeys.DUMMY_ANIMATION_FRAME]);
                var animationCountdown = Int32.Parse(practiceDummy.modData[ModDataKeys.DUMMY_ANIMATION_COUNTDOWN]) - time.ElapsedGameTime.Milliseconds;
                if (animationCountdown <= 0)
                {
                    animationFrame += 1;
                    animationCountdown = CombatDummy.ANIMATION_COOLDOWN;
                    if (animationFrame > 2)
                    {
                        animationFrame = 0;
                        practiceDummy.modData[ModDataKeys.IS_DUMMY_ANIMATING] = false.ToString();
                    }

                    practiceDummy.modData[ModDataKeys.DUMMY_ANIMATION_FRAME] = animationFrame.ToString();
                }

                practiceDummy.modData[ModDataKeys.DUMMY_ANIMATION_COUNTDOWN] = animationCountdown.ToString();
            }

            if (practiceDummy.modData.ContainsKey(ModDataKeys.DUMMY_INVINCIBLE_COUNTDOWN) is true)
            {
                var invincibleCountdown = Int32.Parse(practiceDummy.modData[ModDataKeys.DUMMY_INVINCIBLE_COUNTDOWN]) - time.ElapsedGameTime.Milliseconds;

                if (invincibleCountdown <= 0)
                {
                    practiceDummy.modData[ModDataKeys.IS_DUMMY_INVINCIBLE] = false.ToString();
                }
                else
                {
                    practiceDummy.modData[ModDataKeys.DUMMY_INVINCIBLE_COUNTDOWN] = invincibleCountdown.ToString();
                }
            }

            if (practiceDummy.modData.ContainsKey(ModDataKeys.DUMMY_DAMAGE_COUNTDOWN) is true)
            {
                var damageCountdown = Int32.Parse(practiceDummy.modData[ModDataKeys.DUMMY_DAMAGE_COUNTDOWN]) - time.ElapsedGameTime.Milliseconds;

                if (damageCountdown > -10000)
                {
                    practiceDummy.modData[ModDataKeys.DUMMY_DAMAGE_COUNTDOWN] = damageCountdown.ToString();
                    //practiceDummy.modData[ModDataKeys.DUMMY_COLLECTIVE_DAMAGE] = "0";
                    //practiceDummy.modData[ModDataKeys.DUMMY_DAMAGE_COUNTDOWN] = "1000";
                }
            }
        }

        public static void Draw(Object __instance, SpriteBatch spriteBatch, int x, int y, float alpha)
        {
            int animationFrame = __instance.modData.TryGetValue(ModDataKeys.DUMMY_ANIMATION_FRAME, out string rawAnimationFrame) is false ? 0 : Int32.Parse(rawAnimationFrame);

            Vector2 scaleFactor = __instance.getScale() * 4f;
            Vector2 position = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64 - 64));
            Rectangle destination = new Rectangle((int)(position.X - scaleFactor.X / 2f) + ((__instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0), (int)(position.Y - scaleFactor.Y / 2f) + ((__instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0), (int)(64f + scaleFactor.X), (int)(128f + scaleFactor.Y / 2f));

            alpha = __instance.modData.TryGetValue(ModDataKeys.IS_DUMMY_INVINCIBLE, out string rawIsInvincible) is false ? alpha : Boolean.Parse(rawIsInvincible) ? 0.75f : alpha;
            spriteBatch.Draw(CombatDummy.assetManager.PracticeDummyTexture, destination, new Rectangle(animationFrame * 16, 0, 16, 32), Color.White * alpha, 0f, Vector2.Zero, SpriteEffects.None, __instance.getBoundingBox(new Vector2(x, y)).Bottom / 10000f);

            //Vector2 local = Game1.GlobalToLocal(new Vector2(base.getStandingX(), base.getStandingY() - this.Sprite.SpriteHeight * 4 - 64 + base.yJumpOffset));
            var collectiveDamage = __instance.modData.ContainsKey(ModDataKeys.DUMMY_COLLECTIVE_DAMAGE) ? Int32.Parse(__instance.modData[ModDataKeys.DUMMY_COLLECTIVE_DAMAGE]) : 0;
            var damageCountdown = __instance.modData.ContainsKey(ModDataKeys.DUMMY_DAMAGE_COUNTDOWN) ? Int32.Parse(__instance.modData[ModDataKeys.DUMMY_DAMAGE_COUNTDOWN]) : 0;

            float adjustedAlpha = damageCountdown <= -1000 ? 1 - (Math.Abs(damageCountdown) / 2200f) : 1f;
            if (collectiveDamage > 0 && adjustedAlpha > 0)
            {
                int yOffset = damageCountdown <= 0 ? (Math.Abs(damageCountdown) / 75) : 0;
                SpriteText.drawStringHorizontallyCenteredAt(spriteBatch, collectiveDamage.ToString(), (int)position.X + 32, (int)position.Y - 16 - (yOffset * 2), color: 5, alpha: adjustedAlpha);
            }
        }
    }
}
