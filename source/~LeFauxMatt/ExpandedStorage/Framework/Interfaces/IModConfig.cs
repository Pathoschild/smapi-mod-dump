/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.ExpandedStorage.Framework.Interfaces;

using StardewMods.Common.Models;

/// <summary>Mod config data for Expanded Storage.</summary>
internal interface IModConfig
{
    /// <summary>Gets the default options for different storage types.</summary>
    public Dictionary<string, DefaultStorageOptions> StorageOptions { get; }
}