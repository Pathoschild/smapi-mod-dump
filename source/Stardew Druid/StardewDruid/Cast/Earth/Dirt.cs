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


namespace StardewDruid.Cast.Earth
{
    internal class Dirt : CastHandle
    {

        private readonly Dictionary<string, bool> spawnIndex;

        public Dirt(Vector2 target, Rite rite)
            : base(target, rite)
        {

            castCost = 6;

            if (rite.caster.ForagingLevel >= 8)
            {

                castCost = 4;

            }

            spawnIndex = rite.spawnIndex;

        }

        public override void CastEffect()
        {

            int probability = randomIndex.Next(3);

            if (probability == 0)
            {

                int hoeLevel = Mod.instance.virtualHoe.UpgradeLevel;

                int procChance = 50 - 5 * hoeLevel;

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

                        castFire = true;

                    }

                }

            }
            else if (spawnIndex["trees"] && !riteData.castToggle.ContainsKey("forgetTrees") && targetPlayer.facingDirection != targetPlayer.getGeneralDirectionTowards(targetVector)) // 1/10 tree
            {

                bool treeSpawn = ModUtility.RandomTree(targetLocation, targetVector);

                if (treeSpawn)
                {

                    castFire = true;

                }

            }

            return;

        }

    }
}
