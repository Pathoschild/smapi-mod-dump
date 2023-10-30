/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;
using static StardewValley.FarmerSprite;
using StardewDruid.Cast;
using StardewValley.TerrainFeatures;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;
using StardewValley.Locations;
using System.Threading;
using StardewModdingAPI;
using StardewValley.Monsters;
using Microsoft.Xna.Framework.Graphics;
using StardewDruid.Map;
using System.Reflection.Emit;
using System.ComponentModel.DataAnnotations;
using StardewValley.Characters;
using StardewValley.Objects;

namespace StardewDruid
{
    static class ModUtility
    {

        public static void AnimateEarthCast(Vector2 activeVector, int chargeLevel, int cycleLevel) {

            /*//-------------------------- cast variables

            int animationRow;

            Microsoft.Xna.Framework.Rectangle animationRectangle;

            float animationInterval;

            int animationLength;

            int animationLoops;

            bool animationFlip;

            float animationScale;

            Microsoft.Xna.Framework.Color animationColor;

            Vector2 animationPosition;

            TemporaryAnimatedSprite newAnimation;

            animationFlip = false;

            float colorIncrement = 1.2f - (0.2f * chargeLevel);

            animationColor = new(colorIncrement, 1, colorIncrement, 1);

            animationScale = 0.6f + (0.2f * chargeLevel);

            float vectorCastX = 12 - (6 * chargeLevel);

            float vectorCastY = 0 - 122 - (6 * chargeLevel);

            animationPosition = (activeVector * 64) + new Vector2(vectorCastX, vectorCastY);

            //-------------------------- cast animation

            animationRow = 10;

            animationRectangle = new(0, animationRow * 64, 64, 64);

            animationInterval = 75f;

            animationLength = 8;

            animationLoops = 1;

            newAnimation = new("TileSheets\\animations", animationRectangle, animationInterval, animationLength, animationLoops, animationPosition, false, animationFlip, -1, 0f, animationColor, animationScale, 0f, 0f, 0f);

            Game1.player.currentLocation.temporarySprites.Add(newAnimation);*/

            //-------------------------- sound and pitch

            if (cycleLevel <= 3)
            {

                int pitchLevel = cycleLevel % 4;

                if (chargeLevel == 1)
                {

                    Game1.player.currentLocation.playSoundPitched("discoverMineral", 600 + (pitchLevel * 200));

                    Rumble.rumbleAndFade(1f, 333);

                }

                if (chargeLevel == 3)
                {

                    Game1.player.currentLocation.playSoundPitched("discoverMineral", 700 + (pitchLevel * 200));

                    Rumble.rumbleAndFade(1f, 333);

                }

            }

        }

        public static void AnimateWaterCast(Vector2 activeVector, int chargeLevel, int cycleLevel)
        {

            //-------------------------- cast variables

            int animationRow = 5;

            Microsoft.Xna.Framework.Rectangle animationRectangle = new(128, animationRow * 64, 64, 64);

            float animationInterval = 168f;

            int animationLength = 2;

            bool animationFlip = false;

            float animationScale;

            float animationDepth = activeVector.X * 1000 + activeVector.Y;

            Microsoft.Xna.Framework.Color animationColor;

            Vector2 animationPosition;

            //-------------------------- cast shadow

            animationColor = new(0, 0, 0, 0.5f);

            animationScale = 0.2f * chargeLevel;

            animationPosition = (activeVector * 64) + (new Vector2(6, 6) * (5 - chargeLevel));

            TemporaryAnimatedSprite shadowAnimationOne = new("TileSheets\\animations", animationRectangle, animationInterval, animationLength, 1, animationPosition, false, animationFlip, animationDepth + 1, 0f, animationColor, animationScale, 0f, 0f, 0f);

            Game1.currentLocation.temporarySprites.Add(shadowAnimationOne);

            //-------------------------- cast shadow with delay

            animationScale = 0.1f + (0.2f * chargeLevel);

            animationPosition = (activeVector * 64) + (new Vector2(6, 6) * (5 - chargeLevel)) - new Vector2(3, 3);

            TemporaryAnimatedSprite shadowAnimationTwo = new("TileSheets\\animations", animationRectangle, animationInterval, animationLength, 1, animationPosition, false, animationFlip, animationDepth + 2, 0f, animationColor, animationScale, 0f, 0f, 0f)
            {

                delayBeforeAnimationStart = 334,

            };

            Game1.currentLocation.temporarySprites.Add(shadowAnimationTwo);

            //-------------------------- cast animation

            float colorIncrement = 1.2f - (0.2f * chargeLevel);

            animationColor = new(colorIncrement, colorIncrement, 1, 1); // deepens from white to blue

            animationScale = 0.2f + (0.4f * chargeLevel);

            float vectorCastX = 30 - (12 * chargeLevel);

            float vectorCastY = 0 - 160 - (32f * chargeLevel);

            animationPosition = (activeVector * 64) + new Vector2(vectorCastX, vectorCastY);

            TemporaryAnimatedSprite animationOne = new("TileSheets\\animations", animationRectangle, animationInterval, animationLength, 1, animationPosition, false, animationFlip, animationDepth + 3, 0f, animationColor, animationScale, 0f, 0f, 0f);

            Game1.currentLocation.temporarySprites.Add(animationOne);

            //-------------------------- cast animation with delay

            colorIncrement = 1.1f - (0.2f * chargeLevel);

            animationColor = new(colorIncrement, colorIncrement, 1, 1); // deepens from white to blue

            animationScale = 0.4f + (0.4f * chargeLevel);

            vectorCastX = 24 - (12 * chargeLevel);

            vectorCastY = 0 - 176 - (32f * chargeLevel);

            animationPosition = (activeVector * 64) + new Vector2(vectorCastX, vectorCastY);

            TemporaryAnimatedSprite animationTwo = new("TileSheets\\animations", animationRectangle, animationInterval, animationLength, 1, animationPosition, false, animationFlip, animationDepth + 4, 0f, animationColor, animationScale, 0f, 0f, 0f)
            {

                delayBeforeAnimationStart = 334,

            };

            Game1.currentLocation.temporarySprites.Add(animationTwo);

            //-------------------------- sound and pitch

            if (cycleLevel <= 2)
            {

                int pitchLevel = cycleLevel % 4;

                if (chargeLevel == 1)
                {

                    Game1.currentLocation.playSoundPitched("thunder_small", 600 + (pitchLevel * 200));

                }

                if (chargeLevel == 3)
                {

                    Game1.currentLocation.playSoundPitched("thunder_small", 700 + (pitchLevel * 200));

                }

            }

        }

