/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using SkillfulClothes.Effects.SharedParameters;
using SkillfulClothes.Types;
using StardewValley;
using StardewValley.Buffs;
using StardewValley.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Effects.Skills
{
    class IncreaseSkillLevel : ChangeSkillEffect<IncreaseSkillLevelParameters>
    {        
        public override string SkillName => Parameters.Skill.ToString();

        protected override EffectIcon Icon => Parameters?.Skill.GetIcon() ?? EffectIcon.None;

        protected override void UpdateEffects(Farmer farmer, BuffEffects targetEffects)
        {
            switch (Parameters.Skill)
            {
                case Skill.Farming: targetEffects.FarmingLevel.Value = Parameters.Amount; break;
                case Skill.Fishing: targetEffects.FishingLevel.Value = Parameters.Amount; break;
                case Skill.Foraging: targetEffects.ForagingLevel.Value = Parameters.Amount; break;
                case Skill.Mining: targetEffects.MiningLevel.Value = Parameters.Amount; break;
                case Skill.Combat: targetEffects.CombatLevel.Value = Parameters.Amount; break;
                case Skill.Luck: targetEffects.LuckLevel.Value = Parameters.Amount; break;
            }
        }

        public IncreaseSkillLevel(IncreaseSkillLevelParameters parameters)
            : base(parameters)
        {
            // --
        }

        public IncreaseSkillLevel(Skill skill, int amount)
            : base(IncreaseSkillLevelParameters.With(skill, amount))
        {
            // --
        }
    }

    public class IncreaseSkillLevelParameters : AmountEffectParameters
    {
        public Skill Skill { get; set; }

        public static IncreaseSkillLevelParameters With(Skill skill, int amount)
        {
            return new IncreaseSkillLevelParameters() { Skill = skill, Amount = amount };
        }
    }
}
