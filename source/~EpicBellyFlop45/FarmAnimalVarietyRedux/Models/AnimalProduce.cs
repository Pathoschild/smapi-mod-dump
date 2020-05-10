using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FarmAnimalVarietyRedux.Models
{
    /// <summary>Metadata about an animal's item production.</summary>
    public class AnimalProduce
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The items the animal can produce all year round.</summary>
        public AnimalProduceSeason AllSeasons { get; set; }

        /// <summary>The items the animal can produce in spring.</summary>
        public AnimalProduceSeason Spring { get; set; }

        /// <summary>The items the animal can produce in summer.</summary>
        public AnimalProduceSeason Summer { get; set; }

        /// <summary>The items the animal can produce in fall.</summary>
        public AnimalProduceSeason Fall { get; set; }

        /// <summary>The items the animal can produce in winter.</summary>
        public AnimalProduceSeason Winter { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Get a random product id.</summary>
        /// <param name="harvestType">The harvest type of the item.</param>
        /// <returns>A random product id.</returns>
        public int GetRandomDefault(out HarvestType harvestType)
        {
            var possibleProducts = AllSeasons?.Products?.ToList() ?? new List<AnimalProduct>();
            switch (Game1.currentSeason)
            {
                case "spring":
                    var springProducts = Spring?.Products?.ToList();
                    if (springProducts != null && springProducts.Count > 0)
                        possibleProducts.AddRange(springProducts);
                    break;
                case "summer":
                    var summerProducts = Summer?.Products?.ToList();
                    if (summerProducts != null && summerProducts.Count > 0)
                        possibleProducts.AddRange(summerProducts);
                    break;
                case "fall":
                    var fallProducts = Fall?.Products?.ToList();
                    if (fallProducts != null && fallProducts.Count > 0)
                        possibleProducts.AddRange(fallProducts);
                    break;
                case "winter":
                    var winterProducts = Winter?.Products?.ToList();
                    if (winterProducts != null && winterProducts.Count > 0)
                        possibleProducts.AddRange(winterProducts);
                    break;
            }

            if (possibleProducts.Count == 0)
            {
                harvestType = HarvestType.Lay;
                return -1;
            }

            var product = possibleProducts[Game1.random.Next(possibleProducts.Count)];
            harvestType = product.HarvestType;
            return Convert.ToInt32(product.Id); // already validated so no need to try parse
        }

        /// <summary>Get a random deluxe product id.</summary>
        /// <param name="harvestType">The harvest type of the item.</param>
        /// <returns>A random deluxe product id.</returns>
        public int GetRandomDeluxe(out HarvestType harvestType)
        {
            var possibleProducts = AllSeasons?.DeluxeProducts?.ToList() ?? new List<AnimalProduct>();
            switch (Game1.currentSeason)
            {
                case "spring":
                    var springProducts = Spring?.DeluxeProducts?.ToList();
                    if (springProducts != null && springProducts.Count > 0)
                        possibleProducts.AddRange(springProducts);
                    break;
                case "summer":
                    var summerProducts = Summer?.DeluxeProducts?.ToList();
                    if (summerProducts != null && summerProducts.Count > 0)
                        possibleProducts.AddRange(summerProducts);
                    break;
                case "fall":
                    var fallProducts = Fall?.DeluxeProducts?.ToList();
                    if (fallProducts != null && fallProducts.Count > 0)
                        possibleProducts.AddRange(fallProducts);
                    break;
                case "winter":
                    var winterProducts = Winter?.DeluxeProducts?.ToList();
                    if (winterProducts != null && winterProducts.Count > 0)
                        possibleProducts.AddRange(winterProducts);
                    break;
            }

            if (possibleProducts.Count == 0)
            {
                harvestType = HarvestType.Lay;
                return -1;
            }

            var product = possibleProducts[Game1.random.Next(possibleProducts.Count)];
            harvestType = product.HarvestType;
            return Convert.ToInt32(product.Id); // already validated so no need to try parse
        }
    }
}
