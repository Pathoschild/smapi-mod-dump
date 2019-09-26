using System;

namespace TheChestDimension
{
    class TCDRules
    {
        public string CanOnlyEnterFrom { get; set; } // if not empty, implies CanEnterFromCave = false
        public bool CanEnterFromCave { get; set; } = true;

        public TCDRules()
        {
        }

        public TCDRules(ModConfig config)
        {
            CanOnlyEnterFrom = config.CanOnlyEnterFrom;
            CanEnterFromCave = config.CanEnterFromCave;
        }
    }
}