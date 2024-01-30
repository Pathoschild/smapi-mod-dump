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
using StardewValley;
using System;
using System.Collections.Generic;
using static ProfitCalculator.Utils;

namespace ProfitCalculator.main
{
    /// <summary>
    /// Class used to calculate the profits for crops. Contains all the settings for the calculator and the functions used to calculate the profits. Also contains the list of crops and the crop parsers. <see cref="TotalCropProfit(Crop)"/> and <see cref="TotalCropProfitPerDay(Crop)"/>, <see cref="TotalFertilizerCost(Crop)"/>, <see cref="TotalFertilzerCostPerDay(Crop)"/>, <see cref="TotalSeedsCost(Crop)"/>, <see cref="TotalSeedsCostPerDay(Crop)"/> are the main functions used to calculate the profits. <see cref="RetrieveCropsAsOrderderList"/> and <see cref="RetrieveCropInfos"/> are the main functions used to retrieve the list of crops and crop infos.
    /// </summary>
    public class Calculator
    {
        #region properties

        /// <summary>
        /// List of all crops in the game
        /// </summary>
        private Dictionary<string, Crop> crops;

        /// <summary>
        /// Day of the season
        /// </summary>
        private uint Day { get; set; }

        /// <summary>
        /// Max days of a season
        /// </summary>
        private uint MaxDay { get; set; }

        /// <summary>
        /// Min days of a season
        /// </summary>
        private uint MinDay { get; set; }

        /// <summary>
        /// Season of the year selected
        /// </summary>
        private Season Season { get; set; }

        /// <summary>
        /// Type of produce selected
        /// TODO: Implement this.
        /// </summary>
        private ProduceType ProduceType { get; set; }

        /// <summary>
        /// Type of fertilizer selected
        /// </summary>
        private FertilizerQuality FertilizerQuality { get; set; }

        /// <summary>
        /// Whether or not the player pays for seeds
        /// </summary>
        private bool PayForSeeds { get; set; }

        /// <summary>
        /// Whether or not the player pays for fertilizer
        /// </summary>
        private bool PayForFertilizer { get; set; }

        /// <summary>
        /// Max money the player is willing to spend on seeds or fertilizer
        /// </summary>
        private uint MaxMoney { get; set; }

        /// <summary>
        /// Whether or not to use base stats for the player or the current stats
        /// </summary>
        private bool UseBaseStats { get; set; }

        /// <summary>
        /// Whether or not to calculate crops that are not available for the current season. If false, then crops that are not available for the current season will not be calculated. Not used.
        /// </summary>
        private bool CrossSeason { get; set; }

        private double[] PriceMultipliers { get; set; } = new double[4] { 1.0, 1.25, 1.5, 2.0 };

        /// <summary>
        /// Current farming level of the player or 0 if using base stats, used for calculating crop quality chances
        /// </summary>
        private int FarmingLevel { get; set; }

        #endregion properties

        /// <summary>
        /// Constructor for the calculator, initializes the list of crops and crop parsers. Instantiates the calculator with default values.
        /// </summary>
        public Calculator()
        {
            crops = new Dictionary<string, Crop>();
        }

        /// <summary>
        /// Sets the settings for the calculator to use when calculating profits.
        /// </summary>
        /// <param name="day"><see cref="Day"/></param>
        /// <param name="maxDay"><see cref="MaxDay"/></param>
        /// <param name="minDay"><see cref="MinDay"/></param>
        /// <param name="season"><see cref="Season"/></param>
        /// <param name="produceType"><see cref="ProduceType"/></param>
        /// <param name="fertilizerQuality"> <see cref="FertilizerQuality"/></param>
        /// <param name="payForSeeds"> <see cref="PayForSeeds"/></param>
        /// <param name="payForFertilizer"> <see cref="PayForFertilizer"/></param>
        /// <param name="maxMoney"> <see cref="MaxMoney"/></param>
        /// <param name="useBaseStats"> <see cref="UseBaseStats"/></param>
        /// <param name="crossSeason"> <see cref="CrossSeason"/></param>
        public void SetSettings(uint day, uint maxDay, uint minDay, Season season, ProduceType produceType, FertilizerQuality fertilizerQuality, bool payForSeeds, bool payForFertilizer, uint maxMoney, bool useBaseStats, bool crossSeason = true)
        {
            Day = day;
            MaxDay = maxDay;
            MinDay = minDay;
            Season = season;
            ProduceType = produceType;
            FertilizerQuality = fertilizerQuality;
            PayForSeeds = payForSeeds;
            PayForFertilizer = payForFertilizer;
            MaxMoney = maxMoney;
            UseBaseStats = useBaseStats;
            CrossSeason = crossSeason;
            if (useBaseStats)
            {
                FarmingLevel = 0;
            }
            else
            {
                FarmingLevel = Game1.player.FarmingLevel;
            }
        }

