using System;
using System.Collections.Generic;

namespace DynamicNPCSprites
{
    public class ConditionalTexture
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string[] Weather { get; set; }
        public string[] Season { get; set; }
        public string Sprite { get; set; }
        public string Portrait { get; set; }
    }
}
