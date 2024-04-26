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
using StardewValley.Buffs;

namespace SkillfulClothes.Effects.Skills
{
    class IncreaseImmunity : ChangeSkillEffect<AmountEffectParameters>
    {
        public IncreaseImmunity(AmountEffectParameters parameters) 
            : base(parameters)
        {
            // --
        }

        public IncreaseImmunity(int amount)
            : base(AmountEffectParameters.With(amount))
        {
            // --
        }

        public override string SkillName => "Immunity";

        protected override EffectIcon Icon => EffectIcon.Immunity;

        protected override void UpdateEffects(Farmer farmer, BuffEffects targetEffects)
        {            
            targetEffects.Immunity.Value = Parameters.Amount;            
        }
    }
}
