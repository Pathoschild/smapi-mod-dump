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
using Microsoft.Xna.Framework.Media;
using Netcode;
using StardewDruid.Cast;
using StardewDruid.Cast.Weald;
using StardewDruid.Character;
using StardewDruid.Map;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.GameData.FarmAnimals;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Linq;
using xTile.Dimensions;
using xTile.Layers;
using xTile.ObjectModel;
using xTile.Tiles;

namespace StardewDruid
{
    static class ModUtility
    {

        //======================== Animations

        public static List<TemporaryAnimatedSprite> AnimateDecoration(GameLocation location, Vector2 origin, string name = "weald", float size = 1f, float interval = 1000f, float depth = 0.0001f)
        {

            List<TemporaryAnimatedSprite> animations = new();

            Vector2 originOffset = origin + new Vector2(32, 32) - (new Vector2(32, 32) * (3f * size));

            Microsoft.Xna.Framework.Rectangle rect = new(0, 0, 64, 64);

            Texture2D decorationTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Decorations.png"));

            switch (name)
            {

                case "mists":

                    rect.X += 64;

                    break;

                case "stars":

                    rect.X += 128;

                    break;

                case "fates":

                    rect.X += 192;

                    break;

                case "ether":

                    rect.Y += 64;

                    break;

            }

            TemporaryAnimatedSprite radiusAnimation = new(0, interval, 1, 1, originOffset, false, false)
            {

                sourceRect = rect,

                sourceRectStartingPos = new Vector2(rect.X, rect.Y),

                texture = decorationTexture,

                scale = 3f * size,

                timeBasedMotion = true,

                layerDepth = depth,

                rotationChange = (float)(Math.PI / 120),

                Parent = location,

                alpha = 0.65f,

            };

            location.temporarySprites.Add(radiusAnimation);

            animations.Add(radiusAnimation);

            /*if(name != "Ether")
            {

                TemporaryAnimatedSprite glyphAnimation = new(0, interval, 1, 1, originOffset, false, false)
                {

                    sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 64, 64, 64),

                    sourceRectStartingPos = new Vector2(0, 64),

                    texture = decorationTexture,

                    scale = 3f * size,

                    timeBasedMotion = true,

                    layerDepth = depth + 0.0001f,

                    rotationChange = 0 - (float)(Math.PI / 60),

                    Parent = location,

                    alpha = 0.45f,

                };

                location.temporarySprites.Add(glyphAnimation);

                animations.Add(glyphAnimation);

            }*/

            return animations;

        }

        public static void AnimateQuickWarp(GameLocation location, Vector2 origin, bool reverse = false)
        {

            Vector2 originOffset = origin - new Vector2(32, 32);

            Microsoft.Xna.Framework.Rectangle rect = reverse ? new(0, 32, 32, 32) : new(0, 0, 32, 32);

            TemporaryAnimatedSprite cursorAnimation = new(0, 75, 8, 1, originOffset, false, false)
            {

                sourceRect = rect,

                sourceRectStartingPos = new Vector2(0, rect.Y),

                texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Warp.png")),

                scale = 4f,

                layerDepth = 0.001f,

                alpha = 0.65f,

            };

            location.temporarySprites.Add(cursorAnimation);


        }

        public static TemporaryAnimatedSprite AnimateCursor(GameLocation location, Vector2 origin, string cursor = "weald", float interval = 1200f, float scale = 3f)
        {

            Vector2 originOffset = origin + new Vector2(32,32) - new Vector2(16*scale, 16*scale);

            Microsoft.Xna.Framework.Rectangle rect = new(0, 0, 32, 32);

            switch (cursor)
            {

                case "mists":

                    rect.X += 32;

                    break;

                case "stars":

                    rect.X += 64;

                    break;

                case "fates":

                    rect.X += 96;

                    break;

                case "comet":

                    rect.Y += 32;

                    break;

                case "target":

                    rect.X += 64;

                    rect.Y += 32;

                    break;

                case "death":

                    rect.X += 96;

                    rect.Y += 32;

                    break;

            }

            TemporaryAnimatedSprite cursorAnimation = new(0, interval, 1, 1, originOffset, false, false)
            {

                sourceRect = rect,

                sourceRectStartingPos = new Vector2(rect.X, rect.Y),

                texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Cursors.png")),

                scale = scale,

                layerDepth = 0.001f,

                timeBasedMotion = true,

                rotationChange = (float)(Math.PI / 60),

                alpha = 0.65f,

            };

            location.temporarySprites.Add(cursorAnimation);

            return cursorAnimation;

        }

