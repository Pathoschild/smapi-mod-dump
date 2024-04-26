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
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Randomizer
{
	public class ObjectImageBuilder : ImageBuilder
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
		/// Keeps track of mapped overlay data to item ids so that GetRandomFileName can grab the matching ID
		/// </summary>
		private Dictionary<SpriteOverlayData, string> OverlayDataToItemIds;

		/// <summary>
		/// Keeps track of crop ids mapped to image names so that all the crop images can be linked
		/// </summary>
		private readonly Dictionary<string, CropImageLinkingData> CropIdsToLinkingData;

        /// <summary>
        /// A reverse lookup since we have the image name when we need to find the crop id
        /// </summary>
        private readonly Dictionary<string, string> ImageNameToCropIds;

        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="customFolderName">The folder name of the image type being built</param>
        public ObjectImageBuilder(Dictionary<string, CropImageLinkingData> itemIdsToImageNames) : base()
		{
            Rng = RNG.GetFarmRNG(nameof(ObjectImageBuilder));
            CropIdsToLinkingData = itemIdsToImageNames;
			ImageNameToCropIds = new();
            SubDirectory = "Objects";

			SetAllItemMappings();
			OverlayData = OverlayDataToItemIds.Keys.ToList();

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
			OverlayDataToItemIds = new Dictionary<SpriteOverlayData, string>();

			AddPointsToIdsMapping(FishItem.Get(true).Select(x => x.Id).ToList());
			AddPointsToIdsMapping(BootRandomizer.BootData.Select(x => x.Key).ToList(), isForBoots: true);
			AddPointsToIdsMapping(CropItem.Get().Select(x => x.Id).ToList());
			AddPointsToIdsMapping(CropItem.Get().Select(x => x.MatchingSeedItem.Id).ToList());
			AddPointsToIdsMapping(new List<string> { ObjectIndexes.CherrySapling.GetId(), ObjectIndexes.CoffeeBean.GetId() });
		}

		/// <summary>
		/// Adds all items in the list to the dictionary of point mappings
		/// </summary>
		/// <param name="itemIds"></param>
		/// <param name="isForBoots">Boots use int ids and the default texture by default</param>
		/// <returns></returns>
		protected void AddPointsToIdsMapping(List<string> itemIds, bool isForBoots = false)
		{
			foreach (string id in itemIds)
			{
				if (isForBoots)
				{
					if (int.TryParse(id, out int spriteIndex))
					{
						Point point = GetPointFromIndex(spriteIndex, Item.DefaultTexture);
						var overlayData = new SpriteOverlayData(Item.DefaultTexture, point);
						OverlayDataToItemIds[overlayData] = id;
					}
					else
					{
						Globals.ConsoleWarn($"Image builder tried to parse a boot with a non-integer id: {id}");
					}
				}
				else if (ItemList.Items.TryGetValue(id, out Item item))
				{
					Point point = GetPointFromIndex(item.SpriteIndex, item.Texture);
					var overlayData = new SpriteOverlayData(item.Texture, point);
					OverlayDataToItemIds[overlayData] = id;
				}
				else
				{
					Globals.ConsoleWarn($"Image builder tried to grab a non-existant item with id: {id}");
				}
			}
		}

		/// <summary>
		/// Gets the point in the texture file that belongs to the given item index
		/// Items in spring objects currently ALL have an integer id, so just use that
		/// </summary>
		/// <param name="spriteIndex">The item's sprite index</param>
		/// <param name="texture">The item's texture - used to get the items per row</param>
		/// <returns />
		protected Point GetPointFromIndex(int spriteIndex, string texture)
		{
			int itemsPerRow = GetItemsPerRow(texture);
            return new Point(
				spriteIndex % itemsPerRow, 
				spriteIndex / itemsPerRow);
		}

		/// <summary>
		/// Gets a random file name that matches the crop growth image at the given position (fish excluded)
		/// Will remove the name found from the list
		/// </summary>
		/// <param name="overlayData">The overlay data</param>
		/// <returns>The selected file name</returns>
		protected override string GetRandomFileName(SpriteOverlayData overlayData)
		{
			ImageWidthInPx = 16;

			string itemId = OverlayDataToItemIds[overlayData];
			string fileName = "";
			string subDirectory = "";
			if (BootRandomizer.BootData.Keys.Any(x => x == itemId.ToString()))
			{
				fileName = Rng.GetAndRemoveRandomValueFromList(BootImages);

				if (string.IsNullOrEmpty(fileName))
				{
					Globals.ConsoleWarn($"Could not find the boot image for id {itemId}; using default image instead.");
					return null;
				}

				return fileName;
			}

			Item item = ItemList.Items[itemId];

			if (item.ObjectIndex == ObjectIndexes.CherrySapling)
			{
				ImageWidthInPx = 96;
				return Path.Combine(ImageDirectory, $"{FruitTreeSpritesImageName}.png");
			}

			if (item.IsFish)
			{
				fileName = Rng.GetAndRemoveRandomValueFromList(FishImages);

				if (string.IsNullOrEmpty(fileName))
				{
					Globals.ConsoleWarn($"Could not find the fish image for {item.Name}; using default image instead.");
					return null;
				}

				return fileName;
			}

			string cropId = item.Id;
			if (item.IsCrop || item.ObjectIndex == ObjectIndexes.CoffeeBean)
			{
				subDirectory = $"{Path.DirectorySeparatorChar}{CropsDirectory}";
			}

			else if (item.IsSeed)
			{
				SeedItem seedItem = (SeedItem)item;
				cropId = seedItem.CropId;
				subDirectory = $"{Path.DirectorySeparatorChar}{SeedsDirectory}";
			}

			if (item.IsFlower)
			{
				subDirectory = $"{Path.DirectorySeparatorChar}{FlowersDirectory}";

				CropItem cropItem = (CropItem)item;
				if (cropItem.MatchingSeedItem.HasTint)
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
		/// Hue-shift the image to paste onto the spritesheet, if applicable
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

                RNG rng = RNG.GetFarmRNG($"{nameof(ObjectImageBuilder)}.{fileName}");
                int hueShiftAmount = rng.NextIntWithinRange(0, hueShiftMax);
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
            if (ImageNameToCropIds.TryGetValue(fileName, out string cropId) &&
                CropIdsToLinkingData.TryGetValue(cropId, out CropImageLinkingData linkingData))
            {
				// Seed images need to be created from the input value still
				// Crops just need their hues shifted
                return isSeedImage 
					? CreateSeedPacketImage(image, linkingData)
					: ImageManipulator.ShiftImageHue(image, linkingData.HueShiftValue);
            }

			Globals.ConsoleError($"ObjectImageBuilder: Could not get linking data when manipulating image: {fileName}");
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

			string directory = linkingData.SeedItem.IsTrellisCrop
				? Path.Combine(ImageDirectory, SeedPacketDirectory, TrellisPacketSubDirectory)
				: Path.Combine(ImageDirectory, SeedPacketDirectory);

            var seedPacketFileNames = Directory.GetFiles(directory)
				.Where(x => x.EndsWith(".png"))
				.OrderBy(x => x)
				.ToList();

			string seedPacketFileName = Rng.GetRandomValueFromList(seedPacketFileNames);
			Color seedItemColor = SeasonsExtensions.GetRandomColorForSeasons(linkingData.SeedItem.GrowingSeasons, Rng);

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
		/// <param name="overlayData">The overlay data to check</param>
		/// <returns />
		protected override bool ShouldSaveImage(SpriteOverlayData overlayData)
		{
			string itemId = OverlayDataToItemIds[overlayData];
			if (BootRandomizer.BootData.Keys.Any(x => x == itemId.ToString()))
			{
				return Globals.Config.Boots.Randomize && Globals.Config.Boots.UseCustomImages;
			}

			Item item = ItemList.Items[itemId];
			if (item.IsCrop || item.IsSeed || item.ObjectIndex == ObjectIndexes.CherrySapling)
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
