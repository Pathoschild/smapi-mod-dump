using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExhaustionTweaks
{
    class ModConfig
    {
        //money loss on passout.
        public bool enableSafeSleeping { get; set; } = true;
        public List<String> safeSleepingLocations { get; set; } = new List<string>() { "Farm", "FarmCave" };
        public bool enableSafeSleepingInFarmBuildings { get; set; } = true;
        public bool requireSpouseForSafeSleeping { get; set; } = false;

        public bool enableRobberyOnFarm { get; set; } = true;
        public bool enableRobberyOnNonFarmSafeLocations { get; set; } = true;
        public int percentRobberyChance { get; set; } = 10;
        public int percentFundsLostMinimumInRobbery { get; set; } = 5;
        public int percentFundsLostMaximumInRobbery { get; set; } = 25;

        //percent energy loss at passout.
        public bool reduceEnergyWhenPassingOut { get; set; } = true;
        public int percentEnergyLostWhenPassingOut { get; set; } = 60;

        //consistant energy loss after a specified time.
        public bool enableConstantEnergyLossAfterTime { get; set; } = true;
        public int percentEnergyLostEvery10Minutes { get; set; } = 5;
        public int constantEnergyLossStartTime { get; set; } = 2400;
        public bool disableLossAtMinimumEnergy { get; set; } = true;
        public int minimumEnergyToDisableLoss { get; set; } = 10;

    }
}
