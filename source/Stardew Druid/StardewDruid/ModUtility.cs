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
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewDruid.Cast;
using StardewDruid.Map;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml.Linq;
using xTile.Dimensions;
using xTile.Layers;
using xTile.ObjectModel;
using xTile.Tiles;
using static StardewValley.FarmerSprite;

namespace StardewDruid
{
    static class ModUtility
    {

        public static void WealdSound(int castLevel)
        {

            int chargeLevel = castLevel % 4;

            int pitchLevel = castLevel / 4;

            //-------------------------- sound and pitch

            if (pitchLevel <= 2)
            {

                if (chargeLevel == 1)
                {

                    Game1.player.currentLocation.playSoundPitched("discoverMineral", 600 + (pitchLevel * 200));

                }

                if (chargeLevel == 3)
                {

                    Game1.player.currentLocation.playSoundPitched("discoverMineral", 700 + (pitchLevel * 200));

                }

            }

        }

        public static void MistsSound(int castLevel)
        {
            //int chargeLevel = castLevel % 4;

            //int pitchLevel = castLevel / 4;

            //-------------------------- sound and pitch

            //if (pitchLevel <= 2)
            //{

            //if (chargeLevel == 1)
                if (castLevel == 1)
                {

                    Game1.player.currentLocation.playSoundPitched("thunder_small", 600 + (new Random().Next(5) * 200));

                }

                //if (chargeLevel == 3)
                //{

                //    Game1.player.currentLocation.playSoundPitched("thunder_small", 700 + (pitchLevel * 200));

                //}

            //}

        }

        public static void AnimateRadiusDecoration(GameLocation location, Vector2 vector, string name, float size, float change, float interval = 600f, float depth = 0.0001f)
        {

            TemporaryAnimatedSprite radiusAnimation = new(0, interval, 1, 1, (vector * 64) + new Vector2(32, 32) - (new Vector2(32, 32) * size), false, false)
            {

                sourceRect = new(0, 0, 64, 64),

                sourceRectStartingPos = new Vector2(0, 0),

                texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", name + ".png")),

                scale = 1f, //* size,

                scaleChange = 0.004f * change,

                motion = new Vector2(-0.128f, -0.128f) * change,

                timeBasedMotion = true,

                layerDepth = depth,

                rotationChange = 0.06f,

                alphaFade = 0.0002f,

            };

            location.temporarySprites.Add(radiusAnimation);

        }

