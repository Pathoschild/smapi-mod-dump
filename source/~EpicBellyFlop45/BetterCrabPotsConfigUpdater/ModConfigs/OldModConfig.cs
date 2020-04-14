using System.Collections.Generic;

namespace BetterCrabPotsConfigUpdater
{
    /// <summary>The old configuration model.</summary>
    public class OldModConfig
    {
        public bool EnableTrash { get; set; } = true;
        public bool RequiresBait { get; set; } = true;
        public int PercentChanceForTrash { get; set; } = 20;
        public bool EnableBetterQuality { get; set; } = false;
        public bool EnablePassiveTrash { get; set; } = false;
        public int PercentChanceForPassiveTrash { get; set; } = 20;
        public Dictionary<int, int> WhatCanBeFoundAsPassiveTrash { get; set; }
        public Dictionary<int, int> WhatCanBeFoundInFarmLand { get; set; }
        public Dictionary<int, int> WhatCanBeFoundInFarmLand_AsTrash { get; set; }
        public Dictionary<int, int> WhatCanBeFoundInCindersapForest { get; set; }
        public Dictionary<int, int> WhatCanBeFoundInCindersapForest_AsTrash { get; set; }
        public Dictionary<int, int> WhatCanBeFoundInMountainsLake { get; set; }
        public Dictionary<int, int> WhatCanBeFoundInMountainsLake_AsTrash { get; set; }
        public Dictionary<int, int> WhatCanBeFoundInTown { get; set; }
        public Dictionary<int, int> WhatCanBeFoundInTown_AsTrash { get; set; }
        public Dictionary<int, int> WhatCanBeFoundInMines_Layer20 { get; set; }
        public Dictionary<int, int> WhatCanBeFoundInMines_Layer20_AsTrash { get; set; }
        public Dictionary<int, int> WhatCanBeFoundInMines_Layer60 { get; set; }
        public Dictionary<int, int> WhatCanBeFoundInMines_Layer60_AsTrash { get; set; }
        public Dictionary<int, int> WhatCanBeFoundInMines_Layer100 { get; set; }
        public Dictionary<int, int> WhatCanBeFoundInMines_Layer100_AsTrash { get; set; }
        public Dictionary<int, int> WhatCanBeFoundInMutantBugLair { get; set; }
        public Dictionary<int, int> WhatCanBeFoundInMutantBugLair_AsTrash { get; set; }
        public Dictionary<int, int> WhatCanBeFoundInWitchsSwamp { get; set; }
        public Dictionary<int, int> WhatCanBeFoundInWitchsSwamp_AsTrash { get; set; }
        public Dictionary<int, int> WhatCanBeFoundInSecretWoods { get; set; }
        public Dictionary<int, int> WhatCanBeFoundInSecretWoods_AsTrash { get; set; }
        public Dictionary<int, int> WhatCanBeFoundInDesert { get; set; }
        public Dictionary<int, int> WhatCanBeFoundInDesert_AsTrash { get; set; }
        public Dictionary<int, int> WhatCanBeFoundInSewers { get; set; }
        public Dictionary<int, int> WhatCanBeFoundInSewers_AsTrash { get; set; }
        public Dictionary<int, int> WhatCanBeFoundInOcean { get; set; }
        public Dictionary<int, int> WhatCanBeFoundInOcean_AsTrash { get; set; }
    }
}
