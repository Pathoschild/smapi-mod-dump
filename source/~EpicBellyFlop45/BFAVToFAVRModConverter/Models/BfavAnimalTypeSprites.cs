namespace BFAVToFAVRModConverter.Models
{
    /// <summary>Represents the sprites of an animal's sub type in BFAV's 'content.json' file.</summary>
    public class BfavAnimalTypeSprites
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The path to the adult sprite sheet.</summary>
        public string Adult { get; set; }

        /// <summary>The path to the baby sprite sheet.</summary>
        public string Baby { get; set; }

        /// <summary>The path to the ready to harvest sprite sheet.</summary>
        public string ReadyForHarvest { get; set; }
    }
}
