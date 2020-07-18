using FarmAnimalVarietyRedux.Enums;
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
        /// <summary>Construct an instance.</summary>
        /// <param name="allSeasons">The items the animal can produce all year round.</param>
        /// <param name="spring">The items the animal can produce in spring.</param>
        /// <param name="summer">The items the animal can produce in summer.</param>
        /// <param name="fall">The items the animal can produce in fall.</param>
        /// <param name="winter">The items the animal can produce in winter.</param>
        public AnimalProduce(AnimalProduceSeason allSeasons = null, AnimalProduceSeason spring = null, AnimalProduceSeason summer = null, AnimalProduceSeason fall = null, AnimalProduceSeason winter = null)
        {
            AllSeasons = allSeasons;
            Spring = spring;
            Summer = summer;
            Fall = fall;
            Winter = winter;
        }

        /// <summary>Get a random product id.</summary>
        /// <param name="numberOfHearts">The number of hearts the player has with the animal (This is for the heart based produce).</param>
        /// <param name="harvestType">The harvest type of the item.</param>
        /// <returns>A random product id.</returns>
        public int GetRandomDefault(int numberOfHearts, out HarvestType harvestType)
        {
            var random = new Random();
            var possibleProducts = AllSeasons?.Products?.ToList() ?? new List<AnimalProduct>();
            switch (Game1.currentSeason)
            {
                case "spring":
                    var springProducts = Spring?.Products?.Where(product => product.HeartsRequired <= numberOfHearts && random.Next(100) + 1 <= product.PercentChance).ToList();
                    if (springProducts != null && springProducts.Count > 0)
                        possibleProducts.AddRange(springProducts);
                    break;
                case "summer":
                    var summerProducts = Summer?.Products?.Where(product => product.HeartsRequired <= numberOfHearts && random.Next(100) + 1 <= product.PercentChance).ToList();
                    if (summerProducts != null && summerProducts.Count > 0)
                        possibleProducts.AddRange(summerProducts);
                    break;
                case "fall":
                    var fallProducts = Fall?.Products?.Where(product => product.HeartsRequired <= numberOfHearts && random.Next(100) + 1 <= product.PercentChance).ToList();
                    if (fallProducts != null && fallProducts.Count > 0)
                        possibleProducts.AddRange(fallProducts);
                    break;
                case "winter":
                    var winterProducts = Winter?.Products?.Where(product => product.HeartsRequired <= numberOfHearts && random.Next(100) + 1 <= product.PercentChance).ToList();
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
        public int GetRandomDeluxe(int numberOfHearts, out HarvestType harvestType)
        {
            var random = new Random();
            var possibleProducts = AllSeasons?.DeluxeProducts?.ToList() ?? new List<AnimalProduct>();
            switch (Game1.currentSeason)
            {
                case "spring":
                    var springProducts = Spring?.DeluxeProducts?.Where(product => product.HeartsRequired <= numberOfHearts && random.Next(100) + 1 <= product.PercentChance).ToList();
                    if (springProducts != null && springProducts.Count > 0)
                        possibleProducts.AddRange(springProducts);
                    break;
                case "summer":
                    var summerProducts = Summer?.DeluxeProducts?.Where(product => product.HeartsRequired <= numberOfHearts && random.Next(100) + 1 <= product.PercentChance).ToList();
                    if (summerProducts != null && summerProducts.Count > 0)
                        possibleProducts.AddRange(summerProducts);
                    break;
                case "fall":
                    var fallProducts = Fall?.DeluxeProducts?.Where(product => product.HeartsRequired <= numberOfHearts && random.Next(100) + 1 <= product.PercentChance).ToList();
                    if (fallProducts != null && fallProducts.Count > 0)
                        possibleProducts.AddRange(fallProducts);
                    break;
                case "winter":
                    var winterProducts = Winter?.DeluxeProducts?.Where(product => product.HeartsRequired <= numberOfHearts && random.Next(100) + 1 <= product.PercentChance).ToList();
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
