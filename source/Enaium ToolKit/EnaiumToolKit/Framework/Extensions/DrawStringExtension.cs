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
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace EnaiumToolKit.Framework.Extensions;

public static class DrawStringExtension
{
    public static void DrawString(this SpriteBatch b, string text, Vector2 position, SpriteFont? font = null,
        Color? color = null)
    {
        Utility.drawTextWithShadow(b, text, font ?? Game1.dialogueFont, position, color ?? Game1.textColor, 1f, -1f, -1,
            -1, 0.0f);
    }

    public static void DrawString(this SpriteBatch b, string text, int x, int y, SpriteFont? font = null,
        Color? color = null)
    {
        DrawString(b, text, new Vector2(x, y), font, color);
    }

    public static void DrawStringHCenter(this SpriteBatch b, string text, int x, int y, SpriteFont? font = null,
        Color? color = null)
    {
        var v = (font ?? Game1.dialogueFont).MeasureString(text);
        DrawString(b, text, new Vector2(x - v.X / 2f, y), font, color);
    }

    public static void DrawStringHCenter(this SpriteBatch b, string text, int x, int y, int width,
        SpriteFont? font = null, Color? color = null)
    {
        var v = (font ?? Game1.dialogueFont).MeasureString(text);
        DrawString(b, text, new Vector2(x + width / 2f - v.X / 2f, y), font, color);
    }

    public static void DrawStringVCenter(this SpriteBatch b, string text, int x, int y, SpriteFont? font = null,
        Color? color = null)
    {
        var v = (font ?? Game1.dialogueFont).MeasureString(text);
        DrawString(b, text, new Vector2(x, y - v.Y / 2f), font, color);
    }

    public static void DrawStringVCenter(this SpriteBatch b, string text, int x, int y, int height,
        SpriteFont? font = null, Color? color = null)
    {
        var v = (font ?? Game1.dialogueFont).MeasureString(text);
        DrawString(b, text, new Vector2(x, y + height / 2f - v.Y / 2f), font, color);
    }

    public static void DrawStringVCenter(this SpriteBatch b, string text, Vector2 position, int height,
        SpriteFont? font = null, Color? color = null)
    {
        DrawStringVCenter(b, text, (int)position.X, (int)position.Y, height, font, color);
    }

    public static void DrawStringCenter(this SpriteBatch b, string text, Rectangle bounds, SpriteFont? font = null,
        Color? color = null)
    {
        var v = (font ?? Game1.dialogueFont).MeasureString(text);
        DrawString(b, text,
            new Vector2(bounds.X + bounds.Width / 2f - v.X / 2f, bounds.Y + bounds.Height / 2f - v.Y / 2f),
            font, color);
    }

    public static void DrawStringCenter(this SpriteBatch b, string text, int x, int y, int width, int height,
        SpriteFont? font = null,
        Color? color = null)
    {
        DrawStringCenter(b, text, new Rectangle(x, y, width, height), font, color);
    }

    public static int GetStringWidth(this SpriteBatch b, string text, SpriteFont? font = null)
    {
        return (int)(font ?? Game1.dialogueFont).MeasureString(text).X;
    }

    public static int GetStringHeight(this SpriteBatch b, string text, SpriteFont? font = null)
    {
        return (int)(font ?? Game1.dialogueFont).MeasureString(text).Y;
    }
}