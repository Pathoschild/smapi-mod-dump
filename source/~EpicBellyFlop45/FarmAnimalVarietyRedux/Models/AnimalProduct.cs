/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/EpicBellyFlop45/StardewMods
**
*************************************************/

using FarmAnimalVarietyRedux.Enums;

namespace FarmAnimalVarietyRedux.Models
{
    /// <summary>Metadata about an item an animal can produce.</summary>
    public class AnimalProduct
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The id of the product.</summary>
        public string Id { get; set; }

        /// <summary>The harvest type of the product.</summary>
        public HarvestType HarvestType { get; set; }

        /// <summary>The name of the tool required to harvest to the product.</summary>
        public string ToolName { get; set; }

        /// <summary>The number of friendship hearts required for the animal to produce the product.</summary>
        public int HeartsRequired { get; set; }

        /// <summary>The percent chance of the object being produced.</summary>
        public int PercentChance { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="id">The id of the product.</param>
        /// <param name="harvestType">The harvest type of the product.</param>
        /// <param name="toolName">The name of the tool required to harvest to the product.</param>
        /// <param name="heartsRequired">The number of friendship hearts required for the animal to produce the product.</param>
        /// <param name="percentChance">The percent chance of the object being produced.</param>
        public AnimalProduct(string id, HarvestType harvestType, string toolName, int heartsRequired = 0, int percentChance = 100)
        {
            Id = id;
            HarvestType = harvestType;
            ToolName = toolName;
            HeartsRequired = heartsRequired;
            PercentChance = percentChance;
        }
    }
}
