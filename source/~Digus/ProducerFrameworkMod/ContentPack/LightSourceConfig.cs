using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ProducerFrameworkMod.ContentPack
{
    public class LightSourceConfig
    {
        public int TextureIndex = 4;
        public float Radius = 1.5f;
        public float OffsetX = 0;
        public float OffsetY = 0;
        public int ColorRed = 0;
        public int ColorGreen = 0;
        public int ColorBlue = 0;
        public int ColorAlpha = 255;
        public float ColorFactor = 1;
        public bool AlwaysOn = false;

        internal Color Color = Color.White;

        public LightSourceConfig()
        {
        }

        public LightSourceConfig(int textureIndex, float radius, Color color, float colorFactor, float offsetX, float offsetY, int colorRed, int colorGreen, int colorBlue, int colorAlpha)
        {
            TextureIndex = textureIndex;
            Radius = radius;
            Color = color;
            ColorFactor = colorFactor;
            OffsetX = offsetX;
            OffsetY = offsetY;
            ColorRed = colorRed;
            ColorGreen = colorGreen;
            ColorBlue = colorBlue;
            ColorAlpha = colorAlpha;
        }
    }
}
