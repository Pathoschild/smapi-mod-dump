namespace BFAVToFAVRModConverter.Models
{
    /// <summary>Metadata about an item an animal can produce for FAVR.</summary>
    public class FavrAnimalProduct
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


        /*********
        ** Public Methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="id">The id of the product.</param>
        /// <param name="harvestType">The harvest type of the product.</param>
        /// <param name="toolName">The name of the tool required to harvest to the product.</param>
        public FavrAnimalProduct(string id, HarvestType harvestType, string toolName)
        {
            Id = id;
            HarvestType = harvestType;
            ToolName = toolName;
        }
    }
}
