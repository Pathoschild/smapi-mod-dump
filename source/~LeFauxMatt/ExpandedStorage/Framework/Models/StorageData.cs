/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.ExpandedStorage.Framework.Models;

using StardewMods.Common.Services.Integrations.ExpandedStorage;

/// <inheritdoc />
internal sealed class StorageData : IStorageData
{
    /// <inheritdoc />
    public string CloseNearbySound { get; set; } = "doorCreakReverse";

    /// <inheritdoc />
    public int Frames { get; set; } = 1;

    /// <inheritdoc />
    public bool IsFridge { get; set; }

    /// <inheritdoc />
    public bool OpenNearby { get; set; }

    /// <inheritdoc />
    public string OpenNearbySound { get; set; } = "doorCreak";

    /// <inheritdoc />
    public string OpenSound { get; set; } = "openChest";

    /// <inheritdoc />
    public string PlaceSound { get; set; } = "axe";

    /// <inheritdoc />
    public bool PlayerColor { get; set; }
}