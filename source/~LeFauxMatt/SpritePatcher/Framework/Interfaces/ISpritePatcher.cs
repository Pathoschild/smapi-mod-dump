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

/// <summary>Implementation of draw patches.</summary>
public interface ISpritePatcher
{
    /// <summary>Gets a unique identifier associated the patches.</summary>
    public string Id { get; }

    /// <summary>Gets the <see cref="AllPatches" /> value that corresponds to the patched object.</summary>
    public AllPatches Type { get; }
}