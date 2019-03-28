namespace ConfigureMachineOutputs.Framework.Configs
{
    internal class SeedMakerConfig
    {
        public bool CustomSeedMakerEnabled { get; set; } = true;
        public bool MoreSeedsForQuality { get; set; } = true;
        public int SeedMakerInputMultiplier { get; set; } = 1;
        public int SeedMakerMinOutput { get; set; } = 2;
        public int SeedMakerMaxOutput { get; set; } = 5;
    }
}
