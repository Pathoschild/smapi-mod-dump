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
/// Attribute to set the default vector for a GMCM element.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class GMCMDefaultVectorAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GMCMDefaultVectorAttribute"/> class.
    /// </summary>
    /// <param name="x">x coordinate.</param>
    /// <param name="y">y coordinate.</param>
    public GMCMDefaultVectorAttribute(float x, float y)
    {
        this.X = x;
        this.Y = y;
    }

    /// <summary>
    /// Gets the default X coordinate.
    /// </summary>
    internal float X { get; init; }

    /// <summary>
    /// Gets the default Y coordinate.
    /// </summary>
    internal float Y { get; init; }
}
