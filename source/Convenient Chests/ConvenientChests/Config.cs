using StardewModdingAPI;

namespace ConvenientChests
{
    public class Config
    {
        public bool CategorizeChests { get; set; } = true;
        public bool StashToExistingStacks { get; set; } = true;

        public bool StashToNearbyChests { get; set; } = true;
        public int StashRadius { get; set; } = 5;
        public SButton StashKey { get; set; } = SButton.Q;
        public SButton? StashButton { get; set; } = SButton.RightStick;

        public bool CraftFromChests { get; set; } = true;
        public int CraftRadius { get; set; } = 5;
    }
}
