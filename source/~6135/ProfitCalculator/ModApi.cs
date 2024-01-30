/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/6135/StardewValley.ProfitCalculator
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using static ProfitCalculator.Utils;
using Crop = ProfitCalculator.main.Crop;

#nullable enable

namespace ProfitCalculator
{
    /// <summary>
    /// API for the Profit Calculator mod.
    /// </summary>
    public interface IProfitCalculatorApi
    {
        /// <summary>
        /// Adds a crop to the Profit Calculator.
        /// </summary>
        /// <param name="id"> The id of the crop. Must be unique.</param>
        /// <param name="item"> The item to use for the crop. <see cref="Item"/> </param>
        /// <param name="name"> The name of the crop.</param>
        /// <param name="sprite"> The sprite of the crop. Tuple with source ret</param>
        /// <param name="isTrellisCrop"> Whether the crop is a trellis crop.</param>
        /// <param name="isGiantCrop"> Whether the crop is a giant crop.</param>
        /// <param name="giantSprite"> The sprite of the giant crop. Tuple with source ret</param>
        /// <param name="seeds"> The seeds of the crop. <see cref="Item"/> </param>
        /// <param name="phases"> The phases of the crop.</param>
        /// <param name="regrow"> The regrow of the crop.</param>
        /// <param name="isPaddyCrop"> Whether the crop is a paddy crop.</param>
        /// <param name="seasons"> The seasons of the crop.</param>
        /// <param name="harvestChanceValues"> The harvest chance values of the crop.</param>
        /// <param name="affectByQuality"> Whether the crop is affected by quality.</param>
        /// <param name="affectByFertilizer"> Whether the crop is affected by fertilizer.</param>
        /// <param name="seedPrice"> The price of the seeds.</param>
        void AddCrop
            (
                string id,
                Item item,
                string name,
                Tuple<Texture2D, Rectangle>? sprite,
                bool isTrellisCrop,
                bool isGiantCrop,
                Tuple<Texture2D, Rectangle>? giantSprite,
                Item[] seeds,
                int[] phases,
                int regrow,
                bool isPaddyCrop,
                string[] seasons,
                double[] harvestChanceValues,
                bool affectByQuality = true,
                bool affectByFertilizer = true,
                int? seedPrice = null
            );
    }

    /// <summary>
    /// API for the Profit Calculator mod.
    /// </summary>
    public class ModApi : IProfitCalculatorApi
    {
        private readonly ModEntry Mod;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModApi"/> class.
        /// </summary>
        /// <param name="mod"></param>
        public ModApi(ModEntry mod)
        {
            this.Mod = mod;
        }

        /// <inheritdoc/>
        public void AddCrop(
                string id,
                Item item,
                string name,
                Tuple<Texture2D, Rectangle>? sprite,
                bool isTrellisCrop,
                bool isGiantCrop,
                Tuple<Texture2D, Rectangle>? giantSprite,
                Item[] seeds,
                int[] phases,
                int regrow,
                bool isPaddyCrop,
                string[] seasons,
                double[] harvestChanceValues,
                bool affectByQuality = true,
                bool affectByFertilizer = true,
                int? seedPrice = null
            )
        {
            //convert string[] to Season[]
            Season[] seasonsEnum = new Season[seasons.Length];
            for (int i = 0; i < seasons.Length; i++)
            {
                seasonsEnum[i] = (Season)Enum.Parse(typeof(Season), seasons[i], true);
            }
            Crop crop = new(id, item, name, sprite, isTrellisCrop, isGiantCrop, giantSprite, seeds, phases, regrow, isPaddyCrop, seasonsEnum, harvestChanceValues, affectByQuality, affectByFertilizer, seedPrice);
            Mod.AddCrop(id, crop);
        }
    }
}