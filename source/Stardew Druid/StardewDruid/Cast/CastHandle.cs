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

    public class CastHandle
    {
        public Vector2 targetVector { get; set; }

        public readonly Farmer targetPlayer;

        public readonly GameLocation targetLocation;

        public readonly Rite riteData;

        public bool castFire { get; set; }

        public int castCost { get; set; }

        public bool castLimit { get; set; }

        public Random randomIndex;

        public CastHandle(Vector2 Vector, Rite rite)
        {

            targetVector = Vector;

            randomIndex = rite.randomIndex;

            riteData = rite;

            targetPlayer = riteData.caster;

            targetLocation = riteData.castLocation;

            castFire = false;

            castCost = 2;

            castLimit = false;

        }

        public virtual void CastEffect()
        {

        }


    }
}
