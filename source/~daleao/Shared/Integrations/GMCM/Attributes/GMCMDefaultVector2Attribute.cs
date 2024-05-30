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

/// <summary>Assigns a default value to a GMCM <see cref="Microsoft.Xna.Framework.Vector2"/> property.</summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class GMCMDefaultVector2Attribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="GMCMDefaultVector2Attribute"/> class.</summary>
    /// <param name="x">The default X coordinate.</param>
    /// <param name="y">The default Y coordinate.</param>
    public GMCMDefaultVector2Attribute(float x, float y)
    {
        this.X = x;
        this.Y = y;
    }

    /// <summary>Gets the default X coordinate.</summary>
    public float X { get; }

    /// <summary>Gets the default Y coordinate.</summary>
    public float Y { get; }
}
