namespace FarmAnimalVarietyRedux.Models
{
    /// <summary>Metadata about an animal subtype.</summary>
    public class AnimalSubType
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The name of the subtype.</summary>
        public string Name { get; set; }

        /// <summary>The data of the subtype.</summary>
        public AnimalSubTypeData Data { get; set; }

        /// <summary>The sprite sheets for the subtype.</summary>
        public AnimalSprites Sprites { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="name">The name of the subtype.</param>
        /// <param name="data">The data of the subtype.</param>
        /// <param name="sprites">The sprite sheets for the subtype.</param>
        public AnimalSubType(string name, AnimalSubTypeData data, AnimalSprites sprites)
        {
            Name = name;
            Data = data;
            Sprites = sprites;
        }
    }
}
