/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/EnaiumToolKit
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;

namespace EnaiumToolKit.Framework.Utils
{
    public static class FontUtils
    {
        public static void Draw(SpriteBatch b, String text, int x, int y)
        {
            Utility.drawTextWithShadow(b, text, Game1.dialogueFont,
                new Vector2(x, y), Game1.textColor, 1f,
                -1f,
                -1, -1, 0.0f);
        }

        public static void DrawHCentered(SpriteBatch b, String text, int x, int y)
        {
            Utility.drawTextWithShadow(b, text, Game1.dialogueFont,
                new Vector2(x - SpriteText.getWidthOfString(text) / 2, y), Game1.textColor, 1f,
                -1f,
                -1, -1, 0.0f);
        }

        public static void DrawVCentered(SpriteBatch b, String text, int x, int y)
        {
            Utility.drawTextWithShadow(b, text, Game1.dialogueFont,
                new Vector2(x, y - SpriteText.getHeightOfString(text) / 2), Game1.textColor, 1f,
                -1f,
                -1, -1, 0.0f);
        }

        public static void DrawHvCentered(SpriteBatch b, String text, int x, int y)
        {
            Utility.drawTextWithShadow(b, text, Game1.dialogueFont,
                new Vector2(x - SpriteText.getWidthOfString(text) / 2, y - SpriteText.getHeightOfString(text) / 2),
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
}