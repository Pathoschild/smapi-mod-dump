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
    abstract class ChangeSkillEffect<TParameters> : SingleEffect<TParameters>
        where TParameters : AmountEffectParameters, new()
    {
        public abstract string SkillName { get; }

        protected abstract EffectIcon Icon { get; }

        protected override EffectDescriptionLine GenerateEffectDescription() => new EffectDescriptionLine(Icon, $"+{Parameters.Amount} {SkillName}");

        protected abstract void UpdateEffects(Farmer farmer, BuffEffects targetEffects);

        public ChangeSkillEffect(TParameters parameters)            
            : base(parameters)
        {
            // --
        }

        public override void Apply(Item sourceItem, EffectChangeReason reason)
        {                 
            Logger.Debug($"{SkillName} + {Parameters.Amount}");

            var buff = new ChangeSkillBuff(EffectId);
            BuffEffects effects = new BuffEffects();
            UpdateEffects(Game1.player, effects);
            buff.effects.Add(effects);
            Game1.player.buffs.Apply(buff);
        }

        public override void Remove(Item sourceItem, EffectChangeReason reason)
        {
            Logger.Debug($"{SkillName} - {Parameters.Amount}");
            Game1.player.buffs.Remove(EffectId);            
        }

        class ChangeSkillBuff : Buff
        {
            public ChangeSkillBuff(string id) :
                base(id, null, null, Buff.ENDLESS, null, -1, null, false, null, null)
            {
                this.visible = false; // do not show in UI                
            }
        }
    }
}
