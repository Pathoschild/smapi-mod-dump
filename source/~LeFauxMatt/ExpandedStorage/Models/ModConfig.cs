/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.ExpandedStorage.Models;

using System.Collections.Generic;

/// <summary>
///     Mod config data for Expanded Storage.
/// </summary>
internal sealed class ModConfig
{
    /// <summary>
    ///     Config options for each Expanded Storage chest type.
    /// </summary>
    public Dictionary<string, StorageConfig> Config = new();

    /// <summary>
    ///     Copies all ModConfig data to another ModConfig instance.
    /// </summary>
    /// <param name="other">The ModConfig instance to copy to.</param>
    public void CopyTo(ModConfig other)
    {
        var defaultConfig = new StorageConfig();

        foreach (var (id, config) in other.Config)
        {
            if (this.Config.TryGetValue(id, out var thisConfig))
            {
                thisConfig.CopyTo(config);
                continue;
            }

            defaultConfig.CopyTo(config);
        }
    }
}