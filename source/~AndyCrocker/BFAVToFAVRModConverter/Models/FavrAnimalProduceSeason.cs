/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using System.Collections.Generic;

namespace BFAVToFAVRModConverter.Models
{
    /// <summary>Metadata about what an animal can produce in a season for FAVR.</summary>
    public class FavrAnimalProduceSeason
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The default products an animal can produce in the season.</summary>
        public List<FavrAnimalProduct> Products { get; set; }

        /// <summary>The deluxe products an animal can produce in the season.</summary>
        public List<FavrAnimalProduct> DeluxeProducts { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="products">The default products an animal can produce in the season.</param>
        /// <param name="deluxeProducts">The deluxe products an animal can produce in the season.</param>
        public FavrAnimalProduceSeason(List<FavrAnimalProduct> products, List<FavrAnimalProduct> deluxeProducts)
        {
            Products = products;
            DeluxeProducts = deluxeProducts;
        }
    }
}
