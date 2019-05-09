using System.Collections.Generic;

namespace SadisticBundles
{
    public class GameState
    {
        public static GameState Current;

        public bool Activated { get; set; }
        public bool Declined { get; set; }

        public bool LookingAtVanillaRewards { get; set; }

        public IList<int> Level5PerksAdded = new List<int>();
    }
}