        /// <summary>
        /// Clears the list of crops.
        /// </summary>
        public void ClearCrops()
        {
            crops.Clear();
        }

        /// <summary>
        /// Retrieves the list of crops as an ordered list by profit.
        /// </summary>
        /// <returns> List of crops ordered by profit </returns>
        public List<Crop> RetrieveCropsAsOrderderList()
        {
            // sort crops by profit
            // return list
            List<Crop> cropList = new();
            foreach (KeyValuePair<string, Crop> crop in crops)
            {
                cropList.Add(crop.Value);
            }
            cropList.Sort((x, y) => y.Price.CompareTo(x.Price));
#pragma warning disable
            /*foreach (Crop crop in cropList)
            {
                Monitor.Log($"OC: {crop.Name} Id: {crop.Id} Seed: {crop.Seeds[0].ParentSheetIndex} ValueWithStats: {crop.Price * this.GetAverageValueForCropAfterModifiers()} #Harvests: {crop.TotalHarvestsWithRemainingDays(Season, FertilizerQuality, (int)Day)} TotalProfit: {TotalCropProfit(crop)} " +
                    $"ppd: {TotalCropProfitPerDay(crop)} " +
                    $"tfn: {TotalFertilizerNeeded(crop)} " +
                    $"tfnpd: {TotalFertilzerCostPerDay(crop)} " +
                    $"tsn: {TotalSeedsNeeded(crop)} " +
                    $"tsc: {TotalSeedsCost(crop)} " +
                    $"tscpd: {TotalSeedsCostPerDay(crop)}", LogLevel.Debug);
            }*/
#pragma warning enable
            return cropList;
        }

        /// <summary>
        /// Retrieves the list of <see cref="CropInfo"/> as an ordered list by profit.
        /// </summary>
        /// <returns> List of <see cref="CropInfo"/> ordered by profit </returns>
        public List<CropInfo> RetrieveCropInfos()
        {
            List<CropInfo> cropInfos = new();
            foreach (Crop crop in crops.Values)
            {
                CropInfo ci = RetrieveCropInfo(crop);
                if (ci.TotalHarvests >= 1)
                    if (!PayForSeeds)
                        cropInfos.Add(ci);
                    else if (PayForSeeds && ci.TotalSeedLoss <= MaxMoney)
                        cropInfos.Add(ci);
            }
            cropInfos.Sort((x, y) => y.ProfitPerDay.CompareTo(x.ProfitPerDay));
            return cropInfos;
        }

        /// <summary>
        /// Retrieves the <see cref="CropInfo"/> for a specific crop. Uses information obtained by calling internal functions to calculate the values and build the object.
        /// </summary>
        /// <param name="crop"> Crop to retrieve <see cref="CropInfo"/> for </param>
        /// <returns> <see cref="CropInfo"/> for the crop </returns>
        private CropInfo RetrieveCropInfo(Crop crop)
        {
            double totalProfit = TotalCropProfit(crop);
            double profitPerDay = TotalCropProfitPerDay(crop);
            double totalSeedLoss = TotalSeedsCost(crop);
            double seedLossPerDay = TotalSeedsCostPerDay(crop);
            double totalFertilizerLoss = TotalFertilizerCost(crop);
            double fertilizerLossPerDay = TotalFertilzerCostPerDay(crop);
            ProduceType produceType = ProduceType;
            int duration = crop.TotalAvailableDays(Season, (int)Day);
            int totalHarvests = crop.TotalHarvestsWithRemainingDays(Season, FertilizerQuality, (int)Day);

            float averageGrowthSpeedValueForCrop = crop.GetAverageGrowthSpeedValueForCrop(FertilizerQuality);
            int daysToRemove = (int)Math.Ceiling((float)crop.Days * averageGrowthSpeedValueForCrop);
            int growingDays = Math.Max(crop.Days - daysToRemove, 1);

            int growthTime = growingDays;
            int regrowthTime = crop.Regrow;
            int productCount = crop.MinHarvests;
            double chanceOfExtraProduct = crop.AverageExtraCropsFromRandomness();
            double chanceOfNormalQuality = GetCropBaseQualityChance();
            double chanceOfSilverQuality = GetCropSilverQualityChance();
            double chanceOfGoldQuality = GetCropGoldQualityChance();
            double chanceOfIridiumQuality = GetCropIridiumQualityChance();
            return new CropInfo(crop, totalProfit, profitPerDay, totalSeedLoss, seedLossPerDay, totalFertilizerLoss, fertilizerLossPerDay, produceType, duration, totalHarvests, growthTime, regrowthTime, productCount, chanceOfExtraProduct, chanceOfNormalQuality, chanceOfSilverQuality, chanceOfGoldQuality, chanceOfIridiumQuality);
        }

