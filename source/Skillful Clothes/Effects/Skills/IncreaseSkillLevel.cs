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
    class IncreaseSkillLevel : ChangeSkillEffect
    {
        Skill skill;

        public override string SkillName => skill.ToString();

        EffectIcon icon;
        protected override EffectIcon Icon => icon;        

        protected override void ChangeCurrentLevel(Farmer farmer, int amount)
        {
            switch (skill)
            {
                case Skill.Farming: farmer.addedFarmingLevel.Value += amount; break;
                case Skill.Fishing: farmer.addedFishingLevel.Value += amount; break;
                case Skill.Foraging: farmer.addedForagingLevel.Value += amount; break;
                case Skill.Mining: farmer.addedMiningLevel.Value += amount; break;
                case Skill.Combat: farmer.addedCombatLevel.Value += amount; break;
                case Skill.Luck: farmer.addedLuckLevel.Value += amount; break;
            }
        }

        public IncreaseSkillLevel(Skill skill, int amount)
            : base(amount)
        {
            this.skill = skill;
            icon = skill.GetIcon();
        }
    }
}
