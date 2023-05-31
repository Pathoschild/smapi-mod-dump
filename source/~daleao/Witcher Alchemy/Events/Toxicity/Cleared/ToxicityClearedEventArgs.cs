/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Alchemy.Events.Toxicity.Cleared;

#region using directives

using System;
using StardewValley;

#endregion using directives

/// <summary>The arguments for a <see cref="ToxicityClearedEvent"/>.</summary>
internal class ToxicityClearedEventArgs : EventArgs, IToxicityClearedEventArgs
{
    /// <summary>Initializes a new instance of the <see cref="ToxicityClearedEventArgs"/> class.</summary>
    /// <param name="player">The player who triggered the event.</param>
    internal ToxicityClearedEventArgs(Farmer player)
    {
        this.Player = player;
    }

    /// <inheritdoc />
    public Farmer Player { get; }
}
