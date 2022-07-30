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

/// <summary>
/// The data model used for rings. There was an idea here once.
/// </summary>
public class RingDataModel
{
    /// <summary>
    /// Either the name or int id of the rings?
    /// </summary>
    public List<string> RingIdentifiers { get; set; } = new();

    /// <summary>
    /// The path to the texture to use.
    /// </summary>
    public string TextureLocation { get; set; } = string.Empty;
}