        public static void AnimateSprout(GameLocation location, Vector2 vector)
        {

            TemporaryAnimatedSprite radiusAnimation = new(0, 1000f, 1, 1, (vector * 64) + new Vector2(16, 16), false, false)
            {

                sourceRect = new(0, 0, 32, 32),

                sourceRectStartingPos = new Vector2(0, 0),

                texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Sprout.png")),

                scale = 1f, //* size,

                scaleChange = 0.004f,

                motion = new Vector2(-0.064f, -0.064f),

                timeBasedMotion = true,

                layerDepth = 0.0001f,

                alphaFade = 0.001f,

                //rotationChange = 0.08f,

            };

            location.temporarySprites.Add(radiusAnimation);

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

        public static void AnimateGrowth(GameLocation targetLocation, Vector2 targetVector, Microsoft.Xna.Framework.Color animationColor) // DruidCastGrowth (0.8f, 1, 0.8f, 1)
        {

            int animationRow = 10;

            Microsoft.Xna.Framework.Rectangle animationRectangle = new(0, animationRow * 64, 64, 64);

            float animationInterval = 75f;

            int animationLength = 8;

            int animationLoops = 1;

            Vector2 animationPosition = new(targetVector.X * 64 + 8, targetVector.Y * 64 + 8);

            float animationSort = float.Parse(targetVector.X.ToString() + targetVector.Y.ToString() + "0000");

            TemporaryAnimatedSprite newAnimation = new("TileSheets\\animations", animationRectangle, animationInterval, animationLength, animationLoops, animationPosition, false, false, animationSort, 0f, animationColor, 1.2f, 0f, 0f, 0f);

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

            TemporaryAnimatedSprite backAnimation = new("TileSheets\\animations", backRectangle, animationInterval, animationLength, animationLoops, targetVector * 64 + new Vector2(16, 16), false, false, animationSort, 0.001f, Color.White, 0.5f, 0f, 0f, 0f)
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

            TemporaryAnimatedSprite frontAnimation = new("TileSheets\\animations", frontRectangle, animationInterval, animationLength, animationLoops, animationPosition, false, false, frontSort, 0.001f, animationColor, animationStrength, 0f, 0f, 0f)
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

            TemporaryAnimatedSprite newAnimation = new("TileSheets\\animations", animationRectangle, animationInterval, animationLength, animationLoops, animationPosition, false, animationFlipped, animationSort, 0.001f, animationColor, 0.75f, 0f, 0f, 0f);

            targetLocation.temporarySprites.Add(newAnimation);

            return;

        }

        public static void AnimateBolt(GameLocation targetLocation, Vector2 targetVector, int playSound = 800, int colour = -1)
        {

            List<Color> colourProfile;

            switch (colour)
            {
                case 768:

                    colourProfile = new()
                    {
                        new Color(0.1f, 0.1f, 0f, 1),
                        new Color(0.1f, 0.1f, 0.5f, 1),
                        Color.White,
                    };
                    break;

                case 769:

                    colourProfile = new()
                    {
                        new Color(0f, 0f, 0.5f, 1),
                        new Color(0.5f, 0.5f, 1f, 1),
                        Color.White,
                    };
                    break;

                default:

                    colourProfile = new()
                    {
                        new Color(0.8f, 0.8f, 1f, 1),
                        new Color(0.9f, 0.9f, 1f, 1),
                        Color.White,
                    };
                    break;

            }

            if (playSound >= 500)
            {

                Game1.currentLocation.playSoundPitched("flameSpellHit", 800);

            }

            Vector2 targetPosition = new(targetVector.X * 64, (targetVector.Y * 64) - 256);

            // ------------------------- cloud

            TemporaryAnimatedSprite boltCloud = new("TileSheets\\animations", new(128, 5 * 64, 64, 64), 333f, 3, 1, new Vector2(targetPosition.X + 8, targetPosition.Y), flicker: false, false, (targetPosition.Y + 32f) / 10000f + 0.001f, 0.02f, colourProfile[0], 0.875f, 0f, 0f, 0f);

            targetLocation.temporarySprites.Add(boltCloud);

            // ------------------------- glow

            targetLocation.temporarySprites.Add(new TemporaryAnimatedSprite(362, 75f, 6, 1, targetPosition, flicker: false, flipped: false));

            Microsoft.Xna.Framework.Rectangle sourceRect = new(644, 1078, 37, 57);

            TemporaryAnimatedSprite boltAnimation;

            // ------------------------- first bolt

            boltAnimation = new("LooseSprites\\Cursors", sourceRect, 1000f, 1, 1, targetPosition + new Vector2(0, 24f), flicker: false, Game1.random.NextDouble() < 0.5, (targetPosition.Y + 32f) / 10000f + 0.003f, 0.02f, colourProfile[0], 2f, 0f, 0f, 0f)
            {
                light = true,
                lightRadius = 2f,
                lightcolor = Color.Black
            };

            targetLocation.temporarySprites.Add(boltAnimation);

            // ------------------------- second bolt

            boltAnimation = new("LooseSprites\\Cursors", sourceRect, 1000f, 1, 1, targetPosition + new Vector2(0, 100f), flicker: false, Game1.random.NextDouble() > 0.5, (targetPosition.Y + 32f) / 10000f + 0.003f, 0.0175f, colourProfile[1], 2f, 0f, 0f, 0f)
            {
                light = true,
                lightRadius = 2f,
                lightcolor = Color.Black
            };

            targetLocation.temporarySprites.Add(boltAnimation);

            // ------------------------- third bolt

            boltAnimation = new("LooseSprites\\Cursors", sourceRect, 1000f, 1, 1, targetPosition + new Vector2(0, 176f), flicker: false, Game1.random.NextDouble() < 0.5, (targetPosition.Y + 32f) / 10000f + 0.002f, 0.015f, colourProfile[2], 2f, 0f, 0f, 0f)
            {
                light = true,
                lightRadius = 2f,
                lightcolor = Color.Black
            };

            targetLocation.temporarySprites.Add(boltAnimation);

            return;

        }

        public static void AnimateFishJump(GameLocation targetLocation, Vector2 targetPosition, int fishIndex, bool fishDirection)
        {

            //if(!(Game1.viewport.X <= (targetVector.X*64)) || !(Game1.viewport.Y <= (targetVector.Y*64)))
            if (!Utility.isOnScreen(targetPosition, 128))
            {

                return;

            }

            Vector2 targetVector = targetPosition;

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

            switch (fishDirection)
            {

                case true:

                    fishPosition = new(targetVector.X - 64, targetVector.Y - 8);

                    sloshPosition = new(targetVector.X + 100, targetVector.Y);

                    splashPosition = new(targetVector.X - 128, targetVector.Y - 40);

                    sloshMotion = new(0.160f, -0.5f);

                    sloshAcceleration = new(0f, 0.001f);

                    fishFlip = false;

                    fishRotate = 0.03f;

                    sloshFlip = true;

                    break;

                default:

                    fishPosition = new(targetVector.X + 64, targetVector.Y - 8);

                    sloshPosition = new(targetVector.X - 128, targetVector.Y);

                    splashPosition = new(targetVector.X + 100, targetVector.Y - 40);

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

        public static void AnimateBombZone(GameLocation targetLocation, Vector2 targetVector, Color animationcolor, int animationLoops = 1)
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

            TemporaryAnimatedSprite secondAnimation = new("TileSheets\\animations", animationRectangle, animationInterval, animationLength, animationLoops, animationPosition - new Vector2(24, 24), false, false, animationSort, 0f, animationcolor * 0.65f, 1.75f, 0f, 0f, 0f);

            targetLocation.temporarySprites.Add(secondAnimation);

            // --------------------------- splash radius

            TemporaryAnimatedSprite thirdAnimation = new("TileSheets\\animations", animationRectangle, animationInterval, animationLength, animationLoops, animationPosition - new Vector2(48, 48), false, false, animationSort, 0f, animationcolor * 0.45f, 2.5f, 0f, 0f, 0f);

            targetLocation.temporarySprites.Add(thirdAnimation);

        }

        public static void AnimateMeteor(GameLocation targetLocation, Vector2 targetVector, bool targetDirection)
        {

            Microsoft.Xna.Framework.Rectangle meteorRectangle = new(0, 0, 32, 32);

            Vector2 meteorPosition;

            Vector2 meteorMotion;

            //bool meteorRoll;

            switch (targetDirection)
            {

                case true:

                    meteorPosition = new((targetVector.X - 4) * 64, (targetVector.Y - 8) * 64);

                    meteorMotion = new Vector2(0.32f, 0.64f);

                    // meteorRoll = false;

                    break;

                default:

                    meteorPosition = new((targetVector.X + 4) * 64, (targetVector.Y - 8) * 64);

                    meteorMotion = new Vector2(-0.32f, 0.64f);

                    //meteorRoll = true;

                    break;

            }

            float animationSort = float.Parse("0.0" + targetVector.X.ToString() + targetVector.Y.ToString() + "5");

            TemporaryAnimatedSprite warpAnimation = new(0, 750f, 1, 1, meteorPosition, false, false)
            {

                sourceRect = new(0, 0, 32, 32),

                sourceRectStartingPos = new Vector2(0, 0),

                texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Fireball.png")),

                scale = 2f,

                motion = meteorMotion,

                timeBasedMotion = true,

                rotationChange = -0.08f,

                layerDepth = animationSort,

            };

            targetLocation.temporarySprites.Add(warpAnimation);

            /*TemporaryAnimatedSprite meteorAnimation = new("TileSheets\\Fireball", meteorRectangle, meteorInterval, 12, 1, meteorPosition, flicker: false, meteorRoll, animationSort, 0f, Color.White, 1.75f, 0f, 0f, 0f)
            {

                motion = meteorMotion,

                timeBasedMotion = true,

                rotationChange = targetDirection ? 0.25f : -0.25f,

            };

            targetLocation.temporarySprites.Add(meteorAnimation);

            // ----------------- puff

            Microsoft.Xna.Framework.Rectangle puffRectangle = new(96, 160, 32, 32);

            float puffInterval = 75;

            float puffSort = float.Parse("0.0" + targetVector.X.ToString() + targetVector.Y.ToString());

            TemporaryAnimatedSprite puffAnimation = new("TileSheets\\animations", puffRectangle, puffInterval, 4, 1, meteorPosition, flicker: false, flipped: false, puffSort, 0f, Color.DarkRed, 2f, 0f, 0f, 0f);

            targetLocation.temporarySprites.Add(puffAnimation);*/

            /*TemporaryAnimatedSprite smallAnimation = new(362, 50, 6, 1, new Vector2(targetVector.X * 64f - 16f, targetVector.Y * 64f - 16f), flicker: false, (Game1.random.NextDouble() < 0.5) ? true : false)
            {
                delayBeforeAnimationStart = 750,
                scale = 1.5f,
            };

            targetLocation.temporarySprites.Add(smallAnimation);*/

        }

        public static void ImpactVector(GameLocation targetLocation, Vector2 targetVector)
        {

            if (Game1.random.NextDouble() < 0.5)
            {
                TemporaryAnimatedSprite smallAnimation = new(362, Game1.random.Next(30, 90), 6, 1, new Vector2(targetVector.X * 64f, targetVector.Y * 64f), flicker: false, (Game1.random.NextDouble() < 0.5) ? true : false)
                {
                    //delayBeforeAnimationStart = Game1.random.Next(700)
                    scale = Game1.random.Next(5, 10) * 0.1f,
                    alpha = 0.65f,
                };

                targetLocation.temporarySprites.Add(smallAnimation);
            }
            else
            {

                TemporaryAnimatedSprite smallAnimation = new(5, new Vector2(targetVector.X * 64f, targetVector.Y * 64f), Color.White, 8, flipped: false, 50f)
                {
                    //delayBeforeAnimationStart = Game1.random.Next(200),
                    //scale = (float)Game1.random.Next(5, 15) / 10f
                    scale = Game1.random.Next(5, 10) * 0.1f,
                    alpha = 0.65f,
                };

                targetLocation.temporarySprites.Add(smallAnimation);

            }

        }

        public static void BurnVector(GameLocation location, Vector2 position, Color color, int interval = 175, int length = 5, int loops = 1)
        {

            TemporaryAnimatedSprite fireAnimation = new(0,interval,length,loops,position, false, false)
            {

                sourceRect = new(0, 0, 32, 32),

                sourceRectStartingPos = new Vector2(0,0),

                texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images","Fire.png")),

                scale = 2f, //* size,

                layerDepth = position.Y / 64000,

                alphaFade = 0.001f,

                color = color,

            };

            location.temporarySprites.Add(fireAnimation);

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

        public static string GroundCheck(GameLocation targetLocation, Vector2 neighbour, bool npc = false)
        {

            Layer backLayer = targetLocation.Map.GetLayer("Back");

            Tile backTile;

            int mapWidth = targetLocation.Map.DisplayWidth / 64;

            int mapHeight = targetLocation.Map.DisplayHeight / 64;

            int targetX = (int)neighbour.X;

            int targetY = (int)neighbour.Y;

            if (targetX < 0 || targetX >= mapWidth || targetY < 0 || targetY >= mapHeight)
            {

                return "void";

            }

            backTile = backLayer.Tiles[targetX, targetY];

            if (backTile == null)
            {

                return "void";

            }

            if (backTile.TileIndex == 2154)
            {

                return "void";

            }

            if (backTile.TileIndexProperties.TryGetValue("Water", out _))
            {

                return "water";

            }

            if (npc)
            {
                PropertyValue barrier = null;

                backTile.Properties.TryGetValue("NPCBarrier", out barrier);

                if (barrier != null) { return "barrier"; }

                backTile.Properties.TryGetValue("TemporaryBarrier", out barrier);

                if (barrier != null) { return "barrier"; }

                backTile.TileIndexProperties.TryGetValue("Passable", out barrier);

                if (barrier != null) { return "barrier"; }

                Layer buildingLayer = targetLocation.Map.GetLayer("Buildings");

                if (buildingLayer != null)
                {

                    Tile buildingTile = buildingLayer.Tiles[targetX, targetY];

                    if (buildingTile != null)
                    {

                        buildingTile.TileIndexProperties.TryGetValue("Shadow", out barrier);

                        if (barrier != null) { return "barrier"; }

                        buildingTile.TileIndexProperties.TryGetValue("Passable", out barrier);

                        if (barrier != null) { return "ground"; }

                        buildingTile.TileIndexProperties.TryGetValue("NPCPassable", out barrier);

                        if (barrier != null) { return "ground"; }

                        buildingTile.Properties.TryGetValue("Passable", out barrier);

                        if (barrier != null) { return "ground"; }

                        buildingTile.Properties.TryGetValue("NPCPassable", out barrier);

                        if (barrier != null) { return "ground"; }

                        return "barrier";

                    }

                }

                return "ground";

            }

            if (backTile.TileIndexProperties.TryGetValue("Type", out var typeValue))
            {

                return "ground";

            }

            return "unknown";

        }

        public static Dictionary<string, List<Vector2>> NeighbourCheck(GameLocation targetLocation, Vector2 targetVector, int targetRadius = 1)
        {

            Dictionary<string, List<Vector2>> neighbourList = new();

            List<Vector2> neighbourVectors = GetTilesWithinRadius(targetLocation, targetVector, targetRadius);

            Layer buildingLayer = targetLocation.Map.GetLayer("Buildings");

            Layer pathsLayer = targetLocation.Map.GetLayer("Paths");

            if (!Mod.instance.targetCasts.ContainsKey(targetLocation.Name))
            {

                Mod.instance.targetCasts[targetLocation.Name] = LocationTargets(targetLocation);

            }

            foreach (Vector2 neighbourVector in neighbourVectors)
            {

                if (Mod.instance.targetCasts[targetLocation.Name].ContainsKey(neighbourVector))
                {

                    string targetType = Mod.instance.targetCasts[targetLocation.Name][neighbourVector];

                    if (!neighbourList.ContainsKey(targetType))
                    {

                        neighbourList[targetType] = new();

                    }

                    neighbourList[targetType].Add(neighbourVector);

                    continue;

                }

                Tile buildingTile = buildingLayer.PickTile(new Location((int)neighbourVector.X * 64, (int)neighbourVector.Y * 64), Game1.viewport.Size);

                if (buildingTile != null)
                {

                    if (buildingTile.TileIndexProperties.TryGetValue("Passable", out _) == false)
                    {

                        if (!neighbourList.ContainsKey("Wall"))
                        {

                            neighbourList["Wall"] = new();

                        }

                        neighbourList["Wall"].Add(neighbourVector);

                        continue;

                    }


                }

                if (pathsLayer != null)
                {

                    Tile pathsTile = buildingLayer.PickTile(new Location((int)neighbourVector.X * 64, (int)neighbourVector.Y * 64), Game1.viewport.Size);

                    if (pathsTile != null)
                    {

                        if (!neighbourList.ContainsKey("Path"))
                        {

                            neighbourList["Path"] = new();

                        }

                        neighbourList["Path"].Add(neighbourVector);

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

                            if (hoedCheck.crop != null)
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

                            if (!neighbourList.ContainsKey("Feature"))
                            {

                                neighbourList["Feature"] = new();

                            }

                            neighbourList["Feature"].Add(neighbourVector);

                            break;

                    }

                    continue;

                }

                if (targetLocation.objects.ContainsKey(neighbourVector))
                {

                    if (!neighbourList.ContainsKey("Object"))
                    {

                        neighbourList["Object"] = new();

                    }

                    neighbourList["Object"].Add(neighbourVector);

                    if (targetLocation.objects[neighbourVector] is StardewValley.Fence || targetLocation.objects[neighbourVector] is StardewValley.Objects.BreakableContainer || targetLocation.objects[neighbourVector].bigCraftable.Value)
                    {

                        if (!neighbourList.ContainsKey("BigObject"))
                        {

                            neighbourList["BigObject"] = new();

                        }

                        neighbourList["BigObject"].Add(neighbourVector);

                    }

                }


            }

            return neighbourList;

        }

        public static bool RandomTree(GameLocation targetLocation, Vector2 targetVector)
        {


            int treeIndex = Map.SpawnData.RandomTree(targetLocation);

            StardewValley.TerrainFeatures.Tree newTree = new(treeIndex, 1);

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

            if (generateItem == 829)
            {

                StardewValley.Crop newGinger = new(true, 2, targetX, targetY);

                hoeDirt.crop = newGinger;

                targetLocation.playSound("dirtyHit");

                Game1.stats.SeedsSown++;

                return;

            }

            hoeDirt.plant(generateItem, targetX, targetY, targetPlayer, false, targetLocation);

            //hoeDirt.crop.updateDrawMath(new Vector2(targetX, targetY));

        }

        public static bool GreetVillager(Farmer player, NPC villager, int friendShip = 0)
        {

            villager.faceTowardFarmerForPeriod(3000, 4, false, player);

            bool friendCheck = player.hasPlayerTalkedToNPC(villager.Name);

            if (!friendCheck)
            {

                player.friendshipData[villager.Name].TalkedToToday = true;

                if (friendShip > 0)
                {

                    player.changeFriendship(friendShip, villager);

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

                targetAnimal.friendshipTowardFarmer.Value = Math.Min(1000, targetAnimal.friendshipTowardFarmer.Value + num);

            }
            else
            {

                targetAnimal.friendshipTowardFarmer.Value = Math.Min(1000, targetAnimal.friendshipTowardFarmer.Value + 15);

            }

            if ((targetPlayer.professions.Contains(3) && !targetAnimal.isCoopDweller()) || (targetPlayer.professions.Contains(2) && targetAnimal.isCoopDweller()))
            {

                targetAnimal.friendshipTowardFarmer.Value = Math.Min(1000, targetAnimal.friendshipTowardFarmer.Value + 15);

                targetAnimal.happiness.Value = (byte)Math.Min(255, targetAnimal.happiness.Value + Math.Max(5, 40 - targetAnimal.happinessDrain.Value));

            }

            int num2 = 20;

            if (targetAnimal.wasAutoPet.Value)
            {

                num2 = 32;

            }

            targetAnimal.doEmote((targetAnimal.moodMessage.Value == 4) ? 12 : num2);

            targetAnimal.happiness.Value = (byte)Math.Min(255, targetAnimal.happiness.Value + Math.Max(5, 40 - targetAnimal.happinessDrain.Value));

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

                center + new Vector2(0,-8),

                center + new Vector2(2,-7),
                center + new Vector2(3,-6),

                center + new Vector2(4,-6),
                center + new Vector2(5,-5), // NE
                center + new Vector2(6,-4),

                center + new Vector2(6,-3),
                center + new Vector2(7,-2),

                center + new Vector2(8,0),

                center + new Vector2(7,2),
                center + new Vector2(6,3),

                center + new Vector2(6,4),
                center + new Vector2(5,5), // SE
                center + new Vector2(4,6),

                center + new Vector2(3,6),
                center + new Vector2(2,7),

                center + new Vector2(0,-8),

                center + new Vector2(-2,7),
                center + new Vector2(-3,6),

                center + new Vector2(-4,6),
                center + new Vector2(-5,5), // SW
                center + new Vector2(-6,4),

                center + new Vector2(-6,3),
                center + new Vector2(-7,2),

                center + new Vector2(-8,0),

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

        public static List<Vector2> GetTilesWithinRadius(GameLocation playerLocation, Vector2 center, int level, bool mapCheck = true, int segment = -1)
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

            if (segment != -1)
            {

                List<Vector2> segmentList = new();

                int segmentLength = templateList.Count / 4;

                int segmentModulus = templateList.Count % 4;

                int segmentAdjust = 0;

                if (segmentModulus > 0) { segmentAdjust = 1; }

                int segmentPortion = (segment * segmentLength) - (segmentLength / 2);

                for (int i = 0; i < segmentLength + segmentAdjust; i++)
                {

                    int segmentIndex = segmentPortion + i;

                    if (segmentIndex < 0) { segmentIndex = templateList.Count + segmentIndex; }

                    segmentList.Add(templateList[segmentIndex]);

                }

                templateList = segmentList;

            }


            float mapWidth = (playerLocation.Map.DisplayWidth / 16);

            float mapHeight = (playerLocation.Map.DisplayHeight / 16);

            if (!mapCheck)
            {

                return templateList;


            }

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

        public static Dictionary<Vector2, string> LocationTargets(GameLocation targetLocation)
        {

            Dictionary<Vector2, string> targetCasts = new();

            if (targetLocation.largeTerrainFeatures.Count > 0)
            {

                foreach (LargeTerrainFeature largeTerrainFeature in targetLocation.largeTerrainFeatures)
                {

                    if (largeTerrainFeature is not StardewValley.TerrainFeatures.Bush bushFeature)
                    {

                        continue;

                    }

                    Vector2 originVector = bushFeature.tilePosition.Value;

                    targetCasts[originVector] = "Bush";

                    targetCasts[new Vector2(originVector.X + 1, originVector.Y)] = "Bush";

                    targetCasts[new Vector2(originVector.X, originVector.Y + 1)] = "Bush";

                    targetCasts[new Vector2(originVector.X + 1, originVector.Y + 1)] = "Bush";

                }

            }

            if (targetLocation.resourceClumps.Count > 0)
            {

                foreach (ResourceClump resourceClump in targetLocation.resourceClumps)
                {

                    Vector2 originVector = resourceClump.tile.Value;

                    targetCasts[originVector] = "Clump";

                    targetCasts[new Vector2(originVector.X + 1, originVector.Y)] = "Clump";

                    targetCasts[new Vector2(originVector.X, originVector.Y + 1)] = "Clump";

                    targetCasts[new Vector2(originVector.X + 1, originVector.Y + 1)] = "Clump";

                }

            }

            if (targetLocation is Woods woodsLocation)
            {
                foreach (ResourceClump resourceClump in woodsLocation.stumps)
                {

                    Vector2 originVector = resourceClump.tile.Value;

                    targetCasts[originVector] = "Clump";

                    targetCasts[new Vector2(originVector.X + 1, originVector.Y)] = "Clump";

                    targetCasts[new Vector2(originVector.X, originVector.Y + 1)] = "Clump";

                    targetCasts[new Vector2(originVector.X + 1, originVector.Y + 1)] = "Clump";

                }

            }

            if (targetLocation.furniture.Count > 0)
            {
                foreach (Furniture item in targetLocation.furniture)
                {

                    Vector2 originVector = item.TileLocation;

                    for (int i = 0; i < (item.boundingBox.Width / 64); i++)
                    {

                        for (int j = 0; j < (item.boundingBox.Height / 64); j++)
                        {

                            targetCasts[new Vector2(originVector.X + i, originVector.Y + j)] = "Furniture";

                        }

                    }

                }

            }

            if (targetLocation is BuildableGameLocation farmLocation)
            {

                foreach (Building building in farmLocation.buildings)
                {

                    for (int i = 0; i < (building.tilesWide.Value / 64); i++)
                    {

                        for (int j = 0; j < (building.tilesHigh.Value / 64); j++)
                        {

                            targetCasts[new Vector2(building.tileX.Value + i, building.tileY.Value + j)] = "Building";

                        }

                    }

                }

            }

            return targetCasts;

        }

        public static int GetTravelDirection(Vector2 current, Vector2 previous)
        {

            Vector2 microVector = current - previous;

            int microX = Convert.ToInt32(microVector.X);

            int microY = Convert.ToInt32(microVector.Y);

            int newDirection;

            if (Math.Abs(microY) == Math.Abs(microX))
            {

                if (microY > 0)
                {

                    if (microX > 0) // upper right
                    {
                        newDirection = 4;
                    }
                    else // upper left
                    {
                        newDirection = 7;

                    }

                }
                else
                {

                    if (microX > 0) // lower right
                    {
                        newDirection = 5;
                    }
                    else // lower left
                    {
                        newDirection = 6;

                    }


                }

            }
            else if (Math.Abs(microY) < Math.Abs(microX))
            {

                if (microX > 0) // right
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

            return newDirection;

        }

        public static void AnimateRandomCritter(GameLocation location, Vector2 vector)
        {

            Dictionary<int, List<int>> critterList = new()
            {
                // base frame, move frame, frame length, loops, rect x, rect y, columns, offset x 64, offset y 64, left/right, upper/lower
                [0] = new() { 286, 280, 5, 3, 16, 16, 20, -88, 16, 1, 1 },
                [1] = new() { 306, 300, 5, 3, 16, 16, 20, -88, 16, 1, 1 },
                [2] = new() { 68, 54, 6, 2, 32, 32, 10, -160, 16, 1, 1 },
                [3] = new() { 69, 74, 6, 2, 32, 32, 10, -160, 16, 1, 1 },
                [4] = new() { 61, 62, 6, 2, 32, 32, 10, -160, 16, 1, 1 },

            };
            Dictionary<int, List<int>> birdList = new()
            {
                [0] = new() { 31, 31, 3, 4, 32, 32, 10, 160, -144, -1, -1 },
                [1] = new() { 51, 31, 3, 4, 32, 32, 10, 160, -144, -1, -1 },
            };

            for (int i = 0; i < 5; i++)
            {
                List<int> critterConfig;

                if (i < 3)
                {

                    critterConfig = critterList[Game1.random.Next(critterList.Count)];

                }
                else
                {

                    critterConfig = birdList[Game1.random.Next(birdList.Count)];

                }

                int offset = i % 2;

                int baseColumn = critterConfig[1] % critterConfig[6];

                int baseRow = (critterConfig[1] - baseColumn) / critterConfig[6];

                Microsoft.Xna.Framework.Rectangle animationRectangle = new(baseColumn * critterConfig[4], baseRow * critterConfig[5], critterConfig[4], critterConfig[5]);

                float animationInterval = 80f;

                int animationLength = critterConfig[2];

                int animationLoops = critterConfig[3];

                float critterOffsetX = 32f * offset * critterConfig[9];

                float critterOffsetY = 32f * offset * critterConfig[10];

                Microsoft.Xna.Framework.Color animationColor = Microsoft.Xna.Framework.Color.White;

                Vector2 animationPosition = new(vector.X * 64 + critterConfig[7] + critterOffsetX, vector.Y * 64 + critterConfig[8] + critterOffsetY);

                float animationSort = float.Parse("0.0" + vector.X.ToString() + vector.Y.ToString() + "1");

                TemporaryAnimatedSprite critterAnimation = new("TileSheets\\critters", animationRectangle, animationInterval, animationLength, animationLoops, animationPosition, flicker: false, flipped: false, animationSort, 0.001f, animationColor, 3f, 0f, 0f, 0f)
                {
                    motion = new Vector2(critterConfig[4] / 80f * critterConfig[9], 0), // (float)critterConfig[4] / 80f * critterConfig[9]
                    timeBasedMotion = true,
                };

                location.temporarySprites.Add(critterAnimation);

                // ---------------------------- puff

                animationSort = float.Parse("0.0" + vector.X.ToString() + vector.Y.ToString() + "2");

                TemporaryAnimatedSprite boltCloud = new("TileSheets\\animations", new(128, 5 * 64, 64, 64), 333f, 3, 1, animationPosition, flicker: false, false, animationSort, 0.02f, Color.White, 0.5f, 0f, 0f, 0f);

                location.temporarySprites.Add(boltCloud);

            }

        }

        public static void AnimateButterflySpray(GameLocation location, Vector2 vector)
        {

            location.critters.Add(new Butterfly(vector, false));

            location.critters.Add(new Butterfly(vector - new Vector2(1, 0), false));

            location.critters.Add(new Butterfly(vector + new Vector2(1, 0), false));

            location.critters.Add(new Butterfly(vector - new Vector2(2, 0), false));

            location.critters.Add(new Butterfly(vector + new Vector2(2, 0), false));

        }

        public static void AnimateRandomFish(GameLocation location, Vector2 vector)
        {

            Vector2 position = vector * 64;

            List<int> fishIndexes = new() { 138, 146, 717, };

            int fishIndex = fishIndexes[Game1.random.Next(fishIndexes.Count)];

            AnimateFishJump(location, position - new Vector2(32, 64), fishIndex, true);

            AnimateFishJump(location, position + new Vector2(32, 64), fishIndex, false);

            AnimateFishJump(location, position - new Vector2(32, 0), fishIndex, true);

            AnimateFishJump(location, position + new Vector2(32, 0), fishIndex, false);

        }

        public static void AnimateDeathSpray(GameLocation location, Vector2 position, Color color)
        {

            Utility.makeTemporarySpriteJuicier(new TemporaryAnimatedSprite(45, position, color, 10), location);

            for (int i = 1; i < 3; i++)
            {
                location.temporarySprites.Add(new TemporaryAnimatedSprite(6, position + new Vector2(0f, 1f) * 64f * i, color * 0.75f, 10)
                {
                    delayBeforeAnimationStart = i * 159
                });
                location.temporarySprites.Add(new TemporaryAnimatedSprite(6, position + new Vector2(0f, -1f) * 64f * i, color * 0.75f, 10)
                {
                    delayBeforeAnimationStart = i * 159
                });
                location.temporarySprites.Add(new TemporaryAnimatedSprite(6, position + new Vector2(1f, 0f) * 64f * i, color * 0.75f, 10)
                {
                    delayBeforeAnimationStart = i * 159
                });
                location.temporarySprites.Add(new TemporaryAnimatedSprite(6, position + new Vector2(-1f, 0f) * 64f * i, color * 0.75f, 10)
                {
                    delayBeforeAnimationStart = i * 159
                });
            }

            location.localSound("shadowDie");

        }

        public static float CalculateCritical(float damage, float critChance = 0.1f, float critModifier = 2f)
        {

            Random random = new();

            if (Game1.player.professions.Contains(25))
            {

                critChance += 0.15f;

            }

            if((float)random.NextDouble() > critChance)
            {

                return -1;

            }

            if (Game1.player.professions.Contains(29))
            {
                critModifier += 1;

            }

            damage *= critModifier;

            return damage;

        }

        public static List<int> CalculatePush(GameLocation location, StardewValley.Monsters.Monster monster, Vector2 from, int range = 128)
        {

            List<int> pushList = new() {  0, 0 };

            if (!monster.isGlider.Value && !MonsterData.CustomMonsters().Contains(monster.GetType()) && monster.Slipperiness != -1)
            {
                float num1 = monster.Position.X - from.X;

                float num2 = monster.Position.Y - from.Y;

                int diffX;

                int diffY;

                int num3 = 1;
                
                int num4 = 1;
                
                if ((double)num2 < 0.0)
                {
                    num3 = -1;
                }

                if ((double)num1 < 0.0)
                {
                    num4 = -1;
                }

                if ((double)Math.Abs(num1) < (double)Math.Abs(num2))
                {
                    float num5 = Math.Abs(num1) / Math.Abs(num2);

                    diffX = (int)(range * num4 * (double)num5);

                    diffY = range * num3;

                }
                else
                {
                    float num6 = Math.Abs(num2) / Math.Abs(num1);

                    diffX = range * num4;

                    diffY = (int)(range * num3 * (double)num6);

                }

                pushList[0] = diffX;

                pushList[1] = diffY;

                monster.stunTime = 50;

            }

            return pushList;

        }

        public static void HitMonster(GameLocation targetLocation, Farmer targetPlayer, StardewValley.Monsters.Monster targetMonster, int damage, bool critApplied, int diffX = 0, int diffY = 0)
        {

            bool specialHit = false;

            int damageDealt = 0;

            if (targetMonster is StardewValley.Monsters.Mummy mummy)
            {

                if (mummy.reviveTimer.Value > 0)
                {

                    damageDealt = mummy.takeDamage(1, 0, 0, true, 999f, targetPlayer);

                    specialHit = true;

                }

            }

            if (targetMonster is StardewValley.Monsters.Bug buggy)
            {

                damageDealt = 99;

                buggy.Health = 0;

                buggy.currentLocation.playSound("hitEnemy");

                buggy.deathAnimation();

                specialHit = true;

            }

            if(targetMonster is StardewValley.Monsters.RockCrab crabby)
            {
                Type reflectType = typeof(StardewValley.Monsters.RockCrab);

                FieldInfo reflectField = reflectType.GetField("shellGone", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                var shellGone = reflectField.GetValue(crabby);
;
                if (shellGone != null)
                {

                    if(!(NetBool)shellGone)
                    {
                        FieldInfo shellField = reflectType.GetField("shellHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                        var shellHealth = shellField.GetValue(crabby);
                        
                        for(int i = 0; i < (NetInt)shellHealth; i++)
                        {

                            crabby.hitWithTool(Mod.instance.virtualPick);

                        }

                    }

                }

            }

            if (!specialHit)
            {

                damageDealt = targetMonster.takeDamage(damage, diffX, diffY, false, 999f, targetPlayer);

            }

            foreach (BaseEnchantment enchantment in targetPlayer.enchantments)
            {
                enchantment.OnCalculateDamage(targetMonster, targetLocation, targetPlayer, ref damageDealt);
            }

            targetLocation.removeDamageDebris(targetMonster);

            Microsoft.Xna.Framework.Rectangle boundingBox = targetMonster.GetBoundingBox();

            Color color = new(255, 130, 0);

            if (critApplied)
            {

                color = Color.Yellow;

                targetLocation.playSound("crit");

            }

            targetLocation.debris.Add(new Debris(damageDealt, new Vector2(boundingBox.Center.X + 16, boundingBox.Center.Y), color, critApplied ? 1f + damageDealt / 300f : 1f, targetMonster));

            foreach (BaseEnchantment enchantment2 in targetPlayer.enchantments)
            {
                enchantment2.OnDealDamage(targetMonster, targetLocation, targetPlayer, ref damageDealt);
            }

            if (targetMonster.Health <= 0)
            {

                targetPlayer.checkForQuestComplete(null, 1, 1, null, targetMonster.Name, 4);

                if (Game1.player.team.specialOrders is not null)
                {
                    foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
                    {
                        if (specialOrder.onMonsterSlain != null)
                        {
                            specialOrder.onMonsterSlain(Game1.player, targetMonster);
                        }
                    }
                }

                foreach (BaseEnchantment enchantment3 in targetPlayer.enchantments)
                {
                    enchantment3.OnMonsterSlay(targetMonster, targetLocation, targetPlayer);
                }

                if (targetPlayer.leftRing.Value != null)
                {
                    targetPlayer.leftRing.Value.onMonsterSlay(targetMonster, targetLocation, targetPlayer);
                }

                if (targetPlayer.rightRing.Value != null)
                {
                    targetPlayer.rightRing.Value.onMonsterSlay(targetMonster, targetLocation, targetPlayer);
                }

                if (!(targetMonster is GreenSlime) || (targetMonster as GreenSlime).firstGeneration.Value)
                {
                    if (targetPlayer.IsLocalPlayer)
                    {
                        Game1.stats.monsterKilled(targetMonster.Name);
                    }
                    else if (Game1.IsMasterGame)
                    {
                        targetPlayer.queueMessage(25, Game1.player, targetMonster.Name);
                    }
                }

                targetLocation.monsterDrop(targetMonster, boundingBox.Center.X, boundingBox.Center.Y, targetPlayer);

                targetPlayer.gainExperience(4, targetMonster.ExperienceGained);

                if (targetMonster.isHardModeMonster.Value)
                {
                    Game1.stats.incrementStat("hardModeMonstersKilled", 1);
                }

                targetLocation.characters.Remove(targetMonster);

                Game1.stats.MonstersKilled++;

            }

        }

        public static List<Vector2> Explode(GameLocation targetLocation, Vector2 targetVector, Farmer targetPlayer, int damageRadius, int damage, int powerLevel = 1, int dirt = 0)
        {

            Tool Pick = Mod.instance.virtualPick;

            Tool Axe = Mod.instance.virtualAxe;

            List<Vector2> returnVectors = new();

            TemporaryAnimatedSprite bigAnimation = new(23, 9999f, 6, 1, new Vector2(targetVector.X * 64f, targetVector.Y * 64f), flicker: false, (Game1.random.NextDouble() < 0.5) ? true : false)
            {
                light = true,
                lightRadius = damageRadius + 1,
                lightcolor = Color.Black,
                alphaFade = 0.03f - 3f * 0.003f,
                Parent = targetLocation
            };

            targetLocation.temporarySprites.Add(bigAnimation);

            // ----------------- damage monsters

            List<StardewValley.Monsters.Monster> monsterList = new();

            if (damage > 0)
            {

                foreach (NPC nonPlayableCharacter in targetLocation.characters)
                {

                    if (nonPlayableCharacter is StardewValley.Monsters.Monster monsterCharacter)
                    {

                        if (monsterCharacter.IsInvisible || monsterCharacter.isInvincible() || monsterCharacter.Health <= 0)
                        {

                            continue;

                        }

                        monsterList.Add(monsterCharacter);

                    }

                }

            }

            if(monsterList.Count > 0)
            {

                Vector2 targetPosition = targetVector * 64;

                foreach (StardewValley.Monsters.Monster monster in monsterList)
                {

                    float threshold = 96 + (64 * damageRadius);

                    if (monster.Sprite.SpriteWidth > 16)
                    {
                        threshold += 32f;
                    }

                    if (monster.Sprite.SpriteWidth > 32)
                    {
                        threshold += 32f;
                    }

                    float monsterDifference = Vector2.Distance(monster.Position, targetPosition);

                    if (monsterDifference < threshold)
                    {

                        bool critApplied = false;

                        float critDamage = CalculateCritical(damage);

                        if (critDamage > 0)
                        {

                            damage = (int)critDamage;

                            critApplied = true;

                        }

                        List<int> diff = new() { 0, 0 };

                        if (powerLevel > 1)
                        {

                            CalculatePush(targetLocation, monster, targetPlayer.Position, 64);

                        }

                        HitMonster(targetLocation, targetPlayer, monster, damage, critApplied, diffX: diff[0], diffY: diff[1]);

                    }

                }

            }

            // ----------------- clump destruction

            if (targetLocation.resourceClumps.Count > 0 && powerLevel >= 3)
            {
                
                for (int index = targetLocation.resourceClumps.Count - 1; index >= 0; --index)
                {
                    
                    ResourceClump resourceClump = targetLocation.resourceClumps[index];
                    
                    Vector2 targetVector1 = resourceClump.tile.Value;
                    
                    if ((double)Vector2.Distance(targetVector1, targetVector) <= damageRadius + 1)
                    {
                        
                        switch (resourceClump.parentSheetIndex.Value)
                        {
                            case 600:
                            case 602:
                                DestroyStump(targetLocation, targetPlayer, resourceClump, targetVector1, "Farm");
                                break;
                            default:
                                DestroyBoulder(targetLocation, targetPlayer, resourceClump, targetVector1, 2);
                                break;
                        }
                    
                    }
                
                }
            
            }
            
            if (targetLocation is Woods woods && powerLevel >= 3)
            {
               
                for (int index = woods.stumps.Count - 1; index >= 0; --index)
                {
                    
                    ResourceClump stump = ((NetList<ResourceClump, NetRef<ResourceClump>>)woods.stumps)[index];
                    
                    Vector2 targetVector2 = stump.tile.Value;
                    
                    if ((double)Vector2.Distance(targetVector2, targetVector) <= damageRadius + 1)
                    {

                        DestroyStump(targetLocation, targetPlayer, stump, targetVector2, "Woods");

                    }

                }
            
            }


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

                        if (targetObject.Name == "Stone")
                        {
                            
                            if (powerLevel >= 2)
                            {
                                
                                Vector2 stoneVector = targetPlayer.getTileLocation();

                                targetLocation.OnStoneDestroyed(@targetObject.ParentSheetIndex, (int)stoneVector.X, (int)stoneVector.Y, targetPlayer);

                                targetLocation.objects.Remove(tileVector);

                                destroyVector = true;

                            }

                        }
                        else if (targetObject.Name.Contains("Twig"))
                        {

                            for (int fibreDebris = 2; fibreDebris < i; fibreDebris++)
                            {

                                Throw throwObject = new(targetPlayer, tileVector * 64, 388);

                                throwObject.ThrowObject();
                            }

                            targetLocation.objects.Remove(tileVector);

                            destroyVector = true;

                        }
                        else if (targetObject.Name.Contains("Weed"))
                        {

                            Throw throwObject = new(targetPlayer, tileVector * 64, 771);

                            throwObject.ThrowObject();

                            targetLocation.objects.Remove(tileVector);

                            destroyVector = true;

                        }
                        else if (targetObject.Name.Contains("SupplyCrate"))
                        {
                            targetObject.MinutesUntilReady = 1;

                            targetObject.performToolAction(Pick, targetLocation);

                            targetLocation.objects.Remove(tileVector);

                            destroyVector = true;
                        }
                        else if (targetObject is BreakableContainer breakableContainer)
                        {

                            breakableContainer.releaseContents(targetLocation, targetPlayer);

                            targetLocation.objects.Remove(tileVector);

                            targetLocation.playSound("barrelBreak", 0);

                            destroyVector = true;

                        }
                        else if (targetObject is Fence || targetObject is StardewValley.Objects.Workbench || targetObject is StardewValley.Objects.Furniture || targetObject is StardewValley.Objects.Chest)
                        {

                            // do nothing

                        }
                        else if (powerLevel >= 2)
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

                        if (powerLevel >= 2)
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

                                    Throw throwObject = new(targetPlayer, tileVector * 64, 771);

                                    throwObject.ThrowObject();

                                }

                                destroyVector = true;

                            }

                        }

                    }

                    if (destroyVector)
                    {

                        returnVectors.Add(tileVector);

                    }

                }


                if (dirt > 0 && dirt >= i)
                {

                    Layer backLayer = targetLocation.Map.GetLayer("Back");

                    int wet = (Game1.IsRainingHere(targetLocation) && (bool)targetLocation.IsOutdoors && !targetLocation.Name.Equals("Desert")) ? 1 : 0;

                    int dirtCount = 0;

                    foreach (Vector2 tileVector in tileVectors)
                    {

                        dirtCount++;

                        if(i == dirt && dirtCount % 2 == 1)
                        {

                            continue;

                        }

                        if(GroundCheck(targetLocation, tileVector) == "ground" && NeighbourCheck(targetLocation, tileVector,0).Count == 0)
                        {

                            int tilex = (int)tileVector.X;
                            int tiley = (int)tileVector.Y;

                            Tile backTile = backLayer.Tiles[tilex,tiley];

                            if (backTile.TileIndexProperties.TryGetValue("Diggable", out _))
                            {

                                targetLocation.checkForBuriedItem(tilex,tiley, explosion: false, detectOnly: false, Game1.player);

                                targetLocation.terrainFeatures.Add(tileVector, new HoeDirt(wet,targetLocation));

                            }
                            else if (backTile.TileIndexProperties.TryGetValue("Diggable", out _))
                            {
                                
                                targetLocation.checkForBuriedItem(tilex, tiley, explosion: false, detectOnly: false, Game1.player);

                                targetLocation.terrainFeatures.Add(tileVector, new HoeDirt(wet, targetLocation));

                            }

                        }

                    }

                }

            }

            return returnVectors;

        }

        public static void AnimateQuickWarp(GameLocation location, Vector2 position, string name = "Escape")
        {

            TemporaryAnimatedSprite firstWarp = new(0, 75f, 12, 1, position - new Vector2(16, 16), false, false)
            {

                sourceRect = new(0, 32, 32, 32),

                sourceRectStartingPos = new Vector2(0, 32),

                texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", name + ".png")),

                scale = 3f,

                layerDepth = 999,

                rotationChange = -0.8f,

                alpha = 0.35f

            };

            location.temporarySprites.Add(firstWarp);

        }

        public static void AnimateFate(GameLocation targetLocation, Vector2 playerVector, Vector2 targetVector, int fuelSource = 768, bool fadeAway = false, bool usePosition = false)
        {

            Vector2 targetPosition;

            Vector2 playerPosition;

            if (usePosition)
            {

                targetPosition = targetVector;

                playerPosition = playerVector - new Vector2(0, 64);

            }
            else
            {

                targetPosition = new(targetVector.X * 64, targetVector.Y * 64);

                playerPosition = (playerVector * 64) - new Vector2(0, 64);

            }

            float xOffset = (targetPosition.X - playerPosition.X);

            float yOffset = (targetPosition.Y - playerPosition.Y);

            float motionX = xOffset / 1000;

            float compensate = 0.555f;

            float motionY = (yOffset / 1000) - compensate;

            float animationSort = float.Parse("0.0" + targetVector.X.ToString() + targetVector.Y.ToString()) + 2;

            float animationFade = fadeAway ? 0.001f : 0f;

            Color tingleColor = (fuelSource == 768) ? Color.Yellow : Color.DarkBlue;

            Microsoft.Xna.Framework.Rectangle targetRectangle = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, fuelSource, 16, 16);

            TemporaryAnimatedSprite starAnimation = new("Maps\\springobjects", targetRectangle, 1000f, 1, 1, playerPosition, flicker: false, flipped: false, animationSort, animationFade, Color.White, 3f, 0f, 0f, 0.2f)
            {

                motion = new Vector2(motionX, motionY),

                acceleration = new Vector2(0f, 0.001f),

                timeBasedMotion = true,

            };

            targetLocation.temporarySprites.Add(starAnimation);

            animationSort = float.Parse("0.0" + targetVector.X.ToString() + targetVector.Y.ToString()) + 1;

            TemporaryAnimatedSprite tingleAnimation = new("TileSheets\\animations", new(0, 11 * 64, 64, 64), 62.5f, 8, 2, playerPosition, false, false, animationSort, animationFade, tingleColor, 1f, 0f, 0f, 0.2f)
            {

                motion = new Vector2(motionX, motionY),

                acceleration = new Vector2(0f, 0.001f),

                timeBasedMotion = true,

                delayBeforeAnimationStart = 40,

            };

            targetLocation.temporarySprites.Add(tingleAnimation);

            TemporaryAnimatedSprite tingleAnimationTwo = new("TileSheets\\animations", new(0, 11 * 64, 64, 64), 62.5f, 8, 2, playerPosition, false, false, animationSort, animationFade, tingleColor, 0.75f, 0f, 0f, 0.2f)
            {

                motion = new Vector2(motionX, motionY),

                acceleration = new Vector2(0f, 0.001f),

                timeBasedMotion = true,

                delayBeforeAnimationStart = 80,

            };

            targetLocation.temporarySprites.Add(tingleAnimationTwo);

            TemporaryAnimatedSprite tingleAnimationThree = new("TileSheets\\animations", new(0, 11 * 64, 64, 64), 62.5f, 8, 2, playerPosition, false, false, animationSort, animationFade, tingleColor, 0.75f, 0f, 0f, 0.2f)
            {

                motion = new Vector2(motionX, motionY),

                acceleration = new Vector2(0f, 0.001f),

                timeBasedMotion = true,

                delayBeforeAnimationStart = 120,

            };

            targetLocation.temporarySprites.Add(tingleAnimationThree);

            //targetLocation.playSoundPitched("yoba",700);

        }

        public static TemporaryAnimatedSprite AnimateFateTarget(GameLocation location, Vector2 playerVector, Vector2 targetVector)
        {

            Vector2 targetPosition = targetVector - new Vector2(0, 32);

            Vector2 playerPosition = playerVector - new Vector2(0, 32);

            float xOffset = (targetPosition.X - playerPosition.X);

            float yOffset = (targetPosition.Y - playerPosition.Y);

            float motionX = xOffset / 1000;

            float motionY = yOffset / 1000;

            float animationSort = float.Parse("0.0" + targetVector.X.ToString() + targetVector.Y.ToString()) + 2;

            TemporaryAnimatedSprite warpAnimation = new(0, 1000f, 1, 1, playerPosition, false, false)
            {

                sourceRect = new(0, 0, 64, 64),

                sourceRectStartingPos = new Vector2(0, 0),

                texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Warp.png")),
                scale = 0.75f,
                scaleChange = 0.0005f,

                layerDepth = animationSort,

                motion = new Vector2(motionX, motionY),

                timeBasedMotion = true,

                rotationChange = 0.08f,

            };

            location.temporarySprites.Add(warpAnimation);

            return warpAnimation;

        }

        public static bool MonsterVitals(StardewValley.Monsters.Monster Monster, GameLocation location)
        {

            if (Monster == null)
            {

                return false;

            }

            if (Monster.Health <= 0)
            {
                return false;

            }

            if (Monster.currentLocation == null)
            {

                return false;

            }

            if (!Monster.currentLocation.characters.Contains(Monster))
            {

                return false;

            }

            if (Monster.currentLocation.Name != location.Name)
            {

                return false;

            }

            return true;

        }

        public static void DestroyBoulder(GameLocation targetLocation,Farmer targetPlayer,ResourceClump resourceClump,Vector2 targetVector,int debrisMax)
        {
            Random random = new Random();
            int upgradeLevel = Mod.instance.virtualPick.UpgradeLevel;
            resourceClump.health.Set(1f);
            if (upgradeLevel < 3)
            {
                Pickaxe pickaxe1 = new Pickaxe();
                pickaxe1.UpgradeLevel = 3;
                targetPlayer.Stamina += Math.Min(2f, targetPlayer.MaxStamina - targetPlayer.Stamina);
                pickaxe1.DoFunction(targetLocation, 0, 0, 1, targetPlayer);
                resourceClump.performToolAction(pickaxe1, 1, targetVector, targetLocation);
            }
            else
            {
                //targetPlayer.Stamina += Math.Min(2f, targetPlayer.MaxStamina - targetPlayer.Stamina);
                //Mod.instance.virtualPick.DoFunction(targetLocation, 0, 0, 1, targetPlayer);
                resourceClump.performToolAction(Mod.instance.virtualPick, 1, targetVector, targetLocation);
            }
            resourceClump.NeedsUpdate = false;
            targetLocation._activeTerrainFeatures.Remove(resourceClump);
            targetLocation.resourceClumps.Remove(resourceClump);
            resourceClump.currentLocation = null;

            Throw throwObject;

            if (upgradeLevel >= 3)
            {
                //Game1.createObjectDebris(709, (int)targetVector.X, (int)targetVector.Y, -1, 0, 1f, null);
                //Game1.createObjectDebris(709, (int)targetVector.X + 1, (int)targetVector.Y, -1, 0, 1f, null);

                throwObject = new(targetPlayer, targetVector * 64, 709);

                throwObject.ThrowObject();
                throwObject.ThrowObject();

            }

            for (int index = 0; index < random.Next(1, debrisMax); ++index)
            {
                switch (resourceClump.parentSheetIndex.Value)
                {
                    case 756:
                    case 758:
                        //Game1.createObjectDebris(536, (int)targetVector.X, (int)targetVector.Y, -1, 0, 1f, null);

                        throwObject = new(targetPlayer, targetVector * 64, 536);

                        throwObject.ThrowObject();

                        break;
                    default:
                        if (targetLocation is MineShaft)
                        {
                            MineShaft mineShaft = (MineShaft)targetLocation;
                            if (mineShaft.mineLevel >= 80)
                            {
                               // Game1.createObjectDebris(537, (int)targetVector.X, (int)targetVector.Y, -1, 0, 1f, null);

                                throwObject = new(targetPlayer, targetVector * 64, 537);

                                throwObject.ThrowObject();

                                break;
                            }
                            if (mineShaft.mineLevel >= 121)
                            {
                                //Game1.createObjectDebris(749, (int)targetVector.X, (int)targetVector.Y, -1, 0, 1f, null);

                                throwObject = new(targetPlayer, targetVector * 64, 749);

                                throwObject.ThrowObject();

                                break;
                            }
                        }
                        //Game1.createObjectDebris(535, (int)targetVector.X, (int)targetVector.Y, -1, 0, 1f, null);

                        throwObject = new(targetPlayer, targetVector * 64, 535);

                        throwObject.ThrowObject();

                        break;
                }
            }
        }

        public static void DestroyStump(GameLocation targetLocation,Farmer targetPlayer,ResourceClump resourceClump,Vector2 targetVector,string resourceType)
        {
            int upgradeLevel = Mod.instance.virtualAxe.UpgradeLevel;
            resourceClump.health.Set(1f);
            if (upgradeLevel < 3)
            {
                Axe axe = new Axe();
                axe.UpgradeLevel = 3;
                axe.DoFunction(targetLocation, 0, 0, 1, targetPlayer);
                resourceClump.performToolAction(axe, 1, targetVector, targetLocation);
            }
            else
            {
                //targetPlayer.Stamina += Math.Min(2f, targetPlayer.MaxStamina - targetPlayer.Stamina);
                //(Mod.instance.virtualAxe).DoFunction(targetLocation, 0, 0, 1, targetPlayer);
                resourceClump.performToolAction(Mod.instance.virtualAxe, 1, targetVector, targetLocation);
            }
            resourceClump.NeedsUpdate = false;

            Throw throwObject;

            if (upgradeLevel >= 3)
            {
                //Game1.createObjectDebris(709, (int)targetVector.X, (int)targetVector.Y, -1, 0, 1f, null);
                //Game1.createObjectDebris(709, (int)targetVector.X + 1, (int)targetVector.Y, -1, 0, 1f, null);

                throwObject = new(targetPlayer, targetVector * 64, 709);

                throwObject.ThrowObject();
                throwObject.ThrowObject();
            }
            switch (resourceType)
            {
                case "Woods":
                    Woods woods = targetLocation as Woods;
                    if (!woods.stumps.Contains(resourceClump))
                        break;
                    woods.stumps.Remove(resourceClump);
                    break;
                case "Log":
                    (targetLocation as Forest).log = null;
                    break;
                default:
                    if (targetLocation._activeTerrainFeatures.Contains(resourceClump))
                    {
                        targetLocation._activeTerrainFeatures.Remove(resourceClump);

                    }

                    if (!targetLocation.resourceClumps.Contains(resourceClump))
                    {

                        break;
                    }

                    targetLocation.resourceClumps.Remove(resourceClump);

                    break;
            }
        }

        public static void AnimateRockfalls(GameLocation location, Vector2 vector)
        {
            Random random = new Random();
            for (int index1 = 0; index1 < 10; ++index1)
            {
                int index2 = index1 % 5;
                List<Vector2> tilesWithinRadius = GetTilesWithinRadius(location, vector, index2 + 2);
                if (random.Next(2) == 0)
                    tilesWithinRadius.Reverse();
                int count = tilesWithinRadius.Count;
                if (count != 0)
                {
                    int num1 = new List<int>() { 6, 8, 8, 7, 8 }[index2];
                    int num2 = new List<int>() { 2, 2, 3, 4, 4 }[index2];
                    for (int index3 = 0; index3 < num2; ++index3)
                    {
                        int minValue = num1 * index3;
                        if (minValue + 1 < count)
                        {
                            int castDelay = random.Next(3, 20) * 100;
                            int maxValue = Math.Min(minValue + num1, tilesWithinRadius.Count);
                            int index4 = random.Next(minValue, maxValue);
                            Vector2 vector2 = tilesWithinRadius[index4];
                            AnimateRockfall(location, vector2, castDelay);
                            for (int level = 0; level < 3; ++level)
                            {
                                foreach (Vector2 tilesWithinRadiu in ModUtility.GetTilesWithinRadius(location, vector2, level))
                                    AnimateCastRadius(location, tilesWithinRadiu, new Color(0.8f, 1f, 0.8f, 1f), castDelay + index2 * 200);
                            }
                        }
                    }
                }
            }
        }

        public static void AnimateRockfall(GameLocation targetLocation,Vector2 targetVector,int castDelay,int objectIndex = -1,int scatterIndex = -1)
        {
            if (objectIndex == -1)
            {
                List<int> intList = SpawnData.RockFall(targetLocation, Game1.player);
                objectIndex = intList[0];
                scatterIndex = intList[1];
            }
            Microsoft.Xna.Framework.Rectangle standardTileSheet1 = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, objectIndex, 16, 16);
            Microsoft.Xna.Framework.Rectangle standardTileSheet2 = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, scatterIndex, 16, 16);
            float num1 = 575f;
            Vector2 vector2 = new((float)(targetVector.X * 64.0 + 8.0), (float)((targetVector.Y - 3.0) * 64.0 - 24.0));
            float num2 = 0.0015f;
            float num3 = float.Parse("0.0" + targetVector.X.ToString() + targetVector.Y.ToString() + "1");
            TemporaryAnimatedSprite temporaryAnimatedSprite1 = new TemporaryAnimatedSprite("Maps\\springobjects", standardTileSheet2, num1, 1, 0, vector2, false, false, num3, 1f / 1000f, Color.White, 3f, 0.0f, 0.0f, 0.0f, false)
            {
                acceleration = new Vector2(0.0f, num2),
                timeBasedMotion = true,
                delayBeforeAnimationStart = castDelay
            };
            targetLocation.temporarySprites.Add(temporaryAnimatedSprite1);

