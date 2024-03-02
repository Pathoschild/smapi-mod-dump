/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System;
using System.Linq;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.GameData.Crops;

namespace Pathoschild.Stardew.Common.DataParsers
{
    /// <summary>Analyzes crop data for a tile.</summary>
    internal class CropDataParser
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The crop.</summary>
        public Crop? Crop { get; }

        /// <summary>The crop's underlying data.</summary>
        public CropData? CropData { get; }

        /// <summary>The seasons in which the crop grows.</summary>
        public Season[] Seasons { get; }

        /// <summary>The phase index in <see cref="StardewValley.Crop.phaseDays"/> when the crop can be harvested.</summary>
        public int HarvestablePhase { get; }

        /// <summary>The number of days needed between planting and first harvest.</summary>
        public int DaysToFirstHarvest { get; }

        /// <summary>The number of days needed between harvests, after the first harvest.</summary>
        public int DaysToSubsequentHarvest { get; }

        /// <summary>Whether the crop can be harvested multiple times.</summary>
        public bool HasMultipleHarvests { get; }

        /// <summary>Whether the crop is ready to harvest now.</summary>
        public bool CanHarvestNow { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="crop">The crop.</param>
        /// <param name="isPlanted">Whether the crop is planted.</param>
        public CropDataParser(Crop? crop, bool isPlanted)
        {
            this.Crop = crop;
            this.CropData = crop?.GetData();

            var data = this.CropData;
            if (data != null)
            {
                // get crop data
                this.Seasons = data.Seasons.ToArray();
                this.HasMultipleHarvests = crop!.RegrowsAfterHarvest();
                this.HarvestablePhase = crop.phaseDays.Count - 1;
                this.CanHarvestNow = (crop.currentPhase.Value >= this.HarvestablePhase) && (!crop.fullyGrown.Value || crop.dayOfCurrentPhase.Value <= 0);
                this.DaysToFirstHarvest = crop.phaseDays.Take(crop.phaseDays.Count - 1).Sum(); // ignore harvestable phase
                this.DaysToSubsequentHarvest = data.RegrowDays;

                // adjust for agriculturist profession (10% faster initial growth)
                if (!isPlanted && Game1.player.professions.Contains(Farmer.agriculturist))
                    this.DaysToFirstHarvest = (int)(this.DaysToFirstHarvest * 0.9);
            }
            else
                this.Seasons = Array.Empty<Season>();
        }

        /// <summary>Get the date when the crop will next be ready to harvest.</summary>
        public SDate GetNextHarvest()
        {
            // get crop
            Crop? crop = this.Crop;
            if (crop == null)
                throw new InvalidOperationException("Can't get the harvest date because there's no crop.");

            // get data
            CropData? data = this.CropData;
            if (data == null)
                throw new InvalidOperationException("Can't get the harvest date because the crop has no data.");

            // ready now
            if (this.CanHarvestNow)
                return SDate.Now();

            // growing: days until next harvest
            if (!crop.fullyGrown.Value)
            {
                int daysUntilLastPhase = this.DaysToFirstHarvest - crop.dayOfCurrentPhase.Value - crop.phaseDays.Take(crop.currentPhase.Value).Sum();
                return SDate.Now().AddDays(daysUntilLastPhase);
            }

            // regrowable crop harvested today
            if (crop.dayOfCurrentPhase.Value >= data.RegrowDays)
                return SDate.Now().AddDays(data.RegrowDays);

            // regrowable crop
            // dayOfCurrentPhase decreases to 0 when fully grown, where <=0 is harvestable
            return SDate.Now().AddDays(crop.dayOfCurrentPhase.Value);
        }

        /// <summary>Get a sample item acquired by harvesting the crop.</summary>
        /// <exception cref="InvalidOperationException">There's no crop instance.</exception>
        public Item GetSampleDrop()
        {
            if (this.Crop == null)
                throw new InvalidOperationException("Can't get a sample drop because there's no crop.");

            return ItemRegistry.Create(this.Crop.indexOfHarvest.Value);
        }
    }
}