        public static void AnimateHands(Farmer player, int direction, int timeFrame)
        {

            player.Halt();

            AnimationFrame carryAnimation;

            int frameIndex;

            bool frameFlip = false;

            switch (direction)
            {

                case 0: // Up

                    frameIndex = 12;

                    break;

                case 1: // Right

                    frameIndex = 6;

                    break;

                case 2: // Down

                    frameIndex = 0;

                    break;

                default: // Left

                    frameIndex = 6;

                    frameFlip = true; // same as right but flipped

                    break;

            }

            carryAnimation = new(frameIndex, timeFrame, true, frameFlip);

            player.FarmerSprite.animateOnce(new AnimationFrame[1] { carryAnimation });


        }

        public static void AnimateGrowth(GameLocation targetLocation, Vector2 targetVector) // DruidCastGrowth
        {

            int animationRow = 10;

            Microsoft.Xna.Framework.Rectangle animationRectangle = new(0, animationRow * 64, 64, 64);

            float animationInterval = 75f;

            int animationLength = 8;

            int animationLoops = 1;

            Microsoft.Xna.Framework.Color animationColor = new(0.8f, 1, 0.8f, 1); // light green

            Vector2 animationPosition = new(targetVector.X * 64 + 8, targetVector.Y * 64 + 8);

            float animationSort = float.Parse(targetVector.X.ToString() + targetVector.Y.ToString() + "0000");

            TemporaryAnimatedSprite newAnimation = new("TileSheets\\animations", animationRectangle, animationInterval, animationLength, animationLoops, animationPosition, false, false, animationSort, 0f, animationColor, 1f, 0f, 0f, 0f);

            targetLocation.temporarySprites.Add(newAnimation);

            return;

        }

        public static void AnimateCastRadius(GameLocation targetLocation, Vector2 targetVector, Color animationColor, int delayInterval = 0, int animationLoops = 1, float animationStrength = 0.75f)
        {

            int backRow = 11;

            Microsoft.Xna.Framework.Rectangle backRectangle = new(0, backRow * 64, 64, 64);

            float animationInterval = 75f;

            int animationLength = 8;

            float animationSort = float.Parse("0.0" + targetVector.X.ToString() + targetVector.Y.ToString());

            TemporaryAnimatedSprite backAnimation = new("TileSheets\\animations", backRectangle, animationInterval, animationLength, animationLoops, targetVector*64+new Vector2(16,16), false, false, animationSort, 0f, Color.White, 0.5f, 0f, 0f, 0f)
            {
                delayBeforeAnimationStart = 333 * delayInterval
            };

            targetLocation.temporarySprites.Add(backAnimation);

            // -----------------------

            int frontRow = 10;

            Microsoft.Xna.Framework.Rectangle frontRectangle = new(0, frontRow * 64, 64, 64);

            Vector2 animationPosition;

            if (animationStrength == 0.75f)
            {
                animationPosition = new(targetVector.X * 64 + 8, targetVector.Y * 64 + 8);
            }
            else
            {
                animationPosition = new(targetVector.X * 64 + (32 - (animationStrength * 32f)), targetVector.Y * 64 + (32 - (animationStrength * 32f)));
            }

            float frontSort = float.Parse(animationSort.ToString() + "5");

            TemporaryAnimatedSprite frontAnimation = new("TileSheets\\animations", frontRectangle, animationInterval, animationLength, animationLoops, animationPosition, false, false, frontSort, 0f, animationColor, animationStrength, 0f, 0f, 0f)
            {
                delayBeforeAnimationStart = 333 * delayInterval
            };

            targetLocation.temporarySprites.Add(frontAnimation);

            return;

        }

        public static void AnimateSplash(GameLocation targetLocation, Vector2 targetVector, bool animationFlipped) // DruidCastSplash
        {

            int animationRow = 19;

            Microsoft.Xna.Framework.Rectangle animationRectangle = new(0, animationRow * 64, 128, 128);

            float animationInterval = 150f;

            int animationLength = 4;

            int animationLoops = 1;

            Microsoft.Xna.Framework.Color animationColor = Microsoft.Xna.Framework.Color.White;

            Vector2 animationPosition = new(targetVector.X * 64, targetVector.Y * 64 - 64);

            float animationSort = float.Parse("0.0" + targetVector.X.ToString() + targetVector.Y.ToString());

            TemporaryAnimatedSprite newAnimation = new("TileSheets\\animations", animationRectangle, animationInterval, animationLength, animationLoops, animationPosition, false, animationFlipped, animationSort, 0f, animationColor, 1f, 0f, 0f, 0f);

            targetLocation.temporarySprites.Add(newAnimation);

            return;

        }

        public static void AnimateBolt(GameLocation targetLocation, Vector2 targetVector, int playSound = 800)
        {

            if (playSound >= 500)
            {

                Game1.currentLocation.playSoundPitched("flameSpellHit", 800);

            }

            Vector2 targetPosition = new(targetVector.X * 64, (targetVector.Y * 64) - 256);

            // ------------------------- cloud

            TemporaryAnimatedSprite boltCloud = new("TileSheets\\animations", new(128, 5 * 64, 64, 64), 333f, 3, 1, new Vector2(targetPosition.X + 8, targetPosition.Y), flicker: false, false, (targetPosition.Y + 32f) / 10000f + 0.001f, 0.02f, new Color(0.8f, 0.8f, 1f, 1), 0.875f, 0f, 0f, 0f);

            targetLocation.temporarySprites.Add(boltCloud);

            // ------------------------- glow

            targetLocation.temporarySprites.Add(new TemporaryAnimatedSprite(362, 75f, 6, 1, targetPosition, flicker: false, flipped: false));

            Microsoft.Xna.Framework.Rectangle sourceRect = new(644, 1078, 37, 57);

            TemporaryAnimatedSprite boltAnimation;

            // ------------------------- first bolt

            boltAnimation = new("LooseSprites\\Cursors", sourceRect, 1000f, 1, 1, targetPosition + new Vector2(0, 24f), flicker: false, Game1.random.NextDouble() < 0.5, (targetPosition.Y + 32f) / 10000f + 0.003f, 0.02f, new Color(0.8f, 0.8f, 1f, 1), 2f, 0f, 0f, 0f)
            {
                light = true,
                lightRadius = 2f,
                lightcolor = Color.Black
            };

            targetLocation.temporarySprites.Add(boltAnimation);

            // ------------------------- second bolt

            boltAnimation = new("LooseSprites\\Cursors", sourceRect, 1000f, 1, 1, targetPosition + new Vector2(0, 100f), flicker: false, Game1.random.NextDouble() > 0.5, (targetPosition.Y + 32f) / 10000f + 0.003f, 0.0175f, new Color(0.9f, 0.9f, 1f, 1), 2f, 0f, 0f, 0f)
            {
                light = true,
                lightRadius = 2f,
                lightcolor = Color.Black
            };

            targetLocation.temporarySprites.Add(boltAnimation);

            // ------------------------- third bolt

            boltAnimation = new("LooseSprites\\Cursors", sourceRect, 1000f, 1, 1, targetPosition + new Vector2(0, 176f), flicker: false, Game1.random.NextDouble() < 0.5, (targetPosition.Y + 32f) / 10000f + 0.002f, 0.015f, Color.White, 2f, 0f, 0f, 0f)
            {
                light = true,
                lightRadius = 2f,
                lightcolor = Color.Black
            };

            targetLocation.temporarySprites.Add(boltAnimation);

            return;

        }

