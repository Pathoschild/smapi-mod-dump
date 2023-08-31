/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Shared.Attributes;

/// <summary>Indicates to a factory that the implicitly-used marked symbol should not be instantiated when a third-party mod is installed.</summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ModConflictAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="ModConflictAttribute"/> class.</summary>
    /// <param name="uniqueId">The required mod's unique ID.</param>
    /// <param name="name">A human-readable name for the mod.</param>
    public ModConflictAttribute(string uniqueId, string? name = null)
    {
        this.UniqueId = uniqueId;
        this.Name = name ?? uniqueId;
    }

    /// <summary>Gets the required mod's unique ID.</summary>
    public string UniqueId { get; }

    /// <summary>Gets the human-readable name of the mod.</summary>
    public string Name { get; }
}
