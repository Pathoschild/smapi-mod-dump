/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraBase.Toolkit.Extensions;

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
        if (span.Trim().TrySplitOnce(',', out ReadOnlySpan<char> first, out ReadOnlySpan<char> second)
            && float.TryParse(first.Trim(), out float x) && float.TryParse(second.Trim(), out float y))
        {
            vector = new Vector2(x, y);
            return true;
        }

        vector = default;
        return false;
    }
}
