/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardustCore.UIUtilities;

namespace Revitalize.Framework.Hacks
{
    /// <summary>
    /// Taken from SDV to be able to swap tool colors
    /// </summary>
    public static class ColorChanger
    {

        private static bool AxeNeedsReset;
        private static bool PickaxeNeedsReset;
        private static bool WateringCanNeedsReset;
        private static bool HoeNeedsReset;

        /// <summary>
        /// Swaps the colors for the pickaxe working animation.
        /// </summary>
        /// <param name="TargetTexture"></param>
        public static void SwapPickaxeTextures(Texture2D TargetTexture)
        {
            /*
            if (mostRecentPicxaxeSwap != null)
            {
                if (TargetTexture == mostRecentPicxaxeSwap) return;
            }
            */
            int height = Game1.toolSpriteSheet.Height;
            int width = Game1.toolSpriteSheet.Width;

            Color[] beforeData = new Color[Game1.toolSpriteSheet.Width * Game1.toolSpriteSheet.Height];
            Game1.toolSpriteSheet.GetData<Color>(beforeData);
            Color[] afterData = new Color[TargetTexture.Width * TargetTexture.Height]; //Get a color data to replace
            TargetTexture.GetData<Color>(afterData); //Get data from swap texture.

            ///Convert tool data to grid
            Color[,] beforeGrid = new Color[height, width];
            for (int row = 0; row < height; row++)
            {
                for (int column = 0; column < width; column++)
                {
                    // Assumes row major ordering of the array.
                    beforeGrid[row, column] = beforeData[row * width + column];
                }
            }
            //Convert target data to grid
            int targetHeight = TargetTexture.Height;
            int targetWidth = TargetTexture.Width;
            Color[,] afterGrid = new Color[targetHeight, targetWidth];
            for (int row = 0; row < targetHeight; row++)
            {
                for (int column = 0; column < targetWidth; column++)
                {
                    // Assumes row major ordering of the array.
                    afterGrid[row, column] = afterData[row * targetWidth + column];
                }
            }


            //Copy over data from the target texture into the before grid.
            Rectangle stoneRegion = new Rectangle(0,80,80,32);
            Rectangle copperRegion = new Rectangle();
            Rectangle ironRegion = new Rectangle(224, 80, 80, 32); //Create the region we want to replace.
            Rectangle goldRegion = new Rectangle();
            Rectangle irridumRegon = new Rectangle();

            List<Rectangle> rects = new List<Rectangle>();
            //rects.Add(ironRegion);
            rects.Add(stoneRegion);
            foreach (Rectangle region in rects)
            {

                for (int x = region.X; x < region.X + region.Width; x++)
                {
                    for (int y = region.Y; y < region.Y + region.Height; y++)
                    {
                        //ModCore.log("value is: " + new Vector2(x, y));
                        beforeGrid[y, x] = afterGrid[(int)(y - region.Y), x - region.X]; //Row, column order aka y,x order.
                    }
                }


                //Convert the tool grid back into a 1d color array
                for (int row = 0; row < height; row++)
                {
                    for (int column = 0; column < width; column++)
                    {
                        try
                        {
                            beforeData[row * width + column] = beforeGrid[row, column];
                        }
                        catch (Exception err)
                        {
                            ModCore.log("Setting pixel color at: " + new Vector2(column, row));
                            ModCore.log("That's position: " + (row * width + column).ToString());
                        }
                        //ModCore.log("Setting pixel color at: " + new Vector2(column, row));
                        //ModCore.log("That's position: " + (row * width + column).ToString());
                        //beforeData[row * targetWidth + column] = beforeGrid[row,column];
                    }
                }
            }
            

            //beforeGrid.CopyTo(beforeData, 0);
            //Reapply the texture.
            Game1.toolSpriteSheet.SetData<Color>(beforeData);

            //Stream stream = File.Create(Path.Combine(ModCore.ModHelper.DirectoryPath,"ToolTest.png"));
            //Game1.toolSpriteSheet.SaveAsPng(stream, width, height);
            //stream.Dispose();
            PickaxeNeedsReset = true;
        }

