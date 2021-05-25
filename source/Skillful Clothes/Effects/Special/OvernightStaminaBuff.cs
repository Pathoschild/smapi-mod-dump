/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using SkillfulClothes.Effects.Buffs;
using SkillfulClothes.Types;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Effects.Special
{
    /// <summary>
    /// Grants a time-limited buff to max stamina
    /// if the player slept with the clothing item on
    /// </summary>
    class OvernightStaminaBuff : SingleEffect
    {
        int amount;

        protected override EffectDescriptionLine GenerateEffectDescription() => new EffectDescriptionLine(EffectIcon.MaxEnergy, $"Begin your day with +{amount} max. Energy");        

        public OvernightStaminaBuff(int amount)
        {
            this.amount = amount;
        }

        public override void Apply(Item sourceItem, EffectChangeReason reason)
        {
            if (reason == EffectChangeReason.DayStart)
            {
                Logger.Debug("Grant MaxEnergy buff");

                // create & give buff to player
                MaxStaminaBuff staminaBuff = new MaxStaminaBuff(amount, 360, sourceItem?.DisplayName ?? "");                
                Game1.buffsDisplay.addOtherBuff(staminaBuff);
                
                // Game1.addHUDMessage(new HUDMessage("You awake eager to get to work."));
            }
        }

        public override void Remove(Item sourceItem, EffectChangeReason reason)
        {
            // nothing to do
        }
    }
}
