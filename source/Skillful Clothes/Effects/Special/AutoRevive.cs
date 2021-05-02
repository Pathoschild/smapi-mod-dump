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
using StardewValley;

namespace SkillfulClothes.Effects.Special
{
    class AutoRevive : SingleEffect
    {
        Farmer farmer;

        protected override EffectDescriptionLine GenerateEffectDescription() => new EffectDescriptionLine(EffectIcon.SaveFromDeath, "Restore health to 50% once");

        public override void Apply(Farmer farmer)
        {
            this.farmer = farmer;

            EffectHelper.ModHelper.Events.GameLoop.UpdateTicking += GameLoop_UpdateTicking;
        }

        private void GameLoop_UpdateTicking(object sender, StardewModdingAPI.Events.UpdateTickingEventArgs e)
        {
            if (farmer.health <= 0)
            {             
                farmer.health = farmer.maxHealth / 2;     
                // remove the shirt (it is consumed)
                farmer.shirtItem.Value = null;
            }
        }

        public override void Remove(Farmer farmer)
        {
            if (this.farmer == farmer)
            {
                EffectHelper.ModHelper.Events.GameLoop.UpdateTicking -= GameLoop_UpdateTicking;
            }
        }
    }
}
