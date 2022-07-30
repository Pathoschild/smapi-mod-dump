/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Tools.Configs;

/// <summary>Configs related to the Hoe.</summary>
public class HoeConfig
{
    /// <summary>Whether to apply custom tile area for the Hoe. Keep this at false if using defaults to improve performance.</summary>
    public bool OverrideAffectedTiles { get; set; } = false;

    /// <summary>The area of affected tiles at each power level for the Hoe, in units lengths x units radius.</summary>
    /// <remarks>Note that radius extends to both sides of the farmer.</remarks>
    public int[][] AffectedTiles = {
        new[] {3, 0},
        new[] {5, 0},
        new[] {3, 1},
        new[] {6, 1},
        new[] {5, 2}
    };

    /// <summary>Whether the Hoe can be enchanted with Master.</summary>
    public bool AllowMasterEnchantment { get; set; } = true;
}