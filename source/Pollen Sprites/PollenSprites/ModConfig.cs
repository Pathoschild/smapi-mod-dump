using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;

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
        public double MixedSeeds { get; set; } = 0.45;
        /// <summary>The chance that a random seed from <see cref="SeedManager.FlowerSeeds"/> will be dropped.</summary>
        public double FlowerSeeds { get; set; } = 0.10;
        /// <summary>The chance that a random seed from <see cref="SeedManager.AllSeeds"/> will be dropped.</summary>
        public double AllSeeds { get; set; } = 0.0;
    }
}
