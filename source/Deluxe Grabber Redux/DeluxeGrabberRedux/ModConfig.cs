/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ferdaber/sdv-mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeluxeGrabberRedux
{    
    class ModConfig
    {
        public bool slimeHutch;
        public bool farmCaveMushrooms;
        public bool harvestCrops;
        public bool harvestCropsIndoorPots;
        public int harvestCropsRange;
        public string harvestCropsRangeMode;
        public bool artifactSpots;
        public bool orePan;
        public bool bushes;
        public bool fruitTrees;
        public bool seedTrees;
        public bool flowers;
        public bool reportYield;
        public bool gainExperience;
        public bool fellSecretWoodsStumps;
        public bool garbageCans;

        internal static string[] HarvestCropsRangeMode = new string[] { "Square", "Walk" };

        public ModConfig()
        {
            slimeHutch = true;
            farmCaveMushrooms = true;
            harvestCrops = false;
            harvestCropsIndoorPots = true;
            harvestCropsRange = -1;
            harvestCropsRangeMode = "Walk";
            artifactSpots = false;
            orePan = false;
            bushes = true;
            fruitTrees = false;
            seedTrees = false;
            flowers = false;
            reportYield = true;
            gainExperience = true;
            fellSecretWoodsStumps = false;
            garbageCans = false;
        }
    }
}
