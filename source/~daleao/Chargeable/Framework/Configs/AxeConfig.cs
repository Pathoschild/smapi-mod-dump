/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Chargeable.Framework.Configs;

#region using directives

using DaLion.Shared.Integrations.GMCM.Attributes;
using Newtonsoft.Json;
using StardewValley.Tools;

#endregion using directives

/// <summary>Configs related to the <see cref="Axe"/>.</summary>
public sealed class AxeConfig
{
    /// <summary>Gets a value indicating whether enables charging the <see cref="Axe"/>.</summary>
    [JsonProperty]
    [GMCMPriority(0)]
    public bool EnableCharging { get; internal set; } = true;

    /// <summary>Gets the radius of affected tiles at each upgrade level.</summary>
    [JsonProperty]
    [GMCMPriority(1)]
    [GMCMOverride(typeof(ChargeableConfigMenu), "AxeRadiusAtEachLevelOverride")]
    public uint[] RadiusAtEachPowerLevel { get; internal set; } = [1, 2, 3, 4, 5];

    /// <summary>Gets a value indicating whether to clear fruit tree seeds.</summary>
    [JsonProperty]
    [GMCMPriority(100)]
    public bool ClearFruitTreeSeeds { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to clear fruit trees that aren't fully grown.</summary>
    [JsonProperty]
    [GMCMPriority(101)]
    public bool ClearFruitTreeSaplings { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to cut down fully-grown fruit trees.</summary>
    [JsonProperty]
    [GMCMPriority(102)]
    public bool CutGrownFruitTrees { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to clear non-fruit tree seeds.</summary>
    [JsonProperty]
    [GMCMPriority(103)]
    public bool ClearTreeSeeds { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to clear non-fruit trees that aren't fully grown.</summary>
    [JsonProperty]
    [GMCMPriority(104)]
    public bool ClearTreeSaplings { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to cut down full-grown non-fruit trees.</summary>
    [JsonProperty]
    [GMCMPriority(105)]
    public bool CutGrownTrees { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to cut down non-fruit trees that have a tapper.</summary>
    [JsonProperty]
    [GMCMPriority(106)]
    public bool CutTappedTrees { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to harvest giant crops.</summary>
    [JsonProperty]
    [GMCMPriority(107)]
    public bool CutGiantCrops { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to clear bushes.</summary>
    [JsonProperty]
    [GMCMPriority(108)]
    public bool ClearBushes { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to clear live crops.</summary>
    [JsonProperty]
    [GMCMPriority(109)]
    public bool ClearLiveCrops { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to clear dead crops.</summary>
    [JsonProperty]
    [GMCMPriority(110)]
    public bool ClearDeadCrops { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to clear debris like twigs, giant stumps, fallen logs and weeds.</summary>
    [JsonProperty]
    [GMCMPriority(111)]
    public bool ClearDebris { get; internal set; } = true;

    /// <summary>Gets a value indicating whether the Axe can be enchanted with Reaching.</summary>
    [JsonProperty]
    [GMCMPriority(200)]
    public bool AllowReachingEnchantment { get; internal set; } = true;

    /// <summary>Gets the multiplier to base stamina consumed by the <see cref="Axe"/>.</summary>
    [JsonProperty]
    [GMCMPriority(201)]
    [GMCMRange(0f, 2f)]
    [GMCMStep(0.05f)]
    public float StaminaCostMultiplier { get; internal set; } = 1f;
}
