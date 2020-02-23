using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Randomizer
{
	public abstract class ImageBuilder
	{
		/// <summary>
		/// The size of the width/height in px
		/// </summary>
		protected const int SizeInPx = 16;


		/// <summary>
		/// The name of the output file
		/// </summary>
		private const string OutputFileName = "randomizedImage.png";

		/// <summary>
		/// The path to the custom images
		/// </summary>
		protected string ImageDirectory
		{
			get
			{
				return $"Mods/Randomizer/Assets/CustomImages/{SubDirectory}";
			}
		}

		/// <summary>
		/// The path to the custom images
		/// </summary>
		protected string BaseFileFullPath
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
		private List<string> _filesToPullFrom { get; set; }

		/// <summary>
		/// Builds the image and saves the result into randomizedImage.png
		/// </summary>
		public void BuildImage()
		{
			Bitmap finalImage = null;

			try
			{
				finalImage = new Bitmap(BaseFileFullPath);
				Graphics graphics = Graphics.FromImage(finalImage);

				_filesToPullFrom = GetAllCustomImages();
				foreach (Point position in PositionsToOverlay)
				{
					string randomFileName = GetRandomFileName(position);
					Bitmap bitmap = new Bitmap(randomFileName);
					int xOffset = position.X * SizeInPx;
					int yOffset = position.Y * SizeInPx;
					graphics.DrawImage(bitmap, new Rectangle(xOffset, yOffset, SizeInPx, SizeInPx));
				}

				if (Globals.Config.RandomizeWeapons && Globals.Config.UseCustomWeaponImages_Needs_Above_Setting_On)
				{
					finalImage.Save(OutputFileFullPath);
				}
			}

			catch (Exception ex)
			{
				if (finalImage != null)
				{
					finalImage.Dispose();
				}
				throw ex;
			}
		}

		/// <summary>
		/// Gets all the custom images from the directory excluding the base file name
		/// </summary>
		/// <returns></returns>
		private List<string> GetAllCustomImages()
		{
			List<string> files = Directory.GetFiles(ImageDirectory).ToList();
			return files.Where(x => x.EndsWith(".png") && !x.Contains(BaseFileName)).ToList();
		}

		/// <summary>
		/// Gets a random file name from the files to pull from and removes the found entry from the list
		/// </summary>
		/// <param name="position">The position of the instrument - unused in this version of the function</param>
		/// <returns></returns>
		protected virtual string GetRandomFileName(Point position)
		{
			return Globals.RNGGetAndRemoveRandomValueFromList(_filesToPullFrom);
		}

		/// <summary>
		/// Cleans up all replacement files
		/// Should be called when the game is first loaded, and when you return to the title screen
		/// </summary>
		public static void CleanUpReplacementFiles()
		{
			File.Delete($"Mods/Randomizer/Assets/CustomImages/Weapons/randomizedImage.png");
		}
	}
}
