/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

#region using directives

using StardewValley;
using System;

#endregion using directives

/// <summary>The arguments for an <see cref="UltimateEmptiedEvent"/>.</summary>
public sealed class UltimateEmptiedEventArgs : EventArgs, IUltimateEmptiedEventArgs
{
    /// <inheritdoc />
    public Farmer Player { get; }

    /// <summary>Construct an instance.</summary>
    /// <param name="player">The player who triggered the event.</param>
    internal UltimateEmptiedEventArgs(Farmer player)
    {
        Player = player;
    }
}