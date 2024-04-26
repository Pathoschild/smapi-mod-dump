/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

namespace Randomizer
{
    public class CritterPatcher : ImagePatcher
    {
        private static Texture2D CritterSpriteSheet { get; set; }

        /// <summary>
        /// The critter sprite sheet is kind of a mess of sprites all over the place
        /// This data is to group each animal into its own set so that we can hue
        /// shift each sprite in the same way
        /// </summary>
        private static readonly List<List<CritterSpriteLocations>> CritterLocationData = new()
        {
            // -- Birds --

            // Seagull
            new List<CritterSpriteLocations>() { 
                new(size: 32, startingPoint: new Point(0, 0), numberOfSprites: 10),
                new(size: 32, startingPoint: new Point(0, 32), numberOfSprites: 4)},

            // Crow
            new List<CritterSpriteLocations>() { 
                new(size: 32, startingPoint: new Point(128, 32), numberOfSprites: 6),
                new(size: 32, startingPoint: new Point(0, 64), numberOfSprites: 5)},

            // Brown Bird
            new List<CritterSpriteLocations>() {
                new(size: 32, startingPoint: new Point(160, 64), numberOfSprites: 5),
                new(size: 32, startingPoint: new Point(0, 96), numberOfSprites: 4),
                new(size: 12, startingPoint: new Point(47, 333), numberOfSprites: 5) },

            // Blue Bird
            new List<CritterSpriteLocations>() {
                new(size: 32, startingPoint: new Point(160, 128), numberOfSprites: 5),
                new(size: 32, startingPoint: new Point(0, 160), numberOfSprites: 4)},

            // Purple Bird
            new List<CritterSpriteLocations>() {
                new(size: 32, startingPoint: new Point(160, 384), numberOfSprites: 5),
                new(size: 32, startingPoint: new Point(0, 416), numberOfSprites: 4)},

            // Red Bird
            new List<CritterSpriteLocations>() {
                new(size: 32, startingPoint: new Point(160, 416), numberOfSprites: 5),
                new(size: 32, startingPoint: new Point(0, 448), numberOfSprites: 4)},

            // Owl
            new List<CritterSpriteLocations>() {
                new(size: 32, startingPoint: new Point(96, 256), numberOfSprites: 4) },
            
            // Woodpecker
            new List<CritterSpriteLocations>() {
                new(size: 16, startingPoint: new Point(0, 256), numberOfSprites: 5) },

            // -- Butterflies --

            // Big, colored
            new List<CritterSpriteLocations>() {
                new(size: 16, startingPoint: new Point(128, 96), numberOfSprites: 4) },
            new List<CritterSpriteLocations>() {
                new(size: 16, startingPoint: new Point(192, 96), numberOfSprites: 4) },
            new List<CritterSpriteLocations>() {
                new(size: 16, startingPoint: new Point(256, 96), numberOfSprites: 4) },
            new List<CritterSpriteLocations>() {
                new(size: 16, startingPoint: new Point(128, 112), numberOfSprites: 4) },
            new List<CritterSpriteLocations>() {
                new(size: 16, startingPoint: new Point(192, 112), numberOfSprites: 4) },
            new List<CritterSpriteLocations>() {
                new(size: 16, startingPoint: new Point(256, 112), numberOfSprites: 4) },

            // Small, patterned
            new List<CritterSpriteLocations>() {
                new(size: 16, startingPoint: new Point(0, 128), numberOfSprites: 3) },
            new List<CritterSpriteLocations>() {
                new(size: 16, startingPoint: new Point(48, 128), numberOfSprites: 3) },
            new List<CritterSpriteLocations>() {
                new(size: 16, startingPoint: new Point(96, 128), numberOfSprites: 3) },
            new List<CritterSpriteLocations>() {
                new(size: 16, startingPoint: new Point(0, 144), numberOfSprites: 3) },
            new List<CritterSpriteLocations>() {
                new(size: 16, startingPoint: new Point(48, 144), numberOfSprites: 3) },
            new List<CritterSpriteLocations>() {
                new(size: 16, startingPoint: new Point(96, 144), numberOfSprites: 3) },

            // Tropical
            new List<CritterSpriteLocations>() {
                new(size: 16, startingPoint: new Point(64, 288), numberOfSprites: 4) },
            new List<CritterSpriteLocations>() {
                new(size: 16, startingPoint: new Point(128, 288), numberOfSprites: 4) },
            new List<CritterSpriteLocations>() {
                new(size: 16, startingPoint: new Point(192, 288), numberOfSprites: 4) },
            new List<CritterSpriteLocations>() {
                new(size: 16, startingPoint: new Point(256, 288), numberOfSprites: 4) },
        
            // -- Ground Critters --

            // Gray Bunny
            new List<CritterSpriteLocations>() {
                new(size: 32, startingPoint: new Point(128, 160), numberOfSprites: 6),
                new(size: 32, startingPoint: new Point(256, 192), numberOfSprites: 1) },

            // White Bunny
            new List<CritterSpriteLocations>() {
                new(size: 32, startingPoint: new Point(288, 192), numberOfSprites: 1),
                new(size: 32, startingPoint: new Point(128, 224), numberOfSprites: 6) },

            // Squirrel
            new List<CritterSpriteLocations>() {
                new(size: 32, startingPoint: new Point(0, 192), numberOfSprites: 8) },

            // Frogs
            new List<CritterSpriteLocations>() {
                new(size: 16, startingPoint: new Point(0, 224), numberOfSprites: 7) },
            new List<CritterSpriteLocations>() {
                new(size: 16, startingPoint: new Point(0, 240), numberOfSprites: 7) },

            // Crab
            new List<CritterSpriteLocations>() {
                new(size: 18, startingPoint: new Point(0, 273), numberOfSprites: 3),
                new(size: 18, startingPoint: new Point(0, 291), numberOfSprites: 3)},

            // Red Monkey
            new List<CritterSpriteLocations>() {
                new(width: 20, height: 24, startingPoint: new Point(0, 309), numberOfSprites: 7),
                new(width: 15, height: 12, startingPoint: new Point(0, 333), numberOfSprites: 3) },

            // Monkey
            new List<CritterSpriteLocations>() {
                new(width: 20, height: 24, startingPoint: new Point(141, 309), numberOfSprites: 4) },

            // Gorilla
            new List<CritterSpriteLocations>() {
                new(size: 32, startingPoint: new Point(0, 352), numberOfSprites: 7) },
        };

