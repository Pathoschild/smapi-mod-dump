namespace ConfigureMachineOutputs.Framework.Configs
{
    internal class SlimeIncubatorConfig
    {
        public bool CustomSlimeIncubatorEnabled { get; set; } = true;
        //public int SlimeIncubatorInputMultiplier { get; set; } = 1;
        public int SlimeIncubatorMinOutput { get; set; } = 1;
        public int SlimeIncubatorMaxOutput { get; set; } = 2;
    }
}
