/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

namespace BetterMixedSeeds.Models
{
    /// <summary>Metadata about a seed.</summary>
    public class Seed
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The id of the seed.</summary>
        public int Id { get; }

        /// <summary>The name of the crop of the seed.</summary>
        public string CropName { get; }

        /// <summary>The chance the seed can drop from mixed seeds.</summary>
        public float DropChance { get; }

        /// <summary>Whether the seed is for a trellis crop.</summary>
        public bool IsTrellis { get; }

        /// <summary>The year requirement to plant the seed.</summary>
        public int YearRequirement { get; }

        /// <summary>The season that this seed can be found from mixed seeds.</summary>
        /// <remarks>Seasons aren't all stored together so the same seeds can have different drop chances for different seasons.</remarks>
        public string Season { get; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Constructs an instance.</summary>
        /// <param name="id">The id of the seed.</param>
        /// <param name="cropName">The name of the crop of the seed.</param>
        /// <param name="dropChance">The chance the seed can drop from mixed seeds.</param>
        /// <param name="isTrellis">>Whether the seed is for a trellis crop.</param>
        /// <param name="yearRequirement">The year requirement to plant the seed.</param>
        /// <param name="season">The season that this seed can be found from mixed seeds.</param>
        public Seed(int id, string cropName, float dropChance, bool isTrellis, int yearRequirement, string season)
        {
            Id = id;
            CropName = cropName;
            DropChance = dropChance;
            IsTrellis = isTrellis;
            YearRequirement = yearRequirement;
            Season = season;
        }

        /// <inheritdoc/>
        public override string ToString() => $"Id: {Id}, CropName: {CropName}, DropChance: {DropChance}, Season: {Season}, IsTrellis: {IsTrellis}, YearRequirement: {YearRequirement}";
    }
}
