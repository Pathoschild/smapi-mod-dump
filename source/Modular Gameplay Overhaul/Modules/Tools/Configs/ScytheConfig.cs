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

#endregion using directives

/// <summary>Configs related to the Scythe.</summary>
public sealed class ScytheConfig
{
    /// <summary>Gets the radius of the regular Scythe.</summary>
    [JsonProperty]
    public uint RegularRadius { get; internal set; } = 2;

    /// <summary>Gets the radius of the Golden Scythe.</summary>
    [JsonProperty]
    public uint GoldRadius { get; internal set; } = 4;

    /// <summary>Gets a value indicating whether to clear tree saplings.</summary>
    [JsonProperty]
    public bool ClearTreeSaplings { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to harvest crops.</summary>
    [JsonProperty]
    public bool HarvestCrops { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to harvest flowers.</summary>
    [JsonProperty]
    public bool HarvestFlowers { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to harvest forage.</summary>
    [JsonProperty]
    public bool HarvestForage { get; internal set; } = false;

    /// <summary>Gets a value indicating whether the harvest settings apply only to Golden Scythe.</summary>
    [JsonProperty]
    public bool GoldScytheOnly { get; internal set; } = false;

    /// <summary>Gets a value indicating whether the scythes can be enchanted with Haymaker.</summary>
    [JsonProperty]
    public bool AllowHaymakerEnchantment { get; internal set; } = true;
}