        public static TemporaryAnimatedSprite AnimateCharge(GameLocation location, Vector2 origin, string cursor = "weald")
        {
            
            Vector2 originOffset = origin - new Vector2(24,24);

            Microsoft.Xna.Framework.Rectangle rect = new(0, 64, 32, 32);

            switch (cursor)
            {

                case "chaos":

                    rect.Y += 32;

                    break;

                case "mists":

                    rect.X += 32;

                    break;

                case "stars":

                    rect.X += 64;

                    break;

                case "fates":

                    rect.X += 96;

                    break;

            }

            TemporaryAnimatedSprite cursorAnimation = new(0, 2000, 1, 30, originOffset, false, false)
            {

                sourceRect = rect,

                sourceRectStartingPos = new Vector2(rect.X, rect.Y),

                texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Cursors.png")),

                scale = 3.5f,

                layerDepth = 0.001f,

                timeBasedMotion = true,

                rotationChange = (float)(Math.PI / 120),

                Parent = location,

                alpha = 0.65f,

            };

            location.temporarySprites.Add(cursorAnimation);

            return cursorAnimation;

        }

        public static void AnimateGlare(GameLocation location, Vector2 origin)
        {

            Vector2 originOffset = origin + new Vector2(16, 16);

            TemporaryAnimatedSprite glareAnimation = new(0, 1000f, 1, 1, originOffset, false, false)
            {

                sourceRect = new(0, 0, 32, 32),

                sourceRectStartingPos = new Vector2(0, 0),

                texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Glare.png")),

                scale = 1f,

                scaleChange = 0.002f,

                motion = new Vector2(-0.032f, -0.032f),

                layerDepth = 0.002f,

                timeBasedMotion = true,

                alphaFade = 0.0005f,

            };

            location.temporarySprites.Add(glareAnimation);

        }

        public static void AnimateHands(Farmer player, int direction, int timeFrame)
        {

            if (Mod.instance.DisableHands())
            {

                return;

            }

            player.Halt();

            FarmerSprite.AnimationFrame carryAnimation;

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

            player.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[1] { carryAnimation });

        }

        public static void AnimateSparkles(GameLocation targetLocation, Vector2 targetVector, Microsoft.Xna.Framework.Color animationColor) // DruidCastGrowth (0.8f, 1, 0.8f, 1)
        {

            Microsoft.Xna.Framework.Rectangle bounds = new((int)(targetVector.X * 64 - 64), (int)(targetVector.Y * 64 - 64), 192, 192);

            for (int i = 0; i < 5; i++)
            {
                int sparkleIndex = i % 2 == 0 ? 10 : 11;

                TemporaryAnimatedSprite sparkle = new(sparkleIndex, new Vector2(Game1.random.Next(bounds.X, bounds.Right), Game1.random.Next(bounds.Y, bounds.Bottom)), animationColor)
                {
                    delayBeforeAnimationStart = i*50,
                    texture = Game1.animations,
                };

                targetLocation.temporarySprites.Add(sparkle);

            }

            return;

        }

        public static void AnimateImpact(GameLocation targetLocation, Vector2 origin, int size, int frame = 0, string image = "Impact")
        {

            TemporaryAnimatedSprite bomb = new(0, 100f, 8-frame, 1, new(origin.X - 32 - (32 * size), origin.Y - 48 - (32 * size)), false, false)
            {
                sourceRect = new(64*frame, 0, 64, 64),
                sourceRectStartingPos = new Vector2(64 * frame, 0.0f),
                texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", image+".png")),
                scale = 2f + size,
                timeBasedMotion = true,
                layerDepth = 999f,
                rotationChange = 0.00628f,
                alpha = 0.35f,
                //alphaFade = 1f / 2000f
            };

            targetLocation.temporarySprites.Add(bomb);

            TemporaryAnimatedSprite light = new(23, 500f, 6, 1, origin, false, Game1.random.NextDouble() < 0.5)
            {
                texture = Game1.mouseCursors,
                light = true,
                lightRadius = 2 + size,
                lightcolor = Color.Black,
                alphaFade = 0.03f,
                Parent = targetLocation
            };

            targetLocation.temporarySprites.Add(light);

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

            float animationSort = (targetVector.Y / 10000);

            TemporaryAnimatedSprite newAnimation = new("TileSheets\\animations", animationRectangle, animationInterval, animationLength, animationLoops, animationPosition, false, animationFlipped, animationSort, 0.001f, animationColor, 0.75f, 0f, 0f, 0f);

            targetLocation.temporarySprites.Add(newAnimation);

            return;

        }

