/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Events.Ultimate;

/// <summary>The arguments for an <see cref="UltimateEmptiedEvent"/>.</summary>
public sealed class UltimateEmptiedEventArgs : EventArgs, IUltimateEmptiedEventArgs
{
    /// <summary>Initializes a new instance of the <see cref="UltimateEmptiedEventArgs"/> class.</summary>
    /// <param name="player">The player who triggered the event.</param>
    internal UltimateEmptiedEventArgs(Farmer player)
    {
        this.Player = player;
    }

    /// <inheritdoc />
    public Farmer Player { get; }
}