            vector2 = new((float)(targetVector.X * 64.0 + 8.0), (float)((targetVector.Y - 3.0) * 64.0 + 8.0));
            float num4 = float.Parse("0.0" + targetVector.X.ToString() + targetVector.Y.ToString() + "2");
            TemporaryAnimatedSprite temporaryAnimatedSprite2 = new TemporaryAnimatedSprite("Maps\\springobjects", standardTileSheet1, num1, 1, 0, vector2, false, false, num4, 1f / 1000f, Color.White, 3f, 0.0f, 0.0f, 0.0f, false)
            {
                acceleration = new Vector2(0.0f, num2),
                timeBasedMotion = true,
                delayBeforeAnimationStart = castDelay
            };
            targetLocation.temporarySprites.Add(temporaryAnimatedSprite2);

            vector2 = new((float)(targetVector.X * 64.0 + 16.0), (float)(targetVector.Y * 64.0 + 16.0));
            float num5 = float.Parse("0.0" + targetVector.X.ToString() + targetVector.Y.ToString() + "3");
            TemporaryAnimatedSprite temporaryAnimatedSprite3 = new TemporaryAnimatedSprite("Maps\\springobjects", standardTileSheet1, num1, 1, 0, vector2, false, false, num5, 1f / 1000f, Color.Black * 0.5f, 2f, 0.0f, 0.0f, 0.0f, false)
            {
                delayBeforeAnimationStart = castDelay
            };
            targetLocation.temporarySprites.Add(temporaryAnimatedSprite3);
        }

        public static void DamageFarmers(GameLocation location,Microsoft.Xna.Framework.Rectangle zone,int damage,StardewValley.Monsters.Monster monster,bool parry = false)
        {
            foreach (Farmer farmer in Game1.getAllFarmers())
            {
                if (farmer.currentLocation.Name == location.Name)
                {
                    Microsoft.Xna.Framework.Rectangle boundingBox = farmer.GetBoundingBox();
                    if (boundingBox.Intersects(zone))
                    {
                        farmer.takeDamage(damage, parry, monster);

                    }

                }
            }
        }

    }

}
