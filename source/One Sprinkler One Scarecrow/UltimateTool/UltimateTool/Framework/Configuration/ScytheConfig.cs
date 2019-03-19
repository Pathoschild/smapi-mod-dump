namespace UltimateTool.Framework.Configuration
{
   internal class ScytheConfig
    {
        public bool HarvestForage { get; set; } = true;
        public bool HarvestCrops { get; set; } = true;
        public bool HarvestFlowers { get; set; } = false;
        public bool HarvestFruit { get; set; } = true;
        public bool HarvestGrass { get; set; } = true;
        public bool CutDeadCrops { get; set; } = true;
        public bool CutWeeds { get; set; } = true;
    }
}
