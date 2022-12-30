/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Extensions;

#region using directives

using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Extensions for the <see cref="Vector2"/> struct.</summary>
internal static class Vector2Extensions
{
    /// <summary>Draws a pointer over the <paramref name="tile"/> if it is inside the current viewport.</summary>
    /// <param name="tile">The <see cref="Vector2"/> tile.</param>
    /// <param name="color">The desired color for the pointer.</param>
    internal static void TrackWhenOnScreen(this Vector2 tile, Color color)
    {
        Globals.Pointer.Value.DrawOverTile(tile, color);
    }

    /// <summary>
    ///     Draws a pointer at the edge of the screen, pointing to the <paramref name="tile"/>, if it is outside the
    ///     current viewport.
    /// </summary>
    /// <param name="tile">The <see cref="Vector2"/> tile.</param>
    /// <param name="color">The desired color for the pointer.</param>
    internal static void TrackWhenOffScreen(this Vector2 tile, Color color)
    {
        Globals.Pointer.Value.DrawAsTrackingPointer(tile, color);
    }
}
