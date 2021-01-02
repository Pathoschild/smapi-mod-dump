/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jltaylor-us/StardewToDew
**
*************************************************/

// As the name imples, this file contains code originally by Pathoschild, possibly
// with slight modifictations.  The original is Copyright 2016–2019 Pathoschild
// and other contributors (see NOTICE for license), with any additions
// Copyright 2020 Jamie Taylor.
// Look around in https://github.com/Pathoschild/StardewMods/blob/develop/LookupAnything
// for the current versions of these classes/interfaces.
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ToDew
{
    /// <summary>Provides common utility methods for interacting with the game code shared by my various mods.</summary>
    internal static class CommonHelper
    {
        /// <summary>A blank pixel which can be colorized and stretched to draw geometric shapes.</summary>
        private static readonly Lazy<Texture2D> LazyPixel = new Lazy<Texture2D>(() =>
        {
            Texture2D pixel = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            return pixel;
        });
        /// <summary>A blank pixel which can be colorized and stretched to draw geometric shapes.</summary>
        public static Texture2D Pixel => CommonHelper.LazyPixel.Value;

        /// <summary>Get the dimensions of a space character.</summary>
        /// <param name="font">The font to measure.</param>
        public static float GetSpaceWidth(SpriteFont font)
        {
            return font.MeasureString("A B").X - font.MeasureString("AB").X;
        }

    }

    /// <summary>A snippet of formatted text.</summary>
    internal interface IFormattedText
    {
        /// <summary>The font color (or <c>null</c> for the default color).</summary>
        Color? Color { get; }

        /// <summary>The text to format.</summary>
        string Text { get; }

        /// <summary>Whether to draw bold text.</summary>
        bool Bold { get; }
    }

    /// <summary>A snippet of formatted text.</summary>
    internal struct FormattedText : IFormattedText
    {
        /********
        ** Accessors
        *********/
        /// <summary>The text to format.</summary>
        public string Text { get; }

        /// <summary>The font color (or <c>null</c> for the default color).</summary>
        public Color? Color { get; }

        /// <summary>Whether to draw bold text.</summary>
        public bool Bold { get; }


        /********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="text">The text to format.</param>
        /// <param name="color">The font color (or <c>null</c> for the default color).</param>
        /// <param name="bold">Whether to draw bold text.</param>
        public FormattedText(string text, Color? color = null, bool bold = false)
        {
            this.Text = text;
            this.Color = color;
            this.Bold = bold;
        }
    }

    /// <summary>Provides utility methods for drawing to the screen.</summary>
    internal static class DrawTextHelper
    {
        /*********
        ** Public methods
        *********/
        /****
        ** Drawing
        ****/
        /// <summary>Draw a block of text to the screen with the specified wrap width.</summary>
        /// <param name="batch">The sprite batch.</param>
        /// <param name="font">The sprite font.</param>
        /// <param name="text">The block of text to write.</param>
        /// <param name="position">The position at which to draw the text.</param>
        /// <param name="wrapWidth">The width at which to wrap the text.</param>
        /// <param name="color">The text color.</param>
        /// <param name="bold">Whether to draw bold text.</param>
        /// <param name="scale">The font scale.</param>
        /// <returns>Returns the text dimensions.</returns>
        public static Vector2 DrawTextBlock(this SpriteBatch batch, SpriteFont font, string text, Vector2 position, float wrapWidth, Color? color = null, bool bold = false, float scale = 1)
        {
            return batch.DrawTextBlock(font, new IFormattedText[] { new FormattedText(text, color, bold) }, position, wrapWidth, scale);
        }

        /// <summary>Draw a block of text to the screen with the specified wrap width.</summary>
        /// <param name="batch">The sprite batch.</param>
        /// <param name="font">The sprite font.</param>
        /// <param name="text">The block of text to write.</param>
        /// <param name="position">The position at which to draw the text.</param>
        /// <param name="wrapWidth">The width at which to wrap the text.</param>
        /// <param name="scale">The font scale.</param>
        /// <returns>Returns the text dimensions.</returns>
        public static Vector2 DrawTextBlock(this SpriteBatch batch, SpriteFont font, IEnumerable<IFormattedText> text, Vector2 position, float wrapWidth, float scale = 1)
        {
            if (text == null)
                return new Vector2(0, 0);

            // track draw values
            float xOffset = 0;
            float yOffset = 0;
            float lineHeight = font.MeasureString("ABC").Y * scale;
            float spaceWidth = CommonHelper.GetSpaceWidth(font) * scale;
            float blockWidth = 0;
            float blockHeight = lineHeight;

            // draw text snippets
            foreach (IFormattedText snippet in text)
            {
                if (snippet?.Text == null)
                    continue;

                // track surrounding spaces for combined translations
                bool startSpace = snippet.Text.StartsWith(" ");
                bool endSpace = snippet.Text.EndsWith(" ");

                // get word list
                IList<string> words = new List<string>();
                string[] rawWords = snippet.Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0, last = rawWords.Length - 1; i <= last; i++)
                {
                    // get word
                    string word = rawWords[i];
                    if (startSpace && i == 0)
                        word = $" {word}";
                    if (endSpace && i == last)
                        word += " ";

                    // split on newlines
                    string wordPart = word;
                    int newlineIndex;
                    while ((newlineIndex = wordPart.IndexOf(Environment.NewLine, StringComparison.Ordinal)) >= 0)
                    {
                        if (newlineIndex == 0)
                        {
                            words.Add(Environment.NewLine);
                            wordPart = wordPart.Substring(Environment.NewLine.Length);
                        }
                        else if (newlineIndex > 0)
                        {
                            words.Add(wordPart.Substring(0, newlineIndex));
                            words.Add(Environment.NewLine);
                            wordPart = wordPart.Substring(newlineIndex + Environment.NewLine.Length);
                        }
                    }

                    // add remaining word (after newline split)
                    if (wordPart.Length > 0)
                        words.Add(wordPart);
                }

                // draw words to screen
                bool isFirstOfLine = true;
                foreach (string word in words)
                {
                    // check wrap width
                    float wordWidth = font.MeasureString(word).X * scale;
                    float prependSpace = isFirstOfLine ? 0 : spaceWidth;
                    if (word == Environment.NewLine || ((wordWidth + xOffset + prependSpace) > wrapWidth && (int)xOffset != 0))
                    {
                        xOffset = 0;
                        yOffset += lineHeight;
                        blockHeight += lineHeight;
                        isFirstOfLine = true;
                    }
                    if (word == Environment.NewLine)
                        continue;

                    // draw text
                    Vector2 wordPosition = new Vector2(position.X + xOffset + prependSpace, position.Y + yOffset);
                    if (snippet.Bold)
                        Utility.drawBoldText(batch, word, font, wordPosition, snippet.Color ?? Color.Black, scale);
                    else
                        batch.DrawString(font, word, wordPosition, snippet.Color ?? Color.Black, 0, Vector2.Zero, scale, SpriteEffects.None, 1);

                    // update draw values
                    if (xOffset + wordWidth + prependSpace > blockWidth)
                        blockWidth = xOffset + wordWidth + prependSpace;
                    xOffset += wordWidth + prependSpace;
                    isFirstOfLine = false;
                }
            }

            // return text position & dimensions
            return new Vector2(blockWidth, blockHeight);
        }
    }
    /// <summary>Provides utility methods for drawing to the screen.</summary>
    internal static class DrawHelper
    {

        /****
        ** Drawing
        ****/
        /// <summary>Draw a sprite to the screen.</summary>
        /// <param name="spriteBatch">The sprite batch being drawn.</param>
        /// <param name="sheet">The sprite sheet containing the sprite.</param>
        /// <param name="sprite">The sprite coordinates and dimensions in the sprite sheet.</param>
        /// <param name="x">The X-position at which to draw the sprite.</param>
        /// <param name="y">The X-position at which to draw the sprite.</param>
        /// <param name="color">The color to tint the sprite.</param>
        /// <param name="scale">The scale to draw.</param>
        public static void DrawSprite(this SpriteBatch spriteBatch, Texture2D sheet, Rectangle sprite, float x, float y, Color? color = null, float scale = 1)
        {
            spriteBatch.Draw(sheet, new Vector2(x, y), sprite, color ?? Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
        }

        /// <summary>Draw a sprite to the screen scaled and centered to fit the given dimensions.</summary>
        /// <param name="spriteBatch">The sprite batch being drawn.</param>
        /// <param name="sheet">The sprite sheet containing the sprite.</param>
        /// <param name="sprite">The sprite coordinates and dimensions in the sprite sheet.</param>
        /// <param name="x">The X-position at which to draw the sprite.</param>
        /// <param name="y">The X-position at which to draw the sprite.</param>
        /// <param name="size">The size to draw.</param>
        /// <param name="color">The color to tint the sprite.</param>
        public static void DrawSpriteWithin(this SpriteBatch spriteBatch, Texture2D sheet, Rectangle sprite, float x, float y, Vector2 size, Color? color = null)
        {
            // calculate dimensions
            float largestDimension = Math.Max(sprite.Width, sprite.Height);
            float scale = size.X / largestDimension;
            float leftOffset = Math.Max((size.X - (sprite.Width * scale)) / 2, 0);
            float topOffset = Math.Max((size.Y - (sprite.Height * scale)) / 2, 0);

            // draw
            spriteBatch.DrawSprite(sheet, sprite, x + leftOffset, y + topOffset, color ?? Color.White, scale);
        }

        /// <summary>Draw a sprite to the screen.</summary>
        /// <param name="batch">The sprite batch.</param>
        /// <param name="x">The X-position at which to start the line.</param>
        /// <param name="y">The X-position at which to start the line.</param>
        /// <param name="size">The line dimensions.</param>
        /// <param name="color">The color to tint the sprite.</param>
        public static void DrawLine(this SpriteBatch batch, float x, float y, Vector2 size, Color? color = null)
        {
            batch.Draw(CommonHelper.Pixel, new Rectangle((int)x, (int)y, (int)size.X, (int)size.Y), color ?? Color.White);
        }
    }

    /// <summary>Simplifies access to the game's sprite sheets.</summary>
    /// <remarks>Each sprite is represented by a rectangle, which specifies the coordinates and dimensions of the image in the sprite sheet.</remarks>
    internal static class CommonSprites
    {
        /// <summary>Sprites used to draw icons.</summary>
        public static class Icons
        {
            /// <summary>The sprite sheet containing the icon sprites.</summary>
            public static Texture2D Sheet => Game1.mouseCursors;

            /// <summary>A down arrow.</summary>
            public static readonly Rectangle DownArrow = new Rectangle(12, 76, 40, 44);

            /// <summary>An up arrow.</summary>
            public static readonly Rectangle UpArrow = new Rectangle(76, 72, 40, 44);

            /// <summary>A left arrow.</summary>
            public static readonly Rectangle LeftArrow = new Rectangle(8, 268, 44, 40);

            /// <summary>A right arrow.</summary>
            public static readonly Rectangle RightArrow = new Rectangle(12, 204, 44, 40);

            /// <summary>A speech bubble icon.</summary>
            public static readonly Rectangle SpeechBubble = new Rectangle(66, 4, 14, 12);

            /// <summary>An empty checkbox icon.</summary>
            public static readonly Rectangle EmptyCheckbox = new Rectangle(227, 425, 9, 9);

            /// <summary>A filled heart indicating a friendship level.</summary>
            public static readonly Rectangle FilledHeart = new Rectangle(211, 428, 7, 6);

            /// <summary>An empty heart indicating a missing friendship level.</summary>
            public static readonly Rectangle EmptyHeart = new Rectangle(218, 428, 7, 6);

            /// <summary>A filled checkbox icon.</summary>
            public static readonly Rectangle FilledCheckbox = new Rectangle(236, 425, 9, 9);

            /// <summary>An exit button.</summary>
            public static readonly Rectangle ExitButton = new Rectangle(337, 494, 12, 12);

            /// <summary>A stardrop icon.</summary>
            public static readonly Rectangle Stardrop = new Rectangle(346, 392, 8, 8);
        }
    }
}