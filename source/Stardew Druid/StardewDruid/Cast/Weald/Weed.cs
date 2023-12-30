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

namespace StardewDruid.Cast.Weald
{
    internal class Weed : CastHandle
    {

        public Weed(Vector2 target, Rite rite)
            : base(target, rite)
        {

            castCost = 1;

        }

        public override void CastEffect()
        {

            if (!targetLocation.objects.ContainsKey(targetVector))
            {

                return;

            }

            int powerLevel = Mod.instance.virtualAxe.UpgradeLevel;

            StardewValley.Object targetObject = targetLocation.objects[targetVector];

            targetObject.Fragility = 0;

            int explodeRadius = powerLevel < 2 ? 2 : powerLevel;

            if (targetLocation is StardewValley.Locations.MineShaft && targetObject.name.Contains("Stone"))
            {

                explodeRadius = Math.Min(6, 2 + powerLevel);

            }

            List<Vector2> impactVectors = ModUtility.Explode(targetLocation, targetVector, targetPlayer, explodeRadius, (int)(riteData.castDamage * 0.25), powerLevel:1);

            targetLocation.playSound("flameSpellHit");

            foreach (Vector2 vector in impactVectors)
            {
                ModUtility.ImpactVector(targetLocation, vector);

            }

            castFire = true;

        }

    }

}
