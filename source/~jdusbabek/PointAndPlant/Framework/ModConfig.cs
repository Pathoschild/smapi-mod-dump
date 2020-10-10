/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jdusbabek/stardewvalley
**
*************************************************/

namespace PointAndPlant.Framework
{
    internal class ModConfig
    {
        public string PlowKey { get; set; } = "Z";
        public string PlantKey { get; set; } = "A";
        public string GrowKey { get; set; } = "S";
        public string HarvestKey { get; set; } = "D";

        public int PlantRadius { get; set; } = 4;
        public int GrowRadius { get; set; } = 4;
        public int HarvestRadius { get; set; } = 4;
        public int PlowWidth { get; set; } = 3;
        public int PlowHeight { get; set; } = 6;

        public bool PlowEnabled { get; set; } = true;
        public bool PlantEnabled { get; set; } = true;
        public bool GrowEnabled { get; set; } = true;
        public bool HarvestEnabled { get; set; } = true;

        public int Fertilizer { get; set; } = 369;

        public bool LoggingEnabled { get; set; }
    }
}
