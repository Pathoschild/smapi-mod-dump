/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/RushOrders
**
*************************************************/

namespace RushOrders
{
    public class RushOrdersConfig
    {
        public class PriceFactor_
        {
            public class Tool_
            {
                public double Rush { get; set; } = 1.5;
                public double Now { get; set; } = 2.0;
            }
            public class Building_
            {
                public double RushOneDay { get; set; } = 0.5;
                //public double Now { get; set; } = 3.0;
            }

            public Tool_ Tool { get; set; } = new Tool_();
            public Building_ Building { get; set; } = new Building_();
        }

        public PriceFactor_ PriceFactor { get; set; } = new PriceFactor_();
    }
}
