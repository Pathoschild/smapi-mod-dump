/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

namespace HolidaySales;

/// <summary>
/// The config class for this mod.
/// </summary>
internal sealed class ModConfig
{
    public FestivalsShopBehavior StoreFestivalBehavior { get; set; } = FestivalsShopBehavior.MapDependent;
}

/// <summary>
/// The expected behavior of the shops during festivals.
/// </summary>
internal enum FestivalsShopBehavior
{
    /// <summary>
    /// Festivals close shops (default vanilla behavior).
    /// </summary>
    Closed,

    /// <summary>
    /// Festivals only close shops on "their" map.
    /// </summary>
    MapDependent,

    /// <summary>
    /// Festivals never close shops.
    /// </summary>
    Open,
}
