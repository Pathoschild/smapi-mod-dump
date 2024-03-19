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
#pragma warning disable CS0414 // Field is assigned but its value is never used

namespace PanningUpgrades;

[SConfig]
[SToken]
internal class Config
{

    [SConfig.Option]
    [SToken.FieldToken]
    public bool BuyablePan = false;

    [SConfig.Option(0, 100000, 500)]
    [SToken.FieldToken]
    public int BuyCost = 1000;

    [SConfig.Option(0, 1, 0.01f)]
    public float ExtraDrawBaseChance = 0.7f;

    [SConfig.Option(0, 1, 0.01f)]
    public float DailyLuckMultiplier = 1.0f;

    [SConfig.Option(0, 1, 0.01f)]
    public float LuckLevelMultiplier = 0.1f;

    [SConfig.SectionTitle("AnimationSection")]

    [SConfig.Paragraph("AnimationSectionText")]

    [SConfig.Option]
    public int AnimationFrameDuration = 140;

    [SConfig.Option]
    public int AnimationYOffset = -8;

}
