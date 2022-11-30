/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewRoguelike.HatQuests;
using StardewRoguelike.VirtualProperties;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Tools;
using System;

namespace StardewRoguelike.Patches
{
    internal class FarmerTakeDamagePatch : Patch
    {
        public static int ShellCooldownSeconds = 0;

        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(Farmer), "takeDamage");

        public static bool Prefix(Farmer __instance, int damage, bool overrideParry, Monster damager)
        {
            if (ModEntry.Invincible)
                return false;

            if (Game1.eventUp || __instance.FarmerSprite.isPassingOut())
            {
                return false;
            }
            bool num = damager != null && !damager.isInvincible() && !overrideParry;
            bool monsterDamageCapable = (damager == null || !damager.isInvincible()) && (damager == null || (damager is not GreenSlime && damager is not BigSlime) || !__instance.isWearingRing(520));
            bool playerParryable = __instance.CurrentTool != null && __instance.CurrentTool is MeleeWeapon weapon && weapon.isOnSpecial && weapon.type.Value == 3;
            bool playerDamageable = __instance.CanBeDamaged();
            if (num && playerParryable)
            {
                Rumble.rumble(0.75f, 150f);
                __instance.currentLocation.playSound("parry");
                if (damager is not null)
                    damager.parried(damage, __instance);
            }
            else
            {
                if (!(monsterDamageCapable && playerDamageable))
                    return false;

                damager?.onDealContactDamage(__instance);
                damage += Game1.random.Next(Math.Min(-1, -damage / 8), Math.Max(1, damage / 8));
                int effectiveResilience = __instance.resilience;
                if (__instance.CurrentTool is MeleeWeapon weapon1)
                    effectiveResilience += weapon1.addedDefense.Value;

                if (effectiveResilience >= damage * 0.5f)
                    effectiveResilience -= (int)(effectiveResilience * Game1.random.Next(3) / 10f);

                if (damager != null && (__instance.isWearingRing(839) || Perks.HasPerk(Perks.PerkType.Reflect)))
                {
                    Rectangle monsterBox = damager.GetBoundingBox();
                    Vector2 trajectory = Utility.getAwayFromPlayerTrajectory(monsterBox, __instance);

                    if (Curse.HasCurse(CurseType.PlayerKnockback))
                        trajectory *= 2f;

                    trajectory /= 2f;
                    int damageToMonster = damage;
                    int farmerDamage = Math.Max(1, damage - effectiveResilience);
                    if (farmerDamage < 10)
                        damageToMonster = (int)Math.Ceiling((damageToMonster + farmerDamage) / 2.0);

                    damager.takeDamage(damageToMonster, (int)trajectory.X, (int)trajectory.Y, isBomb: false, 1.0, __instance);
                    damager.currentLocation.debris.Add(new Debris(damageToMonster, new Vector2(monsterBox.Center.X + 16, monsterBox.Center.Y), new Color(255, 130, 0), 1f, damager));
                }
                if (__instance.isWearingRing(524) && !Game1.buffsDisplay.hasBuff(21) && Game1.random.NextDouble() < (0.9 - (double)(__instance.health / 100f)) / (3 - __instance.LuckLevel / 10) + ((__instance.health <= 15) ? 0.2 : 0.0))
                {
                    __instance.currentLocation.playSound("yoba");
                    Game1.buffsDisplay.addOtherBuff(new Buff(21));
                    return false;
                }
                Rumble.rumble(0.75f, 150f);
                int effectiveDamage = damage - effectiveResilience;
                damage = Math.Max(1, effectiveDamage);

                if (Perks.HasPerk(Perks.PerkType.Shield))
                    damage = (int)Math.Round(damage * 0.9f);

                if (Perks.HasPerk(Perks.PerkType.TurtleShell) && ShellCooldownSeconds == 0)
                {
                    __instance.currentLocation.playSound("crafting");
                    damage = (int)Math.Round(damage * 0.5f);
                    ShellCooldownSeconds = 10;
                }

                if (HatQuest.HasBuffFor(HatQuestType.SQUIRE_HELMET))
                    damage = (int)Math.Round(damage * 0.75f);

                if (Curse.HasCurse(CurseType.DamageOverTime))
                {
                    damage = (int)Math.Round(damage * 1.5f);
                    Curse.DOTDamageToTick += damage;
                }
                else
                {
                    __instance.health = Math.Max(0, __instance.health - damage);
                    if (Game1.player.get_FarmerActiveHatQuest() is not null)
                        Game1.player.get_FarmerActiveHatQuest()!.DamageTaken += damage;
                }

                if (Curse.HasCurse(CurseType.BrittleCrown) && Game1.random.NextDouble() < 0.9)
                {
                    int toLose = Game1.random.Next(10, 17) * damage;
                    __instance.currentLocation.playSound("sell");
                    __instance.currentLocation.debris.Add(new Debris(toLose, new Vector2(__instance.getStandingX(), __instance.getStandingY()), Color.Gold, 1f, __instance));
                    Game1.player.Money = Math.Max(0, Game1.player.Money - toLose);
                }

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
                        __instance.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Rectangle(114, 46, 2, 2), 200f, 5, 1, new Vector2(xPos + 32f, -96f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, 0f, 0f)
                        {
                            attachedCharacter = __instance,
                            positionFollowsAttachedCharacter = true,
                            motion = new Vector2(xPos / 32f, -3f),
                            delayBeforeAnimationStart = i * 50,
                            alphaFade = 0.001f,
                            acceleration = new Vector2(0f, 0.1f)
                        });
                    }
                    __instance.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Rectangle(157, 280, 28, 19), 2000f, 1, 1, new Vector2(-20f, -16f), flicker: false, flipped: false, 1E-06f, 0f, Color.White, 4f, 0f, 0f, 0f)
                    {
                        attachedCharacter = __instance,
                        positionFollowsAttachedCharacter = true,
                        alpha = 0.1f,
                        alphaFade = -0.01f,
                        alphaFadeFade = -0.00025f
                    });
                    __instance.health = (int)Math.Min(__instance.maxHealth, __instance.maxHealth * 0.5f + __instance.GetEffectsOfRingMultiplier(863));
                    __instance.hasUsedDailyRevive.Value = true;
                    Curse.DOTDamageToTick = 0;
                }

                if (damager is not null && damage > 0 && Curse.HasCurse(CurseType.PlayerKnockback))
                {
                    Vector2 trajectory = Utility.getAwayFromPositionTrajectory(__instance.GetBoundingBox(), damager.getStandingPosition());
                    __instance.setTrajectory((int)trajectory.X, (int)trajectory.Y);
                }

                if (Curse.AnyFarmerHasCurse(CurseType.MonsterBuffs) && Game1.random.NextDouble() < 0.5)
                {
                    int duration = 5000;

                    // Apply random buff/debuff
                    if (Game1.random.Next(11) >= __instance.immunity)
                    {
                        int[] sourceArray = Game1.random.NextDouble() < 0.5 && !__instance.hasBuff(28) ? Roguelike.RandomDebuffIds : Roguelike.RandomBuffIds;
                        int buffId = sourceArray[Game1.random.Next(sourceArray.Length)];
                        Buff buff = new(buffId)
                        {
                            millisecondsDuration = duration
                        };
                        Game1.buffsDisplay.addOtherBuff(buff);

                        if (buffId == 19)
                            __instance.currentLocation.playSound("frozen");
                        else
                            __instance.currentLocation.playSound("debuffHit");
                    }
                }

                __instance.temporarilyInvincible = true;
                __instance.temporaryInvincibilityTimer = 0;
                __instance.currentTemporaryInvincibilityDuration = 1200 + __instance.GetEffectsOfRingMultiplier(861) * 400;
                __instance.currentLocation.debris.Add(new Debris(damage, new Vector2(__instance.getStandingX() + 8, __instance.getStandingY()), Color.Red, 1f, __instance));
                __instance.currentLocation.playSound("ow");
                Game1.hitShakeTimer = 100 * damage;

                Game1.player.get_FarmerWasDamagedOnThisLevel().Value = true;
            }

            return false;
        }
    }
}
