namespace FarmExpansion.Framework
{
    /// <summary>The JSON model for global settings read from the mod's config file.</summary>
    public class FEConfig
    {
        /// <summary>Determines whether crows can spawn on the farm expansion.</summary>
        public bool enableCrows { get; set; } = true;

        /// <summary>Determines whether to patch an alternate entrance to the expansion in the backwoods.</summary>
        public bool useBackwoodsEntrance { get; set; } = false;
    }
}
