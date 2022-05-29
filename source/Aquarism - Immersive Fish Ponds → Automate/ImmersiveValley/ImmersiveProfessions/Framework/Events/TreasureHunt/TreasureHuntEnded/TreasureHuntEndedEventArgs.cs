/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.TreasureHunt;

#region using directives

using System;
using StardewValley;

#endregion using directives

public class TreasureHuntEndedEventArgs : EventArgs, ITreasureHuntEndedEventArgs
{
    /// <inheritdoc />
    public Farmer Player { get; }

    /// <inheritdoc />
    public bool TreasureFound { get; }

    /// <summary>Construct an instance.</summary>
    internal TreasureHuntEndedEventArgs(Farmer player, bool found)
    {
        Player = player;
        TreasureFound = found;
    }
}