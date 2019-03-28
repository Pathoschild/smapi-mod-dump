namespace ConfigureMachineOutputs.Framework.Configs
{
    internal class FurnaceConfig
    {
        public bool CustomFurnaceEnabled { get; set; } = true;
        public int FurnaceInputMultiplier { get; set; } = 1;
        public int FurnaceMinOutput { get; set; } = 1;
        public int FurnaceMaxOutput { get; set; } = 2;
    }
}
