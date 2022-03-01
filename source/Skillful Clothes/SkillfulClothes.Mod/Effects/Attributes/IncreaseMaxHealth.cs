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
using StardewValley;

namespace SkillfulClothes.Effects.Attributes
{
    class IncreaseMaxHealth : ChangeAttributeMaxEffect
    {
        public override string AttributeName => "Health";

        public override EffectIcon Icon => EffectIcon.MaxHealth;

        public IncreaseMaxHealth(AmountEffectParameters parameters)
            : base(parameters)
        {
            // --
        }

        public IncreaseMaxHealth(int amount)
            : base(AmountEffectParameters.With(amount))
        {
            // --
        }

        protected override int GetCurrentValue(Farmer farmer) => farmer.health;

        protected override void SetCurrentValue(Farmer farmer, int newValue) => farmer.health = newValue;

        protected override int GetMaxValue(Farmer farmer) => farmer.maxHealth;

        protected override void SetMaxValue(Farmer farmer, int newValue) => farmer.maxHealth = newValue;
    }
}
