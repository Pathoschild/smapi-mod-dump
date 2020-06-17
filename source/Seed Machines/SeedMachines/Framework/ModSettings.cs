using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedMachines.Framework
{
    class ModSettings
    {
        public int seedMachinePrice { get; set; } = 10000;
        public String seedMachineIngredients { get; set; } = "787 2 337 5 335 20";
        public int seedMachinePriceForNonSalableSeeds { get; set; } = 300;
        public double seedMachinePriceMultiplier { get; set; } = 2.0;

        public int seedBanditPrice { get; set; } = 15000;
        public String seedBanditIngredients { get; set; } = "787 2 337 5 335 20";
        public int seedBanditOneGamePrice { get; set; } = 100;

        public String themeName { get; set; } = "Default";
    }
}
