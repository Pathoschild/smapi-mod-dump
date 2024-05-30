/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using BirbCore.Attributes;
using MoonShared.Config;

namespace CookingSkill
{
    [SConfig]
    public class Config
    {

        [SConfig.Option(0, 2, 1)]
        public int AlternativeSkillPageIcon { get; set; } = 0;


        [SConfig.Option(0, 100, 1)]
        public int ExperienceFromCooking { get; set; } = 2;


        [SConfig.Option(0.0f, 1.0f, 0.1f)]
        public float ExperienceFromEdibility { get; set; } = 0.50f;


        [SConfig.Option(1, 4000, 1)]
        public int BonusExpLimit { get; set; } = 11;
    }
}
