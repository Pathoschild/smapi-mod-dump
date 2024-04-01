/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.SpritePatcher.Framework.Models;

using StardewMods.SpritePatcher.Framework.Enums.Patches;
using StardewMods.SpritePatcher.Framework.Interfaces;

/// <inheritdoc />
public class DefaultConfig : IModConfig
{
    /// <inheritdoc />
    public bool DeveloperMode { get; set; }

    /// <inheritdoc />
    public Dictionary<AllPatches, bool> PatchedObjects { get; set; } = new();
}