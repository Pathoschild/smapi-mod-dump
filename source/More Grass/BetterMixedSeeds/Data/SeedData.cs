namespace BetterMixedSeeds.Data
{
    /// <summary>Data about a seed.</summary>
    public class SeedData
    {
        /// <summary>The crop name.</summary>
        public string CropName { get; set; }

        /// <summary>The seed id.</summary>
        public int SeedId { get; set; }

        /// <summary>The seed name.</summary>
        public string SeedName { get; set; }

        /// <summary>The year required to plant the seed.</summary>
        public int YearRequirement { get; set; }

        /// <summary>Construct an instance.</summary>
        /// <param name="cropName">The crop name.</param>
        /// <param name="seedId">The seed id.</param>
        /// <param name="seedName">The seed name (for JA).</param>
        /// <param name="yearRequirement">They year required to plant the seed.</param>
        public SeedData(string cropName, int seedId = -1, string seedName = null, int yearRequirement = 0)
        {
            CropName = cropName;
            SeedId = seedId;
            SeedName = seedName;
            YearRequirement = yearRequirement;
        }
    }
}
