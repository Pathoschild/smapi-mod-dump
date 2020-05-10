namespace BFAVToFAVRModConverter.Models
{
    /// <summary>Metadata about an animal's item production for FAVR.</summary>
    public class FavrAnimalProduce
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The items the animal can produce all year round.</summary>
        public FavrAnimalProduceSeason AllSeasons { get; set; }

        /// <summary>The items the animal can produce in spring.</summary>
        public FavrAnimalProduceSeason Spring { get; set; }

        /// <summary>The items the animal can produce in summer.</summary>
        public FavrAnimalProduceSeason Summer { get; set; }

        /// <summary>The items the animal can produce in fall.</summary>
        public FavrAnimalProduceSeason Fall { get; set; }

        /// <summary>The items the animal can produce in winter.</summary>
        public FavrAnimalProduceSeason Winter { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="allSeasons">The items the animal can produce all year round.</param>
        /// <param name="spring">The items the animal can produce in spring.</param>
        /// <param name="summer">The items the animal can produce in summer.</param>
        /// <param name="fall">The items the animal can produce in fall.</param>
        /// <param name="winter">The items the animal can produce in winter.</param>
        public FavrAnimalProduce(FavrAnimalProduceSeason allSeasons, FavrAnimalProduceSeason spring = null, FavrAnimalProduceSeason summer = null, FavrAnimalProduceSeason fall = null, FavrAnimalProduceSeason winter = null)
        {
            AllSeasons = allSeasons;
            Spring = spring;
            Summer = summer;
            Fall = fall;
            Winter = winter;
        }
    }
}
