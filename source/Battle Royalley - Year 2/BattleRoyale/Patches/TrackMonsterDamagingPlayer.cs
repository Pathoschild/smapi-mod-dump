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
using StardewValley.Monsters;
using StardewValley.Tools;
using System;

namespace BattleRoyale.Patches
{
    class TrackMonsterDamagingPlayer : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(Farmer), "takeDamage");

        public static bool Prefix(Farmer __instance, int damage, bool overrideParry, Monster damager)
        {
            if (Game1.eventUp || __instance.FarmerSprite.isPassingOut())
            {
                return false;
            }
            bool num = damager != null && !damager.isInvincible() && !overrideParry;
            bool monsterDamageCapable = (damager == null || !damager.isInvincible()) && (damager == null || (damager is not GreenSlime && damager is not BigSlime) || !__instance.isWearingRing(520));
            bool playerParryable = __instance.CurrentTool != null && __instance.CurrentTool is MeleeWeapon weapon && weapon.isOnSpecial && weapon.type == 3;
            bool playerDamageable = __instance.CanBeDamaged();
            if (num && playerParryable)
            {
                Rumble.rumble(0.75f, 150f);
                __instance.currentLocation.playSound("parry");
                damager.parried(damage, __instance);
            }
            else
            {
                if (!(monsterDamageCapable && playerDamageable))
                {
                    return false;
                }
                damager?.onDealContactDamage(__instance);
                damage += Game1.random.Next(Math.Min(-1, -damage / 8), Math.Max(1, damage / 8));
                int effectiveResilience = __instance.resilience;
                if (__instance.CurrentTool is MeleeWeapon)
                {
                    effectiveResilience += (int)(__instance.CurrentTool as MeleeWeapon).addedDefense;
                }
                if (effectiveResilience >= damage * 0.5f)
                {
                    effectiveResilience -= (int)(effectiveResilience * Game1.random.Next(3) / 10f);
                }
                if (damager != null && __instance.isWearingRing(839))
                {
                    Rectangle monsterBox = damager.GetBoundingBox();
                    Vector2 trajectory = Utility.getAwayFromPlayerTrajectory(monsterBox, __instance);
                    trajectory /= 2f;
                    int damageToMonster = damage;
                    int farmerDamage = Math.Max(1, damage - effectiveResilience);
                    if (farmerDamage < 10)
                    {
                        damageToMonster = (int)Math.Ceiling((damageToMonster + farmerDamage) / 2.0);
                    }
                    damager.takeDamage(damageToMonster, (int)trajectory.X, (int)trajectory.Y, isBomb: false, 1.0, __instance);
                    damager.currentLocation.debris.Add(new Debris(damageToMonster, new Vector2(monsterBox.Center.X + 16, monsterBox.Center.Y), new Color(255, 130, 0), 1f, damager));
                }
                if (__instance.isWearingRing(524) && !Game1.buffsDisplay.hasBuff(21) && Game1.random.NextDouble() < (0.9 - (__instance.health / 100f)) / (3 - __instance.LuckLevel / 10) + ((__instance.health <= 15) ? 0.2 : 0.0))
                {
                    __instance.currentLocation.playSound("yoba");
                    Game1.buffsDisplay.addOtherBuff(new Buff(21));
                    return false;
                }
                Rumble.rumble(0.75f, 150f);
                damage = Math.Max(1, damage - effectiveResilience);

                Round round = ModEntry.BRGame.GetActiveRound();
                if (round != null && !round.InProgress)
                    damage = 0;

                __instance.health = Math.Max(0, __instance.health - damage);

                if (round != null && damager != null)
                    FarmerUtils.TakeDamage(Game1.player, DamageSource.MONSTER, damage, monster: damager.Name);

                if (__instance.health <= 0 && __instance.GetEffectsOfRingMultiplier(863) > 0 && !__instance.hasUsedDailyRevive.Value)
                {
                    Game1.player.startGlowing(new Color(255, 255, 0), border: false, 0.25f);
                    DelayedAction.functionAfterDelay(delegate
                    {
                        __instance.stopGlowing();
                    }, 500);
                    Game1.playSound("yoba");
                    for (int i = 0; i < 13; i++)
                    {
                        float xPos = Game1.random.Next(-32, 33);
                        __instance.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(114, 46, 2, 2), 200f, 5, 1, new Vector2(xPos + 32f, -96f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, 0f, 0f)
                        {
                            attachedCharacter = __instance,
                            positionFollowsAttachedCharacter = true,
                            motion = new Vector2(xPos / 32f, -3f),
                            delayBeforeAnimationStart = i * 50,
                            alphaFade = 0.001f,
                            acceleration = new Vector2(0f, 0.1f)
                        });
                    }
                    __instance.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(157, 280, 28, 19), 2000f, 1, 1, new Vector2(-20f, -16f), flicker: false, flipped: false, 1E-06f, 0f, Color.White, 4f, 0f, 0f, 0f)
                    {
                        attachedCharacter = __instance,
                        positionFollowsAttachedCharacter = true,
                        alpha = 0.1f,
                        alphaFade = -0.01f,
                        alphaFadeFade = -0.00025f
                    });
                    __instance.health = (int)Math.Min(__instance.maxHealth, (float)__instance.maxHealth * 0.5f + (float)__instance.GetEffectsOfRingMultiplier(863));
                    __instance.hasUsedDailyRevive.Value = true;
                }
                __instance.currentLocation.debris.Add(new Debris(damage, new Vector2(__instance.getStandingX() + 8, __instance.getStandingY()), Color.Red, 1f, __instance));
                __instance.currentLocation.playSound("ow");
            }

            return false;
        }
    }
}
