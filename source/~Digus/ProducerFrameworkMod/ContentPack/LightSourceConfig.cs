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
        public ColorType ColorType = ColorType.DefinedColor;
        public int ColorRed = 255;
        public int ColorGreen = 255;
        public int ColorBlue = 255;
        public int ColorAlpha = 255;
        public float ColorFactor = 1;
        public bool AlwaysOn = false;

        internal Color Color = Color.White;
    }
}
