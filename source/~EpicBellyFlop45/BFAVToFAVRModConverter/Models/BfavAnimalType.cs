namespace BFAVToFAVRModConverter.Models
{
    /// <summary>Represents an animal type in BFAV's 'content.json' file.</summary>
    public class BfavAnimalType
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The name of the sub type.</summary>
        public string Type { get; set; }

        /// <summary>The data string of the sub type.</summary>
        public string Data { get; set; }

        /// <summary>The sprites of the sub type.</summary>
        public BfavAnimalTypeSprites Sprites { get; set; }
    }
}
