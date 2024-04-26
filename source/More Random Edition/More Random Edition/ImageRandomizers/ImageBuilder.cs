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
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Randomizer
{
	public abstract class ImageBuilder
    {
        /// <summary>
        /// Data for the specific sprite to replace
        /// </summary>
        protected class SpriteOverlayData
        {
            /// <summary>
            /// The name of the tilesheet
            /// </summary>
            public string TilesheetName { get; set; }

            /// <summary>
            /// The position to overlay the sprite on the spritesheet
            /// </summary>
            public Point TilesheetPosition { get; set; }

            public SpriteOverlayData(string tilesheetName, int x, int y) 
            { 
                TilesheetName = tilesheetName;
                TilesheetPosition = new Point(x, y);
            }

			public SpriteOverlayData(string tilesheetName, Point tilesheetPosition)
			{
				TilesheetName = tilesheetName;
				TilesheetPosition = tilesheetPosition;
			}
		}

        /// <summary>
        /// The RNG to use when grabbing random images - must be initialized in each constructor
        /// </summary>
        protected RNG Rng { get; set; }

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
        /// A map indicating the items per row for each texture, based off of the ImageHeights
        /// </summary>
        private readonly Dictionary<string, int> ItemsPerRowMap = new();

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
        /// <param name="index">If null, uses just the image name; otherwise, prepends an index to it</param>
        public string GetOutputFilePath(int? index = null)
        {
            return index == null 
                ? Path.Combine(ImageDirectory, OutputFileName)
                : Path.Combine(ImageDirectory, $"{index} - {OutputFileName}");
		}

        /// <summary>
        /// The subdirectory where the base file and replacements are located
        /// </summary>
        protected string SubDirectory { get; set; }

        /// <summary>
        /// A list of positions in the file that will be overlayed to
        /// </summary>
        protected List<SpriteOverlayData> OverlayData { get; set; }

        /// <summary>
        /// The files to pull from - gets all images in the directory that don't include the base file
        /// </summary>
        protected List<string> FilesToPullFrom { get; set; }

		/// <summary>
		/// The stardew asset path to use for every image
		/// If this is set, it will ignore the TilesheetName when overlaying images
		/// while using SpriteOverlayData
		/// </summary>
		protected string GlobalStardewAssetPath { get; set; }

        
        /// <summary>
        /// The paths for the assets that we will be modifying
        /// - If we're using the global path returns a list with it as the only entry
        /// - Otherwise, grabs all the distinct tilesheet names
        /// </summary>
        public List<string> StardewAssetPaths { 
            get 
            {
                return GlobalStardewAssetPath == null
                    ? OverlayData.Select(data => data.TilesheetName).Distinct().ToList()
                    : new() { GlobalStardewAssetPath };
			} 
        }

		/// <summary>
		/// The public API to use to get the modified assets of the image builder
		/// </summary>
		/// <returns>A dictionary of the asset name to the texture that should replace it</returns>
		public Dictionary<string, Texture2D> GenerateModifiedAssets()
        {
            // Deleting the random images first in case randomizedImage.png does weird things
            CleanUpRandomizedImages();
			Dictionary<string, Texture2D> modifiedImages = BuildImages();

            if (ShouldSaveImage() && Globals.Config.SaveRandomizedImages)
            {
                int i = 1;
                foreach (Texture2D image in modifiedImages.Values)
                {
					using FileStream stream = File.OpenWrite(GetOutputFilePath(i));
					image.SaveAsPng(stream, image.Width, image.Height);
                    i++;
				}
            }

            return modifiedImages;
        }

        /// <summary>
        /// Cleans up all of the randomized images in the directory
        /// </summary>
        private void CleanUpRandomizedImages()
        {
            Directory.GetFiles(ImageDirectory)
                .Where(file => file.EndsWith(OutputFileName))
                .ToList()
                .ForEach(file => File.Delete(Globals.GetFilePath(file)));
		}

        /// <summary>
        /// Builds the image(s) and saves the result(s) into randomizedImage.png
        /// </summary>
        /// <returns>A dictionary of the asset name to the texture that should replace it</returns>
        protected virtual Dictionary<string, Texture2D> BuildImages()
        {
            Dictionary<string, Texture2D> modifiedAssets = new();

			FilesToPullFrom = GetAllCustomImages();
            foreach (SpriteOverlayData overlayData in OverlayData)
            {
				string randomFileName = GetRandomFileName(overlayData);
				if (string.IsNullOrWhiteSpace(randomFileName) || !ShouldSaveImage(overlayData))
				{
					continue;
				}

				if (!File.Exists(randomFileName))
				{
					Globals.ConsoleError($"File {randomFileName} does not exist! Using default image instead.");
					continue;
				}

				string assetName = overlayData.TilesheetName;
				Point position = overlayData.TilesheetPosition;

				// Do NOT dispose of the texture here, as it is the actual Stardew asset!
				if (!modifiedAssets.TryGetValue(assetName, out Texture2D stardewAssetToModify))
                {
                    stardewAssetToModify = Globals.ModRef.Helper.GameContent.Load<Texture2D>(assetName);
					modifiedAssets[assetName] = stardewAssetToModify;
				}

                using Texture2D originalRandomImage = Texture2D.FromFile(Game1.graphics.GraphicsDevice, randomFileName);
                using Texture2D randomImage = ManipulateImage(originalRandomImage, randomFileName);
                CropAndOverlayImage(position, randomImage, stardewAssetToModify);
            }

            return modifiedAssets;
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
        /// Gets all the custom images from the given directory
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
		/// <param name="overlayData">The tilesheet info of the sprite - unused in this version of the function</param>
		/// <returns></returns>
		protected virtual string GetRandomFileName(SpriteOverlayData overlayData)
        {
            string fileName = Rng.GetAndRemoveRandomValueFromList(FilesToPullFrom);

            if (string.IsNullOrEmpty(fileName))
            {
                Globals.ConsoleWarn($"Not enough images at directory (need more images, using default image): {ImageDirectory}");
                return null;
            }

            return fileName;
        }

        /// <summary>
        /// Gets the items per row for the given texture
        /// </summary>
        /// <param name="texture">The texture  - uses the global path if not given</param>
        /// <returns>The items per row for the given texture</returns>
        protected int GetItemsPerRow(string texture = null)
        {
            texture ??= GlobalStardewAssetPath;

            if (texture == null)
            {
                Globals.ConsoleError("Attempted to get items per row for a non-existant texture!");
                return 1;
            }

            if (ItemsPerRowMap.TryGetValue(texture, out int itemsPerRow))
            {
                return itemsPerRow;
			}

			int width = Globals.ModRef.Helper.GameContent
                .Load<Texture2D>(texture).Width;
            itemsPerRow = width / ImageWidthInPx;
            ItemsPerRowMap[texture] = itemsPerRow;

            return itemsPerRow;
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
        /// <param name="overlayData">The sprite to check</param>
        /// <returns />
        protected virtual bool ShouldSaveImage(SpriteOverlayData overlayData)
        {
            return ShouldSaveImage();
        }
    }
}
