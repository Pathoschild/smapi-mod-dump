/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

namespace ContentPatcher.Framework.ConfigModels
{
    /// <summary>The mod configuration.</summary>
    internal class ModConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>Whether to enable debug features.</summary>
        public bool EnableDebugFeatures { get; set; }

        /// <summary>The key bindings.</summary>
        public ModConfigKeys Controls { get; set; } = new();
    }
}
