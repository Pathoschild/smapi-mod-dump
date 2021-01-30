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
	public class SpringObjectsImageBuilder : ImageBuilder
	{
		private const string CropsDirectory = "Crops";
		private const string FlowersDirectory = "Flowers";
		private const string SeedsDirectory = "Seeds";
		private const string FishDirectory = "Fish";
		private const string BootsDirectory = "Boots";

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
		private readonly Dictionary<int, string> CropIdsToImageNames;

		/// <summary>
		/// The constructor
		/// </summary>
		/// <param name="customFolderName">The folder name of the image type being built</param>
		public SpringObjectsImageBuilder(Dictionary<int, string> itemIdsToImageNames) : base()
		{
			CropIdsToImageNames = itemIdsToImageNames;

			BaseFileName = "springobjects.png";
			SubDirectory = "SpringObjects";

			SetAllItemMappings();
			PositionsToOverlay = PointsToItemIds.Keys.ToList();

			FishImages = Directory.GetFiles($"{ImageDirectory}/{FishDirectory}")
				.Where(x => x.EndsWith(".png"))
				.OrderBy(x => x).ToList();

			BootImages = Directory.GetFiles($"{ImageDirectory}/{BootsDirectory}")
				.Where(x => x.EndsWith(".png"))
				.OrderBy(x => x).ToList();
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
			AddPointsToIdsMapping(BootData.AllBoots.Select(x => x.Id).ToList());
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
		protected Point GetPointFromId(int id)
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
			if (BootData.AllBoots.Any(x => x.Id == itemId))
			{
				fileName = Globals.RNGGetAndRemoveRandomValueFromList(BootImages);

				if (string.IsNullOrEmpty(fileName))
				{
					Globals.ConsoleWarn($"Could not find the boot image for id {itemId}; using default image instead.");
					return null;
				}

				return fileName;
			}

			Item item = ItemList.Items[itemId];

			if (item.Id == (int)ObjectIndexes.CherrySapling)
			{
				ImageWidthInPx = 96;
				return $"{ImageDirectory}/fruitTreeSprites.png";
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
				subDirectory = "/Crops";
			}

			else if (item.IsSeed)
			{
				SeedItem seedItem = (SeedItem)item;
				cropId = seedItem.CropGrowthInfo.CropId;
				subDirectory = "/Seeds";
			}

			if (item.IsFlower)
			{
				subDirectory = "/Flowers";

				CropItem cropItem = (CropItem)item;
				if (cropItem.MatchingSeedItem.CropGrowthInfo.TintColorInfo.HasTint)
				{
					ImageWidthInPx = 32; // Flower images include the stem and the top if they have tint
				}
			}

			if (!CropIdsToImageNames.TryGetValue(cropId, out fileName))
			{
				Globals.ConsoleWarn($"Could not find the matching image for {item.Name}; using default image instead.");
				return null;
			}

			return $"{ImageDirectory}{subDirectory}/{fileName}";
		}

		/// <summary>
		/// Whether the settings premit random crop images
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
			if (BootData.AllBoots.Any(x => x.Id == itemId))
			{
				return Globals.Config.Boots.Randomize && Globals.Config.Boots.UseCustomImages;
			}

			Item item = ItemList.Items[itemId];
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
