using StardewModdingAPI;

namespace Omegasis.BuyBackCollectables.Framework
{
    /// <summary>The mod configuration.</summary>
    internal class ModConfig
    {
        /// <summary>The key which shows the menu.</summary>
        public SButton KeyBinding { get; set; } = SButton.B;

        /// <summary>The multiplier applied to the cost of buying back a collectable.</summary>
        public double CostMultiplier { get; set; } = 3.0;
    }
}
