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
using SkillfulClothes.Types;
using StardewValley;

namespace SkillfulClothes.Effects.Skills
{
    class IncreaseSpeed : ChangeSkillEffect
    {
        Item currentSourceItem;

        public IncreaseSpeed(int amount) 
            : base(amount)
        {
            // --
        }

        protected override EffectIcon Icon => EffectIcon.Speed;

        public override string SkillName => "Speed";        

        protected override void ChangeCurrentLevel(Farmer farmer, int amount) => farmer.addedSpeed = Math.Max(0, farmer.addedSpeed + amount);

        public override void Apply(Item sourceItem, EffectChangeReason reason)
        {
            base.Apply(sourceItem, reason);

            currentSourceItem = sourceItem;

            // if the game resets the speed value, reapply the effect
            EffectHelper.Events.PlayerSpeedWasReset -= Events_PlayerSpeedWasReset;
            EffectHelper.Events.PlayerSpeedWasReset += Events_PlayerSpeedWasReset; 
        }

        private void Events_PlayerSpeedWasReset(object sender, EventArgs e)
        {
            base.Apply(currentSourceItem, EffectChangeReason.Reset);
        }

        public override void Remove(Item sourceItem, EffectChangeReason reason)
        {
            base.Remove(sourceItem, reason);

            EffectHelper.Events.PlayerSpeedWasReset -= Events_PlayerSpeedWasReset;
        }
    }
}
