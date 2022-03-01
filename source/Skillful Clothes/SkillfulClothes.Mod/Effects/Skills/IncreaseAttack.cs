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
using SkillfulClothes.Effects.SharedParameters;
using SkillfulClothes.Types;
using StardewValley;

namespace SkillfulClothes.Effects.Skills
{
    class IncreaseAttack : ChangeSkillEffect<AmountEffectParameters>
    {        
        protected override EffectIcon Icon => EffectIcon.Attack;

        public override string SkillName => "Attack";        

        protected override void ChangeCurrentLevel(Farmer farmer, int amount) => farmer.attack = Math.Max(0, farmer.attack + amount);

        public IncreaseAttack(AmountEffectParameters parameters)
            : base(parameters)
        {
            // --
        }


        public IncreaseAttack(int amount)
            : base(AmountEffectParameters.With(amount))
        {
            // --
        }
    }
}
