using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace HDSprites
{
    // Modified from PyTK.Types.ScaledTexture2D
    // Origial Source: https://github.com/Platonymous/Stardew-Valley-Mods/blob/master/PyTK/Types/ScaledTexture2D.cs
    // Original Licence: GNU General Public License v3.0
    // Original Author: Platonymous
    public class AssetTexture : Texture2D
    {
        public string AssetName { get; set; }
        public float Scale { get; set; }
        public virtual Texture2D HDTexture { get; set; }
        public virtual Texture2D EXTexture { get; set; }

        private int UniqueID { get; set; }
        
        public AssetTexture(string assetName, Texture2D originalTexture, Texture2D hdTexture, float scale = 1, bool shouldEncode = false)
            : base(originalTexture.GraphicsDevice, originalTexture.Width, originalTexture.Height)
        {
            this.AssetName = assetName;
            this.Scale = scale;
            this.HDTexture = hdTexture;
            this.UniqueID = 0;

            Color[] data = new Color[originalTexture.Width * originalTexture.Height];
            originalTexture.GetData(data);
            if (shouldEncode)
            {
                this.UniqueID = assetName.GetHashCode() & 0xffffff;
                data[0] = Encode(this.UniqueID);
            }
            SetData(data);

            // For Mods such as JSON Assets
            this.EXTexture = null;
            if (this.Height * this.Scale > 4096)
            {
                int lowerHeight = (int)(4096 / this.Scale);
                int upperHeight = this.Height - lowerHeight;

                Texture2D lowerTexture = new Texture2D(this.GraphicsDevice, this.Width, lowerHeight);
                lowerTexture.SetData(0, lowerTexture.Bounds, data, 0, lowerTexture.Width * lowerTexture.Height);

                Texture2D upperTexture = new Texture2D(this.GraphicsDevice, this.Width, upperHeight);
                upperTexture.SetData(0, upperTexture.Bounds, data, upperTexture.Width * lowerHeight, upperTexture.Width * upperTexture.Height);

                bool cursorsFlag = this.AssetName.Equals("LooseSprites\\Cursors");
                this.HDTexture = Upscaler.Upscale(lowerTexture, cursorsFlag);
                this.EXTexture = Upscaler.Upscale(upperTexture, cursorsFlag);
            }
        }

        public void SetOriginalTexture(Texture2D texture)
        {
            Color[] data = new Color[texture.Width * texture.Height];
            texture.GetData(data);
            if (this.UniqueID != 0)
            {
                data[0] = Encode(this.UniqueID);
            }
            SetData(data);
        }

        public void SetSubTexture(Texture2D texture, Rectangle fromArea, Rectangle toArea, bool overlay)
        {
            if (texture == null) return;
            if (fromArea.IsEmpty) fromArea = new Rectangle(0, 0, texture.Width, texture.Height);
            if (toArea.IsEmpty) toArea = new Rectangle(0, 0, HDTexture.Width, HDTexture.Height);
            if (fromArea.Width != toArea.Width || fromArea.Height != toArea.Height) return;

            Color[] hdData = new Color[HDTexture.Width * HDTexture.Height];
            HDTexture.GetData(hdData);

            Color[] subData = new Color[texture.Width * texture.Height];
            texture.GetData(subData);

            for (int x = 0; x < fromArea.Width; ++x)
            {
                for (int y = 0; y < fromArea.Height; ++y)
                {
                    int toIndex = (y + toArea.Y) * HDTexture.Width + (x + toArea.X);
                    Color subColor = subData[(y + fromArea.Y) * texture.Width + (x + fromArea.X)];
                    if (!overlay || subColor.A == 255)
                    {
                        hdData[toIndex] = subColor;
                    }
                    else
                    {
                        Color hdColor = hdData[toIndex];

                        float srcR = subColor.R / 255.0f;
                        float srcG = subColor.G / 255.0f;
                        float srcB = subColor.B / 255.0f;
                        float srcA = subColor.A / 255.0f;

                        float dstR = hdColor.R / 255.0f;
                        float dstG = hdColor.G / 255.0f;
                        float dstB = hdColor.B / 255.0f;
                        float dstA = hdColor.A / 255.0f;

                        float outA = srcA + dstA * (1.0f - srcA);
                        float outR = (srcR * srcA + dstR * dstA * (1.0f - srcA)) / outA;
                        float outG = (srcG * srcA + dstG * dstA * (1.0f - srcA)) / outA;
                        float outB = (srcB * srcA + dstB * dstA * (1.0f - srcA)) / outA;

                        Color outColor = new Color((byte)(outR * 255.0f), (byte)(outG * 255.0f), (byte)(outB * 255.0f), (byte)(outA * 255.0f));
                        hdData[toIndex] = outColor;
                    }
                }
            }
            if (this.UniqueID != 0)
            {
                hdData[0] = Encode(this.UniqueID);
            }
            HDTexture.SetData(hdData);
        }

        public bool CheckUniqueID(Color[] data)
        {
            if (data.Length < 1) return false;
            return Decode(data[0]).Equals(this.UniqueID);
        }

        private static Color Encode(int uniqueId)
        {
            return new Color((uniqueId >> 16) & 0xff, (uniqueId >> 8) & 0xff, (uniqueId >> 0) & 0xff, 0);
        }

        private static int Decode(Color color)
        {
            return ((color.R << 16) | (color.G << 8) | (color.B << 0));
        }
    }
}
