/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jpparajeles/StardewValleyMods
**
*************************************************/

namespace WildFlowersReimagined
{
    public sealed class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public FlowerGrassConfig FlowerGrassConfig { get; set; } = new FlowerGrassConfig();
        public float WildflowerGrowChance { get; set; } = 0.005f;
        
    }

    public sealed class FlowerGrassConfig
    {
        public bool UseScythe { get; set; } = true;
    }
}
