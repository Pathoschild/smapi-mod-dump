/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gzhynko/StardewMods
**
*************************************************/

namespace EventBlackBars
{
    /// <summary> The mod config class. More info here: https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Config </summary>
    public class ModConfig
    {
        /// <summary>
        /// The percentage of the height of the screen for a bar to take up.
        /// </summary>
        public double BarHeightPercentage { get; set; } = 10;
        
        /// <summary>
        /// Whether to gradually move the bars in when an event starts, or have them fully out right away.
        /// </summary>
        public bool MoveBarsInSmoothly { get; set; } = true;
    }
}
