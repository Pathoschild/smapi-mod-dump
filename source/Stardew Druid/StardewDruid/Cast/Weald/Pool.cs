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
using StardewDruid.Event.World;
using StardewValley;
using System.Collections.Generic;
using xTile.Dimensions;

namespace StardewDruid.Cast.Weald
{
    internal class Pool : CastHandle
    {

        public Pool(Vector2 target, Rite rite)
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

            /*if (randomIndex.Next(5) == 0 && riteData.spawnIndex["wildspawn"] && !Mod.instance.EffectDisabled("Wildspawn"))
            {
                
                if (!Mod.instance.eventRegister.ContainsKey("wildspawn"))
                {

                    new Event.World.Wildspawn(targetVector, riteData).EventTrigger();

                }
                
                (Mod.instance.eventRegister["wildspawn"] as Wildspawn).SpawnMonster(targetLocation, targetVector, new() { 0, }, "water", true);
               
                //StardewValley.Monsters.Monster spawnMonster = Mod.instance.SpawnMonster(targetLocation, targetVector, new() { 0, }, "water");

                //if (!riteData.castTask.ContainsKey("masterCreature")) //&& spawnMonster != null)
                //{

                //    Mod.instance.UpdateTask("lessonCreature", 1);

                //}

            }*/

            int objectIndex = Map.SpawnData.RandomPoolFish(targetLocation);

            int objectQuality = 0;

            int experienceGain = 8;

            Throw throwObject = new(targetPlayer, targetVector * 64, objectIndex, objectQuality);

            throwObject.ThrowObject();

            targetPlayer.currentLocation.playSound("pullItemFromWater");

            targetPlayer.gainExperience(1, experienceGain); // gain fishing experience

            castFire = true;

            bool targetDirection = targetPlayer.getTileLocation().X <= targetVector.X;

            ModUtility.AnimateSplash(targetLocation, targetVector, targetDirection);

        }

    }

}
