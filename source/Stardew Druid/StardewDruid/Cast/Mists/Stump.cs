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
using StardewValley.TerrainFeatures;
using System;

namespace StardewDruid.Cast.Mists
{
    internal class Stump : CastHandle
    {

        private ResourceClump resourceClump;

        private string resourceType;

        public Stump(Vector2 target, Rite rite, ResourceClump ResourceClump, string ResourceType)
            : base(target, rite)
        {

            resourceClump = ResourceClump;

            resourceType = ResourceType;

        }

        public override void CastEffect()
        {

            int axeLevel = Mod.instance.virtualAxe.UpgradeLevel;

            castCost = Math.Max(2, 36 - targetPlayer.ForagingLevel * axeLevel);

            if (resourceClump == null)
            {

                return;

            }

            ModUtility.DestroyStump(targetLocation, targetPlayer, resourceClump, targetVector, resourceType);

            resourceClump = null;

            castFire = true;

            ModUtility.AnimateBolt(targetLocation, targetVector);

            return;

        }

    }
}
