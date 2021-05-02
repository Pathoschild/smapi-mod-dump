/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using SkillfulClothes.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Types
{
	enum Skill
	{
		Farming = 0,
		Fishing = 1,
		Foraging = 2,
        Mining = 3,						
		Combat = 4,	
		Luck = 5,
    }

	static class SkilLExtensions
    {
		public static EffectIcon GetIcon(this Skill skill)
        {
            switch (skill)
            {
                case Skill.Farming: return EffectIcon.SkillFarming;
                case Skill.Fishing: return EffectIcon.SkillFishing;
                case Skill.Foraging: return EffectIcon.SkillForaging;
                case Skill.Mining: return EffectIcon.SkillMining;
                case Skill.Combat: return EffectIcon.SkillCombat;
                case Skill.Luck: return EffectIcon.SkillLuck;
                default: return EffectIcon.None;
            }
        }
    }
}
