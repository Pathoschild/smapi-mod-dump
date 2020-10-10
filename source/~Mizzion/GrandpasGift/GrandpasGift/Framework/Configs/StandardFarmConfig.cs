/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System.Collections.Generic;

namespace GrandpasGift.Framework.Configs
{
    internal class StandardFarmConfig
    {
        public int BuffAmount { get; set; } = 5;
        public int WeaponId { get; set; } = 20;
        public List<int> BonusItems { get; set; } = new List<int>();
    }
}
