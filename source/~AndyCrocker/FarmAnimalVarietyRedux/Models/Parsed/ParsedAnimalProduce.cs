/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using FarmAnimalVarietyRedux.Models.Converted;

namespace FarmAnimalVarietyRedux.Models.Parsed
{
    /// <summary>Represents an item that an animal can drop.</summary>
    /// <remarks>This is a version of <see cref="AnimalProduce"/> that has <see cref="AnimalProduce.Id"/> as <see langword="string"/>.<br/>The reason this is done is so content packs can have tokens in place of the ids to call mod APIs to get the id (so JsonAsset items can be used for example).</remarks>
    public class ParsedAnimalProduce
    {
        /*********
        ** Accessors
        *********/
        /// <summary>How the animal produce data should be interpreted.</summary>
        /// <remarks>If this is either <see cref="Action.Edit"/> or <see cref="Action.Delete"/> then <see cref="DefaultProductId"/> and <see cref="UpgradedProductId"/> must all be specified from the recipe you're changing (this is because these 2 properties are used to identify recipes, this also means that these properties in a recipe cannot be changed with <see cref="Action.Edit"/>).</remarks>
        public Action Action { get; set; }

        /// <summary>The unique name for the produce.</summary>
        public string UniqueName { get; set; }

        /// <summary>The id of the default product.</summary>
        public string DefaultProductId { get; set; } = "-1";

        /// <summary>The minimum friendship required for the default product to drop.</summary>
        public int? DefaultProductMinFriendship { get; set; }

        /// <summary>The maximum friendship allowed for the default product to drop.</summary>
        public int? DefaultProductMaxFriendship { get; set; }

        /// <summary>The id of the upgraded product.</summary>
        public string UpgradedProductId { get; set; } = "-1";

        /// <summary>The minimum friendship required for the upgraded product to drop.</summary>
        public int? UpgradedProductMinFriendship { get; set; }

        /// <summary>The maximum friendship allowed for the upgraded product to drop.</summary>
        public int? UpgradedProductMaxFriendship { get; set; }

        /// <summary>The percent chance of the updated product to drop.</summary>
        /// <remarks>If this is <see langword="null"/>, then the chance is calculated using the base game calculation.</remarks>
        public float? PercentChanceForUpgradedProduct { get; set; }

        /// <summary>Whether the upgraded product is a 'rare product' (like the Rabbit Foot or Duck Feather).</summary>
        public bool? UpgradedProductIsRare { get; set; }

        /// <summary>The harvest type of the product.</summary>
        public HarvestType? HarvestType { get; set; }

        /// <summary>The number of days between each time the item gets produced.</summary>
        public int? DaysToProduce { get; set; }

        /// <summary>Whether <see cref="DaysToProduce"/> should be reduced by one if the player has the Coop Master profession.</summary>
        public bool? ProduceFasterWithCoopMaster { get; set; }

        /// <summary>Whether <see cref="DaysToProduce"/> should be reduced by one if the player has the Shepherd profession.</summary>
        public bool? ProduceFasterWithShepherd { get; set; }

        /// <summary>The name of the tool required to harvest the product (when the <see cref="HarvestType"/> is <see cref="HarvestType.Tool"/>).</summary>
        public string ToolName { get; set; }

        /// <summary>The sound bank id to play when harvesting the animal with a tool.</summary>
        public string ToolHarvestSound { get; set; }

        /// <summary>The amount of items that get produced at once.</summary>
        public int? Amount { get; set; }

        /// <summary>The season the product can be produced.</summary>
        public string[] Seasons { get; set; }

        /// <summary>The percent chance of the object being produced.</summary>
        public float? PercentChance { get; set; }

        /// <summary>The percent chance of the object producing one extra in it's stack.</summary>
        public float? PercentChanceForOneExtra { get; set; }

        /// <summary>Whether the animal must be male to produce the item.</summary>
        /// <remarks>If <see langword="false"/> is specified the animal must be female to produce the item, if <see langword="null"/> is specified then both genders can produce the item.</remarks>
        public bool? RequiresMale { get; set; }

        /// <summary>Whether the player must have the Coop Master profession for the animal to produce the item.</summary>
        /// <remarks>If <see langword="true"/> is specified then the farmer must have the Coop Master profession for the animal to drop the item, if <see langword="false"/> is specified then the farmer must not have the Coop Master profession for the animal to drop the item, if <see langword="null"/> is specified then it doesn't matter whether the farmer has it or not.</remarks>
        public bool? RequiresCoopMaster { get; set; }

        /// <summary>Whether the player must have the Shepherd profession for the animal to produce the item.</summary>
        /// <remarks>If <see langword="true"/> is specified then the farmer must have the Shepherd profession for the animal to drop the item, if <see langword="false"/> is specified then the farmer must not have the Shepherd profession for the animal to drop the item, if <see langword="null"/> is specified then it doesn't matter whether the farmer has it or not.</remarks>
        public bool? RequiresShepherd { get; set; }

        /// <summary>Whether the product should be standard quality only.</summary>
        public bool? StandardQualityOnly { get; set; }

