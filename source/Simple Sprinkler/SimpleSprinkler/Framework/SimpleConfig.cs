/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ADoby/SimpleSprinkler
**
*************************************************/

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
