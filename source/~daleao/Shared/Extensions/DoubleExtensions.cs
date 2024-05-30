/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Shared.Extensions;

/// <summary>Extensions for the primitive <see cref="double"/> type.</summary>
public static class DoubleExtensions
{
    /// <summary>Determines whether the <paramref name="a"/> and <paramref name="b"/> are approximately equal, with uncertainty <paramref name="eps"/>.</summary>
    /// <param name="a">The first <see cref="double"/> value.</param>
    /// <param name="b">The second <see cref="double"/> value.</param>
    /// <param name="eps">The uncertainty.</param>
    /// <returns><see langword="true"/> if the difference between <paramref name="a"/> and <paramref name="b"/> is less than a factor of <c>1E-15</c>, otherwise <see langword="false"/>.</returns>
    public static bool Approx(this double a, double b, double? eps = null)
    {
        eps ??= Math.Max(Math.Abs(a), Math.Abs(b)) * 1E-15;
        return Math.Abs(a - b) < eps;
    }
}
