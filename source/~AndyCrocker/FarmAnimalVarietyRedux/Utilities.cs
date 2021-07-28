/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using FarmAnimalVarietyRedux.Models;
using FarmAnimalVarietyRedux.Models.Converted;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FarmAnimalVarietyRedux
{
    /// <summary>Contains miscellaneous helper methods.</summary>
    public static class Utilities
    {
        /*********
        ** Public Methods
        *********/
        /// <summary>Constructs a building string.</summary>
        public static string ConstructBuildingString(List<string> buildingNames)
        {
            if (buildingNames == null || buildingNames.Count == 0)
                return "";

            if (buildingNames.Count == 1)
                return buildingNames[0];

            if (buildingNames.Count == 2)
                return $"{buildingNames[0]} or {buildingNames[1]}";

            var copiedBuildingNames = buildingNames.Take(buildingNames.Count - 1).ToList();
            copiedBuildingNames.Add($"or {buildingNames.Last()}");
            return string.Join(", ", copiedBuildingNames);
        }

        /// <summary>Converts a default animal to a custom animal.</summary>
        /// <param name="animal">The animal to convert.</param>
        public static void ConvertDefaultAnimalToCustomAnimal(ref FarmAnimal animal)
        {
            if (animal == null)
                return;

            // ensure animal is actually default
            if (animal.type.Value.Contains('.'))
                return;

            if (animal.modData.TryGetValue($"{ModEntry.Instance.ModManifest.UniqueID}/type", out var parsedType))
            {
                // ensure type exists before setting it to the animal animal type
                if (ModEntry.Instance.Api.GetAnimalSubtypeByInternalName(parsedType) != null)
                    animal.type.Value = parsedType;
                else
                    ModEntry.Instance.Monitor.Log($"An animal with the internal type of {parsedType} was found in the save file but no animal with that type was loaded. Animal will be loaded as a white chicken.", LogLevel.Warn);
            }

            // ensure the animal is a valid internal name
            if (!animal.type.Value.Contains('.'))
                animal.type.Value = $"game.{animal.type.Value}";
        }

        /// <summary>Converts a custom animal to a default animal.</summary>
        /// <param name="animal">The animal to convert.</param>
        public static void ConvertCustomAnimalToDefaultAnimal(ref FarmAnimal animal)
        {
            if (animal == null)
                return;

            // ensure animal is actually custom
            if (!animal.type.Value.Contains('.'))
                return;

            // don't override the type if one exists, this will result in custom animals from uninstalled packs being converted to chickens in a way that's not reversible
            if (!animal.modData.ContainsKey($"{ModEntry.Instance.ModManifest.UniqueID}/type"))
                animal.modData[$"{ModEntry.Instance.ModManifest.UniqueID}/type"] = animal.type;

            animal.type.Value = "White Chicken";
        }

        /// <summary>Retrieves all the pending produce of an animal for a specified harvest type.</summary>
        /// <param name="animal">The animal whose pending produce should be retrieved.</param>
        /// <param name="harvestType">The harvest type the retrieved produce shoudl be.</param>
        /// <param name="harvestableProducePredicate">The predicate to check when filtering on <paramref name="harvestType"/>.</param>
        /// <returns>The pending produce of <paramref name="animal"/> with a harvest type of <paramref name="harvestType"/>.</returns>
        public static Dictionary<AnimalProduce, StardewValley.Object> GetPendingProduceDrops(FarmAnimal animal, HarvestType harvestType, Predicate<AnimalProduce> harvestableProducePredicate = null)
        {
            var pendingProduceDrops = new Dictionary<AnimalProduce, StardewValley.Object>();
            var subtype = ModEntry.Instance.Api.GetAnimalSubtypeByInternalName(animal.type);

            // get all modData products
            var parsedProduces = new List<SavedProduceData>();
            if (animal.modData.TryGetValue($"{ModEntry.Instance.ModManifest.UniqueID}/produces", out var producesString))
                parsedProduces = JsonConvert.DeserializeObject<List<SavedProduceData>>(producesString);

            // get the parsed products that have the specified harvest type, and that is pending to drop (zero 'days till next produce' in modData)
            var harvestTypeParsedProduce = subtype.Produce.Where(product => product.HarvestType == harvestType && (harvestableProducePredicate?.Invoke(product) ?? true));
            var validProduces = Utilities.GetValidAnimalProduce(harvestTypeParsedProduce, animal);
            var readyParsedProduces = parsedProduces
                .Where(product => product.DaysLeft <= 0 && validProduces.Any(animalProduct => animalProduct.UniqueName.ToLower() == product.UniqueName.ToLower()));

            // create objects to produce
            foreach (var readyParsedProduce in readyParsedProduces)
            {
                // try to get the animal produce that matches the current parsed produce
                var animalProduce = harvestTypeParsedProduce.FirstOrDefault(ap => ap.UniqueName.ToLower() == readyParsedProduce.UniqueName.ToLower());
                if (animalProduce == null)
                    continue;

                // determine item to drop (default or upgraded)
                var productId = -1;
                var shouldDropUpgraded = Utilities.ShouldDropUpgradedProduct(animalProduce, animal);
                if (shouldDropUpgraded.HasValue)
                    if (shouldDropUpgraded.Value)
                        productId = animalProduce.UpgradedProductId;
                    else
                        productId = animalProduce.DefaultProductId;

                if (productId == -1)
                    continue;

                // ensure product can be dropped based off of duplicates property
                if (animalProduce.DoNotAllowDuplicates)
                    if (Utilities.IsObjectInPlayerPossession(productId))
                        continue;

                // create object to spawn
                var amount = Utilities.DetermineDropAmount(animalProduce);
                var quality = Utilities.DetermineProductQuality(animal, animalProduce);
                var @object = new StardewValley.Object(productId, amount, quality: quality);
                @object.modData[$"{ModEntry.Instance.ModManifest.UniqueID}/uniqueProduceName"] = animalProduce.UniqueName; // a temp mod data used to easily update the parsed modData
                pendingProduceDrops[animalProduce] = @object;
            }

            return pendingProduceDrops;
        }

        /// <summary>Filters a list of produces to get the ones that are valid for a specified animal to produce.</summary>
        /// <param name="animalProduces">The animal produces to filter for the valid ones.</param>
        /// <param name="animal">The animal to use to determine if a produce is valid.</param>
        /// <returns>The produces that can be dropped by <paramref name="animal"/>.</returns>
        public static IEnumerable<AnimalProduce> GetValidAnimalProduce(IEnumerable<AnimalProduce> animalProduces, FarmAnimal animal)
        {
            var numberOfHearts = animal.friendshipTowardFarmer / 195f;
            foreach (var animalProduce in animalProduces)
                if (animalProduce.Seasons.Any(season => season.ToLower() == Game1.currentSeason.ToLower())
                    && (animal.friendshipTowardFarmer >= animalProduce.DefaultProductMinFriendship && animal.friendshipTowardFarmer <= animalProduce.DefaultProductMaxFriendship)
                        || (animal.friendshipTowardFarmer >= animalProduce.UpgradedProductMinFriendship && animal.friendshipTowardFarmer <= animalProduce.UpgradedProductMaxFriendship))
                {
                    var isGenderValid = (animalProduce.RequiresMale, animal.isMale()) switch
                    {
                        (null, _) => true,
                        (true, true) => true,
                        (false, false) => true,
                        _ => false
                    };

                    var isCoopMasterValid = (animalProduce.RequiresCoopMaster, Game1.player.professions.Contains(Farmer.butcher)) switch // for some reason the CoopMaster profession constant is called Butcher
                    {
                        (null, _) => true,
                        (true, true) => true,
                        (false, false) => true,
                        _ => false
                    };

                    var isShepherdValid = (animalProduce.RequiresShepherd, Game1.player.professions.Contains(Farmer.shepherd)) switch
                    {
                        (null, _) => true,
                        (true, true) => true,
                        (false, false) => true,
                        _ => false
                    };

                    if (isGenderValid && isCoopMasterValid && isShepherdValid)
                        yield return animalProduce;
                }
        }

        /// <summary>Determines if an animal should be able to drop some produce today, based on their happiness and fullness.</summary>
        /// <param name="animal">The animal to determine whether produce should be dropped.</param>
        /// <returns><see langword="true"/> if the animal should drop produce; otherwise, <see langword="false"/>.</returns>
        public static bool ShouldAnimalDropToday(FarmAnimal animal)
        {
            var random = new Random((int)animal.myID / 2 + (int)Game1.stats.DaysPlayed);
            return random.NextDouble() < animal.fullness / 200f && random.NextDouble() < animal.happiness / 70f;
        }

        /// <summary>Determines if the upgraded product in an animal produce should be dropped.</summary>
        /// <param name="produce">The animal produce to use determine whether the upgraded product should be dropped.</param>
        /// <param name="animal">The animal that is producing the product.</param>
        /// <returns><see langword="null"/> if neither the default product or upgraded product could be dropped, <see langword="true"/> if the upgraded product should be dropped; otherwise, <see langword="false"/>.</returns>
        public static bool? ShouldDropUpgradedProduct(AnimalProduce produce, FarmAnimal animal)
        {
            // determine if the upgraded product should be dropped
            var canDropDefaultProduct = produce.DefaultProductId != -1
                && animal.friendshipTowardFarmer >= produce.DefaultProductMinFriendship
                && animal.friendshipTowardFarmer <= produce.DefaultProductMaxFriendship;
            var canDropUpgradedProduct = produce.UpgradedProductId != -1
                && animal.friendshipTowardFarmer >= produce.UpgradedProductMinFriendship
                && animal.friendshipTowardFarmer <= produce.UpgradedProductMaxFriendship;

            // ensure a product can be dropped at all
            if (!canDropDefaultProduct && !canDropUpgradedProduct)
                return null;

            var dropUpgradedProduct = false;
            if (canDropDefaultProduct && canDropUpgradedProduct)
            {
                // determine which product should get dropped
                if (produce.PercentChanceForUpgradedProduct == null)
                {
                    var happinessModifier = 0f;
                    if (animal.happiness > 200)
                        happinessModifier = animal.happiness * 1.5f;
                    else if (animal.happiness >= 100)
                        happinessModifier = animal.happiness - 100;

                    // check if upgraded product is rare, and use the base game algorithm to determine if the rare product should be dropped
                    if (produce.UpgradedProductIsRare)
                        dropUpgradedProduct = Game1.random.NextDouble() < (animal.friendshipTowardFarmer + happinessModifier) / 5000 + Game1.player.team.AverageDailyLuck() + (Game1.player.team.AverageDailyLuck() * .02f);

                    // use the base game algorithm to determine if the upgraded product should get dropped
                    else
                        dropUpgradedProduct = Game1.random.NextDouble() < (animal.friendshipTowardFarmer + happinessModifier) / 1200;
                }
                else
                    dropUpgradedProduct = Game1.random.NextDouble() * 100 + 1 <= produce.PercentChanceForUpgradedProduct;
            }
            else if (canDropUpgradedProduct) // make sure to return true if only the upgraded product is able to be dropped
                dropUpgradedProduct = true;

            return dropUpgradedProduct;
        }

        /// <summary>Gets whether a specified object is in the players possession.</summary>
        /// <param name="objectId">The object to check.</param>
        /// <returns><see langword="true"/>, if the object is in the players possession; otherwise, <see langword="false"/>.</returns>
        /// <remarks>An object is in the players possession if it placed in an animal building or on the farm, in a farmer's inventory, or in a chest.</remarks>
        public static bool IsObjectInPlayerPossession(int objectId)
        {
            // check placed objects and chests/auto grabbers in the animal buildings
            if (Game1.getFarm().buildings
                .Where(building => building.indoors.Value is AnimalHouse)
                .Select(building => building.indoors.Value)
                .Cast<AnimalHouse>()
                .SelectMany(animalHouse => animalHouse.Objects.Values)
                .Any(@object => @object.ParentSheetIndex == objectId
                    || (@object is Chest chest && chest.items.Any(item => item.ParentSheetIndex == objectId))
                    || (@object.heldObject?.Value is Chest heldChest && heldChest.items.Any(item => item.ParentSheetIndex == objectId))))
                return true;

            // check placed objects in the farm
            if (Game1.getFarm().Objects.Values.Any(@object => @object.ParentSheetIndex == objectId))
                return true;

            // check player inventories
            if (Game1.getAllFarmers().Any(farmer => farmer.hasItemInInventory(objectId, 1)))
                return true;

            // check chests
            if (Game1.locations.SelectMany(location => location.Objects.Values)
                    .Where(@object => @object is Chest)
                    .Cast<Chest>()
                    .SelectMany(chest => chest.items)
                    .Any(item => item.ParentSheetIndex == objectId))
                return true;

            return false;
        }

        /// <summary>Determines the quality that the produce should be.</summary>
        /// <param name="animal">The animal that is dropping the product.</param>
        /// <param name="produce">The produce that is being dropped.</param>
        /// <returns>4 when the product should be iridium quality, 2 when the product should be gold quality, 1 when the product should be silver quality, or 0 when the product should be normal quality.</returns>
        public static int DetermineProductQuality(FarmAnimal animal, AnimalProduce produce)
        {
            // check if the product is allowed to have different qualities
            if (produce.StandardQualityOnly)
                return 0;

            // determine base quality chance (using the base game algorithm)
            var qualityChance = animal.friendshipTowardFarmer / 1000f - (1 - animal.happiness / 255f);

            // apply profession bonuses
            if (animal.isCoopDweller() && Game1.getFarmer(animal.ownerID).professions.Contains(Farmer.butcher) // for some reason the CoopMaster profession constant is called Butcher
                || !animal.isCoopDweller() && Game1.getFarmer(animal.ownerID).professions.Contains(Farmer.shepherd))
                qualityChance += .33f;

            // choose quality
            if (qualityChance >= .95f && Game1.random.NextDouble() < qualityChance / 2)
                return 4;
            if (Game1.random.NextDouble() < qualityChance / 2)
                return 2;
            if (Game1.random.NextDouble() < qualityChance)
                return 1;

            return 0;
        }

        /// <summary>Determines the amount of product should be dropped.</summary>
        /// <param name="produce">The produce that is being dropped.</param>
        /// <returns>The stack size of the product to drop.</returns>
        public static int DetermineDropAmount(AnimalProduce produce)
        {
            var amount = produce.Amount;
            if (Game1.random.NextDouble() * 100 + 1 <= produce.PercentChanceForOneExtra)
                amount++;

            return amount;
        }

        /// <summary>Determines the number of days a produce should take to produce.</summary>
        /// <param name="produce">The produce to determine the days to produce for.</param>
        /// <returns>The number of days the product should take to produce.</returns>
        public static int DetermineDaysToProduce(AnimalProduce produce)
        {
            var daysToProduce = produce.DaysToProduce;

            if (produce.ProduceFasterWithCoopMaster && Game1.player.professions.Contains(Farmer.butcher))
                daysToProduce--;
            if (produce.ProduceFasterWithShepherd && Game1.player.professions.Contains(Farmer.shepherd))
                daysToProduce--;

            return Math.Max(0, daysToProduce);
        }
    }
}
