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
using StardewDruid.Event.Scene;
using StardewDruid.Journal;
using StardewValley;
using StardewValley.Locations;
using System.Collections.Generic;
using static StardewDruid.Data.IconData;

namespace StardewDruid.Cast.Weald
{
    internal class Lawn : CastHandle
    {

        public Lawn(Vector2 target)
            : base(target)
        {

            castCost = 6;

            if (Game1.player.ForagingLevel >= 8)
            {

                castCost = 4;

            }

        }

        public override void CastEffect()
        {

            bool forgotTrees = Mod.instance.EffectDisabled("Trees");

            Dictionary<string, List<Vector2>> neighbourList = ModUtility.NeighbourCheck(targetLocation, targetVector, 1, 0);

            int probability = randomIndex.Next(5);

            if (probability == 0 && Mod.instance.rite.spawnIndex["flower"] && neighbourList.Count == 0 && Mod.instance.questHandle.IsComplete(QuestHandle.spawnLesson))
            {

                int randomCrop = SpawnData.RandomFlower();

                StardewValley.Object newFlower = new(
                         randomCrop.ToString(), 1
                    );

                newFlower.IsSpawnedObject = true;

                newFlower.Location = targetLocation;

                newFlower.TileLocation = targetVector;

                if (targetLocation.objects.TryAdd(targetVector, newFlower))
                {

                    castFire = true;

                    Vector2 cursorVector = targetVector * 64 + new Vector2(0, 8);
                    Mod.instance.iconData.CursorIndicator(targetLocation, cursorVector, IconData.cursors.weald);

                }

                /*targetLocation.dropObject(
                    newFlower,
                    new Vector2(targetVector.X * 64, targetVector.Y * 64),
                    Game1.viewport,
                    initialPlacement: true
                );*/



            }
            else if (probability == 1 && Mod.instance.rite.spawnIndex["forage"] && neighbourList.Count == 0)
            {

                int randomCrop = SpawnData.RandomForage(targetLocation);

                StardewValley.Object newForage = new StardewValley.Object(
                    randomCrop.ToString(), 1
                );

                newForage.IsSpawnedObject = true;

                newForage.Location = targetLocation;

                newForage.TileLocation = targetVector;

                if (targetLocation.objects.TryAdd(targetVector, newForage))
                {

                    castFire = true;

                    Vector2 cursorVector = targetVector * 64 + new Vector2(0, 8);

                    Mod.instance.iconData.CursorIndicator(targetLocation, cursorVector, IconData.cursors.weald);

                    if (!Mod.instance.questHandle.IsComplete(QuestHandle.spawnLesson))
                    {

                        Mod.instance.questHandle.UpdateTask(QuestHandle.spawnLesson, 1);

                    }

                }

                /*targetLocation.dropObject(
                    newForage,
                    new Vector2(targetVector.X * 64, targetVector.Y * 64),
                    Game1.viewport,
                    initialPlacement: true
                );*/

            }
            else if (probability == 2)
            {

                int procChance = 65 - Mod.instance.CurrentProgress;

                if (randomIndex.Next(procChance) == 0 && Mod.instance.rite.spawnIndex["artifact"] && Game1.currentSeason == "winter")
                {

                    int tileX = (int)targetVector.X;
                    int tileY = (int)targetVector.Y;

                    if (targetLocation.getTileIndexAt(tileX, tileY, "AlwaysFront") == -1 &&
                        targetLocation.getTileIndexAt(tileX, tileY, "Front") == -1 &&
                        !targetLocation.isBehindBush(targetVector) &&
                        targetLocation.doesTileHaveProperty(tileX, tileY, "Diggable", "Back") != null
                    )
                    {

                        targetLocation.objects.Add(targetVector, new StardewValley.Object("590", 1));

                        castFire = true;

                        Vector2 cursorVector = targetVector * 64 + new Vector2(0, 8);

                        Mod.instance.iconData.CursorIndicator(targetLocation, cursorVector, IconData.cursors.weald);

                    }

                }


            }
            else if (Mod.instance.rite.spawnIndex["trees"] && neighbourList.Count == 0 && !forgotTrees)
            {

                bool treeSpawn = false;

                switch (targetPlayer.FacingDirection)
                {
                    case 0:

                        if (Mod.instance.rite.castVector.Y < targetVector.Y)
                        {
                            treeSpawn = ModUtility.RandomTree(targetLocation, targetVector);

                        }

                        break;

                    case 1:

                        if (Mod.instance.rite.castVector.X > targetVector.X)
                        {
                            treeSpawn = ModUtility.RandomTree(targetLocation, targetVector);

                        }
                        break;

                    case 2:

                        if (Mod.instance.rite.castVector.Y > targetVector.Y)
                        {
                            treeSpawn = ModUtility.RandomTree(targetLocation, targetVector);

                        }
                        break;

                    default:

                        if (Mod.instance.rite.castVector.X < targetVector.X)
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
