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
using StardewValley.BellsAndWhistles;

namespace EnaiumToolKit.Framework.Utils;

public class FontUtils
{
    public static void Draw(SpriteBatch b, string text, int x, int y)
    {
        Utility.drawTextWithShadow(b, text, Game1.dialogueFont,
            new Vector2(x, y), Game1.textColor, 1f,
            -1f,
            -1, -1, 0.0f);
    }

    public static void DrawHCentered(SpriteBatch b, string text, int x, int y)
    {
        var v = Game1.dialogueFont.MeasureString(text);
        Utility.drawTextWithShadow(b, text, Game1.dialogueFont,
            new Vector2(x - GetWidth(text) / 2f, y) + v, Game1.textColor, 1f,
            -1f,
            -1, -1, 0.0f);
    }

    public static void DrawVCentered(SpriteBatch b, string text, int x, int y)
    {
        Utility.drawTextWithShadow(b, text, Game1.dialogueFont,
            new Vector2(x, y - GetHeight(text) / 2f), Game1.textColor, 1f,
            -1f,
            -1, -1, 0.0f);
    }

    public static void DrawHvCentered(SpriteBatch b, string text, int x, int y)
    {
        Utility.drawTextWithShadow(b, text, Game1.dialogueFont,
            new Vector2(x - GetWidth(text) / 2f, y - GetHeight(text) / 2f),
            Game1.textColor, 1f,
            -1f,
            -1, -1, 0.0f);
    }


    public static int GetWidth(string text)
    {
        return SpriteText.getWidthOfString(text);
    }

    public static int GetHeight(string text)
    {
        return SpriteText.getHeightOfString(text);
    }
}