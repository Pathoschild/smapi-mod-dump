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
/// Assigns a property to a GMCM section.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class GMCMSectionAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GMCMSectionAttribute"/> class.
    /// </summary>
    /// <param name="name">The (internal) name of the section.</param>
    /// <param name="order">The ordering the section should be in.</param>
    public GMCMSectionAttribute(string name, int order)
    {
        this.Name = name;
        this.Order = order;
    }

    /// <summary>
    /// Gets the ordering.
    /// </summary>
    internal int Order { get; init; }

    /// <summary>
    /// Gets the internal name for the section.
    /// </summary>
    internal string Name { get; init; }
}
