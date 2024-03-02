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
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using SVObject = StardewValley.Object;

namespace Randomizer
{
    public class ItemList
	{
		/// <summary>
		/// Gets all the resources
		/// </summary>
		/// <returns />
		public static List<Item> GetResources()
		{
			return Items.Values.Where(x => x.IsResource).ToList();
		}

		/// <summary>
		/// Gets the name of the item with the given id
		/// </summary>
		/// <param name="id">The item's id</param>
		/// <returns />
		public static string GetItemName(ObjectIndexes id)
		{
			return GetItem(id).Name;
		}

        /// <summary>
        /// Gets all the foragables
        /// </summary>
        /// <returns />
        public static List<Item> GetForagables()
		{
			return Items.Values.Where(x => x.IsForagable).ToList();
		}

		/// <summary>
		/// Gets all the foragables belonging to the given season
		/// </summary>
		/// <param name="season">The season to get the foragables for</param>
		/// <returns />
		public static List<Item> GetForagables(Seasons season)
		{
			List<Item> foragablesInSeason = new();
			switch (season)
			{
				case Seasons.Spring:
					foragablesInSeason = LocationRandomizer.SpringForagables;
					break;
				case Seasons.Summer:
					foragablesInSeason = LocationRandomizer.SummerForagables;
					break;
				case Seasons.Fall:
					foragablesInSeason = LocationRandomizer.FallForagables;
					break;
				case Seasons.Winter:
					foragablesInSeason = LocationRandomizer.WinterForagables;
					break;
			}

			return Items.Values.Where(x => x.IsForagable && foragablesInSeason.Contains(x)).ToList();
		}

		/// <summary>
		/// Gets all the unique woods foragables
		/// </summary>
		/// <returns />
		public static List<Item> GetUniqueWoodsForagables()
		{
			return Items.Values.Where(x => LocationRandomizer.WoodsForagables.Contains(x)).ToList();
		}

		/// <summary>
		/// Gets all the unique beach foragables
		/// </summary>
		/// <returns />
		public static List<Item> GetUniqueBeachForagables()
		{
			return Items.Values.Where(x => LocationRandomizer.BeachForagables.Contains(x)).ToList();
		}

		/// <summary>
		/// Gets all the unique woods foragables
		/// </summary>
		/// <returns />
		public static List<Item> GetUniqueDesertForagables()
		{
			return Items.Values.Where(x => LocationRandomizer.DesertForagables.Contains(x)).ToList();
		}

		/// <summary>
		/// Gets all the geode minerals
		/// </summary>
		/// <returns />
		public static List<Item> GetGeodeMinerals()
		{
			return Items.Values.Where(x => x.IsGeodeMineral).ToList();
		}

		/// <summary>
		/// Gets all items which are giftable to NPCs
		/// Exclude items marked as impossible - mostly for bundle reasons
		/// </summary>
		/// <returns>List&lt;Item&gt; containing all giftable items</returns>
		public static List<Item> GetGiftables()
		{
			return Items.Values.Where(x => 
				x.DifficultyToObtain < ObtainingDifficulties.Impossible &&
				(
					x.IsAnimalProduct || 
					x.IsArtifact || 
					x.IsCooked || 
					x.IsCrabPotItem || 
					x.IsCrop || 
					x.IsFish || 
					x.IsFlower ||
					x.IsForagable || 
					x.IsFruit ||
					x.IsGeodeMineral ||
					x.IsMayonaisse || 
					x.IsMonsterItem || 
					x.IsResource || 
					x.IsSeed ||
					x.IsSmelted ||
					x.IsTrash)
				).ToList();
		}

		/// <summary>
		/// Gets all the seeds
		/// </summary>
		/// <returns />
		public static List<Item> GetSeeds()
		{
			return Items.Values.Where(x => x.IsSeed).ToList();
		}

		/// <summary>
		/// Gets the seed that grows the given crop
		/// </summary>
		/// <param name="crop">The crop</param>
		/// <returns>The seed the grows the crop</returns>
		public static SeedItem GetSeedFromCrop(CropItem crop)
		{
			int seedId = GetSeeds().Cast<SeedItem>()
				.Where(x => x.CropGrowthInfo.CropId == crop.Id)
				.Select(x => x.Id).FirstOrDefault();

			if (seedId == 0)
			{
				Globals.ConsoleError($"No seed can grow {crop.Name}!");
				return null;
			}

			return (SeedItem)Items[(ObjectIndexes)seedId];
		}

		/// <summary>
		/// Gets all the crops
		/// </summary>
		/// <param name="includeFlowers">Whether to include flowers in the results</param>
		/// <returns />
		public static List<Item> GetCrops(bool includeFlowers = false)
		{
			return Items.Values.Where(x =>
				x.IsCrop && (includeFlowers || (!includeFlowers && !x.IsFlower))
			).ToList();
		}

		/// <summary>
		/// Gets crops that grow in the given season
		/// </summary>
		/// <param name="season">The season</param>
		/// <returns></returns>
		public static List<Item> GetCrops(Seasons season)
		{
			List<int> cropidsInSeason = GetSeeds().Cast<SeedItem>().Where(x =>
				x.CropGrowthInfo.GrowingSeasons.Contains(season)).Select(x => x.CropGrowthInfo.CropId).ToList();

			return Items.Values.Where(x => cropidsInSeason.Contains(x.Id)).ToList();
		}

		/// <summary>
		/// Gets all the flowers
		/// </summary>
		/// <returns />
		public static List<Item> GetFlowers()
		{
			return Items.Values.Where(x => x.IsFlower).ToList();
		}

        /// <summary>
        /// Gets all the fruit
        /// </summary>
        /// <returns />
        public static List<Item> GetFruit()
		{
			return Items.Values.Where(x => x.IsFruit).ToList();
		}

		/// <summary>
		/// Gets all the artifacts
		/// </summary>
		/// <returns />
		public static List<Item> GetArtifacts()
		{
			return Items.Values.Where(x => x.IsArtifact).ToList();
		}

		/// <summary>
		/// Gets all the trash
		/// </summary>
		/// <returns />
		public static List<Item> GetTrash()
		{
			return Items.Values.Where(x => x.IsTrash).ToList();
		}

		/// <summary>
		/// Gets all the cooked items
		/// </summary>
		/// <returns />
		public static List<Item> GetCookedItems()
		{
			return Items.Values.Where(x => x.IsCooked).ToList();
		}

		/// <summary>
		/// Gets all the rings
		/// </summary>
		/// <returns />
		public static List<Item> GetRings()
		{
			return Items.Values.Where(x => x.IsRing).ToList();
		}

		/// <summary>
		/// Gets all the animal products
		/// </summary>
		/// <returns />
		public static List<Item> GetAnimalProducts()
		{
			return Items.Values.Where(x => x.IsAnimalProduct).ToList();
		}

		/// <summary>
		/// Splits <paramref name="itemString"/> by <paramref name="separator"/> and returns a List&lt;Item&gt;.
		/// </summary>
		/// <param name="itemString">String of item IDs separated by a single character.</param>
		/// <param name="separator">The character to split <c>itemString</c> by - defaults to space</param>
		/// <returns />
		public static List<Item> GetItemListFromString(string itemString, char separator = ' ')
		{
			List<Item> itemList = new();

			string[] items = itemString.Trim().Split(separator);
			foreach (string item in items)
			{
				int itemId = int.Parse(item);
				// Negative values represent Item Categories, not Items - ignore
				if (itemId > 0)
				{
					// It's okay if the item doesn't exist, just skip it
					if (Items.TryGetValue((ObjectIndexes)itemId, out var retrievedItem)) {
						itemList.Add(retrievedItem);
					} 
				}
			}

			return itemList;
		}

		/// <summary>
		/// Gets all the items below the given difficulty - exclusive
		/// </summary>
		/// <param name="difficulty">The difficulty</param>
		/// <param name="idsToExclude">Any ids to exclude from the results</param>
		/// <returns>The list of items</returns>
		public static List<Item> GetItemsBelowDifficulty(ObtainingDifficulties difficulty, List<int> idsToExclude = null)
		{
			return Items.Values.Where(x => x.DifficultyToObtain < difficulty &&
				(idsToExclude == null || !idsToExclude.Contains(x.Id)))
			.ToList();
		}

		/// <summary>
		/// Gets one random items equal to the given difficulty
		/// </summary>
		/// <param name="difficulty">The difficulty</param>
		/// /// <param name="idsToExclude">Any ids to exclude from the results</param>
		/// <returns>The list of items</returns>
		public static Item GetRandomItemAtDifficulty(ObtainingDifficulties difficulty, int[] idsToExclude = null)
		{
			return Globals.RNGGetRandomValueFromList(
				Items.Values.Where(x =>
					x.DifficultyToObtain == difficulty &&
					(idsToExclude == null || !idsToExclude.Contains(x.Id))).ToList()
				);
		}

        /// <param name="difficulty">See ObtainingDifficulties</param>
        /// <param name="idsToExclude">List of IDs to exclude</param>
        /// <returns>The list of items, not including any in idsToExclude</returns>
        public static List<Item> GetItemsAtDifficulty(ObtainingDifficulties difficulty, List<int> idsToExclude = null)
        {
            return Items.Values.Where(
                    x => x.DifficultyToObtain == difficulty &&
                    (idsToExclude == null || !idsToExclude.Contains(x.Id))
                ).ToList();
        }

        /// <summary>
        /// Gets all items in given craftable category
        /// </summary>
        /// <param name="category">See Enums/CraftableCategories</param>
        /// <param name="idsToExclude">List of IDs to exclude from results</param>
        /// <returns>The list of items in the given category</returns>
        public static List<Item> GetCraftableItems(CraftableCategories category, List<int> idsToExclude = null)
        {
			List<int> excludedIds = idsToExclude ?? new List<int>();
            return Items.Values.Where(
                    x => x.IsCraftable && (x as CraftableItem).Category == category &&
						!excludedIds.Contains(x.Id) &&
						x.Id > 0
                ).ToList();
        }

        /// <summary>
        /// Gets a random resource item
        /// </summary>
        /// <param name="idsToExclude">Any ids to exclude from the results</param>
		/// <param name="rng">The Random object to use - defaults to the global one</param>
        /// <returns>The resource item</returns>
        public static Item GetRandomResourceItem(int[] idsToExclude = null, Random rng = null)
		{
            var rngToUse = rng ?? Globals.RNG;

            return Globals.RNGGetRandomValueFromList(
				Items.Values
					.Where(x => x.IsResource &&
						(idsToExclude == null || !idsToExclude.Contains(x.Id)))
					.ToList(),
                rngToUse
			);
		}

		/// <summary>
		/// Returns the crafting string of the given object index
		/// Intended to only be passed craftable items, or you'll get an error in the console
		/// </summary>
		/// <param name="objectIndex">The object to look up</param>
		/// <returns />
		public static string GetCraftingString(ObjectIndexes objectIndex)
		{
			Item item = GetItem(objectIndex);
			if (item.IsCraftable)
			{
				return ((CraftableItem)item).GetCraftingString();
			}

			Globals.ConsoleError($"Attempted to create a crafting recipe for a non-craftable item - {item.Name}");
			return string.Empty;
		}

		/// <summary>
		/// Gets a random craftable item out of the list
		/// </summary>
		/// <param name="possibleDifficulties">The difficulties that can be in the result</param>
		/// <param name="itemBeingCrafted">The item being crafted</param>
		/// <param name="idsToExclude">Any ids to not include in the results</param>
		/// <param name="onlyResources">Whether to only include resource items</param>
		/// <returns>The selected item</returns>
		public static Item GetRandomCraftableItem(
			List<ObtainingDifficulties> possibleDifficulties,
			Item itemBeingCrafted,
			List<int> idsToExclude = null,
			bool onlyResources = false)
		{
			List<Item> items = Items.Values
				.Where(x =>
					// Disallow the item being required to craft itself
					x.Id != itemBeingCrafted.Id &&

					// Don't allow items to require the items that they're used to obtain
					(itemBeingCrafted.Id != (int)BigCraftableIndexes.Furnace || !x.IsSmelted) &&
					(itemBeingCrafted.Id != (int)BigCraftableIndexes.MayonnaiseMachine || !x.IsMayonaisse) &&
					(itemBeingCrafted.Id != (int)ObjectIndexes.CrabPot || !x.IsCrabPotItem) &&
					(itemBeingCrafted.Id != (int)BigCraftableIndexes.CheesePress || (x.Id != (int)ObjectIndexes.Cheese) && x.Id != (int)ObjectIndexes.GoatCheese) &&
					(itemBeingCrafted.Id != (int)BigCraftableIndexes.BeeHouse || !x.RequiresBeehouse) &&
					(itemBeingCrafted.Id != (int)BigCraftableIndexes.Keg || !x.RequiresKeg) &&
					((itemBeingCrafted.Id != (int)BigCraftableIndexes.LightningRod) || (x.Id != (int)ObjectIndexes.Battery)) &&

					(possibleDifficulties == null || possibleDifficulties.Contains(x.DifficultyToObtain)) &&
					(idsToExclude == null || !idsToExclude.Contains(x.Id)) &&
					(!onlyResources || x.IsResource)
				).ToList();

			return Globals.RNGGetRandomValueFromList(items);
		}

