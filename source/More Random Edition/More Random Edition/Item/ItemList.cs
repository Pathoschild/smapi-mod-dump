/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

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
		/// Gets the name of the item with the given index
		/// </summary>
		/// <param name="index">The item's index</param>
		/// <returns />
		public static string GetItemName(ObjectIndexes index)
		{
			return index.GetItem().Name;
		}

        /// <summary>
        /// Gets the display name of the item with the given qualifiedId
		/// Should only be used for display purposes!
		/// - currently only does Object, Furuniture, and BigCraftables, though
        /// </summary>
        /// <param name="index">The qualified id</param>
        /// <returns />
        public static string GetDisplayNameFromQualifiedId(string qualifiedId)
        {
            if (Item.IsQualifiedIdForObject(qualifiedId))
			{
				return GetItemFromStringId(qualifiedId).Name;
			}

			return ItemRegistry.GetData(qualifiedId).DisplayName;
        }

        /// <summary>
        /// Gets the qualified id from the given index
        /// </summary>
        /// <param name="index">The item's index</param>
        /// <returns />
        public static string GetQualifiedId(ObjectIndexes index)
		{
			return index.GetItem().QualifiedId;
		}

        /// <summary>
        /// Gets the item from the given id
        /// Handles qualified ids as well
		/// This is ONLY for Objects, NOT BigCraftables
        /// </summary>
        /// <param name="qualifiedId"></param>
        /// <returns>The integer id</returns>
        public static Item GetItemFromStringId(string givenId)
        {
			if (givenId == null)
			{
				return null;
			}

            string[] tokens = givenId.Split(")");
            string id = tokens.Length > 1
                ? tokens[^1]
                : givenId;

            return Items.ContainsKey(id)
                ? Items[id]
                : null;
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
					foragablesInSeason = ForagableRandomizer.SpringForagables;
					break;
				case Seasons.Summer:
					foragablesInSeason = ForagableRandomizer.SummerForagables;
					break;
				case Seasons.Fall:
					foragablesInSeason = ForagableRandomizer.FallForagables;
					break;
				case Seasons.Winter:
					foragablesInSeason = ForagableRandomizer.WinterForagables;
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
			return Items.Values.Where(x => ForagableRandomizer.WoodsForagables.Contains(x)).ToList();
		}

		/// <summary>
		/// Gets all the unique beach foragables
		/// </summary>
		/// <returns />
		public static List<Item> GetUniqueBeachForagables()
		{
			return Items.Values.Where(x => ForagableRandomizer.BeachForagables.Contains(x)).ToList();
		}

		/// <summary>
		/// Gets all the unique woods foragables
		/// </summary>
		/// <returns />
		public static List<Item> GetUniqueDesertForagables()
		{
			return Items.Values.Where(x => ForagableRandomizer.DesertForagables.Contains(x)).ToList();
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
			string seedId = GetSeeds()
				.Cast<SeedItem>()
				.Where(x => x.CropId == crop.Id)
				.Select(x => x.Id)
				.FirstOrDefault();

			if (string.IsNullOrWhiteSpace(seedId))
			{
				Globals.ConsoleError($"No seed can grow {crop.Name}!");
				return null;
			}

			return (SeedItem)Items[seedId];
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
			List<string> cropIdsInSeason = GetSeeds()
				.Cast<SeedItem>()
				.Where(x => x.GrowingSeasons.Contains(season))
				.Select(x => x.CropId)
				.ToList();

			return Items.Values.Where(x => cropIdsInSeason.Contains(x.Id)).ToList();
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
		/// Splits <paramref name="itemString"/> by <paramref name="separator"/> and returns a List&lt;Item&gt;
		/// Assumes that negative values are item categories and ignores them - used
		/// only by the Preference randomizer for now
		/// </summary>
		/// <param name="itemString">String of item IDs separated by a single character.</param>
		/// <param name="separator">The character to split <c>itemString</c> by - defaults to space</param>
		/// <returns />
		public static List<Item> GetItemListFromString(string itemString, char separator = ' ')
		{
			List<Item> itemList = new();

			string[] itemIds = itemString.Trim().Split(separator);
			foreach (string itemId in itemIds)
			{
				// Negative values represent Item Categories, not Items - ignore
				if (!itemId.StartsWith("-"))
				{
					// It's okay if the item doesn't exist, just skip it
					if (Items.TryGetValue(itemId, out var retrievedItem)) {
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
		public static List<Item> GetItemsBelowDifficulty(ObtainingDifficulties difficulty, List<string> idsToExclude = null)
		{
			return Items.Values.Where(x => x.DifficultyToObtain < difficulty &&
				(idsToExclude == null || !idsToExclude.Contains(x.Id)))
			.ToList();
		}

		/// <summary>
		/// Gets one random items equal to the given difficulty
		/// </summary>
		/// <param name="rng">The rng to use</param>
		/// <param name="difficulty">The difficulty</param>
		/// <param name="idsToExclude">Any ids to exclude from the results</param>
		/// <returns>The list of items</returns>
		public static Item GetRandomItemAtDifficulty(RNG rng, ObtainingDifficulties difficulty, string[] idsToExclude = null)
		{
			return rng.GetRandomValueFromList(
				Items.Values.Where(x =>
					x.DifficultyToObtain == difficulty &&
					(idsToExclude == null || !idsToExclude.Contains(x.Id))).ToList()
				);
		}

        /// <summary>
        /// Gets all items equal to the given difficulty
        /// </summary>
        /// <param name="difficulty">See ObtainingDifficulties</param>
        /// <param name="idsToExclude">List of IDs to exclude</param>
        /// <returns>The list of items, not including any in idsToExclude</returns>
        public static List<Item> GetItemsAtDifficulty(ObtainingDifficulties difficulty, List<string> idsToExclude = null)
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
        public static List<Item> GetCraftableItems(CraftableCategories category, List<string> idsToExclude = null)
        {
			List<string> excludedIds = idsToExclude ?? new List<string>();
            return Items.Values.Where(
                    x => x.IsCraftable && (x as CraftableItem).CraftableCategory == category &&
						!excludedIds.Contains(x.Id)
                ).ToList();
        }

        /// <summary>
        /// Gets a random resource item
        /// </summary>
		/// <param name="rng">The RNG to use</param>
        /// <param name="idsToExclude">Any ids to exclude from the results</param>
        /// <returns>The resource item</returns>
        public static Item GetRandomResourceItem(RNG rng, string[] idsToExclude = null)
		{
            return rng.GetRandomValueFromList(
				Items.Values
					.Where(x => x.IsResource &&
						(idsToExclude == null || !idsToExclude.Contains(x.Id)))
					.ToList());
		}

		/// <summary>
		/// Gets a random craftable item out of the list
		/// </summary>
		/// <param name="rng">The RNG to use</param>
		/// <param name="possibleDifficulties">The difficulties that can be in the result</param>
		/// <param name="itemBeingCrafted">The item being crafted</param>
		/// <param name="idsToExclude">Any ids to not include in the results</param>
		/// <param name="onlyResources">Whether to only include resource items</param>
		/// <returns>The selected item</returns>
		public static Item GetRandomCraftableItem(
			RNG rng,
			List<ObtainingDifficulties> possibleDifficulties,
			Item itemBeingCrafted,
			List<string> idsToExclude = null,
			bool onlyResources = false)
		{
			List<Item> items = Items.Values
				.Where(x =>
					// Disallow the item being required to craft itself
					x.Id != itemBeingCrafted.Id &&

					// Don't allow items to require the items that they're used to obtain
					(itemBeingCrafted.Id != BigCraftableIndexes.Furnace.GetId() || !x.IsSmelted) &&
					(itemBeingCrafted.Id != BigCraftableIndexes.MayonnaiseMachine.GetId() || !x.IsMayonaisse) &&
					(itemBeingCrafted.Id != ObjectIndexes.CrabPot.GetId() || !x.IsCrabPotItem) &&
					(itemBeingCrafted.Id != BigCraftableIndexes.CheesePress.GetId() || (x.Id != ObjectIndexes.Cheese.GetId()) && x.Id != ObjectIndexes.GoatCheese.GetId()) &&
					(itemBeingCrafted.Id != BigCraftableIndexes.BeeHouse.GetId() || !x.RequiresBeehouse) &&
					(itemBeingCrafted.Id != BigCraftableIndexes.Keg.GetId() || !x.RequiresKeg) &&
					((itemBeingCrafted.Id != BigCraftableIndexes.LightningRod.GetId()) || (x.Id != ObjectIndexes.Battery.GetId())) &&
					(itemBeingCrafted.Id != BigCraftableIndexes.Tapper.GetId() || !x.IsTapperItem) &&
					(itemBeingCrafted.Id != ObjectIndexes.MysticTreeSeed.GetId() || x.Id != ObjectIndexes.MysticSyrup.GetId()) &&

                    (possibleDifficulties == null || possibleDifficulties.Contains(x.DifficultyToObtain)) &&
					(idsToExclude == null || !idsToExclude.Contains(x.Id)) &&
					(!onlyResources || x.IsResource)
				).ToList();

			return rng.GetRandomValueFromList(items);
		}

		/// <summary>
		/// Gets a list of random furniture items to sell
		/// </summary>
		/// <param name="rng">The RNG to use</param>
		/// <param name="numberToGet">The number of furniture objects to get</param>
		/// <param name="itemsToExclude">The furniture to exclude</param>
		/// <returns>A list of furniture to sell</returns>
		public static List<ISalable> GetRandomFurnitureToSell(
			RNG rng, 
			int numberToGet, 
			List<FurnitureIndexes> itemsToExclude = null)
		{
			return GetRandomFurniture(rng, numberToGet, itemsToExclude)
				.Cast<ISalable>()
				.ToList();
        }

        /// <summary>
        /// Gets a list of random furniture items
        /// </summary>
        /// <param name="rng">The RNG to use</param>
        /// <param name="numberToGet">The number of furniture objects to get</param>
		/// <param name="itemsToExclude">Furniure indexes to not include</param>
        /// <returns>A list of furniture to sell</returns>
        public static List<Furniture> GetRandomFurniture(
            RNG rng,
            int numberToGet, 
			List<FurnitureIndexes> itemsToExclude = null)
        {
            var allFurnitureIds = Enum.GetValues(typeof(FurnitureIndexes))
                .Cast<FurnitureIndexes>()
                .Where(index => itemsToExclude == null || !itemsToExclude.Contains(index))
                .ToList();

            return rng.GetRandomValuesFromList(allFurnitureIds, numberToGet)
                .Select(furnitureIndex => FurnitureFunctions.GetItem(furnitureIndex))
                .ToList();
        }

        /// <summary>
        /// Gets a list of random clothing items to sell
        /// </summary>
        /// <param name="rng">The RNG to use</param>
        /// <param name="numberToGet">The number of clothing objects to get</param>
		/// <param name="itemsToExclude">Clothing indexes to not include</param>
        /// <returns>A list of clothing objects to sell</returns>
        public static List<ISalable> GetRandomClothingToSell(
			RNG rng, 
			int numberToGet,
			List<ClothingIndexes> itemsToExclude = null)
        {
            var allClothingIds = Enum.GetValues(typeof(ClothingIndexes))
                .Cast<ClothingIndexes>()
				.Where(index => itemsToExclude == null || !itemsToExclude.Contains(index))
                .ToList();

            return rng.GetRandomValuesFromList(allClothingIds, numberToGet)
                .Select(clothingIndex => ClothingFunctions.GetItem(clothingIndex))
                .Cast<ISalable>()
                .ToList();
        }

        /// <summary>
        /// Gets a list of random hats to sell
        /// </summary>
        /// <param name="rng">The RNG to use</param>
        /// <param name="numberToGet">The number of hats to get</param>
		/// <param name="itemsToExclude">Item ids to not include</param>
        /// <returns>A list of furniture to sell</returns>
        public static List<ISalable> GetRandomHatsToSell(RNG rng, int numberToGet, List<string> itemsToExclude = null)
        {
            var allHatIds = Enum.GetValues(typeof(HatIndexes))
				.Cast<HatIndexes>()
                .Select(hat => HatFunctions.GetHatId(hat))
                .Where(id => itemsToExclude == null || !itemsToExclude.Contains(id))
                .ToList();

            return rng.GetRandomValuesFromList(allHatIds, numberToGet)
                .Select(hatId => new Hat(hatId))
                .Cast<ISalable>()
                .ToList();
        }

        /// <summary>
        /// Gets a list of random big craftables to sell
        /// </summary>
        /// <param name="rng">The RNG to use</param>
        /// <param name="numberToGet">The number of big craftables to get</param>
		/// <param name="itemsToExclude">BigCraftable indexes to not include</param>
        /// <returns>A list of big craftables to sell</returns>
        public static List<ISalable> GetRandomBigCraftablesToSell(
			RNG rng, 
			int numberToGet, 
			List<BigCraftableIndexes> itemsToExclude = null)
        {
            return GetRandomBigCraftables(rng, numberToGet, itemsToExclude)
				.Cast<ISalable>()
				.ToList();
        }

        /// <summary>
        /// Gets a list of random big craftables
        /// </summary>
        /// <param name="rng">The RNG to use</param>
        /// <param name="numberToGet">The number of big craftables to get</param>
		/// <param name="indexesToExclude">BigCraftable indexes to not include</param>
        /// <returns>A list of big craftables to sell</returns>
        public static List<SVObject> GetRandomBigCraftables(
			RNG rng,
			int numberToGet, 
			List<BigCraftableIndexes> indexesToExclude = null)
        {
			List<string> idsToExclude = indexesToExclude == null
				? null
				: indexesToExclude
                    .Select(index => index.GetId())
                    .ToList();

            var allBigCraftableIds = BigCraftableItems.Keys
                .Where(id => idsToExclude == null || !idsToExclude.Contains(id))
                .ToList();

            return rng.GetRandomValuesFromList(allBigCraftableIds, numberToGet)
                .Select(bigCraftableId => 
					BigCraftableFunctions.GetItem(
						BigCraftableIndexesExtentions.GetBigCraftableIndex(bigCraftableId)))
                .ToList();
        }

        /// <summary>
        /// Gets a random totem
        /// </summary>
        /// <param name="rng>The RNG to use</param>
        public static Item GetRandomTotem(RNG rng)
        {
            var totemList = new List<ObjectIndexes>()
            {
                ObjectIndexes.WarpTotemFarm,
                ObjectIndexes.WarpTotemBeach,
                ObjectIndexes.WarpTotemMountains,
                ObjectIndexes.WarpTotemDesert,
                ObjectIndexes.RainTotem,
				ObjectIndexes.TreasureTotem
            };
            var totemIndex = rng.GetRandomValueFromList(totemList);
			return totemIndex.GetItem();
        }

        public static Dictionary<string, Item> Items { get; private set; }
        public static Dictionary<string, Item> BigCraftableItems { get; private set; }

        public static void Initialize()
		{
			Items = new Dictionary<string, Item>
			{ 
				// Craftable items - Impossible by default
				{ ObjectIndexes.WoodFence.GetId(), new CraftableItem(ObjectIndexes.WoodFence, CraftableCategories.EasyAndNeedMany) },
				{ ObjectIndexes.StoneFence.GetId(), new CraftableItem(ObjectIndexes.StoneFence, CraftableCategories.EasyAndNeedMany) },
				{ ObjectIndexes.IronFence.GetId(), new CraftableItem(ObjectIndexes.IronFence, CraftableCategories.ModerateAndNeedMany) },
				{ ObjectIndexes.HardwoodFence.GetId(), new CraftableItem(ObjectIndexes.HardwoodFence, CraftableCategories.ModerateAndNeedMany) },
				{ ObjectIndexes.Gate.GetId(), new CraftableItem(ObjectIndexes.Gate, CraftableCategories.Easy) },
				{ ObjectIndexes.Torch.GetId(), new CraftableItem(ObjectIndexes.Torch, CraftableCategories.Easy) { DifficultyToObtain = ObtainingDifficulties.SmallTimeRequirements } }, // You can find it in the mines
				{ ObjectIndexes.GrassStarter.GetId(), new CraftableItem(ObjectIndexes.GrassStarter, CraftableCategories.Easy) },
				{ ObjectIndexes.BlueGrassStarter.GetId(), new CraftableItem(ObjectIndexes.BlueGrassStarter, CraftableCategories.Difficult) },
				{ ObjectIndexes.Sprinkler.GetId(), new CraftableItem(ObjectIndexes.Sprinkler, CraftableCategories.ModerateAndNeedMany) },
				{ ObjectIndexes.QualitySprinkler.GetId(), new CraftableItem(ObjectIndexes.QualitySprinkler, CraftableCategories.Moderate) },
				{ ObjectIndexes.IridiumSprinkler.GetId(), new CraftableItem(ObjectIndexes.IridiumSprinkler, CraftableCategories.DifficultAndNeedMany) },
				{ ObjectIndexes.BasicFertilizer.GetId(), new CraftableItem(ObjectIndexes.BasicFertilizer, CraftableCategories.EasyAndNeedMany) },
				{ ObjectIndexes.QualityFertilizer.GetId(), new CraftableItem(ObjectIndexes.QualityFertilizer, CraftableCategories.ModerateAndNeedMany) },
				{ ObjectIndexes.DeluxeFertilizer.GetId(), new CraftableItem(ObjectIndexes.DeluxeFertilizer, CraftableCategories.DifficultAndNeedMany) },
				{ ObjectIndexes.BasicRetainingSoil.GetId(), new CraftableItem(ObjectIndexes.BasicRetainingSoil, CraftableCategories.EasyAndNeedMany) },
				{ ObjectIndexes.QualityRetainingSoil.GetId(), new CraftableItem(ObjectIndexes.QualityRetainingSoil, CraftableCategories.EasyAndNeedMany) },
				{ ObjectIndexes.DeluxeRetainingSoil.GetId(), new CraftableItem(ObjectIndexes.DeluxeRetainingSoil, CraftableCategories.ModerateAndNeedMany) },
				{ ObjectIndexes.SpeedGro.GetId(), new CraftableItem(ObjectIndexes.SpeedGro, CraftableCategories.ModerateAndNeedMany, dataKey: "Speed-Gro") },
				{ ObjectIndexes.DeluxeSpeedGro.GetId(), new CraftableItem(ObjectIndexes.DeluxeSpeedGro, CraftableCategories.ModerateAndNeedMany, dataKey: "Deluxe Speed-Gro") },
				{ ObjectIndexes.CherryBomb.GetId(), new CraftableItem(ObjectIndexes.CherryBomb, CraftableCategories.Easy) },
				{ ObjectIndexes.Bomb.GetId(), new CraftableItem(ObjectIndexes.Bomb, CraftableCategories.Moderate) },
				{ ObjectIndexes.MegaBomb.GetId(), new CraftableItem(ObjectIndexes.MegaBomb, CraftableCategories.Difficult) },
				{ ObjectIndexes.ExplosiveAmmo.GetId(), new CraftableItem(ObjectIndexes.ExplosiveAmmo, CraftableCategories.ModerateAndNeedMany) },
				// Skipping ancient seeds, as it's just meant to get them from the artifact
				{ ObjectIndexes.SpringSeeds.GetId(), new CraftableItem(ObjectIndexes.SpringSeeds, CraftableCategories.Foragables, dataKey: "Wild Seeds (Sp)" ) },
				{ ObjectIndexes.SummerSeeds.GetId(), new CraftableItem(ObjectIndexes.SummerSeeds, CraftableCategories.Foragables, dataKey: "Wild Seeds (Su)" ) },
				{ ObjectIndexes.FallSeeds.GetId(), new CraftableItem(ObjectIndexes.FallSeeds, CraftableCategories.Foragables, dataKey: "Wild Seeds (Fa)" ) },
				{ ObjectIndexes.WinterSeeds.GetId(), new CraftableItem(ObjectIndexes.WinterSeeds, CraftableCategories.Foragables, dataKey: "Wild Seeds (Wi)" ) },
				{ ObjectIndexes.FiberSeeds.GetId(), new CraftableItem(ObjectIndexes.FiberSeeds, CraftableCategories.ModerateAndNeedMany) },
				{ ObjectIndexes.TeaSapling.GetId(), new CraftableItem(ObjectIndexes.TeaSapling, CraftableCategories.Moderate) },
				{ ObjectIndexes.WarpTotemFarm.GetId(), new CraftableItem(ObjectIndexes.WarpTotemFarm, CraftableCategories.ModerateAndNeedMany, dataKey: "Warp Totem: Farm") },
				{ ObjectIndexes.WarpTotemMountains.GetId(), new CraftableItem(ObjectIndexes.WarpTotemMountains, CraftableCategories.ModerateAndNeedMany, dataKey: "Warp Totem: Mountains") },
				{ ObjectIndexes.WarpTotemBeach.GetId(), new CraftableItem(ObjectIndexes.WarpTotemBeach, CraftableCategories.ModerateAndNeedMany, dataKey: "Warp Totem: Beach") },
				{ ObjectIndexes.WarpTotemDesert.GetId(), new CraftableItem(ObjectIndexes.WarpTotemDesert, CraftableCategories.ModerateAndNeedMany, dataKey: "Warp Totem: Desert") },
				{ ObjectIndexes.RainTotem.GetId(), new CraftableItem(ObjectIndexes.RainTotem, CraftableCategories.Difficult) },
				{ ObjectIndexes.TreasureTotem.GetId(), new CraftableItem(ObjectIndexes.TreasureTotem, CraftableCategories.Difficult) },
				{ ObjectIndexes.FieldSnack.GetId(), new CraftableItem(ObjectIndexes.FieldSnack, CraftableCategories.Easy) },
				{ ObjectIndexes.JackOLantern.GetId(), new CraftableItem(ObjectIndexes.JackOLantern, CraftableCategories.DifficultAndNeedMany, dataKey: "Jack-O-Lantern") },
				{ ObjectIndexes.WoodFloor.GetId(), new CraftableItem(ObjectIndexes.WoodFloor, CraftableCategories.EasyAndNeedMany) },
				{ ObjectIndexes.StrawFloor.GetId(), new CraftableItem(ObjectIndexes.StrawFloor, CraftableCategories.EasyAndNeedMany) },
				{ ObjectIndexes.BrickFloor.GetId(), new CraftableItem(ObjectIndexes.BrickFloor, CraftableCategories.EasyAndNeedMany) },
				{ ObjectIndexes.WeatheredFloor.GetId(), new CraftableItem(ObjectIndexes.WeatheredFloor, CraftableCategories.EasyAndNeedMany) },
				{ ObjectIndexes.CrystalFloor.GetId(), new CraftableItem(ObjectIndexes.CrystalFloor, CraftableCategories.ModerateAndNeedMany) },
				{ ObjectIndexes.StoneFloor.GetId(), new CraftableItem(ObjectIndexes.StoneFloor, CraftableCategories.EasyAndNeedMany) },
				{ ObjectIndexes.RusticPlankFloor.GetId(), new CraftableItem(ObjectIndexes.RusticPlankFloor, CraftableCategories.EasyAndNeedMany) },
				{ ObjectIndexes.StoneWalkwayFloor.GetId(), new CraftableItem(ObjectIndexes.StoneWalkwayFloor, CraftableCategories.EasyAndNeedMany) },
				{ ObjectIndexes.WoodPath.GetId(), new CraftableItem(ObjectIndexes.WoodPath, CraftableCategories.EasyAndNeedMany) },
				{ ObjectIndexes.GravelPath.GetId(), new CraftableItem(ObjectIndexes.GravelPath, CraftableCategories.EasyAndNeedMany) },
				{ ObjectIndexes.CobblestonePath.GetId(), new CraftableItem(ObjectIndexes.CobblestonePath, CraftableCategories.EasyAndNeedMany) },
				{ ObjectIndexes.SteppingStonePath.GetId(), new CraftableItem(ObjectIndexes.SteppingStonePath, CraftableCategories.EasyAndNeedMany) },
				{ ObjectIndexes.CrystalPath.GetId(), new CraftableItem(ObjectIndexes.CrystalPath, CraftableCategories.ModerateAndNeedMany) },
				{ ObjectIndexes.Bait.GetId(), new CraftableItem(ObjectIndexes.Bait, CraftableCategories.EasyAndNeedMany) },
				{ ObjectIndexes.WildBait.GetId(), new CraftableItem(ObjectIndexes.WildBait, CraftableCategories.Easy) },
				{ ObjectIndexes.DeluxeBait .GetId(), new CraftableItem(ObjectIndexes.DeluxeBait , CraftableCategories.ModerateAndNeedMany) },
				{ ObjectIndexes.ChallengeBait .GetId(), new CraftableItem(ObjectIndexes.ChallengeBait , CraftableCategories.ModerateAndNeedMany) },
				{ ObjectIndexes.Spinner.GetId(), new CraftableItem(ObjectIndexes.Spinner, CraftableCategories.ModerateAndNeedMany) },
				{ ObjectIndexes.Magnet.GetId(), new CraftableItem(ObjectIndexes.Magnet, CraftableCategories.ModerateAndNeedMany) },
				{ ObjectIndexes.TrapBobber.GetId(), new CraftableItem(ObjectIndexes.TrapBobber, CraftableCategories.Moderate) },
				{ ObjectIndexes.CorkBobber.GetId(), new CraftableItem(ObjectIndexes.CorkBobber, CraftableCategories.Moderate) },
				{ ObjectIndexes.SonarBobber.GetId(), new CraftableItem(ObjectIndexes.SonarBobber, CraftableCategories.Moderate) },
				{ ObjectIndexes.DressedSpinner.GetId(), new CraftableItem(ObjectIndexes.DressedSpinner, CraftableCategories.Moderate) },
				{ ObjectIndexes.TreasureHunter.GetId(), new CraftableItem(ObjectIndexes.TreasureHunter, CraftableCategories.Moderate) },
				{ ObjectIndexes.BarbedHook.GetId(), new CraftableItem(ObjectIndexes.BarbedHook, CraftableCategories.Moderate) },
				{ ObjectIndexes.OilOfGarlic.GetId(), new CraftableItem(ObjectIndexes.OilOfGarlic, CraftableCategories.Difficult, dataKey: "Oil Of Garlic") },
				{ ObjectIndexes.LifeElixir.GetId(), new CraftableItem(ObjectIndexes.LifeElixir, CraftableCategories.DifficultAndNeedMany) },
				{ ObjectIndexes.CrabPot.GetId(), new CraftableItem(ObjectIndexes.CrabPot, CraftableCategories.Moderate, overrideBaseLevelLearnedAt: 1) }, // Limit the level you can learn this to prevent it from being learned twice
				{ ObjectIndexes.BugSteak.GetId(), new CraftableItem(ObjectIndexes.BugSteak, CraftableCategories.ModerateAndNeedMany) },
				{ ObjectIndexes.IridiumBand.GetId(), new CraftableItem(ObjectIndexes.IridiumBand, CraftableCategories.Endgame) { IsRing = true } },
				{ ObjectIndexes.WeddingRing.GetId(), new CraftableItem(ObjectIndexes.WeddingRing, CraftableCategories.Endgame) { IsRing = true } },
				{ ObjectIndexes.RingOfYoba.GetId(), new CraftableItem(ObjectIndexes.RingOfYoba, CraftableCategories.Difficult) { OverrideName = "Ring of Yoba", IsRing = true } },
				{ ObjectIndexes.SturdyRing.GetId(), new CraftableItem(ObjectIndexes.SturdyRing, CraftableCategories.Moderate) { IsRing = true } },
				{ ObjectIndexes.WarriorRing.GetId(), new CraftableItem(ObjectIndexes.WarriorRing, CraftableCategories.Moderate) { IsRing = true } },
				{ ObjectIndexes.GlowstoneRing.GetId(), new CraftableItem(ObjectIndexes.GlowstoneRing, CraftableCategories.Moderate) { IsRing = true } },
				{ ObjectIndexes.ThornsRing.GetId(), new CraftableItem(ObjectIndexes.ThornsRing, CraftableCategories.Moderate) { IsRing = true } },
				{ ObjectIndexes.CookoutKit.GetId(), new CraftableItem(ObjectIndexes.CookoutKit, CraftableCategories.Moderate) },
				{ ObjectIndexes.TentKit.GetId(), new CraftableItem(ObjectIndexes.TentKit, CraftableCategories.Moderate) },
				{ ObjectIndexes.FairyDust.GetId(), new CraftableItem(ObjectIndexes.FairyDust, CraftableCategories.Difficult) },
				{ ObjectIndexes.DrumBlock.GetId(), new CraftableItem(ObjectIndexes.DrumBlock, CraftableCategories.Moderate) { CanStack = false } },
				{ ObjectIndexes.FluteBlock.GetId(), new CraftableItem(ObjectIndexes.FluteBlock, CraftableCategories.Moderate) { CanStack = false } },
				{ ObjectIndexes.MonsterMusk.GetId(), new CraftableItem(ObjectIndexes.MonsterMusk, CraftableCategories.Moderate) },
				{ ObjectIndexes.MysticTreeSeed.GetId(), new CraftableItem(ObjectIndexes.MysticTreeSeed, CraftableCategories.Moderate) },

				// Resources - ObtainingDifficulties.NoRequirements
				{ ObjectIndexes.Wood.GetId(), new ResourceItem(ObjectIndexes.Wood) },
				{ ObjectIndexes.Hardwood.GetId(), new ResourceItem(ObjectIndexes.Hardwood, 1, new Range(1, 15)) { DifficultyToObtain = ObtainingDifficulties.MediumTimeRequirements } },
				{ ObjectIndexes.Moss.GetId(), new ResourceItem(ObjectIndexes.Moss, 1, new Range(1, 5)) { DifficultyToObtain = ObtainingDifficulties.MediumTimeRequirements } },
				{ ObjectIndexes.Stone.GetId(), new ResourceItem(ObjectIndexes.Stone) },
				{ ObjectIndexes.Fiber.GetId(), new ResourceItem(ObjectIndexes.Fiber, 3, new Range(1, 5)) },
				{ ObjectIndexes.Clay.GetId(), new ResourceItem(ObjectIndexes.Clay, 1, new Range(1, 5)) { DifficultyToObtain = ObtainingDifficulties.SmallTimeRequirements } },

				// Items you get as a byproduct of collection resources
				{ ObjectIndexes.Sap.GetId(), new Item(ObjectIndexes.Sap.GetId(), ObtainingDifficulties.NoRequirements) { ItemsRequiredForRecipe = new Range(1, 15) } },
				{ ObjectIndexes.Acorn.GetId(), new Item(ObjectIndexes.Acorn.GetId(), ObtainingDifficulties.NoRequirements) { ItemsRequiredForRecipe = new Range(1, 3) } },
				{ ObjectIndexes.MapleSeed.GetId(), new Item(ObjectIndexes.MapleSeed.GetId(), ObtainingDifficulties.NoRequirements) { ItemsRequiredForRecipe = new Range(1, 3) } },
				{ ObjectIndexes.PineCone.GetId(), new Item(ObjectIndexes.PineCone.GetId(), ObtainingDifficulties.NoRequirements) { ItemsRequiredForRecipe = new Range(1, 3) } },
				{ ObjectIndexes.MixedSeeds.GetId(), new Item(ObjectIndexes.MixedSeeds.GetId(), ObtainingDifficulties.NoRequirements) },

				// Tapper items
				{ ObjectIndexes.OakResin.GetId(), new Item(ObjectIndexes.OakResin.GetId(), ObtainingDifficulties.MediumTimeRequirements) },
				{ ObjectIndexes.MapleSyrup.GetId(), new Item(ObjectIndexes.MapleSyrup.GetId(), ObtainingDifficulties.MediumTimeRequirements) },
				{ ObjectIndexes.PineTar.GetId(), new Item(ObjectIndexes.PineTar.GetId(), ObtainingDifficulties.MediumTimeRequirements) },
				{ ObjectIndexes.MysticSyrup.GetId(), new Item(ObjectIndexes.MysticSyrup.GetId(), ObtainingDifficulties.EndgameItem) },

				// Items you can buy from the shops easily
				{ ObjectIndexes.Hay.GetId(), new Item(ObjectIndexes.Hay.GetId(), ObtainingDifficulties.NoRequirements) { ItemsRequiredForRecipe = new Range(1, 5) } },
				{ ObjectIndexes.Sugar.GetId(), new Item(ObjectIndexes.Sugar.GetId(), ObtainingDifficulties.NoRequirements) { ItemsRequiredForRecipe = new Range(1, 5) } },
				{ ObjectIndexes.Oil.GetId(), new Item(ObjectIndexes.Oil.GetId(), ObtainingDifficulties.NoRequirements) { ItemsRequiredForRecipe = new Range(1, 5) } },
				{ ObjectIndexes.WheatFlour.GetId(), new Item(ObjectIndexes.WheatFlour.GetId(), ObtainingDifficulties.NoRequirements) { ItemsRequiredForRecipe = new Range(1, 5) } },

				// Misc fishing items
				{ ObjectIndexes.Seaweed.GetId(), new Item(ObjectIndexes.Seaweed.GetId(), ObtainingDifficulties.NoRequirements) { ItemsRequiredForRecipe = new Range(1, 3) } },
				{ ObjectIndexes.GreenAlgae.GetId(), new Item(ObjectIndexes.GreenAlgae.GetId(), ObtainingDifficulties.NoRequirements) { ItemsRequiredForRecipe = new Range(1, 3) } },
				{ ObjectIndexes.WhiteAlgae.GetId(), new Item(ObjectIndexes.WhiteAlgae.GetId(), ObtainingDifficulties.MediumTimeRequirements) { ItemsRequiredForRecipe = new Range(1, 3) } },
				{ ObjectIndexes.SeaJelly.GetId(), new Item(ObjectIndexes.SeaJelly.GetId(), ObtainingDifficulties.MediumTimeRequirements) },
				{ ObjectIndexes.CaveJelly.GetId(), new Item(ObjectIndexes.CaveJelly.GetId(), ObtainingDifficulties.MediumTimeRequirements) },
				{ ObjectIndexes.RiverJelly.GetId(), new Item(ObjectIndexes.RiverJelly.GetId(), ObtainingDifficulties.MediumTimeRequirements) },
				{ ObjectIndexes.LeadBobber.GetId(), new Item(ObjectIndexes.LeadBobber.GetId(), ObtainingDifficulties.MediumTimeRequirements) { CanStack = false } },
				{ ObjectIndexes.CuriosityLure.GetId(), new Item(ObjectIndexes.CuriosityLure.GetId(), ObtainingDifficulties.RareItem) { CanStack = false } },

				// Fish - defaults to ObtainingDifficulties.LargeTimeRequirements
				{ ObjectIndexes.Pufferfish.GetId(), new FishItem(ObjectIndexes.Pufferfish) },
				{ ObjectIndexes.Anchovy.GetId(), new FishItem(ObjectIndexes.Anchovy) },
				{ ObjectIndexes.Tuna.GetId(), new FishItem(ObjectIndexes.Tuna) },
				{ ObjectIndexes.Sardine.GetId(), new FishItem(ObjectIndexes.Sardine) },
				{ ObjectIndexes.Bream.GetId(), new FishItem(ObjectIndexes.Bream) },
				{ ObjectIndexes.LargemouthBass.GetId(), new FishItem(ObjectIndexes.LargemouthBass) },
				{ ObjectIndexes.SmallmouthBass.GetId(), new FishItem(ObjectIndexes.SmallmouthBass) },
				{ ObjectIndexes.RainbowTrout.GetId(), new FishItem(ObjectIndexes.RainbowTrout) },
				{ ObjectIndexes.Salmon.GetId(), new FishItem(ObjectIndexes.Salmon) },
				{ ObjectIndexes.Walleye.GetId(), new FishItem(ObjectIndexes.Walleye) },
				{ ObjectIndexes.Perch.GetId(), new FishItem(ObjectIndexes.Perch) },
				{ ObjectIndexes.Carp.GetId(), new FishItem(ObjectIndexes.Carp) },
				{ ObjectIndexes.Catfish.GetId(), new FishItem(ObjectIndexes.Catfish) },
				{ ObjectIndexes.Pike.GetId(), new FishItem(ObjectIndexes.Pike) },
				{ ObjectIndexes.Sunfish.GetId(), new FishItem(ObjectIndexes.Sunfish) },
				{ ObjectIndexes.RedMullet.GetId(), new FishItem(ObjectIndexes.RedMullet) },
				{ ObjectIndexes.Herring.GetId(), new FishItem(ObjectIndexes.Herring) },
				{ ObjectIndexes.Eel.GetId(), new FishItem(ObjectIndexes.Eel) },
				{ ObjectIndexes.Octopus.GetId(), new FishItem(ObjectIndexes.Octopus) },
				{ ObjectIndexes.RedSnapper.GetId(), new FishItem(ObjectIndexes.RedSnapper) },
				{ ObjectIndexes.Squid.GetId(), new FishItem(ObjectIndexes.Squid) },
				{ ObjectIndexes.SeaCucumber.GetId(), new FishItem(ObjectIndexes.SeaCucumber) },
				{ ObjectIndexes.SuperCucumber.GetId(), new FishItem(ObjectIndexes.SuperCucumber) },
				{ ObjectIndexes.Ghostfish.GetId(), new FishItem(ObjectIndexes.Ghostfish) },
				{ ObjectIndexes.Stonefish.GetId(), new FishItem(ObjectIndexes.Stonefish) },
				{ ObjectIndexes.IcePip.GetId(), new FishItem(ObjectIndexes.IcePip) },
				{ ObjectIndexes.LavaEel.GetId(), new FishItem(ObjectIndexes.LavaEel) },
				{ ObjectIndexes.Sandfish.GetId(), new FishItem(ObjectIndexes.Sandfish) },
				{ ObjectIndexes.ScorpionCarp.GetId(), new FishItem(ObjectIndexes.ScorpionCarp) },
				{ ObjectIndexes.Flounder.GetId(), new FishItem(ObjectIndexes.Flounder) },
				{ ObjectIndexes.MidnightCarp.GetId(), new FishItem(ObjectIndexes.MidnightCarp) },
				{ ObjectIndexes.Sturgeon.GetId(), new FishItem(ObjectIndexes.Sturgeon) },
				{ ObjectIndexes.TigerTrout.GetId(), new FishItem(ObjectIndexes.TigerTrout) },
				{ ObjectIndexes.Bullhead.GetId(), new FishItem(ObjectIndexes.Bullhead) },
				{ ObjectIndexes.Tilapia.GetId(), new FishItem(ObjectIndexes.Tilapia) },
				{ ObjectIndexes.Chub.GetId(), new FishItem(ObjectIndexes.Chub) },
				{ ObjectIndexes.Dorado.GetId(), new FishItem(ObjectIndexes.Dorado) },
				{ ObjectIndexes.Albacore.GetId(), new FishItem(ObjectIndexes.Albacore) },
				{ ObjectIndexes.Shad.GetId(), new FishItem(ObjectIndexes.Shad) },
				{ ObjectIndexes.Lingcod.GetId(), new FishItem(ObjectIndexes.Lingcod) },
				{ ObjectIndexes.Halibut.GetId(), new FishItem(ObjectIndexes.Halibut) },
				{ ObjectIndexes.Woodskip.GetId(), new FishItem(ObjectIndexes.Woodskip) },
				{ ObjectIndexes.VoidSalmon.GetId(), new FishItem(ObjectIndexes.VoidSalmon, ObtainingDifficulties.Impossible) },
				{ ObjectIndexes.Slimejack.GetId(), new FishItem(ObjectIndexes.Slimejack, ObtainingDifficulties.Impossible) },
				{ ObjectIndexes.MidnightSquid.GetId(), new FishItem(ObjectIndexes.MidnightSquid, ObtainingDifficulties.RareItem) },
				{ ObjectIndexes.SpookFish.GetId(), new FishItem(ObjectIndexes.SpookFish, ObtainingDifficulties.RareItem) },
				{ ObjectIndexes.Blobfish.GetId(), new FishItem(ObjectIndexes.Blobfish, ObtainingDifficulties.RareItem) },
				{ ObjectIndexes.Crimsonfish.GetId(), new FishItem(ObjectIndexes.Crimsonfish, ObtainingDifficulties.EndgameItem) },
				{ ObjectIndexes.Angler.GetId(), new FishItem(ObjectIndexes.Angler, ObtainingDifficulties.EndgameItem) },
				{ ObjectIndexes.Legend.GetId(), new FishItem(ObjectIndexes.Legend, ObtainingDifficulties.EndgameItem) },
				{ ObjectIndexes.Glacierfish.GetId(), new FishItem(ObjectIndexes.Glacierfish, ObtainingDifficulties.EndgameItem) },
				{ ObjectIndexes.MutantCarp.GetId(), new FishItem(ObjectIndexes.MutantCarp, ObtainingDifficulties.EndgameItem) },

				// Crab pot specific
				{ ObjectIndexes.Lobster.GetId(), new CrabPotItem(ObjectIndexes.Lobster) },
				{ ObjectIndexes.Crab.GetId(), new CrabPotItem(ObjectIndexes.Crab) },
				{ ObjectIndexes.Shrimp.GetId(), new CrabPotItem(ObjectIndexes.Shrimp) },
				{ ObjectIndexes.Crayfish.GetId(), new CrabPotItem(ObjectIndexes.Crayfish) },
				{ ObjectIndexes.Snail.GetId(), new CrabPotItem(ObjectIndexes.Snail) },
				{ ObjectIndexes.Periwinkle.GetId(), new CrabPotItem(ObjectIndexes.Periwinkle) },

				// Items you can find in the mines
				{ ObjectIndexes.CaveCarrot.GetId(), new Item(ObjectIndexes.CaveCarrot.GetId(), ObtainingDifficulties.SmallTimeRequirements) { ItemsRequiredForRecipe = new Range(1, 3) } },
				{ ObjectIndexes.BugMeat.GetId(), new MonsterItem(ObjectIndexes.BugMeat.GetId(), ObtainingDifficulties.SmallTimeRequirements) { ItemsRequiredForRecipe = new Range(1, 5) } },
				{ ObjectIndexes.Slime.GetId(), new MonsterItem(ObjectIndexes.Slime.GetId(), ObtainingDifficulties.SmallTimeRequirements) { ItemsRequiredForRecipe = new Range(1, 10) } },
				{ ObjectIndexes.BatWing.GetId(), new MonsterItem(ObjectIndexes.BatWing.GetId(), ObtainingDifficulties.MediumTimeRequirements) { ItemsRequiredForRecipe = new Range(1, 5) } },
				{ ObjectIndexes.VoidEssence.GetId(), new MonsterItem(ObjectIndexes.VoidEssence.GetId(), ObtainingDifficulties.MediumTimeRequirements) { ItemsRequiredForRecipe = new Range(1, 5) } },
				{ ObjectIndexes.SolarEssence.GetId(), new MonsterItem(ObjectIndexes.SolarEssence.GetId(), ObtainingDifficulties.MediumTimeRequirements) { ItemsRequiredForRecipe = new Range(1, 5) } },
				{ ObjectIndexes.SquidInk.GetId(), new MonsterItem(ObjectIndexes.SquidInk.GetId(), ObtainingDifficulties.MediumTimeRequirements) { ItemsRequiredForRecipe = new Range(1, 5) } },
				{ ObjectIndexes.BoneFragment.GetId(), new MonsterItem(ObjectIndexes.BoneFragment.GetId(), ObtainingDifficulties.MediumTimeRequirements) { ItemsRequiredForRecipe = new Range(1, 5) } },
				{ ObjectIndexes.GreenSlimeEgg.GetId(), new Item(ObjectIndexes.GreenSlimeEgg.GetId(), ObtainingDifficulties.LargeTimeRequirements) },
				{ ObjectIndexes.BlueSlimeEgg.GetId(), new Item(ObjectIndexes.BlueSlimeEgg.GetId(), ObtainingDifficulties.LargeTimeRequirements) },
				{ ObjectIndexes.RedSlimeEgg.GetId(), new Item(ObjectIndexes.RedSlimeEgg.GetId(), ObtainingDifficulties.EndgameItem) },
				{ ObjectIndexes.PurpleSlimeEgg.GetId(), new Item(ObjectIndexes.PurpleSlimeEgg.GetId(), ObtainingDifficulties.EndgameItem) },

				{ ObjectIndexes.Coal.GetId(), new Item(ObjectIndexes.Coal.GetId(), ObtainingDifficulties.SmallTimeRequirements) { ItemsRequiredForRecipe = new Range(1, 5) } },
				{ ObjectIndexes.CopperOre.GetId(), new Item(ObjectIndexes.CopperOre.GetId(), ObtainingDifficulties.SmallTimeRequirements) { ItemsRequiredForRecipe = new Range(1, 5) } },
				{ ObjectIndexes.IronOre.GetId(), new Item(ObjectIndexes.IronOre.GetId(), ObtainingDifficulties.MediumTimeRequirements) { ItemsRequiredForRecipe = new Range(1, 5) } },
				{ ObjectIndexes.GoldOre.GetId(), new Item(ObjectIndexes.GoldOre.GetId(), ObtainingDifficulties.MediumTimeRequirements) { ItemsRequiredForRecipe = new Range(1, 5) } },
				{ ObjectIndexes.IridiumOre.GetId(), new Item(ObjectIndexes.IridiumOre.GetId(), ObtainingDifficulties.EndgameItem) { ItemsRequiredForRecipe = new Range(1, 3) } },

				{ ObjectIndexes.Quartz.GetId(), new Item(ObjectIndexes.Quartz.GetId(), ObtainingDifficulties.SmallTimeRequirements) { ItemsRequiredForRecipe = new Range(1, 3) } },
				{ ObjectIndexes.FireQuartz.GetId(), new Item(ObjectIndexes.FireQuartz.GetId(), ObtainingDifficulties.MediumTimeRequirements) { ItemsRequiredForRecipe = new Range(1, 2) } },
				{ ObjectIndexes.EarthCrystal.GetId(), new Item(ObjectIndexes.EarthCrystal.GetId(), ObtainingDifficulties.SmallTimeRequirements) },
				{ ObjectIndexes.FrozenTear.GetId(), new Item(ObjectIndexes.FrozenTear.GetId(), ObtainingDifficulties.MediumTimeRequirements) { ItemsRequiredForRecipe = new Range(1, 2) } },

				{ ObjectIndexes.Geode.GetId(), new Item(ObjectIndexes.Geode.GetId(), ObtainingDifficulties.SmallTimeRequirements) },
				{ ObjectIndexes.FrozenGeode.GetId(), new Item(ObjectIndexes.FrozenGeode.GetId(), ObtainingDifficulties.MediumTimeRequirements) { ItemsRequiredForRecipe = new Range(1, 3) } },
				{ ObjectIndexes.MagmaGeode.GetId(), new Item(ObjectIndexes.MagmaGeode.GetId(), ObtainingDifficulties.MediumTimeRequirements) { ItemsRequiredForRecipe = new Range(1, 2) } },
				{ ObjectIndexes.OmniGeode.GetId(), new Item(ObjectIndexes.OmniGeode.GetId(), ObtainingDifficulties.MediumTimeRequirements) },

				{ ObjectIndexes.Aquamarine.GetId(), new Item(ObjectIndexes.Aquamarine.GetId(), ObtainingDifficulties.MediumTimeRequirements) },
				{ ObjectIndexes.Amethyst.GetId(), new Item(ObjectIndexes.Amethyst.GetId(), ObtainingDifficulties.MediumTimeRequirements) },
				{ ObjectIndexes.Emerald.GetId(), new Item(ObjectIndexes.Emerald.GetId(), ObtainingDifficulties.MediumTimeRequirements) },
				{ ObjectIndexes.Ruby.GetId(), new Item(ObjectIndexes.Ruby.GetId(), ObtainingDifficulties.MediumTimeRequirements) },
				{ ObjectIndexes.Topaz.GetId(), new Item(ObjectIndexes.Topaz.GetId(), ObtainingDifficulties.MediumTimeRequirements) },
				{ ObjectIndexes.Jade.GetId(), new Item(ObjectIndexes.Jade.GetId(), ObtainingDifficulties.MediumTimeRequirements) },
				{ ObjectIndexes.Diamond.GetId(), new Item(ObjectIndexes.Diamond.GetId(), ObtainingDifficulties.MediumTimeRequirements) },

				// Geode mineral items - ObtainingDifficulties.LargeTimeRequirements
				{ ObjectIndexes.Alamite.GetId(), new GeodeMineralItem(ObjectIndexes.Alamite) },
				{ ObjectIndexes.Calcite.GetId(), new GeodeMineralItem(ObjectIndexes.Calcite) },
				{ ObjectIndexes.Celestine.GetId(), new GeodeMineralItem(ObjectIndexes.Celestine) },
				{ ObjectIndexes.Granite.GetId(), new GeodeMineralItem(ObjectIndexes.Granite) },
				{ ObjectIndexes.Jagoite.GetId(), new GeodeMineralItem(ObjectIndexes.Jagoite) },
				{ ObjectIndexes.Jamborite.GetId(), new GeodeMineralItem(ObjectIndexes.Jamborite) },
				{ ObjectIndexes.Limestone.GetId(), new GeodeMineralItem(ObjectIndexes.Limestone) },
				{ ObjectIndexes.Malachite.GetId(), new GeodeMineralItem(ObjectIndexes.Malachite) },
				{ ObjectIndexes.Mudstone.GetId(), new GeodeMineralItem(ObjectIndexes.Mudstone) },
				{ ObjectIndexes.Nekoite.GetId(), new GeodeMineralItem(ObjectIndexes.Nekoite) },
				{ ObjectIndexes.Orpiment.GetId(), new GeodeMineralItem(ObjectIndexes.Orpiment) },
				{ ObjectIndexes.PetrifiedSlime.GetId(), new GeodeMineralItem(ObjectIndexes.PetrifiedSlime) },
				{ ObjectIndexes.Sandstone.GetId(), new GeodeMineralItem(ObjectIndexes.Sandstone) },
				{ ObjectIndexes.Slate.GetId(), new GeodeMineralItem(ObjectIndexes.Slate) },
				{ ObjectIndexes.ThunderEgg.GetId(), new GeodeMineralItem(ObjectIndexes.ThunderEgg) },

				{ ObjectIndexes.Aerinite.GetId(), new GeodeMineralItem(ObjectIndexes.Aerinite) },
				{ ObjectIndexes.Esperite.GetId(), new GeodeMineralItem(ObjectIndexes.Esperite) },
				{ ObjectIndexes.FairyStone.GetId(), new GeodeMineralItem(ObjectIndexes.FairyStone) },
				{ ObjectIndexes.Fluorapatite.GetId(), new GeodeMineralItem(ObjectIndexes.Fluorapatite) },
				{ ObjectIndexes.Geminite.GetId(), new GeodeMineralItem(ObjectIndexes.Geminite) },
				{ ObjectIndexes.GhostCrystal.GetId(), new GeodeMineralItem(ObjectIndexes.GhostCrystal) },
				{ ObjectIndexes.Hematite.GetId(), new GeodeMineralItem(ObjectIndexes.Hematite) },
				{ ObjectIndexes.Kyanite.GetId(), new GeodeMineralItem(ObjectIndexes.Kyanite) },
				{ ObjectIndexes.Lunarite.GetId(), new GeodeMineralItem(ObjectIndexes.Lunarite) },
				{ ObjectIndexes.Marble.GetId(), new GeodeMineralItem(ObjectIndexes.Marble) },
				{ ObjectIndexes.OceanStone.GetId(), new GeodeMineralItem(ObjectIndexes.OceanStone) },
				{ ObjectIndexes.Opal.GetId(), new GeodeMineralItem(ObjectIndexes.Opal) },
				{ ObjectIndexes.Pyrite.GetId(), new GeodeMineralItem(ObjectIndexes.Pyrite) },
				{ ObjectIndexes.Soapstone.GetId(), new GeodeMineralItem(ObjectIndexes.Soapstone) },

				{ ObjectIndexes.Baryte.GetId(), new GeodeMineralItem(ObjectIndexes.Baryte) },
				{ ObjectIndexes.Basalt.GetId(), new GeodeMineralItem(ObjectIndexes.Basalt) },
				{ ObjectIndexes.Bixite.GetId(), new GeodeMineralItem(ObjectIndexes.Bixite) },
				{ ObjectIndexes.Dolomite.GetId(), new GeodeMineralItem(ObjectIndexes.Dolomite) },
				{ ObjectIndexes.FireOpal.GetId(), new GeodeMineralItem(ObjectIndexes.FireOpal) },
				{ ObjectIndexes.Helvite.GetId(), new GeodeMineralItem(ObjectIndexes.Helvite) },
				{ ObjectIndexes.Jasper.GetId(), new GeodeMineralItem(ObjectIndexes.Jasper) },
				{ ObjectIndexes.LemonStone.GetId(), new GeodeMineralItem(ObjectIndexes.LemonStone) },
				{ ObjectIndexes.Neptunite.GetId(), new GeodeMineralItem(ObjectIndexes.Neptunite) },
				{ ObjectIndexes.Obsidian.GetId(), new GeodeMineralItem(ObjectIndexes.Obsidian) },
				{ ObjectIndexes.StarShards.GetId(), new GeodeMineralItem(ObjectIndexes.StarShards) },
				{ ObjectIndexes.Tigerseye.GetId(), new GeodeMineralItem(ObjectIndexes.Tigerseye) },

				// Rings - a few of them are craftable
				{ ObjectIndexes.SmallGlowRing.GetId(), new RingItem(ObjectIndexes.SmallGlowRing) },
				{ ObjectIndexes.GlowRing.GetId(), new RingItem(ObjectIndexes.GlowRing) },
				{ ObjectIndexes.SmallMagnetRing.GetId(), new RingItem(ObjectIndexes.SmallMagnetRing) },
				{ ObjectIndexes.MagnetRing.GetId(), new RingItem(ObjectIndexes.MagnetRing) },
				{ ObjectIndexes.SlimeCharmerRing.GetId(), new RingItem(ObjectIndexes.SlimeCharmerRing) },
				{ ObjectIndexes.VampireRing.GetId(), new RingItem(ObjectIndexes.VampireRing) },
				{ ObjectIndexes.SavageRing.GetId(), new RingItem(ObjectIndexes.SavageRing) },
				{ ObjectIndexes.BurglarsRing.GetId(), new RingItem(ObjectIndexes.BurglarsRing) },
				{ ObjectIndexes.AmethystRing.GetId(), new RingItem(ObjectIndexes.AmethystRing) },
				{ ObjectIndexes.TopazRing.GetId(), new RingItem(ObjectIndexes.TopazRing) },
				{ ObjectIndexes.AquamarineRing.GetId(), new RingItem(ObjectIndexes.AquamarineRing) },
				{ ObjectIndexes.JadeRing.GetId(), new RingItem(ObjectIndexes.JadeRing) },
				{ ObjectIndexes.EmeraldRing.GetId(), new RingItem(ObjectIndexes.EmeraldRing) },
				{ ObjectIndexes.RubyRing.GetId(), new RingItem(ObjectIndexes.RubyRing) },

				// Animal items - default is ObtainingDifficulties.MediumTimeRequirements, +1 for each building/large version/cheese press required
				{ ObjectIndexes.Honey.GetId(), new AnimalItem(ObjectIndexes.Honey, ObtainingDifficulties.LargeTimeRequirements) { RequiresBeehouse = true } },
				{ ObjectIndexes.WhiteEgg.GetId(), new AnimalItem(ObjectIndexes.WhiteEgg) },
				{ ObjectIndexes.LargeWhiteEgg.GetId(), new AnimalItem(ObjectIndexes.LargeWhiteEgg, ObtainingDifficulties.LargeTimeRequirements) },
				{ ObjectIndexes.BrownEgg.GetId(), new AnimalItem(ObjectIndexes.BrownEgg) },
				{ ObjectIndexes.LargeBrownEgg.GetId(), new AnimalItem(ObjectIndexes.LargeBrownEgg, ObtainingDifficulties.LargeTimeRequirements) },
				{ ObjectIndexes.VoidEgg.GetId(), new AnimalItem(ObjectIndexes.VoidEgg, ObtainingDifficulties.EndgameItem) },
				{ ObjectIndexes.Milk.GetId(), new AnimalItem(ObjectIndexes.Milk) },
				{ ObjectIndexes.LargeMilk.GetId(), new AnimalItem(ObjectIndexes.LargeMilk, ObtainingDifficulties.LargeTimeRequirements) },
				{ ObjectIndexes.GoatMilk.GetId(), new AnimalItem(ObjectIndexes.GoatMilk, ObtainingDifficulties.LargeTimeRequirements) },
				{ ObjectIndexes.LargeGoatMilk.GetId(), new AnimalItem(ObjectIndexes.LargeGoatMilk, ObtainingDifficulties.EndgameItem) },
				{ ObjectIndexes.DuckEgg.GetId(), new AnimalItem(ObjectIndexes.DuckEgg, ObtainingDifficulties.LargeTimeRequirements) },
				{ ObjectIndexes.DuckFeather.GetId(), new AnimalItem(ObjectIndexes.DuckFeather, ObtainingDifficulties.EndgameItem) },
				{ ObjectIndexes.Wool.GetId(), new AnimalItem(ObjectIndexes.Wool, ObtainingDifficulties.EndgameItem) },
				{ ObjectIndexes.Cloth.GetId(), new AnimalItem(ObjectIndexes.Cloth, ObtainingDifficulties.EndgameItem) },
				{ ObjectIndexes.RabbitsFoot.GetId(), new AnimalItem(ObjectIndexes.RabbitsFoot, ObtainingDifficulties.EndgameItem) },
				{ ObjectIndexes.Truffle.GetId(), new AnimalItem(ObjectIndexes.Truffle, ObtainingDifficulties.EndgameItem) },
				{ ObjectIndexes.TruffleOil.GetId(), new AnimalItem(ObjectIndexes.TruffleOil, ObtainingDifficulties.EndgameItem) { RequiresOilMaker = true } },
				{ ObjectIndexes.Mayonnaise.GetId(), new AnimalItem(ObjectIndexes.Mayonnaise) },
				{ ObjectIndexes.DuckMayonnaise.GetId(), new AnimalItem(ObjectIndexes.DuckMayonnaise, ObtainingDifficulties.LargeTimeRequirements) },
				{ ObjectIndexes.VoidMayonnaise.GetId(), new AnimalItem(ObjectIndexes.VoidMayonnaise, ObtainingDifficulties.EndgameItem) },
				{ ObjectIndexes.Cheese.GetId(), new AnimalItem(ObjectIndexes.Cheese, ObtainingDifficulties.LargeTimeRequirements)},
				{ ObjectIndexes.GoatCheese.GetId(), new AnimalItem(ObjectIndexes.GoatCheese, ObtainingDifficulties.EndgameItem)},

				// Artifacts and rare items
				{ ObjectIndexes.DwarfScrollI.GetId(), new ArtifactItem(ObjectIndexes.DwarfScrollI) },
				{ ObjectIndexes.DwarfScrollII.GetId(), new ArtifactItem(ObjectIndexes.DwarfScrollII) },
				{ ObjectIndexes.DwarfScrollIII.GetId(), new ArtifactItem(ObjectIndexes.DwarfScrollIII) },
				{ ObjectIndexes.DwarfScrollIV.GetId(), new ArtifactItem(ObjectIndexes.DwarfScrollIV) },
				{ ObjectIndexes.ChippedAmphora.GetId(), new ArtifactItem(ObjectIndexes.ChippedAmphora) },
				{ ObjectIndexes.Arrowhead.GetId(), new ArtifactItem(ObjectIndexes.Arrowhead) },
				{ ObjectIndexes.AncientDoll.GetId(), new ArtifactItem(ObjectIndexes.AncientDoll) },
				{ ObjectIndexes.ElvishJewelry.GetId(), new ArtifactItem(ObjectIndexes.ElvishJewelry) },
				{ ObjectIndexes.ChewingStick.GetId(), new ArtifactItem(ObjectIndexes.ChewingStick) },
				{ ObjectIndexes.OrnamentalFan.GetId(), new ArtifactItem(ObjectIndexes.OrnamentalFan) },
				{ ObjectIndexes.AncientSword.GetId(), new ArtifactItem(ObjectIndexes.AncientSword) },
				{ ObjectIndexes.RustySpoon.GetId(), new ArtifactItem(ObjectIndexes.RustySpoon) },
				{ ObjectIndexes.RustySpur.GetId(), new ArtifactItem(ObjectIndexes.RustySpur) },
				{ ObjectIndexes.RustyCog.GetId(), new ArtifactItem(ObjectIndexes.RustyCog) },
				{ ObjectIndexes.ChickenStatue.GetId(), new ArtifactItem(ObjectIndexes.ChickenStatue) },
				{ ObjectIndexes.PrehistoricTool.GetId(), new ArtifactItem(ObjectIndexes.PrehistoricTool) },
				{ ObjectIndexes.DriedStarfish.GetId(), new ArtifactItem(ObjectIndexes.DriedStarfish) },
				{ ObjectIndexes.Anchor.GetId(), new ArtifactItem(ObjectIndexes.Anchor) },
				{ ObjectIndexes.GlassShards.GetId(), new ArtifactItem(ObjectIndexes.GlassShards) },
				{ ObjectIndexes.BoneFlute.GetId(), new ArtifactItem(ObjectIndexes.BoneFlute) },
				{ ObjectIndexes.PrehistoricHandaxe.GetId(), new ArtifactItem(ObjectIndexes.PrehistoricHandaxe) },
				{ ObjectIndexes.DwarvishHelm.GetId(), new ArtifactItem(ObjectIndexes.DwarvishHelm) },
				{ ObjectIndexes.DwarfGadget.GetId(), new ArtifactItem(ObjectIndexes.DwarfGadget) },
				{ ObjectIndexes.AncientDrum.GetId(), new ArtifactItem(ObjectIndexes.AncientDrum) },
				{ ObjectIndexes.PrehistoricScapula.GetId(), new ArtifactItem(ObjectIndexes.PrehistoricScapula) },
				{ ObjectIndexes.PrehistoricTibia.GetId(), new ArtifactItem(ObjectIndexes.PrehistoricTibia) },
				{ ObjectIndexes.PrehistoricSkull.GetId(), new ArtifactItem(ObjectIndexes.PrehistoricSkull) },
				{ ObjectIndexes.SkeletalHand.GetId(), new ArtifactItem(ObjectIndexes.SkeletalHand) },
				{ ObjectIndexes.PrehistoricRib.GetId(), new ArtifactItem(ObjectIndexes.PrehistoricRib) },
				{ ObjectIndexes.PrehistoricVertebra.GetId(), new ArtifactItem(ObjectIndexes.PrehistoricVertebra) },
				{ ObjectIndexes.SkeletalTail.GetId(), new ArtifactItem(ObjectIndexes.SkeletalTail) },
				{ ObjectIndexes.NautilusFossil.GetId(), new ArtifactItem(ObjectIndexes.NautilusFossil) },
				{ ObjectIndexes.AmphibianFossil.GetId(), new ArtifactItem(ObjectIndexes.AmphibianFossil) },
				{ ObjectIndexes.PalmFossil.GetId(), new ArtifactItem(ObjectIndexes.PalmFossil) },
				{ ObjectIndexes.Trilobite.GetId(), new ArtifactItem(ObjectIndexes.Trilobite) },

				{ ObjectIndexes.StrangeDoll1.GetId(), new Item(ObjectIndexes.StrangeDoll1.GetId(), ObtainingDifficulties.RareItem) },
				{ ObjectIndexes.StrangeDoll2.GetId(), new Item(ObjectIndexes.StrangeDoll2.GetId(), ObtainingDifficulties.RareItem) },
				{ ObjectIndexes.PrismaticShard.GetId(), new Item(ObjectIndexes.PrismaticShard.GetId(), ObtainingDifficulties.RareItem) },
				{ ObjectIndexes.DinosaurEgg.GetId(), new ArtifactItem(ObjectIndexes.DinosaurEgg, ObtainingDifficulties.RareItem) },
				{ ObjectIndexes.RareDisc.GetId(), new ArtifactItem(ObjectIndexes.RareDisc, ObtainingDifficulties.RareItem) },
				{ ObjectIndexes.GoldenMask.GetId(), new ArtifactItem(ObjectIndexes.GoldenMask, ObtainingDifficulties.RareItem) },
				{ ObjectIndexes.GoldenRelic.GetId(), new ArtifactItem(ObjectIndexes.GoldenRelic, ObtainingDifficulties.RareItem) },
				{ ObjectIndexes.AncientSeed.GetId(), new ArtifactItem(ObjectIndexes.AncientSeed, ObtainingDifficulties.RareItem) },

				// Items on Ginger Island - not randomizing yet, so marking as impossible
				{ ObjectIndexes.TaroRoot.GetId(), new Item(ObjectIndexes.TaroRoot.GetId(), ObtainingDifficulties.Impossible) },
				{ ObjectIndexes.TaroTuber.GetId(), new Item(ObjectIndexes.TaroTuber.GetId(), ObtainingDifficulties.Impossible) },
				{ ObjectIndexes.Pineapple.GetId(), new Item(ObjectIndexes.Pineapple.GetId(), ObtainingDifficulties.Impossible) },
				{ ObjectIndexes.PineappleSeeds.GetId(), new Item(ObjectIndexes.PineappleSeeds.GetId(), ObtainingDifficulties.Impossible) },
				{ ObjectIndexes.CinderShard.GetId(), new Item(ObjectIndexes.CinderShard.GetId(), ObtainingDifficulties.Impossible) },
				{ ObjectIndexes.MagmaCap.GetId(), new Item(ObjectIndexes.MagmaCap.GetId(), ObtainingDifficulties.Impossible) },
				{ ObjectIndexes.DragonTooth.GetId(), new Item(ObjectIndexes.DragonTooth.GetId(), ObtainingDifficulties.Impossible) },

				// Misc - those marked as impossible you can only get a limited amount of
				{ ObjectIndexes.Battery.GetId(), new Item(ObjectIndexes.Battery.GetId(), ObtainingDifficulties.LargeTimeRequirements) },
				{ ObjectIndexes.LuckyPurpleShorts.GetId(), new Item(ObjectIndexes.LuckyPurpleShorts.GetId(), ObtainingDifficulties.Impossible) },
				{ ObjectIndexes.LostAxe.GetId(), new Item(ObjectIndexes.LostAxe.GetId(), ObtainingDifficulties.Impossible) },
				{ ObjectIndexes.BerryBasket.GetId(), new Item(ObjectIndexes.BerryBasket.GetId(), ObtainingDifficulties.Impossible) },
				{ ObjectIndexes.Pearl.GetId(), new Item(ObjectIndexes.Pearl.GetId(), ObtainingDifficulties.Impossible) },
				{ ObjectIndexes.IridiumMilk.GetId(), new Item(ObjectIndexes.IridiumMilk.GetId(), ObtainingDifficulties.Impossible) },
				{ ObjectIndexes.DecorativePot.GetId(), new Item(ObjectIndexes.DecorativePot.GetId(), ObtainingDifficulties.Impossible) { CanStack = false } },
				{ ObjectIndexes.TeaSet.GetId(), new Item(ObjectIndexes.TeaSet.GetId(), ObtainingDifficulties.Impossible) { CanStack = false } },
				{ ObjectIndexes.PurpleMushroom.GetId(), new Item(ObjectIndexes.PurpleMushroom.GetId(), ObtainingDifficulties.MediumTimeRequirements) },
				{ ObjectIndexes.Mead.GetId(), new Item(ObjectIndexes.Mead.GetId(), ObtainingDifficulties.LargeTimeRequirements) { RequiresBeehouse = true, RequiresKeg = true } },
				{ ObjectIndexes.PaleAle.GetId(), new Item(ObjectIndexes.PaleAle.GetId(), ObtainingDifficulties.LargeTimeRequirements) { RequiresKeg = true } },
				{ ObjectIndexes.MermaidsPendant.GetId(), new Item(ObjectIndexes.MermaidsPendant.GetId(), ObtainingDifficulties.EndgameItem) { OverrideName = "Mermaid's Pendant" } },
				{ ObjectIndexes.TreasureChest.GetId(), new Item(ObjectIndexes.TreasureChest.GetId(), ObtainingDifficulties.Impossible) },
				{ ObjectIndexes.MuscleRemedy.GetId(), new Item(ObjectIndexes.MuscleRemedy.GetId(), ObtainingDifficulties.NonCraftingItem) },
				{ ObjectIndexes.EnergyTonic.GetId(), new Item(ObjectIndexes.EnergyTonic.GetId(), ObtainingDifficulties.NonCraftingItem) },
				{ ObjectIndexes.Stardrop.GetId(), new Item(ObjectIndexes.Stardrop.GetId(), ObtainingDifficulties.Impossible) },
				{ ObjectIndexes.Bouquet.GetId(), new Item(ObjectIndexes.Bouquet.GetId(), ObtainingDifficulties.NonCraftingItem) },
				{ ObjectIndexes.Vinegar.GetId(), new Item(ObjectIndexes.Vinegar.GetId(), ObtainingDifficulties.NonCraftingItem) },
				{ ObjectIndexes.Beer.GetId(), new Item(ObjectIndexes.Beer.GetId(), ObtainingDifficulties.NonCraftingItem) },
				{ ObjectIndexes.Wine.GetId(), new Item(ObjectIndexes.Wine.GetId(), ObtainingDifficulties.Impossible) },
				{ ObjectIndexes.Juice.GetId(), new Item(ObjectIndexes.Juice.GetId(), ObtainingDifficulties.Impossible) },
				{ ObjectIndexes.Jelly.GetId(), new Item(ObjectIndexes.Jelly.GetId(), ObtainingDifficulties.Impossible) },
				{ ObjectIndexes.Pickles.GetId(), new Item(ObjectIndexes.Pickles.GetId(), ObtainingDifficulties.Impossible) },
				{ ObjectIndexes.GoldenPumpkin.GetId(), new Item(ObjectIndexes.GoldenPumpkin.GetId(), ObtainingDifficulties.NonCraftingItem) },
				{ ObjectIndexes.Rice.GetId(), new Item(ObjectIndexes.Rice.GetId(), ObtainingDifficulties.MediumTimeRequirements) },
				{ ObjectIndexes.Salmonberry.GetId(), new Item(ObjectIndexes.Salmonberry.GetId(), ObtainingDifficulties.LargeTimeRequirements) },
				{ ObjectIndexes.SpringOnion.GetId(), new Item(ObjectIndexes.SpringOnion.GetId(), ObtainingDifficulties.NonCraftingItem) },
				{ ObjectIndexes.Coffee.GetId(), new Item(ObjectIndexes.Coffee.GetId(), ObtainingDifficulties.NonCraftingItem) },

				// All cooking recipes - ObtainingDifficulties.LargeTimeRequirements
				{ ObjectIndexes.FriedEgg.GetId(), new CookedItem(ObjectIndexes.FriedEgg) },
				{ ObjectIndexes.Omelet.GetId(), new CookedItem(ObjectIndexes.Omelet) },
				{ ObjectIndexes.Salad.GetId(), new CookedItem(ObjectIndexes.Salad) },
				{ ObjectIndexes.CheeseCauliflower.GetId(), new CookedItem(ObjectIndexes.CheeseCauliflower, ObjectIndexes.Cauliflower.GetId()) },
				{ ObjectIndexes.BakedFish.GetId(), new CookedItem(ObjectIndexes.BakedFish) },
				{ ObjectIndexes.ParsnipSoup.GetId(), new CookedItem(ObjectIndexes.ParsnipSoup, ObjectIndexes.Parsnip.GetId()) },
				{ ObjectIndexes.VegetableMedley.GetId(), new CookedItem(ObjectIndexes.VegetableMedley) },
				{ ObjectIndexes.CompleteBreakfast.GetId(), new CookedItem(ObjectIndexes.CompleteBreakfast) },
				{ ObjectIndexes.FriedCalamari.GetId(), new CookedItem(ObjectIndexes.FriedCalamari) },
				{ ObjectIndexes.StrangeBun.GetId(), new CookedItem(ObjectIndexes.StrangeBun) },

				{ ObjectIndexes.LuckyLunch.GetId(), new CookedItem(ObjectIndexes.LuckyLunch) },
				{ ObjectIndexes.FriedMushroom.GetId(), new CookedItem(ObjectIndexes.FriedMushroom) },
				{ ObjectIndexes.Pizza.GetId(), new CookedItem(ObjectIndexes.Pizza) },
				{ ObjectIndexes.BeanHotpot.GetId(), new CookedItem(ObjectIndexes.BeanHotpot, ObjectIndexes.GreenBean.GetId()) },
				{ ObjectIndexes.GlazedYams.GetId(), new CookedItem(ObjectIndexes.GlazedYams, ObjectIndexes.Yam.GetId()) },
				{ ObjectIndexes.CarpSurprise.GetId(), new CookedItem(ObjectIndexes.CarpSurprise, ObjectIndexes.Carp.GetId(), isFishDish: true) },
				{ ObjectIndexes.Hashbrowns.GetId(), new CookedItem(ObjectIndexes.Hashbrowns) },
				{ ObjectIndexes.Pancakes.GetId(), new CookedItem(ObjectIndexes.Pancakes) },
				{ ObjectIndexes.SalmonDinner.GetId(), new CookedItem(ObjectIndexes.SalmonDinner, ObjectIndexes.Salmon.GetId(), isFishDish: true) },
				{ ObjectIndexes.FishTaco.GetId(), new CookedItem(ObjectIndexes.FishTaco) },

				{ ObjectIndexes.CrispyBass.GetId(), new CookedItem(ObjectIndexes.CrispyBass, ObjectIndexes.LargemouthBass.GetId(), isFishDish: true) },
				{ ObjectIndexes.PepperPoppers.GetId(), new CookedItem(ObjectIndexes.PepperPoppers, ObjectIndexes.HotPepper.GetId()) },
				{ ObjectIndexes.Bread.GetId(), new CookedItem(ObjectIndexes.Bread) },
				{ ObjectIndexes.TomKhaSoup.GetId(), new CookedItem(ObjectIndexes.TomKhaSoup) },
				{ ObjectIndexes.TroutSoup.GetId(), new CookedItem(ObjectIndexes.TroutSoup, ObjectIndexes.RainbowTrout.GetId(), isFishDish: true) },
				{ ObjectIndexes.ChocolateCake.GetId(), new CookedItem(ObjectIndexes.ChocolateCake) },
				{ ObjectIndexes.PinkCake.GetId(), new CookedItem(ObjectIndexes.PinkCake) },
				{ ObjectIndexes.RhubarbPie.GetId(), new CookedItem(ObjectIndexes.RhubarbPie, ObjectIndexes.Rhubarb.GetId()) },
				{ ObjectIndexes.Cookie.GetId(), new CookedItem(ObjectIndexes.Cookie) },
				{ ObjectIndexes.Spaghetti.GetId(), new CookedItem(ObjectIndexes.Spaghetti) },

				{ ObjectIndexes.FriedEel.GetId(), new CookedItem(ObjectIndexes.FriedEel, ObjectIndexes.Eel.GetId(), isFishDish: true) },
				{ ObjectIndexes.SpicyEel.GetId(), new CookedItem(ObjectIndexes.SpicyEel, ObjectIndexes.Eel.GetId(), isFishDish: true) },
				{ ObjectIndexes.Sashimi.GetId(), new CookedItem(ObjectIndexes.Sashimi) },
				{ ObjectIndexes.MakiRoll.GetId(), new CookedItem(ObjectIndexes.MakiRoll) },
				{ ObjectIndexes.Tortilla.GetId(), new CookedItem(ObjectIndexes.Tortilla) },
				{ ObjectIndexes.RedPlate.GetId(), new CookedItem(ObjectIndexes.RedPlate) },
				{ ObjectIndexes.EggplantParmesan.GetId(), new CookedItem(ObjectIndexes.EggplantParmesan, ObjectIndexes.Eggplant.GetId()) },
				{ ObjectIndexes.RicePudding.GetId(), new CookedItem(ObjectIndexes.RicePudding, ObjectIndexes.Rice.GetId()) },
				{ ObjectIndexes.IceCream.GetId(), new CookedItem(ObjectIndexes.IceCream) },
				{ ObjectIndexes.BlueberryTart.GetId(), new CookedItem(ObjectIndexes.BlueberryTart, ObjectIndexes.Blueberry.GetId()) },

				{ ObjectIndexes.AutumnsBounty.GetId(), new CookedItem(ObjectIndexes.AutumnsBounty) { OverrideName = "Autumn's Bounty" } },
				{ ObjectIndexes.PumpkinSoup.GetId(), new CookedItem(ObjectIndexes.PumpkinSoup, ObjectIndexes.Pumpkin.GetId()) },
				{ ObjectIndexes.SuperMeal.GetId(), new CookedItem(ObjectIndexes.SuperMeal) },
				{ ObjectIndexes.CranberrySauce.GetId(), new CookedItem(ObjectIndexes.CranberrySauce, ObjectIndexes.Cranberries.GetId()) },
				{ ObjectIndexes.Stuffing.GetId(), new CookedItem(ObjectIndexes.Stuffing) },
				{ ObjectIndexes.FarmersLunch.GetId(), new CookedItem(ObjectIndexes.FarmersLunch) { OverrideName = "Farmer's Lunch" } },
				{ ObjectIndexes.SurvivalBurger.GetId(), new CookedItem(ObjectIndexes.SurvivalBurger) },
				{ ObjectIndexes.DishOTheSea.GetId(), new CookedItem(ObjectIndexes.DishOTheSea) { OverrideName = "Dish o' The Sea" } },
				{ ObjectIndexes.MinersTreat.GetId(), new CookedItem(ObjectIndexes.MinersTreat) { OverrideName = "Miner's Treat" } },
				{ ObjectIndexes.RootsPlatter.GetId(), new CookedItem(ObjectIndexes.RootsPlatter) },

				{ ObjectIndexes.AlgaeSoup.GetId(), new CookedItem(ObjectIndexes.AlgaeSoup) },
				{ ObjectIndexes.PaleBroth.GetId(), new CookedItem(ObjectIndexes.PaleBroth) },
				{ ObjectIndexes.TripleShotEspresso.GetId(), new CookedItem(ObjectIndexes.TripleShotEspresso) },
				{ ObjectIndexes.PlumPudding.GetId(), new CookedItem(ObjectIndexes.PlumPudding) },
				{ ObjectIndexes.ArtichokeDip.GetId(), new CookedItem(ObjectIndexes.ArtichokeDip, ObjectIndexes.Artichoke.GetId()) },
				{ ObjectIndexes.StirFry.GetId(), new CookedItem(ObjectIndexes.StirFry) },
				{ ObjectIndexes.RoastedHazelnuts.GetId(), new CookedItem(ObjectIndexes.RoastedHazelnuts) },
				{ ObjectIndexes.PumpkinPie.GetId(), new CookedItem(ObjectIndexes.PumpkinPie, ObjectIndexes.Pumpkin.GetId()) },
				{ ObjectIndexes.RadishSalad.GetId(), new CookedItem(ObjectIndexes.RadishSalad, ObjectIndexes.Radish.GetId()) },
				{ ObjectIndexes.FruitSalad.GetId(), new CookedItem(ObjectIndexes.FruitSalad, ingredientId: null) },
				{ ObjectIndexes.BlackberryCobbler.GetId(), new CookedItem(ObjectIndexes.BlackberryCobbler) },

				{ ObjectIndexes.CranberryCandy.GetId(), new CookedItem(ObjectIndexes.CranberryCandy, ObjectIndexes.Cranberries.GetId()) },
				{ ObjectIndexes.Bruschetta.GetId(), new CookedItem(ObjectIndexes.Bruschetta) },
				{ ObjectIndexes.Coleslaw.GetId(), new CookedItem(ObjectIndexes.Coleslaw) },
				{ ObjectIndexes.FiddleheadRisotto.GetId(), new CookedItem(ObjectIndexes.FiddleheadRisotto) },
				{ ObjectIndexes.PoppyseedMuffin.GetId(), new CookedItem(ObjectIndexes.PoppyseedMuffin, ObjectIndexes.Poppy.GetId()) },
				{ ObjectIndexes.Chowder.GetId(), new CookedItem(ObjectIndexes.Chowder) },
				{ ObjectIndexes.LobsterBisque.GetId(), new CookedItem(ObjectIndexes.LobsterBisque) },
				{ ObjectIndexes.Escargot.GetId(), new CookedItem(ObjectIndexes.Escargot) },
				{ ObjectIndexes.FishStew.GetId(), new CookedItem(ObjectIndexes.FishStew) },
				{ ObjectIndexes.MapleBar.GetId(), new CookedItem(ObjectIndexes.MapleBar) },

				{ ObjectIndexes.CrabCakes.GetId(), new CookedItem(ObjectIndexes.CrabCakes) },

				// Internally, this IS a cooked item, but functionally - it actually has no matching recipe, so we won't define it that way
				// You can buy it for 3 prismatic shards, so it is very much an endgame item!
                { ObjectIndexes.MagicRockCandy.GetId(), new Item(ObjectIndexes.MagicRockCandy.GetId(), ObtainingDifficulties.EndgameItem) },
			
				// ------ All Foragables - ObtainingDifficulties.LargeTimeRequirements -------
				// Spring Foragables
				{ ObjectIndexes.WildHorseradish.GetId(), new ForagableItem(ObjectIndexes.WildHorseradish) },
				{ ObjectIndexes.Daffodil.GetId(), new ForagableItem(ObjectIndexes.Daffodil) },
				{ ObjectIndexes.Leek.GetId(), new ForagableItem(ObjectIndexes.Leek) },
				{ ObjectIndexes.Dandelion.GetId(), new ForagableItem(ObjectIndexes.Dandelion) },
				{ ObjectIndexes.Morel.GetId(), new ForagableItem(ObjectIndexes.Morel) },
				{ ObjectIndexes.CommonMushroom.GetId(), new ForagableItem(ObjectIndexes.CommonMushroom) }, // Also fall

				// Summer Foragables
				{ ObjectIndexes.SpiceBerry.GetId(), new ForagableItem(ObjectIndexes.SpiceBerry) },
				{ ObjectIndexes.Grape.GetId(), new CropItem(ObjectIndexes.Grape) { ShouldBeForagable = true, DifficultyToObtain = ObtainingDifficulties.LargeTimeRequirements, ItemsRequiredForRecipe = new Range(1, 3)} },
				{ ObjectIndexes.SweetPea.GetId(), new ForagableItem(ObjectIndexes.SweetPea) },
				{ ObjectIndexes.RedMushroom.GetId(), new ForagableItem(ObjectIndexes.RedMushroom) }, // Also fall
				{ ObjectIndexes.FiddleheadFern.GetId(), new ForagableItem(ObjectIndexes.FiddleheadFern) },

				// Fall Foragables
				{ ObjectIndexes.WildPlum.GetId(), new ForagableItem(ObjectIndexes.WildPlum) },
				{ ObjectIndexes.Hazelnut.GetId(), new ForagableItem(ObjectIndexes.Hazelnut) },
				{ ObjectIndexes.Blackberry.GetId(), new ForagableItem(ObjectIndexes.Blackberry) },
				{ ObjectIndexes.Chanterelle.GetId(), new ForagableItem(ObjectIndexes.Chanterelle) },

				// Winter Foragables
				{ ObjectIndexes.WinterRoot.GetId(), new ForagableItem(ObjectIndexes.WinterRoot) },
				{ ObjectIndexes.CrystalFruit.GetId(), new ForagableItem(ObjectIndexes.CrystalFruit) },
				{ ObjectIndexes.SnowYam.GetId(), new ForagableItem(ObjectIndexes.SnowYam) },
				{ ObjectIndexes.Crocus.GetId(), new ForagableItem(ObjectIndexes.Crocus) },
				{ ObjectIndexes.Holly.GetId(), new ForagableItem(ObjectIndexes.Holly) },

				// Beach Foragables - the medium ones can also be obtained from crab pots
				{ ObjectIndexes.NautilusShell.GetId(), new ForagableItem(ObjectIndexes.NautilusShell) },
				{ ObjectIndexes.Coral.GetId(), new ForagableItem(ObjectIndexes.Coral) },
				{ ObjectIndexes.SeaUrchin.GetId(), new ForagableItem(ObjectIndexes.SeaUrchin) },
				{ ObjectIndexes.RainbowShell.GetId(), new ForagableItem(ObjectIndexes.RainbowShell) },
				{ ObjectIndexes.Clam.GetId(), new ForagableItem(ObjectIndexes.Clam) { DifficultyToObtain = ObtainingDifficulties.MediumTimeRequirements } },
				{ ObjectIndexes.Cockle.GetId(), new ForagableItem(ObjectIndexes.Cockle) { DifficultyToObtain = ObtainingDifficulties.MediumTimeRequirements } },
				{ ObjectIndexes.Mussel.GetId(), new ForagableItem(ObjectIndexes.Mussel) { DifficultyToObtain = ObtainingDifficulties.MediumTimeRequirements } },
				{ ObjectIndexes.Oyster.GetId(), new ForagableItem(ObjectIndexes.Oyster) { DifficultyToObtain = ObtainingDifficulties.MediumTimeRequirements } },

				// Desert Foragables
				{ ObjectIndexes.Coconut.GetId(), new ForagableItem(ObjectIndexes.Coconut) },
				{ ObjectIndexes.CactusFruit.GetId(), new CropItem(ObjectIndexes.CactusFruit) { ShouldBeForagable = true, DifficultyToObtain = ObtainingDifficulties.LargeTimeRequirements, ItemsRequiredForRecipe = new Range(1, 3)} },

				// Fruit - since trees are randomized, we're making them foragable
				{ ObjectIndexes.Cherry.GetId(), new ForagableItem(ObjectIndexes.Cherry) { IsFruit = true } },
				{ ObjectIndexes.Apricot.GetId(), new ForagableItem(ObjectIndexes.Apricot) { IsFruit = true } },
				{ ObjectIndexes.Orange.GetId(), new ForagableItem(ObjectIndexes.Orange) { IsFruit = true } },
				{ ObjectIndexes.Peach.GetId(), new ForagableItem(ObjectIndexes.Peach) { IsFruit = true } },
				{ ObjectIndexes.Pomegranate.GetId(), new ForagableItem(ObjectIndexes.Pomegranate) { IsFruit = true } },
				{ ObjectIndexes.Apple.GetId(), new ForagableItem(ObjectIndexes.Apple) { IsFruit = true } },
				// ------ End Foragables -------

				// Smelted Items - ObtainingDifficulties.MediumTimeRequirements
				{ ObjectIndexes.RefinedQuartz.GetId(), new SmeltedItem(ObjectIndexes.RefinedQuartz) },
				{ ObjectIndexes.CopperBar.GetId(), new SmeltedItem(ObjectIndexes.CopperBar) },
				{ ObjectIndexes.IronBar.GetId(), new SmeltedItem(ObjectIndexes.IronBar) },
				{ ObjectIndexes.GoldBar.GetId(), new SmeltedItem(ObjectIndexes.GoldBar) },
				{ ObjectIndexes.IridiumBar.GetId(), new SmeltedItem(ObjectIndexes.IridiumBar, ObtainingDifficulties.EndgameItem) },

				// Trash items - ObtainingDifficulties.NoRequirements
				{ ObjectIndexes.BrokenCD.GetId(), new TrashItem(ObjectIndexes.BrokenCD) { OverrideName = "Broken CD" } },
				{ ObjectIndexes.SoggyNewspaper.GetId(), new TrashItem(ObjectIndexes.SoggyNewspaper) },
				{ ObjectIndexes.Driftwood.GetId(), new TrashItem(ObjectIndexes.Driftwood) },
				{ ObjectIndexes.BrokenGlasses.GetId(), new TrashItem(ObjectIndexes.BrokenGlasses) },
				{ ObjectIndexes.JojaCola.GetId(), new TrashItem(ObjectIndexes.JojaCola) },
				{ ObjectIndexes.Trash.GetId(), new TrashItem(ObjectIndexes.Trash) },

				// Fruit trees - ObtainingDifficulties.SmallTimeRequirements
				{ ObjectIndexes.CherrySapling.GetId(), new Item(ObjectIndexes.CherrySapling.GetId(), ObtainingDifficulties.NonCraftingItem) },
				{ ObjectIndexes.ApricotSapling.GetId(), new Item(ObjectIndexes.ApricotSapling.GetId(), ObtainingDifficulties.NonCraftingItem) },
				{ ObjectIndexes.OrangeSapling.GetId(), new Item(ObjectIndexes.OrangeSapling.GetId(), ObtainingDifficulties.NonCraftingItem) },
				{ ObjectIndexes.PeachSapling.GetId(), new Item(ObjectIndexes.PeachSapling.GetId(), ObtainingDifficulties.NonCraftingItem) },
				{ ObjectIndexes.PomegranateSapling.GetId(), new Item(ObjectIndexes.PomegranateSapling.GetId(), ObtainingDifficulties.NonCraftingItem) },
				{ ObjectIndexes.AppleSapling.GetId(), new Item(ObjectIndexes.AppleSapling.GetId(), ObtainingDifficulties.NonCraftingItem) },

				// Seeds - ObtainingDifficulties.LargeTimeRequirements
				{ ObjectIndexes.ParsnipSeeds.GetId(), new SeedItem(ObjectIndexes.ParsnipSeeds) },
				{ ObjectIndexes.JazzSeeds.GetId(), new SeedItem(ObjectIndexes.JazzSeeds) },
				{ ObjectIndexes.CauliflowerSeeds.GetId(), new SeedItem(ObjectIndexes.CauliflowerSeeds) },
				{ ObjectIndexes.CoffeeBean.GetId(), new SeedItem(ObjectIndexes.CoffeeBean) { Randomize = false } },
				{ ObjectIndexes.GarlicSeeds.GetId(), new SeedItem(ObjectIndexes.GarlicSeeds) },
				{ ObjectIndexes.BeanStarter.GetId(), new SeedItem(ObjectIndexes.BeanStarter) },
				{ ObjectIndexes.KaleSeeds.GetId(), new SeedItem(ObjectIndexes.KaleSeeds) },
				{ ObjectIndexes.PotatoSeeds.GetId(), new SeedItem(ObjectIndexes.PotatoSeeds) },
				{ ObjectIndexes.RhubarbSeeds.GetId(), new SeedItem(ObjectIndexes.RhubarbSeeds) },
				{ ObjectIndexes.StrawberrySeeds.GetId(), new SeedItem(ObjectIndexes.StrawberrySeeds) },
				{ ObjectIndexes.TulipBulb.GetId(), new SeedItem(ObjectIndexes.TulipBulb) },
				{ ObjectIndexes.RiceShoot.GetId(), new SeedItem(ObjectIndexes.RiceShoot) },
				{ ObjectIndexes.BlueberrySeeds.GetId(), new SeedItem(ObjectIndexes.BlueberrySeeds) },
				{ ObjectIndexes.CornSeeds.GetId(), new SeedItem(ObjectIndexes.CornSeeds) },
				{ ObjectIndexes.HopsStarter.GetId(), new SeedItem(ObjectIndexes.HopsStarter) },
				{ ObjectIndexes.PepperSeeds.GetId(), new SeedItem(ObjectIndexes.PepperSeeds) },
				{ ObjectIndexes.MelonSeeds.GetId(), new SeedItem(ObjectIndexes.MelonSeeds) },
				{ ObjectIndexes.PoppySeeds.GetId(), new SeedItem(ObjectIndexes.PoppySeeds) },
				{ ObjectIndexes.RadishSeeds.GetId(), new SeedItem(ObjectIndexes.RadishSeeds) },
				{ ObjectIndexes.RedCabbageSeeds.GetId(), new SeedItem(ObjectIndexes.RedCabbageSeeds) },
				{ ObjectIndexes.StarfruitSeeds.GetId(), new SeedItem(ObjectIndexes.StarfruitSeeds) },
				{ ObjectIndexes.SpangleSeeds.GetId(), new SeedItem(ObjectIndexes.SpangleSeeds) },
				{ ObjectIndexes.SunflowerSeeds.GetId(), new SeedItem(ObjectIndexes.SunflowerSeeds) },
				{ ObjectIndexes.TomatoSeeds.GetId(), new SeedItem(ObjectIndexes.TomatoSeeds) },
				{ ObjectIndexes.WheatSeeds.GetId(), new SeedItem(ObjectIndexes.WheatSeeds) },
				{ ObjectIndexes.AmaranthSeeds.GetId(), new SeedItem(ObjectIndexes.AmaranthSeeds) },
				{ ObjectIndexes.ArtichokeSeeds.GetId(), new SeedItem(ObjectIndexes.ArtichokeSeeds) },
				{ ObjectIndexes.BeetSeeds.GetId(), new SeedItem(ObjectIndexes.BeetSeeds) },
				{ ObjectIndexes.BokChoySeeds.GetId(), new SeedItem(ObjectIndexes.BokChoySeeds) },
				{ ObjectIndexes.CranberrySeeds.GetId(), new SeedItem(ObjectIndexes.CranberrySeeds) },
				{ ObjectIndexes.EggplantSeeds.GetId(), new SeedItem(ObjectIndexes.EggplantSeeds) },
				{ ObjectIndexes.FairySeeds.GetId(), new SeedItem(ObjectIndexes.FairySeeds) },
				{ ObjectIndexes.GrapeStarter.GetId(), new SeedItem(ObjectIndexes.GrapeStarter) },
				{ ObjectIndexes.PumpkinSeeds.GetId(), new SeedItem(ObjectIndexes.PumpkinSeeds) },
				{ ObjectIndexes.YamSeeds.GetId(), new SeedItem(ObjectIndexes.YamSeeds) },
				{ ObjectIndexes.AncientSeeds.GetId(), new SeedItem(ObjectIndexes.AncientSeeds) { Randomize = false, DifficultyToObtain = ObtainingDifficulties.EndgameItem } },
				{ ObjectIndexes.CactusSeeds.GetId(), new SeedItem(ObjectIndexes.CactusSeeds) },
				{ ObjectIndexes.RareSeed.GetId(), new SeedItem(ObjectIndexes.RareSeed) { DifficultyToObtain = ObtainingDifficulties.RareItem } },
				{ ObjectIndexes.CarrotSeeds.GetId(), new SeedItem(ObjectIndexes.CarrotSeeds) { ShuffleBetweenSeeds = false, DifficultyToObtain = ObtainingDifficulties.RareItem } },
				{ ObjectIndexes.SummerSquashSeeds.GetId(), new SeedItem(ObjectIndexes.SummerSquashSeeds) { ShuffleBetweenSeeds = false, DifficultyToObtain = ObtainingDifficulties.RareItem } },
				{ ObjectIndexes.BroccoliSeeds.GetId(), new SeedItem(ObjectIndexes.BroccoliSeeds) { ShuffleBetweenSeeds = false, DifficultyToObtain = ObtainingDifficulties.RareItem } },
				{ ObjectIndexes.PowdermelonSeeds.GetId(), new SeedItem(ObjectIndexes.PowdermelonSeeds) { ShuffleBetweenSeeds = false, DifficultyToObtain = ObtainingDifficulties.RareItem } },

				// Crops - ObtainingDifficulties.LargeTimeRequirements
				{ ObjectIndexes.Parsnip.GetId(), new CropItem(ObjectIndexes.Parsnip) },
				{ ObjectIndexes.BlueJazz.GetId(), new CropItem(ObjectIndexes.BlueJazz) },
				{ ObjectIndexes.Cauliflower.GetId(), new CropItem(ObjectIndexes.Cauliflower) },
				{ ObjectIndexes.Garlic.GetId(), new CropItem(ObjectIndexes.Garlic) },
				{ ObjectIndexes.GreenBean.GetId(), new CropItem(ObjectIndexes.GreenBean) },
				{ ObjectIndexes.Kale.GetId(), new CropItem(ObjectIndexes.Kale) },
				{ ObjectIndexes.Potato.GetId(), new CropItem(ObjectIndexes.Potato) },
				{ ObjectIndexes.Rhubarb.GetId(), new CropItem(ObjectIndexes.Rhubarb) },
				{ ObjectIndexes.Strawberry.GetId(), new CropItem(ObjectIndexes.Strawberry) },
				{ ObjectIndexes.Tulip.GetId(), new CropItem(ObjectIndexes.Tulip) },
				{ ObjectIndexes.Blueberry.GetId(), new CropItem(ObjectIndexes.Blueberry) },
				{ ObjectIndexes.Corn.GetId(), new CropItem(ObjectIndexes.Corn) },
				{ ObjectIndexes.Hops.GetId(), new CropItem(ObjectIndexes.Hops) },
				{ ObjectIndexes.HotPepper.GetId(), new CropItem(ObjectIndexes.HotPepper) },
				{ ObjectIndexes.Melon.GetId(), new CropItem(ObjectIndexes.Melon) },
				{ ObjectIndexes.Poppy.GetId(), new CropItem(ObjectIndexes.Poppy) },
				{ ObjectIndexes.Radish.GetId(), new CropItem(ObjectIndexes.Radish) },
				{ ObjectIndexes.RedCabbage.GetId(), new CropItem(ObjectIndexes.RedCabbage) },
				{ ObjectIndexes.Starfruit.GetId(), new CropItem(ObjectIndexes.Starfruit) },
				{ ObjectIndexes.SummerSpangle.GetId(), new CropItem(ObjectIndexes.SummerSpangle) },
				{ ObjectIndexes.Sunflower.GetId(), new CropItem(ObjectIndexes.Sunflower) },
				{ ObjectIndexes.Tomato.GetId(), new CropItem(ObjectIndexes.Tomato) },
				{ ObjectIndexes.Wheat.GetId(), new CropItem(ObjectIndexes.Wheat) },
				{ ObjectIndexes.Amaranth.GetId(), new CropItem(ObjectIndexes.Amaranth) },
				{ ObjectIndexes.Artichoke.GetId(), new CropItem(ObjectIndexes.Artichoke) },
				{ ObjectIndexes.Beet.GetId(), new CropItem(ObjectIndexes.Beet) },
				{ ObjectIndexes.BokChoy.GetId(), new CropItem(ObjectIndexes.BokChoy) },
				{ ObjectIndexes.Cranberries.GetId(), new CropItem(ObjectIndexes.Cranberries) },
				{ ObjectIndexes.Eggplant.GetId(), new CropItem(ObjectIndexes.Eggplant) },
				{ ObjectIndexes.FairyRose.GetId(), new CropItem(ObjectIndexes.FairyRose) },
				{ ObjectIndexes.Pumpkin.GetId(), new CropItem(ObjectIndexes.Pumpkin) },
				{ ObjectIndexes.Yam.GetId(), new CropItem(ObjectIndexes.Yam) },
				{ ObjectIndexes.AncientFruit.GetId(), new CropItem(ObjectIndexes.AncientFruit) },
				{ ObjectIndexes.SweetGemBerry.GetId(), new CropItem(ObjectIndexes.SweetGemBerry) },
				{ ObjectIndexes.UnmilledRice.GetId(), new CropItem(ObjectIndexes.UnmilledRice) },
				{ ObjectIndexes.Carrot.GetId(), new CropItem(ObjectIndexes.Carrot) { DifficultyToObtain = ObtainingDifficulties.RareItem } },
				{ ObjectIndexes.SummerSquash.GetId(), new CropItem(ObjectIndexes.SummerSquash) { DifficultyToObtain = ObtainingDifficulties.RareItem } },
				{ ObjectIndexes.Broccoli.GetId(), new CropItem(ObjectIndexes.Broccoli) { DifficultyToObtain = ObtainingDifficulties.RareItem } },
				{ ObjectIndexes.Powdermelon.GetId(), new CropItem(ObjectIndexes.Powdermelon) { DifficultyToObtain = ObtainingDifficulties.RareItem } }
			};
			BigCraftableItems = new Dictionary<string, Item>()
			{
				{ BigCraftableIndexes.Chest.GetId(), new CraftableItem(BigCraftableIndexes.Chest, CraftableCategories.Easy) },
                { BigCraftableIndexes.BigChest.GetId(), new CraftableItem(BigCraftableIndexes.BigChest, CraftableCategories.Moderate) },
                { BigCraftableIndexes.StoneChest.GetId(), new CraftableItem(BigCraftableIndexes.StoneChest, CraftableCategories.Easy) },
                { BigCraftableIndexes.BigStoneChest.GetId(), new CraftableItem(BigCraftableIndexes.BigStoneChest, CraftableCategories.Moderate) },
                { BigCraftableIndexes.Scarecrow.GetId(), new CraftableItem(BigCraftableIndexes.Scarecrow, CraftableCategories.Moderate) },
				{ BigCraftableIndexes.BeeHouse.GetId(), new CraftableItem(BigCraftableIndexes.BeeHouse, CraftableCategories.Moderate) },
				{ BigCraftableIndexes.Keg.GetId(), new CraftableItem(BigCraftableIndexes.Keg, CraftableCategories.Moderate) },
				{ BigCraftableIndexes.Cask.GetId(), new CraftableItem(BigCraftableIndexes.Cask, CraftableCategories.Moderate) },
				{ BigCraftableIndexes.Furnace.GetId(), new CraftableItem(BigCraftableIndexes.Furnace, CraftableCategories.Moderate) },
                { BigCraftableIndexes.HeavyFurnace.GetId(), new CraftableItem(BigCraftableIndexes.HeavyFurnace, CraftableCategories.Difficult) },
                { BigCraftableIndexes.Anvil.GetId(), new CraftableItem(BigCraftableIndexes.Anvil, CraftableCategories.Difficult) },
                { BigCraftableIndexes.MiniForge.GetId(), new CraftableItem(BigCraftableIndexes.MiniForge, CraftableCategories.Endgame, dataKey: "Mini-Forge") },
                { BigCraftableIndexes.GardenPot.GetId(), new CraftableItem(BigCraftableIndexes.GardenPot, CraftableCategories.Easy) },
                { BigCraftableIndexes.TextSign.GetId(), new CraftableItem(BigCraftableIndexes.TextSign, CraftableCategories.Easy) },
                { BigCraftableIndexes.WoodSign.GetId(), new CraftableItem(BigCraftableIndexes.WoodSign, CraftableCategories.Easy) },
                { BigCraftableIndexes.StoneSign.GetId(), new CraftableItem(BigCraftableIndexes.StoneSign, CraftableCategories.Easy) },
                { BigCraftableIndexes.DarkSign.GetId(), new CraftableItem(BigCraftableIndexes.DarkSign, CraftableCategories.Moderate) },
                { BigCraftableIndexes.CheesePress.GetId(), new CraftableItem(BigCraftableIndexes.CheesePress, CraftableCategories.Moderate) },
				{ BigCraftableIndexes.MayonnaiseMachine.GetId(), new CraftableItem(BigCraftableIndexes.MayonnaiseMachine, CraftableCategories.Moderate) },
				{ BigCraftableIndexes.SeedMaker.GetId(), new CraftableItem(BigCraftableIndexes.SeedMaker, CraftableCategories.Moderate) },
				{ BigCraftableIndexes.Loom.GetId(), new CraftableItem(BigCraftableIndexes.Loom, CraftableCategories.Moderate) },
				{ BigCraftableIndexes.OilMaker.GetId(), new CraftableItem(BigCraftableIndexes.OilMaker, CraftableCategories.Moderate) },
				{ BigCraftableIndexes.RecyclingMachine.GetId(), new CraftableItem(BigCraftableIndexes.RecyclingMachine, CraftableCategories.Moderate) },
                { BigCraftableIndexes.BaitMaker.GetId(), new CraftableItem(BigCraftableIndexes.BaitMaker, CraftableCategories.Moderate) },
                { BigCraftableIndexes.FishSmoker.GetId(), new CraftableItem(BigCraftableIndexes.FishSmoker, CraftableCategories.Moderate) },
                { BigCraftableIndexes.MushroomLog.GetId(), new CraftableItem(BigCraftableIndexes.MushroomLog, CraftableCategories.Moderate) },
                { BigCraftableIndexes.WormBin.GetId(), new CraftableItem(BigCraftableIndexes.WormBin, CraftableCategories.Moderate) },
                { BigCraftableIndexes.DeluxeWormBin.GetId(), new CraftableItem(BigCraftableIndexes.DeluxeWormBin, CraftableCategories.Difficult) },
                { BigCraftableIndexes.PreservesJar.GetId(), new CraftableItem(BigCraftableIndexes.PreservesJar, CraftableCategories.Moderate) },
				{ BigCraftableIndexes.CharcoalKiln.GetId(), new CraftableItem(BigCraftableIndexes.CharcoalKiln, CraftableCategories.Easy) },
				{ BigCraftableIndexes.Tapper.GetId(), new CraftableItem(BigCraftableIndexes.Tapper, CraftableCategories.Moderate) },
				{ BigCraftableIndexes.LightningRod.GetId(), new CraftableItem(BigCraftableIndexes.LightningRod, CraftableCategories.Moderate) },
				{ BigCraftableIndexes.SlimeIncubator.GetId(), new CraftableItem(BigCraftableIndexes.SlimeIncubator, CraftableCategories.Difficult) },
				{ BigCraftableIndexes.SlimeEggPress.GetId(), new CraftableItem(BigCraftableIndexes.SlimeEggPress, CraftableCategories.DifficultAndNeedMany, dataKey: "Slime Egg-Press") },
				{ BigCraftableIndexes.Crystalarium.GetId(), new CraftableItem(BigCraftableIndexes.Crystalarium, CraftableCategories.Moderate) },
				{ BigCraftableIndexes.MiniJukebox.GetId(), new CraftableItem(BigCraftableIndexes.MiniJukebox, CraftableCategories.Moderate, dataKey: "Mini-Jukebox") },
				{ BigCraftableIndexes.Staircase.GetId(), new CraftableItem(BigCraftableIndexes.Staircase, CraftableCategories.Moderate) },
				{ BigCraftableIndexes.BoneMill.GetId(), new CraftableItem(BigCraftableIndexes.BoneMill, CraftableCategories.Moderate) },
				{ BigCraftableIndexes.TubOFlowers.GetId(), new CraftableItem(BigCraftableIndexes.TubOFlowers, CraftableCategories.Easy, dataKey: "Tub o' Flowers") },
				{ BigCraftableIndexes.WoodenBrazier.GetId(), new CraftableItem(BigCraftableIndexes.WoodenBrazier, CraftableCategories.Easy) },
				{ BigCraftableIndexes.WickedStatue.GetId(), new CraftableItem(BigCraftableIndexes.WickedStatue, CraftableCategories.Easy) },
				{ BigCraftableIndexes.StoneBrazier.GetId(), new CraftableItem(BigCraftableIndexes.StoneBrazier, CraftableCategories.Easy) },
				{ BigCraftableIndexes.GoldBrazier.GetId(), new CraftableItem(BigCraftableIndexes.GoldBrazier, CraftableCategories.Moderate) },
				{ BigCraftableIndexes.Campfire.GetId(), new CraftableItem(BigCraftableIndexes.Campfire, CraftableCategories.Easy) },
				{ BigCraftableIndexes.StumpBrazier.GetId(), new CraftableItem(BigCraftableIndexes.StumpBrazier, CraftableCategories.Moderate) },
				{ BigCraftableIndexes.CarvedBrazier.GetId(), new CraftableItem(BigCraftableIndexes.CarvedBrazier, CraftableCategories.Moderate) },
				{ BigCraftableIndexes.SkullBrazier.GetId(), new CraftableItem(BigCraftableIndexes.SkullBrazier, CraftableCategories.Moderate) },
				{ BigCraftableIndexes.BarrelBrazier.GetId(), new CraftableItem(BigCraftableIndexes.BarrelBrazier, CraftableCategories.Moderate) },
				{ BigCraftableIndexes.MarbleBrazier.GetId(), new CraftableItem(BigCraftableIndexes.MarbleBrazier, CraftableCategories.Difficult) },
				{ BigCraftableIndexes.WoodLampPost.GetId(), new CraftableItem(BigCraftableIndexes.WoodLampPost, CraftableCategories.Moderate, dataKey : "Wood Lamp-post") },
				{ BigCraftableIndexes.IronLampPost.GetId(), new CraftableItem(BigCraftableIndexes.IronLampPost, CraftableCategories.Moderate, dataKey : "Iron Lamp-post") },
                { BigCraftableIndexes.DeluxeScarecrow.GetId(), new CraftableItem(BigCraftableIndexes.DeluxeScarecrow, CraftableCategories.Endgame) },
                { BigCraftableIndexes.GeodeCrusher.GetId(), new CraftableItem(BigCraftableIndexes.GeodeCrusher, CraftableCategories.Moderate) },
                { BigCraftableIndexes.SolarPanel.GetId(), new CraftableItem(BigCraftableIndexes.SolarPanel, CraftableCategories.Moderate) },
                { BigCraftableIndexes.MiniObelisk.GetId(), new CraftableItem(BigCraftableIndexes.MiniObelisk, CraftableCategories.Difficult, dataKey: "Mini-Obelisk") },
                { BigCraftableIndexes.FarmComputer.GetId(), new CraftableItem(BigCraftableIndexes.FarmComputer, CraftableCategories.Difficult) },
                { BigCraftableIndexes.Dehydrator.GetId(), new CraftableItem(BigCraftableIndexes.Dehydrator, CraftableCategories.Moderate) },
                { BigCraftableIndexes.StatueOfBlessings.GetId(), new CraftableItem(BigCraftableIndexes.StatueOfBlessings, CraftableCategories.Endgame) },
                { BigCraftableIndexes.StatueOfTheDwarfKing.GetId(), new CraftableItem(BigCraftableIndexes.StatueOfTheDwarfKing, CraftableCategories.Endgame) },

				// Non-craftable BigObjects
				{ BigCraftableIndexes.Heater.GetId(), new Item(BigCraftableIndexes.Heater.GetId(), ObtainingDifficulties.NonCraftingItem) },
				{ BigCraftableIndexes.AutoGrabber.GetId(), new Item(BigCraftableIndexes.AutoGrabber.GetId(), ObtainingDifficulties.NonCraftingItem) },
				{ BigCraftableIndexes.PrairieKingArcadeSystem.GetId(), new Item(BigCraftableIndexes.PrairieKingArcadeSystem.GetId(), ObtainingDifficulties.NonCraftingItem) },
				{ BigCraftableIndexes.JunimoKartArcadeSystem.GetId(), new Item(BigCraftableIndexes.JunimoKartArcadeSystem.GetId(), ObtainingDifficulties.NonCraftingItem) },
				{ BigCraftableIndexes.SodaMachine.GetId(), new Item(BigCraftableIndexes.SodaMachine.GetId(), ObtainingDifficulties.NonCraftingItem) },
				{ BigCraftableIndexes.HMTGF.GetId(), new Item(BigCraftableIndexes.HMTGF.GetId(), ObtainingDifficulties.NonCraftingItem) { OverrideName = "??HMTGF??" } },
				{ BigCraftableIndexes.PinkyLemon.GetId(), new Item(BigCraftableIndexes.PinkyLemon.GetId(), ObtainingDifficulties.NonCraftingItem) { OverrideName = "??Pinky Lemon??" } },
				{ BigCraftableIndexes.Foroguemon.GetId(), new Item(BigCraftableIndexes.Foroguemon.GetId(), ObtainingDifficulties.NonCraftingItem) { OverrideName = "??Foroguemon??" } },
				{ BigCraftableIndexes.SolidGoldLewis.GetId(), new Item(BigCraftableIndexes.SolidGoldLewis.GetId(), ObtainingDifficulties.NonCraftingItem) },
				{ BigCraftableIndexes.StardewHeroTrophy.GetId(), new Item(BigCraftableIndexes.StardewHeroTrophy.GetId(), ObtainingDifficulties.NonCraftingItem) }
			};

			// Fill out the default fish info based off Data/Fish
			Items.Values.Where(item => item is FishItem)
				.Cast<FishItem>()
				.ToList()
				.ForEach(fishItem => FishData.FillDefaultFishInfo(fishItem));
		}
    }
}
