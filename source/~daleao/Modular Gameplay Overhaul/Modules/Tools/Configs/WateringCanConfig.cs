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

/// <summary>Configs related to the <see cref="StardewValley.Tools.WateringCan"/>.</summary>
public sealed class WateringCanConfig
{
    /// <summary>Gets the multiplier to base stamina consumed by the <see cref="Axe"/>.</summary>
    [JsonProperty]
    public float BaseStaminaMultiplier { get; internal set; } = 1f;

    /// <summary>Gets a value indicating whether use custom tile area for the Watering Can. Keep this at false if using defaults to improve performance.</summary>
    [JsonProperty]
    public bool OverrideAffectedTiles { get; internal set; } = false;

    /// <summary>Gets the area of affected tiles at each power level for the Can, in units lengths x units radius.</summary>
    /// <remarks>Note that radius extends to both sides of the farmer.</remarks>
    [JsonProperty]
    public (uint Length, uint Radius)[] AffectedTilesAtEachPowerLevel { get; internal set; } =
    {
        (3, 0),
        (5, 0),
        (3, 1),
        (6, 1),
        (5, 2),
    };

    /// <summary>Gets a value indicating whether the Watering Can can be enchanted with Master.</summary>
    [JsonProperty]
    public bool AllowMasterEnchantment { get; internal set; } = true;

    /// <summary>Gets a value indicating whether the Watering Can can be enchanted with Swift.</summary>
    [JsonProperty]
    public bool AllowSwiftEnchantment { get; internal set; } = true;
}
