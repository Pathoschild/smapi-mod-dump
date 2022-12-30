/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Rings.Resonance;

/// <summary>A harmonic set of <see cref="IGemstone"/> wavelengths.</summary>
/// <remarks>
///     The interference of vibration patterns between neighboring <see cref="IGemstone"/>s may amplify, dampen or
///     even create new overtones.
/// </remarks>
public interface IChord
{
    /// <summary>Gets the amplitude of the <see cref="IChord"/>.</summary>
    double Amplitude { get; }

    /// <summary>
    ///     Gets the root <see cref="Gemstone"/> of the <see cref="IChord"/>, which determines the
    ///     perceived wavelength.
    /// </summary>
    Gemstone? Root { get; }
}
