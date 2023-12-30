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

namespace StardewDruid.Cast.Mists
{
    internal class Water : CastHandle
    {

        public Water(Vector2 target, Rite rite)
            : base(target, rite)
        {

            castCost = 8;

            if (rite.caster.FishingLevel >= 6)
            {

                castCost = 4;

            }

        }

        public override void CastEffect()
        {

            castCost = Math.Max(8, 48 - (targetPlayer.FishingLevel * 3));

            Event.World.Fishspot fishspotEvent = new(targetVector, riteData);

            fishspotEvent.EventTrigger();

            castFire = true;

            return;

        }

    }

}
