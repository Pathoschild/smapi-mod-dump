/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using SkillfulClothes.Types;
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

        public override void Apply(Item sourceItem, EffectChangeReason reason)
        {
            int curr = GetCurrentValue(Game1.player);
            int max = GetMaxValue(Game1.player);
            if (curr == max)
            {
                // if attribute is full, keep it full                
                SetMaxValue(Game1.player, max + amount);
                SetCurrentValue(Game1.player, curr + amount);                
            }
            else
            {
                SetMaxValue(Game1.player, max + amount);
            }

            Logger.Debug($"Max{AttributeName} + {amount}");
        }

        public override void Remove(Item sourceItem, EffectChangeReason reason)
        {
            int newValue = GetMaxValue(Game1.player) - amount;
            if (GetCurrentValue(Game1.player) > newValue)
            {
                SetCurrentValue(Game1.player, newValue);                
            }
            SetMaxValue(Game1.player, newValue);            

            Logger.Debug($"Max{AttributeName} - {amount}");
        }
    }
}
