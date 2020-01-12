namespace RegularQuality
{
    internal class ModConfig
    {
        // multiply the rarity by this factor and add it on top, e.g.
        // normal  = 0 * 4 = 0 (keep)
        // silver  = 1 * 4 = 4 (4x on top)
        // gold    = 2 * 4 = 8
        // iridium = 3 * 4 = 12
        public int BundleIngredientQualityMultiplicator { get; set; } = 4;
    }
}