        public static void AnimateFishJump(GameLocation targetLocation, Vector2 targetVector)
        {

            int fishIndex = SpawnData.RandomJumpFish(targetLocation);

            //if(!(Game1.viewport.X <= (targetVector.X*64)) || !(Game1.viewport.Y <= (targetVector.Y*64)))
            if (!Utility.isOnScreen(targetVector * 64, 128))
            {

                return;

            }

            targetLocation.playSound("pullItemFromWater");

            DelayedAction.functionAfterDelay(QuickSlosh, 900);

            Vector2 fishPosition;

            Vector2 sloshPosition;

            Vector2 splashPosition;

            Vector2 sloshMotion;

            Vector2 sloshAcceleration;

            bool fishFlip;

            float fishRotate;

            bool sloshFlip;

            switch (Game1.random.Next(2) == 0)
            {

                case true:

                    fishPosition = new((targetVector.X * 64) - 64, (targetVector.Y * 64) - 8);

                    sloshPosition = new((targetVector.X * 64) + 100, targetVector.Y * 64);

                    splashPosition = new((targetVector.X * 64) - 128, (targetVector.Y * 64) - 40);

                    sloshMotion = new(0.160f, -0.5f);

                    sloshAcceleration = new(0f, 0.001f);

                    fishFlip = false;

                    fishRotate = 0.03f;

                    sloshFlip = true;

                    break;

                default:

                    fishPosition = new((targetVector.X * 64) + 64, (targetVector.Y * 64) - 8);

                    sloshPosition = new((targetVector.X * 64) - 128, targetVector.Y * 64);

                    splashPosition = new((targetVector.X * 64) + 100, (targetVector.Y * 64) - 40);

                    sloshMotion = new(-0.160f, -0.5f);

                    sloshAcceleration = new(0f, 0.001f);

                    fishFlip = true;

                    fishRotate = -0.03f;

                    sloshFlip = false;

                    break;


            }


            Microsoft.Xna.Framework.Rectangle targetRectangle = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, fishIndex, 16, 16);

            float animationInterval = 1050f;

            float animationSort = float.Parse("0.0" + targetVector.X.ToString() + targetVector.Y.ToString() + "33");

            TemporaryAnimatedSprite fishAnimation = new("Maps\\springobjects", targetRectangle, animationInterval, 1, 0, fishPosition, flicker: false, flipped: fishFlip, animationSort, 0f, Color.White, 3f, 0f, 0f, fishRotate)
            {

                motion = sloshMotion,

                acceleration = sloshAcceleration,

                timeBasedMotion = true,

            };

            targetLocation.temporarySprites.Add(fishAnimation);

            // ------------------------------------

            int animationRow = 19;

            Microsoft.Xna.Framework.Rectangle animationRectangle = new(0, animationRow * 64, 128, 128);

            animationInterval = 150f;

            int animationLength = 4;

            int animationLoops = 1;

            Microsoft.Xna.Framework.Color animationColor = Microsoft.Xna.Framework.Color.White;

            animationSort = float.Parse("0.0" + targetVector.X.ToString() + targetVector.Y.ToString() + "44");

            TemporaryAnimatedSprite newAnimation = new("TileSheets\\animations", animationRectangle, animationInterval, animationLength, animationLoops, splashPosition, false, sloshFlip, animationSort, 0f, animationColor, 0.75f, 0f, 0.1f, 0f);

            targetLocation.temporarySprites.Add(newAnimation);

            // ------------------------------------

            //animationSort = float.Parse("0.0" + targetVector.X.ToString() + targetVector.Y.ToString() + "55");

            TemporaryAnimatedSprite sloshAnimation = new(28, 200f, 2, 1, sloshPosition, flicker: false, flipped: false)
            {

                delayBeforeAnimationStart = 900,

            };

            targetLocation.temporarySprites.Add(sloshAnimation);

        }

        public static void QuickSlosh()
        {
            Game1.currentLocation.localSound("quickSlosh");

        }

        public static void AnimateMeteorZone(GameLocation targetLocation, Vector2 targetVector, Color animationcolor, int meteorRange = 4, int animationLoops = 1, float animationStrength = 0.75f)
        {

            // --------------------------- splash animation

            int animationRow = 0;

            Microsoft.Xna.Framework.Rectangle animationRectangle = new(0, animationRow * 64, 64, 64);

            float animationInterval = 75f;

            int animationLength = 8;

            Vector2 animationPosition = new((targetVector.X * 64), (targetVector.Y * 64));

            float animationSort = (targetVector.X * 1000) + targetVector.Y;

            TemporaryAnimatedSprite newAnimation = new("TileSheets\\animations", animationRectangle, animationInterval, animationLength, animationLoops, animationPosition, false, false, animationSort, 0f, animationcolor, 1f, 0f, 0f, 0f);

            targetLocation.temporarySprites.Add(newAnimation);

            // --------------------------- splash radius

            AnimateCastRadius(targetLocation, targetVector - new Vector2(0, 1), animationcolor, 0, 1, animationStrength);

            AnimateCastRadius(targetLocation, targetVector - new Vector2(1, 0), animationcolor, 0, 1, animationStrength);

            AnimateCastRadius(targetLocation, targetVector + new Vector2(0, 1), animationcolor, 0, 1, animationStrength);

            AnimateCastRadius(targetLocation, targetVector + new Vector2(1, 0), animationcolor, 0, 1, animationStrength);

        }

