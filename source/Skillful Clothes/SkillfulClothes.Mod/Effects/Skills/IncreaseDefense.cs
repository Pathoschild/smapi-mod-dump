/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using SkillfulClothes.Effects.SharedParameters;
using SkillfulClothes.Types;
using StardewValley;
using StardewValley.Buffs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Effects.Skills
{
    class IncreaseDefense : ChangeSkillEffect<AmountEffectParameters>
    {
        protected override EffectIcon Icon => EffectIcon.Defense;

        public override string SkillName => "Defense";

        protected override void UpdateEffects(Farmer farmer, BuffEffects targetEffects)
        {
            targetEffects.Defense.Value = Parameters.Amount;
        }

        public IncreaseDefense(AmountEffectParameters parameters)
            : base(parameters)
        {
            // --
        }

        public IncreaseDefense(int amount)
            : base(AmountEffectParameters.With(amount))
        {
            // --
        }
    }
}
