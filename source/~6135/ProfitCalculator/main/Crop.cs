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
using System.Collections.Immutable;
using System.Linq;
using static ProfitCalculator.Utils;
using SObject = StardewValley.Object;

#nullable enable

namespace ProfitCalculator.main
{
    /// <summary>
    /// Class <c>Crop</c> models a crop from the game storing all relevant information about it.
    /// </summary>
    public class Crop
    {
        /// <value>Property <c>seedPrice</c> represents the seed buying price, used to calculate the seed purchasing loss and it's per day value.</value>
        private readonly int seedPrice;

        /// <value>Property <c>Id</c> represents the crop id AKA the slot in the sprite sheet.</value>
        public readonly string Id;

        /// <value>Property <c>Item</c> represents the item object of the crop's produce.</value>
        public readonly Item Item;

        /// <value>Property <c>Name</c> represents the crop's name.</value>
        public readonly string Name;

        /// <value>Property <c>Sprite</c> represents the crop's sprite. It's unused as of now.</value>
        public readonly Tuple<Texture2D, Rectangle>? Sprite;

        /// <value>Property <c>IsTrellisCrop</c> represents whether the crop is a trellis crop or not.</value>
        public readonly bool IsTrellisCrop;

        /// <value>Property <c>IsGiantCrop</c> represents whether the crop is a giant crop or not.</value>
        public readonly bool IsGiantCrop;

        /// <value>Property <c>GiantSprite</c> represents the giant crop's sprite. It's unused as of now.</value>
        public readonly Tuple<Texture2D, Rectangle>? GiantSprite;

        /// <value>Property <c>Seeds</c> represents the crop's seeds.</value>
        public readonly ImmutableArray<Item> Seeds;

        /// <value>Property <c>Phases</c> represents the crop's phases. It's unused as of now.</value>
        public readonly ImmutableArray<int> Phases;

        /// <value>Property <c>Regrow</c> represents the crop's regrow time.</value>
        public readonly int Regrow;

        /// <value>Property <c>IsPaddyCrop</c> represents whether the crop is a paddy crop or not.</value>
        public readonly bool IsPaddyCrop;

        /// <value>Property <c>Seasons</c> represents the crop's seasons.</value>
        public readonly ImmutableArray<Season> Seasons;

        /// <value>Property <c>Days</c> represents the crop's total days to grow excluding <see cref="Regrow"/>.</value>
        public readonly int Days;

        /// <value>Property <c>Price</c> represents the crop's selling price, not the same as <see cref="seedPrice"/></value>
        public readonly int Price;

        /// <value>Property <c>ChanceForExtraCrops</c> represents the crop's chance for extra crops.</value>
        public readonly double ChanceForExtraCrops;

        /// <value>Property <c>MaxHarvests</c> represents the crop's maximum drops.</value>
        public readonly int MaxHarvests;

        /// <value>Property <c>MinHarvests</c> represents the crop's minimum drops.</value>
        public readonly int MinHarvests;

        /// <value>Property <c>MaxHarvestIncreasePerFarmingLevel</c> represents the crop's maximum drops increase per farming level.</value>
        public readonly int MaxHarvestIncreasePerFarmingLevel;

        /// <value>Property <c>affectByQuality</c> represents whether the crop is affected by fertilizer quality or not. Some crops like Tea aren't affected by this. </value>
        public readonly bool affectByQuality;

        /// <value>Property <c>affectByFertilizer</c> represents whether the crop is affected by fertilizer or not.</value>
        public readonly bool affectByFertilizer;

