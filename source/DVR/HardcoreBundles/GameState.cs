using System.Collections.Generic;

namespace HardcoreBundles
{
    public class GameState
    {
        public static GameState Current;

        public bool Activated { get; set; }
        public bool Declined { get; set; }

        public IList<int> Level5PerksAdded = new List<int>();


    }
}
