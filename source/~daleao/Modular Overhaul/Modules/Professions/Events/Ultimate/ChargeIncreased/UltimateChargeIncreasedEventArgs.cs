/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Events.Ultimate;

/// <summary>The arguments for an <see cref="UltimateChargeIncreasedEvent"/>.</summary>
public sealed class UltimateChargeIncreasedEventArgs : EventArgs, IUltimateChargeIncreasedEventArgs
{
    /// <summary>Initializes a new instance of the <see cref="UltimateChargeIncreasedEventArgs"/> class.</summary>
    /// <param name="player">The player who triggered the event.</param>
    /// <param name="oldValue">The old charge value.</param>
    /// <param name="newValue">The new charge value.</param>
    internal UltimateChargeIncreasedEventArgs(Farmer player, double oldValue, double newValue)
    {
        this.Player = player;
        this.OldValue = oldValue;
        this.NewValue = newValue;
    }

    /// <inheritdoc />
    public Farmer Player { get; }

    /// <inheritdoc />
    public double OldValue { get; }

    /// <inheritdoc />
    public double NewValue { get; }
}
