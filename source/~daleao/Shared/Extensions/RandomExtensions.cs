/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Shared.Extensions;

/// <summary>Extensions for the <see cref="Random"/> class.</summary>
public static class RandomExtensions
{
    /// <summary>Generates a random boolean value with the the specified probability of success.</summary>
    /// <param name="r">The <see cref="Random"/> number generator.</param>
    /// <param name="p">The p of success (i.e., <see langword="true"/>).</param>
    /// <returns><see langword="true"/> or <see langword="false"/> values with a Binomial distribution and success probability <paramref name="p"/>.</returns>
    public static bool NextBool(this Random r, double p = 0.5)
    {
        return r.NextDouble() < p;
    }

    /// <summary>Samples a random decimal value from a Gaussian distribution with specified <paramref name="mean"/> and <paramref name="stddev"/> using the Box-Muller Transform.</summary>
    /// <param name="r">The <see cref="Random"/> number generator.</param>
    /// <param name="mean">The mean of the Gaussian distribution.</param>
    /// <param name="stddev">The standard deviation of the Gaussian distribution.</param>
    /// <returns>A sample from the resulting Gaussian distribution.</returns>
    public static double NextGaussian(this Random r, double mean = 0d, double stddev = 1d)
    {
        // The method requires sampling from a uniform random of (0,1]
        // but Random.NextDouble() returns a sample of [0,1).
        var u1 = 1.0 - r.NextDouble();
        var u2 = 1.0 - r.NextDouble();
        return mean + (stddev * Math.Sqrt(-2d * Math.Log(u1)) * Math.Cos(2d * Math.PI * u2));
    }
}
