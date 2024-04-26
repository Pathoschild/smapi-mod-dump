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
using StardewDruid.Data;
using StardewValley;
using System.Collections.Generic;
using xTile.Dimensions;

namespace StardewDruid.Cast.Weald
{
    internal class Pool : CastHandle
    {

        public Pool(Vector2 target)
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

            int objectIndex = SpawnData.RandomPoolFish(targetLocation);

            int objectQuality = 0;

            int experienceGain = 8;

            Throw throwObject = new(targetPlayer, targetVector * 64, objectIndex, objectQuality);

            throwObject.ThrowObject();

            targetPlayer.currentLocation.playSound("pullItemFromWater");

            targetPlayer.gainExperience(1, experienceGain); // gain fishing experience

            castFire = true;

            bool targetDirection = targetPlayer.Tile.X <= targetVector.X;

            ModUtility.AnimateSplash(targetLocation, targetVector, targetDirection);
            Vector2 cursorVector = targetVector * 64;
            Mod.instance.iconData.CursorIndicator(targetLocation, cursorVector, IconData.cursors.weald);
        }

    }

}