        public static void AnimateMeteor(GameLocation targetLocation, Vector2 targetVector, bool targetDirection)
        {

            Microsoft.Xna.Framework.Rectangle meteorRectangle = new(0, 0, 32, 32);

            Vector2 meteorPosition;

            Vector2 meteorMotion;

            bool meteorRoll;

            switch (targetDirection)
            {

                case true:

                    meteorPosition = new((targetVector.X - 3) * 64, (targetVector.Y - 6) * 64);

                    meteorMotion = new Vector2(0.32f, 0.64f);

                    meteorRoll = false;

                    break;

                default:

                    meteorPosition = new((targetVector.X + 3) * 64, (targetVector.Y - 6) * 64);

                    meteorMotion = new Vector2(-0.32f, 0.64f);

                    meteorRoll = true;

                    break;

            }

            float meteorInterval = 150;

            float animationSort = float.Parse("0.0" + targetVector.X.ToString() + targetVector.Y.ToString() + "5");

            TemporaryAnimatedSprite meteorAnimation = new("TileSheets\\Fireball", meteorRectangle, meteorInterval, 4, 1, meteorPosition, flicker: false, meteorRoll, animationSort, 0f, Color.White, 2f, 0f, 0f, 0f)
            {

                motion = meteorMotion,

                timeBasedMotion = true,

            };

            targetLocation.temporarySprites.Add(meteorAnimation);

            // ----------------- puff

            Microsoft.Xna.Framework.Rectangle puffRectangle = new(96, 160, 32, 32);

            float puffInterval = 75;

            float puffSort = float.Parse("0.0" + targetVector.X.ToString() + targetVector.Y.ToString());

            TemporaryAnimatedSprite puffAnimation = new("TileSheets\\animations", puffRectangle, puffInterval, 4, 1, meteorPosition, flicker: false, flipped: false, puffSort, 0f, Color.DarkRed, 2f, 0f, 0f, 0f);

            targetLocation.temporarySprites.Add(puffAnimation);

            Game1.currentLocation.playSound("fireball");

        }

        public static void ImpactVector(GameLocation targetLocation, Vector2 targetVector)
        {

            if (Game1.random.NextDouble() < 0.5)
            {
                TemporaryAnimatedSprite smallAnimation = new(362, Game1.random.Next(30, 90), 6, 1, new Vector2(targetVector.X * 64f, targetVector.Y * 64f), flicker: false, (Game1.random.NextDouble() < 0.5) ? true : false)
                {
                    //delayBeforeAnimationStart = Game1.random.Next(700)
                };

                targetLocation.temporarySprites.Add(smallAnimation);
            }
            else
            {

                TemporaryAnimatedSprite smallAnimation = new(5, new Vector2(targetVector.X * 64f, targetVector.Y * 64f), Color.White, 8, flipped: false, 50f)
                {
                    //delayBeforeAnimationStart = Game1.random.Next(200),
                    scale = (float)Game1.random.Next(5, 15) / 10f
                };

                targetLocation.temporarySprites.Add(smallAnimation);

            }

        }

        public static StardewValley.Torch StoneBrazier(GameLocation targetLocation, Vector2 targetVector)
        {

            /*Rectangle sourceRectForObject = new();

            sourceRectForObject.X = 276;
            sourceRectForObject.Y = 1965;
            sourceRectForObject.Width = 8;
            sourceRectForObject.Height = 8;

            float animationSort = targetVector.X * 1000 + targetVector.Y + 2;

            portalAnimation = new("LooseSprites\\Cursors", sourceRectForObject, 100f, 6, 9999, new((targetVector.X * 64f)+12f,(targetVector.Y * 64f)-56f), false, false, animationSort, 0f, Color.Blue, 6f, 0f, 0f, 0f);

            targetLocation.temporarySprites.Add(portalAnimation);*/

            StardewValley.Torch stoneBrazier = new(targetVector, 144, false);

            int portalKey = (int)float.Parse(targetVector.X.ToString() + targetVector.Y.ToString());

            LightSource portalLight = new(4, new((targetVector.X * 64f) + 12f, (targetVector.Y * 64f) - 56f), 2f, new Color(0, 80, 160), portalKey, LightSource.LightContext.None, 0L);

            stoneBrazier.name = "PortalFlame";
            stoneBrazier.CanBeSetDown = false;
            stoneBrazier.Fragility = 2;
            stoneBrazier.setHealth(9999);
            stoneBrazier.isLamp.Value = true;
            stoneBrazier.IsOn = true;
            stoneBrazier.lightSource = portalLight;

            targetLocation.objects.Add(targetVector, stoneBrazier);

            return stoneBrazier;

        }

        public static bool WaterCheck(GameLocation targetLocation, Vector2 targetVector)
        {
            bool check = true;

            Layer backLayer = targetLocation.Map.GetLayer("Back");

            Tile backTile;

            for (int i = 0; i < 4; i++)
            {

                List<Vector2> neighbours = GetTilesWithinRadius(targetLocation, targetVector, i);

                foreach (Vector2 neighbour in neighbours)
                {

                    backTile = backLayer.PickTile(new Location((int)neighbour.X * 64, (int)neighbour.Y * 64), Game1.viewport.Size);

                    if (backTile != null)
                    {

                        if (!backTile.TileIndexProperties.TryGetValue("Water", out _))
                        {

                            check = false;
                        }

                    }

                }

            }

            return check;

        }

