/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using BirbCore.Attributes;

namespace SocializingSkill;

[SConfig]
internal class Config
{
    [SConfig.PageLink("Experience")]
    [SConfig.PageLink("Perks")]
    [SConfig.PageLink("Professions")]

    [SConfig.PageBlock("Experience")]
    [SConfig.SectionTitle("ExperienceModifiers")]
    [SConfig.Option(0, 100)]
    public int ExperienceFromTalking = 5;
    [SConfig.Option(0, 100)]
    public int ExperienceFromGifts = 5;
    [SConfig.Option(0, 100)]
    public int ExperienceFromEvents = 20;
    [SConfig.Option(0, 100)]
    public int ExperienceFromQuests = 50;
    [SConfig.Option(1, 5, 0.1f)]
    public float LovedGiftExpMultiplier = 2;
    [SConfig.Option(1, 5, 0.1f)]
    public float BirthdayGiftExpMultiplier = 5;

    [SConfig.PageBlock("Perks")]
    [SConfig.SectionTitle("SkillPerkModifiers")]
    [SConfig.Option(1, 10)]
    public int ChanceNoFriendshipDecayPerLevel = 5;

    [SConfig.PageBlock("Professions")]
    [SConfig.SectionTitle("ProfessionModifiers")]

    [SConfig.SectionTitle("FriendlyProfessionModifiers")]
    [SConfig.Option(0, 100)]
    public int FriendlyExtraFriendship = 10;
    [SConfig.Option(0, 100)]
    public int FriendlyPrestigeExtraFriendship = 20;

    [SConfig.SectionTitle("SmoothTalkerProfessionModifiers")]
    [SConfig.Option(1.0f, 5.0f, 0.1f)]
    public float SmoothTalkerPositiveMultiplier = 2.0f;
    [SConfig.Option(0.0f, 1.0f, 0.1f)]
    public float SmoothTalkerNegativeMultiplier = 0.5f;
    [SConfig.Option(1.0f, 5.0f, 0.1f)]
    public float SmoothTalkerPrestigePositiveMultiplier = 3.0f;
    [SConfig.Option(0.0f, 1.0f, 0.1f)]
    public float SmoothTalkerPrestigeNegativeMultiplier = 0.0f;

    [SConfig.SectionTitle("GifterProfessionModifiers")]
    [SConfig.Option(0, 100)]
    public int GifterLovedGiftExtraFriendship = 80;
    [SConfig.Option(0, 100)]
    public int GifterLikedGiftExtraFriendship = 40;
    [SConfig.Option(0, 100)]
    public int GifterNeutralGiftExtraFriendship = 20;
    [SConfig.Option(0, 100)]
    public int GifterPrestigeExtraFriendship = 40;

    [SConfig.SectionTitle("HelpfulProfessionModifiers")]
    [SConfig.Option(1.0f, 5.0f, 0.1f)]
    public float HelpfulRewardMultiplier = 2.0f;
    [SConfig.Option(1.0f, 5.0f, 0.1f)]
    public float HelpfulPrestigeRewardMultiplier = 3.0f;


    [SConfig.SectionTitle("HagglerProfessionModifiers")]
    [SConfig.Option(1, 10)]
    public int HagglerMinHeartLevel = 6;
    [SConfig.Option(0, 10)]
    public int HagglerDiscountPercentPerHeartLevel = 2;
    [SConfig.Option(0, 100)]
    public int HagglerPrestigeDiscountPercent = 10;


    [SConfig.SectionTitle("BelovedProfessionModifiers")]
    [SConfig.Option(0, 100)]
    public int BelovedGiftPercentChance = 5;
    [SConfig.Option(0, 100)]
    public int BelovedPrestigeGiftPercentChance = 10;
}
