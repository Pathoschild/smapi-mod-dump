using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mine_Changes.MineChanges.Config
{
    public class MapLayoutChanges
    {
        public int minStonePerDenominator = 10;
        public int maxStonePerDenominator = 30;
        public double stoneDenominator = 100.0d;

        public double minMonsterChance = 0.0002d;
        public int maxRandomMonsterPerDenominator = 200;
        public double maxRandomMonsterDenominator = 10000.0d;

        public double itemOnFloorChance = 0.0025d;
        public double gemStoneChanceDoubled = 0.003;

        //public int baseMaxBarrels = 5;
        public double barrelLuckMultiplier = 20;

        public double chanceForIceCrystals = 0.15d;
        public double chanceForMushrooms = 0.55d;

        public double chanceMonsterHasSpecialItem = 0.00175d;

        public double chanceForBigBolder = 0.005d;
    }
}