        /// <summary>
        /// Swaps the colors for the hoe animation.
        /// </summary>
        /// <param name="TargetTexture"></param>
        public static void SwapHoeTextures(Texture2D TargetTexture)
        {
            /*
            if (mostRecentPicxaxeSwap != null)
            {
                if (TargetTexture == mostRecentPicxaxeSwap) return;
            }
            */
            int height = Game1.toolSpriteSheet.Height;
            int width = Game1.toolSpriteSheet.Width;

            Color[] beforeData = new Color[Game1.toolSpriteSheet.Width * Game1.toolSpriteSheet.Height];
            Game1.toolSpriteSheet.GetData<Color>(beforeData);
            Color[] afterData = new Color[TargetTexture.Width * TargetTexture.Height]; //Get a color data to replace
            TargetTexture.GetData<Color>(afterData); //Get data from swap texture.

            ///Convert tool data to grid
            Color[,] beforeGrid = new Color[height, width];
            for (int row = 0; row < height; row++)
            {
                for (int column = 0; column < width; column++)
                {
                    // Assumes row major ordering of the array.
                    beforeGrid[row, column] = beforeData[row * width + column];
                }
            }
            //Convert target data to grid
            int targetHeight = TargetTexture.Height;
            int targetWidth = TargetTexture.Width;
            Color[,] afterGrid = new Color[targetHeight, targetWidth];
            for (int row = 0; row < targetHeight; row++)
            {
                for (int column = 0; column < targetWidth; column++)
                {
                    // Assumes row major ordering of the array.
                    afterGrid[row, column] = afterData[row * targetWidth + column];
                }
            }


            //Copy over data from the target texture into the before grid.
            Rectangle stoneRegion = new Rectangle(0, 16, 80, 32);
            Rectangle copperRegion = new Rectangle();
            Rectangle ironRegion = new Rectangle(224, 16, 80, 32); //Create the region we want to replace.
            Rectangle goldRegion = new Rectangle();
            Rectangle irridumRegon = new Rectangle();

            List<Rectangle> rects = new List<Rectangle>();
            //rects.Add(ironRegion);
            rects.Add(stoneRegion);
            foreach (Rectangle region in rects)
            {

                for (int x = region.X; x < region.X + region.Width; x++)
                {
                    for (int y = region.Y; y < region.Y + region.Height; y++)
                    {
                        //ModCore.log("value is: " + new Vector2(x, y));
                        beforeGrid[y, x] = afterGrid[(int)(y - region.Y), x - region.X]; //Row, column order aka y,x order.
                    }
                }


                //Convert the tool grid back into a 1d color array
                for (int row = 0; row < height; row++)
                {
                    for (int column = 0; column < width; column++)
                    {
                        try
                        {
                            beforeData[row * width + column] = beforeGrid[row, column];
                        }
                        catch (Exception err)
                        {
                            ModCore.log("Setting pixel color at: " + new Vector2(column, row));
                            ModCore.log("That's position: " + (row * width + column).ToString());
                        }
                        //ModCore.log("Setting pixel color at: " + new Vector2(column, row));
                        //ModCore.log("That's position: " + (row * width + column).ToString());
                        //beforeData[row * targetWidth + column] = beforeGrid[row,column];
                    }
                }
            }


            //beforeGrid.CopyTo(beforeData, 0);
            //Reapply the texture.
            Game1.toolSpriteSheet.SetData<Color>(beforeData);
            HoeNeedsReset = true;
        }

