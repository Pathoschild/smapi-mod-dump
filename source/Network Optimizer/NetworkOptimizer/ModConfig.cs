/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/NetworkOptimizer
**
*************************************************/

namespace NetworkOptimizer
{
    class ModConfig
    {
        public int DefaultInterpolationTicks { get; set; } = 15;

        public int FarmerDeltaBroadcastPeriod { get; set; } = 3;

        public int LocationDeltaBroadcastPeriod { get; set; } = 3;

        public int WorldStateDeltaBroadcastPeriod { get; set; } = 3;
    }
}
