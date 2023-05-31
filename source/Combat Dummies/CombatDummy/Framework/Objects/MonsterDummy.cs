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
using StardewValley;
using StardewValley.Monsters;
using System;

namespace CombatDummy.Framework.Objects
{
    internal class MonsterDummy
    {
        public static bool IsValid(Character instance)
        {
            if (instance is null || instance.modData.ContainsKey(ModDataKeys.MONSTER_DUMMY_FLAG) is false)
            {
                return false;
            }

            return true;
        }

        public static void TakeDamage(Monster instance, int invincibleCountdown, int result, int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, string hitSound)
        {
            // Verify that the Target Dummy exists on the same tile
            // If it does not, delete this monster
            var tilePosition = new Point(0, 0);
            if (instance.modData.TryGetValue(ModDataKeys.MONSTER_HOME_POSITION_X, out var rawHomeX) && int.TryParse(rawHomeX, out var actualHomeX))
            {
                tilePosition.X = actualHomeX;
            }
            if (instance.modData.TryGetValue(ModDataKeys.MONSTER_HOME_POSITION_Y, out var rawHomeY) && int.TryParse(rawHomeY, out var actualHomeY))
            {
                tilePosition.Y = actualHomeY;
            }
            var practiceDummy = instance.currentLocation.getObjectAtTile(tilePosition.X, tilePosition.Y);
            if (PracticeDummy.IsValid(practiceDummy) is false && KnockbackDummy.IsValid(practiceDummy) is false && MaxHitDummy.IsValid(practiceDummy) is false)
            {
                instance.currentLocation.characters.Remove(instance);
                return;
            }

            if (KnockbackDummy.IsValid(practiceDummy))
            {
                // Update the practice dummy's velocity
                var velocity = new Vector2(xTrajectory / 3, yTrajectory / 3);
                KnockbackDummy.SetVelocity(practiceDummy, velocity);
                KnockbackDummy.SetPosition(practiceDummy, KnockbackDummy.GetPosition(practiceDummy), velocity);

                practiceDummy.modData[ModDataKeys.DUMMY_KNOCKBACK_COUNTDOWN] = 2000.ToString();
            }

            // Label the damage amount
            int damageAmount = result;

            // Cache the hit data to the practice dummy
            bool isInvincible = practiceDummy.modData.TryGetValue(ModDataKeys.IS_DUMMY_INVINCIBLE, out string rawIsInvincible) is true ? Boolean.Parse(rawIsInvincible) : false;
            if (isInvincible is false)
            {
                if (damageAmount > 0)
                {
                    var collectiveDamage = practiceDummy.modData.ContainsKey(ModDataKeys.DUMMY_COLLECTIVE_DAMAGE) ? Int32.Parse(practiceDummy.modData[ModDataKeys.DUMMY_COLLECTIVE_DAMAGE]) : 0;
                    var damageCountdown = practiceDummy.modData.ContainsKey(ModDataKeys.DUMMY_DAMAGE_COUNTDOWN) ? Int32.Parse(practiceDummy.modData[ModDataKeys.DUMMY_DAMAGE_COUNTDOWN]) : 0;
                    if (damageCountdown <= 0)
                    {
                        collectiveDamage = 0;
                    }
                    damageCountdown = 1000;

                    if (MaxHitDummy.IsValid(practiceDummy))
                    {
                        practiceDummy.modData[ModDataKeys.DUMMY_COLLECTIVE_DAMAGE] = damageAmount > collectiveDamage ? damageAmount.ToString() : collectiveDamage.ToString();
                    }
                    else
                    {
                        practiceDummy.modData[ModDataKeys.DUMMY_COLLECTIVE_DAMAGE] = (collectiveDamage + damageAmount).ToString();
                    }
                    practiceDummy.modData[ModDataKeys.DUMMY_DAMAGE_COUNTDOWN] = damageCountdown.ToString();
                    //__instance.debris.Add(new Debris(damageAmount, new Vector2(practiceDummyBox.Center.X + 16, practiceDummyBox.Center.Y), false ? Color.Yellow : new Color(255, 130, 0), false ? (1f + (float)damageAmount / 300f) : 1f, Game1.player));
                }
                if (invincibleCountdown > 0)
                {
                    practiceDummy.modData[ModDataKeys.IS_DUMMY_INVINCIBLE] = true.ToString();
                    practiceDummy.modData[ModDataKeys.DUMMY_INVINCIBLE_COUNTDOWN] = invincibleCountdown.ToString();
                }

                bool isAnimating = practiceDummy.modData.TryGetValue(ModDataKeys.IS_DUMMY_ANIMATING, out string rawIsAnimating) is true ? Boolean.Parse(rawIsAnimating) : false;
                if (isAnimating is false)
                {
                    practiceDummy.modData[ModDataKeys.IS_DUMMY_ANIMATING] = true.ToString();
                    practiceDummy.modData[ModDataKeys.DUMMY_ANIMATION_COUNTDOWN] = CombatDummy.ANIMATION_COOLDOWN.ToString();
                    practiceDummy.modData[ModDataKeys.DUMMY_ANIMATION_FRAME] = "1";
                }
            }
        }

        public static void Update(Monster instance, ref int invincibleCountdown, GameTime time, GameLocation location)
        {
            // Verify that the Target Dummy exists on the same tile
            // If it does not, delete this monster
            var tilePosition = new Point(0, 0);
            if (instance.modData.TryGetValue(ModDataKeys.MONSTER_HOME_POSITION_X, out var rawHomeX) && int.TryParse(rawHomeX, out var actualHomeX))
            {
                tilePosition.X = actualHomeX;
            }
            if (instance.modData.TryGetValue(ModDataKeys.MONSTER_HOME_POSITION_Y, out var rawHomeY) && int.TryParse(rawHomeY, out var actualHomeY))
            {
                tilePosition.Y = actualHomeY;
            }

            var practiceDummy = instance.currentLocation.getObjectAtTile(tilePosition.X, tilePosition.Y);
            if (PracticeDummy.IsValid(practiceDummy) is false && KnockbackDummy.IsValid(practiceDummy) is false && MaxHitDummy.IsValid(practiceDummy) is false)
            {
                instance.currentLocation.characters.Remove(instance);
                return;
            }

            if (invincibleCountdown > 0)
            {
                invincibleCountdown -= time.ElapsedGameTime.Milliseconds;
                if (invincibleCountdown <= 0)
                {
                    instance.stopGlowing();
                }
            }

            instance.stunTime = Int32.MaxValue;
            instance.MaxHealth = Int32.MaxValue;
            instance.Health = Int32.MaxValue;
            instance.Slipperiness = 1;
        }

        public static void Draw(Monster instance, SpriteBatch b)
        {
            // Do nothing
        }
    }
}
