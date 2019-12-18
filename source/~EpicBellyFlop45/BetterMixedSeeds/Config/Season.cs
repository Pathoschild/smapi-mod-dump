using System.Collections.Generic;

namespace BetterMixedSeeds.Config
{
    /// <summary>A wrapper for a list of crops. Used in the configuration.</summary>
    public class Season
    {
        /// <summary>The list of crops that are present in this season.</summary>
        public List<Crop> Crops { get; set; }

        /// <summary>Construct and instance.</summary>
        /// <param name="crops">The list of crops that will be added to the season.</param>
        public Season(List<Crop> crops)
        {
            Crops = crops;
        }
    }
}