        public static Dictionary<string, List<Vector2>> NeighbourCheck(GameLocation targetLocation, Vector2 targetVector, int targetRadius = 1)
        {

            Dictionary<string, List<Vector2>> neighbourList = new();

            List<Vector2> neighbourVectors = GetTilesWithinRadius(targetLocation, targetVector, targetRadius);

            Layer backLayer = targetLocation.Map.GetLayer("Back");

            Layer buildingLayer = targetLocation.Map.GetLayer("Buildings");

            Layer pathsLayer = targetLocation.Map.GetLayer("Paths");

            if (targetLocation is BuildableGameLocation)
            {

                foreach (Vector2 neighbourVector in neighbourVectors)
                {

                    BuildableGameLocation farmLocation = targetLocation as BuildableGameLocation;

                    if (farmLocation.isTileOccupiedForPlacement(neighbourVector))
                    {

                        if (!neighbourList.ContainsKey("Building"))
                        {

                            neighbourList["Building"] = new();

                        }

                        neighbourList["Building"].Add(neighbourVector);

                    }

                    continue;


                }

            }

            foreach (Vector2 neighbourVector in neighbourVectors)
            {

                Tile buildingTile = buildingLayer.PickTile(new Location((int)neighbourVector.X * 64, (int)neighbourVector.Y * 64), Game1.viewport.Size);

                if (buildingTile != null)
                {

                    if (buildingTile.TileIndexProperties.TryGetValue("Passable", out _) == false)
                    {

                        if (!neighbourList.ContainsKey("Building"))
                        {

                            neighbourList["Building"] = new();

                        }

                        neighbourList["Building"].Add(neighbourVector);

                    }

                    continue;

                }
                
                if (pathsLayer != null)
                {

                    Tile pathsTile = buildingLayer.PickTile(new Location((int)neighbourVector.X * 64, (int)neighbourVector.Y * 64), Game1.viewport.Size);

                    if (pathsTile != null)
                    {

                        if (!neighbourList.ContainsKey("Building"))
                        {

                            neighbourList["Building"] = new();

                        }

                        neighbourList["Building"].Add(neighbourVector);

                        continue;

                    }

                }

                if (targetLocation.terrainFeatures.ContainsKey(neighbourVector))
                {
                    var terrainFeature = targetLocation.terrainFeatures[neighbourVector];

                    switch (terrainFeature.GetType().Name.ToString())
                    {

                        case "FruitTree":

                            if (!neighbourList.ContainsKey("Sapling"))
                            {

                                neighbourList["Sapling"] = new();

                            }

                            neighbourList["Sapling"].Add(neighbourVector);

                            break;

                        case "Tree":

                            StardewValley.TerrainFeatures.Tree treeCheck = terrainFeature as StardewValley.TerrainFeatures.Tree;

                            if (treeCheck.growthStage.Value >= 5)
                            {

                                if (!neighbourList.ContainsKey("Tree"))
                                {

                                    neighbourList["Tree"] = new();

                                }

                                neighbourList["Tree"].Add(neighbourVector);

                            }
                            else
                            {

                                if (!neighbourList.ContainsKey("Sapling"))
                                {

                                    neighbourList["Sapling"] = new();

                                }

                                neighbourList["Sapling"].Add(neighbourVector);

                            }

                            break;

                        case "HoeDirt":

                            StardewValley.TerrainFeatures.HoeDirt hoedCheck = terrainFeature as StardewValley.TerrainFeatures.HoeDirt;

                            if(hoedCheck.crop != null)
                            {

                                if (!neighbourList.ContainsKey("Crop"))
                                {

                                    neighbourList["Crop"] = new();

                                }

                                neighbourList["Crop"].Add(neighbourVector);

                            }

                            if (!neighbourList.ContainsKey("HoeDirt"))
                            {

                                neighbourList["HoeDirt"] = new();
                                
                            }

                            neighbourList["HoeDirt"].Add(neighbourVector);

                            break;

                        default:

                            break;

                    }

                    continue;

                }

            }

            return neighbourList;

        }

        public static bool RandomTree(GameLocation targetLocation, Vector2 targetVector) {


            Dictionary<string, List<Vector2>> nextList = ModUtility.NeighbourCheck(targetLocation, targetVector, 2);

            if (nextList.ContainsKey("Tree") || nextList.ContainsKey("Sapling"))
            {
                return false;
            }

            int treeIndex = Map.SpawnData.RandomTree(targetLocation);

            StardewValley.TerrainFeatures.Tree newTree = new(treeIndex, 1);

            //newTree.fertilized.Value = true;

            targetLocation.terrainFeatures.Add(targetVector, newTree);

            return true;

        }

        public static void UpgradeCrop(StardewValley.TerrainFeatures.HoeDirt hoeDirt, int targetX, int targetY, Farmer targetPlayer, GameLocation targetLocation, bool enableQuality)
        {

            int generateItem = 770;

            if (enableQuality)
            {

                Dictionary<int, int> objectIndexes = SpawnData.CropList(targetLocation);

                int chance = Game1.random.Next(objectIndexes.Count * 3);

                if (objectIndexes.ContainsKey(chance))
                {
                    generateItem = objectIndexes[chance];

                }

            }

            hoeDirt.destroyCrop(new Vector2(targetX, targetY), false, targetLocation);

            if(generateItem == 829)
            {
            
                StardewValley.Crop newGinger = new(true, 2, targetX, targetY);

                hoeDirt.crop = newGinger;

                targetLocation.playSound("dirtyHit");
                
                Game1.stats.SeedsSown++;

                return;

            }

            hoeDirt.plant(generateItem, targetX, targetY, targetPlayer,false, targetLocation);

            //hoeDirt.crop.updateDrawMath(new Vector2(targetX, targetY));

        }

        public static bool GreetVillager(GameLocation location, Farmer player, NPC villager, bool friendShip = false)
        {

            villager.faceTowardFarmerForPeriod(3000, 4, false, player);

            bool friendCheck = player.hasPlayerTalkedToNPC(villager.Name);

            int emoteIndex = 8;

            if (player.friendshipData.ContainsKey(villager.Name))
            {
                if (player.friendshipData[villager.Name].Points >= 500)
                {

                    emoteIndex = 32;

                }

                if (player.friendshipData[villager.Name].Points >= 1000)
                {

                    emoteIndex = 20;

                }

            }
            else
            {
                
                villager.doEmote(emoteIndex);

                return false;

            }

            villager.doEmote(emoteIndex);

            if (!friendCheck)
            {

                player.friendshipData[villager.Name].TalkedToToday = true;

                if (friendShip)
                {

                    player.changeFriendship(25, villager);

                }

                return true;

            }

            return false;

        }

