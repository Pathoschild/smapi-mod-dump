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

namespace BinningSkill;

[SConfig]
[SToken]
public class Config
{
    [SConfig.PageLink("Experience")]
    [SConfig.PageLink("BonusDrops")]
    [SConfig.PageLink("Professions")]
    [SConfig.PageBlock("Experience")]
    [SConfig.SectionTitle("ExperienceModifiers")]
    [SConfig.Option(0, 100)]
    public int ExperienceFromCheckingTrash = 5;

    [SConfig.Option(0, 100)]
    public int ExperienceFromCheckingRecycling = 2;

    [SConfig.Option(0, 100)]
    public int ExperienceFromComposting = 2;

    [SConfig.Option(0, 100)]
    public int ExperienceFromRecycling = 2;

    // Increase in chance of any drop per level.
    [SConfig.PageBlock("BonusDrops")]
    [SConfig.SectionTitle("BonusDropModifiers")]
    [SConfig.Option(0, 0.1f, 0.001f)]
    public float PerLevelBaseDropChanceBonus = 0.03f;

    // Increase in chance for rare drops (using drbirbdev.BinningSkill_RANDOM condition
    [SConfig.Option(0, 0.1f, 0.001f)]
    public float PerLevelRareDropChanceBonus = 0.001f;

    // What level to Mega drops become available
    [SConfig.Option(0, 10)]
    public int MegaMinLevel = 4;

    // What level to DoubleMega drops become available
    [SConfig.Option(0, 10)]
    public int DoubleMegaMinLevel = 7;

    [SConfig.PageBlock("Professions")]
    [SConfig.SectionTitle("ProfessionModifiers")]


    // Recycler
    // No configs associated
    [SConfig.SectionTitle("RecyclerProfessionModifiers")]
    [SConfig.Paragraph("None")]


    // Environmentalist
    [SConfig.SectionTitle("EnvironmentalistProfessionModifiers")]
    // Gain friendship for every N recyclables
    [SConfig.Option(100, 10000, 100)]
    public int EnvironmentalistRecyclingCountToGainFriendship = 1000;

    // Amount of friendship to gain each time
    [SConfig.Option(0, 100)]
    public int EnvironmentalistRecyclingFriendshipGain = 10;

    // Additional friendship if prestiged
    [SConfig.Option(0, 100)]
    public int EnvironmentalistPrestigeRecyclingFriendshipGain = 10;


    // Salvager
    [SConfig.SectionTitle("SalvagerProfessionModifiers")]

    [SConfig.Option(1, 6)]
    [SToken.FieldToken]
    public int SalvagerCommonDropMin = 3;

    [SConfig.Option(1, 6)]
    [SToken.FieldToken]
    public int SalvagerCommonDropMax = 5;

    [SConfig.Option(1, 6)]
    [SToken.FieldToken]
    public int SalvagerRareDropMin = 1;

    [SConfig.Option(1, 6)]
    [SToken.FieldToken]
    public int SalvagerRareDropMax = 3;


    // Sneak
    [SConfig.SectionTitle("SneakProfessionModifiers")]
    // How quiet is sneaking.  Default noise range is 7, so a value of 7 removes all noise.
    [SConfig.Option(0, 25)]
    public int SneakNoiseReduction = 5;

    // How loud when digging through trash gives friendship.
    [SConfig.Option(0, 25)]
    public int SneakPrestigeNoiseIncrease = 0;


    // Upseller
    [SConfig.SectionTitle("UpsellerProfessionModifiers")]
    [SConfig.Paragraph("None")]
    // No configs associated


    // Reclaimer
    [SConfig.SectionTitle("ReclaimerProfessionModifiers")]
    // The amount of extra value that the reclaimer skill provides.
    [SConfig.Option(0, 1, 0.01f)]
    public float ReclaimerExtraValuePercent = 0.2f;

    [SConfig.Option(0, 1, 0.01f)]
    public float ReclaimerPrestigeExtraValuePercent = 0.2f;
}
