/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mus-candidus/CellarAvailable
**
*************************************************/


namespace CellarAvailable.Framework {
    internal class ConfigEntry {
        /// <summary>Indicates whether to show the community upgrade in the carpenter's menu.</summary>
        public bool ShowCommunityUpgrade { get; set; } = false;
        /// <summary>Indicates whether to remove casks.</summary>
        public bool RemoveCasks { get; set; } = false;
    }
}
