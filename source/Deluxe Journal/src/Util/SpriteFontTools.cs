/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace DeluxeJournal.Util
{
    /// <summary>Provides methods for manipulating on-screen text displayed using a <see cref="SpriteFont"/>.</summary>
    public class SpriteFontTools
    {
        private readonly SpriteFont.Glyph _defaultGlyph;
        private string _ellipsis;
        private float _ellipsisWidth;

        /// <summary>The font reference.</summary>
        public SpriteFont Font { get; }

        /// <summary>Dictionary mapping characters to font glyphs.</summary>
        public Dictionary<char, SpriteFont.Glyph> Glyphs { get; }

        /// <summary>Get the <see cref="SpriteFont.LineSpacing"/> for the underlying font.</summary>
        public int LineSpacing => Font.LineSpacing;

        /// <summary>Trailing indicator to show that a string has been truncated.</summary>
        /// <seealso cref="Truncate(string, int, out string)"/>
        public string OverflowEllipsis
        {
            get => _ellipsis;

            set
            {
                _ellipsis = value;
                _ellipsisWidth = string.IsNullOrEmpty(value) ? 0f : Font.MeasureString(value).X;
            }
        }

        public SpriteFontTools(SpriteFont font, string ellipsis = "...")
        {
            Font = font;
            Glyphs = font.GetGlyphs();
            OverflowEllipsis = _ellipsis = ellipsis;

            if (font.DefaultCharacter is char defaultCharacter && Glyphs.TryGetValue(defaultCharacter, out var glyph))
            {
                _defaultGlyph = glyph;
            }
            else
            {
                _defaultGlyph = SpriteFont.Glyph.Empty;
            }
        }

        /// <summary>
        /// Truncate a string of text given a bounding width. Additionally, the truncated text has the specified
        /// <see cref="OverflowEllipsis"/> appended to the end of the string.
        /// </summary>
        /// <param name="text">Target text to be truncated.</param>
        /// <param name="width">Width to begin truncation.</param>
        /// <param name="truncated">Output text that has been truncated (if necessary). Equals <paramref name="text"/> if the return is <c>false</c>.</param>
        /// <returns><c>true</c> if the text was truncated; <c>false</c> if the text was shorter than <paramref name="width"/>.</returns>
        public bool Truncate(string text, float width, out string truncated)
        {
            float currentWidth = 0f;
            int overflow = 0;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                SpriteFont.Glyph glyph = Glyphs.GetValueOrDefault(c, _defaultGlyph);
                currentWidth += glyph.LeftSideBearing + glyph.Width;

                if (currentWidth > width)
                {
                    truncated = text[0..(i - overflow)] + OverflowEllipsis;
                    return true;
                }
                else if (currentWidth > width - _ellipsisWidth)
                {
                    overflow++;
                }

                currentWidth += glyph.RightSideBearing + Font.Spacing;
            }

            truncated = text;
            return false;
        }

        /// <summary>Wrap text in-place given a bounding width.</summary>
        /// <remarks>
        /// All existing line-feed characters <c>\n</c> are ignored. User-inserted newlines must be denoted with a
        /// carriage-return <c>\r</c> and will be replaced with a carriage-return + line-feed <c>\r\n</c> combination.
        /// </remarks>
        /// <param name="text">Mutable string of text characters.</param>
        /// <param name="width">Text wrapping width.</param>
        /// <returns>The number of newline characters <c>\n</c> in the <paramref name="text"/> after wrapping.</returns>
        public int Wrap(StringBuilder text, float width)
        {
            float currentWidth = 0f;
            int newlines = 0;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                if (c == '\r')
                {
                    currentWidth = 0f;
                    newlines++;

                    if (++i == text.Length || text[i] != '\n')
                    {
                        text.Insert(i, '\n');
                    }
                }
                else if (c == '\n')
                {
                    text.Remove(i--, 1);
                }
                else if (Glyphs.TryGetValue(c, out SpriteFont.Glyph glyph))
                {
                    currentWidth += glyph.WidthIncludingBearings;

                    if (currentWidth > width)
                    {
                        currentWidth = Math.Max(glyph.LeftSideBearing, 0f) + glyph.Width + glyph.RightSideBearing;
                        newlines++;
                        text.Insert(i++, '\n');
                    }
                    else
                    {
                        currentWidth += Font.Spacing;
                    }
                }
            }

            return newlines;
        }
    }
}
