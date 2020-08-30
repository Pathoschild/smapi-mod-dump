namespace GoToBed.Framework {
    internal class ModConfig {
        /// <summary>
        /// Gets or sets a value indicating whether to provide StardewValley13 spouse sleeping behavior.
        /// </summary>
        public bool Stardew13SpouseSleep { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the time when your spouse gets up.
        /// </summary>
        public int SpouseGetUpTime { get; set; } = SpouseBedTimeVerifier.DefaultGetUpTime;

        // <summary>
        /// Gets or sets a value indicating the time when your goes to bed.
        /// </summary>
        public int SpouseGoToBedTime { get; set; } = SpouseBedTimeVerifier.DefaultGoToBedTime;
    }
}
