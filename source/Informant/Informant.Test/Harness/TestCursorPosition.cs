/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-informant
**
*************************************************/

using Microsoft.Xna.Framework;

namespace StardewTests.Harness; 

public record TestCursorPosition : ICursorPosition {
    
    public Vector2 AbsolutePixels { get; set; } = Vector2.Zero;
    public int AbsoluteScale { get; set; } = 2;
    public Vector2 ScreenPixels { get; set; } = Vector2.Zero;
    public int ScreenScale { get; set; } = 2;
    public Vector2 Tile { get; set; } = Vector2.Zero;
    public Vector2 GrabTile { get; set; } = Vector2.Zero;
    
    public Vector2 GetScaledAbsolutePixels() {
        return AbsolutePixels * AbsoluteScale;
    }

    public Vector2 GetScaledScreenPixels() {
        return ScreenPixels * ScreenScale;
    }
    
    public bool Equals(ICursorPosition? other) {
        if (other == null) return false;
        return other.AbsolutePixels == AbsolutePixels &&
               other.ScreenPixels == ScreenPixels &&
               other.Tile == Tile &&
               other.GrabTile == GrabTile;
    }
}