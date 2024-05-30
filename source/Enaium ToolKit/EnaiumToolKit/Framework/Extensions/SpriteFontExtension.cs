/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/EnaiumToolKit
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;

namespace EnaiumToolKit.Framework.Extensions;

public static class SpriteFontExtension
{
    public static string GetEllipsisString(this SpriteFont spriteFont, string text, float width)
    {
        var charWidth = spriteFont.MeasureString($"{text[0]}").X;
        return text.Length * charWidth > width ? $"{text[..(int)(width / charWidth)]}..." : text;
    }
}