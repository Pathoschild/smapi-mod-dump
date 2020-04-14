using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace CustomToolEffect
{
    public class ModConfig
    {
        public Dictionary<int, PowerDefine> AxeDefine { get; set; } = new Dictionary<int, PowerDefine> {
            {0, new PowerDefine(1.4f) },
            {1, new PowerDefine(1.4f) },
            {2, new PowerDefine(1.4f) },
            {3, new PowerDefine(1.4f) },
            {4, new PowerDefine(1.4f) }
        };
        public Dictionary<int, PowerDefine> PickaxeDefine { get; set; } = new Dictionary<int, PowerDefine>
        {
            {0, new PowerDefine(1.4f) },
            {1, new PowerDefine(1.4f) },
            {2, new PowerDefine(1.4f) },
            {3, new PowerDefine(1.4f) },
            {4, new PowerDefine(1.8f) }
        };
        public Dictionary<int, RangeDefine> HoeDefine { get; set; } = new Dictionary<int, RangeDefine>
        {
            {0, new RangeDefine(new Range(1, 1)) },
            {1, new RangeDefine(new Range(1, 3)) },
            {2, new RangeDefine(new Range(1, 5)) },
            {3, new RangeDefine(new Range(3, 3)) },
            {4, new RangeDefine(new Range(5, 5)) }
        };
        public Dictionary<int, RangeDefine> WateringCanDefine { get; set; } = new Dictionary<int, RangeDefine>
        {
            {0, new RangeDefine(new Range(1, 1)) },
            {1, new RangeDefine(new Range(1, 3)) },
            {2, new RangeDefine(new Range(1, 5)) },
            {3, new RangeDefine(new Range(3, 3)) },
            {4, new RangeDefine(new Range(5, 5)) }
        };
        public Dictionary<int, PowerDefine> BombDefine { get; set; } = new Dictionary<int, PowerDefine>
        {
            {286, new PowerDefine(1.4f) },
            {287, new PowerDefine(1.4f) },
            {288, new PowerDefine(1.5f) },
        };
        public class PowerDefine
        {
            public PowerDefine(float power)
            {
                Power = power;
            }
            public float Power { get; set; }
        }

        public class RangeDefine
        {
            public RangeDefine(Range range)
            {
                Range = range;
            }
            public Range Range { get; set; }
        }
        public class Range
        {
            public Range(int width, int length)
            {
                Width = width;
                Length = length;
            }

            public int Length { get; set; }
            public int Width { get; set; }
        }
    }
}
