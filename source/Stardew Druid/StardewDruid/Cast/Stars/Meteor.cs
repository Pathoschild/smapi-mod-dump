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
using System;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;


namespace StardewDruid.Cast.Stars
{
    internal class Meteor : CastHandle
    {

        int targetDirection;

        float damage;

        public Meteor(Vector2 target, Rite rite, float Damage)
            : base(target, rite)
        {

            castCost = Math.Max(6, 14 - Game1.player.CombatLevel);

            targetDirection = rite.direction;

            damage = Damage;

        }

        public override void CastEffect()
        {

            ModUtility.AnimateMeteor(targetLocation, targetVector, targetDirection < 2);

            ModUtility.AnimateRadiusDecoration(targetLocation, targetVector, "Stars", 0.75f, 0.75f, 1000);

            DelayedAction.functionAfterDelay(MeteorImpact, 600);

            if (randomIndex.Next(2) == 0) { Game1.currentLocation.playSound("fireball"); }

            castFire = true;

        }

        public void MeteorImpact()
        {

            if (targetLocation != Game1.currentLocation)
            {

                return;

            }

            ModUtility.DamageMonsters(targetLocation, ModUtility.MonsterProximity(targetLocation, targetVector * 64, 2, true), targetPlayer,(int)damage, true);

            List<Vector2> impactVectors = ModUtility.Explode(targetLocation, targetVector, targetPlayer, 2, powerLevel:2);

            foreach(Vector2 vector in impactVectors)
            {
                
                ModUtility.AnimateDestruction(targetLocation, vector);

            }

            if (randomIndex.Next(2) == 0) { Game1.currentLocation.playSound("flameSpellHit"); }

            castFire = true;

        }

    }

}
