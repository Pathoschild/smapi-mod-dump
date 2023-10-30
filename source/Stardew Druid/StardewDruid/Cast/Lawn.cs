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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

namespace StardewDruid.Cast
{
    internal class Lawn : CastHandle
    {

        private readonly Dictionary<string, bool> spawnIndex;

        public Lawn(Mod mod, Vector2 target, Rite rite)
            : base(mod, target, rite)
        {

            castCost = 4;

            if (rite.caster.ForagingLevel >= 8)
            {

                castCost = 2;

            }

            spawnIndex = rite.spawnIndex;

        }

        public override void CastEarth()
        {

            bool forgotTrees = riteData.castToggle.ContainsKey("forgetTrees");

            Dictionary<string, List<Vector2>> neighbourList = ModUtility.NeighbourCheck(targetLocation, targetVector);

            int probability = randomIndex.Next(120);

            if (probability <= 1 && spawnIndex["flower"] && neighbourList.Count == 0 && riteData.castTask.ContainsKey("masterForage")) // 2/120 flower
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
            else if (probability >= 2 && probability <= 3 && spawnIndex["forage"] && neighbourList.Count == 0) // 2/120 forage
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

                    mod.UpdateTask("lessonForage", 1);

                }

            }
            else if (probability >= 4 && probability <= 15 && spawnIndex["grass"] && neighbourList.ContainsKey("Tree") && !forgotTrees) // 12/120 grass
            {

                StardewValley.TerrainFeatures.Grass grassFeature = new(1, 4);

                targetLocation.terrainFeatures.Add(targetVector, grassFeature);

                Microsoft.Xna.Framework.Rectangle tileRectangle = new((int)targetVector.X * 64 + 1, (int)targetVector.Y * 64 + 1, 62, 62);

                grassFeature.doCollisionAction(tileRectangle, 2, targetVector, null, targetLocation);

                castFire = true;

                castCost = 0;

            }
            else if (probability >= 4 && probability <= 13 && spawnIndex["trees"] && neighbourList.Count == 0 && !forgotTrees) // 10/120 tree
            {

                bool treeSpawn = ModUtility.RandomTree(targetLocation, targetVector);

                if (treeSpawn)
                {

                    castCost = 4;

                    castFire = true;

                    ModUtility.AnimateGrowth(targetLocation, targetVector);

                }

            }
            else if (probability == 16)
            {

                if (spawnIndex["artifact"] && Game1.currentSeason == "winter" && mod.virtualHoe.UpgradeLevel >= 3)
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

                    }

                }

            }

        }

    }

}
