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
using StardewModdingAPI;
using StardewValley;
using System;
using System.ComponentModel.Design;
using static StardewValley.Debris;

namespace StardewDruid.Cast
{
    internal class Grass : CastHandle
    {

        public Grass(Mod mod, Vector2 target, Rite rite)
            : base(mod, target, rite)
        {

            castCost = 0;

        }

        public override void CastEarth()
        {

            int probability = randomIndex.Next(50);

            if (probability >= 5)
            {
                return;

            }

            if (!targetLocation.terrainFeatures.ContainsKey(targetVector))
            {

                return;

            }

            if (targetLocation.terrainFeatures[targetVector] is not StardewValley.TerrainFeatures.Grass)
            {

                return;

            }

            StardewValley.TerrainFeatures.Grass grassFeature = targetLocation.terrainFeatures[targetVector] as StardewValley.TerrainFeatures.Grass;

            int tileX = (int)targetVector.X;

            int tileY = (int)targetVector.Y;

            if (randomIndex.Next(100) == 0) // 1:1000 chance
            {

                Throw ancientObject = new(114,0);

                ancientObject.ThrowObject(targetPlayer, targetVector);

                //Game1.createObjectDebris(114, tileX, tileY);

            }

            if(probability <= 1 && !riteData.castToggle.ContainsKey("forgetSeeds") && riteData.castTask.ContainsKey("masterCreature"))
            {

                switch (Game1.currentSeason)
                {

                    case "spring":

                        Throw springObject = new(495, 0);

                        springObject.ThrowObject(targetPlayer, targetVector);

                        //Game1.createObjectDebris(495, tileX, tileY);

                        break;

                    case "summer":

                        Throw summerObject = new(496, 0);

                        summerObject.ThrowObject(targetPlayer, targetVector);

                        //Game1.createObjectDebris(496, tileX, tileY);

                        break;

                    case "fall":

                        Throw fallObject = new(497, 0);

                        fallObject.ThrowObject(targetPlayer, targetVector);

                        //Game1.createObjectDebris(497, tileX, tileY);

                        break;

                    default:

                        break;

                }

            }  
            else
            {

                Dictionary<int, Throw> throwList = new();

                for (int i = 2; i < probability; i++)
                {
                    
                    throwList[i] = new(771, 0);

                    throwList[i].ThrowObject(targetPlayer, targetVector);

                    //Game1.createObjectDebris(771, tileX, tileY);

                }

            }

            Rectangle tileRectangle = new(tileX * 64 + 1, tileY * 64 + 1, 62, 62);

            grassFeature.doCollisionAction(tileRectangle,2,targetVector,null,Game1.currentLocation);

        }

    }
}
