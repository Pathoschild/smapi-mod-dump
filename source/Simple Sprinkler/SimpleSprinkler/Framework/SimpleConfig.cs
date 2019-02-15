using System.Collections.Generic;

namespace SimpleSprinkler.Framework
{
    internal class SimpleConfig
    {
        public CalculationMethods CalculationMethod { get; set; } = CalculationMethods.CIRCLE;
        public string[] Locations { get; set; } = { "Farm", "Greenhouse" };
        public IDictionary<int, float> Radius { get; set; } = new Dictionary<int, float>
        {
            [599] = 2,
            [621] = 3,
            [645] = 5
        };
    }
}
