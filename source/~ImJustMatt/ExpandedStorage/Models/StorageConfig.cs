/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.ExpandedStorage.Models;

using StardewMods.ExpandedStorage.Framework;

/// <summary>
///     Config data for an Expanded Storage chest.
/// </summary>
internal sealed class StorageConfig
{
    /// <summary>
    ///     Gets or sets data for integration with Better Chests.
    /// </summary>
    public BetterChestsData? BetterChestsData { get; set; }

    /// <summary>
    ///     Copies all StorageConfig data to another StorageConfig instance.
    /// </summary>
    /// <param name="other">The StorageConfig instance to copy to.</param>
    public void CopyTo(StorageConfig other)
    {
        if (this.BetterChestsData is null)
        {
            return;
        }

        other.BetterChestsData ??= new();
        this.BetterChestsData.CopyTo(other.BetterChestsData);
    }
}