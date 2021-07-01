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
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Types
{
	public enum Skill
	{
		Farming = 0,
		Fishing = 1,
		Foraging = 2,
        Mining = 3,						
		Combat = 4,	
		Luck = 5,
    }

    public static class SkilLExtensions
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

        public static int GetCurrentLevel(this Skill skill)
        {
            switch (skill)
            {
                case Skill.Combat: return Game1.player.CombatLevel;
                case Skill.Fishing: return Game1.player.FishingLevel;
                case Skill.Farming: return Game1.player.FarmingLevel;
                case Skill.Mining: return Game1.player.MiningLevel;
                case Skill.Foraging: return Game1.player.ForagingLevel;
                default: return 0;
            }
        }
    }
}
