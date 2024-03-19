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

namespace RanchingToolUpgrades;

[SConfig]
[SToken]
internal class Config
{
    [SConfig.Option(0, 100000, 500)]
    [SToken.FieldToken]
    public int PailBuyCost = 1000;

    [SConfig.Option(0, 100000, 500)]
    [SToken.FieldToken]
    public int ShearsBuyCost = 1000;

    /*
    public bool BuyableAutograbber { get; set; } = true;

    public int AutograbberBuyCost { get; set; } = 25000;

    public float AutograbberUpgradeCostMultiplier { get; set; } = 5.0f;

    public int AutograbberUpgradeCostBars { get; set; } = 10;

    public int AutograbberUpgradeDays { get; set; } = 2;*/

    // N extra friendship per upgrade level.
    [SConfig.Option(0, 10)]
    public int ExtraFriendshipBase = 2;

    // N% chance of higher quality goods.
    [SConfig.Option(0, 1, 0.01f)]
    public float QualityBumpChanceBase = 0.05f;

    // N% chance of double produce.
    [SConfig.Option(0, 1, 0.01f)]
    public float ExtraProduceChance = 0.1f;
}
