using BmFont;
using FelixDev.StardewMods.FeTK.Framework.Data.Parsers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FelixDev.StardewMods.FeTK.Framework.Helpers
{
    /// <summary>
    /// Provides an extended API for the <see cref="SpriteText"/> class to support string coloring.
    /// </summary>
    internal static class SpriteTextHelper
    {
        private static FontFile FontFile = null;
        private static List<Texture2D> fontPages = null;

        private const char SPECIAL_CHAR_NEWLINE = '^';
        private const char SPECIAL_CHAR_HEART = '<';
        private const char SPECIAL_CHAR_STAR = '=';
        private const char SPECIAL_CHAR_ARROW_RIGHT = '>';
        private const char SPECIAL_CHAR_ARROW_LEFT = '@';
        private const char SPECIAL_CHAR_ARROW_UP = '`';
        private const char SPECIAL_CHAR_MONEY = '$';
        private const char SPECIAL_CHAR_SAM = '+';

        private static Dictionary<char, FontChar> _characterMap;

        private static readonly Color textColorDefault = new Color(86, 22, 12);

        public static void DrawString(
            SpriteBatch b,
            string s,
            int x,
            int y,
            Color color,
            int characterPosition = 999999,
            int width = -1,
            int height = 999999,
            float alpha = 1f,
            float layerDepth = 0.88f,
            int drawBGScroll = -1,
            string placeHolderScrollWidthText = "",
            List<TextColorInfo> textColorMappings = null)
        {
            SpriteTextHelper.SetUpCharacterMap();

            if (width == -1)
            {
                width = Game1.graphics.GraphicsDevice.Viewport.Width - x;
                if (drawBGScroll == 1)
                    width = SpriteText.getWidthOfString(s, 999999) * 2;
            }

            if ((double)SpriteText.fontPixelZoom < 4.0 && LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.ko)
                y += (int)((4.0 - (double)SpriteText.fontPixelZoom) * 4.0);

            Vector2 vector2_1 = new Vector2((float)x, (float)y);
            int accumulatedHorizontalSpaceBetweenCharacters = 0;

            if (drawBGScroll != 1)
            {
                if ((double)vector2_1.X + (double)width > (double)(Game1.graphics.GraphicsDevice.Viewport.Width - 4))
                    vector2_1.X = (float)(Game1.graphics.GraphicsDevice.Viewport.Width - width - 4);
                if ((double)vector2_1.X < 0.0)
                    vector2_1.X = 0.0f;
            }

            if (drawBGScroll == 0 || drawBGScroll == 0)
            {
                b.Draw(Game1.mouseCursors, vector2_1 + new Vector2(-12f, -3f) * 4f, new Rectangle?(new Rectangle(325, 318, 12, 18)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth - 1f / 1000f);
                b.Draw(Game1.mouseCursors, vector2_1 + new Vector2(0.0f, -3f) * 4f, new Rectangle?(new Rectangle(337, 318, 1, 18)), Color.White * alpha, 0.0f, Vector2.Zero, new Vector2((float)SpriteText.getWidthOfString(placeHolderScrollWidthText.Length > 0 ? placeHolderScrollWidthText : s, 999999), 4f), SpriteEffects.None, layerDepth - 1f / 1000f);
                b.Draw(Game1.mouseCursors, vector2_1 + new Vector2((float)SpriteText.getWidthOfString(placeHolderScrollWidthText.Length > 0 ? placeHolderScrollWidthText : s, 999999), -12f), new Rectangle?(new Rectangle(338, 318, 12, 18)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth - 1f / 1000f);

                if (placeHolderScrollWidthText.Length > 0)
                {
                    if (drawBGScroll != 0)
                        x += SpriteText.getWidthOfString(placeHolderScrollWidthText, 999999) / 2 - SpriteText.getWidthOfString(s, 999999) / 2;

                    vector2_1.X = (float)x;
                }
                vector2_1.Y += (float)((4.0 - (double)SpriteText.fontPixelZoom) * 4.0);
            }
            else
            {
                switch (drawBGScroll)
                {
                    case 1:
                        b.Draw(Game1.mouseCursors, vector2_1 + new Vector2(-7f, -3f) * 4f, new Rectangle?(new Rectangle(324, 299, 7, 17)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth - 1f / 1000f);
                        b.Draw(Game1.mouseCursors, vector2_1 + new Vector2(0.0f, -3f) * 4f, new Rectangle?(new Rectangle(331, 299, 1, 17)), Color.White * alpha, 0.0f, Vector2.Zero, new Vector2((float)SpriteText.getWidthOfString(placeHolderScrollWidthText.Length > 0 ? placeHolderScrollWidthText : s, 999999), 4f), SpriteEffects.None, layerDepth - 1f / 1000f);
                        b.Draw(Game1.mouseCursors, vector2_1 + new Vector2((float)SpriteText.getWidthOfString(placeHolderScrollWidthText.Length > 0 ? placeHolderScrollWidthText : s, 999999), -12f), new Rectangle?(new Rectangle(332, 299, 7, 17)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth - 1f / 1000f);
                        b.Draw(Game1.mouseCursors, vector2_1 + new Vector2((float)(SpriteText.getWidthOfString(placeHolderScrollWidthText.Length > 0 ? placeHolderScrollWidthText : s, 999999) / 2), 52f), new Rectangle?(new Rectangle(341, 308, 6, 5)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth - 0.0001f);
                        if (placeHolderScrollWidthText.Length > 0)
                        {
                            x += SpriteText.getWidthOfString(placeHolderScrollWidthText, 999999) / 2 - SpriteText.getWidthOfString(s, 999999) / 2;
                            vector2_1.X = (float)x;
                        }
                        vector2_1.Y += (float)((4.0 - (double)SpriteText.fontPixelZoom) * 4.0);
                        break;
                    case 2:
                        b.Draw(Game1.mouseCursors, vector2_1 + new Vector2(-3f, -3f) * 4f, new Rectangle?(new Rectangle(327, 281, 3, 17)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth - 1f / 1000f);
                        b.Draw(Game1.mouseCursors, vector2_1 + new Vector2(0.0f, -3f) * 4f, new Rectangle?(new Rectangle(330, 281, 1, 17)), Color.White * alpha, 0.0f, Vector2.Zero, new Vector2((float)(SpriteText.getWidthOfString(placeHolderScrollWidthText.Length > 0 ? placeHolderScrollWidthText : s, 999999) + 4), 4f), SpriteEffects.None, layerDepth - 1f / 1000f);
                        b.Draw(Game1.mouseCursors, vector2_1 + new Vector2((float)(SpriteText.getWidthOfString(placeHolderScrollWidthText.Length > 0 ? placeHolderScrollWidthText : s, 999999) + 4), -12f), new Rectangle?(new Rectangle(333, 281, 3, 17)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth - 1f / 1000f);
                        if (placeHolderScrollWidthText.Length > 0)
                        {
                            x += SpriteText.getWidthOfString(placeHolderScrollWidthText, 999999) / 2 - SpriteText.getWidthOfString(s, 999999) / 2;
                            vector2_1.X = (float)x;
                        }
                        vector2_1.Y += (float)((4.0 - (double)SpriteText.fontPixelZoom) * 4.0);
                        break;
                }
            }

            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko)
                vector2_1.Y -= 8f;

            s = s.Replace(Environment.NewLine, "");

            if ((LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ja || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.zh || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.th))
                vector2_1.Y -= (float)((4.0 - (double)SpriteText.fontPixelZoom) * 4.0);

            s = s.Replace('♡', SPECIAL_CHAR_HEART);

            for (int index = 0; index < Math.Min(s.Length, characterPosition); ++index)
            {
                if (LocalizedContentManager.CurrentLanguageLatin || SpriteTextHelper.IsSpecialCharacter(s[index]))
                {
                    float fontPixelZoom = SpriteText.fontPixelZoom;
                    if (SpriteTextHelper.IsSpecialCharacter(s[index]))
                        SpriteText.fontPixelZoom = 3f;

                    if (s[index] == SPECIAL_CHAR_NEWLINE)
                    {
                        vector2_1.Y += 18f * SpriteText.fontPixelZoom;
                        vector2_1.X = (float)x;
                        accumulatedHorizontalSpaceBetweenCharacters = 0;
                    }
                    else
                    {
                        if (index > 0)
                        {
                            vector2_1.X += (float)(8.0 * SpriteText.fontPixelZoom + accumulatedHorizontalSpaceBetweenCharacters 
                                + (SpriteText.getWidthOffsetForChar(s[index]) + SpriteText.getWidthOffsetForChar(s[index - 1])) * (double)SpriteText.fontPixelZoom);
                        }

                        accumulatedHorizontalSpaceBetweenCharacters = (int)(0.0 * (double)SpriteText.fontPixelZoom);

                        if (SpriteText.positionOfNextSpace(s, index, (int)vector2_1.X, accumulatedHorizontalSpaceBetweenCharacters) >= x + width - 4)
                        {
                            vector2_1.Y += 18f * SpriteText.fontPixelZoom;
                            accumulatedHorizontalSpaceBetweenCharacters = 0;
                            vector2_1.X = (float)x;
                        }

                        bool flag = char.IsUpper(s[index]) || s[index] == 'ß';
                        Vector2 vector2_2 = new Vector2(0.0f, (float)((flag ? -3 : 0) - 1));

                        if (s[index] == 'Ç')
                            vector2_2.Y += 2f;

                        // Additional code compared to the original version:
                        // 
                        // Retrieve if the user specified color for this character. If no such color exists,
                        // use the default text color.
                        Color charColor = color;
                        if (textColorMappings != null)
                        {
                            var clr = SpriteTextHelper.GetColorForCharacter(index, textColorMappings);
                            if (clr.HasValue)
                            {
                                charColor = clr.Value;
                            }
                        }

                        b.Draw(SpriteText.coloredTexture, 
                            vector2_1 + vector2_2 * SpriteText.fontPixelZoom, 
                            new Rectangle?(SpriteTextHelper.GetSourceRectForChar(s[index])), 
                            (SpriteTextHelper.IsSpecialCharacter(s[index]) ? Color.White : charColor) * alpha, 
                            0.0f, Vector2.Zero, SpriteText.fontPixelZoom, SpriteEffects.None, layerDepth);

                        SpriteText.fontPixelZoom = fontPixelZoom;
                    }
                }
                else if (s[index] == SPECIAL_CHAR_NEWLINE)
                {
                    vector2_1.Y += (float)(SpriteTextHelper.FontFile.Common.LineHeight + 2) * SpriteText.fontPixelZoom;
                    vector2_1.X = (float)x;
                    accumulatedHorizontalSpaceBetweenCharacters = 0;
                }
                else
                {
                    if (index > 0 && SpriteTextHelper.IsSpecialCharacter(s[index - 1]))
                        vector2_1.X += 24f;

                    if (SpriteTextHelper._characterMap.TryGetValue(s[index], out FontChar fontChar))
                    {
                        Rectangle rectangle = new Rectangle(fontChar.X, fontChar.Y, fontChar.Width, fontChar.Height);
                        Texture2D fontPage = SpriteTextHelper.fontPages[fontChar.Page];

                        if (SpriteText.positionOfNextSpace(s, index, (int)vector2_1.X, accumulatedHorizontalSpaceBetweenCharacters) >= x + width - 4)
                        {
                            vector2_1.Y += (float)(SpriteTextHelper.FontFile.Common.LineHeight + 2) * SpriteText.fontPixelZoom;
                            accumulatedHorizontalSpaceBetweenCharacters = 0;
                            vector2_1.X = (float)x;
                        }

                        Vector2 position = new Vector2(vector2_1.X + (float)fontChar.XOffset * SpriteText.fontPixelZoom, vector2_1.Y + (float)fontChar.YOffset * SpriteText.fontPixelZoom);
                        if (drawBGScroll != -1 && LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko)
                            position.Y -= 8f;

                        // Additional code compared to the original version:
                        // 
                        // Retrieve if the user specified color for this character. If no such color exists,
                        // use the default text color.
                        Color charColor = color;
                        if (textColorMappings != null)
                        {
                            var clr = SpriteTextHelper.GetColorForCharacter(index, textColorMappings);
                            if (clr.HasValue)
                            {
                                charColor = clr.Value;
                            }
                        }

                        if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru)
                        {
                            Vector2 vector2_2 = new Vector2(-1f, 1f) * SpriteText.fontPixelZoom;
                            b.Draw(fontPage, position + vector2_2, new Rectangle?(rectangle), charColor * alpha * SpriteText.shadowAlpha, 0.0f, Vector2.Zero, SpriteText.fontPixelZoom, SpriteEffects.None, layerDepth);
                            b.Draw(fontPage, position + new Vector2(0.0f, vector2_2.Y), new Rectangle?(rectangle), charColor * alpha * SpriteText.shadowAlpha, 0.0f, Vector2.Zero, SpriteText.fontPixelZoom, SpriteEffects.None, layerDepth);
                            b.Draw(fontPage, position + new Vector2(vector2_2.X, 0.0f), new Rectangle?(rectangle), charColor * alpha * SpriteText.shadowAlpha, 0.0f, Vector2.Zero, SpriteText.fontPixelZoom, SpriteEffects.None, layerDepth);
                        }

                        b.Draw(fontPage, position, new Rectangle?(rectangle), charColor * alpha, 0.0f, Vector2.Zero, SpriteText.fontPixelZoom, SpriteEffects.None, layerDepth);
                        vector2_1.X += (float)fontChar.XAdvance * SpriteText.fontPixelZoom;
                    }
                }
            }
        }

        private static Color? GetColorForCharacter(int index, List<TextColorInfo> textColorMappings)
        {
            int prevMappingStartIndex = 0;
            foreach (var mapping in textColorMappings)
            {
                if (index >= prevMappingStartIndex + mapping.Text.Length)
                {
                    prevMappingStartIndex += mapping.Text.Length;
                    continue;
                }

                return mapping.Color;
            }

            return null;
        }

        private static void SetUpCharacterMap()
        {
            if (!LocalizedContentManager.CurrentLanguageLatin && SpriteTextHelper._characterMap == null)
            {
                SpriteTextHelper._characterMap = new Dictionary<char, FontChar>();
                SpriteTextHelper.fontPages = new List<Texture2D>();
                switch (LocalizedContentManager.CurrentLanguageCode)
                {
                    case LocalizedContentManager.LanguageCode.ja:
                        SpriteTextHelper.FontFile = SpriteTextHelper.LoadFont("Fonts\\Japanese");
                        SpriteText.fontPixelZoom = 1.75f;
                        break;
                    case LocalizedContentManager.LanguageCode.ru:
                        SpriteTextHelper.FontFile = SpriteTextHelper.LoadFont("Fonts\\Russian");
                        SpriteText.fontPixelZoom = 3f;
                        break;
                    case LocalizedContentManager.LanguageCode.zh:
                        SpriteTextHelper.FontFile = SpriteTextHelper.LoadFont("Fonts\\Chinese");
                        SpriteText.fontPixelZoom = 1.5f;
                        break;
                    case LocalizedContentManager.LanguageCode.th:
                        SpriteTextHelper.FontFile = SpriteTextHelper.LoadFont("Fonts\\Thai");
                        SpriteText.fontPixelZoom = 1.5f;
                        break;
                    case LocalizedContentManager.LanguageCode.ko:
                        SpriteTextHelper.FontFile = SpriteTextHelper.LoadFont("Fonts\\Korean");
                        SpriteText.fontPixelZoom = 1.5f;
                        break;
                }
                foreach (FontChar fontChar in SpriteTextHelper.FontFile.Chars)
                {
                    char id = (char)fontChar.ID;
                    SpriteTextHelper._characterMap.Add(id, fontChar);
                }

                foreach (FontPage page in SpriteTextHelper.FontFile.Pages)
                    SpriteTextHelper.fontPages.Add(Game1.content.Load<Texture2D>("Fonts\\" + page.File));
                LocalizedContentManager.OnLanguageChange += new LocalizedContentManager.LanguageChangedHandler(SpriteTextHelper.OnLanguageChange);
            }
            else
            {
                if (!LocalizedContentManager.CurrentLanguageLatin || (double)SpriteText.fontPixelZoom >= 3.0)
                    return;
                SpriteText.fontPixelZoom = 3f;
            }
        }

        private static bool IsSpecialCharacter(char c)
        {
            return c.Equals(SPECIAL_CHAR_HEART) || c.Equals(SPECIAL_CHAR_STAR) || c.Equals(SPECIAL_CHAR_MONEY)
                || c.Equals(SPECIAL_CHAR_ARROW_RIGHT) || c.Equals(SPECIAL_CHAR_ARROW_LEFT) || c.Equals(SPECIAL_CHAR_ARROW_UP)
                || c.Equals(SPECIAL_CHAR_SAM);
        }

        private static void OnLanguageChange(LocalizedContentManager.LanguageCode code)
        {
            if (SpriteTextHelper._characterMap != null)
                SpriteTextHelper._characterMap.Clear();
            else
                SpriteTextHelper._characterMap = new Dictionary<char, FontChar>();
            if (SpriteTextHelper.fontPages != null)
                SpriteTextHelper.fontPages.Clear();
            else
                SpriteTextHelper.fontPages = new List<Texture2D>();
            switch (code)
            {
                case LocalizedContentManager.LanguageCode.ja:
                    SpriteTextHelper.FontFile = SpriteTextHelper.LoadFont("Fonts\\Japanese");
                    SpriteText.fontPixelZoom = 1.75f;
                    break;
                case LocalizedContentManager.LanguageCode.ru:
                    SpriteTextHelper.FontFile = SpriteTextHelper.LoadFont("Fonts\\Russian");
                    SpriteText.fontPixelZoom = 3f;
                    break;
                case LocalizedContentManager.LanguageCode.zh:
                    SpriteTextHelper.FontFile = SpriteTextHelper.LoadFont("Fonts\\Chinese");
                    SpriteText.fontPixelZoom = 1.5f;
                    break;
                case LocalizedContentManager.LanguageCode.th:
                    SpriteTextHelper.FontFile = SpriteTextHelper.LoadFont("Fonts\\Thai");
                    SpriteText.fontPixelZoom = 1.5f;
                    break;
                case LocalizedContentManager.LanguageCode.ko:
                    SpriteTextHelper.FontFile = SpriteTextHelper.LoadFont("Fonts\\Korean");
                    SpriteText.fontPixelZoom = 1.5f;
                    break;
            }
            foreach (FontChar fontChar in SpriteTextHelper.FontFile.Chars)
            {
                char id = (char)fontChar.ID;
                SpriteTextHelper._characterMap.Add(id, fontChar);
            }
            foreach (FontPage page in SpriteTextHelper.FontFile.Pages)
                SpriteTextHelper.fontPages.Add(Game1.content.Load<Texture2D>("Fonts\\" + page.File));
        }

        private static FontFile LoadFont(string assetName)
        {
            return FontLoader.Parse(Game1.content.Load<XmlSource>(assetName).Source);
        }

        private static Rectangle GetSourceRectForChar(char c)
        {
            int num = (int)c - 32;
            switch (c)
            {
                case 'Ğ':
                    num = 102;
                    break;
                case 'ğ':
                    num = 103;
                    break;
                case 'İ':
                    num = 98;
                    break;
                case 'ı':
                    num = 99;
                    break;
                case 'Ő':
                    num = 105;
                    break;
                case 'ő':
                    num = 106;
                    break;
                case 'Œ':
                    num = 96;
                    break;
                case 'œ':
                    num = 97;
                    break;
                case 'Ş':
                    num = 100;
                    break;
                case 'ş':
                    num = 101;
                    break;
                case 'Ű':
                    num = 107;
                    break;
                case 'ű':
                    num = 108;
                    break;
                case '’':
                    num = 104;
                    break;
            }
            return new Rectangle(num * 8 % SpriteText.spriteTexture.Width, num * 8 / SpriteText.spriteTexture.Width * 16, 8, 16);
        }

        public static Color GetColorFromIndex(int index)
        {
            switch (index)
            {
                case -1:
                    return textColorDefault;
                case 1:
                    return Color.SkyBlue;
                case 2:
                    return Color.Red;
                case 3:
                    return new Color(110, 43, (int)byte.MaxValue);
                case 4:
                    return Color.White;
                case 5:
                    return Color.OrangeRed;
                case 6:
                    return Color.LimeGreen;
                case 7:
                    return Color.Cyan;
                case 8:
                    return new Color(60, 60, 60);
                default:
                    return Color.Black;
            }
        }
    }
}
