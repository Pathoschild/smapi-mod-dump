/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sandman534/Abilities-Experience-Bars
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace AbilitiesExperienceBars
{
    public static class MyHelper
    {
        //PUBLIC FUNCTIONS
        public static Vector2 GetSpriteCenter(Texture2D sprite, float scale)
        {
            return new Vector2((sprite.Width / 2) * scale, (sprite.Height / 2) * scale);
        }
        
        public static Vector2 GetStringCenter(string text, SpriteFont font)
        {
            Vector2 size = font.MeasureString(text);

            return new Vector2(size.X / 2, size.Y / 2);
        }
        
        public static Color ChangeColorIntensity(Color color, float intensity, float alpha)
        {
            Color c1 = color;
            float cR = c1.R * intensity;
            float cG = c1.G * intensity;
            float cB = c1.B * intensity;
            byte cA = (byte)alpha;
            Color c2 = new((int)cR, (int)cG, (int)cB, cA);

            return c2;
        }
        
        public static float AdjustLanguagePosition(float pos, string language)
        {
            if (language == "ja")
            {
                pos += 10;
                return pos;
            }
            else if (language == "zh")
            {
                pos += 6;
                return pos;
            }
            else if (language == "de")
            {
                pos += 10;
                return pos;
            }
            else if(language == "ru")
            {
                pos += 9;
                return pos;
            }
            else
            {
                return pos;
            }
        }

        //FUNCTION HELPERS
        public static string[] Split(this string str, string splitter)
        {
            return str.Split(new[] { splitter }, StringSplitOptions.None);
        }
    }
}
