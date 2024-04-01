/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.SpritePatcher.Framework.Models.Events;

/// <summary>Represents the event arguments for the PatchesChanged event.</summary>
internal sealed class PatchesChangedEventArgs(IList<string> targets) : EventArgs
{
    /// <summary>Gets the patch targets which were changed.</summary>
    public IList<string> ChangedTargets { get; } = targets;
}