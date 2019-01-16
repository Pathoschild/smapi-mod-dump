using StardewModdingAPI;

namespace Omegasis.MuseumRearranger.Framework
{
    /// <summary>The mod configuration.</summary>
    internal class ModConfig
    {
        /// <summary>The key which shows the museum rearranging menu.</summary>
        public SButton ShowMenuKey { get; set; } = SButton.R;

        /// <summary>The key which toggles the inventory box when the menu is open.</summary>
        public SButton ToggleInventoryKey { get; set; } = SButton.T;
    }
}
