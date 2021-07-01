/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

namespace RushOrders.Framework
{
    internal class RushOrdersConfig
    {
        public class PriceFactorModel
        {
            public class ToolModel
            {
                public double Rush { get; set; } = 1.5;
                public double Now { get; set; } = 2.0;
            }
            public class BuildingModel
            {
                public double RushOneDay { get; set; } = 0.5;
                //public double Now { get; set; } = 3.0;
            }

            public ToolModel Tool { get; set; } = new();
            public BuildingModel Building { get; set; } = new();
        }

        public PriceFactorModel PriceFactor { get; set; } = new();
    }
}
