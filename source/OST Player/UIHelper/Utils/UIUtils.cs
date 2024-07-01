/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ProfeJavix/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace UIHelper
{
    internal class UIUtils
    {
        //Adapted from Game1 to fit borders in Rectangle
        internal static void DrawBox(SpriteBatch spriteBatch, Rectangle originalArea, Color color, byte bgAlpha = 255, byte borderAlpha = 255, bool fullBG = false)
        {
            Rectangle area = fullBG? originalArea : new(originalArea.X+16, originalArea.Y+16, originalArea.Width-32, originalArea.Height-32);
            int x = area.X, y = area.Y, width = area.Width, height = area.Height;
            Rectangle sourceRect = new Rectangle(64, 128, 64, 64);
            Texture2D texture = Game1.uncoloredMenuTexture;

            if(bgAlpha > 0){
                Color bgColor = new Color((int)Utility.Lerp(color.R, Math.Min(255, color.R + 150), 0.65f), (int)Utility.Lerp(color.G, Math.Min(255, color.G + 150), 0.65f), (int)Utility.Lerp(color.B, Math.Min(255, color.B + 150), 0.65f))
                {
                    A = bgAlpha
                };
                spriteBatch.Draw(texture, new Rectangle(x, y, width, height), sourceRect, bgColor);
            }

            if (borderAlpha > 0)
            {
                Color borderColor = new Color(color, borderAlpha);
                sourceRect.Y = 0;
                Vector2 vector = new Vector2(-sourceRect.Width * 0.5f, -sourceRect.Height * 0.5f);
                sourceRect.X = 0;
                spriteBatch.Draw(texture, new Vector2(x + vector.X, y + vector.Y), sourceRect, borderColor);
                sourceRect.X = 192;
                spriteBatch.Draw(texture, new Vector2(x + vector.X + width, y + vector.Y), sourceRect, borderColor);
                sourceRect.Y = 192;
                spriteBatch.Draw(texture, new Vector2(x + width + vector.X, y + height + vector.Y), sourceRect, borderColor);
                sourceRect.X = 0;
                spriteBatch.Draw(texture, new Vector2(x + vector.X, y + height + vector.Y), sourceRect, borderColor);
                sourceRect.X = 128;
                sourceRect.Y = 0;
                spriteBatch.Draw(texture, new Rectangle(64 + x + (int)vector.X, y + (int)vector.Y, width - 64, 64), sourceRect, borderColor);
                sourceRect.Y = 192;
                spriteBatch.Draw(texture, new Rectangle(64 + x + (int)vector.X, y + (int)vector.Y + height, width - 64, 64), sourceRect, borderColor);
                sourceRect.Y = 128;
                sourceRect.X = 0;
                spriteBatch.Draw(texture, new Rectangle(x + (int)vector.X, y + (int)vector.Y + 64, 64, height - 64), sourceRect, borderColor);
                sourceRect.X = 192;
                spriteBatch.Draw(texture, new Rectangle(x + width + (int)vector.X, y + (int)vector.Y + 64, 64, height - 64), sourceRect, borderColor);
            }
        }

        internal static Vector2 getCenteredText(Rectangle area, string text, SpriteFont font)
        {

            Vector2 textCenter = new Vector2(area.X + area.Width / 2, area.Y + area.Height / 2);
            Vector2 textSize = font.MeasureString(text);
            return textCenter - textSize / 2;
        }

        ///factor < 1: darker   || factor > 1: brighter
        internal static Color getHighLightColor(Color color, float factor = 0.8f)
        {
            Color highlightedColor = color * factor;
            highlightedColor.A = color.A;
            return highlightedColor;
        }

        internal static string FitTextInArea(string text, int width, SpriteFont font){

            int textWidth = (int)font.MeasureString(text).X;
            if(textWidth > width){
                string newText = text;
                for(int i = text.Length-1; i >= 0; i--){
                    newText = newText.Remove(i);
                    if((int)font.MeasureString(text).X < width)
                        return newText;
                }
            }

            return text;
        }
    }
}