namespace ConfigureMachineOutputs.Framework.Configs
{
    internal class SlimeEggPressConfig
    {
        public bool CustomSlimeEggPressEnabled { get; set; } = true;
        public int SlimeInputMultiplier { get; set; } = 1;
        public int SlimeMinOutput { get; set; } = 1;
        public int SlimeMaxOutput { get; set; } = 2;
    }
}
