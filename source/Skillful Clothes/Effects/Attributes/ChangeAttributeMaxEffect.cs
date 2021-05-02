/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Effects.Attributes
{
    abstract class ChangeAttributeMaxEffect : SingleEffect
    {
        int amount;

        public abstract string AttributeName { get; }

        public virtual EffectIcon Icon => EffectIcon.None;        

        protected abstract int GetMaxValue(Farmer farmer);
        protected abstract void SetMaxValue(Farmer farmer, int newValue);

        protected abstract int GetCurrentValue(Farmer farmer);
        protected abstract void SetCurrentValue(Farmer farmer, int newValue);

        protected override EffectDescriptionLine GenerateEffectDescription() => new EffectDescriptionLine(Icon, $"+{amount} max. {AttributeName}");

        public ChangeAttributeMaxEffect(int amount)
        {
            this.amount = amount;
        }        

        public override void Apply(Farmer farmer)
        {
            int curr = GetCurrentValue(farmer);
            int max = GetMaxValue(farmer);
            if (curr == max)
            {
                // if attribute is full, keep it full                
                SetMaxValue(farmer, max + amount);
                SetCurrentValue(farmer, curr + amount);                
            }
            else
            {
                SetMaxValue(farmer, max + amount);
            }

            Logger.Debug($"Max{AttributeName} + {amount}");
        }

        public override void Remove(Farmer farmer)
        {
            int newValue = GetMaxValue(farmer) - amount;
            if (GetCurrentValue(farmer) > newValue)
            {
                SetCurrentValue(farmer, newValue);                
            }
            SetMaxValue(farmer, newValue);            

            Logger.Debug($"Max{AttributeName} - {amount}");
        }
    }
}
