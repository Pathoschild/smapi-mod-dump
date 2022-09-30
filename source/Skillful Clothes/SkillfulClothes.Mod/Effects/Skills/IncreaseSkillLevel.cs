/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using Newtonsoft.Json.Linq;
using SkillfulClothes.Effects.SharedParameters;
using SkillfulClothes.Types;
using StardewValley;
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

        protected override void ChangeCurrentLevel(Farmer farmer, int amount)
        {
            switch (Parameters.Skill)
            {
                case Skill.Farming: farmer.addedFarmingLevel.Value = Math.Max(0, farmer.addedFarmingLevel.Value + amount); break;
                case Skill.Fishing: farmer.addedFishingLevel.Value = Math.Max(0, farmer.addedFishingLevel.Value + amount); break;
                case Skill.Foraging: farmer.addedForagingLevel.Value = Math.Max(0, farmer.addedForagingLevel.Value + amount); break;
                case Skill.Mining: farmer.addedMiningLevel.Value = Math.Max(0, farmer.addedMiningLevel.Value + amount); break;
                case Skill.Combat: farmer.addedCombatLevel.Value = Math.Max(0, farmer.addedCombatLevel.Value + amount); break;
                case Skill.Luck: farmer.addedLuckLevel.Value = Math.Max(0, farmer.addedLuckLevel.Value + amount); break;
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
