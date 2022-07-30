/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Integrations.BetterChests;

/// <summary>
///     API for Better Chests.
/// </summary>
public interface IBetterChestsApi
{
    /// <summary>
    ///     Registers a chest type based on any object containing the mod data key-value pair.
    /// </summary>
    /// <param name="key">The mod data key.</param>
    /// <param name="value">The mod data value.</param>
    /// <param name="storage">The storage data.</param>
    public void RegisterChest(string key, string value, IStorageData storage);
}