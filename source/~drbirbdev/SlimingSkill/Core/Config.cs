/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BirbShared.Config;

namespace SlimingSkill
{
    [ConfigClass(I18NNameSuffix = "", I18NTextSuffix = "")]
    internal class Config
    {
        [ConfigPageLink("Experience", "ExperienceModifiers")]
        [ConfigPageLink("Perks", "SkillPerkModifiers")]
        [ConfigPageLink("Professions", "ProfessionModifiers")]

        [ConfigPage("Experience")]
        [ConfigSectionTitle("ExperienceModifiers")]
        [ConfigOption(Min = 0, Max = 100, Interval = 1)]
        public int ExperienceFromSlimeKill { get; set; } = 5;
        [ConfigOption(Min = 0, Max = 100, Interval = 1)]
        public int ExperienceFromToughSlimeKill { get; set; } = 20;
        [ConfigOption(Min = 0, Max = 100, Interval = 1)]
        public int ExperienceFromRareSlimeKill { get; set; } = 50;
        [ConfigOption(Min = 0, Max = 100, Interval = 1)]
        public int ExperienceFromSlimeEgg { get; set; } = 30;
        [ConfigOption(Min = 0, Max = 100, Interval = 1)]
        public int ExperienceFromSlimeEggPress { get; set; } = 15;
        [ConfigOption(Min = 0, Max = 100, Interval = 1)]
        public int ExperienceFromSlimeBall { get; set; } = 50;

        [ConfigPage("Perks")]
        [ConfigSectionTitle("SkillPerkModifiers")]
        [ConfigOption(Min = 0, Max = 10, Interval = 1)]
        public int ExtraSlimeValuePerLevel { get; set; } = 1;

        [ConfigPage("Professions")]
        [ConfigSectionTitle("ProfessionModifiers")]

        [ConfigSectionTitle("RancherProfessionModifiers")]


        [ConfigSectionTitle("BreederProfessionModifiers")]


        [ConfigSectionTitle("HatcherProfessionModifiers")]


        [ConfigSectionTitle("HunterProfessionModifiers")]


        [ConfigSectionTitle("PoacherProfessionModifiers")]


        [ConfigSectionTitle("TamerProfessionModifier")]
        public int Dummy { get; set; }
    }
}