        /// <summary>
        /// Constructor for <c>Crop</c> class. It's used to create a new instance of the class.
        /// </summary>
        /// <param name="id">ID of the crop's drop. AKA the slot in the sprite sheet.</param>
        /// <param name="item"> Item object of the crop's produce.</param>
        /// <param name="name"> Name of the crop.</param>
        /// <param name="sprite"> Sprite of the crop. Unused as of now.</param>
        /// <param name="isTrellisCrop"> Whether the crop is a trellis crop or not.</param>
        /// <param name="isGiantCrop"> Whether the crop is a giant crop or not.</param>
        /// <param name="giantSprite"> Giant crop's sprite. Unused as of now.</param>
        /// <param name="seeds"> Crop's seeds. Passed as array but converted to Immutable Array</param>
        /// <param name="phases"> Crop's phases. Passed as array but converted to Immutable Array</param>
        /// <param name="regrow"> Crop's regrow time.</param>
        /// <param name="isPaddyCrop"> Whether the crop is a paddy crop or not.</param>
        /// <param name="seasons"> Crop's seasons. Passed as array but converted to Immutable Array</param>
        /// <param name="harvestChanceValues"> Crop's harvest chance values. Passed as array of length 4 and converted to individual variables for ease of access.</param>
        /// <param name="affectByQuality"> Whether the crop is affected by fertilizer quality or not.</param>
        /// <param name="affectByFertilizer"> Whether the crop is affected by fertilizer or not.</param>
        /// <param name="seedPrice"> Crop's seed buying price. If null, then it's calculated from the first seed in the seeds array. This is made for the override.</param>
        public Crop(string id, Item item, string name, Tuple<Texture2D, Rectangle>? sprite, bool isTrellisCrop, bool isGiantCrop, Tuple<Texture2D, Rectangle>? giantSprite, Item[] seeds, int[] phases, int regrow, bool isPaddyCrop, Season[] seasons, double[] harvestChanceValues, bool affectByQuality = true, bool affectByFertilizer = true, int? seedPrice = null)
        {
            Id = id;
            Item = item;
            Name = name;
            Sprite = sprite;
            IsTrellisCrop = isTrellisCrop;
            IsGiantCrop = isGiantCrop;
            GiantSprite = giantSprite;
            Seeds = ImmutableArray.Create(seeds);
            Phases = ImmutableArray.Create(phases);
            Regrow = regrow;
            IsPaddyCrop = isPaddyCrop;
            Days = Phases.Sum();
            Price = ((SObject)Item).Price;
            Seasons = ImmutableArray.Create(seasons);
            MaxHarvests = (int)harvestChanceValues[0];
            MinHarvests = (int)harvestChanceValues[1];
            MaxHarvestIncreasePerFarmingLevel = (int)harvestChanceValues[2];
            ChanceForExtraCrops = harvestChanceValues[3];
            this.affectByQuality = affectByQuality;
            this.affectByFertilizer = affectByFertilizer;
            if (seedPrice != null)
            {
                this.seedPrice = (int)seedPrice;
            }
            else if (seeds != null)
            {
                int price = seeds[0].salePrice();
                this.seedPrice = price;
            }
            else this.seedPrice = 0;
        }

        /// <summary>
        /// Retrieves the seed buying price of the crop. Used to calculate the seed purchasing loss and it's per day value. If the seed price is not set, then it's calculated from the first seed in the seeds array. This is made for the override.
        /// </summary>
        /// <returns> Seed buying price of the crop. <c>int</c></returns>
        public int GetSeedPrice()
        {
            return seedPrice;
        }

        /// <summary>
        /// Returns whether two crops are equal or not. Two crops are equal if they have the same ID.
        /// </summary>
        /// <returns> Whether two crops are equal or not.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is Crop crop)
            {
                return crop.Id == Id;
            }
            else return false;
        }

        /// <summary>
        /// Returns a string representation of the crop.
        /// </summary>
        /// <returns> String representation of the crop.</returns>
        public override string? ToString()
        {
            return $"{Name} ({Id})"
                + $"\n\tSprite: {Sprite} "
                + $"\tIsTrellisCrop: {IsTrellisCrop}"
                + $"\n\tIsGiantCrop: {IsGiantCrop} "
                + $"\tGiantSprite: {GiantSprite}"
                + $"\n\tSeeds: {Seeds} "
                + $"\tPhases: {Phases}"
                + $"\tRegrow: {Regrow}"
                + $"\n\tIsPaddyCrop: {IsPaddyCrop} "
                + $"\tDays: {Days} "
                + $"\n\tPrice: {Price}";
        }

