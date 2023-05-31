/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Alchemy.Events.Toxicity.Overdosed;

#region using directives

using StardewValley;

#endregion using directives

/// <summary>Interface for the arguments of a <see cref="PlayerOverdosedEvent"/>.</summary>
public interface IPlayerOverdosedEventArgs
{
    /// <summary>Gets the player who triggered the event.</summary>
    Farmer Player { get; }
}