        /// <summary>
        /// Adds a crop to the list of crops.
        /// </summary>
        /// <param name="id"> Id of the crop </param>
        /// <param name="crop"> Crop to add </param>
        public void AddCrop(string id, Crop crop)
        {
            //check if already exists
            if (!crops.ContainsKey(id))
            {
                try
                {
                    crops.Add(id, crop);
                }
                catch (Exception e)
                {
                    Monitor.Log("Failed to add\n" + crop.ToString(), LogLevel.Debug);
                }
            }
        }

        #region Crop Profit Calculations

        /// <summary>
        /// Calculates the total profit for a crop. See <see cref="GetAverageValueForCropAfterModifiers"/>, <see cref="Crop.AverageExtraCropsFromRandomness"/>, <see cref="Crop.TotalHarvestsWithRemainingDays"/> for more information.
        /// </summary>
        /// <param name="crop"> Crop to calculate profit for </param>
        /// <returns> Total profit for the crop </returns>
        private double TotalCropProfit(Crop crop)
        {
            double totalProfitFromFirstProduce;
            double totalProfitFromRemainingProduce;

            if (!crop.affectByQuality)
            {
                totalProfitFromFirstProduce = 0;
                totalProfitFromRemainingProduce = crop.Price;
            }
            else
            {
                double averageValue = crop.Price * this.GetAverageValueForCropAfterModifiers();//only applies to first produce
                totalProfitFromFirstProduce = averageValue;

                double averageExtraCrops = crop.AverageExtraCropsFromRandomness();

                totalProfitFromRemainingProduce = (crop.MinHarvests - 1 >= 0 ? crop.MinHarvests - 1 : 0) * crop.Price;

                totalProfitFromRemainingProduce += (crop.Price * averageExtraCrops);
            }
            if (!UseBaseStats && Game1.player.professions.Contains(Farmer.tiller))
            {
                totalProfitFromRemainingProduce *= 1.1f;
            }
            double result = (totalProfitFromFirstProduce + totalProfitFromRemainingProduce) * crop.TotalHarvestsWithRemainingDays(Season, FertilizerQuality, (int)Day);
            return result;
        }

        /// <summary>
        /// Calculates the total profit per day for a crop. See <see cref="TotalCropProfit"/> for more information. Simply devides the total profit by the total available days for the crop.
        /// </summary>
        /// <param name="crop"> Crop to calculate profit per day for </param>
        /// <returns> Total profit per day for the crop </returns>
        private double TotalCropProfitPerDay(Crop crop)
        {
            double totalProfit = TotalCropProfit(crop);
            if (totalProfit == 0)
            {
                return 0;
            }
            double totalCropProfitPerDay = totalProfit / crop.TotalAvailableDays(Season, (int)Day);
            return totalCropProfitPerDay;
        }

        /// <summary>
        /// Total fertilizer needed for a crop. If planted in greenhouse or if the crop only grows in one season, then only 1 fertilizer is needed. Otherwise, the total number of days the crop is available is divided by 28 and rounded up to get the total number of fertilizer needed.
        /// </summary>
        /// <param name="crop"> Crop to calculate fertilizer needed for </param>
        /// <returns> Total fertilizer needed for the crop </returns>
        private int TotalFertilizerNeeded(Crop crop)
        {
            if (Season == Season.Greenhouse || crop.Seasons.Length == 1)
                return 1;
            else
            {
                return (int)Math.Ceiling(crop.TotalAvailableDays(Season, (int)Day) / 28.0);
            }
        }

        /// <summary>
        /// Total fertilizer cost for a crop. See <see cref="TotalFertilizerNeeded"/> and <see cref="Utils.FertilizerPrices(FertilizerQuality)"/> for more information.
        /// </summary>
        /// <param name="crop"> Crop to calculate fertilizer cost for </param>
        /// <returns> Total fertilizer cost for the crop </returns>
        private int TotalFertilizerCost(Crop crop)
        {
            if (!PayForFertilizer)
            {
                return 0;
            }
            int fertNeeded = TotalFertilizerNeeded(crop);
            int fertCost = Utils.FertilizerPrices(FertilizerQuality);
            return fertNeeded * fertCost;
        }

