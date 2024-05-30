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

/// <summary>Sets the step parameter of a GMCM numeric property.</summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class GMCMStepAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="GMCMStepAttribute"/> class.</summary>
    /// <param name="step">The step value.</param>
    public GMCMStepAttribute(float step)
    {
        this.Step = step;
    }

    /// <summary>Gets the interval value.</summary>
    public float Step { get; }
}
