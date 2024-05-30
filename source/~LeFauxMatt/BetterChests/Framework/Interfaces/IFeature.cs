/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Framework.Interfaces;

/// <summary>Implementation of a Better Chest feature.</summary>
internal interface IFeature
{
    /// <summary>Gets a value indicating whether the feature should be active.</summary>
    public bool ShouldBeActive { get; }
}