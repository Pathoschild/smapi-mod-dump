/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

using System.ComponentModel;

namespace BfavToFavrModConverter.Favr
{
    /// <summary>Represents an item that an FAVR animal can drop.</summary>
    public class FavrAnimalProduce
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The unique name of the produce.</summary>
        public string UniqueName { get; set; } = "0";

        /// <summary>The id of the default product.</summary>
        [DefaultValue("-1")]
        public string DefaultProductId { get; set; }

        /// <summary>The minimum friendship required for the default product to drop.</summary>
        [DefaultValue(0)]
        public int? DefaultProductMinFriendship { get; set; }

        /// <summary>The maximum friendship allowed for the default product to drop.</summary>
        [DefaultValue(1000)]
        public int? DefaultProductMaxFriendship { get; set; }

        /// <summary>The id of the upgraded product.</summary>
        [DefaultValue("-1")]
        public string UpgradedProductId { get; set; }

        /// <summary>The minimum friendship required for the upgraded product to drop.</summary>
        [DefaultValue(200)]
        public int? UpgradedProductMinFriendship { get; set; }

        /// <summary>The maximum friendship allowed for the upgraded product to drop.</summary>
        [DefaultValue(1000)]
        public int? UpgradedProductMaxFriendship { get; set; }

        /// <summary>The percent chance of the updated product to drop.</summary>
        /// <remarks>If this is <see langword="null"/>, then the chance is calculated using the base game calculation.</remarks>
        [DefaultValue(null)]
        public float? PercentChanceForUpgradedProduct { get; set; }

        /// <summary>Whether the upgraded product is a 'rare product' (like the Rabbit Foot or Duck Feather).</summary>
        [DefaultValue(false)]
        public bool? UpgradedProductIsRare { get; set; }

        /// <summary>The harvest type of the product.</summary>
        [DefaultValue(Favr.HarvestType.Lay)]
        public HarvestType? HarvestType { get; set; }

        /// <summary>The number of days between each time the item gets produced.</summary>
        [DefaultValue(1)]
        public int? DaysToProduce { get; set; }

        /// <summary>The name of the tool required to harvest the product (when the <see cref="HarvestType"/> is <see cref="HarvestType.Tool"/>).</summary>
        [DefaultValue(null)]
        public string ToolName { get; set; }

        /// <summary>The sound bank id to play when harvesting the animal with a tool.</summary>
        [DefaultValue(null)]
        public string ToolHarvestSound { get; set; }

        /// <summary>The amount of items that get produced at once.</summary>
        [DefaultValue(1)]
        public int? Amount { get; set; }

        /// <summary>The season the product can be produced.</summary>
        [DefaultValue(null)]
        public string[] Seasons { get; set; }

        /// <summary>The percent chance of the object being produced.</summary>
        [DefaultValue(100.0)]
        public float? PercentChance { get; set; }

        /// <summary>The percent chance of the object producing one extra in it's stack.</summary>
        [DefaultValue(0.0)]
        public float? PercentChanceForOneExtra { get; set; }

        /// <summary>Whether the animal must be male to produce the item.</summary>
        /// <remarks>If <see langword="false"/> is specified the animal must be female to produce the item, if <see langword="null"/> is specified then both genders can produce the item.</remarks>
        [DefaultValue(null)]
        public bool? RequiresMale { get; set; }

        /// <summary>Whether the player must have the Coop Master profession for the animal to produce the item.</summary>
        /// <remarks>If <see langword="true"/> is specified then the farmer must have the Coop Master profession for the animal to drop the item, if <see langword="false"/> is specified then the farmer must not have the Coop Master profession for the animal to drop the item, if <see langword="null"/> is specified then it doesn't matter whether the farmer has it or not.</remarks>
        [DefaultValue(null)]
        public bool? RequiresCoopMaster { get; set; }

        /// <summary>Whether the player must have the Shepherd profession for the animal to produce the item.</summary>
        /// <remarks>If <see langword="true"/> is specified then the farmer must have the Shepherd profession for the animal to drop the item, if <see langword="false"/> is specified then the farmer must not have the Shepherd profession for the animal to drop the item, if <see langword="null"/> is specified then it doesn't matter whether the farmer has it or not.</remarks>
        [DefaultValue(null)]
        public bool? RequiresShepherd { get; set; }

        /// <summary>Whether the product should be standard quality only.</summary>
        [DefaultValue(false)]
        public bool? StandardQualityOnly { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Constructs an instance.</summary>
        /// <param name="defaultProductId">The id of the default product.</param>
        /// <param name="defaultProductMinFriendship">The minimum friendship required for the default product to drop.</param>
        /// <param name="defaultProductMaxFriendship">The maximum friendship allowed for the default product to drop.</param>
        /// <param name="upgradedProductId">The id of the upgraded product.</param>
        /// <param name="upgradedProductMinFriendship">The minimum friendship required for the upgraded product to drop.</param>
        /// <param name="upgradedProductMaxFriendship">The maximum friendship allowed for the upgraded product to drop.</param>
        /// <param name="percentChanceForUpgradedProduct">The percent chance of the updated product to drop.</param>
        /// <param name="harvestType">The harvest type of the product.</param>
        /// <param name="daysToProduce">The number of days between each time the item gets produced.</param>
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
        public FavrAnimalProduce(string defaultProductId = "-1", int? defaultProductMinFriendship = 0, int? defaultProductMaxFriendship = 1000, string upgradedProductId = "-1", int? upgradedProductMinFriendship = 200, int? upgradedProductMaxFriendship = 1000, float? percentChanceForUpgradedProduct = null, bool? upgradedProductIsRare = false, HarvestType harvestType = Favr.HarvestType.Lay, int? daysToProduce = 1, string toolName = null, string toolHarvestSound = null, int? amount = 1, string[] seasons = null, float? percentChance = 100, float percentChanceForOneExtra = 0, bool? requiresMale = null, bool? requiresCoopMaster = null, bool? requiresShepherd = null, bool standardQualityOnly = false)
        {
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
        }
    }
}