        public static void PetAnimal(Farmer targetPlayer, FarmAnimal targetAnimal)
        {

            if (targetAnimal.wasPet.Value)
            {

                return;

            }

            targetAnimal.wasPet.Value = true;

            int num = 7;

            if (targetAnimal.wasAutoPet.Value)
            {

                targetAnimal.friendshipTowardFarmer.Value = Math.Min(1000, (int)targetAnimal.friendshipTowardFarmer.Value + num);

            }
            else
            {

                targetAnimal.friendshipTowardFarmer.Value = Math.Min(1000, (int)targetAnimal.friendshipTowardFarmer.Value + 15);

            }

            if ((targetPlayer.professions.Contains(3) && !targetAnimal.isCoopDweller()) || (targetPlayer.professions.Contains(2) && targetAnimal.isCoopDweller()))
            {

                targetAnimal.friendshipTowardFarmer.Value = Math.Min(1000, (int)targetAnimal.friendshipTowardFarmer.Value + 15);

                targetAnimal.happiness.Value = (byte)Math.Min(255, (byte)targetAnimal.happiness.Value + Math.Max(5, 40 - (byte)targetAnimal.happinessDrain.Value));

            }

            int num2 = 20;

            if (targetAnimal.wasAutoPet.Value)
            {

                num2 = 32;

            }

            targetAnimal.doEmote(((int)targetAnimal.moodMessage.Value == 4) ? 12 : num2);

            targetAnimal.happiness.Value = (byte)Math.Min(255, (byte)targetAnimal.happiness.Value + Math.Max(5, 40 - (byte)targetAnimal.happinessDrain.Value));

            targetAnimal.makeSound();

            targetPlayer.gainExperience(0, 5);

        }

        public static void LearnRecipe(Farmer targetPlayer)
        {

            List<string> recipeList = SpawnData.RecipeList();

            int learnedRecipes = 0;

            foreach (string recipe in recipeList)
            {

                if (!targetPlayer.cookingRecipes.ContainsKey(recipe))
                {

                    targetPlayer.cookingRecipes.Add(recipe, 0);

                    learnedRecipes++;

                }

            }

            if (learnedRecipes >= 1)
            {

                Game1.addHUDMessage(new HUDMessage($"Learned {learnedRecipes} recipes", 2));

            }

        }

        static List<Vector2> TilesWithinOne(Vector2 center)
        {
            List<Vector2> result = new() {

                center + new Vector2(0, -1),    // N
                center + new Vector2(1, -1),    // NE
                center + new Vector2(1, 0),     // E
                center + new Vector2(1, 1),     // SE
                center + new Vector2(0, 1),     // S
                center + new Vector2(-1, 1),    // SW
                center + new Vector2(-1, 0),    // W
                center + new Vector2(-1, -1),   // NW

            };

            return result;

        }

        static List<Vector2> TilesWithinTwo(Vector2 center)
        {
            List<Vector2> result = new()
            {
               
                center + new Vector2(0,-2), // N
                center + new Vector2(1,-2), // NE

                center + new Vector2(2,-1), // NE
                center + new Vector2(2,0), // E
                center + new Vector2(2,1), // SE

                center + new Vector2(1,2), // SE
                center + new Vector2(0,2), // S
                center + new Vector2(-1,2), // SW

                center + new Vector2(-2,1), // SW
                center + new Vector2(-2,0), // W
                center + new Vector2(-2,-1), // NW

                 center + new Vector2(-1,-2), // NW

            };

            return result;

        }

        static List<Vector2> TilesWithinThree(Vector2 center)
        {
            List<Vector2> result = new() {

                center + new Vector2(0,-3), // N
                center + new Vector2(1,-3),

                center + new Vector2(2,-2), // NE

                center + new Vector2(3,-1), // E
                center + new Vector2(3,0),
                center + new Vector2(3,1),

                center + new Vector2(2,2), // SE

                center + new Vector2(1,3), // S
                center + new Vector2(0,3),
                center + new Vector2(-1,3),

                center + new Vector2(-2,2), // SW

                center + new Vector2(-3,1), // W
                center + new Vector2(-3,0),
                center + new Vector2(-3,-1),

                center + new Vector2(-2,-2), // NW

                center + new Vector2(-1,-3), // NNW
 
            };

            return result;

        }

        static List<Vector2> TilesWithinFour(Vector2 center)
        {
            List<Vector2> result = new() {

                
                center + new Vector2(0,-4), // N
                center + new Vector2(1,-4),

                center + new Vector2(2,-3),
                center + new Vector2(3,-3), // NE
                center + new Vector2(3,-2),

                center + new Vector2(4,-1), // E
                center + new Vector2(4,0),
                center + new Vector2(4,1),

                center + new Vector2(3,2),
                center + new Vector2(3,3), // SE
                center + new Vector2(2,3),

                center + new Vector2(1,4), // S
                center + new Vector2(0,4),
                center + new Vector2(-1,4),

                center + new Vector2(-2,3),
                center + new Vector2(-3,3), // SW
                center + new Vector2(-3,2),

                center + new Vector2(-4,1), // W
                center + new Vector2(-4,0),
                center + new Vector2(-4,-1),

                center + new Vector2(-3,-2),
                center + new Vector2(-3,-3), // NW
                center + new Vector2(-2,-3),

                center + new Vector2(-1,-4), // NNW

            };

            return result;

        }

        static List<Vector2> TilesWithinFive(Vector2 center)
        {
            List<Vector2> result = new() {

                center + new Vector2(0,-5), // N
                center + new Vector2(1,-5),

                center + new Vector2(2,-4), // NE
                center + new Vector2(3,-4),
                center + new Vector2(4,-3),
                center + new Vector2(4,-2),

                center + new Vector2(5,-1), // E
                center + new Vector2(5,0),
                center + new Vector2(5,1),

                center + new Vector2(4,2), // SE
                center + new Vector2(4,3),
                center + new Vector2(3,4),
                center + new Vector2(2,4),
               
                center + new Vector2(1,5), // S
                center + new Vector2(0,5),
                center + new Vector2(-1,5),

                center + new Vector2(-2,4), // SW
                center + new Vector2(-3,4),
                center + new Vector2(-4,3),
                center + new Vector2(-4,2),

                center + new Vector2(-5,1), // W
                center + new Vector2(-5,0),
                center + new Vector2(-5,-1),

                center + new Vector2(-4,-2), // NW
                center + new Vector2(-4,-3),
                center + new Vector2(-3,-4),
                center + new Vector2(-2,-4),

                center + new Vector2(-1,-5), // NNW

            };

            return result;

        }

