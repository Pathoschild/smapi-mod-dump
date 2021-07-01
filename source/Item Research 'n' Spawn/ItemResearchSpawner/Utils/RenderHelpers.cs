/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TSlex/StardewValley
**
*************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace ItemResearchSpawner.Utils
{
    internal static class RenderHelpers
    {
        public static void DrawMenuBox(int x, int y, int innerWidth, int innerHeight, out Vector2 innerDrawPosition)
        {
            var spriteBatch = Game1.spriteBatch;

            var outerWidth = innerWidth + UIConstants.BorderWidth * 2;
            var outerHeight = innerHeight + UIConstants.BorderWidth * 2;

            IClickableMenu.drawTextureBox(
                spriteBatch,
                MenuSprites.SpriteMap,
                MenuSprites.MenuSmallBorder,
                x,
                y,
                outerWidth,
                outerHeight,
                Color.White,
                1,
                false);

            innerDrawPosition = new Vector2(x + UIConstants.BorderWidth, y + UIConstants.BorderWidth);
        }

        public static void DrawTextMenuBox(int x, int y, SpriteFont font, string text, int offsetX = 0, int offsetY = 0)
        {
            var spriteBatch = Game1.spriteBatch;
            var bounds = font.MeasureString(text);

            var additionalBounds = new Vector2(offsetX > 0 ? offsetX : 0, offsetY > 0 ? offsetY : 0);

            DrawMenuBox(x, y, (int) ((int) bounds.X + additionalBounds.X), (int) ((int) bounds.Y + additionalBounds.Y),
                out var textPosition);

            Utility.drawTextWithShadow(spriteBatch, text, font,
                new Vector2(textPosition.X + offsetX, textPosition.Y + offsetY), Game1.textColor);
        }
        
        public static void DrawTextMenuBox(int x, int y, int width, SpriteFont font, string text, int offsetX = 0, int offsetY = 0)
        {
            var spriteBatch = Game1.spriteBatch;
            var bounds = font.MeasureString(text);
            
            var additionalBounds = new Vector2(offsetX > 0 ? offsetX : 0, offsetY > 0 ? offsetY : 0);
            
            DrawMenuBox(x, y, width, (int) ((int) bounds.Y + additionalBounds.Y),
                out var textPosition);

            Utility.drawTextWithShadow(spriteBatch, text, font,
                new Vector2(textPosition.X + offsetX, textPosition.Y + offsetY), Game1.textColor);
        }

        public static void DrawItemBox(int x, int y, int innerWidth, int innerHeight, out Vector2 innerDrawPosition)
        {
            var spriteBatch = Game1.spriteBatch;

            IClickableMenu.drawTextureBox(
                spriteBatch,
                MenuSprites.SpriteMap,
                MenuSprites.ItemCell,
                x,
                y,
                innerWidth,
                innerHeight,
                Color.White,
                1,
                false);

            innerDrawPosition = new Vector2(x, y);
        }

        public static int GetChildCenterPosition(int pos, int parentLenght, int childLenght)
        {
            return (int) (pos + parentLenght / 2f - childLenght / 2f);
        }

        public static int GetLabelWidth(SpriteFont font)
        {
            return (int) font.MeasureString("THISISLABELWIDTHYEAH").X;
        }

        public static string TruncateString(string value, SpriteFont font, int maxWidth, string overflowSign = "...")
        {
            var newString = new StringBuilder();
            var width = 0f;

            foreach (var ch in value)
            {
                var charWidth = font.MeasureString(ch.ToString()).X;

                if (width + charWidth > maxWidth)
                {
                    newString.Append(overflowSign);
                    break;
                }

                newString.Append(ch);
                width += charWidth;
            }

            return newString.ToString();
        }
        
        public static string FillString(string value, string fillWith, SpriteFont font, int maxWidth, string overflowSign = "...")
        {
            var truncatedString = TruncateString(value, font, maxWidth, overflowSign);
            var stringWidth = font.MeasureString(truncatedString).X;
            
            var charWidth = font.MeasureString(fillWith).X;
            var charCount = (int) (maxWidth - stringWidth) / charWidth;

            var newString = new StringBuilder();
            
            for (var i = 0; i < charCount; i++)
            {
                newString.Append(fillWith);
            }
            
            newString.Append(truncatedString);

            return newString.ToString();
        }
    }
}