namespace Omegasis.MoreRain.Framework
{
    /// <summary>The mod configuration.</summary>
    internal class ModConfig
    {
        /// <summary>The chance out of 100 that it will rain tomorrow if it's spring.</summary>
        public int SpringRainChance { get; set; } = 15;

        /// <summary>The chance out of 100 that it will storm tomorrow if it's spring.</summary>
        public int SpringThunderChance { get; set; } = 5;

        /// <summary>The chance out of 100 that it will rain tomorrow if it's summer.</summary>
        public int SummerRainChance { get; set; } = 5;

        /// <summary>The chance out of 100 that it will storm tomorrow if it's summer.</summary>
        public int SummerThunderChance { get; set; } = 10;

        /// <summary>The chance out of 100 that it will rain tomorrow if it's fall.</summary>
        public int FallRainChance { get; set; } = 15;

        /// <summary>The chance out of 100 that it will storm tomorrow if it's fall.</summary>
        public int FallThunderChance { get; set; } = 5;

        /// <summary>The chance out of 100 that it will snow tomorrow if it's winter.</summary>
        public int WinterSnowChance { get; set; } = 15;

        /// <summary>Whether to suppress verbose logging.</summary>
        public bool SuppressLog { get; set; } = true;
    }
}
