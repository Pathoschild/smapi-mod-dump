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

namespace SkillfulClothes.Effects.Special
{
    class MultiplyExperience : SingleEffect
    {
        int? lastXp;

        Skill skill;
        float multiplier;

        protected override EffectDescriptionLine GenerateEffectDescription() => new EffectDescriptionLine(skill.GetIcon(), $"Slightly increases {skill.ToString()} experience");

        public MultiplyExperience(Skill skill, float multiplier)
        {
            this.skill = skill;
            this.multiplier = multiplier;
        }

        public override void Apply(Item sourceItem, EffectChangeReason reason)
        {
            lastXp = null;
            EffectHelper.ModHelper.Events.GameLoop.UpdateTicking += GameLoop_UpdateTicking;
        }

        private void GameLoop_UpdateTicking(object sender, StardewModdingAPI.Events.UpdateTickingEventArgs e)
        {
            int currentXp = Game1.player.experiencePoints[(int)skill];

            // atm the xp which lead to a level gain a not multiplied
            if (lastXp.HasValue && currentXp > lastXp)
            {
                int gainedXp = currentXp - lastXp.Value;
                int additionalXp = (int)(gainedXp * (multiplier - 1));
                Logger.Debug($"XP = {gainedXp} + {additionalXp}");
                Game1.player.gainExperience((int)skill, additionalXp);
            }

            lastXp = Game1.player.experiencePoints[(int)skill];
        }

        public override void Remove(Item sourceItem, EffectChangeReason reason)
        {
            EffectHelper.ModHelper.Events.GameLoop.UpdateTicking -= GameLoop_UpdateTicking;
        }
    }
}
