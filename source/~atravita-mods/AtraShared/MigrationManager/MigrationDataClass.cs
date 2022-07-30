/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

namespace AtraShared.MigrationManager;

/// <summary>
/// Data class that holds information for the migrator.
/// </summary>
public class MigrationDataClass
{
    /// <summary>
    /// Gets or sets a map from the save name to the last vesrion used.
    /// </summary>
    public Dictionary<string, string> VersionMap { get; set; } = new();
}