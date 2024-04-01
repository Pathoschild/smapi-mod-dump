/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.SpritePatcher.Framework.Interfaces;

using StardewMods.SpritePatcher.Framework.Enums.Patches;

/// <summary>Mod config data for Sprite Patcher.</summary>
public interface IModConfig
{
    /// <summary>Gets a value indicating whether developer mode is enabled.</summary>
    bool DeveloperMode { get; }

    /// <summary>Gets a value indicating which objects are enabled.</summary>
    Dictionary<AllPatches, bool> PatchedObjects { get; }
}