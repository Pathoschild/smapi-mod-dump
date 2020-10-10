/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mus-candidus/TrendyHaley
**
*************************************************/

using System.Collections.Generic;


namespace TrendyHaley.Framework {
    internal class ModConfig {
        /// <summary>User configuration dictionary.</summary>
        public IDictionary<string, ConfigEntry> SaveGame { get; set; }
            = new Dictionary<string, ConfigEntry>();
    }
}
