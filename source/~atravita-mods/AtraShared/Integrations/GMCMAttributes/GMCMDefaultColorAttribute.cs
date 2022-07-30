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
/// Attribute to set the default color for a GMCM element.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class GMCMDefaultColorAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GMCMDefaultColorAttribute"/> class.
    /// </summary>
    /// <param name="r">red component.</param>
    /// <param name="g">green component.</param>
    /// <param name="b">blue component.</param>
    /// <param name="a">alpha.</param>
    public GMCMDefaultColorAttribute(byte r, byte g, byte b, byte a)
    {
        this.R = r;
        this.G = g;
        this.B = b;
        this.A = a;
    }

    /// <summary>
    /// Gets the red component.
    /// </summary>
    internal byte R { get; init; }

    /// <summary>
    /// Gets the green component.
    /// </summary>
    internal byte G { get; init; }

    /// <summary>
    /// Gets the blue component.
    /// </summary>
    internal byte B { get; init; }

    /// <summary>
    /// Gets the alpha.
    /// </summary>
    internal byte A { get; init; }
}
