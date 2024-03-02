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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Randomizer
{
    public class CropGrowthImageBuilder : ImageBuilder
	{
		private const string NormalDirectory = "NormalCrops";
		private const string RegrowingDirectory = "RegrowingCrops";
		private const string TrellisDirectory = "TrellisCrops";
		private const string FlowersDirectory = "Flowers";

		private List<string> NormalImages { get; set; }
		private List<string> RegrowingImages { get; set; }
		private List<string> TrellisImages { get; set; }
		private List<string> FlowerImages { get; set; }

		/// <summary>
		/// Keeps track of crop ids mapped to image names so that all the crop images can be linked
		/// </summary>
		public Dictionary<int, CropImageLinkingData> CropIdsToLinkingData;

		/// <summary>
		/// Keeps track of crop growth images to crop ids
		/// </summary>
		private Dictionary<Point, int> CropGrowthImagePointsToIds;

        /// <summary>
        /// A reverse lookup since we have the image name when we need to find the crop id
        /// </summary>
        private readonly Dictionary<string, int> ImageNameToCropIds;

        public CropGrowthImageBuilder()
		{
			CropIdsToLinkingData = new Dictionary<int, CropImageLinkingData>();
            ImageNameToCropIds = new();

            StardewAssetPath = $"TileSheets/crops";
			SubDirectory = "CropGrowth";
			SetUpCropGrowthImagePointsToIds();
			PositionsToOverlay = CropGrowthImagePointsToIds.Keys.ToList();

			ImageHeightInPx = 32;
			ImageWidthInPx = 128;
			OffsetHeightInPx = 32;
			OffsetWidthInPx = 128;

			NormalImages = Directory.GetFiles(Path.Combine(ImageDirectory, NormalDirectory))
				.Where(x => x.EndsWith("-4.png") || x.EndsWith("-5.png"))
				.Select(x => x.Replace("-4.png", "").Replace("-5.png", ""))
				.Distinct()
				.OrderBy(x => x)
				.ToList();

			RegrowingImages = Directory.GetFiles(Path.Combine(ImageDirectory, RegrowingDirectory))
				.Where(x => x.EndsWith(".png"))
				.OrderBy(x => x)
				.ToList();
			TrellisImages = Directory.GetFiles(Path.Combine(ImageDirectory, TrellisDirectory))
				.Where(x => x.EndsWith(".png"))
				.OrderBy(x => x)
				.ToList();
			FlowerImages = Directory.GetFiles(Path.Combine(ImageDirectory, FlowersDirectory))
				.Where(x => x.EndsWith(".png") && !x.EndsWith("-NoHue.png"))
				.OrderBy(x => x)
				.ToList();

			ValidateCropImages();
		}

		/// <summary>
		/// Gets the map of crop growth images to their ids
		/// Excludes Ancient Seeds, as they aren't randomized
		/// Coffee beans are also the crop, so use their seed ID as the crop ID
		/// </summary>
		/// <returns />
		private void SetUpCropGrowthImagePointsToIds()
		{
			const int itemsPerRow = 2;

			CropGrowthImagePointsToIds = new Dictionary<Point, int>();
			List<int> seedIdsToExclude = new()
			{
				(int)ObjectIndexes.AncientSeeds
			};

			foreach (SeedItem seedItem in ItemList.GetSeeds().Where(x => !seedIdsToExclude.Contains(x.Id)).Cast<SeedItem>())
			{
				int sheetIndex = seedItem.CropGrowthInfo.GraphicId;
				int cropId = seedItem.Id == (int)ObjectIndexes.CoffeeBean ?
					seedItem.Id : seedItem.CropGrowthInfo.CropId;

				CropGrowthImagePointsToIds[new Point(sheetIndex % itemsPerRow, sheetIndex / itemsPerRow)] = cropId;
			}
		}

		/// <summary>
		/// Gets a random file name that matches the crop growth image at the given position
		/// Will remove the name found from the list
		/// </summary>
		/// <param name="position">The position</param>
		/// <returns>The selected file name</returns>
		protected override string GetRandomFileName(Point position)
		{
			string fileName;
			int cropId = CropGrowthImagePointsToIds[position];
			Item item = ItemList.Items[(ObjectIndexes)cropId];

			SeedItem seedItem = item.Id == (int)ObjectIndexes.CoffeeBean ?
				(SeedItem)item : ((CropItem)item).MatchingSeedItem;
			CropGrowthInformation growthInfo = seedItem.CropGrowthInfo;

			FixWidthValue(seedItem.CropGrowthInfo.GraphicId);

			if (item.IsFlower)
			{
				fileName = Globals.RNGGetAndRemoveRandomValueFromList(FlowerImages);

				if (!seedItem.CropGrowthInfo.TintColorInfo.HasTint)
				{
					fileName = $"{fileName[..^4]}-NoHue.png";
				}
			}

			else if (growthInfo.IsTrellisCrop)
			{
				fileName = Globals.RNGGetAndRemoveRandomValueFromList(TrellisImages);
			}

			else if (growthInfo.RegrowsAfterHarvest)
			{
				fileName = Globals.RNGGetAndRemoveRandomValueFromList(RegrowingImages);
			}

			else
			{
				fileName = Globals.RNGGetAndRemoveRandomValueFromList(NormalImages);

				if (growthInfo.GrowthStages.Count <= 4)
				{
					fileName += "-4.png";
				}

				else
				{
					fileName += "-5.png";
				}
			}

			if (string.IsNullOrEmpty(fileName) || fileName == "-4.png" || fileName == "-5.png")
			{
				Globals.ConsoleWarn($"Using default image for crop growth - you may not have enough crop growth images: {position.X}, {position.Y}");
				return null;
			}

			var linkingFileName = Path.GetFileName(fileName)
				.Replace("-4.png", ".png")
				.Replace("-5.png", ".png")
				.Replace("-NoHue.png", ".png");
            CropIdsToLinkingData[cropId] = new CropImageLinkingData(linkingFileName, seedItem);
            ImageNameToCropIds[fileName] = cropId; // Pass in this file name since it's the one we have in MainipulateImage
            return fileName;
		}

        /// <summary>
        /// Hue-shift the image to paste onto SpringObjects, if applicable
        /// </summary>
        /// <param name="image">The image to potentially hue shift</param>
        /// <param name="fileName">The full path of the image - needed so we can check the sub-directory</param>
        /// <returns>The manipulated image (or the input, if nothing is done)</returns>
        protected override Texture2D ManipulateImage(Texture2D image, string fileName)
        {
			string endingFileName = fileName.Split(
				$"CustomImages{Path.DirectorySeparatorChar}CropGrowth{Path.DirectorySeparatorChar}")[1];

			// Flowers are the only thing that we're NOT hue-shifting
			if (endingFileName.StartsWith("Flowers"))
			{
				return image;
            }

            if (ImageNameToCropIds.TryGetValue(fileName, out int cropId) &&
				CropIdsToLinkingData.TryGetValue(cropId, out CropImageLinkingData linkingData))
            {
                Random rng = Globals.GetFarmRNG($"{nameof(CropGrowthImageBuilder)}{fileName}");
                linkingData.HueShiftValue = Range.GetRandomValue(0, Globals.Config.Crops.HueShiftMax, rng);
                return ImageManipulator.ShiftImageHue(image, linkingData.HueShiftValue);
            }

            Globals.ConsoleError($"CropGrowthBuilder: Could not get linking data when manipulating image: {fileName}");
            return image;
        }

        /// <summary>
        /// Fix the width value given the graphic id
        /// This is to prevent the giant cauliflower from being cut off
        /// </summary>
        /// <param name="graphicId">The graphic ID to check</param>
        private void FixWidthValue(int graphicId)
		{
			List<int> graphicIndexesWithSmallerWidths = new() { 32, 34 };
			if (graphicIndexesWithSmallerWidths.Contains(graphicId))
			{
				ImageWidthInPx = 112;
			}
			else
			{
				ImageWidthInPx = 128;
			}
		}

		/// <summary>
		/// Whether the settings premit random crop growth images
		/// </summary>
		/// <returns>True if so, false otherwise</returns>
		public override bool ShouldSaveImage()
		{
			return Globals.Config.Crops.Randomize && Globals.Config.Crops.UseCustomImages;
		}

		/// <summary>
		/// Validates that the crop growth images map to the appropriate directories
		/// </summary>
		private void ValidateCropImages()
		{
			// Gather data for normal images
			string normalCropGrowthDirectory = Path.Combine(ImageDirectory, NormalDirectory);
			List<string> normalImageNames = Directory.GetFiles(normalCropGrowthDirectory).ToList();

			List<string> normal4StageImages = normalImageNames
				.Where(x => x.EndsWith("-4.png"))
				.Select(x => Path.GetFileName(x.Replace("-4.png", "")))
				.ToList();

			List<string> normal5StageImages = normalImageNames
				.Where(x => x.EndsWith("-5.png"))
				.Select(x => Path.GetFileName(x.Replace("-5.png", "")))
				.ToList();

			// Validate that the stage 4 and 5 match
			if (normal4StageImages.Count != normal5StageImages.Count)
			{
				string missingNumber = normal5StageImages.Count > normal4StageImages.Count ? "5" : "4";
				Globals.ConsoleWarn($"Missing a stage {missingNumber} image at: {normalCropGrowthDirectory}");
			}

			// Gather the all of the crop growth images and validate whether their names are all unique
			List<string> normalCropGrowthImages = NormalImages.Select(x => Path.GetFileNameWithoutExtension(x)).ToList();
			List<string> regrowingCropGrowthImages = RegrowingImages.Select(x => Path.GetFileNameWithoutExtension(x)).ToList();
			List<string> trellisCropGrowthImages = TrellisImages.Select(x => Path.GetFileNameWithoutExtension(x)).ToList();
			List<string> flowerCropGrowthImages = FlowerImages.Select(x => Path.GetFileNameWithoutExtension(x)).ToList();

			List<string> allCropGrowthImages = normalCropGrowthImages
				.Concat(regrowingCropGrowthImages)
				.Concat(trellisCropGrowthImages)
				.Concat(flowerCropGrowthImages)
				.Distinct()
				.ToList();

			int countOfEachPath = normalCropGrowthImages.Count + regrowingCropGrowthImages.Count + trellisCropGrowthImages.Count + flowerCropGrowthImages.Count;
			if (allCropGrowthImages.Count != countOfEachPath)
			{
				Globals.ConsoleWarn($"Duplicate image name detected in one of the folders at: {ImageDirectory}");
			}

			// Check that every crop growth image has a matching seed packet
			string seedImageDirectory = Path.Combine(CustomImagesPath, "SpringObjects", "Seeds");
			List<string> seedImageNames = Directory.GetFiles(seedImageDirectory)
				.Where(x => x.EndsWith(".png"))
				.Select(x => Path.GetFileNameWithoutExtension(x))
				.ToList();

			foreach (string growthImageName in allCropGrowthImages)
			{
				if (!seedImageNames.Contains(growthImageName))
				{
					Globals.ConsoleWarn($"{growthImageName}.png not found at: {seedImageDirectory}");
				}
			}

			// Check that all crop growth images exist as a crop or flower
			string cropImageDirectory = Path.Combine(CustomImagesPath, "SpringObjects", "Crops");
            List<string> cropImageNames = Directory.GetFiles(cropImageDirectory)
				.Where(x => x.EndsWith(".png"))
				.Select(x => Path.GetFileNameWithoutExtension(x))
				.ToList();

			foreach (string cropImageName in normalCropGrowthImages.Concat(regrowingCropGrowthImages).Concat(trellisCropGrowthImages))
			{
				if (!cropImageNames.Contains(cropImageName))
				{
					Globals.ConsoleWarn($"{cropImageName}.png not found at: {cropImageDirectory}");
				}
			}

			string flowerImageDirectory = Path.Combine(CustomImagesPath, "SpringObjects", "Flowers");
            List<string> flowerImageNames = Directory.GetFiles(flowerImageDirectory)
				.Where(x => x.EndsWith(".png"))
				.Select(x => Path.GetFileNameWithoutExtension(x))
				.ToList();

			foreach (string flowerImageName in flowerCropGrowthImages)
			{
				if (!flowerImageNames.Contains(flowerImageName))
				{
					Globals.ConsoleWarn($"{flowerImageName}.png not found at: {flowerImageDirectory}");
				}
			}

			// Check that each flower image contains the no-hue version
			List<string> noHueFlowerImages = Directory.GetFiles(Path.Combine(ImageDirectory, FlowersDirectory))
				.Where(x => x.EndsWith("-NoHue.png"))
				.Select(x => Path.GetFileNameWithoutExtension(x))
				.OrderBy(x => x)
				.ToList();

			foreach (string flowerImageName in flowerCropGrowthImages)
			{
				if (!noHueFlowerImages.Contains($"{flowerImageName}-NoHue"))
				{
					Globals.ConsoleWarn($"{flowerImageName}-NoHue.png not found at: {ImageDirectory}/{FlowersDirectory}");
				}
			}

			// Check that there's at least one seed packet template for trellis and non-trellis seeds
			string seedPacketDirectory = $"{CustomImagesPath}/SpringObjects/{SpringObjectsImageBuilder.SeedPacketDirectory}";
			string tellisPacketDirectory = $"{seedPacketDirectory}/{SpringObjectsImageBuilder.TrellisPacketSubDirectory}";

			if (!Directory.GetFiles(seedPacketDirectory).Where(x => x.EndsWith(".png")).Any()) 
			{
                Globals.ConsoleWarn($"No seed packet images found at: {seedPacketDirectory}");
            }

            if (!Directory.GetFiles(seedPacketDirectory).Where(x => x.EndsWith(".png")).Any())
            {
                Globals.ConsoleWarn($"No trellis packet images found at: {tellisPacketDirectory}");
            }
        }
	}
}
