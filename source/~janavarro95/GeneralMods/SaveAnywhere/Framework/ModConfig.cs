using StardewModdingAPI;

namespace Omegasis.SaveAnywhere.Framework
{
    /// <summary>The mod configuration.</summary>
    internal class ModConfig
    {
        /// <summary>The key which initiates a save.</summary>
        public SButton SaveKey { get; set; } = SButton.K;
    }
}
