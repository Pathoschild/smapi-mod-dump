/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using Microsoft.Xna.Framework;

#endregion using directives

public static class Vector2Extensions
{
    /// <summary>Draw a pointer over the tile if it is inside the current viewport.</summary>
    /// <param name="color">The desired color for the pointer.</param>
    public static void TrackWhenOnScreen(this Vector2 tile, Color color)
    {
        ModEntry.PlayerState.Pointer.DrawOverTile(tile, color);
    }

    /// <summary>Draw a pointer at the edge of the screen, pointing to the tile, if it is outside the current viewport.</summary>
    /// <param name="color">The desired color for the pointer.</param>
    public static void TrackWhenOffScreen(this Vector2 tile, Color color)
    {
        ModEntry.PlayerState.Pointer.DrawAsTrackingPointer(tile, color);
    }
}