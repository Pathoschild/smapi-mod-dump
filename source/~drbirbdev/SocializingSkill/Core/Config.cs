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

namespace SocializingSkill
{
    [ConfigClass(I18NNameSuffix = "")]
    internal class Config
    {
        [ConfigPageLink("Experience", "Experience Modifiers")]
        [ConfigPageLink("Perks", "Skill Perk Modifiers")]
        [ConfigPageLink("Professions", "Profession Modifiers")]

        [ConfigPage("Experience")]
        [ConfigSectionTitle("Experience Modifiers")]
        [ConfigOption(Min = 0, Max = 100, Interval = 1)]
        public int ExperienceFromTalking { get; set; } = 2;
        [ConfigOption(Min = 0, Max = 100, Interval = 1)]
        public int ExperienceFromGifts { get; set; } = 5;
        [ConfigOption(Min = 0, Max = 100, Interval = 1)]
        public int ExperienceFromEvents { get; set; } = 10;
        [ConfigOption(Min = 0, Max = 100, Interval = 1)]
        public int ExperienceFromQuests { get; set; } = 20;

        [ConfigPage("Perks")]
        [ConfigSectionTitle("Skill Perk Modifiers")]
        [ConfigOption(Min = 1, Max = 10, Interval = 1)]
        public int FriendshipRecoveryPerLevel { get; set; } = 1;

        [ConfigPage("Professions")]
        [ConfigSectionTitle("Profession Modifiers")]

        [ConfigSectionTitle("Friendly Profession Modifiers")]
        [ConfigOption(Min = 0, Max = 100, Interval = 1)]
        public int FriendlyExtraFriendship { get; set; } = 10;


        [ConfigSectionTitle("Smooth Talker Profession Modifiers")]
        [ConfigOption(Min = -5.0f, Max = 5.0f, Interval = 0.1f)]
        public float SmoothTalkerPositiveMultiplier { get; set; } = 2.0f;
        [ConfigOption(Min = -5.0f, Max = 5.0f, Interval = 0.1f)]
        public float SmoothTalkerNegativeMultiplier { get; set; } = 0.5f;


        [ConfigSectionTitle("Gifter Profession Modifiers")]
        [ConfigOption(Min = 0, Max = 100, Interval = 1)]
        public int GifterLovedGiftExtraFriendship { get; set; } = 80;
        [ConfigOption(Min = 0, Max = 100, Interval = 1)]
        public int GifterLikedGiftExtraFriendship { get; set; } = 40;
        [ConfigOption(Min = 0, Max = 100, Interval = 1)]
        public int GifterNeutralGiftExtraFriendship { get; set; } = 20;

        [ConfigSectionTitle("Helpful Profession Modifiers")]
        [ConfigOption(Min = 1.0f, Max = 5.0f, Interval = 0.1f)]
        public float HelpfulRewardMultiplier { get; set; } = 2.0f;


        [ConfigSectionTitle("Haggler Profession Modifiers")]
        [ConfigOption(Min = 1, Max = 10, Interval = 1)]
        public int HagglerMinHeartLevel { get; set; } = 6;
        [ConfigOption(Min = 0, Max = 10, Interval = 1)]
        public int HagglerDiscountPercentPerHeartLevel { get; set; } = 2;


        [ConfigSectionTitle("Beloved Profession Modifiers")]
        [ConfigOption(Min = 0, Max = 100, Interval = 1)]
        public int BelovedGiftPercentChance { get; set; } = 5;
        [ConfigOption(Min = 0, Max = 100, Interval = 1)]
        public int BelovedGiftRarePercentChance { get; set; } = 30;
        [ConfigOption(Min = 0, Max = 100, Interval = 1)]
        public int BelovedGiftSuperRarePercentChance { get; set; } = 30;
    }
}
