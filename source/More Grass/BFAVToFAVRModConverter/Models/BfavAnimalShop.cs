namespace BFAVToFAVRModConverter.Models
{
    /// <summary>Represents the animal shop data in BFAV's 'content.json' file.</summary>
    public class BfavAnimalShop
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The name of the animal.</summary>
        public string Name { get; set; }

        /// <summary>The description of the animal.</summary>
        public string Description { get; set; }

        /// <summary>The price of the animal/</summary>
        public int Price { get; set; }

        /// <summary>The path to the shop icon of the animal.</summary>
        public string Icon { get; set; }
    }
}
