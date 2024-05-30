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

/// <summary>Extensions for the primitive <see cref="float"/> type.</summary>
public static class FloatExtensions
{
    /// <summary>Determines whether the <paramref name="a"/> and <paramref name="b"/> are approximately equal, with uncertainty <paramref name="eps"/>.</summary>
    /// <param name="a">The first <see cref="float"/> value.</param>
    /// <param name="b">The second <see cref="float"/> value.</param>
    /// <param name="eps">The uncertainty.</param>
    /// <returns><see langword="true"/> if the difference between <paramref name="a"/> and <paramref name="b"/> is less than a factor of <c>1E-6</c>, otherwise <see langword="false"/>.</returns>
    public static bool Approx(this float a, float b, float? eps = null)
    {
        eps ??= MathF.Max(MathF.Abs(a), MathF.Abs(b)) * 1E-6f;
        return MathF.Abs(a - b) < eps;
    }
}
