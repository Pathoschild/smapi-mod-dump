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
using StardewValley;
using StardewValley.Minigames;
using StardewValley.Tools;
using System;

namespace StardewDruid.Cast.Weald
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

            int randomFish = SpawnData.RandomLowFish(targetLocation);

            int objectQuality = 0;

            int experienceGain;

            experienceGain = 8;

            if(11 - targetPlayer.fishingLevel.Value <= 0)
            {

                objectQuality = 2;

                experienceGain = 16;

            }
            else if (randomIndex.Next(11 - targetPlayer.fishingLevel.Value) == 0)
            {

                objectQuality = 2;

                experienceGain = 16;

            }

            StardewDruid.Cast.Throw throwObject = new(targetPlayer, targetVector * 64, randomFish, objectQuality);

            //Game1.player.checkForQuestComplete(null, randomFish, 1, throwObject.objectInstance, "fish", 7);

            Game1.player.checkForQuestComplete(null, randomFish, 1, null, null, 7);

            throwObject.ThrowObject();

            targetPlayer.currentLocation.playSound("pullItemFromWater");

            targetPlayer.gainExperience(1, experienceGain); // gain fishing experience

            castFire = true;

            bool targetDirection = (targetPlayer.Tile.X > targetVector.X);

            ModUtility.AnimateSplash(targetLocation, targetVector, targetDirection);
            Vector2 cursorVector = targetVector * 64;
            ModUtility.AnimateCursor(targetLocation, cursorVector);
        }


    }

}
