using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCrabPots
{
    class ModConfig
    {
        public bool EnableTrash { get; set; } = true;
        public bool RequiresBait { get; set; } = true;
        public int PercentChanceForTrash { get; set; } = 20;
        public bool EnableBetterQuality { get; set; } = false;
        public bool EnablePassiveTrash { get; set; } = false;
        public int PercentChanceForPassiveTrash { get; set; } = 20;
        public Dictionary<int, int> WhatCanBeFoundAsPassiveTrash { get; set; } = new Dictionary<int, int> { { 168, 1 }, { 169, 1 }, { 170, 1 }, { 171, 1 }, { 172, 1 } };
        public Dictionary<int, int> WhatCanBeFoundInFarmLand { get; set; } = new Dictionary<int, int>() { { 716, 1 }, { 721, 1 }, { 722, 1 } };
        public Dictionary<int, int> WhatCanBeFoundInFarmLand_AsTrash { get; set; } = new Dictionary<int, int>() { { 168, 1 }, { 169, 1 }, { 170, 1 }, { 171, 1 }, { 172, 1 } };
        public Dictionary<int, int> WhatCanBeFoundInCindersapForest { get; set; } = new Dictionary<int, int>() { { 716, 1 }, { 721, 1 }, { 722, 1 } };
        public Dictionary<int, int> WhatCanBeFoundInCindersapForest_AsTrash { get; set; } = new Dictionary<int, int>() { { 168, 1 }, { 169, 1 }, { 170, 1 }, { 171, 1 }, { 172, 1 } };
        public Dictionary<int, int> WhatCanBeFoundInMountainsLake { get; set; } = new Dictionary<int, int>() { { 716, 1 }, { 721, 1 }, { 722, 1 } };
        public Dictionary<int, int> WhatCanBeFoundInMountainsLake_AsTrash { get; set; } = new Dictionary<int, int>() { { 168, 1 }, { 169, 1 }, { 170, 1 }, { 171, 1 }, { 172, 1 } };
        public Dictionary<int, int> WhatCanBeFoundInTown { get; set; } = new Dictionary<int, int>() { { 716, 1 }, { 721, 1 }, { 722, 1 } };
        public Dictionary<int, int> WhatCanBeFoundInTown_AsTrash { get; set; } = new Dictionary<int, int>() { { 168, 1 }, { 169, 1 }, { 170, 1 }, { 171, 1 }, { 172, 1 } };
        public Dictionary<int, int> WhatCanBeFoundInMines_Layer20 { get; set; } = new Dictionary<int, int>() { { 716, 1 }, { 721, 1 }, { 722, 1 } };
        public Dictionary<int, int> WhatCanBeFoundInMines_Layer20_AsTrash { get; set; } = new Dictionary<int, int>() { { 168, 1 }, { 169, 1 }, { 170, 1 }, { 171, 1 }, { 172, 1 } };
        public Dictionary<int, int> WhatCanBeFoundInMines_Layer60 { get; set; } = new Dictionary<int, int>() { { 716, 1 }, { 721, 1 }, { 722, 1 } };
        public Dictionary<int, int> WhatCanBeFoundInMines_Layer60_AsTrash { get; set; } = new Dictionary<int, int>() { { 168, 1 }, { 169, 1 }, { 170, 1 }, { 171, 1 }, { 172, 1 } };
        public Dictionary<int, int> WhatCanBeFoundInMines_Layer100 { get; set; } = new Dictionary<int, int>() { { 716, 1 }, { 721, 1 }, { 722, 1 } };
        public Dictionary<int, int> WhatCanBeFoundInMines_Layer100_AsTrash { get; set; } = new Dictionary<int, int>() { { 168, 1 }, { 169, 1 }, { 170, 1 }, { 171, 1 }, { 172, 1 } };
        public Dictionary<int, int> WhatCanBeFoundInMutantBugLair { get; set; } = new Dictionary<int, int>() { { 716, 1 }, { 721, 1 }, { 722, 1 } };
        public Dictionary<int, int> WhatCanBeFoundInMutantBugLair_AsTrash { get; set; } = new Dictionary<int, int>() { { 168, 1 }, { 169, 1 }, { 170, 1 }, { 171, 1 }, { 172, 1 } };
        public Dictionary<int, int> WhatCanBeFoundInWitchsSwamp { get; set; } = new Dictionary<int, int>() { { 716, 1 }, { 721, 1 }, { 722, 1 } };
        public Dictionary<int, int> WhatCanBeFoundInWitchsSwamp_AsTrash { get; set; } = new Dictionary<int, int>() { { 168, 1 }, { 169, 1 }, { 170, 1 }, { 171, 1 }, { 172, 1 } };
        public Dictionary<int, int> WhatCanBeFoundInSecretWoods { get; set; } = new Dictionary<int, int>() { { 716, 1 }, { 721, 1 }, { 722, 1 } };
        public Dictionary<int, int> WhatCanBeFoundInSecretWoods_AsTrash { get; set; } = new Dictionary<int, int>() { { 168, 1 }, { 169, 1 }, { 170, 1 }, { 171, 1 }, { 172, 1 } };
        public Dictionary<int, int> WhatCanBeFoundInDesert { get; set; } = new Dictionary<int, int>() { { 716, 1 }, { 721, 1 }, { 722, 1 } };
        public Dictionary<int, int> WhatCanBeFoundInDesert_AsTrash { get; set; } = new Dictionary<int, int>() { { 168, 1 }, { 169, 1 }, { 170, 1 }, { 171, 1 }, { 172, 1 } };
        public Dictionary<int, int> WhatCanBeFoundInSewers { get; set; } = new Dictionary<int, int>() { { 716, 1 }, { 721, 1 }, { 722, 1 } };
        public Dictionary<int, int> WhatCanBeFoundInSewers_AsTrash { get; set; } = new Dictionary<int, int>() { { 168, 1 }, { 169, 1 }, { 170, 1 }, { 171, 1 }, { 172, 1 } };
        public Dictionary<int, int> WhatCanBeFoundInOcean { get; set; } = new Dictionary<int, int>() { { 715, 1 }, { 372, 1 }, { 717, 1 }, { 718, 1 }, { 719, 1 }, { 720, 1 }, { 723, 1 } };
        public Dictionary<int, int> WhatCanBeFoundInOcean_AsTrash { get; set; } = new Dictionary<int, int>() { { 168, 1 }, { 169, 1 }, { 170, 1 }, { 171, 1 }, { 172, 1 } };

        /*
        public Dictionary<int, int> WhatCanBeFoundInFreshWater { get; set; } = new Dictionary<int, int>() { { 716, 1 }, { 721, 1 }, { 722, 1 } };
        public Dictionary<int, int> WhatCanBeFoundInOcean { get; set; } = new Dictionary<int, int>() { { 715, 1 }, { 372, 1 }, { 717, 1 }, { 718, 1 }, { 719, 1 }, { 720, 1 }, { 723, 1 } };
        public Dictionary<int, int> WhatCanBeFoundAsTrashInFreshWater { get; set; } = new Dictionary<int, int>() { { 168, 1 }, { 169, 1 }, { 170, 1 }, { 171, 1 }, { 172, 1 } };
        public Dictionary<int, int> WhatCanBeFoundAsTrashInOcean { get; set; } = new Dictionary<int, int>() { { 168, 1 }, { 169, 1 }, { 170, 1 }, { 171, 1 }, { 172, 1 } };
        */
    }
}
