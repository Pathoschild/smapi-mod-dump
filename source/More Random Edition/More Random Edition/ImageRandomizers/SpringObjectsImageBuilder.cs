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
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Randomizer
{
    public class SpringObjectsImageBuilder : ImageBuilder
	{
        private const string CropsDirectory = "Crops";
        private const string FlowersDirectory = "Flowers";
        private const string SeedsDirectory = "Seeds";
        public const string SeedPacketDirectory = "SeedPackets";
        public const string TrellisPacketSubDirectory = "TrellisPackets";
        private const string FishDirectory = "Fish";
        private const string BootsDirectory = "Boots";
		private const string FruitTreeSpritesImageName = "fruitTreeSprites";

        private List<string> FishImages { get; set; }
		private List<string> BootImages { get; set; }

		/// <summary>
		/// The number of items per row in the spring objects file
		/// </summary>
		protected const int ItemsPerRow = 24;

		/// <summary>
		/// Keeps track of mapped points to item ids so that GetRandomFileName can grab the matching ID
		/// </summary>
		private Dictionary<Point, int> PointsToItemIds;

		/// <summary>
		/// Keeps track of crop ids mapped to image names so that all the crop images can be linked
		/// </summary>
		private readonly Dictionary<int, CropImageLinkingData> CropIdsToLinkingData;

        /// <summary>
        /// A reverse lookup since we have the image name when we need to find the crop id
        /// </summary>
        private readonly Dictionary<string, int> ImageNameToCropIds;

        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="customFolderName">The folder name of the image type being built</param>
        public SpringObjectsImageBuilder(Dictionary<int, CropImageLinkingData> itemIdsToImageNames) : base()
		{
			CropIdsToLinkingData = itemIdsToImageNames;
			ImageNameToCropIds = new();

			StardewAssetPath = "Maps/springobjects";
            SubDirectory = "SpringObjects";

			SetAllItemMappings();
			PositionsToOverlay = PointsToItemIds.Keys.ToList();

			FishImages = Directory.GetFiles(Path.Combine(ImageDirectory, FishDirectory))
				.Where(x => x.EndsWith(".png"))
				.OrderBy(x => x)
				.ToList();

			BootImages = Directory.GetFiles(Path.Combine(ImageDirectory, BootsDirectory))
                .Where(x => x.EndsWith(".png"))
				.OrderBy(x => x)
				.ToList();
		}

		/// <summary>
		/// Sets the item mappings for all the spring objects
		/// Includes the cherry sapling becuase that's the first fruit tree and we need to
		/// replace the fruit tree sapling images as well
		/// </summary>
		private void SetAllItemMappings()
		{
			PointsToItemIds = new Dictionary<Point, int>();

			AddPointsToIdsMapping(FishItem.Get(true).Select(x => x.Id).ToList());
			AddPointsToIdsMapping(BootRandomizer.BootData.Select(x => x.Key).ToList());
			AddPointsToIdsMapping(CropItem.Get().Select(x => x.Id).ToList());
			AddPointsToIdsMapping(CropItem.Get().Select(x => x.MatchingSeedItem.Id).ToList());
			AddPointsToIdsMapping(new List<int> { (int)ObjectIndexes.CherrySapling, (int)ObjectIndexes.CoffeeBean });
		}

		/// <summary>
		/// Adds all items in the list to the dictionary of point mappings
		/// </summary>
		/// <param name="items"></param>
		/// <returns></returns>
		protected void AddPointsToIdsMapping(List<int> itemIds)
		{
			foreach (int id in itemIds)
			{
				PointsToItemIds[GetPointFromId(id)] = id;
			}
		}

		/// <summary>
		/// Gets the point in the springobjects file that belongs to the given item id
		/// </summary>
		/// <param name="id">The id</param>
		/// <returns />
		protected static Point GetPointFromId(int id)
		{
			return new Point(id % ItemsPerRow, id / ItemsPerRow);
		}

		/// <summary>
		/// Gets a random file name that matches the crop growth image at the given position (fish excluded)
		/// Will remove the name found from the list
		/// </summary>
		/// <param name="position">The position</param>
		/// <returns>The selected file name</returns>
		protected override string GetRandomFileName(Point position)
		{
			ImageWidthInPx = 16;

			int itemId = PointsToItemIds[position];
			string fileName = "";
			string subDirectory = "";
			if (BootRandomizer.BootData.Keys.Any(x => x == itemId))
			{
				fileName = Globals.RNGGetAndRemoveRandomValueFromList(BootImages);

				if (string.IsNullOrEmpty(fileName))
				{
					Globals.ConsoleWarn($"Could not find the boot image for id {itemId}; using default image instead.");
					return null;
				}

				return fileName;
			}

			Item item = ItemList.Items[(ObjectIndexes)itemId];

			if (item.Id == (int)ObjectIndexes.CherrySapling)
			{
				ImageWidthInPx = 96;
				return Path.Combine(ImageDirectory, $"{FruitTreeSpritesImageName}.png");
			}

			if (item.IsFish)
			{
				fileName = Globals.RNGGetAndRemoveRandomValueFromList(FishImages);

				if (string.IsNullOrEmpty(fileName))
				{
					Globals.ConsoleWarn($"Could not find the fish image for {item.Name}; using default image instead.");
					return null;
				}

				return fileName;
			}

			int cropId = item.Id;
			if (item.IsCrop || item.Id == (int)ObjectIndexes.CoffeeBean)
			{
				subDirectory = $"{Path.DirectorySeparatorChar}{CropsDirectory}";
			}

			else if (item.IsSeed)
			{
				SeedItem seedItem = (SeedItem)item;
				cropId = seedItem.CropGrowthInfo.CropId;
				subDirectory = $"{Path.DirectorySeparatorChar}{SeedsDirectory}";
			}

			if (item.IsFlower)
			{
				subDirectory = $"{Path.DirectorySeparatorChar}{FlowersDirectory}";

				CropItem cropItem = (CropItem)item;
				if (cropItem.MatchingSeedItem.CropGrowthInfo.TintColorInfo.HasTint)
				{
					ImageWidthInPx = 32; // Flower images include the stem and the top if they have tint
				}
			}

			if (!CropIdsToLinkingData.TryGetValue(cropId, out CropImageLinkingData linkingData))
			{
				Globals.ConsoleWarn($"Could not find the matching image for {item.Name}; using default image instead.");
				return null;
			}

			string fullImagePath = Path.Combine($"{ImageDirectory}{subDirectory}", linkingData.ImageName);
            ImageNameToCropIds[fullImagePath] = cropId; // Use this path because it's what we have in MainipulateImage
            return fullImagePath;
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
				$"CustomImages{Path.DirectorySeparatorChar}{SubDirectory}{Path.DirectorySeparatorChar}")[1];

			// Use the LinkingData to grab the hue shift value for crops and seeds
			bool isCropImage = endingFileName.StartsWith(CropsDirectory);
			bool isSeedImage = endingFileName.StartsWith(SeedsDirectory);
            if (isCropImage || isSeedImage)
			{
				return ManipulateCropImage(image, fileName, isSeedImage);
			}

			// Fish, boots, and saplings are three more candidates for hue-shifts
			bool isFishImage = endingFileName.StartsWith(FishDirectory);
			bool isBootImage = endingFileName.StartsWith(BootsDirectory);
			bool isFruitTreeImage = endingFileName.StartsWith(FruitTreeSpritesImageName);
            if (isFishImage || isBootImage || isFruitTreeImage ) 
			{
				var hueShiftMax = Globals.Config.Fish.HueShiftMax;
				if (isBootImage) 
				{ 
					hueShiftMax = Globals.Config.Boots.HueShiftMax;
				}
                else if (isFruitTreeImage) 
				{ 
					hueShiftMax = Globals.Config.Crops.HueShiftMax; 
				}

                Random rng = Globals.GetFarmRNG($"{nameof(SpringObjectsImageBuilder)}{fileName}");
                int hueShiftAmount = Range.GetRandomValue(0, hueShiftMax, rng);
                return ImageManipulator.ShiftImageHue(image, hueShiftAmount);
            }

            return image;
        }

        /// <summary>
        /// Crops and seeds are linked together with the LinkingData, so we'll check this to 
        /// get their hue shift values
		/// 
		/// This also creates the seed packet image
        /// </summary>
        /// <param name="image">The image to manipulate</param>
        /// <param name="fileName">The partial location of the image to use to look up the cropId</param>
        /// <returns>The manipulated image</returns>
        private Texture2D ManipulateCropImage(Texture2D image, string fileName, bool isSeedImage)
		{
            if (ImageNameToCropIds.TryGetValue(fileName, out int cropId) &&
                CropIdsToLinkingData.TryGetValue(cropId, out CropImageLinkingData linkingData))
            {
				// Seed images need to be created from the input value still
				// Crops just need their hues shifted
                return isSeedImage 
					? CreateSeedPacketImage(image, linkingData)
					: ImageManipulator.ShiftImageHue(image, linkingData.HueShiftValue);
            }

			Globals.ConsoleError($"SpringObjectBuilder: Could not get linking data when manipulating image: {fileName}");
			return image;
        }

		/// <summary>
		/// Steps for constructing the seed packet image
		/// - Take the seed image and hue shift it by the value in linkingData
		/// - Grab a random SeedPacket image out of the SeedPackets directory
		///   - use the TrellisPackets subdirectory if it's a trellis image
		/// - Multiply the image by a random color
		/// </summary>
		/// <param name="seedImage">The base seed image, to be hue shifted and overlayed onto a seed packet</param>
		/// <param name="linkingData">Used to get the hue value and whether it's a trellis crop</param>
		/// <returns></returns>
		private Texture2D CreateSeedPacketImage(Texture2D seedImage, CropImageLinkingData linkingData)
        {
			seedImage = ImageManipulator.ShiftImageHue(seedImage, linkingData.HueShiftValue);

			string directory = linkingData.SeedItem.CropGrowthInfo.IsTrellisCrop
				? Path.Combine(ImageDirectory, SeedPacketDirectory, TrellisPacketSubDirectory)
				: Path.Combine(ImageDirectory, SeedPacketDirectory);

            var seedPacketFileNames = Directory.GetFiles(directory)
				.Where(x => x.EndsWith(".png"))
				.OrderBy(x => x)
				.ToList();

			string seedPacketFileName = Globals.RNGGetRandomValueFromList(seedPacketFileNames);
			Color seedItemColor = SeasonFunctions.GetRandomColorForSeasons(linkingData.SeedItem.GrowingSeasons);

            using Texture2D seedPacketOriginalImage = Texture2D.FromFile(Game1.graphics.GraphicsDevice, seedPacketFileName);
			using Texture2D seedPacketImage = ImageManipulator.MultiplyImageByColor(seedPacketOriginalImage, seedItemColor);
            return ImageManipulator.OverlayImages(seedPacketImage, seedImage);
        }

        /// <summary>
        /// Whether the settings permit random crop images
        /// </summary>
        /// <returns>True if so, false otherwise</returns>
        public override bool ShouldSaveImage()
		{
			bool randomizeCrops = Globals.Config.Crops.Randomize && Globals.Config.Crops.UseCustomImages;
			bool randomizeFish = Globals.Config.Fish.Randomize;
			return randomizeCrops || randomizeFish;
		}

		/// <summary>
		/// Whether we should actually save the image file, or if the setting is off
		/// This checks whether the image is a crop or a fish and checks the specific setting
		/// </summary>
		/// <param name="point">The point to check at</param>
		/// <returns />
		protected override bool ShouldSaveImage(Point point)
		{
			int itemId = PointsToItemIds[point];
			if (BootRandomizer.BootData.Keys.Any(x => x == itemId))
			{
				return Globals.Config.Boots.Randomize && Globals.Config.Boots.UseCustomImages;
			}

			Item item = ItemList.Items[(ObjectIndexes)itemId];
			if (item.IsCrop || item.IsSeed || item.Id == (int)ObjectIndexes.CherrySapling)
			{
				return Globals.Config.Crops.Randomize && Globals.Config.Crops.UseCustomImages;
			}

			if (item.IsFish)
			{
				return Globals.Config.Fish.Randomize;
			}

			return false;
		}
    }
}
