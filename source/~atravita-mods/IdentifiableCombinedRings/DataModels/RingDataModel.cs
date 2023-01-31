/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

namespace IdentifiableCombinedRings.DataModels;

internal readonly record struct RingPair(int first, int second);

/// <summary>
/// The data model used for rings.
/// </summary>
public sealed class RingDataModel
{
    /// <summary>
    /// Gets or sets either the name or int id of the rings.
    /// </summary>
    public string? RingIdentifiers { get; set; }

    /// <summary>
    /// Gets or sets the path to the texture to use.
    /// </summary>
    public string TextureLocation { get; set; } = string.Empty;
}
