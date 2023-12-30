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
using StardewDruid.Map;

namespace StardewDruid.Cast.Weald
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

            int randomFish = SpawnData.RandomLowFish(targetLocation);

            int objectQuality = 0;

            int experienceGain;

            experienceGain = 8;

            if (randomIndex.Next(11 - targetPlayer.fishingLevel.Value) == 0)
            {

                objectQuality = 2;

                experienceGain = 16;

            }

            StardewDruid.Cast.Throw throwObject = new(targetPlayer, targetVector * 64, randomFish, objectQuality);

            throwObject.ThrowObject();

            targetPlayer.currentLocation.playSound("pullItemFromWater");

            targetPlayer.gainExperience(1, experienceGain); // gain fishing experience

            castFire = true;

            bool targetDirection = (targetPlayer.getTileLocation().X > targetVector.X); // false animation goes left to right, true animation right to left, check if target is right of left

            ModUtility.AnimateSplash(targetLocation, targetVector, targetDirection);

        }


    }

}
