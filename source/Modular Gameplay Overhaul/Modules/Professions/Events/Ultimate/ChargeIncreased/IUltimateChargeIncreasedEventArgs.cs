/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Events.Ultimate.ChargeIncreased;

/// <summary>Interface for the arguments of an <see cref="UltimateChargeIncreasedEvent"/>.</summary>
public interface IUltimateChargeIncreasedEventArgs
{
    /// <summary>Gets the player who triggered the event.</summary>
    Farmer Player { get; }

    /// <summary>Gets the previous charge value.</summary>
    double OldValue { get; }

    /// <summary>Gets the new charge value.</summary>
    double NewValue { get; }
}