        /// <summary>
        /// The path to the critter asset
        /// </summary>
        public const string StardewAssetPath = "TileSheets/critters";

        public CritterPatcher()
        {
            // This patcher doesn't actually use this, it's strictly for saving the randomized image
            SubFolder = "CustomImages/HueShiftedCritters";
        }

        public override void OnAssetRequested(IAssetData asset)
        {
            CritterSpriteSheet = Globals.ModRef.Helper.GameContent.Load<Texture2D>(StardewAssetPath);
            RNG rng = RNG.GetDailyRNG(nameof(CritterPatcher));

            var editor = asset.AsImage();
            HueShiftAllCritters(editor, rng);

            // With the way we're doing this, we need to re-load the asset to write the current one
            // so we'll only execute this if apprpriate
            if (Globals.Config.SaveRandomizedImages)
            {
                TryWriteRandomizedImage(Globals.ModRef.Helper.GameContent.Load<Texture2D>(StardewAssetPath));
            }
        }

        private static void HueShiftAllCritters(IAssetDataForImage editor, RNG rng)
        {
            foreach(List<CritterSpriteLocations> spriteLocsList in CritterLocationData)
            {
                int hueShiftValue = rng.NextIntWithinRange(0, Globals.Config.Animals.CritterHueShiftMax);
                foreach (CritterSpriteLocations spriteLocs in spriteLocsList)
                {
                    HueShiftFromSpriteLocs(spriteLocs, editor, hueShiftValue);
                }
            }
        }

        private static void HueShiftFromSpriteLocs(
            CritterSpriteLocations spriteLocs, 
            IAssetDataForImage editor, 
            int hueShiftValue)
        {
            int startingX = spriteLocs.StartingPoint.X;
            int y = spriteLocs.StartingPoint.Y;
            int width = spriteLocs.Width;
            int height = spriteLocs.Height;
            for (int i = 0; i < spriteLocs.NumberOfSprites; i++)
            {
                int x = (width * i) + startingX;
                Rectangle cropRectangle = new(x, y, width, height);

                using Texture2D hueShiftedCritter = ImageManipulator.ShiftImageHue(
                    ImageManipulator.Crop(CritterSpriteSheet, cropRectangle, Game1.graphics.GraphicsDevice),
                    hueShiftValue);

                editor.PatchImage(hueShiftedCritter, targetArea: cropRectangle);
            }
        }

        private class CritterSpriteLocations
        {
            /// <summary>
            /// The width of the sprite in pixels
            /// </summary>
            public int Width { get; set; }

            /// <summary>
            /// The height of the sprite in pixels
            /// </summary>
            public int Height { get; set; }

            /// <summary>
            /// Where to start grabbing rectangles - this is (x, y) in pixels
            /// </summary>
            public Point StartingPoint { get; set; }

            /// <summary>
            /// How many sprites in a row there are there
            /// </summary>
            public int NumberOfSprites { get; set; }

            public CritterSpriteLocations(int size, Point startingPoint, int numberOfSprites)
            {
                Width = size;
                Height = size;
                StartingPoint = startingPoint;
                NumberOfSprites = numberOfSprites;
            }

            public CritterSpriteLocations(int width, int height, Point startingPoint, int numberOfSprites)
            {
                Width = width;
                Height = height;
                StartingPoint = startingPoint;
                NumberOfSprites = numberOfSprites;
            }
        }
    }
}
