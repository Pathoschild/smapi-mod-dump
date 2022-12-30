/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Rings;

#region using directives

using Newtonsoft.Json;

#endregion using directives

/// <summary>The user-configurable settings for Rings.</summary>
public sealed class Config : Shared.Configs.Config
{
    /// <summary>Gets a value indicating whether to improve certain underwhelming rings.</summary>
    [JsonProperty]
    public bool RebalancedRings { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to add new combat recipes for crafting gemstone rings.</summary>
    [JsonProperty]
    public bool CraftableGemRings { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to add new mining recipes for crafting Glow and Magnet rings.</summary>
    [JsonProperty]
    public bool CraftableGlowAndMagnetRings { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to replace the Glowstone Ring recipe.</summary>
    [JsonProperty]
    public bool ImmersiveGlowstoneRecipe { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to replace the Iridium Band recipe and effect.</summary>
    [JsonProperty]
    public bool TheOneInfinityBand { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to allow gemstone resonance to take place.</summary>
    [JsonProperty]
    public bool EnableResonance { get; internal set; } = true;
}
