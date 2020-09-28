namespace RegularQuality
{
	/// <summary>Mod Configuration settings.</summary>
	internal class ModConfig
	{
		/// <summary>Multiply the rarity by this factor and add it on top, e.g.</summary>
		/// <example>
		/// normal  = 0 * 4 = 0 (keep)
		/// silver  = 1 * 4 = 4 (4x on top)
		/// gold    = 2 * 4 = 8
		/// iridium = 3 * 4 = 12
		/// .</example>
		public int BundleIngredientQualityMultiplicator { get; set; } = 4;
	}
}
