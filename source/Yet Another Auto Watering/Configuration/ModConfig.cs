/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ZhuoYun233/YetAnotherAutoWatering-StardrewValleyMod
**
*************************************************/

using StardewModdingAPI;

namespace YetAnotherAutoWatering.Configuration
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
        /// OLD VERSION: Sets the button to reload this config file, default F5
        /// </summary>
        //public SButton ConfigReloadKey { get; set; } = SButton.F5;

        /// <summary>
        /// Sets the button to auto water once manually, default F6
        /// </summary>
        public SButton WaterNowKey { get; set; } = SButton.F6;

        /// <summary>
        /// Auto fertilizing type, default disabled.
        /// Corresponding ID and Possible type values:
        ///     null: Disabled
        ///     0:   Remove all fertilizer
        ///     368: Basic Fertilizer
        ///     369: Quality Fertilizer
        ///     370: Basic Retaining Soil
        ///     371: Quality Retaining Soil
        ///     465: Speed-Gro
        ///     466: Deluxe Speed-Gro
        ///     918: Hyper Speed-Gro
        ///     919: Deluxe Fertilizer
        ///     920: Deluxe Retaining Soil
        /// </summary>
        public string FertilizerType { get; set; } = "Disabled";
        /// <summary>
        /// Sets what days of the week to auto water, default every day
        /// </summary>
        public DaysToWater DaysToWater { get; set; } = new DaysToWater();
    }
}
