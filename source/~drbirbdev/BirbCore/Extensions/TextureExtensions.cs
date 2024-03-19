/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace BirbCore.Extensions;
public static class TextureExtensions
{
    public static Texture2D GetTextureRect(this IRawTextureData texture, int x, int y, int width, int height)
    {
        if (x < 0 || y < 0 || width <= 0 || height <= 0 || x + width > texture.Width || y + height > texture.Height)
        {
            throw new ArgumentOutOfRangeException(nameof(texture), "subtexture rect must be within texture");
        }

        Color[] color = new Color[width * height];
        int i = 0;
        for (int dy = y; dy < y + height; dy++)
        {
            for (int dx = x; dx < x + width; dx++)
            {
                color[i] = texture.Data[dx + (dy * texture.Width)];
                i++;
            }
        }
        Texture2D result = new(Game1.graphics.GraphicsDevice, width, height);
        result.SetData(color);
        return result;
    }

    public static Color GetColor(this IRawTextureData texture, int x, int y)
    {
        if (x < 0 || y < 0 || x > texture.Width || y > texture.Height)
        {
            throw new ArgumentOutOfRangeException(nameof(texture), "pixel must be within texture");
        }
        return texture.Data[x + (y * texture.Width)];
    }
}
