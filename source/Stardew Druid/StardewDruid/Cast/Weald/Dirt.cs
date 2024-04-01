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
using StardewValley;
using System.Collections.Generic;


namespace StardewDruid.Cast.Weald
{
    internal class Dirt : CastHandle
    {

        public Dirt(Vector2 target)
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

            int probability = randomIndex.Next(3);

            if (probability == 0)
            {

                int hoeLevel = Mod.instance.virtualHoe.UpgradeLevel;

                int procChance = 50 - 5 * hoeLevel;

                if (randomIndex.Next(procChance) == 0 && Mod.instance.rite.spawnIndex["artifact"] && hoeLevel >= 3)
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

                    }

                }

            }
            else if (Mod.instance.rite.spawnIndex["trees"] && !Mod.instance.EffectDisabled("Trees")) // 1/10 tree
            {

                bool treeSpawn  = false;

                switch (targetPlayer.FacingDirection)
                {
                    case 0:

                        if(Mod.instance.rite.castVector.Y < targetVector.Y)
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

            return;

        }

    }
}
