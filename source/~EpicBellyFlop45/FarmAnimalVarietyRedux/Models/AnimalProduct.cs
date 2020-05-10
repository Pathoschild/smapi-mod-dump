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
    }
}
