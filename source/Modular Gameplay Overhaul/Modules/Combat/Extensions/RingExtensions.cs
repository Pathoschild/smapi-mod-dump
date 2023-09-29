/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.Extensions;

#region using directives

using System.Diagnostics.CodeAnalysis;
using DaLion.Overhaul.Modules.Combat.Integrations;
using DaLion.Overhaul.Modules.Combat.Resonance;
using StardewValley.Objects;

#endregion using directives

/// <summary>Extensions for the <see cref="Ring"/> class.</summary>
internal static class RingExtensions
{
    /// <summary>Determines whether the <paramref name="ring"/> is one of the gemstone rings.</summary>
    /// <param name="ring">The <see cref="Ring"/>.</param>
    /// <returns>
    ///     <see langword="true"/> if the <paramref name="ring"/>'s index matches any of the gemstone ring indices, otherwise <see langword="false"/>.
    /// </returns>
    internal static bool IsGemRing(this Ring ring)
    {
        return Gemstone.TryFromRing(ring.ParentSheetIndex, out _);
    }

    /// <summary>
    ///     Determines whether the <paramref name="ring"/> is a combined instance of Infinity Band, and if so cast to
    ///     <see cref="CombinedRing"/>.
    /// </summary>
    /// <param name="ring">The <see cref="Ring"/>.</param>
    /// <param name="infinity">The ring as a <see cref="CombinedRing"/> instance.</param>
    /// <returns>
    ///     <see langword="true"/> if the <paramref name="ring"/> can be casted to <see cref="CombinedRing"/>, it's
    ///     index is that of Infinity Band, and it contains combined gemstone rings, otherwise <see langword="false"/>.
    /// </returns>
    internal static bool IsCombinedInfinityBand(this Ring ring, [NotNullWhen(true)] out CombinedRing? infinity)
    {
        if (JsonAssetsIntegration.InfinityBandIndex.HasValue && ring is CombinedRing combined &&
            combined.ParentSheetIndex == JsonAssetsIntegration.InfinityBandIndex.Value && combined.combinedRings.Count > 0)
        {
            infinity = combined;
        }
        else
        {
            infinity = null;
        }

        return infinity is not null;
    }
}
