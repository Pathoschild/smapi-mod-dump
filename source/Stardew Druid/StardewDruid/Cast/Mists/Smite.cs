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
using System;
using System.Collections.Generic;

namespace StardewDruid.Cast.Mists
{
    internal class Smite : CastHandle
    {

        private StardewValley.Monsters.Monster targetMonster;

        public int colour;

        public float damage;

        public Smite(Vector2 target, Rite rite, StardewValley.Monsters.Monster TargetMonster, float Damage, int Colour = -1)
            : base(target, rite)
        {

            int castCombat = rite.caster.CombatLevel / 2;

            castCost = Math.Max(6, 12 - castCombat);

            targetMonster = TargetMonster;

            colour = Colour;

            damage = Damage;

        }

        public override void CastEffect()
        {

            if (targetMonster == null || targetMonster.Health <= 0 || !targetLocation.characters.Contains(targetMonster))
            {

                return;

            }

            float critChance = 0.1f;

            if (!riteData.castTask.ContainsKey("masterSmite"))
            {

                Mod.instance.UpdateTask("lessonSmite", 1);

            }
            else
            {

                critChance += 0.3f;

            }

            int damageApplied = (int)(damage * 0.7);

            bool critApplied = false;

            float critDamage = ModUtility.CalculateCritical(damageApplied, critChance);

            if (critDamage > 0)
            {

                damageApplied = (int)critDamage;

                critApplied = true;

            }

            List<int> diff = ModUtility.CalculatePush(targetLocation, targetMonster, targetPlayer.Position, 64);

            ModUtility.HitMonster(targetLocation, targetPlayer, targetMonster, damageApplied, critApplied, diffX: diff[0], diffY: diff[1]);

            if (targetMonster.Health <= 0)
            {

                ModUtility.AnimateBolt(targetLocation, new Vector2(targetVector.X, targetVector.Y - 1), 1200, colour);

            }
            else
            {

                ModUtility.AnimateBolt(targetLocation, new Vector2(targetVector.X, targetVector.Y - 1), 600 + randomIndex.Next(1, 8) * 100, colour);

            }

            castFire = true;

        }

    }

}
