
namespace CellarAvailable.Framework {
    internal class ConfigEntry {
        /// <summary>Indicates whether to show the community upgrade in the carpenter's menu.</summary>
        public bool ShowCommunityUpgrade { get; set; } = false;
        /// <summary>Indicates whether to remove casks.</summary>
        public bool RemoveCasks { get; set; } = false;
    }
}
