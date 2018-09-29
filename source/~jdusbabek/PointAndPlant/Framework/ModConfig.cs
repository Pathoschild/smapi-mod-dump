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
