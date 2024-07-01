/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/chiccenFL/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;

namespace WaterYourCrops
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public bool OnlyWaterCan { get; set; } = true;
        public Color IndicatorColor { get; set; } = Color.LightPink;
        public float IndicatorOpacity { get; set; } = 1f;
        public bool Debug { get; set; } = false;

    }
}
