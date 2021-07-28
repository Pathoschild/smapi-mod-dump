/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.IO;

namespace PlatoTK.Content
{
    internal class TextureHelper : InnerHelper, ITextureHelper
    {
        internal static Texture2D _whitePixel;

        internal static Texture2D _whiteCircle;

        public Texture2D WhitePixel => _whitePixel;

        public Texture2D WhiteCircle => _whiteCircle;

        public TextureHelper(IPlatoHelper helper)
            : base(helper)
        {
            if (_whitePixel == null)
            {
                _whitePixel = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
                Color[] colors = new Color[1] { Color.White };
                _whitePixel.SetData(colors);
            }

            if (_whiteCircle == null)
                _whiteCircle = GetCircle(499,Color.White);
        }

        public Texture2D ExtractArea(Texture2D texture, Rectangle targetArea)
        {
            Color[] data = new Color[texture.Width * texture.Height];
            texture.GetData(data);
            int w = targetArea.Width;
            int h = targetArea.Height;
            Color[] data2 = new Color[w * h];

            int x2 = targetArea.X;
            int y2 = targetArea.Y;

            for (int x = x2; x < w + x2; x++)
                for (int y = y2; y < h + y2; y++)
                    data2[(y - y2) * w + (x - x2)] = data[y * texture.Width + x];

            Texture2D result = new Texture2D(texture.GraphicsDevice, w, h);
            result.SetData(data2);
            return result;
        }

        public Texture2D ExtractTile(Texture2D texture, int index, int tileWidth = 16, int tileHeight = 16)
        {
            Rectangle sourceRectangle = Game1.getSourceRectForStandardTileSheet(texture, index, tileWidth, tileHeight);
            return ExtractArea(texture, sourceRectangle);
        }

        public Texture2D GetTrimed(Texture2D texture)
        {
            Color[] data = new Color[texture.Width * texture.Height];
            texture.GetData(data);

            int trimX = -1;
            int trimY = -1;
            int trimX2 = -1;
            int trimY2 = -1;

            for (int x = 0; x < texture.Width; x++)
                for (int y = 0; y < texture.Height; y++)
                    if (data[y * texture.Width + x].A != 0)
                    {
                        if (trimX == -1 || x < trimX)
                            trimX = x;

                        if (trimY == -1 || y < trimY)
                            trimY = y;
                    }

            for (int x = texture.Width - 1; x > -1; x--)
                for (int y = texture.Height - 1; y > -1; y--)
                    if (data[y * texture.Width + x].A != 0)
                    {
                        if (trimX2 == -1 || x > trimX2)
                            trimX2 = x;

                        if (trimY2 == -1 || y > trimY2)
                            trimY2 = y;
                    }

            int trimW = trimX2 - trimX;
            int trimH = trimY2 - trimY;

            Texture2D result = ExtractArea(texture, new Rectangle(trimX, trimY, trimW, trimH));
            return result;
        }

        public Texture2D GetPatched(Texture2D texture, Point target, Texture2D patch, Rectangle? sourceArea = null)
        {
            if(sourceArea.HasValue)
                patch = ExtractArea(patch, sourceArea.Value);

            Color[] data = new Color[texture.Width * texture.Height];
            texture.GetData(data);
            int w = patch.Width;
            int h = patch.Height;
            Color[] data2 = new Color[w * h];
            patch.GetData(data2);

            int x2 = target.X;
            int y2 = target.Y;

            for (int x = x2; x < w + x2; x++)
                for (int y = y2; y < h + y2; y++)
                    data[y * texture.Width + x] = data2[(y - y2) * w + (x - x2)];

            Texture2D result = new Texture2D(texture.GraphicsDevice, texture.Width, texture.Height);
            result.SetData(data);
            return result;
        }

        public Texture2D GetCircle(int radius, Color color)
        {
            int diameter = radius * 2;
            Rectangle bounds = new Rectangle(0, 0, diameter + 1, diameter + 1);
            Point c = bounds.Center;
            int sDist = radius * radius;
            return GetRectangle(bounds.Width, bounds.Height, (x, y, w, h) => GetSquaredDistance(new Point(x, y), c) > sDist ? Color.Transparent : color);
        }

        public Texture2D GetRectangle(int width, int height, Color color)
        {
            Texture2D rect = new Texture2D(Game1.graphics.GraphicsDevice, width, height);
            Color[] data = new Color[width * height];
            for (int i = 0; i < data.Length; ++i)
                data[i] = color;
            rect.SetData(data);
            return rect;
        }

        internal Texture2D GetRectangle(int width, int height, Func<int, int, int, int, Color> colorPicker)
        {
            Texture2D rect = new Texture2D(Game1.graphics.GraphicsDevice, width, height);

            Color[] data = new Color[width * height];
            for (int i = 0; i < data.Length; ++i)
            {
                int x = i % width;
                int y = (i - x) / width;
                data[i] = colorPicker(x, y, width, height);
            }
            rect.SetData(data);
            return rect;
        }

        internal float GetSquaredDistance(Point point1, Point point2)
        {
            float a = (point1.X - point2.X);
            float b = (point1.Y - point2.Y);
            return (a * a) + (b * b);
        }

        public Texture2D ResizeTexture(Texture2D texture, int width, int height)
        {
            using (MemoryStream mem = new MemoryStream())
            {
                texture.SaveAsPng(mem, width, height);
                return Texture2D.FromStream(texture.GraphicsDevice, mem);
            }
        }

        public bool TryParseColorFromString(string value, out Color color)
        {
            if (value == null || value.Length < 7 || !value.StartsWith("#"))
            {
                color = Color.White;
                return false;
            }

            if (value.Length == 7)
            {
                byte r = (byte)(Convert.ToUInt32(value.Substring(1, 2), 16));
                byte g = (byte)(Convert.ToUInt32(value.Substring(3, 2), 16));
                byte b = (byte)(Convert.ToUInt32(value.Substring(5, 2), 16));
                color = new Color() { R = r, G = g, B = b };
                return true;
            }
            else if (value.Length == 9)
            {
                byte a = (byte)(Convert.ToUInt32(value.Substring(1, 2), 16));
                byte r = (byte)(Convert.ToUInt32(value.Substring(3, 2), 16));
                byte g = (byte)(Convert.ToUInt32(value.Substring(5, 2), 16));
                byte b = (byte)(Convert.ToUInt32(value.Substring(7, 2), 16));

                color = new Color() { R = r, G = g, B = b, A = a };
                return true;
            }

            color = Color.White;
            return false;
        }

        public string ColorToString(Color color)
        {
            return "#" + (color.A != 255 ? color.A.ToString("X2") : "") + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }
    }
}