        /// <summary>Whether the item can not be produced if it's in the player possession.</summary>
        public bool? DoNotAllowDuplicates { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Constructs an instance.</summary>
        public ParsedAnimalProduce() { }

        /// <summary>Constructs an instance.</summary>
        /// <param name="action">How the animal produce data should be interpreted.</param>
        /// <param name="uniqueName">The unique name for the produce.</param>
        /// <param name="defaultProductId">The id of the default product.</param>
        /// <param name="defaultProductMinFriendship">The minimum friendship required for the default product to drop.</param>
        /// <param name="defaultProductMaxFriendship">The maximum friendship allowed for the default product to drop.</param>
        /// <param name="upgradedProductId">The id of the upgraded product.</param>
        /// <param name="upgradedProductMinFriendship">The minimum friendship required for the upgraded product to drop.</param>
        /// <param name="upgradedProductMaxFriendship">The maximum friendship allowed for the upgraded product to drop.</param>
        /// <param name="percentChanceForUpgradedProduct">The percent chance of the updated product to drop.</param>
        /// <param name="upgradedProductIsRare">Whether the upgraded product is a 'rare product' (like the Rabbit Foot or Duck Feather).</param>
        /// <param name="harvestType">The harvest type of the product.</param>
        /// <param name="daysToProduce">The number of days between each time the item gets produced.</param>
        /// <param name="produceFasterWithCoopMaster">Whether <see cref="DaysToProduce"/> should be reduced by one if the player has the Coop Master profession.</param>
        /// <param name="produceFasterWithShepherd">Whether <see cref="DaysToProduce"/> should be reduced by one if the player has the Shepherd profession.</param>
        /// <param name="toolName">The name of the tool required to harvest the product (when the <see cref="HarvestType"/> is <see cref="HarvestType.Tool"/>).</param>
        /// <param name="toolHarvestSound">The sound bank id to play when harvesting the animal with a tool.</param>
        /// <param name="amount">The amount of items that get produced at once.</param>
        /// <param name="seasons">The season the product can be produced.</param>
        /// <param name="percentChance">The percent chance of the object being produced.</param>
        /// <param name="percentChanceForOneExtra">The percent chance of the object producing one extra in it's stack.</param>
        /// <param name="requiresMale">Whether the animal must be male to produce the item.</param>
        /// <param name="requiresCoopMaster">Whether the player must have the Coop Master profession for the animal to produce the item.</param>
        /// <param name="requiresShepherd">Whether the player must have the Shepherd profession for the animal to produce the item.</param>
        /// <param name="standardQualityOnly">Whether the product should be standard quality only.</param>
        /// <param name="doNotAllowDuplicates">Whether the item can not be produced if it's in the player possession.</param>
        public ParsedAnimalProduce(Action action, string uniqueName, string defaultProductId = "-1", int? defaultProductMinFriendship = 0, int? defaultProductMaxFriendship = 1000, string upgradedProductId = "-1", int? upgradedProductMinFriendship = 200, int? upgradedProductMaxFriendship = 1000, float? percentChanceForUpgradedProduct = null, bool? upgradedProductIsRare = false, HarvestType harvestType = FarmAnimalVarietyRedux.HarvestType.Lay, int? daysToProduce = 1, bool? produceFasterWithCoopMaster = false, bool? produceFasterWithShepherd = false, string toolName = null, string toolHarvestSound = null, int? amount = 1, string[] seasons = null, float? percentChance = 100, float percentChanceForOneExtra = 0, bool? requiresMale = null, bool? requiresCoopMaster = null, bool? requiresShepherd = null, bool standardQualityOnly = false, bool doNotAllowDuplicates = false)
        {
            Action = action;
            UniqueName = uniqueName;
            DefaultProductId = defaultProductId;
            DefaultProductMinFriendship = defaultProductMinFriendship;
            DefaultProductMaxFriendship = defaultProductMaxFriendship;
            UpgradedProductId = upgradedProductId;
            UpgradedProductMinFriendship = upgradedProductMinFriendship;
            UpgradedProductMaxFriendship = upgradedProductMaxFriendship;
            PercentChanceForUpgradedProduct = percentChanceForUpgradedProduct;
            UpgradedProductIsRare = upgradedProductIsRare;
            HarvestType = harvestType;
            DaysToProduce = daysToProduce;
            ProduceFasterWithCoopMaster = produceFasterWithCoopMaster;
            ProduceFasterWithShepherd = produceFasterWithShepherd;
            ToolName = toolName;
            ToolHarvestSound = toolHarvestSound;
            Amount = amount;
            Seasons = seasons;
            PercentChance = percentChance;
            PercentChanceForOneExtra = percentChanceForOneExtra;
            RequiresMale = requiresMale;
            RequiresCoopMaster = requiresCoopMaster;
            RequiresShepherd = requiresShepherd;
            StandardQualityOnly = standardQualityOnly;
            DoNotAllowDuplicates = doNotAllowDuplicates;
        }
    }
}
