/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatOverhaulMod.Buffs
{
    public class CustomBuffData
    {
        public class EffectInstance
        {
            public string EffectId { get; set; }
            public float Modifier { get; set; }
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }

        public bool FarmerOnly { get; set; } = false;
        public bool IsConsideredDebuff { get; set; } = false;

        public EffectInstance[] Effects { get; set; }
    }
}