		/// <summary>
		/// Gets a list of random furniture items to sell
		/// </summary>
		/// <param name="rng">The RNG to use - not optional since this is only used with shops</param>
		/// <param name="numberToGet">The number of furniture objects to get</param>
		/// <returns>A list of furniture to sell</returns>
		public static List<ISalable> GetRandomFurnitureToSell(Random rng, int numberToGet, List<int> itemsToExclude = null)
		{
			return GetRandomFurniture(numberToGet, itemsToExclude, rng)
				.Cast<ISalable>()
				.ToList();
        }

        /// <summary>
        /// Gets a list of random furniture items
        /// </summary>
        /// <param name="numberToGet">The number of furniture objects to get</param>
		/// <param name="itemsToExclude">Item ids to not include</param>
        /// <param name="rng">The RNG to use</param>
        /// <returns>A list of furniture to sell</returns>
        public static List<Furniture> GetRandomFurniture(int numberToGet, List<int> itemsToExclude = null, Random rng = null)
        {
            var rngToUse = rng ?? Globals.RNG;

            var allFurnitureIds = Enum.GetValues(typeof(FurnitureIndexes))
                .Cast<int>()
                .Where(id => itemsToExclude == null || !itemsToExclude.Contains(id))
                .ToList();

            return Globals.RNGGetRandomValuesFromList(allFurnitureIds, numberToGet, rngToUse)
                .Select(furnitureId => Furniture.GetFurnitureInstance(furnitureId))
                .ToList();
        }

        /// <summary>
        /// Gets a list of random clothing items to sell
        /// </summary>
        /// <param name="rng">The RNG to use - not optional since this is only used with shops</param>
        /// <param name="numberToGet">The number of clothing objects to get</param>
		/// <param name="itemsToExclude">Item ids to not include</param>
        /// <returns>A list of clothing objects to sell</returns>
        public static List<ISalable> GetRandomClothingToSell(Random rng, int numberToGet, List<int> itemsToExclude = null)
        {
            var allClothingIds = Enum.GetValues(typeof(ClothingIndexes))
                .Cast<int>()
				.Where(id => itemsToExclude == null || !itemsToExclude.Contains(id))
                .ToList();

            return Globals.RNGGetRandomValuesFromList(allClothingIds, numberToGet, rng)
                .Select(clothingId => new Clothing(clothingId))
                .Cast<ISalable>()
                .ToList();
        }

        /// <summary>
        /// Gets a list of random hats to sell
        /// </summary>
        /// <param name="rng">The RNG to use - not optional since this is only used with shops</param>
        /// <param name="numberToGet">The number of hats to get</param>
		/// <param name="itemsToExclude">Item ids to not include</param>
        /// <returns>A list of furniture to sell</returns>
        public static List<ISalable> GetRandomHatsToSell(Random rng, int numberToGet, List<int> itemsToExclude = null)
        {
            var allHatIds = Enum.GetValues(typeof(HatIndexes))
                .Cast<int>()
                .Where(id => itemsToExclude == null || !itemsToExclude.Contains(id))
                .ToList();

            return Globals.RNGGetRandomValuesFromList(allHatIds, numberToGet, rng)
                .Select(clothingId => new Hat(clothingId))
                .Cast<ISalable>()
                .ToList();
        }

        /// <summary>
        /// Gets a list of random big craftables to sell
        /// </summary>
        /// <param name="rng">The RNG to use - not optional since this is only used with shops</param>
        /// <param name="numberToGet">The number of big craftables to get</param>
		/// <param name="itemsToExclude">Item ids to not include</param>
        /// <returns>A list of big craftables to sell</returns>
        public static List<ISalable> GetRandomBigCraftablesToSell(Random rng, int numberToGet, List<int> itemsToExclude = null)
        {
            return GetRandomBigCraftables(numberToGet, itemsToExclude, rng)
				.Cast<ISalable>()
				.ToList();
        }

        /// <summary>
        /// Gets a list of random big craftables
        /// </summary>
        /// <param name="numberToGet">The number of big craftables to get</param>
		/// <param name="itemsToExclude">Item ids to not include</param>
        /// <param name="rng">The RNG to use</param>
        /// <returns>A list of big craftables to sell</returns>
        public static List<SVObject> GetRandomBigCraftables(int numberToGet, List<int> itemsToExclude = null, Random rng = null)
        {
            var rngToUse = rng ?? Globals.RNG;

            var allBigCraftableIds = BigCraftableItems.Keys
                .Cast<int>()
                .Where(id => itemsToExclude == null || !itemsToExclude.Contains(id))
                .ToList();

            return Globals.RNGGetRandomValuesFromList(allBigCraftableIds, numberToGet, rng)
                .Select(bigCraftableId => new SVObject(Vector2.Zero, bigCraftableId))
                .ToList();
        }

        /// <summary>
        /// Adds a random totem type, always costing 500 Qi Coins
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="shopRNG"></param>
        public static Item GetRandomTotem(Random rng = null)
        {
            var rngToUse = rng ?? Globals.RNG;

            var totemList = new List<ObjectIndexes>()
            {
                ObjectIndexes.WarpTotemFarm,
                ObjectIndexes.WarpTotemBeach,
                ObjectIndexes.WarpTotemMountains,
                ObjectIndexes.WarpTotemDesert,
                ObjectIndexes.RainTotem
            };
            var totemId = Globals.RNGGetRandomValueFromList(totemList, rngToUse);
			return Items[totemId];
        }

		public static Dictionary<int, string> OriginalItemList { get; private set; }
        public static Dictionary<ObjectIndexes, Item> Items { get; private set; }
        public static Dictionary<BigCraftableIndexes, Item> BigCraftableItems { get; private set; }

		public static Item GetItem(ObjectIndexes index)
		{
			return Items[index];
		}

        public static Item GetBigCraftableItem(BigCraftableIndexes index)
        {
            return BigCraftableItems[index];
        }