        /// <summary>
        /// Swaps the colors for the axe working animation.
        /// </summary>
        /// <param name="TargetTexture"></param>
        public static void SwapAxeTextures(Texture2D TargetTexture)
        {
            /*
            if (mostRecentPicxaxeSwap != null)
            {
                if (TargetTexture == mostRecentPicxaxeSwap) return;
            }
            */
            int height = Game1.toolSpriteSheet.Height;
            int width = Game1.toolSpriteSheet.Width;

            Color[] beforeData = new Color[Game1.toolSpriteSheet.Width * Game1.toolSpriteSheet.Height];
            Game1.toolSpriteSheet.GetData<Color>(beforeData);
            Color[] afterData = new Color[TargetTexture.Width * TargetTexture.Height]; //Get a color data to replace
            TargetTexture.GetData<Color>(afterData); //Get data from swap texture.

            ///Convert tool data to grid
            Color[,] beforeGrid = new Color[height, width];
            for (int row = 0; row < height; row++)
            {
                for (int column = 0; column < width; column++)
                {
                    // Assumes row major ordering of the array.
                    beforeGrid[row, column] = beforeData[row * width + column];
                }
            }
            //Convert target data to grid
            int targetHeight = TargetTexture.Height;
            int targetWidth = TargetTexture.Width;
            Color[,] afterGrid = new Color[targetHeight, targetWidth];
            for (int row = 0; row < targetHeight; row++)
            {
                for (int column = 0; column < targetWidth; column++)
                {
                    // Assumes row major ordering of the array.
                    afterGrid[row, column] = afterData[row * targetWidth + column];
                }
            }


            //Copy over data from the target texture into the before grid.
            Rectangle stoneRegion = new Rectangle(0, 144, 80, 32);
            Rectangle copperRegion = new Rectangle();
            Rectangle ironRegion = new Rectangle(224, 144, 80, 32); //Create the region we want to replace.
            Rectangle goldRegion = new Rectangle();
            Rectangle irridumRegon = new Rectangle();

            List<Rectangle> rects = new List<Rectangle>();
            //rects.Add(ironRegion);
            rects.Add(stoneRegion);
            foreach (Rectangle region in rects)
            {

                for (int x = region.X; x < region.X + region.Width; x++)
                {
                    for (int y = region.Y; y < region.Y + region.Height; y++)
                    {
                        //ModCore.log("value is: " + new Vector2(x, y));
                        beforeGrid[y, x] = afterGrid[(int)(y - region.Y), x - region.X]; //Row, column order aka y,x order.
                    }
                }


                //Convert the tool grid back into a 1d color array
                for (int row = 0; row < height; row++)
                {
                    for (int column = 0; column < width; column++)
                    {
                        try
                        {
                            beforeData[row * width + column] = beforeGrid[row, column];
                        }
                        catch (Exception err)
                        {
                            ModCore.log("Setting pixel color at: " + new Vector2(column, row));
                            ModCore.log("That's position: " + (row * width + column).ToString());
                        }
                        //ModCore.log("Setting pixel color at: " + new Vector2(column, row));
                        //ModCore.log("That's position: " + (row * width + column).ToString());
                        //beforeData[row * targetWidth + column] = beforeGrid[row,column];
                    }
                }
            }


            //beforeGrid.CopyTo(beforeData, 0);
            //Reapply the texture.
            Game1.toolSpriteSheet.SetData<Color>(beforeData);
            AxeNeedsReset = true;
        }

