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
    class IncreaseSpeed : ChangeSkillEffect
    {
        public IncreaseSpeed(int amount) 
            : base(amount)
        {
            // --
        }

        protected override EffectIcon Icon => EffectIcon.Speed;

        public override string SkillName => "Speed";        

        protected override void ChangeCurrentLevel(Farmer farmer, int amount) => farmer.addedSpeed += amount;
    }
}
