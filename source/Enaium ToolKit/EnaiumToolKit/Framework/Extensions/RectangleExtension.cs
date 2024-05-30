/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/EnaiumToolKit
**
*************************************************/

using Microsoft.Xna.Framework;

namespace EnaiumToolKit.Framework.Extensions;

public static class RectangleExtension
{
    public static bool IsHover(this Rectangle rectangle, int mouseX, int mouseY)
    {
        return mouseX >= rectangle.X && mouseX - rectangle.Width <= rectangle.X && mouseY >= rectangle.Y &&
               mouseY - rectangle.Height <= rectangle.Y;
    }

    public static Vector2 Position(this Rectangle rectangle)
    {
        return new Vector2(rectangle.X, rectangle.Y);
    }
}