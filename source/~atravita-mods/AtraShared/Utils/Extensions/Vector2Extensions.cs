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
    /// Finds the Manhattan (taxicab) distance between two vectors.
    /// </summary>
    /// <param name="self">First vector.</param>
    /// <param name="other">Second vector.</param>
    /// <returns>Manhattan distance.</returns>
    public static float ManhattanDistance(this Vector2 self, Vector2 other)
        => Math.Abs(self.X - other.X) + Math.Abs(self.Y - other.Y);

    /// <summary>
    /// Finds the chessboard (Chebyshev) distance between two vectors.
    /// </summary>
    /// <param name="self">First vector.</param>
    /// <param name="other">Second vector.</param>
    /// <returns>Chessboard distance.</returns>
    public static float ChessboardDistance(this Vector2 self, Vector2 other)
        => Math.Max(Math.Abs(self.X - other.X), Math.Abs(self.Y - other.Y));

    /// <summary>
    /// Finds the midpoint between two vectors.
    /// </summary>
    /// <param name="self">First vector.</param>
    /// <param name="other">Second vector.</param>
    /// <returns>Midpoint.</returns>
    public static Vector2 Midpoint(this Vector2 self, Vector2 other)
        => new(self.X + ((other.X - self.X) / 2), self.Y + ((other.Y - self.Y) / 2));

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
