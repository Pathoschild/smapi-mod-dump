using System;

namespace DynamicPrice
{
    public class ModConfig
    {
        public bool DynamicWithDate { get; set; } = true;
        public bool DynamicWithDecay { get; set; } = true;
        public int Seed { get; set; } = new Random().Next();
        public float ChangeRateMultiplier { get; set; } = 0.5f;
        public DiscountRule Discount { get; set; } = new DiscountRule(0.0f, 0.5f);
        public class DiscountRule
        {
            public float Min { get; set; }
            public float Max { get; set; }

            public DiscountRule(float min, float max)
            {
                Min = min;
                Max = max;
            }
        }
    }
}
