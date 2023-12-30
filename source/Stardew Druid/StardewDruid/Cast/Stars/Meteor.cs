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

namespace StardewDruid.Cast.Stars
{
    internal class Meteor : CastHandle
    {

        int targetDirection;

        int meteorRange;

        public Meteor(Vector2 target, Rite rite, int range = 2)
            : base(target, rite)
        {

            castCost = Math.Max(6, 14 - Game1.player.CombatLevel);

            targetDirection = rite.direction;

            meteorRange = range;

        }

        public override void CastEffect()
        {

            //ModUtility.AnimateMeteorZone(targetLocation, targetVector, new Color(1f, 0.4f, 0.4f, 1));

            ModUtility.AnimateMeteor(targetLocation, targetVector, targetDirection < 2);

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

            List<Vector2> impactVectors = ModUtility.Explode(targetLocation, targetVector, targetPlayer, meteorRange, (int)(riteData.castDamage * 1.5), powerLevel:2);

            foreach(Vector2 vector in impactVectors)
            {
                
                ModUtility.ImpactVector(targetLocation, vector);

            }

            if (randomIndex.Next(2) == 0) { Game1.currentLocation.playSound("flameSpellHit"); }

            castFire = true;

        }

    }
}