        /// <summary>
        /// Returns the hash code of the crop.
        /// </summary>
        /// <returns> Hash code of the crop. <c>int</c></returns>
        public override int GetHashCode()
        {
            //using FNV-1a hash
            unchecked
            {
                const int p = 16777619;
                int hash = (int)2166136261;

                hash = (hash ^ Id.GetHashCode()) * p;
                hash = (hash ^ Name.GetHashCode()) * p;
                hash = (hash ^ IsTrellisCrop.GetHashCode()) * p;
                hash = (hash ^ IsGiantCrop.GetHashCode()) * p;
                hash = (hash ^ Regrow) * p;
                hash = (hash ^ IsPaddyCrop.GetHashCode()) * p;
                hash = (hash ^ Days) * p;
                hash = (hash ^ Price) * p;
                hash = (hash ^ ChanceForExtraCrops.GetHashCode()) * p;
                hash = (hash ^ MaxHarvests) * p;
                hash = (hash ^ MinHarvests) * p;
                hash = (hash ^ MaxHarvestIncreasePerFarmingLevel) * p;

                return hash;
            }
        }

        #region Growth Values Calculations

        /// <summary>
        /// Calculates the average growth speed value for the crop.
        /// It's calculated by adding fertilizer modifiers to 1.0f and finally adding 0.25f if the crop is a paddy crop and 0.1f if the player has the agriculturist profession.
        /// </summary>
        /// <param name="fertilizerQuality"> Quality of the used Fertilizer</param>
        /// <returns> Average growth speed value for the crop. <c>float</c></returns>
        public float GetAverageGrowthSpeedValueForCrop(FertilizerQuality fertilizerQuality)
        {
            float speedIncreaseModifier = 0.0f;
            if (!affectByFertilizer)
            {
                speedIncreaseModifier = 1.0f;
            }
            else if ((int)fertilizerQuality == -1)
            {
                speedIncreaseModifier += 0.1f;
            }
            else if ((int)fertilizerQuality == -2)
            {
                speedIncreaseModifier += 0.25f;
            }
            else if ((int)fertilizerQuality == -3)
            {
                speedIncreaseModifier += 0.33f;
            }
            //if paddy crop then add 0.25f and if profession is agriculturist then add 0.1f
            if (IsPaddyCrop)
            {
                speedIncreaseModifier += 0.25f;
            }
            if (Game1.player.professions.Contains(Farmer.agriculturist))
            {
                speedIncreaseModifier += 0.1f;
            }
            return speedIncreaseModifier;
        }

        /// <summary>
        /// Checks whether the crop is available for the current season.
        /// </summary>
        /// <param name="currentSeason"></param>
        /// <returns> Whether the crop is available for the current season or not.</returns>
        public bool IsAvailableForCurrentSeason(Season currentSeason)
        {
            return Seasons.Contains(currentSeason);
        }

        /// <summary>
        /// Returns the total available days for planting and harvesting the crop. Depends on which seasons the crop can grow.
        /// </summary>
        /// <param name="currentSeason">Current season of type Season <see cref="Season"/></param>
        /// <param name="day">Current day as int, can be from 0 to 1</param>
        /// <returns> Total available days for planting and harvesting the crop. <c>int</c></returns>
        public int TotalAvailableDays(Season currentSeason, int day)
        {
            int totalAvailableDays = 0;
            if (IsAvailableForCurrentSeason(currentSeason))
            {
                //Each season has 28 days,
                //get index of current season
                int seasonIndex = Array.IndexOf(Seasons.ToArray(), currentSeason);
                //iterate over the array and add the number of days for each season that is later than the current season
                for (int i = seasonIndex + 1; i < Seasons.Length; i++)
                {
                    totalAvailableDays += 28;
                }
                //add the number of days in the current season
                totalAvailableDays += TotalAvailableDaysInCurrentSeason(day);
            }
            if (currentSeason == Season.Greenhouse)
            {
                totalAvailableDays = (28 * 4);
            }
            return totalAvailableDays;
        }

