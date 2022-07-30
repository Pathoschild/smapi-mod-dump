/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;

namespace AtraShared.Utils.Extensions;

/// <summary>
/// Holds extensions for Vector2.
/// </summary>
public static class Vector2Extensions
{
    /// <summary>
    /// Tries to parse a vector2 from a string.
    /// </summary>
    /// <param name="str">the string.</param>
    /// <param name="vector">out param, the vector or default.</param>
    /// <returns>true if successful, false otherwise.</returns>
    public static bool TryParseVector2(this string str, out Vector2 vector)
        => str.AsSpan().TryParseVector2(out vector);

    /// <summary>
    /// Tries to parse a vector2 from a ReadOnlySpan.
    /// </summary>
    /// <param name="span">the span.</param>
    /// <param name="vector">out param, the vector or default.</param>
    /// <returns>true if successful, false otherwise.</returns>
    public static bool TryParseVector2(this ReadOnlySpan<char> span, out Vector2 vector)
    {
        span = span.Trim();

        int index = span.IndexOf(',');

        if (index <= 0 || index >= span.Length - 1 // comma should not be the first or last position
            || !float.TryParse(span[..index], out float x)
            || !float.TryParse(span[(index + 1)..], out float y))
        {
            vector = default;
            return false;
        }

        vector = new Vector2(x, y);
        return true;
    }
}
