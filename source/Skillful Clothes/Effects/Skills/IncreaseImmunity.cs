/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace SkillfulClothes.Effects.Skills
{
    class IncreaseImmunity : ChangeSkillEffect
    {
        public IncreaseImmunity(int amount) 
            : base(amount)
        {
            // --
        }

        public override string SkillName => "Immunity";

        protected override EffectIcon Icon => EffectIcon.Immunity;

        protected override void ChangeCurrentLevel(Farmer farmer, int amount) => farmer.immunity = Math.Max(0, farmer.immunity + amount);
    }
}
