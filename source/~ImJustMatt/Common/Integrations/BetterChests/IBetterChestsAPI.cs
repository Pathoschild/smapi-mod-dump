/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace Common.Integrations.BetterChests;

using System.Collections.Generic;
using StardewModdingAPI;

/// <summary>
///     API for Better Chests.
/// </summary>
public interface IBetterChestsApi
{
    /// <summary>
    ///     Adds GMCM options for storage data.
    /// </summary>
    /// <param name="manifest">The mod's manifest.</param>
    /// <param name="data">A dictionary of key/value strings representing storage data.</param>
    public void AddChestOptions(IManifest manifest, IDictionary<string, string> data);

    /// <summary>
    ///     Registers an Item as a storage based on its name.
    /// </summary>
    /// <param name="name">The name of the storage to register.</param>
    /// <returns>True if the data was successfully saved.</returns>
    public bool RegisterChest(string name);

    /// <summary>
    ///     Registers a mod data key to use to find the storage name.
    /// </summary>
    /// <param name="key">The mod data key to register.</param>
    public void RegisterModDataKey(string key);
}