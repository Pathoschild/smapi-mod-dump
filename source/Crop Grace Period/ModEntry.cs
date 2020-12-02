/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stoobinator/Stardew-Valley-Grace-Period
**
*************************************************/

using System.Collections.Generic;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace CropGracePeriod
{
    /// <summary>
    /// Mod to allow crops planted in a previous season to survive into the next
    /// Crops that regrow multiple times (e.g. beans) will allow one additional harvest before dying
    /// </summary>
    public class ModEntry : Mod
    {
        #region Fields

        /// <summary>
        /// Configuration for the mod
        /// </summary>
        private ModConfig Config;

        /// <summary>
        /// A list of crops that should be destroyed if harvested
        /// </summary>
        private List<Crop> CropsToWatch;

        /// <summary>
        /// A list of trees with three fruits on them
        /// </summary>
        private List<FruitTree> FullTrees;

        #endregion

        #region Entry point

        /// <summary>
        /// The mod entry point, called after the mod is first loaded
        /// </summary>
        /// <param name="helper">Provides simplified APIs for writing mods</param>
        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.DayEnding += OnDayEnding;
            helper.Events.GameLoop.Saving += OnSaving;
        }

        #endregion

        #region Events

        /// <summary>
        /// Tracks crops and trees that might need to be specially handled at day's end
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event data</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            GameLocation farm = Game1.getLocationFromName("Farm");
            CropsToWatch = new List<Crop>();
            FullTrees = new List<FruitTree>();

            foreach (var tile in farm.terrainFeatures)
            {
                foreach (TerrainFeature feature in tile.Values)
                {
                    if (feature is HoeDirt dirt
                        && IsSuspiciousCrop(dirt.crop))
                    {
                        CropsToWatch.Add(dirt.crop);
                    }
                    else if (feature is FruitTree tree
                        && tree.fruitsOnTree == 3)
                    {
                        FullTrees.Add(tree);
                    }
                }
            }
        }

        /// <summary>
        /// Kill all crops not protected by this mod
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event data</param>
        private void OnDayEnding(object sender, DayEndingEventArgs e)
        {
            GameLocation farm = Game1.getLocationFromName("Farm");
            var date = SDate.Now().AddDays(1);

            if (IsGracePeriodActive(date))
            {
                farm.IsGreenhouse = true;

                // Manually kill all out of season crops not protected by the grace period
                foreach (var tile in farm.terrainFeatures)
                {
                    foreach (TerrainFeature feature in tile.Values)
                    {
                        if (feature is HoeDirt dirt &&
                            ShouldKillCrop(dirt.crop, date))
                        {
                            dirt.crop.Kill();
                        }
                    }
                }

                // Kill any out of season multi-harvest crops that were harvested 
                foreach (Crop crop in CropsToWatch)
                {
                    if (crop.fullyGrown)
                    {
                        crop.Kill();
                    }
                }
            }
        }

        /// <summary>
        /// Turn off greenhouse effect
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event data</param>
        private void OnSaving(object sender, SavingEventArgs e)
        {
            GameLocation farm = Game1.getLocationFromName("Farm");
            var date = SDate.Now();

            farm.IsGreenhouse = false;

            // Handle out-of-season fruit trees
            foreach (TerrainFeature feature in farm.terrainFeatures.Values)
            {
                if (feature is FruitTree tree
                    && Game1.currentSeason != tree.fruitSeason)
                {
                    tree.GreenHouseTree = false;

                    // Ignore burnt trees 
                    if (tree.struckByLightningCountdown.Value > 0)
                    {
                        continue;
                    }
                    // Clear out of season trees that are empty
                    else if (!IsCropInGracePeriod(date, new NetStringList { tree.fruitSeason })
                        || tree.fruitsOnTree.Value == 1)
                    {
                        tree.fruitsOnTree.Value = 0;
                    }
                    // Allow fruits to stay on trees in grace period
                    else if (!FullTrees.Contains(tree))
                    {
                        tree.fruitsOnTree.Value -= 1;
                    }
                }
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Check if plant is an out of season, fully-grown crop that can be
        /// harvested multiple times, e.g. a bean plant in summer
        /// </summary>
        /// <param name="crop">The crop to check</param>
        /// <returns>True if we should keep an eye on this crop, false otherwise</returns>
        private bool IsSuspiciousCrop(Crop crop)
        {
            if (crop != null
                && crop.regrowAfterHarvest.Value > -1
                && !crop.seasonsToGrowIn.Contains(Game1.currentSeason)
                && IsHarvestable(crop))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check if a crop can be harvested
        /// </summary>
        /// <param name="crop">The crop to check</param>
        /// <returns>True if the crop can be harvested, false otherwise</returns>
        private bool IsHarvestable(Crop crop)
        {
            return crop.currentPhase.Value >= crop.phaseDays.Count - 1
                && (!crop.fullyGrown.Value || crop.dayOfCurrentPhase.Value <= 0);
        }

        /// <summary>
        /// Checks if any grace period is currently active
        /// </summary>
        /// <param name="date">The current date</param>
        /// <returns>True if a grace period is active, false otherwise</returns>
        private bool IsGracePeriodActive(SDate date)
        {
            int[] periods = { Config.Spring, Config.Summer, Config.Fall, Config.Winter };

            for (int i = 0; i < 3; i++)
            {
                int daysSinceSeasonEnded = date.Day + 28 * i;
                if (daysSinceSeasonEnded >= date.DaysSinceStart)
                {
                    break;
                }

                SDate lastSeasonEnd = date.AddDays(-daysSinceSeasonEnded);
                if (date <= lastSeasonEnd.AddDays(periods[lastSeasonEnd.SeasonIndex]))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Check if a crop should be killed
        /// </summary>
        /// <param name="crop">The crop to check</param>
        /// <param name="date">The current date</param>
        /// <returns>True if the crop should be killed, false otherwise</returns>
        private bool ShouldKillCrop(Crop crop, SDate date)
        {
            // Don't kill a non-existent crop
            if (crop == null)
            {
                return false;
            }
            // If it is currently the crop's season, don't kill it
            else if (crop.seasonsToGrowIn.Contains(date.Season))
            {
                return false;
            }
            // Check if crop is within a grace period 
            else if (IsCropInGracePeriod(date, crop.seasonsToGrowIn))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks the previous three seasons to see if a crop is within any
        /// of their grace periods
        /// </summary>
        /// <param name="date">The current date</param>
        /// <param name="seasonsToGrowIn">The seasons a crop can grow in</param>
        /// <returns>True if the crop is within the grace period, false otherwise</returns>
        private bool IsCropInGracePeriod(SDate date, NetStringList seasonsToGrowIn)
        {
            int[] periods = { Config.Spring, Config.Summer, Config.Fall, Config.Winter };

            for (int i = 0; i < 3; i++)
            {
                int daysSinceSeasonEnded = date.Day + 28 * i;
                if (daysSinceSeasonEnded >= date.DaysSinceStart)
                {
                    break;
                }

                SDate lastSeasonEnd = date.AddDays(-daysSinceSeasonEnded);
                if (date <= lastSeasonEnd.AddDays(periods[lastSeasonEnd.SeasonIndex])
                    && seasonsToGrowIn.Contains(lastSeasonEnd.Season))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion
    }

    /// <summary>
    /// Configuration class for this mod
    /// </summary>
    internal class ModConfig
    {
        /// <summary>
        /// How long to protect Spring crops after Spring ends
        /// </summary>
        public int Spring { get; set; } = 28;

        /// <summary>
        /// How long to protect Summer crops after Summer ends
        /// </summary>
        public int Summer { get; set; } = 28;

        /// <summary>
        /// How long to protect Fall crops after Fall ends
        /// </summary>
        public int Fall { get; set; } = 0;

        /// <summary>
        /// How long to protect Winter crops after Winter ends
        /// </summary>
        public int Winter { get; set; } = 0;
    }
}