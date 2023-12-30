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
using System.Collections.Generic;

namespace StardewDruid.Cast.Weald
{
    internal class Lawn : CastHandle
    {

        public Lawn(Vector2 target, Rite rite)
            : base(target, rite)
        {

            castCost = 6;

            if (rite.caster.ForagingLevel >= 8)
            {

                castCost = 4;

            }

        }

        public override void CastEffect()
        {

            bool forgotTrees = Mod.instance.EffectDisabled("Trees");

            Dictionary<string, List<Vector2>> neighbourList = ModUtility.NeighbourCheck(targetLocation, targetVector);

            int probability = randomIndex.Next(5);

            if (probability == 0 && riteData.spawnIndex["flower"] && neighbourList.Count == 0 && riteData.castTask.ContainsKey("masterForage")) // 2/120 flower
            {

                int randomCrop = SpawnData.RandomFlower();

                StardewValley.Object newFlower = new(
                        targetVector,
                        randomCrop,
                        null,
                        canBeSetDown: false,
                        canBeGrabbed: true,
                        isHoedirt: false,
                        isSpawnedObject: true
                    );

                targetLocation.dropObject(
                    newFlower,
                    new Vector2(targetVector.X * 64, targetVector.Y * 64),
                    Game1.viewport,
                    initialPlacement: true
                );

                castFire = true;

            }
            else if (probability == 1 && riteData.spawnIndex["forage"] && neighbourList.Count == 0) // 2/120 forage
            {

                int randomCrop = SpawnData.RandomForage(targetLocation);

                StardewValley.Object newForage = new StardewValley.Object(
                    targetVector,
                    randomCrop,
                    null,
                    canBeSetDown: false,
                    canBeGrabbed: true,
                    isHoedirt: false,
                    isSpawnedObject: true
                );

                targetLocation.dropObject(
                    newForage,
                    new Vector2(targetVector.X * 64, targetVector.Y * 64),
                    Game1.viewport,
                    initialPlacement: true
                );

                castFire = true;

                if (!riteData.castTask.ContainsKey("masterForage"))
                {

                    Mod.instance.UpdateTask("lessonForage", 1);

                }

            }
            else if (probability == 2)
            {

                if (riteData.spawnIndex["artifact"] && Game1.currentSeason == "winter" && Mod.instance.virtualHoe.UpgradeLevel >= 3)
                {

                    int tileX = (int)targetVector.X;
                    int tileY = (int)targetVector.Y;

                    if (targetLocation.getTileIndexAt(tileX, tileY, "AlwaysFront") == -1 &&
                        targetLocation.getTileIndexAt(tileX, tileY, "Front") == -1 &&
                        !targetLocation.isBehindBush(targetVector) &&
                        targetLocation.doesTileHaveProperty(tileX, tileY, "Diggable", "Back") != null
                    )
                    {

                        targetLocation.objects.Add(targetVector, new StardewValley.Object(targetVector, 590, 1));

                        castFire = true;

                    }

                }


            }
            else if (riteData.spawnIndex["trees"] && neighbourList.Count == 0 && !forgotTrees) // 10/120 tree
            {

                bool treeSpawn = false;

                switch (targetPlayer.FacingDirection)
                {
                    case 0:

                        if (riteData.castVector.Y < targetVector.Y)
                        {
                            treeSpawn = ModUtility.RandomTree(targetLocation, targetVector);

                        }

                        break;

                    case 1:

                        if (riteData.castVector.X > targetVector.X)
                        {
                            treeSpawn = ModUtility.RandomTree(targetLocation, targetVector);

                        }
                        break;

                    case 2:

                        if (riteData.castVector.Y > targetVector.Y)
                        {
                            treeSpawn = ModUtility.RandomTree(targetLocation, targetVector);

                        }
                        break;

                    default:

                        if (riteData.castVector.X < targetVector.X)
                        {
                            treeSpawn = ModUtility.RandomTree(targetLocation, targetVector);

                        }
                        break;

                }

                if (treeSpawn)
                {

                    castFire = true;

                }
            }


        }

    }

}
