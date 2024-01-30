/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/6135/StardewValley.ProfitCalculator
**
*************************************************/

using StardewModdingAPI;

namespace ProfitCalculator
{
    /// <summary>
    /// The mod config.
    /// </summary>
    public class ModConfig
    {
        /// <summary> The hotkey to open the calculator. </summary>
        public SButton HotKey { get; set; }

        /// <summary> The delay in frames before the tooltip is shown. </summary>
        public int ToolTipDelay { get; set; }

        /// <summary>
        ///  Creates a new mod config with default values.
        /// </summary>
        public ModConfig()
        {
            HotKey = SButton.F8;
            ToolTipDelay = 30;
        }
    }
}