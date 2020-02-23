namespace BetterMixedSeeds.Config
{
    /// <summary>A data model that contain properties for name, enabled, and chance.</summary>
    public class Crop
    {
        /// <summary>The crops name, this is the display name used in the configuration.</summary>
        public string Name { get; set; }

        /// <summary>Whether the crop should get added the seed list.</summary>
        public bool Enabled { get; set; }

        /// <summary>The chance the crop will have at being picked. This is the number of times the seed get's added to the seed list, high numbers can lead to performance issues.</summary>
        public int Chance { get; set; }

        /// <summary>Construct and instance.</summary>
        /// <param name="name">Crop name.</param>
        /// <param name="enabled">Whether the crop is enabled.</param>
        /// <param name="chance">The crop chance. (number of times it gets added to the seed list)</param>
        public Crop(string name, bool enabled, int chance)
        {
            Name = name;
            Enabled = enabled;
            Chance = chance;
        }
    }
}
