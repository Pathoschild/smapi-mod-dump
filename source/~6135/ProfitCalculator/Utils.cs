/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/6135/StardewValley.ProfitCalculator
**
*************************************************/

using JsonAssets;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.IO;

#nullable enable

namespace ProfitCalculator
{
    /// <summary>
    /// Provides a set of tools to be used by multiple classes of the mod.
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// The mod's helper. Declared here to prevend the need to pass it to every class that needs it.
        /// </summary>
        public static IModHelper? Helper { get; set; }

        /// <summary>
        /// The mod's monitor. Declared here to prevend the need to pass it to every class that needs it.
        /// </summary>
        public static IMonitor? Monitor { get; set; }

        /// <summary>
        /// The Json Assets API. Declared here to prevend the need to pass it to every class that needs it.
        /// </summary>
        public static IApi? JApi { get; set; }

        /// <summary>
        /// Sets the mod's helper, monitor, and APIs static variables. This method should be called by the mod's entry point.
        /// </summary>
        /// <param name="_helper"> The mod's helper.</param>
        /// <param name="_monitor"> The mod's monitor.</param>
        /// <param name="jApi"> The Json Assets API.</param>
        public static void Initialize(IModHelper _helper, IMonitor _monitor, IApi? jApi = null)
        {
            Helper = _helper;
            Monitor = _monitor;
            JApi = jApi;
        }

        /// <summary>
        /// Gets the days of a season. Unused.
        /// </summary>
        /// <param name="season"> The season to get the days of.</param>
        /// <returns> The number of days in the season.</returns>
        public static int GetSeasonDays(Season season)
        {
            return season switch
            {
                Season.Spring => 28,
                Season.Summer => 28,
                Season.Fall => 28,
                Season.Winter => 28,
                Season.Greenhouse => 112,
                _ => 0,
            };
        }

        /// <summary>
        /// Season enum.
        /// </summary>
        public enum Season
        {
            /// <summary> Spring season. </summary>
            Spring = 0,

            /// <summary> Summer season. </summary>
            Summer = 1,

            /// <summary> Fall season. </summary>
            Fall = 2,

            /// <summary> Winter season. </summary>
            Winter = 3,

            /// <summary> Greenhouse season. </summary>
            Greenhouse = 4
        }

        /// <summary>
        /// Produce type enum.
        /// </summary>
        public enum ProduceType
        {
            /// <summary> Crops. </summary>
            Raw,

            /// <summary> Artisan goods. </summary>
            Keg,

            /// <summary> Artisan goods. </summary>
            Cask
        }

        /// <summary>
        /// Fertilizer quality enum.
        /// </summary>
        public enum FertilizerQuality
        {
            /// <summary> No fertilizer. </summary>
            None = 0,

            /// <summary> Basic fertilizer. </summary>
            Basic = 1,

            /// <summary> Quality fertilizer. </summary>
            Quality = 2,

            /// <summary> Deluxe fertilizer. </summary>
            Deluxe = 3,

            /// <summary> Speed-Gro fertilizer. </summary>
            SpeedGro = -1,

            /// <summary> Deluxe Speed-Gro fertilizer. </summary>
            DeluxeSpeedGro = -2,

            /// <summary> Hyper Speed-Gro fertilizer. </summary>
            HyperSpeedGro = -3
        }

        //get season translated names
        private static string GetTranslatedName(string str)
        {
            //convert string to lowercase
            str = str.ToLower();
            return Helper?.Translation.Get(str) ?? "Error";
        }

        /// <summary>
        /// Get translated season name.
        /// </summary>
        /// <param name="season"> The season to get the translated name of.</param>
        /// <returns> The translated name of the season.</returns>
        public static string GetTranslatedSeason(Season season)
        {
            return GetTranslatedName(season.ToString());
        }

        /// <summary>
        /// Get translated produce type name.
        /// </summary>
        /// <param name="produceType"> The produce type to get the translated name of.</param>
        /// <returns> The translated name of the produce type.</returns>
        public static string GetTranslatedProduceType(ProduceType produceType)
        {
            return GetTranslatedName(produceType.ToString());
        }

        /// <summary>
        /// Get translated fertilizer quality name.
        /// </summary>
        /// <param name="fertilizerQuality"> The fertilizer quality to get the translated name of.</param>
        /// <returns> The translated name of the fertilizer quality.</returns>
        public static string GetTranslatedFertilizerQuality(FertilizerQuality fertilizerQuality)
        {
            return GetTranslatedName(fertilizerQuality.ToString());
        }

        /// <summary>
        /// Get translated season name. All seasons.
        /// </summary>
        /// <returns> Array of all translated season names.</returns>
        public static string[] GetAllTranslatedSeasons()
        {
            string[] names = Enum.GetNames(typeof(Season));
            string[] translatedNames = new string[names.Length];
            foreach (string name in names)
            {
                translatedNames[Array.IndexOf(names, name)] = GetTranslatedName(name);
            }
            return translatedNames;
        }

        /// <summary>
        /// Get all translated produce type names.
        /// </summary>
        /// <returns> Array of all translated produce type names.</returns>
        public static string[] GetAllTranslatedProduceTypes()
        {
            string[] names = Enum.GetNames(typeof(ProduceType));
            string[] translatedNames = new string[names.Length];
            foreach (string name in names)
            {
                translatedNames[Array.IndexOf(names, name)] = GetTranslatedName(name);
            }
            return translatedNames;
        }

        /// <summary>
        /// Get all translated fertilizer quality names.
        /// </summary>
        /// <returns> Array of all translated fertilizer quality names.</returns>
        public static string[] GetAllTranslatedFertilizerQualities()
        {
            string[] names = Enum.GetNames(typeof(FertilizerQuality));
            string[] translatedNames = new string[names.Length];
            foreach (string name in names)
            {
                translatedNames[Array.IndexOf(names, name)] = GetTranslatedName(name);
            }
            return translatedNames;
        }

        /// <summary>
        /// Get prices of each fertilizer quality.
        /// </summary>
        /// <param name="fq"> The fertilizer quality to get the price of.</param>
        /// <returns> The price of the fertilizer quality.</returns>
        public static int FertilizerPrices(FertilizerQuality fq)
        {
            return fq switch
            {
                FertilizerQuality.None => 0,
                FertilizerQuality.Basic => 100,
                FertilizerQuality.Quality => 150,
                FertilizerQuality.Deluxe => 200,
                FertilizerQuality.SpeedGro => 100,
                FertilizerQuality.DeluxeSpeedGro => 150,
                FertilizerQuality.HyperSpeedGro => 200,
                _ => 0,
            };
        }
    }
}