        static List<Vector2> TilesWithinSix(Vector2 center)
        {
            List<Vector2> result = new() {

                center + new Vector2(0,-6), // N
                center + new Vector2(1,-6),

                center + new Vector2(2,-5),
                center + new Vector2(3,-5),
                center + new Vector2(4,-4), // NE
                center + new Vector2(5,-3),
                center + new Vector2(5,-2),

                center + new Vector2(6,-1),
                center + new Vector2(6,0), // E
                center + new Vector2(6,1),

                center + new Vector2(5,2),
                center + new Vector2(5,3),
                center + new Vector2(4,4), // SE
                center + new Vector2(3,5),
                center + new Vector2(2,5),

                center + new Vector2(1,6),
                center + new Vector2(0,6), // S
                center + new Vector2(-1,6),

                center + new Vector2(-2,5),
                center + new Vector2(-3,5),
                center + new Vector2(-4,4), // SW
                center + new Vector2(-5,3),
                center + new Vector2(-5,2),

                center + new Vector2(-6,-1),
                center + new Vector2(-6,0), // W
                center + new Vector2(-6,1),

                center + new Vector2(-5,-2),
                center + new Vector2(-5,-3),
                center + new Vector2(-4,-4), // NW
                center + new Vector2(-3,-5),
                center + new Vector2(-2,-5),

                center + new Vector2(-1,-6), // NNW

            };

            return result;

        }

        static List<Vector2> TilesWithinSeven(Vector2 center)
        {
            List<Vector2> result = new() {

                center + new Vector2(0,-7), // N
                center + new Vector2(1,-7),

                center + new Vector2(2,-6),
                //center + new Vector2(3,-6),

                center + new Vector2(4,-5), // NE
                center + new Vector2(5,-4), // NE

                //center + new Vector2(6,-3),
                center + new Vector2(6,-2),

                center + new Vector2(7,-1),
                center + new Vector2(7,0), // E
                center + new Vector2(7,1),

                center + new Vector2(6,2),
                //center + new Vector2(6,3),

                center + new Vector2(5,4), // SE
                center + new Vector2(4,5), // SE

                //center + new Vector2(3,6),
                center + new Vector2(2,6),

                center + new Vector2(1,7),
                center + new Vector2(0,7), // S
                center + new Vector2(-1,7),

                center + new Vector2(-2,6),
                //center + new Vector2(-3,6),

                center + new Vector2(-4,5), // SW
                center + new Vector2(-5,4), // SW

                //center + new Vector2(-6,3),
                center + new Vector2(-6,2),

                center + new Vector2(-7,-1),
                center + new Vector2(-7,0), // W
                center + new Vector2(-7,1),

                center + new Vector2(-6,-2),
                //center + new Vector2(-6,-3),

                center + new Vector2(-5,-4), // NW
                center + new Vector2(-4,-5), // NW

                //center + new Vector2(-3,-6),
                center + new Vector2(-2,-6),

                center + new Vector2(-1,-7), // NNW

            };

            return result;

        }

        static List<Vector2> TilesWithinEight(Vector2 center)
        {
            List<Vector2> result = new() {

                center + new Vector2(2,-7),
                center + new Vector2(3,-6),

                center + new Vector2(4,-6),
                center + new Vector2(5,-5), // NE
                center + new Vector2(6,-4),

                center + new Vector2(6,-3),
                center + new Vector2(7,-2),

                center + new Vector2(7,2),
                center + new Vector2(6,3),

                center + new Vector2(6,4),
                center + new Vector2(5,5), // SE
                center + new Vector2(4,6),

                center + new Vector2(3,6),
                center + new Vector2(2,7),

                center + new Vector2(-2,7),
                center + new Vector2(-3,6),

                center + new Vector2(-4,6),
                center + new Vector2(-5,5), // SW
                center + new Vector2(-6,4),

                center + new Vector2(-6,3),
                center + new Vector2(-7,2),

                center + new Vector2(-7,-2),
                center + new Vector2(-6,-3),

                center + new Vector2(-6,-4),
                center + new Vector2(-5,-5), // NW
                center + new Vector2(-4,-6),

                center + new Vector2(-3,-6),
                center + new Vector2(-2,-7),


            };

            return result;

        }

        public static List<Vector2> GetTilesWithinRadius(GameLocation playerLocation, Vector2 center, int level)
        {

            List<Vector2> templateList;

            switch (level)
            {   
                case 1:
                    templateList = TilesWithinOne(center);
                    break;
                case 2:
                    templateList = TilesWithinTwo(center);
                    break;
                case 3:
                    templateList = TilesWithinThree(center);
                    break;
                case 4:
                    templateList = TilesWithinFour(center);
                    break;
                case 5:
                    templateList = TilesWithinFive(center);
                    break;
                case 6:
                    templateList = TilesWithinSix(center);
                    break;
                case 7:
                    templateList = TilesWithinSeven(center);
                    break;
                case 8:
                    templateList = TilesWithinEight(center);
                    break;
                default: // 0
                    templateList = new() { center, };
                    break;

            }

            float mapWidth = (playerLocation.Map.DisplayWidth / 16);

            float mapHeight = (playerLocation.Map.DisplayHeight / 16);

            List<Vector2> vectorList = new();

            foreach (Vector2 testVector in templateList)
            {

                if (testVector.X < 0 || testVector.X > mapWidth || testVector.Y < 0 || testVector.Y > mapHeight)
                {

                }
                else
                {

                    vectorList.Add(testVector);

                }

            }

            return vectorList;

        }

        public static List<int> CastWaterAdjust(Vector2 vector, int direction)
        {

            Point mousePoint = Game1.getMousePosition();

            if (mousePoint.Equals(new(0)))
            {
                return CastWaterNormal(vector, direction);
            
            }

            Vector2 playerPosition = Game1.player.Position;

            Vector2 viewPortPosition = Game1.viewportPositionLerp;

            Vector2 mousePosition = new(mousePoint.X + viewPortPosition.X, mousePoint.Y + viewPortPosition.Y);

            float vectorDistance = Vector2.Distance(playerPosition, mousePosition);

            if (vectorDistance <= 96)
            {

                return CastWaterNormal(vector, direction);

            }

            Vector2 macroVector = mousePosition - playerPosition;

            if(vectorDistance > 448)
            {

                float adjustmentRatio = 448 / vectorDistance;

                macroVector *= adjustmentRatio;

            }

            int microX = Convert.ToInt32(macroVector.X / 64);

            int microY = Convert.ToInt32(macroVector.Y / 64);

            int newDirection;

            if(microY <= microX)
            {
                if(microX > 0) // right
                {
                    newDirection = 1;
                }
                else // left
                {
                    newDirection = 3;

                }
            }
            else
            {
                if (microY > 0) // down
                {
                    newDirection = 2;
                }
                else // up
                {
                    newDirection = 0;

                }
            }

            List<int> targetList = new()
            {
                newDirection,
                microX + (int)vector.X,
                microY + (int)vector.Y
            };

            return targetList;

        }

