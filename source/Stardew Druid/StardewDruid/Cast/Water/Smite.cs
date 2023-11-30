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

namespace StardewDruid.Cast.Water
{
    internal class Smite : CastHandle
    {

        private StardewValley.Monsters.Monster targetMonster;

        public int colour;

        public Smite(Vector2 target, Rite rite, StardewValley.Monsters.Monster TargetMonster, int Colour = -1)
            : base(target, rite)
        {

            int castCombat = rite.caster.CombatLevel / 2;

            castCost = Math.Max(6, 12 - castCombat);

            targetMonster = TargetMonster;

            colour = Colour;

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

                critChance += 0.2f;

            }

            if (targetPlayer.professions.Contains(25))
            {

                critChance += 0.15f;

            }

            int damageApplied = randomIndex.Next(riteData.castDamage, riteData.castDamage * 2);

            int critModifier = 2;

            if (targetPlayer.professions.Contains(29))
            {
                critModifier += 1;

            }

            bool critApplied = false;

            if (randomIndex.NextDouble() <= critChance)
            {

                damageApplied *= critModifier;

                critApplied = true;

            }

            ModUtility.HitMonster(targetLocation, targetPlayer, targetMonster, damageApplied, critApplied);

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
