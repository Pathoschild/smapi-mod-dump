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
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using xTile.Dimensions;
using xTile.Tiles;

namespace StardewDruid.Cast
{
    internal class Rockfall: CastHandle
    {

        //private readonly MineShaft shaftLocation;

        //public int objectIndex;

        //public int objectStrength;

        public int debrisIndex;

        public int powerLevel;

        public bool challengeCast;

        public bool generateRock;

        public bool generateHoed;

        public Rockfall(Mod mod, Vector2 target, Rite rite, bool rock = false, bool hoed = false)
            : base(mod, target, rite)
        {

            castCost = 1;

            powerLevel = mod.virtualPick.UpgradeLevel;

            generateRock = rock;

            generateHoed = hoed;

        }

        public override void CastEarth()
        {

            int tileX = (int)targetVector.X;

            int tileY = (int)targetVector.Y;

            List<int> indexes = Map.SpawnData.RockFall(targetLocation,targetPlayer);

            int objectIndex = indexes[0];

            int scatterIndex = indexes[1];

            debrisIndex = indexes[2];

            Microsoft.Xna.Framework.Rectangle objectRectangle = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, objectIndex, 16, 16);

            Microsoft.Xna.Framework.Rectangle scatterRectangle = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, scatterIndex, 16, 16);

            float animationInterval = 575f;

            Vector2 animationPosition;

            float animationAcceleration;

            float animationSort;

            TemporaryAnimatedSprite animationScatter;

            TemporaryAnimatedSprite animationRock;

            // ----------------------------- large debris

            animationPosition = new(targetVector.X * 64 + 8, (targetVector.Y - 3) * 64 - 24);

            animationAcceleration = 0.0015f;

            animationSort = float.Parse("0.0" + targetVector.X.ToString() + targetVector.Y.ToString() + "1");

            animationScatter = new("Maps\\springobjects", scatterRectangle, animationInterval, 1, 0, animationPosition, flicker: false, flipped: false, animationSort, 0.001f, Color.White, 3f, 0f, 0f, 0f)
            {
                acceleration = new Vector2(0, animationAcceleration),
                timeBasedMotion = true,
            };

            targetPlayer.currentLocation.temporarySprites.Add(animationScatter);

            // ------------------------------ rock generation

            animationPosition = new(targetVector.X * 64 + 8, (targetVector.Y - 3) * 64 + 8);

            animationSort = float.Parse("0.0" + targetVector.X.ToString() + targetVector.Y.ToString() + "2");

            animationRock = new("Maps\\springobjects", objectRectangle, animationInterval, 1, 0, animationPosition, flicker: false, flipped: false, animationSort, 0.001f, Color.White, 3f, 0f, 0f, 0f)
            {
                acceleration = new Vector2(0, animationAcceleration),
                timeBasedMotion = true,
            };

            targetPlayer.currentLocation.temporarySprites.Add(animationRock);

            // ----------------------------- shadow

            animationPosition = new(targetVector.X * 64 + 16, targetVector.Y * 64 + 16);

            animationSort = float.Parse("0.0" + targetVector.X.ToString() + targetVector.Y.ToString() + "3");

            animationRock = new("Maps\\springobjects", objectRectangle, animationInterval, 1, 0, animationPosition, flicker: false, flipped: false, animationSort, 0.001f, Color.Black * 0.5f, 2f, 0f, 0f, 0f);

            targetPlayer.currentLocation.temporarySprites.Add(animationRock);

            // ------------------------------ impacts

            DelayedAction.functionAfterDelay(DebrisImpact, 575);

            if (generateRock)
            {
                
                DelayedAction.functionAfterDelay(RockImpact, 600);

            }

            if (generateHoed)
            {

                DelayedAction.functionAfterDelay(DirtImpact, 600);

            }

            castFire = true;

        }

        public void DebrisImpact()
        {
            ModUtility.ImpactVector(targetLocation, targetVector);
            
            if (riteData.castTask.ContainsKey("masterRockfall") || challengeCast)
            {

                Microsoft.Xna.Framework.Rectangle areaOfEffect = new(
                    ((int)targetVector.X* 64) - 32,
                    ((int)targetVector.Y* 64) - 32,
                    128,
                    128
                );

                int castDamage = riteData.castDamage / 2;

                targetLocation.damageMonster(areaOfEffect, castDamage, riteData.castDamage, true, targetPlayer);

            }
        }

        public void RockImpact()
        {   
            int rockCut = randomIndex.Next(2);

            int generateRock = 1 + (int) (randomIndex.Next(powerLevel) / 2);

            for (int i = 0; i < generateRock; i++)
            {

                if (i == 0)
                {
                    
                    if (targetPlayer.professions.Contains(21) && rockCut == 0)
                    {

                        Game1.createObjectDebris(382, (int)targetVector.X, (int)targetVector.Y);

                    } else if (targetPlayer.professions.Contains(19) && rockCut == 0)
                    {

                        Game1.createObjectDebris(debrisIndex, (int)targetVector.X, (int)targetVector.Y);

                    }

                    Game1.createObjectDebris(debrisIndex, (int)targetVector.X, (int)targetVector.Y);

                }
                else
                {

                    Game1.createObjectDebris(390, (int)targetVector.X, (int)targetVector.Y);

                }

            }

            if (!riteData.castTask.ContainsKey("masterRockfall"))
            {

                mod.UpdateTask("lessonRockfall", generateRock);

            }


        }

        public void DirtImpact()
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

        }

    }

}