        public static List<int> CastWaterNormal(Vector2 vector, int direction)
        {

            Dictionary<int, Vector2> vectorIndex = new()
            {

                [0] = vector + new Vector2(0, -5),// up
                [1] = vector + new Vector2(5, 0), // right
                [2] = vector + new Vector2(0, 5),// down
                [3] = vector + new Vector2(-5, 0), // left

            };

            Vector2 targetVector = vectorIndex[direction];

            List<int> targetList = new()
            {
                direction,
                (int)targetVector.X,
                (int)targetVector.Y
            };

            return targetList; 

        }

        public static void Explode(GameLocation targetLocation, Vector2 targetVector, Farmer targetPlayer, int damageRadius, int castDamage, int powerLevel, Tool Pick, Tool Axe)
        {

            targetLocation.playSound("flameSpellHit");

            targetPlayer.Stamina += Math.Min(2, targetPlayer.MaxStamina - targetPlayer.Stamina);

            Pick.DoFunction(targetLocation, 0, 0, 1, targetPlayer);

            targetPlayer.Stamina += Math.Min(2, targetPlayer.MaxStamina - targetPlayer.Stamina);

            Axe.DoFunction(targetLocation, 0, 0, 1, targetPlayer);


            // ----------------- damage monsters


            TemporaryAnimatedSprite bigAnimation = new(23, 9999f, 6, 1, new Vector2(targetVector.X * 64f, targetVector.Y * 64f), flicker: false, (Game1.random.NextDouble() < 0.5) ? true : false)
            {
                light = true,
                lightRadius = 3,
                lightcolor = Color.Black,
                alphaFade = 0.03f - 3f * 0.003f,
                Parent = targetLocation
            };

            targetLocation.temporarySprites.Add(bigAnimation);

            int damageDiameter = (damageRadius * 2) + 1;

            Microsoft.Xna.Framework.Rectangle areaOfEffect = new Microsoft.Xna.Framework.Rectangle((int)(targetVector.X - damageRadius) * 64, (int)(targetVector.Y - damageRadius) * 64, damageDiameter * 64, damageDiameter * 64);

            targetLocation.damageMonster(areaOfEffect, castDamage, castDamage * 2, true, targetPlayer);

            // ----------------- object destruction

            List<Vector2> tileVectors;

            int impactRadius = damageRadius + 1;

            for (int i = 0; i < impactRadius; i++)
            {

                if (i == 0)
                {

                    tileVectors = new List<Vector2>
                    {

                        targetVector

                    };

                }
                else
                {

                    tileVectors = GetTilesWithinRadius(targetLocation, targetVector, i);

                }

                bool destroyVector;

                foreach (Vector2 tileVector in tileVectors)
                {

                    destroyVector = false;

                    if (targetLocation.objects.ContainsKey(tileVector))
                    {

                        StardewValley.Object targetObject = targetLocation.objects[tileVector];

                        if (targetObject.Name.Contains("Stone"))
                        {

                            targetLocation.OnStoneDestroyed(@targetObject.ParentSheetIndex, (int)tileVector.X, (int)tileVector.Y, targetPlayer);

                            targetLocation.objects.Remove(tileVector);

                            destroyVector = true;

                        }
                        else if (targetObject.Name.Contains("Twig"))
                        {

                            for (int fibreDebris = 2; fibreDebris < i; fibreDebris++)
                            {
                                Game1.createObjectDebris(388, (int)tileVector.X, (int)tileVector.Y);

                            }

                            targetLocation.objects.Remove(tileVector);

                            destroyVector = true;

                        }
                        else if (targetObject.Name.Contains("Weed"))
                        {

                            Game1.createObjectDebris(771, (int)tileVector.X, (int)tileVector.Y);

                            targetLocation.objects.Remove(tileVector);

                            destroyVector = true;

                        }
                        else if (targetObject is BreakableContainer)
                        {

                            targetObject.setHealth(1);

                            targetObject.performToolAction(Pick, targetLocation);

                            targetLocation.objects.Remove(tileVector);

                            destroyVector = true;

                        }
                        else if (targetObject is Fence || targetObject is StardewValley.Objects.Workbench || targetObject is StardewValley.Objects.Furniture || targetObject is StardewValley.Objects.Chest)
                        {

                            // do nothing

                        }
                        else
                        {
                            // ----------------- dislodge craftable

                            for (int j = 0; j < 2; j++)
                            {

                                Tool toolUse = (j == 0) ? Pick : Axe;

                                if (targetLocation.objects.ContainsKey(tileVector) && targetObject.performToolAction(toolUse, targetLocation))
                                {
                                    targetObject.performRemoveAction(tileVector, targetLocation);

                                    targetObject.dropItem(targetLocation, tileVector * 64, tileVector * 64 + new Vector2(0, 32));

                                    targetLocation.objects.Remove(tileVector);

                                }

                            }

                        }

                    }

                    if (targetLocation.terrainFeatures.ContainsKey(tileVector))
                    {

                        if(powerLevel > 1)
                        {

                            if (targetLocation.terrainFeatures[tileVector] is StardewValley.TerrainFeatures.Tree)
                            {

                                StardewValley.TerrainFeatures.Tree targetTree = targetLocation.terrainFeatures[tileVector] as StardewValley.TerrainFeatures.Tree;

                                if (targetTree.growthStage.Value >= 5)
                                {

                                    targetTree.performToolAction(null, (int)targetTree.health.Value, tileVector, targetLocation);

                                }
                                else
                                {

                                    targetTree.performToolAction(Axe, 0, tileVector, targetLocation);

                                    targetLocation.terrainFeatures.Remove(tileVector);

                                }

                                targetTree = null;

                                destroyVector = true;

                            }

                        }

                        if (targetLocation.terrainFeatures.ContainsKey(tileVector))
                        {

                            if (targetLocation.terrainFeatures[tileVector] is StardewValley.TerrainFeatures.Grass)
                            {

                                targetLocation.terrainFeatures.Remove(tileVector);


                                if (Game1.random.NextDouble() < 0.5)
                                {

                                    Game1.createObjectDebris(771, (int)tileVector.X, (int)tileVector.Y);

                                }

                                destroyVector = true;

                            }

                        }

                    }

                    if (i == damageRadius || destroyVector)
                    {

                        ImpactVector(targetLocation, tileVector);

                    }

                }

            }

        }

    }

}
