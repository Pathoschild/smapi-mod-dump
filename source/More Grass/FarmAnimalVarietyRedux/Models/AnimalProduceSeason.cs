using System.Collections.Generic;

namespace FarmAnimalVarietyRedux.Models
{
    /// <summary>Metadata about what an animal can produce in a season.</summary>
    public class AnimalProduceSeason
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The default products an animal can produce in the season.</summary>
        public List<AnimalProduct> Products { get; set; }

        /// <summary>The deluxe products an animal can produce in the season.</summary>
        public List<AnimalProduct> DeluxeProducts { get; set; }
    }
}
