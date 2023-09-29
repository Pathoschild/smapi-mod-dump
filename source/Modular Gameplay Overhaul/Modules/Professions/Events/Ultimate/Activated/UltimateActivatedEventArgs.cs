/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Events.Ultimate.Activated;

/// <summary>The arguments for an <see cref="UltimateActivatedEvent"/>.</summary>
public sealed class UltimateActivatedEventArgs : EventArgs, IUltimateActivatedEventArgs
{
    /// <summary>Initializes a new instance of the <see cref="UltimateActivatedEventArgs"/> class.</summary>
    /// <param name="player">The player who triggered the event.</param>
    internal UltimateActivatedEventArgs(Farmer player)
    {
        this.Player = player;
    }

    /// <inheritdoc />
    public Farmer Player { get; }
}
