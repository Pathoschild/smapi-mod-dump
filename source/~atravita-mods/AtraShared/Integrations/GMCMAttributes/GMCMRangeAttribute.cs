/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

namespace AtraShared.Integrations.GMCMAttributes;

/// <summary>
/// Defines the range to use with GMCM.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class GMCMRangeAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GMCMRangeAttribute"/> class.
    /// </summary>
    /// <param name="min">minimum value.</param>
    /// <param name="max">maximum value.</param>
    public GMCMRangeAttribute(double min, double max)
    {
        this.Min = min;
        this.Max = max;
    }

    /// <summary>
    /// Gets the min value.
    /// </summary>
    internal double Min { get; init; }

    /// <summary>
    /// Gets the max value.
    /// </summary>
    internal double Max { get; init; }
}