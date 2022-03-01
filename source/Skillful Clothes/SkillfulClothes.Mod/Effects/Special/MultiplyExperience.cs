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
    class MultiplyExperience : SingleEffect<MultiplyExperienceParameters>
    {
        int? lastXp;

        protected override EffectDescriptionLine GenerateEffectDescription() => new EffectDescriptionLine(Parameters.Skill.GetIcon(), $"Slightly increases {Parameters.Skill.ToString()} experience");

        public MultiplyExperience(MultiplyExperienceParameters parameters)
            : base(parameters)
        {
            // --
        }

        public MultiplyExperience(Skill skill, float multiplier)
            : base(MultiplyExperienceParameters.With(skill, multiplier))
        {
            // --
        }

        public override void Apply(Item sourceItem, EffectChangeReason reason)
        {
            lastXp = null;
            EffectHelper.ModHelper.Events.GameLoop.UpdateTicking += GameLoop_UpdateTicking;
        }

        private void GameLoop_UpdateTicking(object sender, StardewModdingAPI.Events.UpdateTickingEventArgs e)
        {
            int currentXp = Game1.player.experiencePoints[(int)Parameters.Skill];

            // atm the xp which lead to a level gain a not multiplied
            if (lastXp.HasValue && currentXp > lastXp)
            {
                int gainedXp = currentXp - lastXp.Value;
                int additionalXp = (int)(gainedXp * (Parameters.Multiplier - 1));
                Logger.Debug($"XP = {gainedXp} + {additionalXp}");
                Game1.player.gainExperience((int)Parameters.Skill, additionalXp);
            }

            lastXp = Game1.player.experiencePoints[(int)Parameters.Skill];
        }

        public override void Remove(Item sourceItem, EffectChangeReason reason)
        {
            EffectHelper.ModHelper.Events.GameLoop.UpdateTicking -= GameLoop_UpdateTicking;
        }
    }

    public class MultiplyExperienceParameters : IEffectParameters
    {
        public Skill Skill { get; set; } = Skill.Farming;
        public float Multiplier { get; set; } = 1.2f;

        public static MultiplyExperienceParameters With(Skill skill, float multiplier)
        {
            return new MultiplyExperienceParameters() { Skill = skill, Multiplier = multiplier };
        }
    }
}
