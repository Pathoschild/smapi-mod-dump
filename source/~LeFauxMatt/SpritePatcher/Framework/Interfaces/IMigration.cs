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

/// <summary>Migrates patches to a given format version.</summary>
internal interface IMigration
{
    /// <summary>Gets the version of the migration.</summary>
    ISemanticVersion Version { get; }
}