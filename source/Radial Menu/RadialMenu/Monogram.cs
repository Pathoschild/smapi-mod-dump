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
using StardewValley;
using System.Globalization;

namespace RadialMenu;

internal class Monogram
{
    protected static SpriteFont Font => Game1.dialogueFont;

    public static void Draw(
        SpriteBatch spriteBatch,
        Rectangle destinationRect,
        string text,
        Color color)
    {
        var monogramText = GetMonogramText(text);
        if (string.IsNullOrEmpty(monogramText))
        {
            return;
        }
        var size = Font.MeasureString(monogramText);
        var scale = Math.Min(destinationRect.Width / size.X, destinationRect.Height / size.Y);
        var scaledSize = size * scale;
        var position = new Vector2(
            destinationRect.Center.X - scaledSize.X / 2,
            destinationRect.Center.Y - scaledSize.Y / 2);
        Utility.drawTextWithColoredShadow(
            spriteBatch,
            monogramText,
            Font,
            position,
            color,
            Color.DimGray,
            scale: scale);
    }

    public static Vector2? Measure(string text)
    {
        var monogramText = GetMonogramText(text);
        return !string.IsNullOrEmpty(monogramText)
            ? Font.MeasureString(monogramText)
            : null;
    }

    private static string GetMonogramChar(string word)
    {
        return char.ToUpper(word[0], CultureInfo.CurrentCulture).ToString();
    }

    private static string GetMonogramText(string text)
    {
        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return words.Length >= 2
            ? GetMonogramChar(words[0]) + GetMonogramChar(words[^1])
            : words.Length == 1
                ? GetMonogramChar(words[0])
                : "";
    }
}
