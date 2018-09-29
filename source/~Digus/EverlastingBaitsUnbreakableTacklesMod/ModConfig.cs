using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EverlastingBaitsAndUnbreakableTacklesMod
{
    public class ModConfig
    {
        public bool DisableIridiumQualityFish;
        public bool IridiumQualityFishOnlyWithWildBait;
        public bool IridiumQualityFishOnlyWithIridiumQualityBait;
        public float IridiumQualityFishMinimumSize;

        public ModConfig()
        {
            DisableIridiumQualityFish = false;
            IridiumQualityFishOnlyWithWildBait = true;
            IridiumQualityFishOnlyWithIridiumQualityBait = false;
            IridiumQualityFishMinimumSize = 0.95f;
        }
    }
}
