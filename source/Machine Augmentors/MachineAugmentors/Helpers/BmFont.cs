/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-MachineAugmentors
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MachineAugmentors.Helpers
{
    public class BmFont
    {
        public string fontFilePath { get; }
        public FontFile fontFile { get; }
        public Texture2D fontTexture { get; }
        public FontRenderer _fontRenderer { get; }

        public BmFont(string fontFilePath, Texture2D fontTexture)
        {
            this.fontFilePath = fontFilePath;
            fontFile = FontLoader.Load(fontFilePath);
            this.fontTexture = fontTexture;
            _fontRenderer = new FontRenderer(fontFile, this.fontTexture);
        }

        public void Draw(SpriteBatch _spriteBatch, string message, Vector2 pos, float scale, Color color)
        {
            _fontRenderer.DrawText(_spriteBatch, (int)pos.X, (int)pos.Y, message, scale, color);
        }

        public Vector2 Measure(string text, float scale)
        {
            return _fontRenderer.Measure(text, scale);
        }
    }


    public class FontRenderer
    {
        public static FontFile Load(Stream stream)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(FontFile));
            FontFile file = (FontFile)deserializer.Deserialize(stream);
            return file;
        }

        public FontRenderer(FontFile fontFile, Texture2D fontTexture)
        {
            _fontFile = fontFile;
            _texture = fontTexture;
            _characterMap = new Dictionary<char, FontChar>();

            foreach (var fontCharacter in _fontFile.Chars)
            {
                char c = (char)fontCharacter.ID;
                _characterMap.Add(c, fontCharacter);
            }
        }

        private Dictionary<char, FontChar> _characterMap { get; }
        private FontFile _fontFile { get; }
        private Texture2D _texture { get; }

        public void DrawText(SpriteBatch spriteBatch, int x, int y, string text, float scale, Color color)
        {
            float dx = x;
            float dy = y;
            foreach (char c in text)
            {
                if (c == '\n')
                {
                    dy += _fontFile.Info.Size * scale;
                    dx = x;
                }

                if (_characterMap.TryGetValue(c, out FontChar fc))
                {
                    var sourceRectangle = new Rectangle(fc.X, fc.Y, fc.Width, fc.Height);
                    var position = new Vector2(dx + fc.XOffset * scale, dy + fc.YOffset * scale);
                    spriteBatch.Draw(_texture, position, sourceRectangle, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
                    dx += fc.XAdvance * scale;
                }
            }
        }

        public Vector2 Measure(string text, float scale)
        {
            List<float> LineWidths = new List<float>();

            float CurrentLineWidth = 0;
            foreach (char c in text)
            {
                if (c == '\n')
                {
                    LineWidths.Add(CurrentLineWidth);
                    CurrentLineWidth = 0;
                }

                if (_characterMap.TryGetValue(c, out FontChar fc))
                {
                    CurrentLineWidth += fc.XAdvance * scale;
                }
            }
            LineWidths.Add(CurrentLineWidth);

            return new Vector2(LineWidths.Max(), LineWidths.Count * _fontFile.Info.Size * scale);
        }
    }

    [Serializable]
    [XmlRoot("font")]
    public class FontFile
    {
        [XmlElement("info")]
        public FontInfo Info { get; set; }

        [XmlElement("common")]
        public FontCommon Common { get; set; }

        [XmlArray("pages")]
        [XmlArrayItem("page")]
        public List<FontPage> Pages { get; set; }

        [XmlArray("chars")]
        [XmlArrayItem("char")]
        public List<FontChar> Chars { get; set; }

        [XmlArray("kernings")]
        [XmlArrayItem("kerning")]
        public List<FontKerning> Kernings { get; set; }
    }

    [Serializable]
    public class FontInfo
    {
        [XmlAttribute("face")]
        public string Face { get; set; }

        [XmlAttribute("size")]
        public int Size { get; set; }

        [XmlAttribute("bold")]
        public int Bold { get; set; }

        [XmlAttribute("italic")]
        public int Italic { get; set; }

        [XmlAttribute("charset")]
        public string CharSet { get; set; }

        [XmlAttribute("unicode")]
        public int Unicode { get; set; }

        [XmlAttribute("stretchH")]
        public int StretchHeight { get; set; }

        [XmlAttribute("smooth")]
        public int Smooth { get; set; }

        [XmlAttribute("aa")]
        public int SuperSampling { get; set; }

        private Rectangle _Padding;
        [XmlAttribute("padding")]
        public string Padding
        {
            get
            {
                return _Padding.X + "," + _Padding.Y + "," + _Padding.Width + "," + _Padding.Height;
            }
            set
            {
                string[] padding = value.Split(',');
                _Padding = new Rectangle(Convert.ToInt32(padding[0]), Convert.ToInt32(padding[1]), Convert.ToInt32(padding[2]), Convert.ToInt32(padding[3]));
            }
        }

        private Point _Spacing;
        [XmlAttribute("spacing")]
        public string Spacing
        {
            get
            {
                return _Spacing.X + "," + _Spacing.Y;
            }
            set
            {
                string[] spacing = value.Split(',');
                _Spacing = new Point(Convert.ToInt32(spacing[0]), Convert.ToInt32(spacing[1]));
            }
        }

        [XmlAttribute("outline")]
        public int OutLine { get; set; }
    }

    [Serializable]
    public class FontCommon
    {
        [XmlAttribute("lineHeight")]
        public int LineHeight { get; set; }

        [XmlAttribute("base")]
        public int Base { get; set; }

        [XmlAttribute("scaleW")]
        public int ScaleW { get; set; }

        [XmlAttribute("scaleH")]
        public int ScaleH { get; set; }

        [XmlAttribute("pages")]
        public int Pages { get; set; }

        [XmlAttribute("packed")]
        public int Packed { get; set; }

        [XmlAttribute("alphaChnl")]
        public int AlphaChannel { get; set; }

        [XmlAttribute("redChnl")]
        public int RedChannel { get; set; }

        [XmlAttribute("greenChnl")]
        public int GreenChannel { get; set; }

        [XmlAttribute("blueChnl")]
        public int BlueChannel { get; set; }
    }

    [Serializable]
    public class FontPage
    {
        [XmlAttribute("id")]
        public int ID { get; set; }

        [XmlAttribute("file")]
        public string File { get; set; }
    }

    [Serializable]
    public class FontChar
    {
        [XmlAttribute("id")]
        public int ID { get; set; }

        [XmlAttribute("x")]
        public int X { get; set; }

        [XmlAttribute("y")]
        public int Y { get; set; }

        [XmlAttribute("width")]
        public int Width { get; set; }

        [XmlAttribute("height")]
        public int Height { get; set; }

        [XmlAttribute("xoffset")]
        public int XOffset { get; set; }

        [XmlAttribute("yoffset")]
        public int YOffset { get; set; }

        [XmlAttribute("xadvance")]
        public int XAdvance { get; set; }

        [XmlAttribute("page")]
        public int Page { get; set; }

        [XmlAttribute("chnl")]
        public int Channel { get; set; }
    }

    [Serializable]
    public class FontKerning
    {
        [XmlAttribute("first")]
        public int First { get; set; }

        [XmlAttribute("second")]
        public int Second { get; set; }

        [XmlAttribute("amount")]
        public int Amount { get; set; }
    }

    public class FontLoader
    {
        public static FontFile Load(String filename)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(FontFile));
            TextReader textReader = new StreamReader(filename);
            FontFile file = (FontFile)deserializer.Deserialize(textReader);
            textReader.Close();
            return file;
        }
    }
}