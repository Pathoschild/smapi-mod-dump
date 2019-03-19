using System.Collections.Generic;

namespace GrandpasGift.Framework.Configs
{
    internal class RiverlandFarmConfig
    {
        public int BuffAmount { get; set; } = 5;
        public int WeaponId { get; set; } = 20;
        public List<int> BonusItems { get; set; } = new List<int>();
    }
}
