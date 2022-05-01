/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

#nullable disable

namespace Pathoschild.Stardew.Automate.Framework.Models
{
    /// <summary>Mod compatibility options.</summary>
    internal class ModCompatibilityConfig
    {
        /// <summary>Enable compatibility with Better Junimos. If it's installed, Junimo huts won't output fertilizer or seeds.</summary>
        public bool BetterJunimos { get; set; } = true;

        /// <summary>Whether to log a warning if the player installs a custom-machine mod that requires a separate compatibility patch which isn't installed.</summary>
        public bool WarnForMissingBridgeMod { get; set; } = true;
    }
}
