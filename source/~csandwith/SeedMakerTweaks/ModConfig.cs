/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

namespace SeedMakerTweaks
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public float AncientSeedChance { get; set; } = 0.5f;
        public float MixedSeedChance { get; set; } = 2f;
        public int MinSeeds { get; set; } = 1;
        public int MaxSeeds { get; set; } = 4;
        public int MinMixedSeeds { get; set; } = 1;
        public int MaxMixedSeeds { get; set; } = 5;

    }
}
