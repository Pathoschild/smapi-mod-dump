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


namespace StardewDruid.Cast
{
    internal class Dirt : CastHandle
    {

        private readonly Dictionary<string, bool> spawnIndex;

        public Dirt(Mod mod, Vector2 target, Rite rite)
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

            Dictionary<string, List<Vector2>> neighbourList = ModUtility.NeighbourCheck(targetLocation, targetVector);

            int probability = randomIndex.Next(10);

            if (probability <= 1 && spawnIndex["grass"] && neighbourList.ContainsKey("Tree") && !riteData.castToggle.ContainsKey("forgetTrees")) // 2/10 grass
            {

                StardewValley.TerrainFeatures.Grass grassFeature = new(1, 4);

                targetLocation.terrainFeatures.Add(targetVector, grassFeature);

                Microsoft.Xna.Framework.Rectangle tileRectangle = new((int)targetVector.X * 64 + 1, (int)targetVector.Y * 64 + 1, 62, 62);

                grassFeature.doCollisionAction(tileRectangle, 2, targetVector, null, targetLocation);

                castCost = 0;

                castFire = true;

            }
            else if (probability == 2 && spawnIndex["trees"] && neighbourList.Count == 0 && !riteData.castToggle.ContainsKey("forgetTrees")) // 1/10 tree
            {

                bool treeSpawn = ModUtility.RandomTree(targetLocation, targetVector);

                if (treeSpawn)
                {

                    castCost = 4;

                    castFire = true;

                    ModUtility.AnimateGrowth(targetLocation, targetVector);

                }

            }
            else if (probability == 3)
            {

                int hoeLevel = mod.virtualHoe.UpgradeLevel;

                int procChance = 50 - (5  * hoeLevel);

                if (randomIndex.Next(procChance) == 0 && spawnIndex["artifact"] && hoeLevel >= 3)
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

            return;

        }

    }
}
