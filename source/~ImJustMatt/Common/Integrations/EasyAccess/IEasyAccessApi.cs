/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Integrations.EasyAccess;

using System.Collections.Generic;
using StardewModdingAPI;

/// <summary>
///     API for Easy Access.
/// </summary>
public interface IEasyAccessApi
{
    /// <summary>
    ///     Adds GMCM options for producer data.
    /// </summary>
    /// <param name="manifest">The mod's manifest.</param>
    /// <param name="data">A dictionary of key/value strings representing producer data.</param>
    public void AddProducerOptions(IManifest manifest, IDictionary<string, string> data);

    /// <summary>
    ///     Registers a mod data key to use to find the producer name.
    /// </summary>
    /// <param name="key">The mod data key to register.</param>
    public void RegisterModDataKey(string key);

    /// <summary>
    ///     Registers an Item as a producer based on its name.
    /// </summary>
    /// <param name="name">The name of the producer to register.</param>
    /// <returns>True if the data was successfully saved.</returns>
    public bool RegisterProducer(string name);
}