        public static void AnimateBolt(GameLocation location, Vector2 origin, int playSound = 800)
        {

            Vector2 originOffset = new(origin.X - 64, origin.Y - 304);

            TemporaryAnimatedSprite boltAnimation = new(0, 120, 5, 1, originOffset, false, (Game1.random.NextDouble() < 0.5) ? true : false)
            {

                sourceRect = new(0, 0, 64, 128),

                sourceRectStartingPos = new Vector2(0, 0),

                texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Bolt.png")),

                layerDepth = 999f,

                alpha = 0.65f,

                scale = 3f,

            };

            location.temporarySprites.Add(boltAnimation);

            Vector2 lightOffset = new(origin.X, origin.Y - 192);

            TemporaryAnimatedSprite lightAnimation = new(23, 500f, 1, 1, lightOffset, flicker: false, (Game1.random.NextDouble() < 0.5) ? true : false)
            {
                light = true,
                lightRadius = 6,
                lightcolor = Color.Black,
                alpha = 0.5f,
                alphaFade = 0.001f,
                Parent = location,

            };

            location.temporarySprites.Add(lightAnimation);

            if (playSound >= 500)
            {

                Game1.currentLocation.playSound("flameSpellHit", originOffset, 800);

            }

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

            DelayedAction.functionAfterDelay(SoundSlosh, 900);

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

            float animationSort = (targetVector.Y / 10000) + 0.00003f;

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

            animationSort = (targetVector.Y / 10000) + 0.00004f;

            TemporaryAnimatedSprite newAnimation = new("TileSheets\\animations", animationRectangle, animationInterval, animationLength, animationLoops, splashPosition, false, sloshFlip, animationSort, 0f, animationColor, 0.75f, 0f, 0.1f, 0f);

            targetLocation.temporarySprites.Add(newAnimation);

            // ------------------------------------

            TemporaryAnimatedSprite sloshAnimation = new(28, 200f, 2, 1, sloshPosition, flicker: false, flipped: false)
            {

                delayBeforeAnimationStart = 900,

            };

            targetLocation.temporarySprites.Add(sloshAnimation);

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
                        }
                    }
                }
            }
        }

        public static void AnimateRockfall(GameLocation targetLocation, Vector2 targetVector, int castDelay, int objectIndex = -1, int scatterIndex = -1)
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
            float num3 = (targetVector.Y / 10000) + 0.00001f;
            TemporaryAnimatedSprite temporaryAnimatedSprite1 = new TemporaryAnimatedSprite("Maps\\springobjects", standardTileSheet2, num1, 1, 0, vector2, false, false, num3, 1f / 1000f, Color.White, 3f, 0.0f, 0.0f, 0.0f, false)
            {
                acceleration = new Vector2(0.0f, num2),
                timeBasedMotion = true,
                delayBeforeAnimationStart = castDelay
            };
            targetLocation.temporarySprites.Add(temporaryAnimatedSprite1);

            vector2 = new((float)(targetVector.X * 64.0 + 8.0), (float)((targetVector.Y - 3.0) * 64.0 + 8.0));
            float num4 = (targetVector.Y / 10000) + 0.00002f;
            TemporaryAnimatedSprite temporaryAnimatedSprite2 = new TemporaryAnimatedSprite("Maps\\springobjects", standardTileSheet1, num1, 1, 0, vector2, false, false, num4, 1f / 1000f, Color.White, 3f, 0.0f, 0.0f, 0.0f, false)
            {
                acceleration = new Vector2(0.0f, num2),
                timeBasedMotion = true,
                delayBeforeAnimationStart = castDelay
            };
            targetLocation.temporarySprites.Add(temporaryAnimatedSprite2);

            vector2 = new((float)(targetVector.X * 64.0 + 16.0), (float)(targetVector.Y * 64.0 + 16.0));
            float num5 = (targetVector.Y / 10000) + 0.00003f;
            TemporaryAnimatedSprite temporaryAnimatedSprite3 = new TemporaryAnimatedSprite("Maps\\springobjects", standardTileSheet1, num1, 1, 0, vector2, false, false, num5, 1f / 1000f, Color.Black * 0.5f, 2f, 0.0f, 0.0f, 0.0f, false)
            {
                delayBeforeAnimationStart = castDelay
            };
            targetLocation.temporarySprites.Add(temporaryAnimatedSprite3);
        }

        public static void SoundSlosh()
        {
            Game1.currentLocation.localSound("quickSlosh");

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

                float animationSort = (vector.Y / 10000) + 0.00001f;

                TemporaryAnimatedSprite critterAnimation = new("TileSheets\\critters", animationRectangle, animationInterval, animationLength, animationLoops, animationPosition, flicker: false, flipped: false, animationSort, 0.001f, animationColor, 3f, 0f, 0f, 0f)
                {
                    motion = new Vector2(critterConfig[4] / 80f * critterConfig[9], 0), // (float)critterConfig[4] / 80f * critterConfig[9]
                    timeBasedMotion = true,
                };

                location.temporarySprites.Add(critterAnimation);

                // ---------------------------- puff

                animationSort = (vector.Y / 10000) + 0.00002f;

                TemporaryAnimatedSprite boltCloud = new("TileSheets\\animations", new(128, 5 * 64, 64, 64), 333f, 3, 1, animationPosition, flicker: false, false, animationSort, 0.02f, Color.White, 0.5f, 0f, 0f, 0f);

                location.temporarySprites.Add(boltCloud);

            }

        }

        public static void AnimateButterflySpray(GameLocation location, Vector2 vector)
        {

            location.critters.Add(new Butterfly(location,vector, false));

            location.critters.Add(new Butterfly(location, vector - new Vector2(1, 0), false));

            location.critters.Add(new Butterfly(location, vector + new Vector2(1, 0), false));

            location.critters.Add(new Butterfly(location, vector - new Vector2(2, 0), false));

            location.critters.Add(new Butterfly(location, vector + new Vector2(2, 0), false));

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

            float animationSort = (targetVector.Y / 10000) + 0.00001f;

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

            animationSort = (targetVector.Y / 10000) + 0.00002f;

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

        //======================== Gameworld Interactions

        public static bool RandomTree(GameLocation targetLocation, Vector2 targetVector)
        {

            int treeIndex = Map.SpawnData.RandomTree(targetLocation);

            StardewValley.TerrainFeatures.Tree newTree = new(treeIndex.ToString(), 1);

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

            hoeDirt.destroyCrop(true);

            if (generateItem == 829)
            {

                StardewValley.Crop newGinger = new(true, "2", targetX, targetY, targetLocation);

                hoeDirt.crop = newGinger;

                targetLocation.playSound("dirtyHit");

                Game1.stats.SeedsSown++;

                return;

            }

            hoeDirt.plant(generateItem.ToString(),targetPlayer, false);

            //hoeDirt.crop.updateDrawMath(new Vector2(targetX, targetY));

        }

        public static bool GreetVillager(Farmer player, NPC villager, int friendShip = 0)
        {

            bool friendCheck = player.hasPlayerTalkedToNPC(villager.Name);

            Game1.player.checkForQuestComplete(villager, -1, -1, null, null, 5);

            if (!friendCheck && player.friendshipData.ContainsKey(villager.Name))
            {

                villager.faceTowardFarmerForPeriod(3000, 4, false, player);

                player.friendshipData[villager.Name].TalkedToToday = true;

                if (friendShip > 0)
                {

                    player.changeFriendship(friendShip, villager);

                }

                return true;

            }

            return false;

        }

        public static void UpdateFriendship(Farmer player, List<string> NPCIndex)
        {

            foreach (string NPCName in NPCIndex)
            {

                NPC characterFromName = Game1.getCharacterFromName(NPCName);

                characterFromName ??= Game1.getCharacterFromName<Child>(NPCName, mustBeVillager: false);

                if (characterFromName != null && player.friendshipData.ContainsKey(NPCName))
                {

                    player.changeFriendship(375, characterFromName);

                }

            }

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

            FarmAnimalData animalData = targetAnimal.GetAnimalData();

            int num2 = animalData?.HappinessDrain ?? 0;

            if (animalData != null && animalData.ProfessionForHappinessBoost >= 0 && targetPlayer.professions.Contains(animalData.ProfessionForHappinessBoost))
            {

                targetAnimal.friendshipTowardFarmer.Value = Math.Min(1000, targetAnimal.friendshipTowardFarmer.Value + 15);

                targetAnimal.happiness.Value = (byte)Math.Min(255, (int)targetAnimal.happiness.Value + Math.Max(5, 30 + num2));

            }

            targetAnimal.doEmote(20);

            targetAnimal.happiness.Value = (byte)Math.Min(255, (int)targetAnimal.happiness.Value + Math.Max(5, 30 + num2));

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

        //======================== Tile Interactions

        public static bool WaterCheck(GameLocation targetLocation, Vector2 targetVector, int radius = 4)
        {
            bool check = true;

            Layer backLayer = targetLocation.Map.GetLayer("Back");

            Tile backTile;

            for (int i = 0; i < radius; i++)
            {

                List<Vector2> neighbours = GetTilesWithinRadius(targetLocation, targetVector, i);

                foreach (Vector2 neighbour in neighbours)
                {

                    backTile = backLayer.PickTile(new xTile.Dimensions.Location((int)neighbour.X * 64, (int)neighbour.Y * 64), Game1.viewport.Size);

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

            if(!targetLocation.IsOutdoors)
            {

                Layer frontLayer = Game1.player.currentLocation.Map.GetLayer("Front");
                
                Tile frontTile = frontLayer.Tiles[targetX, targetY];

                if (frontTile != null)
                {
                    return "void";

                }

            }

            if (backTile.TileIndexProperties.TryGetValue("Water", out _))
            {

                return "water";

            }

            PropertyValue backing = null;

            backTile.TileIndexProperties.TryGetValue("Type", out backing);

            if (backing != null)
            {

                return "ground";

            }

            if (targetLocation.IsOutdoors)
            {

                List<int> grounds = new() {304, 351, 404, 356, 300, 305, };

                if (grounds.Contains(backTile.TileIndex))
                {
                    return "ground";

                }

            }

            if(targetLocation is Caldera)
            {

                if(backTile.TileIndex == 28)
                {
                    return "ground";
                }

            }

            if (targetLocation is Sewer)
            {

                if (backTile.TileIndex == 34 || backTile.TileIndex == 41 || backTile.TileIndex == 42)
                {
                    return "ground";
                }

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

            return "unknown";

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

                    Vector2 originVector = bushFeature.Tile;

                    targetCasts[originVector] = "Bush";

                    switch (bushFeature.size.Value)
                    {
                        case 0:
                        case 3:
                            targetCasts[new Vector2(originVector.X, originVector.Y + 1)] = "Bush";
                            targetCasts[new Vector2(originVector.X, originVector.Y - 1)] = "Bush";
                            break;
                        case 1:
                        case 4:
                            targetCasts[new Vector2(originVector.X + 1, originVector.Y)] = "Bush";
                            targetCasts[new Vector2(originVector.X, originVector.Y + 1)] = "Bush";
                            targetCasts[new Vector2(originVector.X + 1, originVector.Y +1)] = "Bush";
                            targetCasts[new Vector2(originVector.X, originVector.Y -1)] = "Bush";
                            targetCasts[new Vector2(originVector.X + 1, originVector.Y -1)] = "Bush";
                            break;
                        case 2:
                            targetCasts[new Vector2(originVector.X + 1, originVector.Y)] = "Bush";
                            targetCasts[new Vector2(originVector.X + 2, originVector.Y)] = "Bush";
                            targetCasts[new Vector2(originVector.X, originVector.Y + 1)] = "Bush";
                            targetCasts[new Vector2(originVector.X + 1, originVector.Y +1)] = "Bush";
                            targetCasts[new Vector2(originVector.X + 2, originVector.Y +1)] = "Bush";
                            targetCasts[new Vector2(originVector.X, originVector.Y - 1)] = "Bush";
                            targetCasts[new Vector2(originVector.X + 1, originVector.Y - 1)] = "Bush";
                            targetCasts[new Vector2(originVector.X + 2, originVector.Y - 1)] = "Bush";
                            break;
                    }

                }

            }

            if (targetLocation.resourceClumps.Count > 0)
            {

                foreach (ResourceClump resourceClump in targetLocation.resourceClumps)
                {

                    Vector2 originVector = resourceClump.Tile;

                    targetCasts[originVector] = "Clump";

                    targetCasts[new Vector2(originVector.X + 1, originVector.Y)] = "Clump";

                    targetCasts[new Vector2(originVector.X, originVector.Y + 1)] = "Clump";

                    targetCasts[new Vector2(originVector.X + 1, originVector.Y + 1)] = "Clump";

                }

            }
            /*
            if (targetLocation is Woods woodsLocation)
            {
                foreach (ResourceClump resourceClump in woodsLocation.stumps)
                {

                    Vector2 originVector = resourceClump.Tile;

                    targetCasts[originVector] = "Clump";

                    targetCasts[new Vector2(originVector.X + 1, originVector.Y)] = "Clump";

                    targetCasts[new Vector2(originVector.X, originVector.Y + 1)] = "Clump";

                    targetCasts[new Vector2(originVector.X + 1, originVector.Y + 1)] = "Clump";

                }

            }*/

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
            foreach (Building building in targetLocation.buildings)
            {

                for (int i = 0; i < building.tilesWide.Value; i++)
                {

                    for (int j = 0; j < building.tilesHigh.Value; j++)
                    {

                        targetCasts[new Vector2(building.tileX.Value + i, building.tileY.Value + j)] = "Building";

                    }

                }

            }

            return targetCasts;

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

                Tile buildingTile = buildingLayer.PickTile(new xTile.Dimensions.Location((int)neighbourVector.X * 64, (int)neighbourVector.Y * 64), Game1.viewport.Size);

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

                    if (targetLocation is Beach)
                    {
                        
                        List<int> tidalList = new() { 60, 61, 62, 63, 77, 78, 79, 80, 94, 95, 96, 97, 104, 287, 288, 304, 305, 321, 362, 363 };

                        if (tidalList.Contains(buildingTile.TileIndex))
                        {

                            neighbourList["Pool"].Add(neighbourVector);

                            continue;

                        }

                    }

                }

                if (pathsLayer != null)
                {

                    Tile pathsTile = buildingLayer.PickTile(new xTile.Dimensions.Location((int)neighbourVector.X * 64, (int)neighbourVector.Y * 64), Game1.viewport.Size);

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

        public static List<Vector2> GetTilesBetweenVectors(GameLocation location, Vector2 distant, Vector2 near)
        {
            
            List<Vector2> vectorList = new();

            float increment = (int)(Vector2.Distance(distant, near) / 32);

            Vector2 factor = (distant - near) / increment;

            Vector2 check = near + factor;

            for(int i = 1; i <= increment; i++)
            {

                check += factor;

                Vector2 tile = new((int)(check.X / 64), (int)check.Y / 64);

                if (!vectorList.Contains(tile) && tile != near)
                {
                    vectorList.Add(tile);
                }

            }

            return vectorList;

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

            if (!monster.isGlider.Value && !MonsterData.BossMonster(monster) && monster.Slipperiness != -1)
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

                monster.stunTime.Set(Math.Max(monster.stunTime.Value,range));

            }

            return pushList;

        }

        public static List<Farmer> FarmerProximity(GameLocation targetLocation, Vector2 targetPosition, int radius, bool checkInvincible = false)
        {

            List<Farmer> farmers = new();

            int impact = radius * 64;

            impact += 32;

            impact = Math.Max(64, impact);

            foreach (Farmer farmer in Game1.getAllFarmers())
            {

                if(checkInvincible && farmer.temporarilyInvincible)
                {
                    
                    continue;

                }

                if (farmer.currentLocation.Name == targetLocation.Name)
                {

                    if (Vector2.Distance(targetPosition, farmer.Position) <= impact)
                    {

                        farmers.Add(farmer);

                    }

                }

            }

            return farmers;
        
        }

        public static void DamageFarmers(GameLocation targetLocation, List<Farmer> farmers, int damage, StardewValley.Monsters.Monster monster, bool parry = false)
        {

            if(farmers.Count == 0)
            {
                return;
            }

            foreach (Farmer farmer in farmers)
            {

                if ((farmer.health + farmer.buffs.Defense) - damage < 10)
                {

                    Mod.instance.CriticalCondition();

                    break;

                }

                farmer.takeDamage(damage, parry, monster);

            }

        }

        public static List<StardewValley.Monsters.Monster> MonsterProximity(GameLocation targetLocation, List<Vector2> targetPosition, int radius, bool checkInvincible = false)
        {

            List<StardewValley.Monsters.Monster> monsterList = new();

            float threshold = 32 + (64 * radius);

            foreach (NPC nonPlayableCharacter in targetLocation.characters)
            {

                if (nonPlayableCharacter is StardewValley.Monsters.Monster monster)
                {

                    if (monster.IsInvisible|| monster.Health <= 0)
                    {

                        continue;

                    }

                    if(checkInvincible && monster.isInvincible())
                    {
                        continue;
                    }

                    float monsterThreshold = threshold;

                    if (monster.Sprite.SpriteWidth > 16)
                    {
                        monsterThreshold += 32f;
                    }

                    if (monster.Sprite.SpriteWidth > 32)
                    {
                        monsterThreshold += 32f;
                    }

                    if (Proximation(monster.Position, targetPosition, monsterThreshold))
                    {

                        monsterList.Add(monster);

                    }

                }

            }

            return monsterList;

        }

        public static bool Proximation(Vector2 position, List<Vector2> positions, float threshold)
        {

            foreach (Vector2 attempt in positions)
            {

                float difference = Vector2.Distance(position, attempt);

                if (difference < threshold)
                {

                    return true;

                }

            }

            return false;

        }

        public static List<StardewValley.Monsters.Monster> MonsterIntersect(GameLocation targetLocation, Microsoft.Xna.Framework.Rectangle hitBox, bool checkInvincible = false)
        {

            List<StardewValley.Monsters.Monster> monsterList = new();

            foreach (NPC nonPlayableCharacter in targetLocation.characters)
            {

                if (nonPlayableCharacter is StardewValley.Monsters.Monster monster)
                {

                    if (monster.IsInvisible || monster.Health <= 0)
                    {

                        continue;

                    }

                    if (checkInvincible && monster.isInvincible())
                    {
                        continue;
                    }

                    Microsoft.Xna.Framework.Rectangle boundingBox = monster.GetBoundingBox();

                    if (boundingBox.Intersects(hitBox))
                    {

                        monsterList.Add(monster);

                    }

                }

            }

            return monsterList;

        }

        public static void DamageMonsters(GameLocation targetLocation, List<StardewValley.Monsters.Monster> monsterList, Farmer targetPlayer, int damage, bool push = false)
        {

            if(monsterList.Count == 0)
            {
                return;
            }

            foreach (StardewValley.Monsters.Monster monster in monsterList)
            {

                bool critApplied = false;

                float critDamage = CalculateCritical(damage);

                if (critDamage > 0)
                {

                    damage = (int)critDamage;

                    critApplied = true;

                }

                List<int> diff = new() { 0, 0 };

                if (push)
                {

                    CalculatePush(targetLocation, monster, targetPlayer.Position, 64);

                }

                Vector2 monsterPosition = monster.Position;

                HitMonster(targetLocation, targetPlayer, monster, damage, critApplied, diffX: diff[0], diffY: diff[1]);

            }

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

                    if(!(shellGone as NetBool).Value)
                    {
                        FieldInfo shellField = reflectType.GetField("shellHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                        var shellHealth = shellField.GetValue(crabby);
                        
                        for(int i = 0; i < (shellHealth as NetInt).Value; i++)
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

            foreach (StardewValley.Enchantments.BaseEnchantment enchantment in targetPlayer.enchantments)
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

            foreach (StardewValley.Enchantments.BaseEnchantment enchantment2 in targetPlayer.enchantments)
            {
                enchantment2.OnDealDamage(targetMonster, targetLocation, targetPlayer, ref damageDealt);
            }

            if (targetMonster.Health <= 0)
            {

                targetPlayer.checkForQuestComplete(null, 1, 1, null, targetMonster.Name, 4);

                if (Game1.player.team.specialOrders is not null)
                {
                    foreach (StardewValley.SpecialOrders.SpecialOrder specialOrder in Game1.player.team.specialOrders)
                    {
                        if (specialOrder.onMonsterSlain != null)
                        {
                            specialOrder.onMonsterSlain(Game1.player, targetMonster);
                        }
                    }
                }

                foreach (StardewValley.Enchantments.BaseEnchantment enchantment3 in targetPlayer.enchantments)
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
                    Game1.stats.Increment("hardModeMonstersKilled", 1);
                }

                //targetLocation.characters.Remove(targetMonster);

                Game1.stats.MonstersKilled++;

            }

        }

        public static List<Vector2> Explode(GameLocation targetLocation, Vector2 targetVector, Farmer targetPlayer, int radius, int powerLevel = 1)
        {

            Tool Pick = Mod.instance.virtualPick;

            Tool Axe = Mod.instance.virtualAxe;

            List<Vector2> returnVectors = new();

            // ----------------- clump destruction

            if (targetLocation.resourceClumps.Count > 0 && powerLevel >= 4)
            {
                
                for (int index = targetLocation.resourceClumps.Count - 1; index >= 0; --index)
                {
                    
                    ResourceClump resourceClump = targetLocation.resourceClumps[index];
                    
                    Vector2 targetVector1 = resourceClump.Tile;
                    
                    if ((double)Vector2.Distance(targetVector1, targetVector) <= radius + 1)
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

            // ----------------- object destruction

            List<Vector2> tileVectors;

            int impactRadius = radius + 1;

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

                                targetLocation.OnStoneDestroyed(@targetObject.ParentSheetIndex.ToString(), (int)tileVector.X, (int)tileVector.Y, targetPlayer);

                                targetLocation.objects.Remove(tileVector);

                                destroyVector = true;

                            }

                        }
                        else if (targetObject.Name.Contains("Twig"))
                        {

                            Throw throwObject = new(targetPlayer, tileVector * 64, 388);

                            throwObject.ThrowObject();

                            targetObject.onExplosion(targetPlayer);

                            targetLocation.objects.Remove(tileVector);

                            destroyVector = true;

                        }
                        else if (targetObject.Name.Contains("Weed"))
                        {

                            Throw throwObject = new(targetPlayer, tileVector * 64, 771);

                            throwObject.ThrowObject();

                            targetObject.onExplosion(targetPlayer);

                            targetLocation.objects.Remove(tileVector);

                            destroyVector = true;

                        }
                        else if (targetObject.Name.Contains("SupplyCrate"))
                        {
                            targetObject.MinutesUntilReady = 1;

                            targetObject.performToolAction(Pick);

                            targetLocation.objects.Remove(tileVector);

                            destroyVector = true;
                        }
                        else if (targetObject is BreakableContainer breakableContainer)
                        {

                            breakableContainer.releaseContents(targetPlayer);

                            targetLocation.objects.Remove(tileVector);

                            targetLocation.playSound("barrelBreak", tileVector*64);

                            destroyVector = true;

                        }
                        else if (targetObject is Fence || targetObject is StardewValley.Objects.Workbench || targetObject is StardewValley.Objects.Furniture || targetObject is StardewValley.Objects.Chest)
                        {

                            // do nothing

                        }
                        else if (powerLevel >= 3)
                        {

                            // ----------------- dislodge craftable

                            for (int j = 0; j < 2; j++)
                            {

                                Tool toolUse = (j == 0) ? Pick : Axe;

                                if (targetLocation.objects.ContainsKey(tileVector) && targetObject.performToolAction(toolUse))
                                {
                                    targetObject.performRemoveAction();

                                    targetObject.dropItem(targetLocation, tileVector * 64, tileVector * 64 + new Vector2(0, 32));

                                    targetLocation.objects.Remove(tileVector);

                                }

                            }

                        }

                    }

                    if (targetLocation.terrainFeatures.ContainsKey(tileVector))
                    {

                        if (powerLevel >= 3)
                        {

                            if (targetLocation.terrainFeatures[tileVector] is StardewValley.TerrainFeatures.Tree)
                            {

                                StardewValley.TerrainFeatures.Tree targetTree = targetLocation.terrainFeatures[tileVector] as StardewValley.TerrainFeatures.Tree;

                                if (targetTree.growthStage.Value >= 5)
                                {

                                    targetTree.performToolAction(null, (int)targetTree.health.Value, tileVector);

                                }
                                else
                                {

                                    targetTree.performToolAction(Axe, 0, tileVector);

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

            }

            return returnVectors;

        }

        public static void Reave(GameLocation targetLocation, Vector2 targetVector, Farmer targetPlayer, int radius)
        {
            
            List<Vector2> tileVectors;

            Layer backLayer = targetLocation.Map.GetLayer("Back");

            int wet = (Game1.IsRainingHere(targetLocation) && (bool)targetLocation.IsOutdoors && !targetLocation.Name.Equals("Desert")) ? 1 : 0;

            for (int i = 0; i < radius + 1; i++)
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

                int dirtCount = 0;

                foreach (Vector2 tileVector in tileVectors)
                {

                    dirtCount++;

                    if (i == radius && dirtCount % 2 == 1)
                    {

                        continue;

                    }

                    if (GroundCheck(targetLocation, tileVector) == "ground" && NeighbourCheck(targetLocation, tileVector, 0).Count == 0)
                    {

                        int tilex = (int)tileVector.X;
                        int tiley = (int)tileVector.Y;

                        Tile backTile = backLayer.Tiles[tilex, tiley];

                        if (backTile.TileIndexProperties.TryGetValue("Diggable", out _))
                        {

                            targetLocation.checkForBuriedItem(tilex, tiley, explosion: false, detectOnly: false, Game1.player);

                            targetLocation.terrainFeatures.Add(tileVector, new HoeDirt(wet, targetLocation));

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
                resourceClump.performToolAction(pickaxe1, 1, targetVector);
            }
            else
            {
                //targetPlayer.Stamina += Math.Min(2f, targetPlayer.MaxStamina - targetPlayer.Stamina);
                //Mod.instance.virtualPick.DoFunction(targetLocation, 0, 0, 1, targetPlayer);
                resourceClump.performToolAction(Mod.instance.virtualPick, 1, targetVector);
            }
            resourceClump.NeedsUpdate = false;
            targetLocation._activeTerrainFeatures.Remove(resourceClump);
            targetLocation.resourceClumps.Remove(resourceClump);

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
                resourceClump.performToolAction(axe, 1, targetVector);
            }
            else
            {
                //targetPlayer.Stamina += Math.Min(2f, targetPlayer.MaxStamina - targetPlayer.Stamina);
                //(Mod.instance.virtualAxe).DoFunction(targetLocation, 0, 0, 1, targetPlayer);
                resourceClump.performToolAction(Mod.instance.virtualAxe, 1, targetVector);
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
                /*case "Woods":
                    Woods woods = targetLocation as Woods;
                    if (!woods.stumps.Contains(resourceClump))
                        break;
                    woods.stumps.Remove(resourceClump);
                    break;
                case "Log":
                    (targetLocation as Forest).log = null;
                    break;*/
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

    }

}
