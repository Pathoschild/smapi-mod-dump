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
using StardewValley.Tools;
using System;

namespace BattleRoyale
{
    public static class TakeDamageExtension
    {
        public static void takeDamage(this Farmer farmer, DamageSource source, int damage, bool overrideParry, Farmer damager)
        {
            if (Game1.eventUp || farmer.FarmerSprite.isPassingOut())
            {
                return;
            }
            if (source == DamageSource.THORNS)
                overrideParry = true;

            bool playerParryable = farmer.CurrentTool != null && farmer.CurrentTool is MeleeWeapon weapon && weapon.isOnSpecial && weapon.type == 3 && !overrideParry;
            bool playerDamageable = farmer.CanBeDamaged();
            if (playerParryable)
            {
                Rumble.rumble(0.75f, 150f);
                farmer.currentLocation.playSound("parry");
            }
            else
            {
                if (!playerDamageable)
                {
                    return;
                }

                int effectiveResilience = 0;
                if (source != DamageSource.THORNS && source != DamageSource.STORM)
                {
                    effectiveResilience = farmer.resilience;
                    damage += Game1.random.Next(Math.Min(-1, -damage / 8), Math.Max(1, damage / 8));
                    if (farmer.CurrentTool is MeleeWeapon)
                    {
                        effectiveResilience += (int)(farmer.CurrentTool as MeleeWeapon).addedDefense;
                    }
                    if (effectiveResilience >= damage * 0.5f)
                    {
                        effectiveResilience -= (int)(effectiveResilience * Game1.random.Next(3) / 10f);
                    }
                }

                if (damager != null && farmer.isWearingRing(839) && source != DamageSource.THORNS)
                {
                    int multiplier = farmer.GetEffectsOfRingMultiplier(839);
                    int damageToMonster = Math.Max(1, (int)Math.Floor(damage * 0.2)) * multiplier;
                    NetworkUtils.SendDamageToPlayer(damager, DamageSource.THORNS, damageToMonster, farmer.UniqueMultiplayerID);
                }
                if (farmer.isWearingRing(524) && !Game1.buffsDisplay.hasBuff(21) && Game1.random.NextDouble() < (0.9 - (farmer.health / 100f)) / (3 - farmer.LuckLevel / 10) + ((farmer.health <= 15) ? 0.2 : 0.0))
                {
                    farmer.currentLocation.playSound("yoba");
                    Game1.buffsDisplay.addOtherBuff(new Buff(21));
                    return;
                }
                Rumble.rumble(0.75f, 150f);
                damage = Math.Max(1, damage - effectiveResilience);
                farmer.health = Math.Max(0, farmer.health - damage);
                if (farmer.health <= 0 && farmer.GetEffectsOfRingMultiplier(863) > 0 && !farmer.hasUsedDailyRevive.Value)
                {
                    Game1.player.startGlowing(new Color(255, 255, 0), border: false, 0.25f);
                    DelayedAction.functionAfterDelay(delegate
                    {
                        farmer.stopGlowing();
                    }, 500);
                    Game1.playSound("yoba");
                    for (int i = 0; i < 13; i++)
                    {
                        float xPos = Game1.random.Next(-32, 33);
                        farmer.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Rectangle(114, 46, 2, 2), 200f, 5, 1, new Vector2(xPos + 32f, -96f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, 0f, 0f)
                        {
                            attachedCharacter = farmer,
                            positionFollowsAttachedCharacter = true,
                            motion = new Vector2(xPos / 32f, -3f),
                            delayBeforeAnimationStart = i * 50,
                            alphaFade = 0.001f,
                            acceleration = new Vector2(0f, 0.1f)
                        });
                    }
                    farmer.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Rectangle(157, 280, 28, 19), 2000f, 1, 1, new Vector2(-20f, -16f), flicker: false, flipped: false, 1E-06f, 0f, Color.White, 4f, 0f, 0f, 0f)
                    {
                        attachedCharacter = farmer,
                        positionFollowsAttachedCharacter = true,
                        alpha = 0.1f,
                        alphaFade = -0.01f,
                        alphaFadeFade = -0.00025f
                    });
                    farmer.health = (int)Math.Min(farmer.maxHealth, farmer.maxHealth * 0.5f + farmer.GetEffectsOfRingMultiplier(863));
                    farmer.hasUsedDailyRevive.Value = true;
                }

                if (source != DamageSource.THORNS)
                    farmer.currentLocation.debris.Add(new Debris(damage, new Vector2(farmer.getStandingX() + 8, farmer.getStandingY()), Color.Red, 1f, farmer));
                else
                    farmer.currentLocation.debris.Add(new Debris(damage, new Vector2(farmer.getStandingX() + 8, farmer.getStandingY()), new Color(255, 130, 0), 1f, farmer));

                farmer.currentLocation.playSound("ow");
            }
        }
    }
}
