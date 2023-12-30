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
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

namespace StardewDruid.Cast.Weald
{
    internal class Rockfall : CastHandle
    {

        public int debrisIndex;

        public int powerLevel;

        public bool challengeCast;

        public bool generateRock;

        //public bool generateHoed;

        public int castDelay;

        public Rockfall(Vector2 target, Rite rite)
            : base(target, rite)
        {

            castCost = 1;

            powerLevel = Mod.instance.virtualPick.UpgradeLevel;

            castDelay = 0;

        }

        public override void CastEffect()
        {

            int tileX = (int)targetVector.X;

            int tileY = (int)targetVector.Y;

            Layer backLayer = riteData.castLocation.Map.GetLayer("Back");

            Tile backTile = backLayer.PickTile(new Location(tileX * 64, tileY * 64), Game1.viewport.Size);

            if (backTile == null)
            {

                return;

            }

            if (backTile.TileIndexProperties.TryGetValue("Type", out var typeValue))
            {

                //if (typeValue == "Stone")
                //{

                    generateRock = true;

                //}
                //else if (typeValue == "Dirt")
                //{

                //    generateHoed = true;

                //}

            }

            List<int> indexes = Map.SpawnData.RockFall(targetLocation, targetPlayer, Mod.instance.rockCasts[riteData.castLocation.Name]);

            int objectIndex = indexes[0];

            int scatterIndex = indexes[1];

            debrisIndex = indexes[2];

            ModUtility.AnimateRockfall(this.targetLocation, this.targetVector, this.castDelay, objectIndex, scatterIndex);

            // ------------------------------ impacts

            //DelayedAction.functionAfterDelay(DebrisImpact, 575 + castDelay);

            //if (generateRock)
            //{

                DelayedAction.functionAfterDelay(RockImpact, 600 + castDelay);

            //}

            //if (generateHoed)
            //{

             //   DelayedAction.functionAfterDelay(DirtImpact, 600 + castDelay);

            //}

            castFire = true;

        }

        /*public void DebrisImpact()
        {
            ModUtility.ImpactVector(targetLocation, targetVector);

            if (riteData.castTask.ContainsKey("masterRockfall") || challengeCast)
            {

                Microsoft.Xna.Framework.Rectangle areaOfEffect = new(
                    (int)targetVector.X * 64 - 32,
                    (int)targetVector.Y * 64 - 32,
                    128,
                    128
                );

                int castDamage = riteData.castDamage / 2;

                targetLocation.damageMonster(areaOfEffect, castDamage, riteData.castDamage, true, targetPlayer);

            }

        }*/

        public void RockImpact()
        {

            ModUtility.ImpactVector(targetLocation, targetVector);

            ModUtility.Explode(targetLocation, targetVector, targetPlayer, 2, riteData.castDamage / 3, powerLevel:1, dirt:1);

            if (!generateRock) { return; }

            int rockCut = randomIndex.Next(2);

            int generateAmt = 1 + randomIndex.Next(powerLevel) / 2;

            for (int i = 0; i < generateAmt; i++)
            {
                //Throw throwObject;
                if (i == 0)
                {

                    if (targetPlayer.professions.Contains(21) && rockCut == 0)
                    {

                        Game1.createObjectDebris(382, (int)targetVector.X, (int)targetVector.Y);
                        //throwObject = new(targetPlayer, targetVector * 64, 382);

                        //throwObject.ThrowObject();
                    }
                    else if (targetPlayer.professions.Contains(19) && rockCut == 0)
                    {

                        Game1.createObjectDebris(debrisIndex, (int)targetVector.X, (int)targetVector.Y);
                        //throwObject = new(targetPlayer, targetVector * 64, debrisIndex);

                        //throwObject.ThrowObject();
                    }

                    Game1.createObjectDebris(debrisIndex, (int)targetVector.X, (int)targetVector.Y);
                    //throwObject = new(targetPlayer, targetVector * 64, debrisIndex);

                    //throwObject.ThrowObject();
                }
                else
                {

                    Game1.createObjectDebris(390, (int)targetVector.X, (int)targetVector.Y);
                    //throwObject = new(targetPlayer, targetVector * 64, 390);

                    //throwObject.ThrowObject();
                
                }

            }

            if (!riteData.castTask.ContainsKey("masterRockfall"))
            {

                Mod.instance.UpdateTask("lessonRockfall", generateAmt);

            }

        }

        /*public void DirtImpact()
        {

            List<Vector2> tileVectors = new()
            {
                targetVector,
                targetVector + new Vector2(-1,0),
                targetVector + new Vector2(0,-1),
                targetVector + new Vector2(1,0),
                targetVector + new Vector2(0,1),

            };

            foreach (Vector2 tileVector in tileVectors)
            {
                if (!targetLocation.objects.ContainsKey(tileVector) && targetLocation.doesTileHaveProperty((int)tileVector.X, (int)tileVector.Y, "Diggable", "Back") != null && !targetLocation.isTileHoeDirt(tileVector))
                {

                    targetLocation.checkForBuriedItem((int)tileVector.X, (int)tileVector.Y, explosion: true, detectOnly: false, targetPlayer);

                    targetLocation.makeHoeDirt(tileVector);

                }

            }

        }*/

    }

}
