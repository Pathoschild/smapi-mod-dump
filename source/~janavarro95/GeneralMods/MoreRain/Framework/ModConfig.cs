/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

namespace Omegasis.MoreRain.Framework
{
    /// <summary>The mod configuration.</summary>
    internal class ModConfig
    {
        /// <summary>The chance out of 100 that it will rain tomorrow if it's spring.</summary>
        public int SpringRainChance { get; set; } = 15;

        /// <summary>The chance out of 100 that it will storm tomorrow if it's spring.</summary>
        public int SpringThunderChance { get; set; } = 5;

        /// <summary>
        /// Changes the mod's logic to prioritize setting a thunderstorm before checking for just a normal rainy day.
        /// False = The mod will try to set a normal rainy day first.
        /// True = The mod will try to set a thunderstorm (stormy) day first.
        /// Default:False
        /// </summary>
        public bool PrioritizeSpringStorms { get; set; } = false;

        /// <summary>The chance out of 100 that it will rain tomorrow if it's summer.</summary>
        public int SummerRainChance { get; set; } = 5;

        /// <summary>The chance out of 100 that it will storm tomorrow if it's summer.</summary>
        public int SummerThunderChance { get; set; } = 10;

        /// <summary>
        /// Changes the mod's logic to prioritize setting a thunderstorm before checking for just a normal rainy day.
        /// False = The mod will try to set a normal rainy day first.
        /// True = The mod will try to set a thunderstorm (stormy) day first.
        /// Default:True
        /// </summary>
        public bool PrioritizeSummerStorms { get; set; } = true;

        /// <summary>The chance out of 100 that it will rain tomorrow if it's fall.</summary>
        public int FallRainChance { get; set; } = 15;

        /// <summary>The chance out of 100 that it will storm tomorrow if it's fall.</summary>
        public int FallThunderChance { get; set; } = 5;


        /// <summary>
        /// Changes the mod's logic to prioritize setting a thunderstorm before checking for just a normal rainy day.
        /// False = The mod will try to set a normal rainy day first.
        /// True = The mod will try to set a thunderstorm (stormy) day first.
        /// Default:False
        /// </summary>
        public bool PrioritizeFallStorms { get; set; } = false;

        /// <summary>
        /// If set to true the mod will try to make it snow in fall just for fun.
        /// </summary>
        public bool SnowInFall { get; set; } = false;

        /// <summary>
        /// The chance amouunt for it to snow in the fall.
        /// </summary>
        public int FallSnowChance { get; set; } = 5;

        /// <summary>The chance out of 100 that it will snow tomorrow if it's winter.</summary>
        public int WinterSnowChance { get; set; } = 15;

        /// <summary>
        /// If set to true then the mod will check to set rainy days in the winter. Default: False
        /// </summary>
        public bool RainInWinter { get; set; } = false;
        /// <summary>
        /// The chance that it will rain on a winter day. Only checked if the RainInWinter config option is true.
        /// </summary>
        public int WinterRainChance { get; set; } = 10;

        /// <summary>Whether to suppress verbose logging.</summary>
        public bool SuppressLog { get; set; } = true;
    }
}
