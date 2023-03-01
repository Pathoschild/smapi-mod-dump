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

/// <summary>Indicates to a factory that an implicitly-used marked symbol should only be instantiated when a third-party mod is installed, or adds third-party mod metadata to an explicitly-instantiated class.</summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class IgnoreWithModAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="IgnoreWithModAttribute"/> class.</summary>
    /// <param name="uniqueId">The required mod's unique ID.</param>
    /// <param name="name">A human-readable name for the mod.</param>
    /// <param name="version">The minimum required version.</param>
    public IgnoreWithModAttribute(string uniqueId, string? name = null, string? version = null)
    {
        this.UniqueId = uniqueId;
        this.Name = name ?? uniqueId;
    }

    /// <summary>Gets the required mod's unique ID.</summary>
    public string UniqueId { get; }

    /// <summary>Gets the human-readable name of the mod.</summary>
    public string Name { get; }
}
