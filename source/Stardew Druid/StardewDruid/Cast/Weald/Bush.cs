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
using StardewDruid.Journal;
using StardewValley;
using StardewValley.BellsAndWhistles;
using System;
using System.Collections.Generic;

namespace StardewDruid.Cast.Weald
{
    internal class Bush : CastHandle
    {

        public StardewValley.TerrainFeatures.Bush bushFeature;

        public Bush(Vector2 target,  StardewValley.TerrainFeatures.Bush bush)
            : base(target)
        {


            castCost = Math.Max(2, 6 - (Game1.player.ForagingLevel/ 2));

            bushFeature = bush;

        }

        public override void CastEffect()
        {

            if (bushFeature == null)
            {
            
                return;
         
            }


            if (!Mod.instance.questHandle.IsComplete(QuestHandle.bushLesson))
            {

                Mod.instance.questHandle.UpdateTask(QuestHandle.bushLesson, 1);

            }

            bushFeature.performToolAction(null, 1, targetVector);

            if(bushFeature.size.Value == 3)
            {

                int age = bushFeature.getAge();

                if (age < 20)
                {

                    int newage = age++;

                    int newdate = Math.Max(1,(int)Game1.stats.DaysPlayed - newage);

                    targetLocation.terrainFeatures.Remove(targetVector);

                    targetLocation.terrainFeatures.Add(targetVector, new StardewValley.TerrainFeatures.Bush(targetVector,3,targetLocation,newdate));

                    return;

                }

            }

            int probability = randomIndex.Next(25 - Mod.instance.rite.caster.ForagingLevel);

            if (probability > 1)
            {
                return;
            }

            int objectIndex = SpawnData.RandomBushForage(bushFeature,probability);

            int randomQuality = randomIndex.Next(11 - targetPlayer.foragingLevel.Value);

            int objectQuality = 0;

            if (randomQuality == 0)
            {

                objectQuality = 2;

            }

            if (targetPlayer.professions.Contains(16))
            {

                objectQuality = 4;

            }

            int throwAmount = 1;

            if (targetPlayer.professions.Contains(13))
            {

                throwAmount = randomIndex.Next(1, 3);

            }

            for (int i = 0; i < throwAmount; i++)
            {

                Throw throwObject = new(targetPlayer, targetVector * 64, objectIndex, objectQuality);

                throwObject.ThrowObject();

            };

            castFire = true;

            ModUtility.AnimateSparkles(targetLocation, targetVector, new(0.8f, 1, 0.8f, 1));

            targetPlayer.gainExperience(2, 2); // gain foraging experience

            bushFeature.performToolAction(null, 1, targetVector);

            if (Game1.currentSeason == "summer")
            {

                Game1.currentLocation.critters.Add(new Firefly(targetVector + new Vector2(randomIndex.Next(-2, 3), randomIndex.Next(-2, 3))));

                Game1.currentLocation.critters.Add(new Firefly(targetVector + new Vector2(randomIndex.Next(-2, 3), randomIndex.Next(-2, 3))));

            }
            else
            {

                Game1.currentLocation.critters.Add(new Butterfly(targetLocation,targetVector + new Vector2(randomIndex.Next(-2, 3), randomIndex.Next(-2, 3)), false));

                Game1.currentLocation.critters.Add(new Butterfly(targetLocation,targetVector + new Vector2(randomIndex.Next(-2, 3), randomIndex.Next(-2, 3)), false));

            }

        }

    }
}
