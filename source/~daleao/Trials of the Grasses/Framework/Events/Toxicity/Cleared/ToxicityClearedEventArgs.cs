/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Alchemy.Framework.Events.Toxicity;

#region using directives

using StardewValley;
using System;

#endregion using directives

/// <summary>The arguments for a <see cref="ToxicityClearedEvent"/>.</summary>
internal class ToxicityClearedEventArgs : EventArgs, IToxicityClearedEventArgs
{
    /// <inheritdoc />
    public Farmer Player { get; }

    /// <summary>Construct an instance.</summary>
    /// <param name="player">The player who triggered the event.</param>
    internal ToxicityClearedEventArgs(Farmer player)
    {
        Player = player;
    }
}