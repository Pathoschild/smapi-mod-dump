/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/focustense/StardewRadialMenu
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics.CodeAnalysis;

namespace RadialMenu;

public record TextureSegment(Texture2D Texture, Rectangle? SourceRect);

public record TextureSegmentPath(string AssetPath, Rectangle SourceRect)
{
    public static bool TryParse(
        string formattedPath,
        [MaybeNullWhen(false)] out TextureSegmentPath parsed)
    {
        parsed = null;
        var parts = formattedPath.Split(':');
        if (parts.Length != 2)
        {
            return false;
        }
        var assetPath = parts[0].Trim();
        var formattedRect = parts[1].Trim();
        if (!formattedRect.StartsWith("(") || !formattedRect.EndsWith(")"))
        {
            return false;
        }
        var coords = formattedRect[1..^1].Split(',');
        if (coords.Length != 4)
        {
            return false;
        }
        if (int.TryParse(coords[0], out int x)
            && int.TryParse(coords[1], out int y)
            && int.TryParse(coords[2], out int width)
            && int.TryParse(coords[3], out int height))
        {
            parsed = new(assetPath, new Rectangle(x, y, width, height));
            return true;
        }
        return false;
    }
}