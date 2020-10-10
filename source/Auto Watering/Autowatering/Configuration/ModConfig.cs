/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stellarashes/SDVMods
**
*************************************************/

using StardewModdingAPI;

namespace Autowatering.Configuration
{
    /// <summary>
    /// Config file for Autowatering
    /// </summary>
    public class ModConfig
    {
        /// <summary>
        /// Sets whether this mod is enabled, default true
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Sets the button to reload this config file, default f5
        /// </summary>
        public SButton ConfigReloadKey { get; set; } = SButton.F5;

        /// <summary>
        /// Optional integer that represents the fertilizer to auto apply to all, default null
        /// Possible values:
        ///     null: disable auto changing the fertilizer
        ///     0: always remove all fertilizer
        ///     368: low quality fertilizer
        ///     369: high quality fertilizer
        ///     370: water retention soil
        ///     371: quality water retention soil
        ///     465: speed gro
        ///     466: super speed gro
        /// </summary>
        public int? Fertilizer { get; set; } = null;

        /// <summary>
        /// Sets what days of the week to auto water, default every day
        /// </summary>
        public DaysToWater DaysToWater { get; set; } = new DaysToWater();
    }
}
