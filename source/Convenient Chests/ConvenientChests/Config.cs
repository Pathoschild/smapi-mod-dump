using Microsoft.Xna.Framework.Input;

namespace ConvenientChests
{
    public class Config
    {
        public bool CategorizeChests { get; set; } = true;
        public bool StashToExistingStacks { get; set; } = true;

        public bool StashToNearbyChests { get; set; } = true;
        public int StashRadius { get; set; } = 5;
        public Keys StashKey { get; set; } = Keys.Q;
        public Buttons? StashButton { get; set; } = Buttons.RightStick;

        public bool CraftFromChests { get; set; } = true;
        public int CraftRadius { get; set; } = 5;
    }
}