        /// <summary>
        /// Total fertilizer cost per day for a crop. See <see cref="TotalFertilizerCost"/> for more information. Simply devides the total fertilizer cost by the total available days for the crop.
        /// </summary>
        /// <param name="crop"> Crop to calculate fertilizer cost per day for </param>
        /// <returns> Total fertilizer cost per day for the crop </returns>
        private double TotalFertilzerCostPerDay(Crop crop)
        {
            int fertCost = TotalFertilizerCost(crop);
            if (fertCost == 0)
            {
                return 0;
            }
            double totalFertilizerCostPerDay = (double)fertCost / (double)crop.TotalAvailableDays(Season, (int)Day);
            return totalFertilizerCostPerDay;
        }

        /// <summary>
        /// Total seeds needed for a crop. If the crop regrows, then only 1 seed is needed. Otherwise, the total number of harvests is calculated and multiplied by the number of seeds needed per harvest.
        /// </summary>
        /// <param name="crop"> Crop to calculate seeds needed for </param>
        /// <returns> Total seeds needed for the crop </returns>
        private int TotalSeedsNeeded(Crop crop)
        {
            if (crop.Regrow > 0 && crop.TotalAvailableDays(Season, (int)Day) > 0)
                return 1;
            else return crop.TotalHarvestsWithRemainingDays(Season, FertilizerQuality, (int)Day);
        }

        /// <summary>
        /// Total seeds cost for a crop. See <see cref="TotalSeedsNeeded"/> and <see cref="Crop.GetSeedPrice"/> for more information.
        /// </summary>
        /// <param name="crop"> Crop to calculate seeds cost for </param>
        /// <returns> Total seeds cost for the crop </returns>
        private int TotalSeedsCost(Crop crop)
        {
            if (!PayForSeeds)
                return 0;
            int seedsNeeded = TotalSeedsNeeded(crop);
            int seedCost = crop.GetSeedPrice();

            return seedsNeeded * seedCost;
        }

        /// <summary>
        /// Total seeds cost per day for a crop. See <see cref="TotalSeedsCost"/> for more information. Simply devides the total seeds cost by the total available days for the crop.
        /// </summary>
        /// <param name="crop"></param>
        /// <returns></returns>
        private double TotalSeedsCostPerDay(Crop crop)
        {
            int seedCost = TotalSeedsCost(crop);
            if (seedCost == 0)
            {
                return 0;
            }
            double totalSeedsCostPerDay = (double)seedCost / (double)crop.TotalAvailableDays(Season, (int)Day);
            return totalSeedsCostPerDay;
        }

        #endregion Crop Profit Calculations

        #region Crop Modifer Value Calculations

        /// <summary>
        /// Prints the crop chance tables for all farming levels. Used for debugging.
        /// </summary>
        public void PrintCropChanceTablesForAllFarmingLevels()
        {
            int backupFarmingLevel = FarmingLevel;
            Monitor.Log("|Farming Level\tBase Chance\tSilver Chance\tGold Chance\tIridium Chance\tAvg Value|", LogLevel.Debug);
            for (int i = 0; i < 15; i++)
            {
                FarmingLevel = i;
                double chanceForGoldQuality = GetCropGoldQualityChance();
                double chanceForSilverQuality = GetCropSilverQualityChance();
                double chanceForIridiumQuality = GetCropIridiumQualityChance();
                double chanceForBaseQuality = GetCropBaseQualityChance();
                double averageValue = GetAverageValueForCropAfterModifiers();

                Monitor.Log(
                    $"|{FarmingLevel}\t\t\t   "
                    + $"{chanceForBaseQuality * 100:##}\t\t"
                    + $"{chanceForSilverQuality * 100:##}\t\t"
                    + $"{chanceForGoldQuality * 100:##}\t\t"
                    + $"{chanceForIridiumQuality * 100:##}\t\t"
                    + $"{averageValue}|"
                    , LogLevel.Debug);
            }
            FarmingLevel = backupFarmingLevel;
        }

        /// <summary>
        /// Prints the crop chance tables for all farming levels and fertilizer types. Used for debugging.
        /// </summary>
        public void PrintCropChanceTablesForAllFarmingLevelsAndFertilizerType()
        {
            FertilizerQuality backupFertilizerQuality = FertilizerQuality;
            for (int i = 0; i < 4; i++)
            {
                FertilizerQuality = (FertilizerQuality)i;
                Monitor.Log($"Fertilizer: {FertilizerQuality}", LogLevel.Debug);
                PrintCropChanceTablesForAllFarmingLevels();
            }
            FertilizerQuality = backupFertilizerQuality;
        }

