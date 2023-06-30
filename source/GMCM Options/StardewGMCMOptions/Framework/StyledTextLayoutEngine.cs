/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jltaylor-us/StardewGMCMOptions
**
*************************************************/

// Copyright 2023 Jamie Taylor
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace GMCMOptions.Framework {
    public class StyledTextLayoutEngine : ITextLayoutEngine {
        private readonly Action<string, LogLevel> log;
        List<StyledTextLine> styledTextLines = new();
        public StyledTextLayoutEngine(Action<string, LogLevel> log) {
            this.log = log;
        }

        /// <inheritdoc/>
        public void DrawLastLayout(SpriteBatch b, int left, int top) {
            foreach (var line in styledTextLines) {
                line.Draw(b, left, top);
                top += Game1.smallFont.LineSpacing;
            }
        }

        /// <inheritdoc/>
        public int Layout(string text, int width) {
            this.styledTextLines = Wrap(ParseText(text), width);
            return Game1.smallFont.LineSpacing * styledTextLines.Count;
        }

        /// <summary>
        ///   Turn a list of <c cref="StyledText">StyledText</c> chunks into a list of
        ///   <c cref="StyledTextLine">StyledTextLine</c>s.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        private static List<StyledTextLine> Wrap(List<StyledText> items, int width) {
            List<StyledTextLine> result = new();
            if (items.Count == 0) {
                return result;
            }
            SplitStyle splitStyle = SplitStyle.Word;
            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ja || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.zh || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.th) {
                // these are the languages handled this way in Game1.parseText.
                splitStyle = SplitStyle.Char;
            }
            StyledTextLine currentLine = new();
            foreach (var itemItr in items) {
                var item = itemItr;
                while (item.Width > width - currentLine.Width) {
                    (StyledText? i1, StyledText i2) = item.Split(width - currentLine.Width, currentLine.Empty(), splitStyle);
                    item = i2;
                    if (i1 is not null) {
                        currentLine.Add(i1);
                    }
                    result.Add(currentLine);
                    currentLine = new();
                }
                if (item.Width > 0) {
                    currentLine.Add(item);
                }
            }
            if (!currentLine.Empty()) {
                result.Add(currentLine);
            }
            return result;
        }

        /// <summary>
        ///   Parse the given text into a list of <c cref="StyledText">StyledText</c> chunks.
        /// </summary>
        private List<StyledText> ParseText(string text) {
            List<StyledText> items = new();
            XmlReaderSettings settings = new();
            settings.XmlResolver = null; // don't try to resolve any external references
            settings.ConformanceLevel = ConformanceLevel.Auto;
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreComments = true;
            settings.IgnoreWhitespace = true; // only ignores "insignificant" whitespace
            var reader = XmlReader.Create(new System.IO.StringReader(text), settings);
            Stack<bool> boldStack = new();
            boldStack.Push(false);
            Stack<Color> colorStack = new();
            colorStack.Push(Game1.textColor);
            try {
                while (reader.Read()) {
                    if (reader.NodeType == XmlNodeType.Text || reader.NodeType == XmlNodeType.CDATA) {
                        StyledText st = new(reader.Value);
                        st.isBold = boldStack.Peek();
                        st.color = colorStack.Peek();
                        items.Add(st);
                    } else if (reader.NodeType == XmlNodeType.Element) {
                        switch (reader.Name) {
                            case "b":
                                boldStack.Push(true);
                                break;
                            case "color":
                                Color c = GetColorFromTag(reader) ?? colorStack.Peek();
                                colorStack.Push(c);
                                break;
                            default:
                                log($"ignoring unknown element {reader.Name}", LogLevel.Info);
                                // just ingore the tag.  (The other reasonable option would be to turn it into
                                // plain text, but ignoring it is more forward-compatible.)
                                break;
                        }
                    } else if (reader.NodeType == XmlNodeType.EndElement) {
                        // parser will ensure that tags are balanced; no need to worry about underflow
                        switch (reader.Name) {
                            case "b":
                                boldStack.Pop();
                                break;
                            case "color":
                                colorStack.Pop();
                                break;
                            default:
                                // we already reported an unknown tag when it was opened
                                break;
                        }
                    }
                }
            } catch (XmlException ex) {
                log($"text is not valid XML: {ex.Message}", LogLevel.Warn);
            }
            return items;
        }

        /// <summary>Attempt to get a color based on the current tag's attributes.</summary>
        private Color? GetColorFromTag(XmlReader reader) {
            string? colorName = reader["name"];
            if (colorName is not null) {
                Color? c = GetColorNamed(colorName);
                if (c is null) {
                    log($"unknown color name \"{colorName}\"; ignoring", LogLevel.Warn);
                }
                return c;
            }
            int attrsPresent = 0;
            Byte parseByte(string which) {
                string? str = reader[which];
                if (str is null) {
                    return 0;
                }
                attrsPresent++;
                byte b = 0;
                if (!Byte.TryParse(str, out b)) {
                    log($"Invalid value for color attribute {which}: \"{str}\".  (Value must be a number 0-255); using 0", LogLevel.Warn);
                }
                return b;
            }
            byte r = parseByte("r");
            byte g = parseByte("g");
            byte b = parseByte("b");
            if (attrsPresent == 0) {
                log("color has neither \"name\" nor \"r\", \"g\", and \"b\" attributes; ignoring", LogLevel.Warn);
                return null;
            }
            return new Color(r, g, b);
        }

        /// <summary>Get the color from the matching field in <c cref="Color">Color</c>.</summary>
        /// <param name="colorName">
        ///   The color name.  If no color with that name is found, attempt a match on <paramref name="colorName"/>
        ///   with its first character uppercased.
        /// </param>
        /// <returns>The matching color if it exists, or <c>null</c>.</returns>
        private static Color? GetColorNamed(string colorName) {
            if (colorName == "") return null;
            PropertyInfo? pi = typeof(Color).GetProperty(colorName);
            if (pi is null) {
                pi = typeof(Color).GetProperty(colorName.Substring(0, 1).ToUpperInvariant() + colorName.Substring(1));
            }
            return (Color?)pi?.GetValue(null);
        }
    }

    internal enum SplitStyle {
        Word, Char
    }

    /// <summary>A chunk of text that has one style</summary>
    internal class StyledText {
        public bool isBold = false;
        public Color color = StardewValley.Game1.textColor;
        public string Text { get; private set; }
        public int Width { get; }
        public int Height { get; }
        public StyledText(string text) {
            this.Text = text;
            var size = Game1.smallFont.MeasureString(text);
            Width = (int)size.X;
            Height = (int)size.Y;
        }
        public void Draw(SpriteBatch b, int left, int top) {
            if (isBold) {
                Utility.drawBoldText(b, Text, Game1.smallFont, new Vector2(left, top), color);
            } else {
                b.DrawString(Game1.smallFont, Text, new Vector2(left, top), color);
            }
        }
        /// <summary>
        ///   Return a new <c cref="StyledText">StyledText</c> with the given text and
        ///   this <c>StyledText</c>'s style attributes.
        /// </summary>
        /// <param name="newText">The new text</param>
        /// <returns>A <c>StyledText</c> with the given text and the same style as this object.</returns>
        private StyledText WithNewText(string newText) {
            StyledText result = new(newText);
            result.isBold = isBold;
            result.color = color;
            return result;
        }

        /// <summary>
        ///   Split this object at the given width (plus any trailing whitespace from that point until the next
        ///   non-whitespace character).
        /// </summary>
        /// <param name="width">The maximum width of the text to return in <c>head</c>.</param>
        /// <param name="forceFirst">
        ///   Return <em>something</em> in <c>head</c> even if the first split point is beyond
        ///   <paramref name="width"/>.
        /// </param>
        /// <param name="splitStyle">specifies where splitting may be performed</param>
        /// <returns>
        ///   A pair of <c>StyledText</c> resulting from splitting this object's text into text that fits
        ///   in <paramref name="width"/> and the remaining text (which could be the empty string).  If
        ///   there is no split point before <paramref name="width"/> (and <paramref name="forceFirst"/> is
        ///   <c>false</c>) then the first item will be <c>null</c>.
        /// </returns>
        public (StyledText? head, StyledText tail) Split(int width, bool forceFirst, SplitStyle splitStyle) {
            // Is there a more efficient way to do this?  Yes.  Will the performance here matter?  Unlikely.
            int lastGood = 0;
            for (int cursor = 0; cursor < Text.Length; cursor++) {
                if (splitStyle == SplitStyle.Word && ! Char.IsWhiteSpace(Text, cursor)) {
                    continue;
                }
                if (Game1.smallFont.MeasureString(Text.Substring(0, cursor + 1)).X <= width) {
                    lastGood = cursor;
                    continue;
                }
                if (lastGood == 0 && ! forceFirst) {
                    return (null, this);
                }
                // include any trailing whitespace (when splitting by word)
                if (splitStyle == SplitStyle.Word) {
                    while (Char.IsWhiteSpace(Text, lastGood) && lastGood < Text.Length) lastGood++;
                }
                return (WithNewText(Text.Substring(0, lastGood)), WithNewText(Text.Substring(lastGood)));
            }
            // I don't think we should get here... maybe if the text is wider than allowed but only
            // because of trailing whitespace.
            return (this, WithNewText(""));
        }
    }

    /// <summary>A list of <c cref="StyledText">StyledText</c> objects that belong to one line.</summary>
    internal class StyledTextLine {
        private List<StyledText> items = new();
        public int Width { get; private set; }
        public int Height { get; private set; }
        public void Add(StyledText item) {
            items.Add(item);
            Width += item.Width;
            Height = Math.Max(Height, item.Height);
        }
        public void Draw(SpriteBatch b, int left, int top) {
            foreach (var item in items) {
                item.Draw(b, left, top);
                left += item.Width;
            }
        }
        public bool Empty() {
            return items.Count == 0;
        }
    }
}
