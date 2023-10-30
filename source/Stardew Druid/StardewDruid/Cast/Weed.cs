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

namespace StardewDruid.Cast
{
    internal class Weed : CastHandle
    {

        public Weed(Mod mod, Vector2 target, Rite rite)
            : base(mod, target, rite)
        {

            castCost = 1;

        }

        public override void CastEarth()
        {

            if(!targetLocation.objects.ContainsKey(targetVector))
            {

                return;

            }

            int powerLevel = mod.virtualAxe.UpgradeLevel;

            StardewValley.Object targetObject = targetLocation.objects[targetVector];

            targetObject.Fragility = 0;

            int explodeRadius = (powerLevel < 2) ? 2 : powerLevel;

            if (targetLocation is StardewValley.Locations.MineShaft && targetObject.name.Contains("Stone"))
            {

                explodeRadius = Math.Min(6,2 + powerLevel);

            }

            ModUtility.Explode(targetLocation, targetVector, targetPlayer, explodeRadius, (int)(riteData.castDamage*0.25), 1, mod.virtualPick, mod.virtualAxe);

            castFire = true;

        }

    }

}
