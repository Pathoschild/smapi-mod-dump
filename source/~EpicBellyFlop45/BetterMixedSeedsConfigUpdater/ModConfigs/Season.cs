using System.Collections.Generic;

namespace BetterMixedSeedsConfigUpdater.ModConfigs
{
    class Season
    {
        public List<Crop> Crops { get; set; }

        public Season(List<Crop> crops)
        {
            Crops = crops;
        }
    }
}
