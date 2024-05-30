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

/// <summary>Assigns a priority to GMCM property.</summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class GMCMPriorityAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="GMCMPriorityAttribute"/> class.</summary>
    /// <param name="priority">The priority of the property in the page.</param>
    public GMCMPriorityAttribute(uint priority)
    {
        this.Priority = priority;
    }

    /// <summary>Gets the priority of the property in the page.</summary>
    public uint Priority { get; }
}
