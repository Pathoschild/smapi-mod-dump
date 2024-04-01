/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/


using StardewModdingAPI;

namespace CropVariation
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public bool EnableTrellisResize { get; set; } = true;
        public int ColorVariation { get; set; } = 40;
        public int SizeVariationPercent { get; set; } = 20;
        public int SizeVariationQualityFactor { get; set; } = 100;
        public int ColorVariationQualityFactor { get; set; } = 100;
    }
}
