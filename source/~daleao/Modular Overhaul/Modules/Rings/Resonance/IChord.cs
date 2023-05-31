/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
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
    /// <summary>Gets the <see cref="Gemstone"/>s that make up the <see cref="Chord"/>.</summary>
    /// <remarks>
    ///     The notes are sorted by resulting harmony, with the <see cref="Root"/> at index zero and remaining notes
    ///     ordered by increasing intervals with the former.
    /// </remarks>
    Gemstone[] Notes { get; }

    /// <summary>
    ///     Gets the root <see cref="Gemstone"/> of the <see cref="IChord"/>, which determines the
    ///     perceived wavelength.
    /// </summary>
    Gemstone? Root { get; }

    /// <summary>Gets the amplitude of the <see cref="Root"/> note's resonance.</summary>
    double Amplitude { get; }
}
