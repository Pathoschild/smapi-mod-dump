/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Integrations.ExpandedStorage;

/// <summary>
///     API for ExpandedStorage.
/// </summary>
public interface IExpandedStorageApi
{
    /// <summary>
    ///     Loads an Expanded Storage content pack in the legacy format.
    /// </summary>
    /// <param name="manifest">The mod manifest.</param>
    /// <param name="path">The path to the content pack.</param>
    /// <returns>Returns true if the content pack could be loaded.</returns>
    public bool LoadContentPack(IManifest manifest, string path);

    /// <summary>
    ///     Loads an Expanded Storage content pack in the legacy format.
    /// </summary>
    /// <param name="contentPack">The content pack to load.</param>
    /// <returns>Returns true if the content pack could be loaded.</returns>
    public bool LoadContentPack(IContentPack contentPack);

    /// <summary>
    ///     Registers a custom Expanded Storage chest.
    /// </summary>
    /// <param name="id">A unique id for the custom storage.</param>
    /// <param name="storage">The custom storage to load.</param>
    /// <returns>Returns true if the storage could be loaded.</returns>
    public bool RegisterStorage(string id, ICustomStorage storage);
}