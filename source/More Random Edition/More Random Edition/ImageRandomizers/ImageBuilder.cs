/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Linq;
using StardewValley;

namespace Randomizer
{
    public abstract class ImageBuilder
    {
        /// <summary>
        /// The image width in px - used when determining whether to crop and when drawing the image itself
        /// </summary>
        protected int ImageWidthInPx = 16;

        /// <summary>
        /// The image height in px - used when determining whether to crop and when drawing the image itself
        /// </summary>
        protected int ImageHeightInPx = 16;

        /// <summary>
        /// The offset width in px - used when positioning the image
        /// </summary>
        protected int OffsetWidthInPx = 16;

        /// <summary>
        /// The image height in px - used when positioning the image
        /// </summary>
        protected int OffsetHeightInPx = 16;

        /// <summary>
        /// The image height in px - this is the initial height to start drawing at
        /// </summary>
        protected int InitialHeightOffetInPx = 0;

        /// <summary>
        /// The name of the output file
        /// </summary>
        protected const string OutputFileName = "randomizedImage.png";

        /// <summary>
        /// The path for all the custom images
        /// </summary>
        protected readonly string CustomImagesPath = Globals.GetFilePath(Path.Combine("assets", "CustomImages"));

        /// <summary>
        /// The path to the custom images
        /// </summary>
        protected string ImageDirectory
        {
            get
            {
                return Path.Combine(CustomImagesPath, SubDirectory);
            }
        }

        /// <summary>
        /// The path to the custom images
        /// </summary>
        public string OutputFileFullPath
        {
            get
            {
                return Path.Combine(ImageDirectory, OutputFileName);
            }
        }

        /// <summary>
        /// The subdirectory where the base file and replacements are located
        /// </summary>
        protected string SubDirectory { get; set; }

        /// <summary>
        /// A list of positions in the file that will be overlayed to
        /// </summary>
        protected List<Point> PositionsToOverlay { get; set; }

        /// <summary>
        /// The files to pull from - gets all images in the directory that don't include the base file
        /// </summary>
        protected List<string> FilesToPullFrom { get; set; }

        /// <summary>
        /// The path for the asset that we will be modifying
        /// </summary>
        public string StardewAssetPath { get; set; }

        /// <summary>
        /// The public API to use to get the modified asset of the image builder
        /// </summary>
        /// <returns></returns>
        public Texture2D GenerateModifiedAsset()
        {
            // Deleting this first just in case randomizedImage.png does weird things
            File.Delete(Globals.GetFilePath(OutputFileFullPath));
            var modifiedImage = BuildImage();

            if (ShouldSaveImage() && Globals.Config.SaveRandomizedImages)
            {
                using FileStream stream = File.OpenWrite(OutputFileFullPath);
                modifiedImage.SaveAsPng(stream, modifiedImage.Width, modifiedImage.Height);
            }

            return modifiedImage;
        }

        /// <summary>
        /// Builds the image and saves the result into randomizedImage.png
        /// </summary>
        protected virtual Texture2D BuildImage()
        {
            // Do NOT dispose of this here, as it is the actual Stardew asset!
            Texture2D stardewAssetToModify = Globals.ModRef.Helper.GameContent
                .Load<Texture2D>(StardewAssetPath);

            FilesToPullFrom = GetAllCustomImages();
            foreach (Point position in PositionsToOverlay)
            {
                string randomFileName = GetRandomFileName(position);
                if (string.IsNullOrWhiteSpace(randomFileName) || !ShouldSaveImage(position))
                {
                    continue;
                }

                if (!File.Exists(randomFileName))
                {
                    Globals.ConsoleError($"File {randomFileName} does not exist! Using default image instead.");
                    continue;
                }

                using Texture2D originalRandomImage = Texture2D.FromFile(Game1.graphics.GraphicsDevice, randomFileName);
                using Texture2D randomImage = ManipulateImage(originalRandomImage, randomFileName);
                CropAndOverlayImage(position, randomImage, stardewAssetToModify);
            }

            return stardewAssetToModify;
        }

        /// <summary>
        /// Manipulate the image in some way - will be left to the parent classes to decide
        /// </summary>
        /// <param name="image"></param>
        /// <returns>The manipulated image (the input, in this case)</returns>
        protected virtual Texture2D ManipulateImage(Texture2D image, string fileName)
        {
            return image;
        }

        /// <summary>
        /// Crops out an image from the position and the random image given to us and places it
        /// onto a final image that we will use as the output
        /// Based on https://stackoverflow.com/questions/16137500/cropping-texture2d-on-all-sides-in-xna-c-sharp
        /// </summary>
        /// <param name="position">The position to start the cropping</param>
        /// <param name="randomImage">The image to crop from</param>
        /// <param name="finalImage">The image to crop to</param>
        private void CropAndOverlayImage(Point position, Texture2D randomImage, Texture2D finalImage)
        {
            int xOffset = position.X * OffsetWidthInPx;
            int yOffset = position.Y * OffsetHeightInPx + InitialHeightOffetInPx;

            Rectangle sourceRect = new Rectangle(0, 0, ImageWidthInPx, ImageHeightInPx);
            Color[] data = new Color[sourceRect.Width * sourceRect.Height];
            randomImage.GetData(0, sourceRect, data, 0, sourceRect.Width * sourceRect.Height);

            Rectangle destRect = new Rectangle(xOffset, yOffset, ImageWidthInPx, ImageHeightInPx);
            finalImage.SetData(0, destRect, data, 0, destRect.Width * destRect.Height);
        }

        /// <summary>
        /// Gets all the custom images from the given director
        /// </summary>
        /// <returns></returns>
        private List<string> GetAllCustomImages()
        {
            List<string> files = Directory.GetFiles(ImageDirectory).ToList();
            return files.Where(x =>
                !x.EndsWith(OutputFileName) &&
                x.EndsWith(".png"))
            .ToList();
        }

        /// <summary>
        /// Gets a random file name from the files to pull from and removes the found entry from the list
        /// </summary>
        /// <param name="position">The position of the instrument - unused in this version of the function</param>
        /// <returns></returns>
        protected virtual string GetRandomFileName(Point position)
        {
            string fileName = Globals.RNGGetAndRemoveRandomValueFromList(FilesToPullFrom);

            if (string.IsNullOrEmpty(fileName))
            {
                Globals.ConsoleWarn($"Not enough images at directory (need more images, using default image): {ImageDirectory}");
                return null;
            }

            return fileName;
        }

        /// <summary>
        /// Whether we should actually save the image file, or if the setting is off
        /// </summary>
        /// <returns />
        public abstract bool ShouldSaveImage();

        /// <summary>
        /// Whether we should actually save the image file, or if the setting is off
        /// This is used to check individual images - default is to check for the entire image builder
        /// </summary>
        /// <param name="point">The point to check at</param>
        /// <returns />
        protected virtual bool ShouldSaveImage(Point point)
        {
            return ShouldSaveImage();
        }
    }
}
