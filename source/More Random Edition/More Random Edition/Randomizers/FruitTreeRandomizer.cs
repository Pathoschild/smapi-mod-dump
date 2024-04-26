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
using StardewValley.GameData.FruitTrees;
using StardewValley.GameData.Objects;
using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
    /// <summary>
    /// Randomizes what fruit trees go
    /// 
    /// For testing, use debug command "fruittrees":
    /// - Instantly grows all fruit trees
    /// - Trees already grown will bear a new fruit if the current season is valid
    /// </summary>
    public class FruitTreeRandomizer
    {
        private static RNG Rng { get; set; }

        const int NumberOfRandomFruitTreeCategoryTrees = 2;
        const int NumberOfRandomFruitTreeItemTrees = 2;

        /// <summary>
        /// The list of ids of all randomized fruit tree ids
        /// </summary>
        public static readonly List<string> RandomizedFruitTreeIds = new()
        {
            ObjectIndexes.CherrySapling.GetId(),
            ObjectIndexes.ApricotSapling.GetId(),
            ObjectIndexes.OrangeSapling.GetId(),
            ObjectIndexes.PeachSapling.GetId(),
            ObjectIndexes.PomegranateSapling.GetId(),
            ObjectIndexes.AppleSapling.GetId()
        };

        /// <summary>
        /// Randomize fruit tree information - sets up trees as follows:
        /// - 2 trees of a random category (like fish, ores, etc)
        /// - 1 tree that grows random (O) items
        /// - 1 tree that grows random (BC) items
        /// - 2 trees that grow a specific random item
        /// </summary>
        /// <param name="objectReplacements">The object information - for fruit tree names and prices</param>
        public static Dictionary<string, FruitTreeData> Randomize(
            Dictionary<string, ObjectData> objectReplacements)
        {
            Dictionary<string, FruitTreeData> fruitTreeReplacements = new();

            if (!Globals.Config.RandomizeFruitTrees)
            {
                return fruitTreeReplacements;
            }

            Rng = RNG.GetFarmRNG(nameof(FruitTreeRandomizer));

            List<Seasons> startingSeasons = GetStartingSeasonList();

            List<ItemCategories> fruitTreeCategories = 
                CategoryExtentions.GetRandomCategories(Rng, NumberOfRandomFruitTreeCategoryTrees);
            List<Item> fruitTreeSingleItems = GetListOfRandomFruitTreeItems();

            CreateFruitTreeWithCategory(
                RandomizedFruitTreeIds[0],
                fruitTreeCategories[0],
                startingSeasons[0],
                fruitTreeReplacements,
                objectReplacements);

            CreateFruitTreeWithCategory(
                RandomizedFruitTreeIds[1],
                fruitTreeCategories[1],
                startingSeasons[1],
                fruitTreeReplacements,
                objectReplacements);

            CreateFruitTreeWithRandomItem(
                RandomizedFruitTreeIds[2],
                true,
                startingSeasons[2],
                fruitTreeReplacements,
                objectReplacements);

            CreateFruitTreeWithRandomItem(
                RandomizedFruitTreeIds[3],
                false,
                startingSeasons[3],
                fruitTreeReplacements,
                objectReplacements);

            CreateFruitTreeWithSingleRandomItem(
                RandomizedFruitTreeIds[4],
                fruitTreeSingleItems[0],
                startingSeasons[4],
                fruitTreeReplacements,
                objectReplacements);

            CreateFruitTreeWithSingleRandomItem(
                RandomizedFruitTreeIds[5],
                fruitTreeSingleItems[1],
                startingSeasons[5],
                fruitTreeReplacements,
                objectReplacements);

            return fruitTreeReplacements;
        }

        /// <summary>
        /// Gets the starting list of seasons - consists of a list of seasons for every
        /// randomized fruit tree, guaranteed to contain at least one of each season
        /// </summary>
        /// <returns>The list of seasons</returns>
        private static List<Seasons> GetStartingSeasonList()
        {
            List<Seasons> seasonList = new()
            {
                Seasons.Spring,
                Seasons.Summer,
                Seasons.Fall,
                Seasons.Winter
            };

            // Add any additional seasons
            while(seasonList.Count < RandomizedFruitTreeIds.Count)
            {
                seasonList.Add(SeasonsExtensions.GetRandomSeason(Rng));
            }

            return seasonList;
        }
            
        /// <summary>
        /// Gets the list of items for random fruit trees
        /// </summary>
        /// <returns>The list of random fruit tree items</returns>
        private static List<Item> GetListOfRandomFruitTreeItems()
        {
            List<Item> allPotentialTreesItems = ItemList.Items.Values.Where(x =>
                RandomizedFruitTreeIds.Contains(x.Id) || x.DifficultyToObtain < ObtainingDifficulties.Impossible
            ).ToList();

            return Rng.GetRandomValuesFromList(
                allPotentialTreesItems, 
                NumberOfRandomFruitTreeItemTrees);
        }

        /// <summary>
        /// Creates a random fruit tree with the given category
        /// </summary>
        /// <param name="fruitTreeId">The id of the tree to replace</param>
        /// <param name="category">The category for the tree to grow</param>
        /// <param name="guaranteedSeason">A guaranteed season that fruit should be produced</param>
        /// <param name="fruitTreeReplacements">The dictionary of fruit tree replacements</param>
        /// <param name="objectReplacements">The dictionary of object replacements</param>
        private static void CreateFruitTreeWithCategory(
            string fruitTreeId, 
            ItemCategories category, 
            Seasons guaranteedSeason,
            Dictionary<string, FruitTreeData> fruitTreeReplacements,
            Dictionary<string, ObjectData> objectReplacements)
        {
            FruitTreeData fruitTreeToModify = Game1.fruitTreeData[fruitTreeId];
            fruitTreeToModify.Seasons = GetRandomSeasonsList(guaranteedSeason);

            // We currently just modify the existing one fruit
            FruitTreeFruitData fruitToModify = fruitTreeToModify.Fruit[0];
            fruitToModify.ItemId = "RANDOM_ITEMS (O)";
            fruitToModify.PerItemCondition = $"ITEM_CATEGORY Target {(int)category}";

            string categoryDisplayName = category.GetTranslation();
            string categoryTreeName = Globals.GetTranslation(
                "sapling-text", 
                new { itemName = categoryDisplayName });

            AddToReplacementDictionaries(
                fruitTreeId, 
                fruitTreeToModify,
                categoryDisplayName,
                categoryTreeName,
                fruitTreeReplacements,
                objectReplacements);
        }

        /// <summary>
        /// Creates a random fruit tree that grows random items
        /// - currently grows either Object, or Big Craftables
        /// </summary>
        /// <param name="fruitTreeId">The id of the tree to replace</param>
        /// <param name="isNormalObject">True if it grows (O) items, false if (BC) items</param>
        /// <param name="guaranteedSeason">A guaranteed season that fruit should be produced</param>
        /// <param name="fruitTreeReplacements">The dictionary of fruit tree replacements</param>
        /// <param name="objectReplacements">The dictionary of object replacements</param>
        private static void CreateFruitTreeWithRandomItem(
            string fruitTreeId,
            bool isNormalObject,
            Seasons guaranteedSeason,
            Dictionary<string, FruitTreeData> fruitTreeReplacements,
            Dictionary<string, ObjectData> objectReplacements)
        {
            FruitTreeData fruitTreeToModify = Game1.fruitTreeData[fruitTreeId.ToString()];
            fruitTreeToModify.Seasons = GetRandomSeasonsList(guaranteedSeason);

            string itemType = isNormalObject ? "(O)" : "(BC)";
            string itemTypeDisplayName = isNormalObject ? "\"O\"" : "\"BC\"";

            // We currently just modify the existing one fruit
            FruitTreeFruitData fruitToModify = fruitTreeToModify.Fruit[0];
            fruitToModify.ItemId = $"RANDOM_ITEMS {itemType}";

            string randomItemTreeName = Globals.GetTranslation(
                "sapling-text",
                new { itemName = itemTypeDisplayName });

            AddToReplacementDictionaries(
                fruitTreeId,
                fruitTreeToModify,
                itemTypeDisplayName,
                randomItemTreeName,
                fruitTreeReplacements,
                objectReplacements,
                basePrice: isNormalObject ? 5000 : 8000);
        }

        /// <summary>
        /// Creates a random fruit tree that grows a single random item
        /// </summary>
        /// <param name="fruitTreeId">The id of the tree to replace</param>
        /// <param name="randomItem">The itme the tree should grow</param>
        /// <param name="guaranteedSeason">A guaranteed season that fruit should be produced</param>
        /// <param name="fruitTreeReplacements">The dictionary of fruit tree replacements</param>
        /// <param name="objectReplacements">The dictionary of object replacements</param>
        private static void CreateFruitTreeWithSingleRandomItem(
            string fruitTreeId,
            Item randomItem,
            Seasons guaranteedSeason,
            Dictionary<string, FruitTreeData> fruitTreeReplacements,
            Dictionary<string, ObjectData> objectReplacements)
        {
            FruitTreeData fruitTreeToModify = Game1.fruitTreeData[fruitTreeId.ToString()];
            fruitTreeToModify.Seasons = GetRandomSeasonsList(guaranteedSeason);

            // We currently just modify the existing one fruit
            FruitTreeFruitData fruitToModify = fruitTreeToModify.Fruit[0];
            fruitToModify.ItemId = randomItem.QualifiedId;

            string singleRandomItemTreeName = fruitTreeId == randomItem.Id
                ? Globals.GetTranslation("item-recursion-sapling-name")
                : Globals.GetTranslation("sapling-text", new { itemName = randomItem.DisplayName });

            // The fruit tree will be worth a base cost of 10 times the sale price
            // We divide it by two because "salePrice" is the SHOP sale price, not our sale price
            // Use 5000 if there is no sale price
            int defaultPrice = (randomItem.GetSaliableObject().salePrice() / 2) * 10;
            int basePrice = defaultPrice > 0 ? defaultPrice : 5000;

            AddToReplacementDictionaries(
                fruitTreeId,
                fruitTreeToModify,
                randomItem.DisplayName,
                singleRandomItemTreeName,
                fruitTreeReplacements,
                objectReplacements,
                basePrice);
        }

        /// <summary>
        /// Adds additional seasons to the list
        /// Currently will add 2 additional seasons to the given one, and removes generated dups
        /// </summary>
        /// <param name="startingSeason">The seasons the tree is guaranteed to have already</param>
        /// <returns>The list of seasons</returns>
        private static List<Season> GetRandomSeasonsList(Seasons startingSeason)
        {
            List<Seasons> seasonList = new()
            {
                startingSeason,
                SeasonsExtensions.GetRandomSeason(Rng),
                SeasonsExtensions.GetRandomSeason(Rng)
            };

            return seasonList
                .Distinct()
                .OrderBy(season => (int)season)
                .Cast<Season>()
                .ToList();
        }

        /// <summary>
        /// Gets the price for a fruit tree
        /// - Trees start at a base price of 5000, or whatever the item should be worth
        /// - Increased by 5% per season after the first one
        /// - 10% variance
        /// </summary>
        /// <returns>The price of the fruit tree</returns>
        private static int GetPriceForFruitTree(FruitTreeData fruitTree, double basePrice)
        {
            double percentageIncrease = (fruitTree.Seasons.Count - 1) * 5;
            double basePriceToUse = basePrice * (1 + (percentageIncrease / 100));
            return Rng.NextIntWithinPercentage((int)basePriceToUse, 10);
        }

        /// <summary>
        /// Adds the fruit tree into to the replacement dictionaries so that
        /// they are actually modified in-game
        /// </summary>
        /// <param name="fruitTreeId">The fruit tree id that is modified</param>
        /// <param name="modifiedFruitTree">The modified fruit tree data</param>
        /// <param name="saplingItemDisplayName">The display name of the item the fruit tree grows</param>
        /// <param name="saplingDisplayName">The display name of the fruit tree</param>
        /// <param name="fruitTreeReplacements">The dictionary of fruit tree replacements</param>
        /// <param name="objectReplacements">The dictionary of object replacements</param>
        /// <param name="basePrice">The base price for the tree - defaults to 5000</param>
        private static void AddToReplacementDictionaries(
            string fruitTreeId,
            FruitTreeData modifiedFruitTree,
            string saplingItemDisplayName,
            string saplingDisplayName,
            Dictionary<string, FruitTreeData> fruitTreeReplacements,
            Dictionary<string, ObjectData> objectReplacements,
            int basePrice = 5000)
        {
            int fruitTreePrice = GetPriceForFruitTree(modifiedFruitTree, basePrice) / 2;

            fruitTreeReplacements[fruitTreeId] = modifiedFruitTree;

            // Replace the fruit tree name/price/description
            ObjectData fruitTreeObject = EditedObjects.DefaultObjectInformation[fruitTreeId];
            fruitTreeObject.Price = fruitTreePrice;
            fruitTreeObject.DisplayName = saplingDisplayName;
            fruitTreeObject.Description = Globals.GetTranslation(
                "sapling-description",
                new {
                    itemName = saplingItemDisplayName, 
                    season = string.Join(", ", modifiedFruitTree.Seasons.Select(s => s.ToString()))
                });
            objectReplacements[fruitTreeId.ToString()] = fruitTreeObject;
        }
    }
}
