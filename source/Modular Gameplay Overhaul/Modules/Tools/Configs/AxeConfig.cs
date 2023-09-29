/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Tools.Configs;

#region using directives

using Newtonsoft.Json;
using StardewValley.Tools;

#endregion using directives

/// <summary>Configs related to the <see cref="Axe"/>.</summary>
public sealed class AxeConfig
{
    /// <summary>Gets the multiplier to base stamina consumed by the <see cref="Axe"/>.</summary>
    [JsonProperty]
    public float BaseStaminaCostMultiplier { get; internal set; } = 1f;

    /// <summary>Gets a value indicating whether enables charging the <see cref="Axe"/>.</summary>
    [JsonProperty]
    public bool EnableCharging { get; internal set; } = true;

    /// <summary>Gets axe must be at least this level to charge.</summary>
    [JsonProperty]
    public UpgradeLevel RequiredUpgradeForCharging { get; internal set; } = UpgradeLevel.Copper;

    /// <summary>Gets the radius of affected tiles at each upgrade level.</summary>
    [JsonProperty]
    public uint[] RadiusAtEachPowerLevel { get; internal set; } = { 1, 2, 3, 4, 5, 6, 7 };

    /// <summary>Gets a value which multiplies the stamina consumption for a <see cref="Shockwave"/>.</summary>
    [JsonProperty]
    public float ChargedStaminaCostMultiplier { get; internal set; } = 1f;

    /// <summary>Gets a value indicating whether to clear fruit tree seeds.</summary>
    [JsonProperty]
    public bool ClearFruitTreeSeeds { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to clear fruit trees that aren't fully grown.</summary>
    [JsonProperty]
    public bool ClearFruitTreeSaplings { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to cut down fully-grown fruit trees.</summary>
    [JsonProperty]
    public bool CutGrownFruitTrees { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to clear non-fruit tree seeds.</summary>
    [JsonProperty]
    public bool ClearTreeSeeds { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to clear non-fruit trees that aren't fully grown.</summary>
    [JsonProperty]
    public bool ClearTreeSaplings { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to cut down full-grown non-fruit trees.</summary>
    [JsonProperty]
    public bool CutGrownTrees { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to cut down non-fruit trees that have a tapper.</summary>
    [JsonProperty]
    public bool CutTappedTrees { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to harvest giant crops.</summary>
    [JsonProperty]
    public bool CutGiantCrops { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to clear bushes.</summary>
    [JsonProperty]
    public bool ClearBushes { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to clear live crops.</summary>
    [JsonProperty]
    public bool ClearLiveCrops { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to clear dead crops.</summary>
    [JsonProperty]
    public bool ClearDeadCrops { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to clear debris like twigs, giant stumps, fallen logs and weeds.</summary>
    [JsonProperty]
    public bool ClearDebris { get; internal set; } = true;

    /// <summary>Gets a value indicating whether the Axe can be enchanted with Master.</summary>
    [JsonProperty]
    public bool AllowMasterEnchantment { get; internal set; } = true;

    /// <summary>Gets a value indicating whether the Axe can be enchanted with Reaching.</summary>
    [JsonProperty]
    public bool AllowReachingEnchantment { get; internal set; } = true;
}
