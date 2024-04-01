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

namespace StardewDruid.Cast.Mists
{
    internal class Water : CastHandle
    {

        public Water(Vector2 target)
            : base(target)
        {

            castCost = 8;

            if (Game1.player.FishingLevel >= 6)
            {

                castCost = 4;

            }

        }

        public override void CastEffect()
        {

            castCost = Math.Max(8, 48 - (targetPlayer.FishingLevel * 3));

            Fishspot fishspotEvent = new(targetVector);

            fishspotEvent.EventTrigger();

            castFire = true;

            return;

        }

    }

}