        /// <summary>
        /// Swaps the colors for the watering can working animation.
        /// </summary>
        /// <param name="TargetTexture"></param>
        public static void SwapWateringCanTextures(Texture2D TargetTexture)
        {
            /*
            if (mostRecentPicxaxeSwap != null)
            {
                if (TargetTexture == mostRecentPicxaxeSwap) return;
            }
            */
            int height = Game1.toolSpriteSheet.Height;
            int width = Game1.toolSpriteSheet.Width;

            Color[] beforeData = new Color[Game1.toolSpriteSheet.Width * Game1.toolSpriteSheet.Height];
            Game1.toolSpriteSheet.GetData<Color>(beforeData);
            Color[] afterData = new Color[TargetTexture.Width * TargetTexture.Height]; //Get a color data to replace
            TargetTexture.GetData<Color>(afterData); //Get data from swap texture.

            ///Convert tool data to grid
            Color[,] beforeGrid = new Color[height, width];
            for (int row = 0; row < height; row++)
            {
                for (int column = 0; column < width; column++)
                {
                    // Assumes row major ordering of the array.
                    beforeGrid[row, column] = beforeData[row * width + column];
                }
            }
            //Convert target data to grid
            int targetHeight = TargetTexture.Height;
            int targetWidth = TargetTexture.Width;
            Color[,] afterGrid = new Color[targetHeight, targetWidth];
            for (int row = 0; row < targetHeight; row++)
            {
                for (int column = 0; column < targetWidth; column++)
                {
                    // Assumes row major ordering of the array.
                    afterGrid[row, column] = afterData[row * targetWidth + column];
                }
            }


            //Copy over data from the target texture into the before grid.
            Rectangle stoneRegion = new Rectangle(0, 208, 80, 32);
            Rectangle copperRegion = new Rectangle();
            Rectangle ironRegion = new Rectangle(224, 208, 80, 32); //Create the region we want to replace.
            Rectangle goldRegion = new Rectangle();
            Rectangle irridumRegon = new Rectangle();

            List<Rectangle> rects = new List<Rectangle>();
            //rects.Add(ironRegion);
            rects.Add(stoneRegion);
            foreach (Rectangle region in rects)
            {

                for (int x = region.X; x < region.X + region.Width; x++)
                {
                    for (int y = region.Y; y < region.Y + region.Height; y++)
                    {
                        //ModCore.log("value is: " + new Vector2(x, y));
                        beforeGrid[y, x] = afterGrid[(int)(y - region.Y), x - region.X]; //Row, column order aka y,x order.
                    }
                }


                //Convert the tool grid back into a 1d color array
                for (int row = 0; row < height; row++)
                {
                    for (int column = 0; column < width; column++)
                    {
                        try
                        {
                            beforeData[row * width + column] = beforeGrid[row, column];
                        }
                        catch (Exception err)
                        {
                            ModCore.log("Setting pixel color at: " + new Vector2(column, row));
                            ModCore.log("That's position: " + (row * width + column).ToString());
                        }
                        //ModCore.log("Setting pixel color at: " + new Vector2(column, row));
                        //ModCore.log("That's position: " + (row * width + column).ToString());
                        //beforeData[row * targetWidth + column] = beforeGrid[row,column];
                    }
                }
            }


            //beforeGrid.CopyTo(beforeData, 0);
            //Reapply the texture.
            Game1.toolSpriteSheet.SetData<Color>(beforeData);
            WateringCanNeedsReset = true;
        }

        /// <summary>
        /// Resets the colors for the pickaxe.
        /// </summary>
        public static void ResetPickaxeTexture()
        {
            SwapPickaxeTextures(TextureManager.GetTexture(ModCore.Manifest, "Tools", "DefaultPickaxeWorking"));
        }

        /// <summary>
        /// Resets the colors for the hoe.
        /// </summary>
        public static void ResetHoeTexture()
        {
            SwapHoeTextures(TextureManager.GetTexture(ModCore.Manifest, "Tools", "DefaultHoeWorking"));
        }

        /// <summary>
        /// Resets the colors for the axe.
        /// </summary>
        public static void ResetAxeTexture()
        {
            SwapAxeTextures(TextureManager.GetTexture(ModCore.Manifest, "Tools", "DefaultAxeWorking"));
        }

        /// <summary>
        /// Resets the colors for the watering can.
        /// </summary>
        public static void ResetWateringCanTexture()
        {
            SwapAxeTextures(TextureManager.GetTexture(ModCore.Manifest, "Tools", "DefaultWateringCanWorking"));
        }

        /// <summary>
        /// Resets all of the custom color swaps for tools.
        /// </summary>
        public static void ResetToolColorSwaps()
        {
            if (PickaxeNeedsReset)
            {
                ResetPickaxeTexture();
                PickaxeNeedsReset = false;
            }
            if (HoeNeedsReset)
            {
                ResetHoeTexture();
                HoeNeedsReset = false;
            }
            if (AxeNeedsReset)
            {
                ResetAxeTexture();
                AxeNeedsReset = false;
            }
            if (WateringCanNeedsReset)
            {
                ResetWateringCanTexture();
                WateringCanNeedsReset = false;
            }
        }
    }
}