        /// <summary>
        /// Returns the total available days for planting and harvesting the crop for the current season. Depends on which seasons the crop can grow.
        /// </summary>
        /// <param name="day">Current day as int, can be from 0 to 1</param>
        /// <returns>Total available days for planting and harvesting the crop in current season. <c>int</c></returns>
        public static int TotalAvailableDaysInCurrentSeason(int day)
        {
            return 28 - day;
        }

        /// <summary>
        /// Returns the total harvests for the crop for the available time. Depends on which seasons the crop can grow, the current day , and the fertilizer quality.
        /// </summary>
        /// <param name="currentSeason"> Current season of type Season <see cref="Season"/></param>
        /// <param name="fertilizerQuality"> Quality of the used Fertilizer of type FertilizerQuality <see cref="FertilizerQuality"/></param>
        /// <param name="day"> Current day as int, can be from 0 to 1</param>
        /// <returns> Total number of harvests for the crop for the available time. <c>int</c></returns>
        public int TotalHarvestsWithRemainingDays(Season currentSeason, FertilizerQuality fertilizerQuality, int day)
        {
            int totalHarvestTimes = 0;
            int totalAvailableDays = TotalAvailableDays(currentSeason, day);
            if (Id.Equals("382"))
            {
                Console.WriteLine($"Total Available Days: {totalAvailableDays}");
            }
            //season is Greenhouse
            float averageGrowthSpeedValueForCrop = GetAverageGrowthSpeedValueForCrop(fertilizerQuality);
            int daysToRemove = (int)Math.Ceiling((float)Days * averageGrowthSpeedValueForCrop);
            int growingDays = Math.Max(Days - daysToRemove, 1);
            if (IsAvailableForCurrentSeason(currentSeason) || currentSeason == Season.Greenhouse)
            {
                if (totalAvailableDays < growingDays)
                    return 0;
                //if the crop regrows, then the total harvest times are 1 for the first harvest and then the number of times it can regrow in the remaining days. We always need to subtract one to account for the day lost in the planting day.
                if (Regrow > 0)
                {
                    totalHarvestTimes = ((int)(1 + ((totalAvailableDays - growingDays) / (double)Regrow)));
                }
                else
                    totalHarvestTimes = totalAvailableDays / growingDays;
            }
            return totalHarvestTimes;
        }

        /// <summary>
        /// How many extra crops can be harvested from the crop. Depends on farming level and extra per level defined. Currently Unused
        /// </summary>
        /// <returns> Number of extra crops that can be harvested from the crop. <c>int</c></returns>
        public int ExtraCropsFromFarmingLevel()
        {
            //TODO: Actually use this
            double totalCrops = MinHarvests;
            if (MinHarvests > 1 || MaxHarvests > 1)
            {
                int max_harvest_increase = 0;
                if (MaxHarvestIncreasePerFarmingLevel > 0)
                {
                    max_harvest_increase = Game1.player.FarmingLevel / MaxHarvestIncreasePerFarmingLevel;
                }
                totalCrops = (double)(MinHarvests + MaxHarvests + max_harvest_increase) / 2.0;
            }
            return (int)totalCrops;
        }

        /// <summary>
        /// Meant to calculate the average extra crops from luck if any. Currently Unused
        /// </summary>
        /// <returns> Average extra crops from luck. <c>double</c></returns>
        public double AverageExtraCropsFromRandomness()
        {
            //TODO: Verify this is correct
            double AverageExtraCrop = ChanceForExtraCrops;

#pragma warning disable S125
            // Sections of code should not be commented out
            /*
                        if (ChanceForExtraCrops <= 0.0)
                            return AverageExtraCrop;

                        var items = Enumerable.Range(1, 2);
                        AverageExtraCrop += items.Select(i => Math.Pow(ChanceForExtraCrops, i)).Sum();
                        */

            //average extra crops, should be 0.111 for 0.1 chance and
            return AverageExtraCrop;
#pragma warning restore S125 // Sections of code should not be commented out
        }

        #endregion Growth Values Calculations
    }
}