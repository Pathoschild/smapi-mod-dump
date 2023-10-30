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
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;
using static StardewValley.Minigames.MineCart.Whale;

namespace StardewDruid.Cast
{
    internal class Water : CastHandle
    {

        public Water(Mod mod, Vector2 target, Rite rite)
            : base(mod, target, rite)
        {

        }

        public override void CastEarth()
        {
            
            /*if (randomIndex.Next(5) == 0 && riteData.spawnIndex["critter"] && !riteData.castToggle.ContainsKey("forgetCritters"))
            {

                int spawnMonster = mod.SpawnMonster(targetLocation, targetVector, new() { 0, }, "water");

                if (!riteData.castTask.ContainsKey("masterCreature") && spawnMonster >= 1)
                {

                    mod.UpdateTask("lessonCreature", 1);

                }

            }*/

            int randomFish = SpawnData.RandomLowFish(targetLocation);

            int objectQuality = 0;

            int experienceGain;

            experienceGain = 8;

            if (randomIndex.Next(11 - targetPlayer.fishingLevel.Value) == 0)
            {

                objectQuality = 2;

                experienceGain = 16;

            }

            StardewDruid.Cast.Throw throwObject = new(randomFish, objectQuality);

            throwObject.ThrowObject(targetPlayer, targetVector);

            targetPlayer.currentLocation.playSound("pullItemFromWater");

            targetPlayer.gainExperience(1, experienceGain); // gain fishing experience

            castFire = true;

            bool targetDirection = (targetPlayer.getTileLocation().X <= targetVector.X);

            ModUtility.AnimateSplash(targetLocation,targetVector,targetDirection);

        }

        public override void CastWater() {

            castCost = Math.Max(8, 48 - (targetPlayer.FishingLevel * 3));

            Event.FishSpot fishspotEvent = new(mod, targetVector, riteData, new Quest());

            fishspotEvent.EventTrigger();

            //castLimit = true;

            castFire = true;

            return;

        }

    }

}
