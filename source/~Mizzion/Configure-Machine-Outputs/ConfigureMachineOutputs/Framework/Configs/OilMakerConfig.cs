namespace ConfigureMachineOutputs.Framework.Configs
{
    internal class OilMakerConfig
    {
        public bool CustomOilMakerEnabled { get; set; } = true;
        public int OilMakerInputMultiplier { get; set; } = 1;
        public int OilMakerMinOutput { get; set; } = 1;
        public int OilMakerMaxOutput { get; set; } = 2;
    }
}
