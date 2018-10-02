using StardewModdingAPI;

namespace CleanFarm
{
    public class ModConfig
    {
        /// <summary>Should we remove all the grass.</summary>
        public bool RemoveGrass { get; set; } = false;

        /// <summary>Should we remove all the weeds.</summary>
        public bool RemoveWeeds { get; set; } = true;

        /// <summary>Should we remove all the stones.</summary>
        public bool RemoveStones { get; set; } = false;

        /// <summary>Should we remove all the twigs.</summary>
        public bool RemoveTwigs { get; set; } = false;

        /// <summary>Should we remove all tree stumps.</summary>
        public bool RemoveStumps { get; set; } = false;

        /// <summary>Should we remove all saplings (all trees below growth stage 5).</summary>
        public bool RemoveSaplings { get; set; } = false;

        /// <summary>
        /// If RemoveSaplings is true then all trees with a max growth stage below this value will be removed.
        /// 0 = seed, 1 = sprout, 2 = sapling, 3 = bush, 4 = small tree, 5 = tree.
        /// </summary>
        public int MaxTreeGrowthStageToAllow { get; set; } = 5;

        /// <summary>Should we remove all the large log resource clumps.</summary>
        public bool RemoveLargeLogs { get; set; } = false;

        /// <summary>Should we remove all large rock resource clumps.</summary>
        public bool RemoveLargeRocks { get; set; } = false;

        /// <summary>Should bushes be removed.</summary>
        public bool RemoveBushes { get; set; } = false;

        /// <summary>Should we print the name of each item that was removed and how many of that type were removed in the console.</summary>
        public bool ReportRemovedItemsToConsole { get; set; } = true;
    }
}
