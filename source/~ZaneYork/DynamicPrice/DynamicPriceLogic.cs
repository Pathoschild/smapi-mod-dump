using StardewModdingAPI.Utilities;
using System;
using SObject = StardewValley.Object;

namespace DynamicPrice
{
    public class DynamicPriceLogic
    {
        internal static int calc(SObject item, int price)
        {
            if(ModEntry.ModConfig.DynamicWithDecay && ModEntry.ccdApi != null)
            {
                int decayDays = ModEntry.ccdApi.getCropDecayDays(item);
                SDate harvestDate = ModEntry.ccdApi.getCropHarvestDate(item);
                if(harvestDate != null)
                {
                    float decayPercent = 1 - (SDate.Now().DaysSinceStart - harvestDate.DaysSinceStart) * 1.0f / decayDays;
                    if (decayPercent < 0)
                        decayPercent = 0;
                    if (decayPercent <= 1.0)
                    {
                        // y = 0.8135x2 - 1.8008x + 0.9848 ( 0 <= x <= 1)
                        float discount = 0.8135f * decayPercent * decayPercent - 1.8008f * decayPercent + 0.9848f;
                        discount = (ModEntry.ModConfig.Discount.Max - ModEntry.ModConfig.Discount.Min) * discount + ModEntry.ModConfig.Discount.Min;
                        price = (int)(price * (1 - discount));
                    }
                }
            }
            return (int)(price * getFloatingPriceRatio(item));
        }
        internal static float getFloatingPriceRatio(SObject item)
        {
            if (!ModEntry.ModConfig.DynamicWithDate)
                return 1;
            int nowDays = SDate.Now().DaysSinceStart;
            int nowWeeks = nowDays / 7;
            int nowDayOfWeek = nowDays % 7 + 1;
            Random random = new Random(ModEntry.ModConfig.Seed + nowWeeks + item.ParentSheetIndex);
            Random random2 = new Random(ModEntry.ModConfig.Seed + nowDays + item.ParentSheetIndex);
            double[] factorsMax;
            double[] factorsMin;
            switch (random.Next(4))
            {
                case 0:
                    factorsMax = new double[7] { 0.0047, -0.1065, +0.9434, -4.0844, +8.9769, -9.4342, +5.15 };
                    factorsMin = new double[7] { 0.0035, -0.0821, +0.7493, -3.3229, +7.3972, -7.645, +3.5 };
                    break;
                case 1:
                    factorsMax = new double[7] { 0.0005, -0.0181, +0.2226, -1.226, +3.3019, -4.2808, +3.45 };
                    factorsMin = new double[7] { 0.0056, -0.1342, +1.2597, -5.8042, +13.635, -15.162, +6.7 };
                    break;
                case 2:
                    factorsMax = new double[7] { 0.0201, -0.446, +3.8454, -16.37, +36.209, -39.269, +16.92 };
                    factorsMin = new double[7] { 0.0091, -0.2031, +1.7712, -7.701, +17.645, -20.121, +9.25 };
                    break;
                default:
                    factorsMax = new double[7] { 0.0015, -0.0387, +0.3819, -1.869, +4.6916, -5.6773, +3.42 };
                    factorsMin = new double[7] { 0.001, -0.0252, +0.2385, -1.1115, +2.6354, -3.0883, +2.2 };
                    break;
            }
            double yMax = 0, yMin = 0;
            for (int i = 0; i < factorsMax.Length; i++)
            {
                yMax += factorsMax[i] * Math.Pow(nowDayOfWeek, factorsMax.Length - i - 1);
            }
            for (int i = 0; i < factorsMin.Length; i++)
            {
                yMin += factorsMin[i] * Math.Pow(nowDayOfWeek, factorsMin.Length - i - 1);
            }
            double y = random2.NextDouble() * Math.Abs(yMax - yMin) + yMin;
            return (float)((y - 1) * ModEntry.ModConfig.ChangeRateMultiplier) + 1;
        }
    }
}
