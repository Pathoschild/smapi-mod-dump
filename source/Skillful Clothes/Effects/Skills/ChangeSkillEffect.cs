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

namespace SkillfulClothes.Effects.Skills
{
    abstract class ChangeSkillEffect : SingleEffect
    {
        int amount;        

        public abstract string SkillName { get; }

        protected abstract EffectIcon Icon { get; }

        protected override EffectDescriptionLine GenerateEffectDescription() => new EffectDescriptionLine(Icon, $"+{amount} {SkillName}");

        protected abstract void ChangeCurrentLevel(Farmer farmer, int amount);

        public ChangeSkillEffect(int amount)
        {
            this.amount = amount;
        }

        public override void Apply(Item sourceItem, EffectChangeReason reason)
        {
            ChangeCurrentLevel(Game1.player, amount);            

            Logger.Debug($"{SkillName} + {amount}");
        }

        public override void Remove(Item sourceItem, EffectChangeReason reason)
        {
            ChangeCurrentLevel(Game1.player, - amount);

            Logger.Debug($"{SkillName} - {amount}");
        }
    }
}
