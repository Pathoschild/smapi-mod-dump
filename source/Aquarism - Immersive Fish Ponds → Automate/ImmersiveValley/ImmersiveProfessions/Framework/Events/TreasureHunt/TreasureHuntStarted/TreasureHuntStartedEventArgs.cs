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
using Microsoft.Xna.Framework;
using StardewValley;

#endregion using directives

public class TreasureHuntStartedEventArgs : EventArgs, ITreasureHuntStartedEventArgs
{
    /// <inheritdoc />
    public Farmer Player { get; }

    /// <inheritdoc />
    public Vector2 Target { get; }

    /// <summary>Construct an instance.</summary>
    internal TreasureHuntStartedEventArgs(Farmer player, Vector2 target)
    {
        Player = player;
        Target = target;
    }
}