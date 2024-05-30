/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Shared.Integrations.GMCM.Attributes;

/// <summary>Sets the minimum and maximum parameters of a GMCM numeric property.</summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class GMCMRangeAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="GMCMRangeAttribute"/> class.</summary>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    public GMCMRangeAttribute(float min, float max)
    {
        this.Min = min;
        this.Max = max;
    }

    /// <summary>Gets the minimum value.</summary>
    public float Min { get; }

    /// <summary>Gets the maximum value.</summary>
    public float Max { get; }
}
