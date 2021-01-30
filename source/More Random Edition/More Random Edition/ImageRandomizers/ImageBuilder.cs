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
using System.Drawing;
using System.IO;
using System.Linq;

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
		private const string OutputFileName = "randomizedImage.png";

		/// <summary>
		/// The path for all the custom images
		/// </summary>
		protected readonly string CustomImagesPath = Globals.GetFilePath("Assets/CustomImages");

		/// <summary>
		/// The path to the custom images
		/// </summary>
		protected string ImageDirectory
		{
			get
			{
				return $"{CustomImagesPath}/{SubDirectory}";
			}
		}

		/// <summary>
		/// The path to the custom images
		/// </summary>
		public string BaseFileFullPath
		{
			get
			{
				return $"{ImageDirectory}/{BaseFileName}";
			}
		}

		/// <summary>
		/// The path to the custom images
		/// </summary>
		public string OutputFileFullPath
		{
			get
			{
				return $"{ImageDirectory}/{OutputFileName}";
			}
		}

		/// <summary>
		/// The output path as needed by SMAPI
		/// </summary>
		public string SMAPIOutputFilePath
		{
			get
			{
				return $"Assets/CustomImages/{SubDirectory}/{OutputFileName}";
			}
		}

		/// <summary>
		/// The name of the base file
		/// </summary>
		protected string BaseFileName { get; set; }

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
		/// Builds the image and saves the result into randomizedImage.png
		/// </summary>
		public void BuildImage()
		{
			Bitmap finalImage = null;

			finalImage = new Bitmap(BaseFileFullPath);
			using (Graphics graphics = Graphics.FromImage(finalImage))
			{
				FilesToPullFrom = GetAllCustomImages();
				foreach (Point position in PositionsToOverlay)
				{
					string randomFileName = GetRandomFileName(position);
					if (string.IsNullOrWhiteSpace(randomFileName) || !ShouldSaveImage(position)) { continue; }

					if (!File.Exists(randomFileName))
					{
						Globals.ConsoleError($"File {randomFileName} does not exist! Using default image instead.");
						continue;
					}

					Bitmap bitmap = new Bitmap(randomFileName);
					if (bitmap.Width != ImageWidthInPx || bitmap.Height != ImageHeightInPx)
					{
						bitmap = CropImage(bitmap, ImageWidthInPx, ImageHeightInPx);
					}

					int xOffset = position.X * OffsetWidthInPx;
					int yOffset = position.Y * OffsetHeightInPx + InitialHeightOffetInPx;

					graphics.FillRectangle(
						new SolidBrush(Color.FromArgb(0, 0, 1)),
						new Rectangle(xOffset, yOffset, ImageWidthInPx, ImageHeightInPx));
					graphics.DrawImage(bitmap, new Rectangle(xOffset, yOffset, ImageWidthInPx, ImageHeightInPx));
				}

				finalImage.MakeTransparent(Color.FromArgb(0, 0, 1));
				if (ShouldSaveImage())
				{
					finalImage.Save(OutputFileFullPath);
				}
			}
		}

		/// <summary>
		/// Crops the given image
		/// Based on https://stackoverflow.com/questions/734930/how-to-crop-an-image-using-c
		/// </summary>
		/// <param name="bitmap">The bitmap containing the original image</param>
		/// <param name="width">The desired width</param>
		/// <param name="height">The desired height</param>
		/// <returns>The cropped image</returns>
		public Bitmap CropImage(Bitmap bitmap, int width, int height)
		{
			Bitmap newBitmap = new Bitmap(bitmap);
			Rectangle rect = new Rectangle(0, 0, width, height);
			return newBitmap.Clone(rect, newBitmap.PixelFormat);
		}

		/// <summary>
		/// Gets all the custom images from the given directory excluding the base file name
		/// </summary>
		/// <returns></returns>
		private List<string> GetAllCustomImages()
		{
			List<string> files = Directory.GetFiles(ImageDirectory).ToList();
			return files.Where(x =>
				!x.EndsWith(OutputFileName) &&
				!x.EndsWith(BaseFileName) &&
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

		/// <summary>
		/// Cleans up all replacement files
		/// Should be called when the game is first loaded
		/// </summary>
		public static void CleanUpReplacementFiles()
		{
			File.Delete(Globals.GetFilePath("Assets/CustomImages/Bundles/randomizedImage.png"));
			File.Delete(Globals.GetFilePath("Assets/CustomImages/Weapons/randomizedImage.png"));
			File.Delete(Globals.GetFilePath("Assets/CustomImages/SpringObjects/randomizedImage.png"));
			File.Delete(Globals.GetFilePath("Assets/CustomImages/CropGrowth/randomizedImage.png"));
		}
	}
}
