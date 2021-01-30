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
		public Dictionary<int, string> CropIdsToImageNames;

		/// <summary>
		/// Keeps track of crop growth images to crop ids
		/// </summary>
		private Dictionary<Point, int> CropGrowthImagePointsToIds;

		public CropGrowthImageBuilder()
		{
			CropIdsToImageNames = new Dictionary<int, string>();
			BaseFileName = "Crops.png";
			SubDirectory = "CropGrowth";
			SetUpCropGrowthImagePointsToIds();
			PositionsToOverlay = CropGrowthImagePointsToIds.Keys.ToList();

			ImageHeightInPx = 32;
			ImageWidthInPx = 128;
			OffsetHeightInPx = 32;
			OffsetWidthInPx = 128;

			NormalImages = Directory.GetFiles($"{ImageDirectory}/{NormalDirectory}")
				.Where(x => x.EndsWith("-4.png") || x.EndsWith("-5.png"))
				.Select(x => x.Replace("-4.png", "").Replace("-5.png", ""))
				.Distinct()
				.OrderBy(x => x)
				.ToList();

			RegrowingImages = Directory.GetFiles($"{ImageDirectory}/{RegrowingDirectory}").Where(x => x.EndsWith(".png")).OrderBy(x => x).ToList();
			TrellisImages = Directory.GetFiles($"{ImageDirectory}/{TrellisDirectory}").Where(x => x.EndsWith(".png")).OrderBy(x => x).ToList();
			FlowerImages = Directory.GetFiles($"{ImageDirectory}/{FlowersDirectory}").Where(x => x.EndsWith(".png") && !x.EndsWith("-NoHue.png")).OrderBy(x => x).ToList();

			ValidateCropImages();
		}

		/// <summary>
		/// Gets the map of crop growth images to their ids
		/// Excludes Ancient Seeds, as they aren't randomized
		/// Coffee beans are also the crop, so use their seed ID as the crop ID
		/// </summary>
		/// <returns></returns>
		private void SetUpCropGrowthImagePointsToIds()
		{
			const int itemsPerRow = 2;

			CropGrowthImagePointsToIds = new Dictionary<Point, int>();
			List<int> seedIdsToExclude = new List<int>
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
			string fileName = "";

			int cropId = CropGrowthImagePointsToIds[position];
			Item item = ItemList.Items[cropId];

			SeedItem seedItem = item.Id == (int)ObjectIndexes.CoffeeBean ?
				(SeedItem)item : ((CropItem)item).MatchingSeedItem;
			CropGrowthInformation growthInfo = seedItem.CropGrowthInfo;

			FixWidthValue(seedItem.CropGrowthInfo.GraphicId);

			if (item.IsFlower)
			{
				fileName = Globals.RNGGetAndRemoveRandomValueFromList(FlowerImages);

				if (!seedItem.CropGrowthInfo.TintColorInfo.HasTint)
				{
					fileName = $"{fileName.Substring(0, fileName.Length - 4)}-NoHue.png";
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


			CropIdsToImageNames[cropId] = Path.GetFileName(fileName).Replace("-4.png", ".png").Replace("-5.png", ".png").Replace("-NoHue.png", ".png");
			return fileName;
		}


		/// <summary>
		/// Fix the width value given the graphic id
		/// This is to prevent the giant cauliflower from being cut off
		/// </summary>
		/// <param name="graphicId">The graphic ID to check</param>
		private void FixWidthValue(int graphicId)
		{
			List<int> graphicIndexesWithSmallerWidths = new List<int> { 32, 34 };
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
			string normalCropGrowthDirectory = $"{ImageDirectory}/{NormalDirectory}";
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
			string seedImageDirectory = $"{CustomImagesPath}/SpringObjects/Seeds";
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
			string cropImageDirectory = $"{CustomImagesPath}/SpringObjects/Crops";
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

			string flowerImageDirectory = $"{CustomImagesPath}/SpringObjects/Flowers";
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
			List<string> noHueFlowerImages = Directory.GetFiles($"{ImageDirectory}/{FlowersDirectory}")
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
		}
	}
}