        /// <summary>
        /// Prints the average crop value modifier for current farming level and fertilizer type. Used for debugging. Uses <see cref="GetCropGoldQualityChance"/>, <see cref="GetCropSilverQualityChance"/>, <see cref="GetCropIridiumQualityChance"/>, <see cref="GetCropBaseQualityChance"/>. <see cref="GetCropBaseQualityChance"/> and <see cref="PriceMultipliers"/> to calculate the average value modifier for the crop.
        /// </summary>
        /// <returns> Average crop value modifier for current farming level and fertilizer type </returns>
        public double GetAverageValueMultiplierForCrop()
        {
            double[] priceMultipliers = PriceMultipliers;

            //apply farm level quality modifiers
            double chanceForGoldQuality = GetCropGoldQualityChance();
            double chanceForSilverQuality = GetCropSilverQualityChance();
            double chanceForIridiumQuality = GetCropIridiumQualityChance();
            double chanceForBaseQuality = GetCropBaseQualityChance();
            //calculate average value modifier for price
            double averageValue = 0f;
            averageValue += chanceForBaseQuality * priceMultipliers[0];
            averageValue += chanceForSilverQuality * priceMultipliers[1];
            averageValue += chanceForGoldQuality * priceMultipliers[2];
            averageValue += chanceForIridiumQuality * priceMultipliers[3];
            return averageValue;
        }

        /// <summary>
        /// Calculates the average crop value modifier applying relevant skill modifiers. See <see cref="GetAverageValueMultiplierForCrop"/> for more information.
        /// </summary>
        /// <returns> Average crop value modifier applying relevant skill modifiers </returns>
        public double GetAverageValueForCropAfterModifiers()
        {
            double averageValue = this.GetAverageValueMultiplierForCrop();
            if (!UseBaseStats && Game1.player.professions.Contains(Farmer.tiller))
            {
                averageValue *= 1.1f;
            }
            return Math.Round(averageValue, 2);
        }

        /// <summary>
        /// Calculates the base chance for gold quality. See <see cref="StardewValley.Crop.harvest"/> for more information.
        /// </summary>
        /// <param name="limit"> Limit for the chance</param>
        /// <returns> Base chance for gold quality </returns>
        private double GetCropBaseGoldQualityChance(double limit = 9999999999)
        {
            int fertilizerQualityLevel = ((int)FertilizerQuality) > 0 ? ((int)FertilizerQuality) : 0;
            double part1 = (0.2 * (FarmingLevel / 10.0)) + 0.01;
            double part2 = 0.2 * (fertilizerQualityLevel * ((FarmingLevel + 2) / 12.0));
            return Math.Min(limit, part1 + part2);
        }

        /// <summary>
        /// Calculates the chance for normal quality. See <see cref="StardewValley.Crop.harvest"/> for more information.
        /// </summary>
        /// <returns> Chance for normal quality </returns>
        private double GetCropBaseQualityChance()
        {
            return FertilizerQuality >= FertilizerQuality.Deluxe ? 0f : Math.Max(0f, 1f - (this.GetCropIridiumQualityChance() + this.GetCropGoldQualityChance() + this.GetCropSilverQualityChance()));
        }

        /// <summary>
        /// Calculates the chance for silver quality. See <see cref="StardewValley.Crop.harvest"/> for more information.
        /// </summary>
        /// <returns> Chance for silver quality </returns>
        private double GetCropSilverQualityChance()
        {
            return FertilizerQuality >= FertilizerQuality.Deluxe ? 1f - (this.GetCropIridiumQualityChance() + this.GetCropGoldQualityChance()) : (1f - this.GetCropIridiumQualityChance()) * (1f - this.GetCropBaseGoldQualityChance()) * Math.Min(0.75, 2 * this.GetCropBaseGoldQualityChance());
        }

        /// <summary>
        /// Calculates the chance for gold quality. See <see cref="StardewValley.Crop.harvest"/> for more information.
        /// </summary>
        /// <returns> Chance for gold quality </returns>
        private double GetCropGoldQualityChance()
        {
            return this.GetCropBaseGoldQualityChance(1f) * (1f - this.GetCropIridiumQualityChance());
        }

        /// <summary>
        /// Calculates the chance for iridium quality. See <see cref="StardewValley.Crop.harvest"/> for more information.
        /// </summary>
        /// <returns> Chance for iridium quality </returns>
        private double GetCropIridiumQualityChance()
        {
            return FertilizerQuality >= FertilizerQuality.Deluxe ? GetCropBaseGoldQualityChance() / 2.0 : 0f;
        }

        #endregion Crop Modifer Value Calculations
    }
}