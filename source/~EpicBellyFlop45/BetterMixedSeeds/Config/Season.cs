using System.Collections.Generic;

namespace BetterMixedSeeds.Config
{
    public class Season
    {
        public List<Crop> Crops { get; set; }

        public Season(List<Crop> crops)
        {
            Crops = crops;
        }
    }
}
