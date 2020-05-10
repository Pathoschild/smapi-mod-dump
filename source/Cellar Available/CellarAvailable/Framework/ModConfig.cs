using System.Collections.Generic;


namespace CellarAvailable.Framework {
    internal class ModConfig {
        /// <summary>User configuration dictionary.</summary>
        public IDictionary<string, ConfigEntry> SaveGame { get; set; }
            = new Dictionary<string, ConfigEntry>();
    }
}
