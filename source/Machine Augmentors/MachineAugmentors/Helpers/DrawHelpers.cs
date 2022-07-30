/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-MachineAugmentors
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MachineAugmentors.Helpers
{
    public static class DrawHelpers
    {
        /// <summary>The width of a single digit when rendered via <see cref="Utility.drawTinyDigits(int, SpriteBatch, Vector2, float, float, Color)"/> with a base scale of 1.0f</summary>
        public const int TinyDigitBaseWidth = 5;
        /// <summary>The height of a single digit when rendered via <see cref="Utility.drawTinyDigits(int, SpriteBatch, Vector2, float, float, Color)"/> with a base scale of 1.0f</summary>
        public const int TinyDigitBaseHeight = 7;

        /// <summary>Returns the width of a number drawn via <see cref="Utility.drawTinyDigits(int, SpriteBatch, Vector2, float, float, Color)"/></summary>
        public static float MeasureNumber(int Number, float Scale)
        {
            int NumDigits = GetNumDigits(Number);
            return MeasureDigits(NumDigits, Scale);
        }

        /// <summary>Returns the width of a number containing the specific amount of digits, drawn via <see cref="Utility.drawTinyDigits(int, SpriteBatch, Vector2, float, float, Color)"/></summary>
        public static float MeasureDigits(int DigitCount, float Scale)
        {
            return TinyDigitBaseWidth * DigitCount * Scale;
        }

        public static int GetNumDigits(int Value)
        {
            if (Value == 0)
                return 1;
            else
                return (int)Math.Floor(Math.Log10(Value) + 1);
        }

        private const float SpecialNumberDigitScale = 3.5f;

        /// <summary>Draws the given Text using <see cref="Utility.drawTinyDigits(int, SpriteBatch, Vector2, float, float, Color)"/> for the numbers,<para/>
        /// or <see cref="DrawStringWithShadow(SpriteBatch, SpriteFont, string, float, float, Color, Color, int, int)"/> for the other characters.<para/>
        /// Warning - Does NOT support linebreaks.</summary>
        public static void DrawStringWithSpecialNumbers(SpriteBatch SB, Vector2 Position, string Text, float Scale, Color Color)
        {
            float DigitScale = SpecialNumberDigitScale * Scale;

            List<string> Substrings = SplitByNumbers(Text);

            //  The '%' character needs to be rendered lower than other characters because Game1.tinyFont sucks and doesn't have good character positions
            Substrings = Substrings.SelectMany(x => x.SplitAndKeepDelimiter(new char[] { '%' })).ToList();

            Dictionary<string, Vector2> SubstringSizes = new Dictionary<string, Vector2>();
            foreach (string Substring in Substrings)
            {
                if (SubstringSizes.ContainsKey(Substring))
                    continue;

                if (int.TryParse(Substring, out int Number))
                {
                    SubstringSizes.Add(Substring, new Vector2(MeasureDigits(Substring.Length, DigitScale), TinyDigitBaseHeight * DigitScale));
                }
                else
                {
                    SubstringSizes.Add(Substring, Game1.tinyFont.MeasureString(Substring) * Scale);
                }
            }

            Vector2 TotalSize = new Vector2(SubstringSizes.Values.Sum(x => x.X), SubstringSizes.Values.Max(x => x.Y));

            Vector2 CurrentPosition = Position;
            foreach (string Substring in Substrings)
            {
                Vector2 SubstringSize = SubstringSizes[Substring];
                Vector2 SubstringPosition = new Vector2(CurrentPosition.X, CurrentPosition.Y + (TotalSize.Y - SubstringSize.Y) / 2.0f);

                if (int.TryParse(Substring, out int Number))
                {
                    //  Draw leading zeroes
                    if (Number != 0)
                    {
                        for (int i = 0; i < Substring.Length; i++)
                        {
                            if (Substring[i] == '0')
                            {
                                Utility.drawTinyDigits(0, SB, SubstringPosition, DigitScale, 1.0f, Color);
                                SubstringPosition.X += MeasureDigits(1, DigitScale);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    //  Draw the rest of the number
                    Utility.drawTinyDigits(Number, SB, SubstringPosition, DigitScale, 1.0f, Color);
                }
                else
                {
                    if (Substring == "%")
                        SubstringPosition.Y += 3 * Scale;

                    DrawStringWithShadow(SB, Game1.tinyFont, Substring, SubstringPosition.X, SubstringPosition.Y, Scale, Color, Color.Black, 2, 2);
                }

                CurrentPosition.X += SubstringSize.X;
            }
        }

        /// <summary>Returns the rendered size of the given text when drawn with <see cref="DrawStringWithSpecialNumbers(SpriteBatch, Vector2, string, float)"/><para/>
        /// Warning - Does NOT support linebreaks.</summary>
        public static Vector2 MeasureStringWithSpecialNumbers(string Text, float Scale, float VerticalPadding)
        {
            List<string> Substrings = SplitByNumbers(Text);

            float DigitScale = SpecialNumberDigitScale * Scale;

            Vector2 Size = new Vector2();
            foreach (string Substring in Substrings)
            {
                Vector2 SubstringSize;
                if (int.TryParse(Substring, out int Number))
                    SubstringSize = new Vector2(MeasureDigits(Substring.Length, DigitScale), TinyDigitBaseHeight * DigitScale);
                else
                    SubstringSize = Game1.tinyFont.MeasureString(Substring) * Scale;

                Size.X += SubstringSize.X;
                Size.Y = Math.Max(Size.Y, SubstringSize.Y);
            }

            return new Vector2(Size.X, Size.Y + VerticalPadding * 2);
        }

        /// <summary>Splits the given string into substrings where each one is either all digits, or no digits.<para/>
        /// EX: "Hello world (25.3%)" splits into { "Hello world (", "25", ".", "3", "%)" }</summary>
        private static List<string> SplitByNumbers(string Text)
        {
            List<string> Substrings = new List<string>();
            for (int i = 0; i < Text.Length; i++)
            {
                string CurrentSubstring = "";
                bool FindDigits = char.IsDigit(Text[i]);
                while (i < Text.Length && char.IsDigit(Text[i]) == FindDigits)
                {
                    CurrentSubstring += Text[i];
                    i++;
                }
                Substrings.Add(CurrentSubstring);
                i--;
            }
            return Substrings;
        }

        public static void DrawBox(SpriteBatch b, Rectangle Destination) { DrawBox(b, Destination.X, Destination.Y, Destination.Width, Destination.Height); }
        public static void DrawBox(SpriteBatch b, int X, int Y, int Width, int Height)
        {
            //  Fill in the box
            //b.Draw(Game1.menuTexture, new Rectangle(X + 12, Y + 16, Width - 12 - 16, Height - 12 - 16), new Rectangle(12, 272, 1, 32), Color.White); // non-smooth gradient but still looks decent
            b.Draw(Game1.menuTexture, new Rectangle(X + 12, Y + 16, Width - 12 - 16, Height - 12 - 16), new Rectangle(64, 128, 64, 64), Color.White); // smoother gradient tile
            //  Draw top-left corner
            b.Draw(Game1.menuTexture, new Vector2(X, Y), new Rectangle(0, 256, 16, 16), Color.White);
            //  Draw top center
            b.Draw(Game1.menuTexture, new Rectangle(X + 16, Y, Width - 16 * 2, 16), new Rectangle(16, 256, 16, 16), Color.White);
            //  Draw top-right corner
            b.Draw(Game1.menuTexture, new Vector2(X + Width - 16, Y), new Rectangle(44, 256, 16, 16), Color.White);
            //  Draw left center
            b.Draw(Game1.menuTexture, new Rectangle(X, Y + 16, 12, Height - 16 - 12), new Rectangle(0, 272, 12, 12), Color.White);
            //  Draw right center
            b.Draw(Game1.menuTexture, new Rectangle(X + Width - 16, Y + 16, 16, Height - 16 - 12), new Rectangle(44, 272, 16, 16), Color.White);
            //  Draw bottom-left corner
            b.Draw(Game1.menuTexture, new Vector2(X, Y + Height - 12), new Rectangle(0, 304, 12, 12), Color.White);
            //  Draw bottom center
            b.Draw(Game1.menuTexture, new Rectangle(X + 12, Y + Height - 12, Width - 12 * 2, 12), new Rectangle(12, 304, 12, 12), Color.White);
            //  Draw bottom-right corner
            b.Draw(Game1.menuTexture, new Vector2(X + Width - 12, Y + Height - 12), new Rectangle(48, 304, 12, 12), Color.White);
        }

        /// <summary>Draws a horizontal separator (I.E. a horizontal line) using the same menu textures as the horizontal separator in the main GameMenu inventory tab</summary>
        public static void DrawHorizontalSeparator(SpriteBatch b, Rectangle Position) { DrawHorizontalSeparator(b, Position.X, Position.Y, Position.Width, Position.Height); }
        /// <summary>Draws a horizontal separator (I.E. a horizontal line) using the same menu textures as the horizontal separator in the main GameMenu inventory tab</summary>
        public static void DrawHorizontalSeparator(SpriteBatch b, int X, int Y, int Width, int Height = 24)
        {
            int DefaultHeight = 24;
            int EdgeSize = (int)(32 * (Height * 1.0 / DefaultHeight));

            int XMargin = (int)(-4 * (Height * 1.0 / DefaultHeight));
            X += XMargin;
            Width -= 2 * XMargin;

            //  Draw the center portion
            b.Draw(Game1.menuTexture, new Rectangle(X + EdgeSize, Y, Width - EdgeSize * 2, Height), new Rectangle(44, 84, 1, 24), Color.White);
            //  Draw the left edge
            b.Draw(Game1.menuTexture, new Rectangle(X, Y, EdgeSize, Height), new Rectangle(12, 84, 32, 24), Color.White);
            //  Draw the right edge
            b.Draw(Game1.menuTexture, new Rectangle(X + Width - EdgeSize, Y, EdgeSize, Height), new Rectangle(212, 84, 32, 24), Color.White);
        }

        public static void DrawBorder(SpriteBatch b, Rectangle Destination, int Thickness, Color Color)
        {
            Texture2D TextureColor = TextureHelpers.GetSolidColorTexture(Game1.graphics.GraphicsDevice, Color);
            b.Draw(TextureColor, new Rectangle(Destination.X, Destination.Y, Thickness, Destination.Height), Color.White); // Left border
            b.Draw(TextureColor, new Rectangle(Destination.Right - Thickness, Destination.Y, Thickness, Destination.Height), Color.White); // Right border
            b.Draw(TextureColor, new Rectangle(Destination.X + Thickness, Destination.Y, Destination.Width - Thickness * 2, Thickness), Color.White); // Top border
            b.Draw(TextureColor, new Rectangle(Destination.X + Thickness, Destination.Bottom - Thickness, Destination.Width - Thickness * 2, Thickness), Color.White); // Bottom border
        }

        public static void DrawStringWithShadow(SpriteBatch b, SpriteFont Font, string Text, float XPosition, float YPosition, float Scale, Color MainColor, Color ShadowColor, int ShadowXOffset, int ShadowYOffset)
        {
            Vector2 Position = new Vector2(XPosition, YPosition);
            b.DrawString(Font, Text, Position + new Vector2(ShadowXOffset, ShadowYOffset), ShadowColor, 0.0f, Vector2.Zero, Scale, SpriteEffects.None, 1.0f);
            b.DrawString(Font, Text, Position, MainColor, 0.0f, Vector2.Zero, Scale, SpriteEffects.None, 1.0f);
        }

        /// <summary>Returns the topleft position to draw a component of the given Size, so that it is guaranteed to be fully visible on screen</summary>
        /// <param name="Size">The size of the component to draw</param>
        /// <param name="Anchor">The desired rectangle to anchor the drawn component to.<para/>
        /// By default, it will attempt to draw the component at the bottom-right of this anchor. But if it doesn't fit within the viewport, it will be moved to the left and/or top side of the anchor.</param>
        /// <param name="Padding">An additional threshold that defines how close the drawn component can get to the edge of the game's viewport. Use 0 to allow the component to just barely touch the edge before re-positioning to the other side of the anchor.</param>
        public static Point GetTopleftPosition(Point Size, Rectangle Anchor, int Padding = 30)
        {
            int XPosition = Anchor.Right + Size.X > Game1.viewport.Size.Width - Padding ? Anchor.Left - Size.X : Anchor.Right;
            int YPosition = Anchor.Bottom + Size.Y > Game1.viewport.Size.Height - Padding ? Anchor.Top - Size.Y : Anchor.Bottom;
            return new Point(XPosition, YPosition);
        }

        /// <summary>Returns the topleft position to draw a component of the given Size, so that it is guaranteed to be fully visible on screen</summary>
        /// <param name="Size">The size of the component to draw</param>
        /// <param name="Anchor">The desired top-left position of the component to draw. (in screen-space)<para/>
        /// Use <see cref="Game1.getMouseX"/> and <see cref="Game1.getMouseY"/> to anchor the drawn component around the mouse cursor</param>
        /// <param name="Padding">An additional threshold that defines how close the drawn component can get to the edge of the game's viewport. Use 0 to allow the component to just barely touch the edge before re-positioning to the other side of the anchor.</param>
        public static Point GetTopleftPosition(Point Size, Point Anchor, int Padding = 30)
        {
            return GetTopleftPosition(Size, new Rectangle(Anchor.X - 8, Anchor.Y - 8, 8 + 48, 8 + 48), Padding);
        }
    }
}
