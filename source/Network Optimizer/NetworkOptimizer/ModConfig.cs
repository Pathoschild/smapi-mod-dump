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
