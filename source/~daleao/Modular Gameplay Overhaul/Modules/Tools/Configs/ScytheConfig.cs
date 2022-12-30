/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
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

    /// <summary>Gets a value indicating whether the scythes can be enchanted with Haymaker.</summary>
    [JsonProperty]
    public bool AllowHaymakerEnchantment { get; internal set; } = true;
}
