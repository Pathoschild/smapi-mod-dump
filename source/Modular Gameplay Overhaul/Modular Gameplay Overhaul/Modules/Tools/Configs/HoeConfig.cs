/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Tools.Configs;

#region using directives

using Newtonsoft.Json;
using StardewValley.Tools;

#endregion using directives

/// <summary>Configs related to the <see cref="StardewValley.Tools.Hoe"/>.</summary>
public sealed class HoeConfig
{
    /// <summary>Gets the area of affected tiles at each power level for the Hoe, in units lengths x units radius.</summary>
    /// <remarks>Note that radius extends to both sides of the farmer.</remarks>
    [JsonProperty]
    public uint[][] AffectedTiles { get; internal set; } =
    {
        new uint[] { 3, 0 },
        new uint[] { 5, 0 },
        new uint[] { 3, 1 },
        new uint[] { 6, 1 },
        new uint[] { 5, 2 },
    };

    /// <summary>
    ///     Gets a value indicating whether to apply custom tile area for the Hoe. Keep this at false if using defaults to improve
    ///     performance.
    /// </summary>
    [JsonProperty]
    public bool OverrideAffectedTiles { get; internal set; } = false;

    /// <summary>Gets a value indicating whether the Hoe can be enchanted with Master.</summary>
    [JsonProperty]
    public bool AllowMasterEnchantment { get; internal set; } = true;

    /// <summary>Gets the multiplier to base stamina consumed by the <see cref="Axe"/>.</summary>
    [JsonProperty]
    public float BaseStaminaMultiplier { get; internal set; } = 1f;
}
