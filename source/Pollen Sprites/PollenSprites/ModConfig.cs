/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/PollenSprites
**
*************************************************/

namespace PollenSprites
{
    /// <summary>The set of options available in this mod's config.json file.</summary>
    public class ModConfig
    {
        /// <summary>If true, pollen sprites should apply a debuff on contact with players.</summary>
        public bool EnableSlowDebuff { get; set; } = false;
        /// <summary>If true, pollen sprites should drain energy on contact with players.</summary>
        public bool EnableEnergyDrain { get; set; } = false;
        /// <summary>Settings for each group of seeds pollen sprites can drop.</summary>
        public SeedDropChances SeedDropChances { get; set; } = new SeedDropChances();
    }

    /// <summary>Settings for each group of seeds pollen sprites can drop.</summary>
    public class SeedDropChances
    {
        /// <summary>The chance that <see cref="SeedManager.MixedSeeds"/> will be dropped.</summary>
        public float MixedSeeds { get; set; } = 0.50f;
        /// <summary>The chance that a random seed from <see cref="SeedManager.FlowerSeeds"/> will be dropped.</summary>
        public float FlowerSeeds { get; set; } = 0.20f;
        /// <summary>The chance that a random seed from <see cref="SeedManager.AllSeeds"/> will be dropped.</summary>
        public float AllSeeds { get; set; } = 0f;
    }
}
