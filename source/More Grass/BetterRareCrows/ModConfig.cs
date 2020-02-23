namespace BetterRarecrows
{
    /// <summary>The mod configuration.</summary>
    class ModConfig
    {
        /// <summary>The number of distinct rarecrows the player needs to have placed on their farm.</summary>
        public int NumberOfRequiredRareCrows { get; set; } = 8;

        /// <summary>Whether each placed rarecrow should slightly lower the chances of crops from spawning.</summary>
        public bool EnableProgressiveMode { get; set; } = true;

        /// <summary>The chance that each rarecrow has at stopping crows coming, with progressive mode (At a chance of 10, 5 rarecrows would sheild the farm 50% or time etc).</summary>
        public int ProgressivePercentPerRarecrow { get; set; } = 10;
    }
}
