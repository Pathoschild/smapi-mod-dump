/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/GreenhouseGatherers
**
*************************************************/

using GreenhouseGatherers.GreenhouseGatherers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseGatherers.GreenhouseGatherers
{
    public class ModConfig
    {
        public bool DoJunimosEatExcessCrops { get; set; }
        public bool DoJunimosHarvestFromPots { get; set; }
        public bool DoJunimosAppearAfterHarvest { get; set; }
        public bool DoJunimosHarvestFromFruitTrees { get; set; }
        public bool DoJunimosHarvestFromFlowers { get; set; }
        public bool DoJunimosSowSeedsAfterHarvest { get; set; }
        public bool ForceRecipeUnlock { get; set; }
        public bool EnableHarvestMessage { get; set; }

        public int MaxAmountOfJunimosToAppearAfterHarvest { get; set; }
        public int MinimumFruitOnTreeBeforeHarvest { get; set; }

        public ModConfig()
        {
            DoJunimosEatExcessCrops = true;
            DoJunimosHarvestFromPots = true;
            DoJunimosAppearAfterHarvest = true;
            DoJunimosHarvestFromFruitTrees = true;
            DoJunimosHarvestFromFlowers = true;
            DoJunimosSowSeedsAfterHarvest = true;

            ForceRecipeUnlock = false;
            EnableHarvestMessage = true;

            MaxAmountOfJunimosToAppearAfterHarvest = -1;
            MinimumFruitOnTreeBeforeHarvest = 3;
        }
    }
}
