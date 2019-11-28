namespace LocationCleaner.Framework.Config
{
    internal class Trees
    {
        public bool ClearTrees { get; set; } = true; //Whether or not to clear any trees
        public bool LeaveRandomTree { get; set; } = true; //Whether or not to randomize tree selection
        public int RandomLeaveTreeChance { get; set; } = 75;
        public bool ClearSeeds { get; set; } = false; // seedStage = 0;
        public bool ClearSprout { get; set; } = false; // sproutStage = 1;
        public bool ClearSapling { get; set; } = false; // saplingStage = 2;
        public bool ClearBush { get; set; } = true; // bushStage = 3;
        public bool ClearSmallTree { get; set; } = true; //treeStage = 4;
        public bool ClearFullTree { get; set; } = true; //treeStage = 5;
    }
}
