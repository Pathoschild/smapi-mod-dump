/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Tools;
using System;
using System.Threading;

namespace StardewDruid.Cast
{
    internal class Smite: CastHandle
    {

        private StardewValley.Monsters.Monster targetMonster;

        public Smite(Mod mod, Vector2 target, Rite rite, StardewValley.Monsters.Monster TargetMonster)
            : base(mod, target, rite)
        {

            int castCombat = rite.caster.CombatLevel / 2;

            castCost = Math.Max(6, 12-castCombat);

            targetMonster = TargetMonster;

        }

        public override void CastWater()
        {

            if(targetMonster == null || targetMonster.Health <= 0 || !targetLocation.characters.Contains(targetMonster))
            {

                return;

            }

            float critChance = 0.1f;

            if (!riteData.castTask.ContainsKey("masterSmite"))
            {

                mod.UpdateTask("lessonSmite", 1);

            }
            else
            {

                critChance += 0.2f;


                //if(targetPlayer.CurrentTool is MeleeWeapon meleeWeapon)
                //{
                 //   critChance += meleeWeapon.critChance.Value * 0.025f;

                //}

            }

            if (targetPlayer.professions.Contains(25))
            {

                critChance += 0.15f;

            }

            //Rectangle areaOfEffect = targetMonster.GetBoundingBox();

            //targetMonster.invincibleCountdown = 0;

            //Rectangle monsterHitbox = targetMonster.nextPosition(targetMonster.FacingDirection);

            //Rectangle areaOfEffect = new(monsterHitbox.X - 8, monsterHitbox.Y - 8, monsterHitbox.Width + 16, monsterHitbox.Height + 16);

            //targetLocation.damageMonster(areaOfEffect, riteData.castDamage, riteData.castDamage * 2, false, 0f, targetPlayer.CombatLevel, critChance, 2, false, targetPlayer);

            int damageApplied = randomIndex.Next(riteData.castDamage, riteData.castDamage * 2);

            int critModifier = 2;

            if (targetPlayer.professions.Contains(29))
            {
                critModifier += 1;

            }

            bool critApplied = false;

            if(randomIndex.NextDouble() <= critChance){

                damageApplied *= critModifier;

                //targetLocation.playSound("crit");

                critApplied = true;

            }

            int damageDealt = targetMonster.takeDamage(damageApplied, 0, 0, false, 999f, targetPlayer);

            if (damageDealt <= 0)
            {
                return;            
            }

            foreach (BaseEnchantment enchantment in targetPlayer.enchantments)
            {
                enchantment.OnCalculateDamage(targetMonster, targetLocation, targetPlayer, ref damageDealt);
            }

            targetLocation.removeDamageDebris(targetMonster);

            Microsoft.Xna.Framework.Rectangle boundingBox = targetMonster.GetBoundingBox();

            targetLocation.debris.Add(new Debris(damageDealt, new Vector2(boundingBox.Center.X + 16, boundingBox.Center.Y), critApplied ? Color.Yellow : new Color(255, 130, 0), critApplied ? (1f + (float)damageDealt / 300f) : 1f, targetMonster));

            foreach (BaseEnchantment enchantment2 in targetPlayer.enchantments)
            {
                enchantment2.OnDealDamage(targetMonster, targetLocation, targetPlayer, ref damageDealt);
            }

            if (targetMonster.Health <= 0)
            {
                targetPlayer.checkForQuestComplete(null, 1, 1, null, targetMonster.Name, 4);

                if (Game1.player.team.specialOrders is not null)
                {
                    foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
                    {
                        if (specialOrder.onMonsterSlain != null)
                        {
                            specialOrder.onMonsterSlain(Game1.player, targetMonster);
                        }
                    }
                }

                foreach (BaseEnchantment enchantment3 in targetPlayer.enchantments)
                {
                    enchantment3.OnMonsterSlay(targetMonster, targetLocation, targetPlayer);
                }

                if (targetPlayer.leftRing.Value != null)
                {
                    targetPlayer.leftRing.Value.onMonsterSlay(targetMonster, targetLocation, targetPlayer);
                }

                if (targetPlayer.rightRing.Value != null)
                {
                    targetPlayer.rightRing.Value.onMonsterSlay(targetMonster, targetLocation, targetPlayer);
                }

                if (!(targetMonster is GreenSlime) || (bool)(targetMonster as GreenSlime).firstGeneration.Value)
                {
                    if (targetPlayer.IsLocalPlayer)
                    {
                        Game1.stats.monsterKilled(targetMonster.Name);
                    }
                    else if (Game1.IsMasterGame)
                    {
                        targetPlayer.queueMessage(25, Game1.player, targetMonster.Name);
                    }
                }

                targetLocation.monsterDrop(targetMonster, boundingBox.Center.X, boundingBox.Center.Y, targetPlayer);

                targetPlayer.gainExperience(4, targetMonster.ExperienceGained);

                if ((bool)targetMonster.isHardModeMonster.Value)
                {
                    Game1.stats.incrementStat("hardModeMonstersKilled", 1);
                }

                targetLocation.characters.Remove(targetMonster);

                Game1.stats.MonstersKilled++;

                ModUtility.AnimateBolt(targetLocation, new Vector2(targetVector.X, targetVector.Y - 1), 1200);

            }
            else
            {

                ModUtility.AnimateBolt(targetLocation, new Vector2(targetVector.X, targetVector.Y - 1), 600+(randomIndex.Next(1,8)*100));

            }
            

            /*if (randomIndex.Next(2) == 0)
            {

                castLimit = true;

            }*/

            castFire = true;

            return;

        }

    }

}
