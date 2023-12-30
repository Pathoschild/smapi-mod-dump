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
using System.Collections.Generic;

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

            if (randomIndex.Next(5) == 0 && riteData.spawnIndex["wildspawn"] && !Mod.instance.EffectDisabled("Wildspawn"))
            {

                StardewValley.Monsters.Monster spawnMonster = Mod.instance.SpawnMonster(targetLocation, targetVector, new() { 0, }, "water");

                if (!riteData.castTask.ContainsKey("masterCreature") && spawnMonster != null)
                {

                    Mod.instance.UpdateTask("lessonCreature", 1);

                }

            }

            Dictionary<int, int> objectIndexes;

            if (targetLocation.Name.Contains("Beach"))
            {

                objectIndexes = new Dictionary<int, int>()
                {

                    [0] = 392, // nautilus shell
                    [1] = 152, // seaweed
                    [2] = 152, // seaweed
                    [3] = 397, // urchin
                    [4] = 718, // cockle
                    [5] = 715, // lobster
                    [6] = 720, // shrimp
                    [7] = 719, // mussel
                };

            }
            else
            {

                objectIndexes = new Dictionary<int, int>()
                {

                    [0] = 153, // algae
                    [1] = 153, // algae
                    [2] = 153, // algae
                    [3] = 153, // algae
                    [4] = 721, // snail 721
                    [5] = 716, // crayfish 716
                    [6] = 722, // periwinkle 722
                    [7] = 717, // crab 717

                };

            }

            int probability = randomIndex.Next(objectIndexes.Count);

            int objectIndex = objectIndexes[probability];

            int objectQuality = 0;

            int experienceGain;

            if (probability <= 3)
            {

                experienceGain = 6;

            }
            else
            {

                experienceGain = 12;

            }

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
