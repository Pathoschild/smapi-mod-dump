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


        /*********
        ** Public Methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="products">The default products an animal can produce in the season.</param>
        /// <param name="deluxeProducts">The deluxe products an animal can produce in the season.</param>
        public AnimalProduceSeason(List<AnimalProduct> products, List<AnimalProduct> deluxeProducts = null)
        {
            Products = products;
            DeluxeProducts = deluxeProducts;
        }
    }
}
