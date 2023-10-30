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
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics;
using System.Xml.Linq;
using xTile;

namespace StardewDruid.Cast
{
    internal class Meteor: CastHandle
    {

        int targetDirection;

        int meteorRange;

        public Meteor(Mod mod, Vector2 target, Rite rite, int range = 2)
            : base(mod, target, rite)
        {

            castCost = Math.Max(6,14-Game1.player.CombatLevel);

            targetDirection = rite.direction;

            meteorRange = range;

        }

        public override void CastStars()
        {

            ModUtility.AnimateMeteorZone(targetLocation, targetVector, new Color(1f,0.4f,0.4f,1), meteorRange);

            ModUtility.AnimateMeteor(targetLocation, targetVector, targetDirection < 2);

            DelayedAction.functionAfterDelay(MeteorImpact, 600);

            castFire = true;

        }

        public void MeteorImpact()
        {

            if(targetLocation != Game1.currentLocation)
            {

                return;

            }

            ModUtility.Explode(targetLocation, targetVector, targetPlayer, meteorRange, riteData.castDamage, 2, mod.virtualPick, mod.virtualAxe);

            castFire = true;

        }

    }
}