        public static void Initialize()
		{
			OriginalItemList = Globals.ModRef.Helper.GameContent
				.Load<Dictionary<int, string>>("Data/ObjectInformation");
            Items = new Dictionary<ObjectIndexes, Item>
			{ 
				// Craftable items - Impossible by default
				{ ObjectIndexes.WoodFence, new CraftableItem((int)ObjectIndexes.WoodFence, CraftableCategories.EasyAndNeedMany) },
				{ ObjectIndexes.StoneFence, new CraftableItem((int)ObjectIndexes.StoneFence, CraftableCategories.EasyAndNeedMany) },
				{ ObjectIndexes.IronFence, new CraftableItem((int)ObjectIndexes.IronFence, CraftableCategories.ModerateAndNeedMany) },
				{ ObjectIndexes.HardwoodFence, new CraftableItem((int)ObjectIndexes.HardwoodFence, CraftableCategories.ModerateAndNeedMany) },
				{ ObjectIndexes.Gate, new CraftableItem((int)ObjectIndexes.Gate, CraftableCategories.Easy) },
				{ ObjectIndexes.Torch, new CraftableItem((int)ObjectIndexes.Torch, CraftableCategories.Easy) { DifficultyToObtain = ObtainingDifficulties.SmallTimeRequirements } }, // You can find it in the mines
				{ ObjectIndexes.Sprinkler, new CraftableItem((int)ObjectIndexes.Sprinkler, CraftableCategories.ModerateAndNeedMany) },
				{ ObjectIndexes.QualitySprinkler, new CraftableItem((int)ObjectIndexes.QualitySprinkler, CraftableCategories.Moderate) },
				{ ObjectIndexes.IridiumSprinkler, new CraftableItem((int)ObjectIndexes.IridiumSprinkler, CraftableCategories.DifficultAndNeedMany) },
				{ ObjectIndexes.BasicFertilizer, new CraftableItem((int)ObjectIndexes.BasicFertilizer, CraftableCategories.EasyAndNeedMany) },
				{ ObjectIndexes.QualityFertilizer, new CraftableItem((int)ObjectIndexes.QualityFertilizer, CraftableCategories.Easy) },
				{ ObjectIndexes.BasicRetainingSoil, new CraftableItem((int)ObjectIndexes.BasicRetainingSoil, CraftableCategories.EasyAndNeedMany) },
				{ ObjectIndexes.QualityRetainingSoil, new CraftableItem((int)ObjectIndexes.QualityRetainingSoil, CraftableCategories.EasyAndNeedMany) },
				{ ObjectIndexes.SpeedGro, new CraftableItem((int)ObjectIndexes.SpeedGro, CraftableCategories.ModerateAndNeedMany, dataKey: "Speed-Gro") },
				{ ObjectIndexes.DeluxeSpeedGro, new CraftableItem((int)ObjectIndexes.DeluxeSpeedGro, CraftableCategories.ModerateAndNeedMany, dataKey: "Deluxe Speed-Gro") },
				{ ObjectIndexes.CherryBomb, new CraftableItem((int)ObjectIndexes.CherryBomb, CraftableCategories.Easy) },
				{ ObjectIndexes.Bomb, new CraftableItem((int)ObjectIndexes.Bomb, CraftableCategories.Moderate) },
				{ ObjectIndexes.MegaBomb, new CraftableItem((int)ObjectIndexes.MegaBomb, CraftableCategories.Difficult) },
				{ ObjectIndexes.ExplosiveAmmo, new CraftableItem((int)ObjectIndexes.ExplosiveAmmo, CraftableCategories.ModerateAndNeedMany) },
				// Skipping ancient seeds, as it's just meant to get them from the artifact
				{ ObjectIndexes.SpringSeeds, new CraftableItem((int)ObjectIndexes.SpringSeeds, CraftableCategories.Foragables, dataKey: "Wild Seeds (Sp)" ) },
				{ ObjectIndexes.SummerSeeds, new CraftableItem((int)ObjectIndexes.SummerSeeds, CraftableCategories.Foragables, dataKey: "Wild Seeds (Su)" ) },
				{ ObjectIndexes.FallSeeds, new CraftableItem((int)ObjectIndexes.FallSeeds, CraftableCategories.Foragables, dataKey: "Wild Seeds (Fa)" ) },
                { ObjectIndexes.WinterSeeds, new CraftableItem((int)ObjectIndexes.WinterSeeds, CraftableCategories.Foragables, dataKey: "Wild Seeds (Wi)" ) },
				{ ObjectIndexes.WarpTotemFarm, new CraftableItem((int)ObjectIndexes.WarpTotemFarm, CraftableCategories.ModerateAndNeedMany, dataKey: "Warp Totem: Farm") },
				{ ObjectIndexes.WarpTotemMountains, new CraftableItem((int)ObjectIndexes.WarpTotemMountains, CraftableCategories.ModerateAndNeedMany, dataKey: "Warp Totem: Mountains") },
				{ ObjectIndexes.WarpTotemBeach, new CraftableItem((int)ObjectIndexes.WarpTotemBeach, CraftableCategories.ModerateAndNeedMany, dataKey: "Warp Totem: Beach") },
				{ ObjectIndexes.WarpTotemDesert, new CraftableItem((int)ObjectIndexes.WarpTotemDesert, CraftableCategories.ModerateAndNeedMany, dataKey: "Warp Totem: Desert") },
				{ ObjectIndexes.RainTotem, new CraftableItem((int)ObjectIndexes.RainTotem, CraftableCategories.Difficult) },
				{ ObjectIndexes.FieldSnack, new CraftableItem((int)ObjectIndexes.FieldSnack, CraftableCategories.Easy) },
				{ ObjectIndexes.JackOLantern, new CraftableItem((int)ObjectIndexes.JackOLantern, CraftableCategories.DifficultAndNeedMany, dataKey: "Jack-O-Lantern") },
				{ ObjectIndexes.WoodFloor, new CraftableItem((int)ObjectIndexes.WoodFloor, CraftableCategories.EasyAndNeedMany) },
				{ ObjectIndexes.StrawFloor, new CraftableItem((int)ObjectIndexes.StrawFloor, CraftableCategories.EasyAndNeedMany) },
				{ ObjectIndexes.BrickFloor, new CraftableItem((int)ObjectIndexes.BrickFloor, CraftableCategories.EasyAndNeedMany) },
				{ ObjectIndexes.WeatheredFloor, new CraftableItem((int)ObjectIndexes.WeatheredFloor, CraftableCategories.EasyAndNeedMany) },
				{ ObjectIndexes.CrystalFloor, new CraftableItem((int)ObjectIndexes.CrystalFloor, CraftableCategories.ModerateAndNeedMany) },
				{ ObjectIndexes.StoneFloor, new CraftableItem((int)ObjectIndexes.StoneFloor, CraftableCategories.EasyAndNeedMany) },
				{ ObjectIndexes.WoodPath, new CraftableItem((int)ObjectIndexes.WoodPath, CraftableCategories.EasyAndNeedMany) },
				{ ObjectIndexes.GravelPath, new CraftableItem((int)ObjectIndexes.GravelPath, CraftableCategories.EasyAndNeedMany) },
				{ ObjectIndexes.CobblestonePath, new CraftableItem((int)ObjectIndexes.CobblestonePath, CraftableCategories.EasyAndNeedMany) },
				{ ObjectIndexes.SteppingStonePath, new CraftableItem((int)ObjectIndexes.SteppingStonePath, CraftableCategories.EasyAndNeedMany) },
				{ ObjectIndexes.CrystalPath, new CraftableItem((int)ObjectIndexes.CrystalPath, CraftableCategories.ModerateAndNeedMany) },
				{ ObjectIndexes.WildBait, new CraftableItem((int)ObjectIndexes.WildBait, CraftableCategories.Easy) },
				{ ObjectIndexes.Bait, new CraftableItem((int)ObjectIndexes.Bait, CraftableCategories.EasyAndNeedMany) },
				{ ObjectIndexes.Spinner, new CraftableItem((int)ObjectIndexes.Spinner, CraftableCategories.ModerateAndNeedMany) },
				{ ObjectIndexes.Magnet, new CraftableItem((int)ObjectIndexes.Magnet, CraftableCategories.ModerateAndNeedMany) },
				{ ObjectIndexes.TrapBobber, new CraftableItem((int)ObjectIndexes.TrapBobber, CraftableCategories.Moderate) },
				{ ObjectIndexes.CorkBobber, new CraftableItem((int)ObjectIndexes.CorkBobber, CraftableCategories.Moderate) },
				{ ObjectIndexes.DressedSpinner, new CraftableItem((int)ObjectIndexes.DressedSpinner, CraftableCategories.Moderate) },
				{ ObjectIndexes.TreasureHunter, new CraftableItem((int)ObjectIndexes.TreasureHunter, CraftableCategories.Moderate) },
				{ ObjectIndexes.BarbedHook, new CraftableItem((int)ObjectIndexes.BarbedHook, CraftableCategories.Moderate) },
				{ ObjectIndexes.OilOfGarlic, new CraftableItem((int)ObjectIndexes.OilOfGarlic, CraftableCategories.Difficult, dataKey: "Oil Of Garlic") },
				{ ObjectIndexes.LifeElixir, new CraftableItem((int)ObjectIndexes.LifeElixir, CraftableCategories.DifficultAndNeedMany) },
				{ ObjectIndexes.CrabPot, new CraftableItem((int)ObjectIndexes.CrabPot, CraftableCategories.Moderate, overrideBaseLevelLearnedAt: 1) }, // Limit the level you can learn this to prevent it from being learned twice
				{ ObjectIndexes.IridiumBand, new CraftableItem((int)ObjectIndexes.IridiumBand, CraftableCategories.Endgame) { IsRing = true } },
				{ ObjectIndexes.WeddingRing, new CraftableItem((int)ObjectIndexes.WeddingRing, CraftableCategories.Endgame) { IsRing = true } },
				{ ObjectIndexes.RingOfYoba, new CraftableItem((int)ObjectIndexes.RingOfYoba, CraftableCategories.Difficult) { OverrideName = "Ring of Yoba", IsRing = true } },
				{ ObjectIndexes.SturdyRing, new CraftableItem((int)ObjectIndexes.SturdyRing, CraftableCategories.Moderate) { IsRing = true } },
				{ ObjectIndexes.WarriorRing, new CraftableItem((int)ObjectIndexes.WarriorRing, CraftableCategories.Moderate) { IsRing = true } },
				
				// Resources - ObtainingDifficulties.NoRequirements
				{ ObjectIndexes.Wood, new ResourceItem((int)ObjectIndexes.Wood) },
				{ ObjectIndexes.Hardwood, new ResourceItem((int)ObjectIndexes.Hardwood, 1, new Range(1, 15)) { DifficultyToObtain = ObtainingDifficulties.MediumTimeRequirements } },
				{ ObjectIndexes.Stone, new ResourceItem((int)ObjectIndexes.Stone) },
				{ ObjectIndexes.Fiber, new ResourceItem((int)ObjectIndexes.Fiber, 3, new Range(1, 5)) },
				{ ObjectIndexes.Clay, new ResourceItem((int)ObjectIndexes.Clay, 1, new Range(1, 5)) { DifficultyToObtain = ObtainingDifficulties.SmallTimeRequirements } },

				// Items you get as a byproduct of collection resources
				{ ObjectIndexes.Sap, new Item((int)ObjectIndexes.Sap, ObtainingDifficulties.NoRequirements) { ItemsRequiredForRecipe = new Range(1, 15) } },
				{ ObjectIndexes.Acorn, new Item((int)ObjectIndexes.Acorn, ObtainingDifficulties.NoRequirements) { ItemsRequiredForRecipe = new Range(1, 3) } },
				{ ObjectIndexes.MapleSeed, new Item((int)ObjectIndexes.MapleSeed, ObtainingDifficulties.NoRequirements) { ItemsRequiredForRecipe = new Range(1, 3) } },
				{ ObjectIndexes.PineCone, new Item((int)ObjectIndexes.PineCone, ObtainingDifficulties.NoRequirements) { ItemsRequiredForRecipe = new Range(1, 3) } },
				{ ObjectIndexes.MixedSeeds, new Item((int)ObjectIndexes.MixedSeeds, ObtainingDifficulties.NoRequirements) },

				// Tapper items
				{ ObjectIndexes.OakResin, new Item((int)ObjectIndexes.OakResin, ObtainingDifficulties.MediumTimeRequirements) },
				{ ObjectIndexes.MapleSyrup, new Item((int)ObjectIndexes.MapleSyrup, ObtainingDifficulties.MediumTimeRequirements) },
				{ ObjectIndexes.PineTar, new Item((int)ObjectIndexes.PineTar, ObtainingDifficulties.MediumTimeRequirements) },

				// Items you can buy from the shops easily
				{ ObjectIndexes.Hay, new Item((int)ObjectIndexes.Hay, ObtainingDifficulties.NoRequirements) { ItemsRequiredForRecipe = new Range(1, 5) } },
				{ ObjectIndexes.Sugar, new Item((int)ObjectIndexes.Sugar, ObtainingDifficulties.NoRequirements) { ItemsRequiredForRecipe = new Range(1, 5) } },
				{ ObjectIndexes.Oil, new Item((int)ObjectIndexes.Oil, ObtainingDifficulties.NoRequirements) { ItemsRequiredForRecipe = new Range(1, 5) } },
				{ ObjectIndexes.WheatFlour, new Item((int)ObjectIndexes.WheatFlour, ObtainingDifficulties.NoRequirements) { ItemsRequiredForRecipe = new Range(1, 5) } },

				// Misc fishing items
				{ ObjectIndexes.Seaweed, new Item((int)ObjectIndexes.Seaweed, ObtainingDifficulties.NoRequirements) { ItemsRequiredForRecipe = new Range(1, 3) } },
				{ ObjectIndexes.GreenAlgae, new Item((int)ObjectIndexes.GreenAlgae, ObtainingDifficulties.NoRequirements) { ItemsRequiredForRecipe = new Range(1, 3) } },
				{ ObjectIndexes.WhiteAlgae, new Item((int)ObjectIndexes.WhiteAlgae, ObtainingDifficulties.MediumTimeRequirements) { ItemsRequiredForRecipe = new Range(1, 3) } },
				{ ObjectIndexes.LeadBobber, new Item((int)ObjectIndexes.LeadBobber, ObtainingDifficulties.MediumTimeRequirements) { CanStack = false } },
				{ ObjectIndexes.CuriosityLure, new Item((int)ObjectIndexes.CuriosityLure, ObtainingDifficulties.RareItem) { CanStack = false } },

				// Fish - defaults to ObtainingDifficulties.LargeTimeRequirements
				{ ObjectIndexes.AnyFish, new FishItem((int)ObjectIndexes.AnyFish, ObtainingDifficulties.NonCraftingItem) },

				{ ObjectIndexes.Pufferfish, new FishItem((int)ObjectIndexes.Pufferfish) },
				{ ObjectIndexes.Anchovy, new FishItem((int)ObjectIndexes.Anchovy) },
				{ ObjectIndexes.Tuna, new FishItem((int)ObjectIndexes.Tuna) },
				{ ObjectIndexes.Sardine, new FishItem((int)ObjectIndexes.Sardine) },
				{ ObjectIndexes.Bream, new FishItem((int)ObjectIndexes.Bream) },
				{ ObjectIndexes.LargemouthBass, new FishItem((int)ObjectIndexes.LargemouthBass) },
				{ ObjectIndexes.SmallmouthBass, new FishItem((int)ObjectIndexes.SmallmouthBass) },
				{ ObjectIndexes.RainbowTrout, new FishItem((int)ObjectIndexes.RainbowTrout) },
				{ ObjectIndexes.Salmon, new FishItem((int)ObjectIndexes.Salmon) },
				{ ObjectIndexes.Walleye, new FishItem((int)ObjectIndexes.Walleye) },
				{ ObjectIndexes.Perch, new FishItem((int)ObjectIndexes.Perch) },
				{ ObjectIndexes.Carp, new FishItem((int)ObjectIndexes.Carp) },
				{ ObjectIndexes.Catfish, new FishItem((int)ObjectIndexes.Catfish) },
				{ ObjectIndexes.Pike, new FishItem((int)ObjectIndexes.Pike) },
				{ ObjectIndexes.Sunfish, new FishItem((int)ObjectIndexes.Sunfish) },
				{ ObjectIndexes.RedMullet, new FishItem((int)ObjectIndexes.RedMullet) },
				{ ObjectIndexes.Herring, new FishItem((int)ObjectIndexes.Herring) },
				{ ObjectIndexes.Eel, new FishItem((int)ObjectIndexes.Eel) },
				{ ObjectIndexes.Octopus, new FishItem((int)ObjectIndexes.Octopus) },
				{ ObjectIndexes.RedSnapper, new FishItem((int)ObjectIndexes.RedSnapper) },
				{ ObjectIndexes.Squid, new FishItem((int)ObjectIndexes.Squid) },
				{ ObjectIndexes.SeaCucumber, new FishItem((int)ObjectIndexes.SeaCucumber) },
				{ ObjectIndexes.SuperCucumber, new FishItem((int)ObjectIndexes.SuperCucumber) },
				{ ObjectIndexes.Ghostfish, new FishItem((int)ObjectIndexes.Ghostfish) },
				{ ObjectIndexes.Stonefish, new FishItem((int)ObjectIndexes.Stonefish) },
				{ ObjectIndexes.IcePip, new FishItem((int)ObjectIndexes.IcePip) },
				{ ObjectIndexes.LavaEel, new FishItem((int)ObjectIndexes.LavaEel) },
				{ ObjectIndexes.Sandfish, new FishItem((int)ObjectIndexes.Sandfish) },
				{ ObjectIndexes.ScorpionCarp, new FishItem((int)ObjectIndexes.ScorpionCarp) },
				{ ObjectIndexes.Flounder, new FishItem((int)ObjectIndexes.Flounder) },
				{ ObjectIndexes.MidnightCarp, new FishItem((int)ObjectIndexes.MidnightCarp) },
				{ ObjectIndexes.Sturgeon, new FishItem((int)ObjectIndexes.Sturgeon) },
				{ ObjectIndexes.TigerTrout, new FishItem((int)ObjectIndexes.TigerTrout) },
				{ ObjectIndexes.Bullhead, new FishItem((int)ObjectIndexes.Bullhead) },
				{ ObjectIndexes.Tilapia, new FishItem((int)ObjectIndexes.Tilapia) },
				{ ObjectIndexes.Chub, new FishItem((int)ObjectIndexes.Chub) },
				{ ObjectIndexes.Dorado, new FishItem((int)ObjectIndexes.Dorado) },
				{ ObjectIndexes.Albacore, new FishItem((int)ObjectIndexes.Albacore) },
				{ ObjectIndexes.Shad, new FishItem((int)ObjectIndexes.Shad) },
				{ ObjectIndexes.Lingcod, new FishItem((int)ObjectIndexes.Lingcod) },
				{ ObjectIndexes.Halibut, new FishItem((int)ObjectIndexes.Halibut) },
				{ ObjectIndexes.Woodskip, new FishItem((int)ObjectIndexes.Woodskip) },
				{ ObjectIndexes.VoidSalmon, new FishItem((int)ObjectIndexes.VoidSalmon, ObtainingDifficulties.Impossible) },
				{ ObjectIndexes.Slimejack, new FishItem((int)ObjectIndexes.Slimejack, ObtainingDifficulties.Impossible) },
				{ ObjectIndexes.MidnightSquid, new FishItem((int)ObjectIndexes.MidnightSquid, ObtainingDifficulties.RareItem) },
				{ ObjectIndexes.SpookFish, new FishItem((int)ObjectIndexes.SpookFish, ObtainingDifficulties.RareItem) },
				{ ObjectIndexes.Blobfish, new FishItem((int)ObjectIndexes.Blobfish, ObtainingDifficulties.RareItem) },
				{ ObjectIndexes.Crimsonfish, new FishItem((int)ObjectIndexes.Crimsonfish, ObtainingDifficulties.EndgameItem) },
				{ ObjectIndexes.Angler, new FishItem((int)ObjectIndexes.Angler, ObtainingDifficulties.EndgameItem) },
				{ ObjectIndexes.Legend, new FishItem((int)ObjectIndexes.Legend, ObtainingDifficulties.EndgameItem) },
				{ ObjectIndexes.Glacierfish, new FishItem((int)ObjectIndexes.Glacierfish, ObtainingDifficulties.EndgameItem) },
				{ ObjectIndexes.MutantCarp, new FishItem((int)ObjectIndexes.MutantCarp, ObtainingDifficulties.EndgameItem) },

				// Crab pot specific
				{ ObjectIndexes.Lobster, new CrabPotItem((int)ObjectIndexes.Lobster) },
				{ ObjectIndexes.Crab, new CrabPotItem((int)ObjectIndexes.Crab) },
				{ ObjectIndexes.Shrimp, new CrabPotItem((int)ObjectIndexes.Shrimp) },
				{ ObjectIndexes.Crayfish, new CrabPotItem((int)ObjectIndexes.Crayfish) },
				{ ObjectIndexes.Snail, new CrabPotItem((int)ObjectIndexes.Snail) },
				{ ObjectIndexes.Periwinkle, new CrabPotItem((int)ObjectIndexes.Periwinkle) },

				// Items you can find in the mines
				{ ObjectIndexes.CaveCarrot, new Item((int)ObjectIndexes.CaveCarrot, ObtainingDifficulties.SmallTimeRequirements) { ItemsRequiredForRecipe = new Range(1, 3) } },
				{ ObjectIndexes.BugMeat, new MonsterItem((int)ObjectIndexes.BugMeat, ObtainingDifficulties.SmallTimeRequirements) { ItemsRequiredForRecipe = new Range(1, 5) } },
				{ ObjectIndexes.Slime, new MonsterItem((int)ObjectIndexes.Slime, ObtainingDifficulties.SmallTimeRequirements) { ItemsRequiredForRecipe = new Range(1, 10) } },
				{ ObjectIndexes.BatWing, new MonsterItem((int)ObjectIndexes.BatWing, ObtainingDifficulties.MediumTimeRequirements) { ItemsRequiredForRecipe = new Range(1, 5) } },
				{ ObjectIndexes.VoidEssence, new MonsterItem((int)ObjectIndexes.VoidEssence, ObtainingDifficulties.MediumTimeRequirements) { ItemsRequiredForRecipe = new Range(1, 5) } },
				{ ObjectIndexes.SolarEssence, new MonsterItem((int)ObjectIndexes.SolarEssence, ObtainingDifficulties.MediumTimeRequirements) { ItemsRequiredForRecipe = new Range(1, 5) } },
				{ ObjectIndexes.SquidInk, new MonsterItem((int)ObjectIndexes.SquidInk, ObtainingDifficulties.MediumTimeRequirements) { ItemsRequiredForRecipe = new Range(1, 5) } },
				{ ObjectIndexes.BoneFragment, new Item((int)ObjectIndexes.BoneFragment, ObtainingDifficulties.LargeTimeRequirements) },
				{ ObjectIndexes.GreenSlimeEgg, new Item((int)ObjectIndexes.GreenSlimeEgg, ObtainingDifficulties.LargeTimeRequirements) },
				{ ObjectIndexes.BlueSlimeEgg, new Item((int)ObjectIndexes.BlueSlimeEgg, ObtainingDifficulties.LargeTimeRequirements) },
				{ ObjectIndexes.RedSlimeEgg, new Item((int)ObjectIndexes.RedSlimeEgg, ObtainingDifficulties.EndgameItem) },
				{ ObjectIndexes.PurpleSlimeEgg, new Item((int)ObjectIndexes.PurpleSlimeEgg, ObtainingDifficulties.EndgameItem) },

				{ ObjectIndexes.Coal, new Item((int)ObjectIndexes.Coal, ObtainingDifficulties.SmallTimeRequirements) { ItemsRequiredForRecipe = new Range(1, 5) } },
				{ ObjectIndexes.CopperOre, new Item((int)ObjectIndexes.CopperOre, ObtainingDifficulties.SmallTimeRequirements) { ItemsRequiredForRecipe = new Range(1, 5) } },
				{ ObjectIndexes.IronOre, new Item((int)ObjectIndexes.IronOre, ObtainingDifficulties.MediumTimeRequirements) { ItemsRequiredForRecipe = new Range(1, 5) } },
				{ ObjectIndexes.GoldOre, new Item((int)ObjectIndexes.GoldOre, ObtainingDifficulties.MediumTimeRequirements) { ItemsRequiredForRecipe = new Range(1, 5) } },
				{ ObjectIndexes.IridiumOre, new Item((int)ObjectIndexes.IridiumOre, ObtainingDifficulties.EndgameItem) { ItemsRequiredForRecipe = new Range(1, 5) } },

				{ ObjectIndexes.Quartz, new Item((int)ObjectIndexes.Quartz, ObtainingDifficulties.SmallTimeRequirements) { ItemsRequiredForRecipe = new Range(1, 3) } },
				{ ObjectIndexes.FireQuartz, new Item((int)ObjectIndexes.FireQuartz, ObtainingDifficulties.MediumTimeRequirements) { ItemsRequiredForRecipe = new Range(1, 3) } },
				{ ObjectIndexes.EarthCrystal, new Item((int)ObjectIndexes.EarthCrystal, ObtainingDifficulties.SmallTimeRequirements) { ItemsRequiredForRecipe = new Range(1, 3) } },
				{ ObjectIndexes.FrozenTear, new Item((int)ObjectIndexes.FrozenTear, ObtainingDifficulties.MediumTimeRequirements) { ItemsRequiredForRecipe = new Range(1, 3) } },

				{ ObjectIndexes.Geode, new Item((int)ObjectIndexes.Geode, ObtainingDifficulties.SmallTimeRequirements) },
				{ ObjectIndexes.FrozenGeode, new Item((int)ObjectIndexes.FrozenGeode, ObtainingDifficulties.MediumTimeRequirements) { ItemsRequiredForRecipe = new Range(1, 3) } },
				{ ObjectIndexes.MagmaGeode, new Item((int)ObjectIndexes.MagmaGeode, ObtainingDifficulties.MediumTimeRequirements) { ItemsRequiredForRecipe = new Range(1, 2) } },
				{ ObjectIndexes.OmniGeode, new Item((int)ObjectIndexes.OmniGeode, ObtainingDifficulties.MediumTimeRequirements) },

				{ ObjectIndexes.Aquamarine, new Item((int)ObjectIndexes.Aquamarine, ObtainingDifficulties.MediumTimeRequirements) },
				{ ObjectIndexes.Amethyst, new Item((int)ObjectIndexes.Amethyst, ObtainingDifficulties.MediumTimeRequirements) },
				{ ObjectIndexes.Emerald, new Item((int)ObjectIndexes.Emerald, ObtainingDifficulties.MediumTimeRequirements) },
				{ ObjectIndexes.Ruby, new Item((int)ObjectIndexes.Ruby, ObtainingDifficulties.MediumTimeRequirements) },
				{ ObjectIndexes.Topaz, new Item((int)ObjectIndexes.Topaz, ObtainingDifficulties.MediumTimeRequirements) },
				{ ObjectIndexes.Jade, new Item((int)ObjectIndexes.Jade, ObtainingDifficulties.MediumTimeRequirements) },
				{ ObjectIndexes.Diamond, new Item((int)ObjectIndexes.Diamond, ObtainingDifficulties.MediumTimeRequirements) },

				// Geode mineral items - ObtainingDifficulties.LargeTimeRequirements
				{ ObjectIndexes.Alamite, new GeodeMineralItem((int)ObjectIndexes.Alamite) },
				{ ObjectIndexes.Calcite, new GeodeMineralItem((int)ObjectIndexes.Calcite) },
				{ ObjectIndexes.Celestine, new GeodeMineralItem((int)ObjectIndexes.Celestine) },
				{ ObjectIndexes.Granite, new GeodeMineralItem((int)ObjectIndexes.Granite) },
				{ ObjectIndexes.Jagoite, new GeodeMineralItem((int)ObjectIndexes.Jagoite) },
				{ ObjectIndexes.Jamborite, new GeodeMineralItem((int)ObjectIndexes.Jamborite) },
				{ ObjectIndexes.Limestone, new GeodeMineralItem((int)ObjectIndexes.Limestone) },
				{ ObjectIndexes.Malachite, new GeodeMineralItem((int)ObjectIndexes.Malachite) },
				{ ObjectIndexes.Mudstone, new GeodeMineralItem((int)ObjectIndexes.Mudstone) },
				{ ObjectIndexes.Nekoite, new GeodeMineralItem((int)ObjectIndexes.Nekoite) },
				{ ObjectIndexes.Orpiment, new GeodeMineralItem((int)ObjectIndexes.Orpiment) },
				{ ObjectIndexes.PetrifiedSlime, new GeodeMineralItem((int)ObjectIndexes.PetrifiedSlime) },
				{ ObjectIndexes.Sandstone, new GeodeMineralItem((int)ObjectIndexes.Sandstone) },
				{ ObjectIndexes.Slate, new GeodeMineralItem((int)ObjectIndexes.Slate) },
				{ ObjectIndexes.ThunderEgg, new GeodeMineralItem((int)ObjectIndexes.ThunderEgg) },

				{ ObjectIndexes.Aerinite, new GeodeMineralItem((int)ObjectIndexes.Aerinite) },
				{ ObjectIndexes.Esperite, new GeodeMineralItem((int)ObjectIndexes.Esperite) },
				{ ObjectIndexes.FairyStone, new GeodeMineralItem((int)ObjectIndexes.FairyStone) },
				{ ObjectIndexes.Fluorapatite, new GeodeMineralItem((int)ObjectIndexes.Fluorapatite) },
				{ ObjectIndexes.Geminite, new GeodeMineralItem((int)ObjectIndexes.Geminite) },
				{ ObjectIndexes.GhostCrystal, new GeodeMineralItem((int)ObjectIndexes.GhostCrystal) },
				{ ObjectIndexes.Hematite, new GeodeMineralItem((int)ObjectIndexes.Hematite) },
				{ ObjectIndexes.Kyanite, new GeodeMineralItem((int)ObjectIndexes.Kyanite) },
				{ ObjectIndexes.Lunarite, new GeodeMineralItem((int)ObjectIndexes.Lunarite) },
				{ ObjectIndexes.Marble, new GeodeMineralItem((int)ObjectIndexes.Marble) },
				{ ObjectIndexes.OceanStone, new GeodeMineralItem((int)ObjectIndexes.OceanStone) },
				{ ObjectIndexes.Opal, new GeodeMineralItem((int)ObjectIndexes.Opal) },
				{ ObjectIndexes.Pyrite, new GeodeMineralItem((int)ObjectIndexes.Pyrite) },
				{ ObjectIndexes.Soapstone, new GeodeMineralItem((int)ObjectIndexes.Soapstone) },

				{ ObjectIndexes.Baryte, new GeodeMineralItem((int)ObjectIndexes.Baryte) },
				{ ObjectIndexes.Basalt, new GeodeMineralItem((int)ObjectIndexes.Basalt) },
				{ ObjectIndexes.Bixite, new GeodeMineralItem((int)ObjectIndexes.Bixite) },
				{ ObjectIndexes.Dolomite, new GeodeMineralItem((int)ObjectIndexes.Dolomite) },
				{ ObjectIndexes.FireOpal, new GeodeMineralItem((int)ObjectIndexes.FireOpal) },
				{ ObjectIndexes.Helvite, new GeodeMineralItem((int)ObjectIndexes.Helvite) },
				{ ObjectIndexes.Jasper, new GeodeMineralItem((int)ObjectIndexes.Jasper) },
				{ ObjectIndexes.LemonStone, new GeodeMineralItem((int)ObjectIndexes.LemonStone) },
				{ ObjectIndexes.Neptunite, new GeodeMineralItem((int)ObjectIndexes.Neptunite) },
				{ ObjectIndexes.Obsidian, new GeodeMineralItem((int)ObjectIndexes.Obsidian) },
				{ ObjectIndexes.StarShards, new GeodeMineralItem((int)ObjectIndexes.StarShards) },
				{ ObjectIndexes.Tigerseye, new GeodeMineralItem((int)ObjectIndexes.Tigerseye) },

				// Rings - a few of them are craftable
				{ ObjectIndexes.SmallGlowRing, new RingItem((int)ObjectIndexes.SmallGlowRing) },
				{ ObjectIndexes.GlowRing, new RingItem((int)ObjectIndexes.GlowRing) },
				{ ObjectIndexes.SmallMagnetRing, new RingItem((int)ObjectIndexes.SmallMagnetRing) },
				{ ObjectIndexes.MagnetRing, new RingItem((int)ObjectIndexes.MagnetRing) },
				{ ObjectIndexes.SlimeCharmerRing, new RingItem((int)ObjectIndexes.SlimeCharmerRing) },
				{ ObjectIndexes.VampireRing, new RingItem((int)ObjectIndexes.VampireRing) },
				{ ObjectIndexes.SavageRing, new RingItem((int)ObjectIndexes.SavageRing) },
				{ ObjectIndexes.BurglarsRing, new RingItem((int)ObjectIndexes.BurglarsRing) },
				{ ObjectIndexes.AmethystRing, new RingItem((int)ObjectIndexes.AmethystRing) },
				{ ObjectIndexes.TopazRing, new RingItem((int)ObjectIndexes.TopazRing) },
				{ ObjectIndexes.AquamarineRing, new RingItem((int)ObjectIndexes.AquamarineRing) },
				{ ObjectIndexes.JadeRing, new RingItem((int)ObjectIndexes.JadeRing) },
				{ ObjectIndexes.EmeraldRing, new RingItem((int)ObjectIndexes.EmeraldRing) },
				{ ObjectIndexes.RubyRing, new RingItem((int)ObjectIndexes.RubyRing) },

				// Animal items - default is ObtainingDifficulties.MediumTimeRequirements, +1 for each building/large version/cheese press required
				{ ObjectIndexes.Honey, new AnimalItem((int)ObjectIndexes.Honey, ObtainingDifficulties.LargeTimeRequirements) { RequiresBeehouse = true } },
				{ ObjectIndexes.WhiteEgg, new AnimalItem((int)ObjectIndexes.WhiteEgg) },
				{ ObjectIndexes.LargeWhiteEgg, new AnimalItem((int)ObjectIndexes.LargeWhiteEgg, ObtainingDifficulties.LargeTimeRequirements) },
				{ ObjectIndexes.BrownEgg, new AnimalItem((int)ObjectIndexes.BrownEgg) },
				{ ObjectIndexes.LargeBrownEgg, new AnimalItem((int)ObjectIndexes.LargeBrownEgg, ObtainingDifficulties.LargeTimeRequirements) },
				{ ObjectIndexes.VoidEgg, new AnimalItem((int)ObjectIndexes.VoidEgg, ObtainingDifficulties.EndgameItem) },
				{ ObjectIndexes.Milk, new AnimalItem((int)ObjectIndexes.Milk) },
				{ ObjectIndexes.LargeMilk, new AnimalItem((int)ObjectIndexes.LargeMilk, ObtainingDifficulties.LargeTimeRequirements) },
				{ ObjectIndexes.GoatMilk, new AnimalItem((int)ObjectIndexes.GoatMilk, ObtainingDifficulties.LargeTimeRequirements) },
				{ ObjectIndexes.LargeGoatMilk, new AnimalItem((int)ObjectIndexes.LargeGoatMilk, ObtainingDifficulties.EndgameItem) },
				{ ObjectIndexes.DuckEgg, new AnimalItem((int)ObjectIndexes.DuckEgg, ObtainingDifficulties.LargeTimeRequirements) },
				{ ObjectIndexes.DuckFeather, new AnimalItem((int)ObjectIndexes.DuckFeather, ObtainingDifficulties.EndgameItem) },
				{ ObjectIndexes.Wool, new AnimalItem((int)ObjectIndexes.Wool, ObtainingDifficulties.EndgameItem) },
				{ ObjectIndexes.Cloth, new AnimalItem((int)ObjectIndexes.Cloth, ObtainingDifficulties.EndgameItem) },
				{ ObjectIndexes.RabbitsFoot, new AnimalItem((int)ObjectIndexes.RabbitsFoot, ObtainingDifficulties.EndgameItem) },
				{ ObjectIndexes.Truffle, new AnimalItem((int)ObjectIndexes.Truffle, ObtainingDifficulties.EndgameItem) },
				{ ObjectIndexes.TruffleOil, new AnimalItem((int)ObjectIndexes.TruffleOil, ObtainingDifficulties.EndgameItem) { RequiresOilMaker = true } },
				{ ObjectIndexes.Mayonnaise, new AnimalItem((int)ObjectIndexes.Mayonnaise) { IsMayonaisse = true } },
				{ ObjectIndexes.DuckMayonnaise, new AnimalItem((int)ObjectIndexes.DuckMayonnaise, ObtainingDifficulties.LargeTimeRequirements) { IsMayonaisse = true } },
				{ ObjectIndexes.VoidMayonnaise, new AnimalItem((int)ObjectIndexes.VoidMayonnaise, ObtainingDifficulties.EndgameItem) { IsMayonaisse = true } },
				{ ObjectIndexes.Cheese, new AnimalItem((int)ObjectIndexes.Cheese, ObtainingDifficulties.LargeTimeRequirements)},
				{ ObjectIndexes.GoatCheese, new AnimalItem((int) ObjectIndexes.GoatCheese, ObtainingDifficulties.EndgameItem)},

				// Artifacts and rare items
				{ ObjectIndexes.DwarfScrollI, new ArtifactItem((int)ObjectIndexes.DwarfScrollI) },
				{ ObjectIndexes.DwarfScrollII, new ArtifactItem((int)ObjectIndexes.DwarfScrollII) },
				{ ObjectIndexes.DwarfScrollIII, new ArtifactItem((int)ObjectIndexes.DwarfScrollIII) },
				{ ObjectIndexes.DwarfScrollIV, new ArtifactItem((int)ObjectIndexes.DwarfScrollIV) },
				{ ObjectIndexes.ChippedAmphora, new ArtifactItem((int)ObjectIndexes.ChippedAmphora) },
				{ ObjectIndexes.Arrowhead, new ArtifactItem((int)ObjectIndexes.Arrowhead) },
				{ ObjectIndexes.AncientDoll, new ArtifactItem((int)ObjectIndexes.AncientDoll) },
				{ ObjectIndexes.ElvishJewelry, new ArtifactItem((int)ObjectIndexes.ElvishJewelry) },
				{ ObjectIndexes.ChewingStick, new ArtifactItem((int)ObjectIndexes.ChewingStick) },
				{ ObjectIndexes.OrnamentalFan, new ArtifactItem((int)ObjectIndexes.OrnamentalFan) },
				{ ObjectIndexes.AncientSword, new ArtifactItem((int)ObjectIndexes.AncientSword) },
				{ ObjectIndexes.RustySpoon, new ArtifactItem((int)ObjectIndexes.RustySpoon) },
				{ ObjectIndexes.RustySpur, new ArtifactItem((int)ObjectIndexes.RustySpur) },
				{ ObjectIndexes.RustyCog, new ArtifactItem((int)ObjectIndexes.RustyCog) },
				{ ObjectIndexes.ChickenStatue, new ArtifactItem((int)ObjectIndexes.ChickenStatue) },
				{ ObjectIndexes.PrehistoricTool, new ArtifactItem((int)ObjectIndexes.PrehistoricTool) },
				{ ObjectIndexes.DriedStarfish, new ArtifactItem((int)ObjectIndexes.DriedStarfish) },
				{ ObjectIndexes.Anchor, new ArtifactItem((int)ObjectIndexes.Anchor) },
				{ ObjectIndexes.GlassShards, new ArtifactItem((int)ObjectIndexes.GlassShards) },
				{ ObjectIndexes.BoneFlute, new ArtifactItem((int)ObjectIndexes.BoneFlute) },
				{ ObjectIndexes.PrehistoricHandaxe, new ArtifactItem((int)ObjectIndexes.PrehistoricHandaxe) },
				{ ObjectIndexes.DwarvishHelm, new ArtifactItem((int)ObjectIndexes.DwarvishHelm) },
				{ ObjectIndexes.DwarfGadget, new ArtifactItem((int)ObjectIndexes.DwarfGadget) },
				{ ObjectIndexes.AncientDrum, new ArtifactItem((int)ObjectIndexes.AncientDrum) },
				{ ObjectIndexes.PrehistoricScapula, new ArtifactItem((int)ObjectIndexes.PrehistoricScapula) },
				{ ObjectIndexes.PrehistoricTibia, new ArtifactItem((int)ObjectIndexes.PrehistoricTibia) },
				{ ObjectIndexes.PrehistoricSkull, new ArtifactItem((int)ObjectIndexes.PrehistoricSkull) },
				{ ObjectIndexes.SkeletalHand, new ArtifactItem((int)ObjectIndexes.SkeletalHand) },
				{ ObjectIndexes.PrehistoricRib, new ArtifactItem((int)ObjectIndexes.PrehistoricRib) },
				{ ObjectIndexes.PrehistoricVertebra, new ArtifactItem((int)ObjectIndexes.PrehistoricVertebra) },
				{ ObjectIndexes.SkeletalTail, new ArtifactItem((int)ObjectIndexes.SkeletalTail) },
				{ ObjectIndexes.NautilusFossil, new ArtifactItem((int)ObjectIndexes.NautilusFossil) },
				{ ObjectIndexes.AmphibianFossil, new ArtifactItem((int)ObjectIndexes.AmphibianFossil) },
				{ ObjectIndexes.PalmFossil, new ArtifactItem((int)ObjectIndexes.PalmFossil) },
				{ ObjectIndexes.Trilobite, new ArtifactItem((int)ObjectIndexes.Trilobite) },

				{ ObjectIndexes.StrangeDoll1, new Item((int)ObjectIndexes.StrangeDoll1, ObtainingDifficulties.RareItem) },
				{ ObjectIndexes.StrangeDoll2, new Item((int)ObjectIndexes.StrangeDoll2, ObtainingDifficulties.RareItem) },
				{ ObjectIndexes.PrismaticShard, new Item((int)ObjectIndexes.PrismaticShard, ObtainingDifficulties.RareItem) },
				{ ObjectIndexes.DinosaurEgg, new ArtifactItem((int)ObjectIndexes.DinosaurEgg, ObtainingDifficulties.RareItem) },
				{ ObjectIndexes.RareDisc, new ArtifactItem((int)ObjectIndexes.RareDisc, ObtainingDifficulties.RareItem) },
				{ ObjectIndexes.GoldenMask, new ArtifactItem((int)ObjectIndexes.GoldenMask, ObtainingDifficulties.RareItem) },
				{ ObjectIndexes.GoldenRelic, new ArtifactItem((int)ObjectIndexes.GoldenRelic, ObtainingDifficulties.RareItem) },
				{ ObjectIndexes.AncientSeed, new ArtifactItem((int)ObjectIndexes.AncientSeed, ObtainingDifficulties.RareItem) },

				// Items on Ginger Island - not randomizing yet, so marking as impossible
				{ ObjectIndexes.TaroRoot, new Item((int)ObjectIndexes.TaroRoot, ObtainingDifficulties.Impossible) },
				{ ObjectIndexes.TaroTuber, new Item((int)ObjectIndexes.TaroTuber, ObtainingDifficulties.Impossible) },
				{ ObjectIndexes.Pineapple, new Item((int)ObjectIndexes.Pineapple, ObtainingDifficulties.Impossible) },
				{ ObjectIndexes.PineappleSeeds, new Item((int)ObjectIndexes.PineappleSeeds, ObtainingDifficulties.Impossible) },
				{ ObjectIndexes.CinderShard, new Item((int)ObjectIndexes.CinderShard, ObtainingDifficulties.Impossible) },
				{ ObjectIndexes.MagmaCap, new Item((int)ObjectIndexes.MagmaCap, ObtainingDifficulties.Impossible) },
				{ ObjectIndexes.DragonTooth, new Item((int)ObjectIndexes.DragonTooth, ObtainingDifficulties.Impossible) },

				// Misc - those marked as impossible you can only get a limited amount of
				{ ObjectIndexes.Battery, new Item((int)ObjectIndexes.Battery, ObtainingDifficulties.LargeTimeRequirements) },
				{ ObjectIndexes.LuckyPurpleShorts, new Item((int)ObjectIndexes.LuckyPurpleShorts, ObtainingDifficulties.Impossible) },
				{ ObjectIndexes.LostAxe, new Item((int)ObjectIndexes.LostAxe, ObtainingDifficulties.Impossible) },
				{ ObjectIndexes.BerryBasket, new Item((int)ObjectIndexes.BerryBasket, ObtainingDifficulties.Impossible) },
				{ ObjectIndexes.Pearl, new Item((int)ObjectIndexes.Pearl, ObtainingDifficulties.Impossible) },
				{ ObjectIndexes.IridiumMilk, new Item((int)ObjectIndexes.IridiumMilk, ObtainingDifficulties.Impossible) },
				{ ObjectIndexes.DecorativePot, new Item((int)ObjectIndexes.DecorativePot, ObtainingDifficulties.Impossible) { CanStack = false } },
				{ ObjectIndexes.DrumBlock, new Item((int)ObjectIndexes.DrumBlock, ObtainingDifficulties.Impossible) { CanStack = false } },
				{ ObjectIndexes.FluteBlock, new Item((int)ObjectIndexes.FluteBlock, ObtainingDifficulties.Impossible) { CanStack = false } },
				{ ObjectIndexes.TeaSet, new Item((int)ObjectIndexes.TeaSet, ObtainingDifficulties.Impossible) { CanStack = false } },
				{ ObjectIndexes.PurpleMushroom, new Item((int)ObjectIndexes.PurpleMushroom, ObtainingDifficulties.MediumTimeRequirements) },
				{ ObjectIndexes.Mead, new Item((int)ObjectIndexes.Mead, ObtainingDifficulties.LargeTimeRequirements) { RequiresBeehouse = true, RequiresKeg = true } },
				{ ObjectIndexes.PaleAle, new Item((int)ObjectIndexes.PaleAle, ObtainingDifficulties.LargeTimeRequirements) { RequiresKeg = true } },
				{ ObjectIndexes.MermaidsPendant, new Item((int)ObjectIndexes.MermaidsPendant, ObtainingDifficulties.EndgameItem) { OverrideName = "Mermaid's Pendant" } },
				{ ObjectIndexes.TreasureChest, new Item((int)ObjectIndexes.TreasureChest, ObtainingDifficulties.Impossible) },
				{ ObjectIndexes.MuscleRemedy, new Item((int)ObjectIndexes.MuscleRemedy, ObtainingDifficulties.NonCraftingItem) },
				{ ObjectIndexes.EnergyTonic, new Item((int)ObjectIndexes.EnergyTonic, ObtainingDifficulties.NonCraftingItem) },
				{ ObjectIndexes.Stardrop, new Item((int)ObjectIndexes.Stardrop, ObtainingDifficulties.Impossible) },
				{ ObjectIndexes.Bouquet, new Item((int)ObjectIndexes.Bouquet, ObtainingDifficulties.NonCraftingItem) },
				{ ObjectIndexes.Vinegar, new Item((int)ObjectIndexes.Vinegar, ObtainingDifficulties.NonCraftingItem) },
				{ ObjectIndexes.Beer, new Item((int)ObjectIndexes.Beer, ObtainingDifficulties.NonCraftingItem) },
				{ ObjectIndexes.Wine, new Item((int)ObjectIndexes.Wine, ObtainingDifficulties.Impossible) },
				{ ObjectIndexes.Juice, new Item((int)ObjectIndexes.Juice, ObtainingDifficulties.Impossible) },
				{ ObjectIndexes.Jelly, new Item((int)ObjectIndexes.Jelly, ObtainingDifficulties.Impossible) },
				{ ObjectIndexes.Pickles, new Item((int)ObjectIndexes.Pickles, ObtainingDifficulties.Impossible) },
				{ ObjectIndexes.GoldenPumpkin, new Item((int)ObjectIndexes.GoldenPumpkin, ObtainingDifficulties.NonCraftingItem) },
				{ ObjectIndexes.Rice, new Item((int)ObjectIndexes.Rice, ObtainingDifficulties.MediumTimeRequirements) },
				{ ObjectIndexes.Salmonberry, new Item((int)ObjectIndexes.Salmonberry, ObtainingDifficulties.LargeTimeRequirements) },
				{ ObjectIndexes.GrassStarter, new Item((int)ObjectIndexes.GrassStarter, ObtainingDifficulties.NonCraftingItem) },
				{ ObjectIndexes.SpringOnion, new Item((int)ObjectIndexes.SpringOnion, ObtainingDifficulties.NonCraftingItem) },
				{ ObjectIndexes.Coffee, new Item((int)ObjectIndexes.Coffee, ObtainingDifficulties.NonCraftingItem) },

				// All cooking recipes - ObtainingDifficulties.LargeTimeRequirements
				{ ObjectIndexes.FriedEgg, new CookedItem((int)ObjectIndexes.FriedEgg) },
				{ ObjectIndexes.Omelet, new CookedItem((int)ObjectIndexes.Omelet) },
				{ ObjectIndexes.Salad, new CookedItem((int)ObjectIndexes.Salad) },
				{ ObjectIndexes.CheeseCauliflower, new CookedItem((int)ObjectIndexes.CheeseCauliflower, (int)ObjectIndexes.Cauliflower) },
				{ ObjectIndexes.BakedFish, new CookedItem((int)ObjectIndexes.BakedFish) },
				{ ObjectIndexes.ParsnipSoup, new CookedItem((int)ObjectIndexes.ParsnipSoup, (int)ObjectIndexes.Parsnip) },
				{ ObjectIndexes.VegetableMedley, new CookedItem((int)ObjectIndexes.VegetableMedley) },
				{ ObjectIndexes.CompleteBreakfast, new CookedItem((int)ObjectIndexes.CompleteBreakfast) },
				{ ObjectIndexes.FriedCalamari, new CookedItem((int)ObjectIndexes.FriedCalamari) },
				{ ObjectIndexes.StrangeBun, new CookedItem((int)ObjectIndexes.StrangeBun) },

				{ ObjectIndexes.LuckyLunch, new CookedItem((int)ObjectIndexes.LuckyLunch) },
				{ ObjectIndexes.FriedMushroom, new CookedItem((int)ObjectIndexes.FriedMushroom) },
				{ ObjectIndexes.Pizza, new CookedItem((int)ObjectIndexes.Pizza) },
				{ ObjectIndexes.BeanHotpot, new CookedItem((int)ObjectIndexes.BeanHotpot, (int)ObjectIndexes.GreenBean) },
				{ ObjectIndexes.GlazedYams, new CookedItem((int)ObjectIndexes.GlazedYams, (int)ObjectIndexes.Yam) },
				{ ObjectIndexes.CarpSurprise, new CookedItem((int)ObjectIndexes.CarpSurprise, (int)ObjectIndexes.Carp, isFishDish: true) },
				{ ObjectIndexes.Hashbrowns, new CookedItem((int)ObjectIndexes.Hashbrowns) },
				{ ObjectIndexes.Pancakes, new CookedItem((int)ObjectIndexes.Pancakes) },
				{ ObjectIndexes.SalmonDinner, new CookedItem((int)ObjectIndexes.SalmonDinner, (int)ObjectIndexes.Salmon, isFishDish: true) },
				{ ObjectIndexes.FishTaco, new CookedItem((int)ObjectIndexes.FishTaco) },

				{ ObjectIndexes.CrispyBass, new CookedItem((int)ObjectIndexes.CrispyBass, (int)ObjectIndexes.LargemouthBass, isFishDish: true) },
				{ ObjectIndexes.PepperPoppers, new CookedItem((int)ObjectIndexes.PepperPoppers, (int)ObjectIndexes.HotPepper) },
				{ ObjectIndexes.Bread, new CookedItem((int)ObjectIndexes.Bread) },
				{ ObjectIndexes.TomKhaSoup, new CookedItem((int)ObjectIndexes.TomKhaSoup) },
				{ ObjectIndexes.TroutSoup, new CookedItem((int)ObjectIndexes.TroutSoup, (int)ObjectIndexes.RainbowTrout, isFishDish: true) },
				{ ObjectIndexes.ChocolateCake, new CookedItem((int)ObjectIndexes.ChocolateCake) },
				{ ObjectIndexes.PinkCake, new CookedItem((int)ObjectIndexes.PinkCake) },
				{ ObjectIndexes.RhubarbPie, new CookedItem((int)ObjectIndexes.RhubarbPie, (int)ObjectIndexes.Rhubarb) },
				{ ObjectIndexes.Cookie, new CookedItem((int)ObjectIndexes.Cookie) },
				{ ObjectIndexes.Spaghetti, new CookedItem((int)ObjectIndexes.Spaghetti) },

				{ ObjectIndexes.FriedEel, new CookedItem((int)ObjectIndexes.FriedEel, (int)ObjectIndexes.Eel, isFishDish: true) },
				{ ObjectIndexes.SpicyEel, new CookedItem((int)ObjectIndexes.SpicyEel, (int)ObjectIndexes.Eel, isFishDish: true) },
				{ ObjectIndexes.Sashimi, new CookedItem((int)ObjectIndexes.Sashimi) },
				{ ObjectIndexes.MakiRoll, new CookedItem((int)ObjectIndexes.MakiRoll) },
				{ ObjectIndexes.Tortilla, new CookedItem((int)ObjectIndexes.Tortilla) },
				{ ObjectIndexes.RedPlate, new CookedItem((int)ObjectIndexes.RedPlate) },
				{ ObjectIndexes.EggplantParmesan, new CookedItem((int)ObjectIndexes.EggplantParmesan, (int)ObjectIndexes.Eggplant) },
				{ ObjectIndexes.RicePudding, new CookedItem((int)ObjectIndexes.RicePudding, (int)ObjectIndexes.Rice) },
				{ ObjectIndexes.IceCream, new CookedItem((int)ObjectIndexes.IceCream) },
				{ ObjectIndexes.BlueberryTart, new CookedItem((int)ObjectIndexes.BlueberryTart, (int)ObjectIndexes.Blueberry) },

				{ ObjectIndexes.AutumnsBounty, new CookedItem((int)ObjectIndexes.AutumnsBounty) { OverrideName = "Autumn's Bounty" } },
				{ ObjectIndexes.PumpkinSoup, new CookedItem((int)ObjectIndexes.PumpkinSoup, (int)ObjectIndexes.Pumpkin) },
				{ ObjectIndexes.SuperMeal, new CookedItem((int)ObjectIndexes.SuperMeal) },
				{ ObjectIndexes.CranberrySauce, new CookedItem((int)ObjectIndexes.CranberrySauce, (int)ObjectIndexes.Cranberries) },
				{ ObjectIndexes.Stuffing, new CookedItem((int)ObjectIndexes.Stuffing) },
				{ ObjectIndexes.FarmersLunch, new CookedItem((int)ObjectIndexes.FarmersLunch) { OverrideName = "Farmer's Lunch" } },
				{ ObjectIndexes.SurvivalBurger, new CookedItem((int)ObjectIndexes.SurvivalBurger) },
				{ ObjectIndexes.DishOTheSea, new CookedItem((int)ObjectIndexes.DishOTheSea) { OverrideName = "Dish o' The Sea" } },
				{ ObjectIndexes.MinersTreat, new CookedItem((int)ObjectIndexes.MinersTreat) { OverrideName = "Miner's Treat" } },
				{ ObjectIndexes.RootsPlatter, new CookedItem((int)ObjectIndexes.RootsPlatter) },

				{ ObjectIndexes.AlgaeSoup, new CookedItem((int)ObjectIndexes.AlgaeSoup) },
				{ ObjectIndexes.PaleBroth, new CookedItem((int)ObjectIndexes.PaleBroth) },
                { ObjectIndexes.TripleShotEspresso, new CookedItem((int)ObjectIndexes.TripleShotEspresso) },
                { ObjectIndexes.PlumPudding, new CookedItem((int)ObjectIndexes.PlumPudding) },
				{ ObjectIndexes.ArtichokeDip, new CookedItem((int)ObjectIndexes.ArtichokeDip, (int)ObjectIndexes.Artichoke) },
				{ ObjectIndexes.StirFry, new CookedItem((int)ObjectIndexes.StirFry) },
				{ ObjectIndexes.RoastedHazelnuts, new CookedItem((int)ObjectIndexes.RoastedHazelnuts) },
				{ ObjectIndexes.PumpkinPie, new CookedItem((int)ObjectIndexes.PumpkinPie, (int)ObjectIndexes.Pumpkin) },
				{ ObjectIndexes.RadishSalad, new CookedItem((int)ObjectIndexes.RadishSalad, (int)ObjectIndexes.Radish) },
				{ ObjectIndexes.FruitSalad, new CookedItem((int)ObjectIndexes.FruitSalad, ingredientId: null) },
				{ ObjectIndexes.BlackberryCobbler, new CookedItem((int)ObjectIndexes.BlackberryCobbler) },

				{ ObjectIndexes.CranberryCandy, new CookedItem((int)ObjectIndexes.CranberryCandy, (int)ObjectIndexes.Cranberries) },
				{ ObjectIndexes.Bruschetta, new CookedItem((int)ObjectIndexes.Bruschetta) },
				{ ObjectIndexes.Coleslaw, new CookedItem((int)ObjectIndexes.Coleslaw) },
				{ ObjectIndexes.FiddleheadRisotto, new CookedItem((int)ObjectIndexes.FiddleheadRisotto) },
				{ ObjectIndexes.PoppyseedMuffin, new CookedItem((int)ObjectIndexes.PoppyseedMuffin, (int)ObjectIndexes.Poppy) },
				{ ObjectIndexes.Chowder, new CookedItem((int)ObjectIndexes.Chowder) },
				{ ObjectIndexes.LobsterBisque, new CookedItem((int)ObjectIndexes.LobsterBisque) },
				{ ObjectIndexes.Escargot, new CookedItem((int)ObjectIndexes.Escargot) },
				{ ObjectIndexes.FishStew, new CookedItem((int)ObjectIndexes.FishStew) },
				{ ObjectIndexes.MapleBar, new CookedItem((int)ObjectIndexes.MapleBar) },

				{ ObjectIndexes.CrabCakes, new CookedItem((int)ObjectIndexes.CrabCakes) },

				// Internally, this IS a cooked item, but functionally - it actually has no matching recipe, so we won't define it that way
				// You can buy it for 3 prismatic shards, so it is very much an endgame item!
                { ObjectIndexes.MagicRockCandy, new Item((int)ObjectIndexes.MagicRockCandy, ObtainingDifficulties.EndgameItem) },
			
				// ------ All Foragables - ObtainingDifficulties.LargeTimeRequirements -------
				// Spring Foragables
				{ ObjectIndexes.WildHorseradish, new ForagableItem((int)ObjectIndexes.WildHorseradish) },
				{ ObjectIndexes.Daffodil, new ForagableItem((int)ObjectIndexes.Daffodil) },
				{ ObjectIndexes.Leek, new ForagableItem((int)ObjectIndexes.Leek) },
				{ ObjectIndexes.Dandelion, new ForagableItem((int)ObjectIndexes.Dandelion) },
				{ ObjectIndexes.Morel, new ForagableItem((int)ObjectIndexes.Morel) },
				{ ObjectIndexes.CommonMushroom, new ForagableItem((int)ObjectIndexes.CommonMushroom) }, // Also fall

				// Summer Foragables
				{ ObjectIndexes.SpiceBerry, new ForagableItem((int)ObjectIndexes.SpiceBerry) },
				{ ObjectIndexes.Grape, new CropItem((int)ObjectIndexes.Grape, "8/Basic -75") { ShouldBeForagable = true, DifficultyToObtain = ObtainingDifficulties.LargeTimeRequirements, ItemsRequiredForRecipe = new Range(1, 3)} },
				{ ObjectIndexes.SweetPea, new ForagableItem((int)ObjectIndexes.SweetPea) },
				{ ObjectIndexes.RedMushroom, new ForagableItem((int)ObjectIndexes.RedMushroom) }, // Also fall
				{ ObjectIndexes.FiddleheadFern, new ForagableItem((int)ObjectIndexes.FiddleheadFern) },

				// Fall Foragables
				{ ObjectIndexes.WildPlum, new ForagableItem((int)ObjectIndexes.WildPlum) },
				{ ObjectIndexes.Hazelnut, new ForagableItem((int)ObjectIndexes.Hazelnut) },
				{ ObjectIndexes.Blackberry, new ForagableItem((int)ObjectIndexes.Blackberry) },
				{ ObjectIndexes.Chanterelle, new ForagableItem((int)ObjectIndexes.Chanterelle) },

				// Winter Foragables
				{ ObjectIndexes.WinterRoot, new ForagableItem((int)ObjectIndexes.WinterRoot) },
				{ ObjectIndexes.CrystalFruit, new ForagableItem((int)ObjectIndexes.CrystalFruit) },
				{ ObjectIndexes.SnowYam, new ForagableItem((int)ObjectIndexes.SnowYam) },
				{ ObjectIndexes.Crocus, new ForagableItem((int)ObjectIndexes.Crocus) },
				{ ObjectIndexes.Holly, new ForagableItem((int)ObjectIndexes.Holly) },

				// Beach Foragables - the medium ones can also be obtained from crab pots
				{ ObjectIndexes.NautilusShell, new ForagableItem((int)ObjectIndexes.NautilusShell) },
				{ ObjectIndexes.Coral, new ForagableItem((int)ObjectIndexes.Coral) },
				{ ObjectIndexes.SeaUrchin, new ForagableItem((int)ObjectIndexes.SeaUrchin) },
				{ ObjectIndexes.RainbowShell, new ForagableItem((int)ObjectIndexes.RainbowShell) },
				{ ObjectIndexes.Clam, new ForagableItem((int)ObjectIndexes.Clam) { DifficultyToObtain = ObtainingDifficulties.MediumTimeRequirements, IsCrabPotItem = true } },
				{ ObjectIndexes.Cockle, new ForagableItem((int)ObjectIndexes.Cockle) { DifficultyToObtain = ObtainingDifficulties.MediumTimeRequirements, IsCrabPotItem = true } },
				{ ObjectIndexes.Mussel, new ForagableItem((int)ObjectIndexes.Mussel) { DifficultyToObtain = ObtainingDifficulties.MediumTimeRequirements, IsCrabPotItem = true } },
				{ ObjectIndexes.Oyster, new ForagableItem((int)ObjectIndexes.Oyster) { DifficultyToObtain = ObtainingDifficulties.MediumTimeRequirements, IsCrabPotItem = true } },

				// Desert Foragables
				{ ObjectIndexes.Coconut, new ForagableItem((int)ObjectIndexes.Coconut) },
				{ ObjectIndexes.CactusFruit, new CropItem((int)ObjectIndexes.CactusFruit, "30/Basic -79") { ShouldBeForagable = true, DifficultyToObtain = ObtainingDifficulties.LargeTimeRequirements, ItemsRequiredForRecipe = new Range(1, 3)} },

				// Fruit - since trees are randomized, we're making them foragable
				{ ObjectIndexes.Cherry, new ForagableItem((int)ObjectIndexes.Cherry) { IsFruit = true } },
				{ ObjectIndexes.Apricot, new ForagableItem((int)ObjectIndexes.Apricot) { IsFruit = true } },
				{ ObjectIndexes.Orange, new ForagableItem((int)ObjectIndexes.Orange) { IsFruit = true } },
				{ ObjectIndexes.Peach, new ForagableItem((int)ObjectIndexes.Peach) { IsFruit = true } },
				{ ObjectIndexes.Pomegranate, new ForagableItem((int)ObjectIndexes.Pomegranate) { IsFruit = true } },
				{ ObjectIndexes.Apple, new ForagableItem((int)ObjectIndexes.Apple) { IsFruit = true } },
				// ------ End Foragables -------

				// Smelted Items - ObtainingDifficulties.MediumTimeRequirements
				{ ObjectIndexes.RefinedQuartz, new SmeltedItem((int)ObjectIndexes.RefinedQuartz) },
				{ ObjectIndexes.CopperBar, new SmeltedItem((int)ObjectIndexes.CopperBar) },
				{ ObjectIndexes.IronBar, new SmeltedItem((int)ObjectIndexes.IronBar) },
				{ ObjectIndexes.GoldBar, new SmeltedItem((int)ObjectIndexes.GoldBar) },
				{ ObjectIndexes.IridiumBar, new SmeltedItem((int)ObjectIndexes.IridiumBar, ObtainingDifficulties.EndgameItem) },

				// Trash items - ObtainingDifficulties.NoRequirements
				{ ObjectIndexes.BrokenCD, new TrashItem((int)ObjectIndexes.BrokenCD) { OverrideName = "Broken CD" } },
				{ ObjectIndexes.SoggyNewspaper, new TrashItem((int)ObjectIndexes.SoggyNewspaper) },
				{ ObjectIndexes.Driftwood, new TrashItem((int)ObjectIndexes.Driftwood) },
				{ ObjectIndexes.BrokenGlasses, new TrashItem((int)ObjectIndexes.BrokenGlasses) },
				{ ObjectIndexes.JojaCola, new TrashItem((int)ObjectIndexes.JojaCola) },
				{ ObjectIndexes.Trash, new TrashItem((int)ObjectIndexes.Trash) },

				// Fruit trees - ObtainingDifficulties.SmallTimeRequirements
				{ ObjectIndexes.CherrySapling, new Item((int)ObjectIndexes.CherrySapling, ObtainingDifficulties.NonCraftingItem) },
				{ ObjectIndexes.ApricotSapling, new Item((int)ObjectIndexes.ApricotSapling, ObtainingDifficulties.NonCraftingItem) },
				{ ObjectIndexes.OrangeSapling, new Item((int)ObjectIndexes.OrangeSapling, ObtainingDifficulties.NonCraftingItem) },
				{ ObjectIndexes.PeachSapling, new Item((int)ObjectIndexes.PeachSapling, ObtainingDifficulties.NonCraftingItem) },
				{ ObjectIndexes.PomegranateSapling, new Item((int)ObjectIndexes.PomegranateSapling, ObtainingDifficulties.NonCraftingItem) },
				{ ObjectIndexes.AppleSapling, new Item((int)ObjectIndexes.AppleSapling, ObtainingDifficulties.NonCraftingItem) },

				// Seeds - ObtainingDifficulties.LargeTimeRequirements
				{ ObjectIndexes.ParsnipSeeds, new SeedItem((int)ObjectIndexes.ParsnipSeeds, new List<Seasons> { Seasons.Spring }) },
				{ ObjectIndexes.JazzSeeds, new SeedItem((int)ObjectIndexes.JazzSeeds, new List<Seasons> { Seasons.Spring }) },
				{ ObjectIndexes.CauliflowerSeeds, new SeedItem((int)ObjectIndexes.CauliflowerSeeds, new List<Seasons> { Seasons.Spring }) },
				{ ObjectIndexes.CoffeeBean, new SeedItem((int)ObjectIndexes.CoffeeBean, new List<Seasons> { Seasons.Spring, Seasons.Summer }) { Randomize = false, Price = 15 } },
				{ ObjectIndexes.GarlicSeeds, new SeedItem((int)ObjectIndexes.GarlicSeeds, new List<Seasons> { Seasons.Spring }) },
				{ ObjectIndexes.BeanStarter, new SeedItem((int)ObjectIndexes.BeanStarter, new List<Seasons> { Seasons.Spring }) },
				{ ObjectIndexes.KaleSeeds, new SeedItem((int)ObjectIndexes.KaleSeeds, new List<Seasons> { Seasons.Spring }) },
				{ ObjectIndexes.PotatoSeeds, new SeedItem((int)ObjectIndexes.PotatoSeeds, new List<Seasons> { Seasons.Spring }) },
				{ ObjectIndexes.RhubarbSeeds, new SeedItem((int)ObjectIndexes.RhubarbSeeds, new List<Seasons> { Seasons.Spring }) },
				{ ObjectIndexes.StrawberrySeeds, new SeedItem((int)ObjectIndexes.StrawberrySeeds, new List<Seasons> { Seasons.Spring }) },
				{ ObjectIndexes.TulipBulb, new SeedItem((int)ObjectIndexes.TulipBulb, new List<Seasons> { Seasons.Spring }) },
				{ ObjectIndexes.RiceShoot, new SeedItem((int)ObjectIndexes.RiceShoot, new List<Seasons> { Seasons.Spring }) },
				{ ObjectIndexes.BlueberrySeeds, new SeedItem((int)ObjectIndexes.BlueberrySeeds, new List<Seasons> { Seasons.Summer }) },
				{ ObjectIndexes.CornSeeds, new SeedItem((int)ObjectIndexes.CornSeeds, new List<Seasons> { Seasons.Summer,Seasons.Fall }) },
				{ ObjectIndexes.HopsStarter, new SeedItem((int)ObjectIndexes.HopsStarter, new List<Seasons> { Seasons.Summer }) },
				{ ObjectIndexes.PepperSeeds, new SeedItem((int)ObjectIndexes.PepperSeeds, new List<Seasons> { Seasons.Summer }) },
				{ ObjectIndexes.MelonSeeds, new SeedItem((int)ObjectIndexes.MelonSeeds, new List<Seasons> { Seasons.Summer }) },
				{ ObjectIndexes.PoppySeeds, new SeedItem((int)ObjectIndexes.PoppySeeds, new List<Seasons> { Seasons.Summer }) },
				{ ObjectIndexes.RadishSeeds, new SeedItem((int)ObjectIndexes.RadishSeeds, new List<Seasons> { Seasons.Summer }) },
				{ ObjectIndexes.RedCabbageSeeds, new SeedItem((int)ObjectIndexes.RedCabbageSeeds, new List<Seasons> { Seasons.Summer }) },
				{ ObjectIndexes.StarfruitSeeds, new SeedItem((int)ObjectIndexes.StarfruitSeeds, new List<Seasons> { Seasons.Summer }) },
				{ ObjectIndexes.SpangleSeeds, new SeedItem((int)ObjectIndexes.SpangleSeeds, new List<Seasons> { Seasons.Summer }) },
				{ ObjectIndexes.SunflowerSeeds, new SeedItem((int)ObjectIndexes.SunflowerSeeds, new List<Seasons> { Seasons.Summer, Seasons.Fall }) },
				{ ObjectIndexes.TomatoSeeds, new SeedItem((int)ObjectIndexes.TomatoSeeds, new List<Seasons> { Seasons.Summer }) },
				{ ObjectIndexes.WheatSeeds, new SeedItem((int)ObjectIndexes.WheatSeeds, new List<Seasons> { Seasons.Summer, Seasons.Fall }) },
				{ ObjectIndexes.AmaranthSeeds, new SeedItem((int)ObjectIndexes.AmaranthSeeds, new List<Seasons> { Seasons.Fall }) },
				{ ObjectIndexes.ArtichokeSeeds, new SeedItem((int)ObjectIndexes.ArtichokeSeeds, new List<Seasons> { Seasons.Fall }) },
				{ ObjectIndexes.BeetSeeds, new SeedItem((int)ObjectIndexes.BeetSeeds, new List<Seasons> { Seasons.Fall }) },
				{ ObjectIndexes.BokChoySeeds, new SeedItem((int)ObjectIndexes.BokChoySeeds, new List<Seasons> { Seasons.Fall }) },
				{ ObjectIndexes.CranberrySeeds, new SeedItem((int)ObjectIndexes.CranberrySeeds, new List<Seasons> { Seasons.Fall }) },
				{ ObjectIndexes.EggplantSeeds, new SeedItem((int)ObjectIndexes.EggplantSeeds, new List<Seasons> { Seasons.Fall }) },
				{ ObjectIndexes.FairySeeds, new SeedItem((int)ObjectIndexes.FairySeeds, new List<Seasons> { Seasons.Fall }) },
				{ ObjectIndexes.GrapeStarter, new SeedItem((int)ObjectIndexes.GrapeStarter, new List<Seasons> { Seasons.Fall }) },
				{ ObjectIndexes.PumpkinSeeds, new SeedItem((int)ObjectIndexes.PumpkinSeeds, new List<Seasons> { Seasons.Fall }) },
				{ ObjectIndexes.YamSeeds, new SeedItem((int)ObjectIndexes.YamSeeds, new List<Seasons> { Seasons.Fall }) },
				{ ObjectIndexes.AncientSeeds, new SeedItem((int)ObjectIndexes.AncientSeeds, new List<Seasons> { Seasons.Spring, Seasons.Summer, Seasons.Fall }) { Randomize = false } },
				{ ObjectIndexes.CactusSeeds, new SeedItem((int)ObjectIndexes.CactusSeeds, new List<Seasons> { Seasons.Spring, Seasons.Summer, Seasons.Fall, Seasons.Winter }) },
				{ ObjectIndexes.RareSeed, new SeedItem((int)ObjectIndexes.RareSeed, new List<Seasons> { Seasons.Fall }) },

				// Crops - ObtainingDifficulties.LargeTimeRequirements
				{ ObjectIndexes.Parsnip, new CropItem((int)ObjectIndexes.Parsnip, "10/Basic -75") },
				{ ObjectIndexes.BlueJazz, new CropItem((int)ObjectIndexes.BlueJazz, "18/Basic -80") },
				{ ObjectIndexes.Cauliflower, new CropItem((int)ObjectIndexes.Cauliflower, "10/Basic -75") },
				{ ObjectIndexes.Garlic, new CropItem((int)ObjectIndexes.Garlic, "8/Basic -75") },
				{ ObjectIndexes.GreenBean, new CropItem((int)ObjectIndexes.GreenBean, "10/Basic -75") },
				{ ObjectIndexes.Kale, new CropItem((int)ObjectIndexes.Kale, "20/Basic -75") },
				{ ObjectIndexes.Potato, new CropItem((int)ObjectIndexes.Potato, "10/Basic -75") },
				{ ObjectIndexes.Rhubarb, new CropItem((int)ObjectIndexes.Rhubarb, "-300/Basic -79") },
				{ ObjectIndexes.Strawberry, new CropItem((int)ObjectIndexes.Strawberry, "20/Basic -79") },
				{ ObjectIndexes.Tulip, new CropItem((int)ObjectIndexes.Tulip, "18/Basic -80") },
				{ ObjectIndexes.Blueberry, new CropItem((int)ObjectIndexes.Blueberry, "10/Basic -79") },
				{ ObjectIndexes.Corn, new CropItem((int)ObjectIndexes.Corn, "10/Basic -75") },
				{ ObjectIndexes.Hops, new CropItem((int)ObjectIndexes.Hops, "18/Basic -75") },
				{ ObjectIndexes.HotPepper, new CropItem((int)ObjectIndexes.HotPepper, "5/Basic -79") },
				{ ObjectIndexes.Melon, new CropItem((int)ObjectIndexes.Melon, "45/Basic -79") },
				{ ObjectIndexes.Poppy, new CropItem((int)ObjectIndexes.Poppy, "18/Basic -80") },
				{ ObjectIndexes.Radish, new CropItem((int)ObjectIndexes.Radish, "18/Basic -75") },
				{ ObjectIndexes.RedCabbage, new CropItem((int)ObjectIndexes.RedCabbage, "30/Basic -75") },
				{ ObjectIndexes.Starfruit, new CropItem((int)ObjectIndexes.Starfruit, "50/Basic -79") },
				{ ObjectIndexes.SummerSpangle, new CropItem((int)ObjectIndexes.SummerSpangle, "18/Basic -80") },
				{ ObjectIndexes.Sunflower, new CropItem((int)ObjectIndexes.Sunflower, "18/Basic -80") },
				{ ObjectIndexes.Tomato, new CropItem((int)ObjectIndexes.Tomato, "8/Basic -75") },
				{ ObjectIndexes.Wheat, new CropItem((int)ObjectIndexes.Wheat, "-300/Basic -75") },
				{ ObjectIndexes.Amaranth, new CropItem((int)ObjectIndexes.Amaranth, "20/Basic -75") },
				{ ObjectIndexes.Artichoke, new CropItem((int)ObjectIndexes.Artichoke, "12/Basic -75") },
				{ ObjectIndexes.Beet, new CropItem((int)ObjectIndexes.Beet, "Basic -75/Beet") },
				{ ObjectIndexes.BokChoy, new CropItem((int)ObjectIndexes.BokChoy, "10/Basic -75") },
				{ ObjectIndexes.Cranberries, new CropItem((int)ObjectIndexes.Cranberries, "30/Basic -81") },
				{ ObjectIndexes.Eggplant, new CropItem((int)ObjectIndexes.Eggplant, "8/Basic -75") },
				{ ObjectIndexes.FairyRose, new CropItem((int)ObjectIndexes.FairyRose, "18/Basic -80") },
				{ ObjectIndexes.Pumpkin, new CropItem((int)ObjectIndexes.Pumpkin, "-300/Basic -75") },
				{ ObjectIndexes.Yam, new CropItem((int)ObjectIndexes.Yam, "18/Basic -75") },
				{ ObjectIndexes.AncientFruit, new CropItem((int)ObjectIndexes.AncientFruit, "-300/Basic -79") },
				{ ObjectIndexes.SweetGemBerry, new CropItem((int)ObjectIndexes.SweetGemBerry, "-300/Basic -17") },
				{ ObjectIndexes.UnmilledRice, new CropItem((int)ObjectIndexes.UnmilledRice, "1/Basic -75") },
			};
			BigCraftableItems = new Dictionary<BigCraftableIndexes, Item>()
			{
				{ BigCraftableIndexes.Chest, new CraftableItem((int)BigCraftableIndexes.Chest, CraftableCategories.Easy, isBigCraftable: true) },
				{ BigCraftableIndexes.Scarecrow, new CraftableItem((int)BigCraftableIndexes.Scarecrow, CraftableCategories.Moderate, isBigCraftable: true) },
				{ BigCraftableIndexes.BeeHouse, new CraftableItem((int)BigCraftableIndexes.BeeHouse, CraftableCategories.Moderate, isBigCraftable: true) },
				{ BigCraftableIndexes.Keg, new CraftableItem((int)BigCraftableIndexes.Keg, CraftableCategories.Moderate, isBigCraftable: true) },
				{ BigCraftableIndexes.Cask, new CraftableItem((int)BigCraftableIndexes.Cask, CraftableCategories.Moderate, isBigCraftable: true) },
				{ BigCraftableIndexes.Furnace, new CraftableItem((int)BigCraftableIndexes.Furnace, CraftableCategories.Moderate, isBigCraftable: true) },
				{ BigCraftableIndexes.GardenPot, new CraftableItem((int)BigCraftableIndexes.GardenPot, CraftableCategories.Easy, isBigCraftable: true) },
				{ BigCraftableIndexes.WoodSign, new CraftableItem((int)BigCraftableIndexes.WoodSign, CraftableCategories.Easy, isBigCraftable: true) },
				{ BigCraftableIndexes.StoneSign, new CraftableItem((int)BigCraftableIndexes.StoneSign,  CraftableCategories.Easy, isBigCraftable: true) },
				{ BigCraftableIndexes.CheesePress, new CraftableItem((int)BigCraftableIndexes.CheesePress, CraftableCategories.Moderate, isBigCraftable: true) },
				{ BigCraftableIndexes.MayonnaiseMachine, new CraftableItem((int)BigCraftableIndexes.MayonnaiseMachine, CraftableCategories.Moderate, isBigCraftable: true) },
				{ BigCraftableIndexes.SeedMaker, new CraftableItem((int)BigCraftableIndexes.SeedMaker, CraftableCategories.Moderate, isBigCraftable: true) },
				{ BigCraftableIndexes.Loom, new CraftableItem((int)BigCraftableIndexes.Loom, CraftableCategories.Moderate, isBigCraftable: true) },
				{ BigCraftableIndexes.OilMaker, new CraftableItem((int)BigCraftableIndexes.OilMaker, CraftableCategories.Moderate, isBigCraftable: true) },
				{ BigCraftableIndexes.RecyclingMachine, new CraftableItem((int)BigCraftableIndexes.RecyclingMachine, CraftableCategories.Moderate, isBigCraftable: true) },
				{ BigCraftableIndexes.WormBin, new CraftableItem((int)BigCraftableIndexes.WormBin, CraftableCategories.Difficult, isBigCraftable: true) },
				{ BigCraftableIndexes.PreservesJar, new CraftableItem((int)BigCraftableIndexes.PreservesJar, CraftableCategories.Moderate, isBigCraftable: true) },
				{ BigCraftableIndexes.CharcoalKiln, new CraftableItem((int)BigCraftableIndexes.CharcoalKiln, CraftableCategories.Easy, isBigCraftable: true) },
				{ BigCraftableIndexes.Tapper, new CraftableItem((int)BigCraftableIndexes.Tapper, CraftableCategories.Moderate, isBigCraftable: true) },
				{ BigCraftableIndexes.LightningRod, new CraftableItem((int)BigCraftableIndexes.LightningRod, CraftableCategories.Moderate, isBigCraftable: true) },
				{ BigCraftableIndexes.SlimeIncubator, new CraftableItem((int)BigCraftableIndexes.SlimeIncubator, CraftableCategories.Difficult, isBigCraftable: true) },
				{ BigCraftableIndexes.SlimeEggPress, new CraftableItem((int)BigCraftableIndexes.SlimeEggPress, CraftableCategories.DifficultAndNeedMany, isBigCraftable: true, dataKey: "Slime Egg-Press") },
				{ BigCraftableIndexes.Crystalarium, new CraftableItem((int)BigCraftableIndexes.Crystalarium, CraftableCategories.Moderate, isBigCraftable: true) },
				{ BigCraftableIndexes.MiniJukebox, new CraftableItem((int)BigCraftableIndexes.MiniJukebox, CraftableCategories.Moderate, isBigCraftable: true, dataKey: "Mini-Jukebox") },
				{ BigCraftableIndexes.Staircase, new CraftableItem((int)BigCraftableIndexes.Staircase, CraftableCategories.Moderate, isBigCraftable: true) },
				{ BigCraftableIndexes.TubOFlowers, new CraftableItem((int)BigCraftableIndexes.TubOFlowers, CraftableCategories.Easy, isBigCraftable: true, dataKey: "Tub o' Flowers") },
				{ BigCraftableIndexes.WoodenBrazier, new CraftableItem((int)BigCraftableIndexes.WoodenBrazier, CraftableCategories.Easy, isBigCraftable: true) },
				{ BigCraftableIndexes.WickedStatue, new CraftableItem((int)BigCraftableIndexes.WickedStatue, CraftableCategories.Easy, isBigCraftable: true) },
				{ BigCraftableIndexes.StoneBrazier, new CraftableItem((int)BigCraftableIndexes.StoneBrazier, CraftableCategories.Easy, isBigCraftable: true) },
				{ BigCraftableIndexes.GoldBrazier, new CraftableItem((int)BigCraftableIndexes.GoldBrazier,  CraftableCategories.Moderate, isBigCraftable: true) },
				{ BigCraftableIndexes.Campfire, new CraftableItem((int)BigCraftableIndexes.Campfire, CraftableCategories.Easy, isBigCraftable: true) },
				{ BigCraftableIndexes.StumpBrazier, new CraftableItem((int)BigCraftableIndexes.StumpBrazier, CraftableCategories.Moderate, isBigCraftable: true) },
				{ BigCraftableIndexes.CarvedBrazier, new CraftableItem((int)BigCraftableIndexes.CarvedBrazier, CraftableCategories.Moderate, isBigCraftable: true) },
				{ BigCraftableIndexes.SkullBrazier, new CraftableItem((int)BigCraftableIndexes.SkullBrazier, CraftableCategories.Moderate, isBigCraftable: true) },
				{ BigCraftableIndexes.BarrelBrazier, new CraftableItem((int)BigCraftableIndexes.BarrelBrazier, CraftableCategories.Moderate, isBigCraftable: true) },
				{ BigCraftableIndexes.MarbleBrazier, new CraftableItem((int)BigCraftableIndexes.MarbleBrazier, CraftableCategories.Difficult, isBigCraftable: true) },
				{ BigCraftableIndexes.WoodLampPost, new CraftableItem((int) BigCraftableIndexes.WoodLampPost, CraftableCategories.Moderate, isBigCraftable: true, dataKey : "Wood Lamp-post") },
				{ BigCraftableIndexes.IronLampPost, new CraftableItem((int) BigCraftableIndexes.IronLampPost, CraftableCategories.Moderate, isBigCraftable: true, dataKey : "Iron Lamp-post") },

				// Non-craftable BigObjects
				{ BigCraftableIndexes.Heater, new Item((int)BigCraftableIndexes.Heater, ObtainingDifficulties.NonCraftingItem, isBigCraftable: true) },
				{ BigCraftableIndexes.AutoGrabber, new Item((int)BigCraftableIndexes.AutoGrabber, ObtainingDifficulties.NonCraftingItem, isBigCraftable: true) },
				{ BigCraftableIndexes.PrairieKingArcadeSystem, new Item((int)BigCraftableIndexes.PrairieKingArcadeSystem, ObtainingDifficulties.NonCraftingItem, isBigCraftable: true) },
				{ BigCraftableIndexes.JunimoKartArcadeSystem, new Item((int)BigCraftableIndexes.JunimoKartArcadeSystem, ObtainingDifficulties.NonCraftingItem, isBigCraftable: true) },
				{ BigCraftableIndexes.SodaMachine, new Item((int)BigCraftableIndexes.SodaMachine, ObtainingDifficulties.NonCraftingItem, isBigCraftable: true) },
				{ BigCraftableIndexes.HMTGF, new Item((int)BigCraftableIndexes.HMTGF, ObtainingDifficulties.NonCraftingItem, isBigCraftable: true) { OverrideName = "??HMTGF??" } },
				{ BigCraftableIndexes.PinkyLemon, new Item((int)BigCraftableIndexes.PinkyLemon, ObtainingDifficulties.NonCraftingItem, isBigCraftable: true) { OverrideName = "??Pinky Lemon??" } },
				{ BigCraftableIndexes.Foroguemon, new Item((int)BigCraftableIndexes.Foroguemon, ObtainingDifficulties.NonCraftingItem, isBigCraftable: true) { OverrideName = "??Foroguemon??" } },
				{ BigCraftableIndexes.SolidGoldLewis, new Item((int)BigCraftableIndexes.SolidGoldLewis, ObtainingDifficulties.NonCraftingItem, isBigCraftable: true) },
				{ BigCraftableIndexes.StardewHeroTrophy, new Item((int)BigCraftableIndexes.StardewHeroTrophy, ObtainingDifficulties.NonCraftingItem, isBigCraftable: true) }
			};

			// Populate the AvailableLocations/Seasons now that all fish are initialized
			// Afterwards, fill out the default fish info
			FishData.InitializeFishToLocations();
			Items.Values.Where(item => item.Id > 0 && item is FishItem)
				.Cast<FishItem>()
				.ToList()
				.ForEach(fishItem => FishData.FillDefaultFishInfo(fishItem));
        }

        /// <summary>
        /// Gets the value currently in the ObjectInformation asset
        /// </summary>
        /// <param name="dataToGet">The index of the data to retrieve</param>
        /// <returns>The data</returns>
        public static string GetOriginalItemData(int itemId, ObjectInformationIndexes dataToGet)
        {
			if (itemId < 0) { return ""; }
            return OriginalItemList[itemId].Split("/")[(int)dataToGet];
        }
    }
}
